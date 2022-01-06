using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using VisaNet.Application.Interfaces;
using VisaNet.Common.ApiClient;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.Debit;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;
using VisaNet.Utilities.Googl;
using VisaNet.Common.AzureUpload;

namespace VisaNet.Application.Implementations
{
    public class ServiceCustomerSite : IServiceCustomerSite
    {
        private readonly IServiceWebhookRegistration _serviceWebhookRegistration;
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly IServiceService _serviceService;
        private readonly IServiceSmsNotification _serviceSmsNotification;
        private readonly IServiceWsPaymentCancellation _serviceWsPaymentCancellation;
        private readonly IServicePayment _servicePayment;
        private readonly ITransactionContext _transactionContext;
        private readonly IServiceServiceValidator _serviceServiceValidator;

        private string _folderBlob = ConfigurationManager.AppSettings["AzureCommercesImagesUrl"];

        public ServiceCustomerSite(IServiceWebhookRegistration serviceWebhookRegistration, IServiceEmailMessage serviceEmailMessage,
            IServiceService serviceService, IServiceSmsNotification serviceSmsNotification, IServiceWsPaymentCancellation serviceWsPaymentCancellation,
            IServicePayment servicePayment, ITransactionContext transactionContext, IServiceServiceValidator serviceServiceValidator)
        {
            _serviceWebhookRegistration = serviceWebhookRegistration;
            _serviceEmailMessage = serviceEmailMessage;
            _serviceService = serviceService;
            _serviceSmsNotification = serviceSmsNotification;
            _serviceWsPaymentCancellation = serviceWsPaymentCancellation;
            _servicePayment = servicePayment;
            _transactionContext = transactionContext;
            _serviceServiceValidator = serviceServiceValidator;
        }

        public WebhookAccessTokenDto SendAccessTokenByMail(CustomerSiteGenerateAccessTokenDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
            {
                throw new BusinessException(CodeExceptions.USER_EMAIL_DUPLICATED);
            }
            WebhookAccessTokenDto result = null;
            if (!string.IsNullOrEmpty(dto.GeneratedUrl))
            {
                result = ResetTokentime(dto);
                SendAccessTokenByEmail(dto, dto.GeneratedUrl);
                UpdateAccessTokenStatus(dto);
            }
            else
            {
                result = Generate(dto);
                SendAccessTokenByEmail(dto, result.Url);
                UpdateAccessTokenStatus(dto);
            }

            return result;
        }

        public WebhookAccessTokenDto SendAccessTokenBySms(CustomerSiteGenerateAccessTokenDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.MobileNumber))
            {
                throw new BusinessException(CodeExceptions.USER_MOBILEPHONE_MISSING);
            }
            WebhookAccessTokenDto result = null;

