using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServiceCyberSourceAcknowledgement : BaseService<CyberSourceAcknowledgement, CyberSourceAcknowledgementDto>, IServiceCyberSourceAcknowledgement
    {
        private readonly IServicePayment _paymentServicePayment;
        private readonly IServicePayment _servicePayment;
        private readonly IServiceApplicationUser _applicationUserService;
        private readonly IServiceAnonymousUser _anonymousService;
        private readonly IServiceService _serviceService;
        private readonly IRepositoryCyberSourceVoid _cyberSourceVoidRepository;
        private readonly ICsAckLoggerHelper _loggerHelper;

        public ServiceCyberSourceAcknowledgement(IServicePayment paymentServicePayment,
            IRepositoryCyberSourceAcknowledgement repository, IServicePayment servicePayment,
            IServiceApplicationUser applicationUserService, IServiceAnonymousUser anonymousService,
            IServiceService serviceService, IRepositoryCyberSourceVoid cyberSourceVoidRepository,
            ICsAckLoggerHelper loggerHelper)
            : base(repository)
        {
            _paymentServicePayment = paymentServicePayment;
            _servicePayment = servicePayment;
            _applicationUserService = applicationUserService;
            _anonymousService = anonymousService;
            _serviceService = serviceService;
            _cyberSourceVoidRepository = cyberSourceVoidRepository;
            _loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Process a CyberSource POST 
        /// </summary>
        /// <param name="post"></param>
        public void Process(CyberSourceAcknowledgementDto post)
        {
            var platfrom = GetHost(post.Platform);
            _loggerHelper.LogCSPostStarted(post, platfrom);

            try
            {
                if (IsPayment(post))
                {
                    var payments = _paymentServicePayment.AllNoTracking(null, x => x.TransactionNumber == post.TransactionId);
                    if (!payments.Any())
                    {
                        Create(post);
                        _loggerHelper.LogCSPostRecieved(post, platfrom);
                    }
                }
            }
            catch (Exception e)
            {
                _loggerHelper.LogCSPostException(e, platfrom);
            }
        }

        /// <summary>
        /// Voids payments which has not been removed from CyberSourceAcknowledgements
        /// </summary>
        public void VoidPayments()
        {
            var ackMarginMinutes = int.Parse(ConfigurationManager.AppSettings["CyberSourceAcknowledgementTime"]);
            var ackMarginDate = DateTime.Now.AddMinutes(-ackMarginMinutes);

            var cyberSourceAcknowledgements = All(null, x => x.DateTime <= ackMarginDate && x.Processed == false).ToList();

            _loggerHelper.LogTransactionIds(cyberSourceAcknowledgements);

            foreach (var acknowledgement in cyberSourceAcknowledgements)
            {
                //Controlo nuevamente que no exista el payment (por si llegó primero el Post de CS)
                var payments = _paymentServicePayment.AllNoTracking(null, x => x.TransactionNumber == acknowledgement.TransactionId);
                if (!payments.Any())
                {
                    if (IsPayment(acknowledgement))
                    {
                        try
                        {
                            var userId = Guid.Parse(acknowledgement.UserId);
                            var registeredUser = _applicationUserService.AllNoTracking(null, x => x.Id == userId).FirstOrDefault();
                            var anonymousUser = _anonymousService.AllNoTracking(null, x => x.Id == userId).FirstOrDefault();
                            acknowledgement.IsUserRegistered = (registeredUser != null);
                            var service = _serviceService.GetById(acknowledgement.ServiceId);

                            CyberSourceOperationData result = null;

                            //SI ES UN PAGO SE REALIZA UN VOID
                            if (acknowledgement.ReasonCode == (int) ErrorCodeDto.CYBERSOURCE_OK)
                            {
                                var cancel = new CancelPayment
                                {
                                    UserId = userId,
                                    Token = acknowledgement.PaymentToken,
                                    RequestId = acknowledgement.TransactionId,
                                    UserType = acknowledgement.IsUserRegistered ? LogUserType.Registered : LogUserType.NoRegistered,
                                    Amount = acknowledgement.ReqAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                    UserEmail = acknowledgement.IsUserRegistered ? registeredUser.Email : anonymousUser != null ? anonymousUser.Email : "",
                                    IdTransaccion = acknowledgement.TransactionId,
                                    Currency = acknowledgement.ReqCurrency,
                                    PaymentPlatform = acknowledgement.Platform,
                                    ServiceDto = service,
                                    ServiceId = acknowledgement.ServiceId
                                };

                                //Hago el Reverso
                                _loggerHelper.LogExecuteVoidStarted(cancel);
                               result = _servicePayment.CancelPaymentCybersource(cancel);    
                            }

                            //SI ES UN ERROR DE DM SE REALIZAO UN CREDIT CARD FULL AUTHORIZATION REVERSAL
                            if (acknowledgement.ReasonCode == (int)ErrorCodeDto.DECISIONMANAGER)
                            {
                                var reversal = new RefundPayment()
                                {
                                    UserId = userId,
                                    Token = acknowledgement.PaymentToken,
                                    RequestId = acknowledgement.TransactionId,
                                    UserType = acknowledgement.IsUserRegistered ? LogUserType.Registered : LogUserType.NoRegistered,
                                    Amount = acknowledgement.ReqAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                    IdTransaccion = acknowledgement.TransactionId,
                                    Currency = acknowledgement.ReqCurrency,
                                    PaymentPlatform = acknowledgement.Platform,
                                    ServiceDto = service,
                                    ServiceId = acknowledgement.ServiceId
                                };
                                result = _servicePayment.ReversePayment(reversal);
                            }
                            
                            if (result != null)
                            {
                                _loggerHelper.LogExecuteReversalResult(result, acknowledgement.TransactionId);
                                SaveVoidResult(acknowledgement, result);
                                SetCSAcknowledgementProcessed(acknowledgement.Id);
                            }
                            else
                            {
                                //Si devuelve null es porque ocurrió una excepcion
                                _loggerHelper.LogExecuteVoidException();
                            }
                        }
                        catch (Exception e)
                        {
                            _loggerHelper.LogExecuteVoidException(e);
                        }
                    }
                }
                else
                {
                    //Si existe el Payment (llegó antes el Post de CS) se borra el registro
                    DeleteCSAcknowledgement(acknowledgement.Id);
                }
            }
        }

        public override IQueryable<CyberSourceAcknowledgement> GetDataForTable()
        {
            throw new System.NotImplementedException();
        }

        public override CyberSourceAcknowledgementDto Converter(CyberSourceAcknowledgement entity)
        {
            return new CyberSourceAcknowledgementDto
            {
                Id = entity.Id,
                ReasonCode = entity.ReasonCode,
                TransactionId = entity.TransactionId,
                UserId = entity.UserId,
                Decision = entity.Decision,
                Message = entity.Message,
                BillTransRefNo = entity.BillTransRefNo,
                ReqCardNumber = entity.ReqCardNumber,
                ReqCardExpiryDate = entity.ReqCardExpiryDate,
                ReqProfileId = entity.ReqProfileId,
                ReqCardType = entity.ReqCardType,
                ReqPaymentMethod = entity.ReqPaymentMethod,
                ReqTransactionType = entity.ReqTransactionType,
                ReqTransactionUuid = entity.ReqTransactionUuid,
                ReqCurrency = entity.ReqCurrency,
                ReqReferenceNumber = entity.ReqReferenceNumber,
                ReqAmount = entity.ReqAmount,
                AuthAvsCode = entity.AuthAvsCode,
                AuthCode = entity.AuthCode,
                AuthAmount = entity.AuthAmount,
                AuthTime = entity.AuthTime,
                AuthResponse = entity.AuthResponse,
                AuthTransRefNo = entity.AuthTransRefNo,
                PaymentToken = entity.PaymentToken,
                DateTime = entity.DateTime,
                Platform = (PaymentPlatformDto)(int)entity.Platform,
                ServiceId = entity.ServiceId,
                OperationId = entity.OperationId,
                CardId = entity.CardId,
                Processed = entity.Processed
            };
        }

        public override CyberSourceAcknowledgement Converter(CyberSourceAcknowledgementDto entity)
        {
            return new CyberSourceAcknowledgement
            {
                Id = entity.Id != Guid.Empty ? entity.Id : Guid.NewGuid(),
                ReasonCode = entity.ReasonCode,
                TransactionId = entity.TransactionId,
                UserId = entity.UserId,
                Decision = entity.Decision,
                Message = entity.Message,
                BillTransRefNo = entity.BillTransRefNo,
                ReqCardNumber = entity.ReqCardNumber,
                ReqCardExpiryDate = entity.ReqCardExpiryDate,
                ReqProfileId = entity.ReqProfileId,
                ReqCardType = entity.ReqCardType,
                ReqPaymentMethod = entity.ReqPaymentMethod,
                ReqTransactionType = entity.ReqTransactionType,
                ReqTransactionUuid = entity.ReqTransactionUuid,
                ReqCurrency = entity.ReqCurrency,
                ReqReferenceNumber = entity.ReqReferenceNumber,
                ReqAmount = entity.ReqAmount,
                AuthAvsCode = entity.AuthAvsCode,
                AuthCode = entity.AuthCode,
                AuthAmount = entity.AuthAmount,
                AuthTime = entity.AuthTime,
                AuthResponse = entity.AuthResponse,
                AuthTransRefNo = entity.AuthTransRefNo,
                PaymentToken = entity.PaymentToken,
                DateTime = entity.DateTime,
                Platform = (PaymentPlatform)(int)entity.Platform,
                ServiceId = entity.ServiceId,
                OperationId = entity.OperationId,
                CardId = entity.CardId,
                Processed = entity.Processed
            };
        }

        private CyberSourceVoid Converter(CyberSourceVoidDto entity)
        {
            return new CyberSourceVoid
            {
                Id = entity.Id != Guid.Empty ? entity.Id : Guid.NewGuid(),
                TransactionId = entity.TransactionId,
                DateTime = entity.DateTime,
                VoidNumber = entity.VoidNumber,
                VoidCode = entity.VoidCode,
                ReverseNumber = entity.ReverseNumber,
                ReverseCode = entity.ReverseCode,
                RefundNumber = entity.RefundNumber,
                RefundCode = entity.RefundCode,
                CyberSourceAcknowledgementId = entity.CyberSourceAcknowledgementId
            };
        }

        /// <summary>
        /// Returns true if it is a successful payement
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        private static bool IsPayment(CyberSourceAcknowledgementDto post)
        {
            return post.ReqTransactionType.ToUpper() == "SALE" && (post.ReasonCode == (int)ErrorCodeDto.CYBERSOURCE_OK || post.ReasonCode == (int)ErrorCodeDto.DECISIONMANAGER);
        }

        /// <summary>
        /// Dada la plataforma por la que se generó el pago en CS, retorna la plataforma a loguear
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        private LogPlatform GetHost(PaymentPlatformDto platform)
        {
            switch (platform)
            {
                case PaymentPlatformDto.VisaNet:
                    return LogPlatform.VisaNet;
                case PaymentPlatformDto.Itau:
                    return LogPlatform.Itau;
                case PaymentPlatformDto.Apps:
                    return LogPlatform.Apps;
                case PaymentPlatformDto.Mobile:
                    return LogPlatform.Mobile;
                default:
                    return LogPlatform.Default;
            }
        }

        /// <summary>
        /// Se persiste un registro con el resultado del void
        /// </summary>
        /// <param name="acknowledgement"></param>
        /// <param name="result"></param>
        private void SaveVoidResult(CyberSourceAcknowledgementDto acknowledgement, CyberSourceOperationData result)
        {
            string voidNumber = null;
            int? voidCode = null;
            string reverseNumber = null;
            int? reverseCode = null;
            string refundNumber = null;
            int? refundCode = null;

            if (result != null)
            {
                voidNumber = result.VoidData != null ? result.VoidData.PaymentRequestId : null;
                voidCode = result.VoidData != null ? (int?)result.VoidData.PaymentResponseCode : null;
                reverseNumber = result.ReversalData != null ? result.ReversalData.PaymentRequestId : null;
                reverseCode = result.ReversalData != null ? (int?)result.ReversalData.PaymentResponseCode : null;
                refundNumber = result.RefundData != null ? result.RefundData.PaymentRequestId : null;
                refundCode = result.RefundData != null ? (int?)result.RefundData.PaymentResponseCode : null;
            }

            var cyberSourceVoid = new CyberSourceVoidDto
            {
                Id = Guid.NewGuid(),
                TransactionId = acknowledgement.TransactionId,
                DateTime = DateTime.Now,
                VoidNumber = voidNumber,
                VoidCode = voidCode,
                ReverseNumber = reverseNumber,
                ReverseCode = reverseCode,
                RefundNumber = refundNumber,
                RefundCode = refundCode,
                CyberSourceAcknowledgementId = acknowledgement.Id,
            };

            _cyberSourceVoidRepository.Create(Converter(cyberSourceVoid));
            _cyberSourceVoidRepository.Save();
        }

        private void SetCSAcknowledgementProcessed(Guid acknowledgementId)
        {
            Repository.ContextTrackChanges = true;
            var acknowledgement = Repository.GetById(acknowledgementId);
            acknowledgement.Processed = true;
            Repository.Edit(acknowledgement);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        private void DeleteCSAcknowledgement(Guid acknowledgementId)
        {
            Delete(acknowledgementId);
        }

    }
}