            if (!string.IsNullOrEmpty(dto.GeneratedUrl))
            {
                result = ResetTokentime(dto);
                SendAccessTokenBySms(dto, dto.GeneratedUrl);
                UpdateAccessTokenStatus(dto);
            }
            else
            {
                result = Generate(dto);
                SendAccessTokenBySms(dto, result.Url);
                UpdateAccessTokenStatus(dto);
            }
            return result;
        }

        public WebhookAccessTokenDto SendAccessTokenByWhatsapp(CustomerSiteGenerateAccessTokenDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.MobileNumber))
            {
                throw new BusinessException(CodeExceptions.USER_MOBILEPHONE_MISSING);
            }
            WebhookAccessTokenDto result = null;

            if (!string.IsNullOrEmpty(dto.GeneratedUrl))
            {
                result = ResetTokentime(dto);
                UpdateAccessTokenStatus(dto);
            }
            else
            {
                result = Generate(dto);
                UpdateAccessTokenStatus(dto);
            }
            return result;
        }

        public WebhookAccessTokenDto GenerateAccessToken(CustomerSiteGenerateAccessTokenDto dto)
        {
            WebhookAccessTokenDto result = null;
            result = !string.IsNullOrEmpty(dto.GeneratedUrl) ? ResetTokentime(dto) : Generate(dto);
            return result;
        }

        private void UpdateAccessTokenStatus(CustomerSiteGenerateAccessTokenDto dto)
        {
            var webhook = _serviceWebhookRegistration.GetByIdOperation(dto.IdOperation, dto.ServiceId);
            _serviceWebhookRegistration.UpdateStatusAccessToken(webhook.Id, WebhookAccessState.Send);
        }

        public void ResetPasswordEmail(ResetPasswordEmailDto dto)
        {
            _serviceEmailMessage.SendCustomerSiteResetPasswordEmail(dto);
        }

        public void NewUserEmail(NewUserEmailDto dto)
        {
            _serviceEmailMessage.SendCustomerSiteNewUserEmail(dto);
        }

        private WebhookAccessTokenDto Generate(CustomerSiteGenerateAccessTokenDto dto)
        {
            //VALIDAR LA CONFIGURACION DEL SERVICIO
            var validService = _serviceServiceValidator.ValidateLinkService(dto.ServiceId);
            if (!validService)
            {
                return null;
            }

            //SI YA EXISTE UN REGISTRO CON ID OPERATION Y ID APP SOLO ACTUALIZO EL ACCESSTOKEN
            var webhook = _serviceWebhookRegistration.GetByIdOperation(dto.IdOperation, dto.ServiceId);

            if (webhook == null)
            {
                //TRANSFORM CustomerSiteGenerateAccessTokenDto TO WEEBHOOKREGISTRATION
                var webhookregistrationDto = GenerateWebhookRegistration(dto);

                //GUARDO EL WEEBHOOKREGISTRATION
                webhook = _serviceWebhookRegistration.Create(webhookregistrationDto, true);
            }

            //GUARDO EL ACCESSTOKEN
            var accessToken = webhook.WebhookAccessTokenDto != null ?
                _serviceWebhookRegistration.RegenerateToken(webhook.Id) :
                _serviceWebhookRegistration.GenerateAccessToken(webhook);

            //ACORTAR ACCESSTOKEN
            var accessTokenEncoded = HttpContext.Current.Server.UrlEncode(accessToken.AccessToken);
            var url = ShortenUrl(dto.AccessTokenBaseUrl + accessTokenEncoded);

            if (string.IsNullOrEmpty(url))
                return null;

            accessToken.Url = url;

            return accessToken;
        }

        private WebhookAccessTokenDto ResetTokentime(CustomerSiteGenerateAccessTokenDto dto)
        {
            var webhook = _serviceWebhookRegistration.GetByIdOperation(dto.IdOperation, dto.ServiceId);
            _serviceWebhookRegistration.ResetAccessToken(webhook.Id);
            webhook.WebhookAccessTokenDto.Url = dto.GeneratedUrl;

            var service = _serviceService.GetById(dto.ServiceId, x => x.ServiceContainer);
            dto.ServiceDto = service;

            return webhook.WebhookAccessTokenDto;
        }

        private string ShortenUrl(string url)
        {
            //GOOGLE SHORTEN URL
            var b = new GoogleShortenUrlApi();
            var shortUrl = b.Excecute(url);

            return shortUrl;
        }

        private void SendAccessTokenByEmail(CustomerSiteGenerateAccessTokenDto dto, string url)
        {
            _serviceEmailMessage.SendCustomerSiteBillEmail(dto, url);
        }

        private void SendAccessTokenBySms(CustomerSiteGenerateAccessTokenDto dto, string shortUrl)
        {
            _serviceSmsNotification.SendVonAccessSms(new SmsMessageVonAccessDto()
            {
                ShortUrl = shortUrl,
                PhoneNumber = dto.MobileNumber
            });
        }

        private WebhookRegistrationDto GenerateWebhookRegistration(CustomerSiteGenerateAccessTokenDto entity)
        {
            var service = _serviceService.GetById(entity.ServiceId, x => x.ServiceContainer);
            var idapp = service.ServiceContainerDto == null ? service.UrlName : service.ServiceContainerDto.UrlName;
            entity.ServiceDto = service;

            var webhookRegistrationDto = new WebhookRegistrationDto
            {
                IdApp = idapp,
                IdOperation = entity.IdOperation,
                UrlCallback = entity.UrlCallback,
                MerchantId = service.MerchantId,
                Bill = new BillDataInputDto()
                {
                    ExternalId = entity.BillExternalId,
                    Currency =
                        entity.BillCurrency.Equals("UYU", StringComparison.CurrentCultureIgnoreCase) ||
                        entity.BillCurrency.Equals("N", StringComparison.CurrentCultureIgnoreCase) ? "N" : "D",
                    Amount = entity.BillAmount.ToString("###0.00").Replace(",", "").Replace(".", ""),
                    TaxedAmount = entity.BillTaxedAmount.ToString("###0.00").Replace(",", "").Replace(".", ""),
                    Description = string.Empty,
                    FinalConsumer = entity.BillFinalConsumer ? "1" : "0",
                    GenerationDate = entity.BillGenerationDate.ToString("yyyyMMdd"),
                    Quota = entity.BillQuota ? "S" : "N",
                    ExpirationDate = entity.BillExpirationDate.ToString("yyyyMMdd")
                },
                UserData = new UserDataInputDto()
                {
                    Address = entity.Address,
                    Email = entity.Email,
                    IdentityNumber = entity.IdentityNumber,
                    MobileNumber = entity.MobileNumber,
                    Name = string.Empty,
                    Surname = string.Empty,
                    PhoneNumber = string.Empty,
                },
                EnableEmailChange = "S",
                EnableRememberUser = entity.EnableRememberUser,
                SendEmail = !string.IsNullOrEmpty(entity.SendEmail) && entity.SendEmail.Equals("S", StringComparison.InvariantCultureIgnoreCase)
            };

            return webhookRegistrationDto;
        }

        public bool CancelAccessToken(Guid accessTokenId)
        {
            return _serviceWebhookRegistration.CancelAccessToken(accessTokenId);
        }

        public TransactionResult CancelTansaction(CustomerSiteCancelTransactionDto dto)
        {
            try
            {
                //SI YA EXISTE UN REGISTRO CON ID OPERATION Y ID APP SOLO ACTUALIZO EL ACCESSTOKEN
                var webhook = _serviceWebhookRegistration.GetByIdOperation(dto.IdOperationTransaction, dto.IdService);

                var log = new WsPaymentCancellationDto()
                {
                    IdApp = webhook.IdApp,
                    IdOperation = dto.IdOperation,
                    IdOperacionCobro = dto.IdOperationTransaction,
                    Codresult = -1,
                    CreationDate = DateTime.Now
                };
                var updatedDto = _serviceWsPaymentCancellation.Create(log, true);


                var result = Cancel(updatedDto);

                return result;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }
            return new TransactionResult()
            {
                OperationResult = 1,
                CsTransactionNumber = string.Empty,
            };
        }

        private TransactionResult Cancel(WsPaymentCancellationDto data)
        {
            var resultData = new TransactionResult();
            PaymentDto payment = null;
            CyberSourceOperationData cyberSourceOperationData = null;
            var now = DateTime.Now;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration CancelPayment: idOperacion {0}, id app {1}", data.IdOperation, data.IdApp));

                //Si ya hay una operacion de cancelacion con resultado 0 no se hace nada
                var cancelationOp = _serviceWsPaymentCancellation.AllNoTracking(null, x => x.IdOperacionCobro.Equals(data.IdOperacionCobro)
                    && x.IdApp.Equals(data.IdApp)).OrderByDescending(x => x.CreationDate);
                if (cancelationOp.Any() && cancelationOp.Any(x => x.Codresult == 0))
                {
                    resultData.OperationResult = 62;
                    EditWsPaymentCancellationDto(data, resultData.OperationResult);
                    return resultData;
                }

                var operation = _serviceWebhookRegistration.GetByIdOperation(data.IdOperacionCobro, data.IdApp);
                if (operation == null)
                {
                    resultData.OperationResult = 60;
                    EditWsPaymentCancellationDto(data, resultData.OperationResult);
                    return resultData;
                }

                if (operation.PaymentId == null || operation.PaymentId == Guid.Empty || !operation.PaymentId.HasValue)
                {
                    resultData.OperationResult = 61;
                    EditWsPaymentCancellationDto(data, resultData.OperationResult);
                    return resultData;
                }

                payment = _servicePayment.GetById(operation.PaymentId.Value, x => x.Card, x => x.Bills, x => x.Service, x => x.RegisteredUser, x => x.AnonymousUser);
                if (payment.PaymentStatus != PaymentStatusDto.Done)
                {
                    resultData.OperationResult = 65;
                    EditWsPaymentCancellationDto(data, resultData.OperationResult);
                    return resultData;
                }

                var isRegisteredUser = payment.RegisteredUserId.HasValue;

                var cancel = new CancelPayment
                {
                    UserId = isRegisteredUser ? payment.RegisteredUserId.Value : payment.AnonymousUserId.Value,
                    UserEmail = isRegisteredUser ? payment.RegisteredUser.Email : payment.AnonymousUser.Email,
                    UserType = isRegisteredUser ? LogUserType.Registered : LogUserType.NoRegistered,
                    Token = isRegisteredUser ? payment.Card.PaymentToken : string.Empty,
                    RequestId = payment.TransactionNumber,
                    PaymentPlatform = PaymentPlatformDto.Apps,
                    IdTransaccion = Guid.NewGuid().ToString(),
                    Currency = payment.Currency,
                    Amount = payment.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                    ServiceId = payment.ServiceId,
                    ServiceDto = payment.ServiceDto
                };
                cyberSourceOperationData = _servicePayment.CancelPaymentCybersource(cancel);

                if (cyberSourceOperationData.VoidData != null)
                {
                    if (cyberSourceOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        data.Codresult = 0;
                        resultData.OperationResult = 0;
                        resultData.CsTransactionNumber = cyberSourceOperationData.VoidData.PaymentRequestId;
                        NotifyCustomer(payment, cyberSourceOperationData, now);
                    }
                    else
                    {
                        data.Codresult = cyberSourceOperationData.VoidData.PaymentResponseCode;
                        resultData.OperationResult = cyberSourceOperationData.RefundData.PaymentResponseCode;
                    }
                }
                if (cyberSourceOperationData.RefundData != null)
                {
                    if (cyberSourceOperationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        data.Codresult = 0;
                        resultData.OperationResult = 0;
                        resultData.CsTransactionNumber = cyberSourceOperationData.RefundData.PaymentRequestId;
                        NotifyCustomer(payment, cyberSourceOperationData, now);
                    }
                    else
                    {
                        data.Codresult = cyberSourceOperationData.RefundData.PaymentResponseCode;
                        resultData.OperationResult = cyberSourceOperationData.RefundData.PaymentResponseCode;
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration CancelPayment - Exception");
                NLogLogger.LogAppsEvent(exception);
                resultData.OperationResult = 1;
            }
            EditWsPaymentCancellationDto(data, resultData.OperationResult);
            return resultData;
        }

        private void EditWsPaymentCancellationDto(WsPaymentCancellationDto data, int codResult)
        {
            try
            {
                data.Codresult = codResult;
                _serviceWsPaymentCancellation.Edit(data);
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration CancelPayment LOG - Exception");
                NLogLogger.LogAppsEvent(e);
            }
        }

        private void NotifyCustomer(PaymentDto payment, CyberSourceOperationData operationData, DateTime now)
        {
            var service = payment.ServiceDto;
            ServiceDto serviceContainer = null;

            var isRegisteredUser = payment.RegisteredUser != null;
            var email = isRegisteredUser ? payment.RegisteredUser.Email : payment.AnonymousUser.Email;

            if (service.ServiceContainerId.HasValue)
            {
                serviceContainer = _serviceService.GetById(service.ServiceContainerId.Value);
            }

            var cancellationTransaccionId = operationData.RefundData != null && operationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted
                ? operationData.RefundData.PaymentRequestId
                : string.Empty;

            var cancellationAmount = payment.AmountTocybersource.ToString("##,#0.00", CultureInfo.CurrentCulture);

            var serviceName = serviceContainer != null
                   ? string.Format("{0} ({1})", serviceContainer.Name, service.Name)
                   : string.Format("{0}", service.Name);

            var dateString = payment.Date.ToString("dd/MM/yyyy hh:mm");
            var mailDesc =
                    string.Format(
                        "El comercio {0} realizo la cancelación de la transacción {1} realizada el {2}", serviceName, payment.TransactionNumber, dateString);

            //Email notification
            if (payment.PaymentPlatform == PaymentPlatformDto.Apps)
            {
                _serviceEmailMessage.SendPaymentDoneCancellation(email, mailDesc, payment.TransactionNumber,
                    dateString, payment.AmountTocybersource.ToString("##,#0.00", CultureInfo.CurrentCulture), cancellationTransaccionId,
                    now.ToString("dd/MM/yyyy hh:mm"), cancellationAmount);
            }
        }

        /// <summary>
        /// METODO A USAR POR PORTAL WEB VISANETPAGOS. SE GUARDA EN CATCHE JSON PARA NO OBTENER SIEMPRE DESDE PORTALA DE COMERCIOS
        /// </summary>
        /// <param name="productIds"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public IList<CustomerSiteCommerceDto> GetCommercesDebit(List<int> productIds = null, string search = null)
        {
            IList<CustomerSiteCommerceDto> finalList = null;

            try
            {
                var filePath = ConfigurationManager.AppSettings["CommercesList"];
                var tempList = new DebitCommercesDto();
                try
                {
                    using (var r = new StreamReader(filePath))
                    {
                        var jsonString = r.ReadToEnd();
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            tempList = ReadDebitCommercesJson(jsonString);

                            if (!string.IsNullOrEmpty(search))
                            {
                                search = search.ToLower();
                                tempList.Commerces = tempList.Commerces.Where(x => x.Name.ToLower().Contains(search)
                                    || x.ProductosListDto.Any(y => y.Description.ToLower().Contains(search))).ToList();
                            }
                            if (productIds != null && productIds.Any())
                            {
                                tempList.Commerces = tempList.Commerces.Where(x => x.ProductosListDto.Any(y => productIds.Contains(y.DebitProductid.Value))).ToList();
                            }

                            return tempList.Commerces;
                        }
                    }
                }
                catch (Exception)
                {
                    // si falla la lectura del archivo, se buscan desde customerSite en las siguientes lineas
                }

                var result = CustomerSiteApiClient<string>.GetdebitCommerces(MapperCommercesDebitJson, new CustomerSiteCommerceFilterDto(), _transactionContext);
                if (!string.IsNullOrEmpty(result))
                {
                    var list = ReadDebitCommercesJson(result);

                    // si la lista obtenida de la base viene vacia, se utiliza la del json
                    finalList = list.Commerces.Count == 0
                        ? tempList.Commerces
                        : list.Commerces;

                    // serialize JSON to a string and then write string to a file
                    File.WriteAllText(filePath, result);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                throw;
            }
            return finalList;
        }

        public CustomerSiteCommerceDto FindCommerceDebit(Guid id)
        {
            var list = GetCommercesDebit();
            var commerce = list.FirstOrDefault(x => x.Id == id);
            if (commerce != null)
            {
                commerce.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, commerce.Id, commerce.ImageName);
            }
            return commerce;
        }

        public void UpdateCommerceDebitCatche()
        {
            var filePath = ConfigurationManager.AppSettings["CommercesList"];
            File.Delete(filePath);
            GetCommercesDebit();
        }

        /// <summary>
        /// METODO A USAR POR ADMIN DE VISANETPAGOS
        /// </summary>
        /// <returns></returns>
        public IList<CustomerSiteCommerceDto> GetCommercesDebitFromCustomerSite(CustomerSiteCommerceFilterDto filterDto)
        {
            var result = CustomerSiteApiClient<DebitCommercesDto>.GetdebitCommerces(ReadDebitCommercesJson, filterDto, _transactionContext);

            var commerces = result.Commerces;

            foreach (var c in commerces)
            {
                c.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, c.Id, c.ImageName);
            }

            return commerces;
        }

        private string MapperCommercesDebitJson(string json)
        {
            return json;
        }
        private DebitCommercesDto ReadDebitCommercesJson(string json)
        {
            var content = JsonConvert.DeserializeObject<DebitCommercesDto>(json);
            return content;
        }
    }
}