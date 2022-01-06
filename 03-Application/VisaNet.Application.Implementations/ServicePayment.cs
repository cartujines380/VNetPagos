using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ComplexTypes;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServicePayment : BaseService<Payment, PaymentDto>, IServicePayment
    {
        private readonly IServiceCard _serviceCard;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceService _serviceService;
        private readonly IServiceBill _serviceBill;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly ILoggerService _loggerService;
        private readonly ICyberSourceAccess _cyberSourceAccess;
        private readonly IServicePaymentIdentifier _paymentIdentifier;
        private readonly IServiceQuotation _serviceQuotation;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceNotification _serviceNotification;
        private readonly IServiceAnalyzeCsCall _serviceAnalyzeCsCall;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IRepositoryParameters _repositoryParameters;
        private readonly IServicePaymentTicket _servicePaymentTicket;
        private readonly IServiceExternalNotification _serviceExternalNotification;

        private string _serviceFolderBlob = ConfigurationManager.AppSettings["AzureServicesImagesUrl"];

        public ServicePayment(IRepositoryPayment repository, IServiceCard serviceCard, IServiceBill serviceBill, IServiceApplicationUser serviceApplicationUser, IServiceAnonymousUser serviceAnonymousUser,
                                ServiceServiceAssosiate serviceServiceAssosiate, IServiceService serviceService, IServiceBin serviceBin, ILoggerService loggerService, ICyberSourceAccess cyberSourceAccess, IServicePaymentIdentifier paymentIdentifier,
                                IServiceQuotation serviceQuotation, IServiceFixedNotification serviceFixedNotification, IServiceNotification serviceNotification, IServiceAnalyzeCsCall serviceAnalyzeCsCall,
                                IServiceEmailMessage serviceNotificationMessage, IRepositoryParameters repositoryParameters, IServicePaymentTicket servicePaymentTicket, IServiceExternalNotification serviceExternalNotification)
            : base(repository)
        {
            _repositoryPayment = repository;
            _serviceCard = serviceCard;
            _serviceBill = serviceBill;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceAnonymousUser = serviceAnonymousUser;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _serviceService = serviceService;
            _serviceBin = serviceBin;
            _loggerService = loggerService;
            _cyberSourceAccess = cyberSourceAccess;
            _paymentIdentifier = paymentIdentifier;
            _serviceQuotation = serviceQuotation;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceNotification = serviceNotification;
            _serviceAnalyzeCsCall = serviceAnalyzeCsCall;
            _serviceNotificationMessage = serviceNotificationMessage;
            _repositoryParameters = repositoryParameters;
            _servicePaymentTicket = servicePaymentTicket;
            _serviceExternalNotification = serviceExternalNotification;
        }

        public override IQueryable<Payment> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override PaymentDto Converter(Payment entity)
        {
            if (entity == null) return null;

            return new PaymentDto
            {
                Id = entity.Id,
                Date = entity.Date,
                CardId = entity.CardId,
                Card = entity.Card == null ? null : _serviceCard.Converter(entity.Card),
                ServiceId = entity.ServiceId,
                ServiceDto = entity.Service != null ? new ServiceDto()
                {
                    Id = entity.ServiceId,
                    ReferenceParamName = entity.Service.ReferenceParamName,
                    ReferenceParamName2 = entity.Service.ReferenceParamName2,
                    ReferenceParamName3 = entity.Service.ReferenceParamName3,
                    ReferenceParamName4 = entity.Service.ReferenceParamName4,
                    ReferenceParamName5 = entity.Service.ReferenceParamName5,
                    ReferenceParamName6 = entity.Service.ReferenceParamName6,
                    Name = entity.Service.Name,
                    ServiceCategoryId = entity.Service.ServiceCategoryId,
                    ServiceCategory = entity.Service.ServiceCategory != null ? new ServiceCategoryDto
                    {
                        Id = entity.Service.ServiceCategory.Id,
                        Name = entity.Service.ServiceCategory.Name
                    } : null,
                    MerchantId = entity.Service.MerchantId,
                    CybersourceTransactionKey = entity.Service.CybersourceTransactionKey,
                    ServiceContainerDto = entity.Service.ServiceContainer != null ? new ServiceDto()
                    {
                        Name = entity.Service.ServiceContainer.Name
                    } : null,
                } : null,

                ServiceAssociatedId = entity.ServiceAssosiatedId,
                ServiceAssociatedDto = entity.ServiceAssosiated == null ? null : _serviceServiceAssosiate.Converter(entity.ServiceAssosiated),
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                Description = entity.Description,
                Bills = entity.Bills.Select(b => _serviceBill.Converter(b)).ToList(),
                RegisteredUserId = entity.RegisteredUserId,
                RegisteredUser = entity.RegisteredUser == null ? null : _serviceApplicationUser.Converter(entity.RegisteredUser),
                AnonymousUserId = entity.AnonymousUserId,
                AnonymousUser = entity.AnonymousUser == null ? null : _serviceAnonymousUser.Converter(entity.AnonymousUser),
                PaymentType = (PaymentTypeDto)(int)entity.PaymentType,
                TransactionNumber = entity.TransactionNumber,
                GatewayId = entity.GatewayId,
                Currency = entity.Currency,
                TotalAmount = entity.TotalAmount,
                TotalTaxedAmount = entity.TotalTaxedAmount,
                Discount = entity.Discount,
                DiscountApplyed = entity.DiscountApplyed,
                PaymentIdentifierDto = entity.PaymentIdentifier != null ? new PaymentIdentifierDto()
                {
                    CyberSourceTransactionIdentifier = entity.PaymentIdentifier.CyberSourceTransactionIdentifier,
                    UniqueIdentifier = entity.PaymentIdentifier.UniqueIdentifier
                } : null,
                PaymentIdentifierId = entity.PaymentIdentifierId,
                AmountTocybersource = entity.AmountTocybersource,
                PaymentPlatform = (PaymentPlatformDto)(int)entity.PaymentPlatform,
                GatewayDto = new GatewayDto()
                {
                    Name = entity.Gateway != null ? entity.Gateway.Name : "",
                    Enum = entity.Gateway != null ? entity.Gateway.Enum : -1,
                    Id = entity.GatewayId
                },

                DiscountObjId = entity.DiscountObjId,
                DiscountObj = entity.DiscountObj != null ? new DiscountDto
                {
                    Id = entity.DiscountObj.Id,
                    From = entity.DiscountObj.From,
                    To = entity.DiscountObj.To,
                    DiscountType = (DiscountTypeDto)(int)entity.DiscountObj.DiscountType,
                    Fixed = entity.DiscountObj.Fixed,
                    Additional = entity.DiscountObj.Additional,
                    MaximumAmount = entity.DiscountObj.MaximumAmount,
                    CardType = (CardTypeDto)(int)entity.DiscountObj.CardType,
                    DiscountLabel = (DiscountLabelTypeDto)(int)entity.DiscountObj.DiscountLabel
                } : null,
                PaymentStatus = (PaymentStatusDto)(int)entity.PaymentStatus,
                Quotas = entity.Quotas,
            };
        }

        public override Payment Converter(PaymentDto entity)
        {
            if (entity == null) return null;

            var payment = new Payment
            {
                Id = entity.Id,
                Date = entity.Date,
                CardId = entity.CardId,
                ServiceId = entity.ServiceId,
                ServiceAssosiatedId = entity.ServiceAssociatedId,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                Description = entity.Description,
                RegisteredUserId = entity.RegisteredUserId,
                AnonymousUserId = entity.AnonymousUserId,
                PaymentType = (PaymentType)(int)entity.PaymentType,
                TransactionNumber = entity.TransactionNumber,
                Currency = entity.Currency,
                TotalAmount = entity.TotalAmount,
                TotalTaxedAmount = entity.TotalTaxedAmount,
                Discount = entity.Discount,
                DiscountApplyed = entity.DiscountApplyed,
                AmountTocybersource = entity.AmountTocybersource,

                DiscountObjId = entity.DiscountObjId == Guid.Empty ? null : entity.DiscountObjId,

                CyberSourceData = new CyberSourceData
                {
                    Decision = entity.CyberSourceData.Decision,
                    ReasonCode = entity.CyberSourceData.ReasonCode,
                    BillTransRefNo = entity.CyberSourceData.BillTransRefNo,
                    Message = entity.CyberSourceData.Message,
                    PaymentToken = entity.CyberSourceData.PaymentToken,
                    TransactionId = entity.CyberSourceData.TransactionId,
                    AuthAmount = entity.CyberSourceData.AuthAmount,
                    AuthAvsCode = entity.CyberSourceData.AuthAvsCode,
                    AuthCode = entity.CyberSourceData.AuthCode,
                    AuthResponse = entity.CyberSourceData.AuthResponse,
                    AuthTime = entity.CyberSourceData.AuthTime,
                    AuthTransRefNo = entity.CyberSourceData.AuthTransRefNo,

                    ReqAmount = entity.CyberSourceData.ReqAmount,
                    ReqCardExpiryDate = entity.CyberSourceData.ReqCardExpiryDate,
                    ReqCardNumber = entity.CyberSourceData.ReqCardNumber,
                    ReqCardType = entity.CyberSourceData.ReqCardType,
                    ReqCurrency = entity.CyberSourceData.ReqCurrency,
                    ReqPaymentMethod = entity.CyberSourceData.ReqPaymentMethod,
                    ReqProfileId = entity.CyberSourceData.ReqProfileId,
                    ReqReferenceNumber = entity.CyberSourceData.ReqReferenceNumber,
                    ReqTransactionType = entity.CyberSourceData.ReqTransactionType,
                    ReqTransactionUuid = entity.CyberSourceData.ReqTransactionUuid,
                },
                VerifyByVisaData = new VerifyByVisaData
                {
                    PayerAuthenticationEci = entity.VerifyByVisaData.PayerAuthenticationEci,
                    PayerAuthenticationXid = entity.VerifyByVisaData.PayerAuthenticationXid,
                    PayerAuthenticationCavv = entity.VerifyByVisaData.PayerAuthenticationCavv,
                    PayerAuthenticationProofXml = entity.VerifyByVisaData.PayerAuthenticationProofXml,
                },
                GatewayId = entity.GatewayId,
                PaymentIdentifierId = entity.PaymentIdentifierId,
                PaymentPlatform = (PaymentPlatform)(int)entity.PaymentPlatform,
                PaymentStatus = (PaymentStatus)(int)entity.PaymentStatus,
                Quotas = entity.Quotas == 0 ? 1 : entity.Quotas,
            };


            return payment;
        }

        public override PaymentDto Create(PaymentDto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            var paymentId = Guid.Empty;

            //FIX RAPIDO POR MONTO DESCUENTO MAL
            entity.Discount = entity.Bills.FirstOrDefault().DiscountAmount;

            var efEntity = Converter(entity);

            if (efEntity.CardId == Guid.Empty)
            {
                efEntity.Card = new Card()
                {
                    CybersourceTransactionId = entity.Card.CybersourceTransactionId,
                    DueDate = entity.Card.DueDate,
                    MaskedNumber = entity.Card.MaskedNumber,
                    PaymentToken = entity.Card.PaymentToken,
                    Name = entity.Card.Name,
                    Description = entity.Card.Description
                };
                efEntity.Card.GenerateNewIdentity();
            }
            efEntity.GenerateNewIdentity();

            Repository.Create(efEntity);
            Repository.Save();

            paymentId = efEntity.Id;

            _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                entity.RegisteredUserId.HasValue ? LogUserType.Registered : entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Other,
                entity.RegisteredUserId.HasValue ? entity.RegisteredUserId.Value : entity.AnonymousUserId.HasValue ? entity.AnonymousUserId.Value : Guid.Empty,
                string.Format(LogStrings.Payment_Entity_Cretion, entity.TransactionNumber),
                string.Format(LogStrings.CallCenter_Payment_Success, entity.TransactionNumber));

            //Si no puedo guardar la factura, no tengo que cancelar.

            foreach (var bill in entity.Bills)
            {
                try
                {
                    bill.PaymentId = efEntity.Id;

                    if ((int)entity.GatewayEnum == (int)GatewayEnum.Sucive)
                    {
                        bill.ExpirationDate = DateTime.Now;
                    }

                    if (bill.ExpirationDate == null || bill.ExpirationDate == DateTime.MinValue)
                    {
                        //Va a generar un error.
                        bill.ExpirationDate = DateTime.Today;
                        NLogLogger.LogEvent(NLogType.Error, "Error en fecha a la hora de guardar. Fecha " + bill.ExpirationDate);
                    }

                    //Se intenta persistir la Bill hasta 3 intentos
                    var billCreateRetries = 0;
                    var createBillSuccessful = false;
                    while (!createBillSuccessful)
                    {
                        try
                        {
                            _serviceBill.Create(bill);
                            createBillSuccessful = true;
                        }
                        catch (Exception e)
                        {
                            billCreateRetries++;
                            NLogLogger.LogEvent(e);
                            NLogLogger.LogEvent(NLogType.Info, "Error al persistir la Bill de PaymentId: " + paymentId + " intento " + billCreateRetries);

                            if (billCreateRetries == 3)
                            {
                                //Se tira la excepcion para que la agarre el catch original
                                throw e;
                            }
                        }
                    }

                    _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                        entity.RegisteredUserId.HasValue ? LogUserType.Registered : entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Other,
                        entity.RegisteredUserId.HasValue ? entity.RegisteredUserId.Value : entity.AnonymousUserId.HasValue ? entity.AnonymousUserId.Value : Guid.Empty,
                        string.Format(LogStrings.Payment_Bill_Entity_Cretion, bill.BillExternalId, entity.TransactionNumber),
                        string.Format(LogStrings.CallCenter_Payment_Bill_Success, bill.BillExternalId, entity.TransactionNumber));

                }
                catch (Exception e)
                {
                    _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                        entity.RegisteredUserId.HasValue ? LogUserType.Registered : entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Other,
                        entity.RegisteredUserId.HasValue ? entity.RegisteredUserId.Value : entity.AnonymousUserId.HasValue ? entity.AnonymousUserId.Value : Guid.Empty,
                        string.Format(LogStrings.Bill_Save_Error, entity.TransactionNumber),
                        string.Format(LogStrings.Bill_Save_Error_Callcenter));

                    NLogLogger.LogEvent(NLogType.Error, "Error al querer persisitr la factura");
                    NLogLogger.LogEvent(e);
                    var msg =
                        String.Format(
                            "Trns: {0}, Factura: BillExternalId {1}, SucivePreBillNumber {2}, ExpirationDate {3}",
                            entity != null ? entity.TransactionNumber : "", bill == null ? "" : bill.BillExternalId,
                            bill == null ? "" : bill.SucivePreBillNumber,
                            bill == null ? DateTime.Now : bill.ExpirationDate);
                    NLogLogger.LogEvent(NLogType.Error, msg);

                    var delete = paymentId != Guid.Empty && entity.GatewayEnum != GatewayEnumDto.Banred &&
                                 entity.GatewayEnum != GatewayEnumDto.Sistarbanc
                                 && entity.GatewayEnum != GatewayEnumDto.Sucive &&
                                 entity.GatewayEnum != GatewayEnumDto.Geocom;

                    #region Notification Fix
                    var notiDesc = string.Format("Error en Pago Factura - Persistir factura.");
                    var notiDetail = string.Format("Ha ocurrido un error al intentar persistir una factura en BD. El pago si quedo registrado.<br />" +
                                                   "Se necesita agregar la factura a mano en BD para poder ver la transacción en el portal y ser procesada en el TC33.<br /><br />" +
                                                   "Los datos de la consulta son: <br/> " +
                                                   "Nro transaccion en CS: {0} <br />" +
                                                   "Datos de la factura:<br />" +
                                                   "    Payment Id: {1}<br />" +
                                                   "    BillExternalId: {2}<br />" +
                                                   "    Currency: {3}<br />" +
                                                   "    Amount: {4}<br />" +
                                                   "    Discount: {5}<br />" +
                                                   "    DiscountAmount: {6}<br />" +
                                                   "    TaxedAmount: {7}<br />" +
                                                   "    FinalConsumer: {8}<br />" +
                                                   "    ExpirationDate: {9}<br />" +
                                                   "    GatewayTransactionBrouId: {10}<br />" +
                                                   "    GatewayTransactionId: {11}<br />" +
                                                   "    SucivePreBillNumber: {12}<br />" +
                                                   "    Gateway: {13}<br />" +
                                                   "    Description: {14}<br /><br />" +
                                                   "{15}<br /><br />", entity.TransactionNumber, bill.PaymentId, bill.BillExternalId, bill.Currency, bill.Amount,
                                                   bill.Discount, bill.DiscountAmount, bill.TaxedAmount, bill.FinalConsumer, bill.ExpirationDate,
                                                   bill.GatewayTransactionBrouId, bill.GatewayTransactionId, bill.SucivePreBillNumber, bill.Gateway, bill.Description,
                                                   _serviceFixedNotification.ExceptionMsg(e));

                    if (delete)
                    {
                        notiDetail = notiDetail + string.Format("Por la pasarela({0}) es posible eliminar la transacción de la BD. Chequear que se pudo realizar.<br />" +
                                                                "De tener que notificar a un servicio externo no es posible eliminar la transacción.", bill.Gateway);
                    }
                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.PaymentError,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Error,
                        Description = notiDesc,
                        Detail = notiDetail
                    });
                    #endregion


                    //SI EXISTE UN PAYMENT, NO PUDE GUARDAR LA FACTURA Y NO NOTIFIQUE AL ENTE, ELIMINO EL PAYMENT
                    if (delete)
                    {
                        NLogLogger.LogEvent(NLogType.Error, string.Format("Como la pasarela({0}) lo permite, elimino el pago de BD.", entity.GatewayEnum));
                        Delete(paymentId);

                        _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                        entity.RegisteredUserId.HasValue ? LogUserType.Registered : entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Other,
                        entity.RegisteredUserId.HasValue ? entity.RegisteredUserId.Value : entity.AnonymousUserId.HasValue ? entity.AnonymousUserId.Value : Guid.Empty,
                        string.Format(LogStrings.Bill_Save_Error_Deleted, entity.TransactionNumber),
                        string.Format(LogStrings.Bill_Save_Error_Callcenter_Deleted));
                    }
                    else
                    {
                        #region email_ visa

                        var parameters = _repositoryParameters.AllNoTracking().First();
                        var title = "VISANETPAGOS - ERROR INTERNO - CRITICO";
                        var emailMsg = "Error al intentar persistir una factura. El comercio fue notificado, el pago fue persitido pero no tiene factura asociada. " + msg;
                        var exceptionMessage = e != null ? e.Message : "";
                        var stackTrace = e != null ? e.StackTrace : "";
                        var innerException = e != null ? e.InnerException : null;

                        var user = new
                        {
                            Name = entity.RegisteredUser != null ? entity.RegisteredUser.Name : (entity.AnonymousUser != null ? entity.AnonymousUser.Name : ""),
                            Surname = entity.RegisteredUser != null ? entity.RegisteredUser.Surname : (entity.AnonymousUser != null ? entity.AnonymousUser.Surname : ""),
                            Email = entity.RegisteredUser != null ? entity.RegisteredUser.Email : (entity.AnonymousUser != null ? entity.AnonymousUser.Email : ""),
                        };
                        _serviceNotificationMessage.SendInternalErrorNotification(parameters, title, user, emailMsg, exceptionMessage, stackTrace, innerException);
                        #endregion
                    }
                    return null;
                }
            }

            //SI SE CAE LA NOTIFICACION, NO ME PERJUDICA EL PAGO !!
            try
            {
                #region Mail
                ServiceAssociatedDto serviceAssociatedDto = null;

                if (entity.PaymentPlatform == PaymentPlatformDto.VisaNet || entity.PaymentPlatform == PaymentPlatformDto.Mobile) //entity.PaymentPlatform == PaymentPlatformDto.Apps ||
                {
                    //Obtengo el nombre y descripcion del servicio para el asunto y cuerpo del mail
                    var serviceNameAndDescription = string.Empty;
                    if (entity.ServiceDto != null && !string.IsNullOrEmpty(entity.ServiceDto.Name))
                    {
                        serviceNameAndDescription = entity.ServiceDto.Name;
                    }

                    if (entity.ServiceAssociatedId != null && entity.ServiceAssociatedId != Guid.Empty)
                    {
                        serviceAssociatedDto = _serviceServiceAssosiate.GetById((Guid)entity.ServiceAssociatedId, x => x.NotificationConfig);
                        if (!string.IsNullOrEmpty(serviceAssociatedDto.Description))
                        {
                            serviceNameAndDescription += " - " + serviceAssociatedDto.Description;
                        }
                    }

                    //Si es pago de servicio no asociado, o es servicio asociado y tiene activada la notificacion de pago -> notifico
                    if ((serviceAssociatedDto == null) ||
                        (serviceAssociatedDto.NotificationConfigDto != null &&
                        serviceAssociatedDto.NotificationConfigDto.SuccessPaymentDto != null &&
                        serviceAssociatedDto.NotificationConfigDto.SuccessPaymentDto.Email))
                    {
                        byte[] arrBytes;
                        string mimeType;

                        var userId = entity.RegisteredUserId.HasValue ? entity.RegisteredUserId : entity.AnonymousUserId;
                        GeneratePaymentTicket(efEntity.Id, efEntity.TransactionNumber, userId.Value, out arrBytes, out mimeType);

                        _serviceNotificationMessage.SendNewPayment(entity.AnonymousUserId != null, efEntity.TransactionNumber, entity.AnonymousUser, entity.RegisteredUser, serviceNameAndDescription, arrBytes, mimeType);

                    }
                }
                #endregion

                #region AppNotification

                if ((entity.PaymentPlatform == PaymentPlatformDto.VisaNet || entity.PaymentPlatform == PaymentPlatformDto.Mobile) //|| entity.PaymentPlatform == PaymentPlatformDto.Apps
                    && entity.RegisteredUserId.HasValue)
                {
                    _serviceNotification.Create(new NotificationDto
                    {
                        Date = DateTime.Now,
                        Message = EnumHelpers.GetName(typeof(EmailType), (int)EmailType.NewPayment, EnumsStrings.ResourceManager),
                        NotificationPrupose = NotificationPruposeDto.SueccessNotification,
                        RegisteredUserId = entity.RegisteredUserId.Value,
                        ServiceId = entity.ServiceId
                    });
                }
                #endregion
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }

            Repository.ContextTrackChanges = false;

            //traigo de BD el objeto, sino el converter solo tiene los ids
            return returnEntity ? GetById(efEntity.Id, p => p.ServiceAssosiated, p => p.Service, p => p.AnonymousUser, p => p.RegisteredUser, p => p.Card, p => p.Bills, p => p.PaymentIdentifier, p => p.DiscountObj) : null;
        }

        public CybersourceCreatePaymentDto NotifyGateways(PaymentDto entity)
        {
            ServiceDto service = null;
            var userId = new Guid();
            var paymentDone = false;
            var result = new CybersourceCreatePaymentDto();
            try
            {
                string name = "";
                string surname = "";
                var isRegistredUser = false;
                if (entity.AnonymousUserId != null) //el usuario anonimo ya se encuentra creado
                {
                    var user = _serviceAnonymousUser.GetById(entity.AnonymousUserId.Value);
                    entity.AnonymousUser = user;
                    userId = user.Id;
                    name = user.Name;
                    surname = user.Surname;
                }
                else if (entity.RegisteredUserId != null /*&& entity.PaymentType != PaymentTypeDto.Automatic*/)
                {
                    var applicationUser = _serviceApplicationUser.GetById(entity.RegisteredUserId.Value);
                    entity.RegisteredUser = applicationUser;
                    userId = entity.RegisteredUserId.Value;
                    name = applicationUser == null || string.IsNullOrEmpty(applicationUser.Name) ? "" : applicationUser.Name;
                    surname = applicationUser == null || string.IsNullOrEmpty(applicationUser.Surname) ? "" : applicationUser.Surname;
                    isRegistredUser = true;

                    //CREO LA TARJETA SI NO EXISTE
                    var createdCard = CreateNewCard(entity.Card, userId);
                    entity.Card = createdCard;
                }
                if (String.IsNullOrEmpty(name) && entity.ServiceAssociatedDto != null)
                {
                    userId = entity.ServiceAssociatedDto.UserId;
                    name = entity.ServiceAssociatedDto.RegisteredUserDto != null
                        ? entity.ServiceAssociatedDto.RegisteredUserDto.Name
                        : "";
                    surname = entity.ServiceAssociatedDto.RegisteredUserDto != null
                        ? entity.ServiceAssociatedDto.RegisteredUserDto.Surname
                        : "";
                }

                var bill = entity.Bills.FirstOrDefault();

                service =
                    _serviceService.All(null, s => s.Id == entity.ServiceId, s => s.ServiceGateways,
                        s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

                if (service == null)
                    throw new ProviderWithoutConectionException("servicio null");
                if (service.ServiceGatewaysDto == null || !service.ServiceGatewaysDto.Any())
                    throw new ProviderWithoutConectionException("servicio sin pasarelas o nullas");
                if (service.ServiceGatewaysDto.First().Gateway == null || !service.ServiceGatewaysDto.Any())
                    throw new ProviderWithoutConectionException("servicio con pasarelas internas nulas");

                var serviceGateway = bill.GatewayId != null && bill.GatewayId != Guid.Empty
                    ? service.ServiceGatewaysDto.FirstOrDefault(g => g.Active && ((g.Gateway.Id == bill.GatewayId)))
                    : service.ServiceGatewaysDto.FirstOrDefault(g => g.Active && ((g.Gateway.Enum == (int)bill.Gateway)));


                //TENGO QUE GENERAR UN ID PARA EL PAYMENT PARTICULAR PARA SISTARBANC
                var pIdenfifierDto = _paymentIdentifier.Create(
                        new PaymentIdentifierDto { CyberSourceTransactionIdentifier = entity.TransactionNumber }, true);
                entity.PaymentIdentifierId = pIdenfifierDto.Id;
                entity.PaymentIdentifierDto = pIdenfifierDto;

                entity.Card = entity.Card ?? _serviceCard.GetById(entity.CardId);


                var binNumber = int.Parse(entity.Card.MaskedNumber.Substring(0, 6));
                var bin = _serviceBin.Find(binNumber) ?? _serviceBin.GetDefaultBin();
                foreach (var billDto in entity.Bills)
                {
                    billDto.GatewayTransactionId = bin.BankDto != null && bin.BankDto.Name.Equals("BROU", StringComparison.InvariantCultureIgnoreCase) ? billDto.GatewayTransactionBrouId : billDto.GatewayTransactionId;
                }

                //SI HAY UN ERROR AL MARCAR EL PAGO EN UNA PASARELA, SE DEBE CANCELAR EN CYBERSOURCE
                var notify = new NotifyPaymentDto();

                notify.UserType = entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered;
                notify.GatewayEnum = (GatewayEnumDto)serviceGateway.Gateway.Enum;
                notify.Name = name;
                notify.Surname = surname;
                notify.UserId = userId;
                notify.UserRegistred = isRegistredUser;
                notify.TransactionNumber = entity.PaymentIdentifierDto.UniqueIdentifier.ToString();
                notify.ServiceType = serviceGateway.ServiceType;
                notify.ServiceGatewayReferenceId = serviceGateway.ReferenceId;
                notify.GatewayId = serviceGateway.GatewayId;
                notify.PossibleGateways = service.ServiceGatewaysDto;
                notify.Bills = entity.Bills;
                notify.References = new string[]
                {
                    entity.ReferenceNumber, entity.ReferenceNumber2,
                    entity.ReferenceNumber3,
                    entity.ReferenceNumber4, entity.ReferenceNumber5,
                    entity.ReferenceNumber6,
                };
                notify.Bin = binNumber.ToString();
                notify.ServiceDepartament = (int)service.Departament;
                notify.CybersourceTransactionNumber = entity.TransactionNumber;

                notify.AutomaticPaymentDto = entity.PaymentType == PaymentTypeDto.Automatic ?
                    entity.ServiceAssociatedDto != null ? entity.ServiceAssociatedDto.AutomaticPaymentDto : null : null;

                //Nuevo para VisaNetOn (Apps) TODO: proximamente refactorear
                notify.PaymentDto = entity;

                var billsUpdated = MakePayment(notify);

                //SI LA PASARELA ES CARRETERA, IMPORTE O APPS O PAGOLINK, CANCELO EN CS YA QUE NO NOTIFIQUE A NADIE
                if (notify.GatewayEnum == GatewayEnumDto.Banred || notify.GatewayEnum == GatewayEnumDto.Sistarbanc ||
                    notify.GatewayEnum == GatewayEnumDto.Sucive || notify.GatewayEnum == GatewayEnumDto.Geocom)
                {
                    paymentDone = true;
                }

                if (billsUpdated != null)
                {
                    entity.GatewayId = notify.GatewayId;
                    entity.Bills = billsUpdated;
                }
                else
                {
                    throw new Exception("Bills vacias");
                }

                #region Registered User
                if (isRegistredUser)
                {
                    //PERSISTO EL REGISTRO DE UN PAGO EN EL SISTEMA
                    //si el servicio ya esta asociado, no necesito crear uno nuevo
                    var refs = new string[]
                               {
                                   entity.ReferenceNumber, entity.ReferenceNumber2,
                                   entity.ReferenceNumber3, entity.ReferenceNumber4,
                                   entity.ReferenceNumber5, entity.ReferenceNumber6
                               };

                    var serviceAssosiatedId = (entity.ServiceAssociatedId == null || entity.ServiceAssociatedId == Guid.Empty) ?
                            _serviceServiceAssosiate.IsServiceAssosiatedToUser(entity.RegisteredUserId.Value, entity.ServiceId, refs[0], refs[1], refs[2], refs[3], refs[4], refs[5])
                            : entity.ServiceAssociatedId;

                    //creo el nuevo servicio si no esta asociado
                    if (serviceAssosiatedId == Guid.Empty)
                    {
                        var fullNotifications = service.ServiceGatewaysDto.Count > 1 || service.EnableAutomaticPayment || service.ServiceGatewaysDto.FirstOrDefault().Gateway.Enum != (int)GatewayEnum.Carretera;
                        var serviceDto = new ServiceAssociatedDto
                        {
                            ReferenceNumber = entity.ReferenceNumber,
                            ReferenceNumber2 = entity.ReferenceNumber2,
                            ReferenceNumber3 = entity.ReferenceNumber3,
                            ReferenceNumber4 = entity.ReferenceNumber4,
                            ReferenceNumber5 = entity.ReferenceNumber5,
                            //si el servicio es de sucive, en el ultimo parametro de referencia agrego el id padron, para saltear una consulta.
                            ReferenceNumber6 = entity.ReferenceNumber6,
                            Description = entity.Description,
                            ServiceId = entity.ServiceId, //TODO: si tiene servicio contenedor poner el Id del contenedor??
                            DefaultCardId = entity.Card.Id,
                            Enabled = true,
                        };

                        var noticonf = new NotificationConfigDto
                        {
                            DaysBeforeDueDate = 5,
                            BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                            {
                                Email = fullNotifications,
                                Sms = false,
                                Web = false
                            },
                            ExpiredBillDto = new ExpiredBillDto()
                            {
                                Email = fullNotifications,
                                Sms = false,
                                Web = false
                            },
                            NewBillDto = new NewBillDto()
                            {
                                Email = fullNotifications,
                                Sms = false,
                                Web = false
                            },
                            FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                            {
                                Email = fullNotifications,
                                Sms = false,
                                Web = false
                            },
                            SuccessPaymentDto = new SuccessPaymentDto()
                            {
                                Email = true,
                                Sms = false,
                                Web = false
                            }
                        };

                        serviceDto.NotificationConfigDto = noticonf;
                        serviceDto.UserId = userId;

                        const bool notifyExternal = false; //Cuando se realiza un pago no se notifica asociacion
                        var newServiceAssosiated = _serviceServiceAssosiate.CreateOrUpdateDeleted(serviceDto, notifyExternal);
                        entity.ServiceAssociatedId = newServiceAssosiated.Id;
                    }
                    else
                    {
                        entity.ServiceAssociatedId = serviceAssosiatedId;
                        if (!String.IsNullOrEmpty(entity.Description))
                        {
                            var serviceAssociated = _serviceServiceAssosiate.GetById(serviceAssosiatedId.Value);
                            serviceAssociated.Description = entity.Description;
                            _serviceServiceAssosiate.EditDescription(serviceAssociated);

                            entity.ServiceAssociatedDto = serviceAssociated;
                        }
                    }
                }
                #endregion

                var dto = Create(entity, true);

                dto.GatewayEnum = entity.GatewayEnum;

                dto.ServiceDto = service;
                dto.ServiceAssociatedDto = entity.ServiceAssociatedDto;

                result.NewPaymentDto = dto;

                return result;
            }
            catch (ProviderWithoutConectionException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "Service Payment Excepcion");
                NLogLogger.LogEvent(ex);
                result.ExceptionCapture = ex;
                //Si se notifico al ente, solo envio mail
                if (paymentDone)
                {
                    var msg = String.Format("Se produjo un error en el portal, se notifico al ente pero no se pudo guardar el pago y/o facturas. Request ID:{0}, Fecha: {1}",
                        entity.TransactionNumber, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    var data = JsonConvert.SerializeObject(new
                    {
                        Message = msg,
                        Title = "Error en PORTAL al guardar pago.",
                    });
                    NotifyError(data);
                }
                else
                {
                    //SI O SI, HAY Q CANCELAR SIN IMPORTAR Q TIPO DE EXCEPTION SEA
                    var error = entity.TransactionNumber + " mascara:" + entity.Card.MaskedNumber;
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Gateway,
                        entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered,
                        userId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Must, error),
                        string.Format(LogStrings.CallCenter_Payment_Cancel, entity.TransactionNumber), ex);

                    var cancel = new CancelPayment();
                    cancel.UserId = userId;
                    cancel.Token = entity.Card.PaymentToken;
                    cancel.RequestId = entity.TransactionNumber;
                    cancel.UserType = entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered;
                    cancel.Amount = entity.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    cancel.UserEmail = entity.RegisteredUser != null ? entity.RegisteredUser.Email : entity.AnonymousUser != null ? entity.AnonymousUser.Email : "";
                    cancel.IdTransaccion = entity.TransactionNumber;
                    cancel.Currency = entity.Currency;
                    cancel.PaymentPlatform = entity.PaymentPlatform;
                    cancel.ServiceDto = entity.ServiceDto;
                    cancel.ServiceId = entity.ServiceId;

                    NLogLogger.LogEvent(NLogType.Info, "Service Payment - Cancel payment - entity.AmountTocybersource: " + entity.AmountTocybersource);
                    var cancelResult = CancelPaymentCybersource(cancel);
                    result.CyberSourceOperationData.VoidData = cancelResult.VoidData;
                    result.CyberSourceOperationData.ReversalData = cancelResult.ReversalData;
                    result.CyberSourceOperationData.RefundData = cancelResult.RefundData;
                }
                result.NewPaymentDto = null;
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Service Payment Excepcion");
                NLogLogger.LogEvent(e);
                result.ExceptionCapture = e;
                if (paymentDone)
                {
                    var msg =
                        String.Format(
                            "Se produjo un error en el portal, se notifico al ente pero no se pudo guardar el pago y/o facturas. Request ID:{0}, Fecha: {1}",
                            entity.TransactionNumber, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    var data = JsonConvert.SerializeObject(new
                    {
                        Message = msg,
                        Title =
                                                                   "Error en PORTAL al guardar pago.",
                    });
                    NotifyError(data);
                }
                else
                {
                    //SI O SI, HAY Q CANCELAR SIN IMPORTAR Q TIPO DE EXCEPTION SEA
                    var error = entity.TransactionNumber + " mascara:" + entity.Card.MaskedNumber;
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Gateway,
                        entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered,
                        userId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Must, error),
                        string.Format(LogStrings.CallCenter_Payment_Cancel, entity.TransactionNumber), e);

                    var cancel = new CancelPayment();
                    cancel.UserId = userId;
                    cancel.Token = entity.Card.PaymentToken;
                    cancel.RequestId = entity.TransactionNumber;
                    cancel.UserType = entity.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered;
                    cancel.Amount = entity.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    cancel.UserEmail = entity.RegisteredUser != null ? entity.RegisteredUser.Email : entity.AnonymousUser != null ? entity.AnonymousUser.Email : "";
                    cancel.IdTransaccion = entity.TransactionNumber;
                    cancel.Currency = entity.Currency;
                    cancel.PaymentPlatform = entity.PaymentPlatform;
                    cancel.ServiceDto = entity.ServiceDto;
                    cancel.ServiceId = entity.ServiceId;

                    NLogLogger.LogEvent(NLogType.Info, "Service Payment - Cancel payment - entity.AmountTocybersource: " + entity.AmountTocybersource);

                    var cancelResult = CancelPaymentCybersource(cancel);
                    result.CyberSourceOperationData.VoidData = cancelResult.VoidData;
                    result.CyberSourceOperationData.ReversalData = cancelResult.ReversalData;
                    result.CyberSourceOperationData.RefundData = cancelResult.RefundData;

                }
                result.NewPaymentDto = null;
                return result;
            }
        }

        public CybersourceCreatePaymentDto NotifyGateways(IDictionary<string, string> csDictionaryData)
        {
            var result = new CybersourceCreatePaymentDto
            {
                CyberSourceOperationData = new CyberSourceOperationData()
            };

            try
            {
                var tnrs = csDictionaryData.ContainsKey("transaction_id") ? csDictionaryData["transaction_id"] : string.Empty;
                NLogLogger.LogEvent(NLogType.Info, "Llega llamada NotifyGateways para TRNS " + tnrs);

                var cybersourceResponse = _serviceAnalyzeCsCall.ProcessCybersourceOperation(csDictionaryData);
                result.CyberSourceOperationData.PaymentData = cybersourceResponse.PaymentData;

                //ERROR EN CYBERSOURCE
                if (cybersourceResponse.PaymentData.PaymentResponseCode != (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (cybersourceResponse.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.DECISIONMANAGER)
                    {
                        var reverseResult = ReversePayment(new RefundPayment()
                        {
                            Amount = cybersourceResponse.CyberSourceData.ReqAmount,
                            Currency = cybersourceResponse.CyberSourceData.ReqCurrency,
                            ServiceId = cybersourceResponse.CyberSourceMerchantDefinedData.ServiceId,
                            UserId = cybersourceResponse.CyberSourceMerchantDefinedData.UserId,
                            UserType = cybersourceResponse.PaymentDto.RegisteredUserId != null && cybersourceResponse.PaymentDto.RegisteredUserId != Guid.Empty ?
                                           LogUserType.Registered : LogUserType.NoRegistered,
                            PaymentPlatform = cybersourceResponse.PaymentDto.PaymentPlatform,
                            Token = cybersourceResponse.CyberSourceData.PaymentToken,
                            IdOperation = cybersourceResponse.CyberSourceMerchantDefinedData.OperationId,
                            IdTransaccion = cybersourceResponse.PaymentData.PaymentRequestId,
                            RequestId = cybersourceResponse.PaymentData.PaymentRequestId,
                        });
                        result.CyberSourceOperationData.ReversalData = reverseResult.ReversalData;
                    }
                    return result;
                }

                var gateway =
                    _serviceService.GetGateways()
                        .FirstOrDefault(x => x.Id == cybersourceResponse.CyberSourceMerchantDefinedData.GatewayId);
                cybersourceResponse.PaymentDto.GatewayEnum = (GatewayEnumDto)gateway.Enum;

                if (cybersourceResponse.PaymentDto.RegisteredUserId.HasValue)
                {
                    cybersourceResponse.PaymentDto.RegisteredUser =
                        _serviceApplicationUser.GetById(cybersourceResponse.PaymentDto.RegisteredUserId.Value);
                }

                if (cybersourceResponse.PaymentDto.AnonymousUserId.HasValue)
                {
                    cybersourceResponse.PaymentDto.AnonymousUser = _serviceAnonymousUser.GetById(cybersourceResponse.PaymentDto.AnonymousUserId.Value);
                }

                if (cybersourceResponse.PaymentDto.Card == null)
                {
                    var card = _serviceCard.GetById(cybersourceResponse.PaymentDto.CardId);
                    cybersourceResponse.PaymentDto.Card = card;
                }

                var service = _serviceService.GetById(cybersourceResponse.PaymentDto.ServiceId);
                cybersourceResponse.PaymentDto.ServiceDto = service;

                LogCybersourceData(cybersourceResponse.PaymentDto, cybersourceResponse);
                result.NewPaymentDto = cybersourceResponse.PaymentDto;

                //check que factura no este marcada como paga en el portal
                var info = new CheckBillInsertedDto()
                {
                    ServiceId = cybersourceResponse.CyberSourceMerchantDefinedData.ServiceId,
                    SucivePreBillNumber = cybersourceResponse.CyberSourceMerchantDefinedData.BillSucivePreBillNumber,
                    BillExternalId = cybersourceResponse.CyberSourceMerchantDefinedData.BillNumber,
                    GatewayEnum = (GatewayEnumDto)gateway.Enum,
                };
                if (BillAlreadyPaid(info))
                {
                    CancelPaymentCybersource(new CancelPayment()
                    {
                        ServiceId = cybersourceResponse.CyberSourceMerchantDefinedData.ServiceId,
                        Amount = cybersourceResponse.PaymentDto.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Currency = cybersourceResponse.PaymentDto.Currency,
                        PaymentPlatform = cybersourceResponse.PaymentDto.PaymentPlatform,
                        UserEmail = cybersourceResponse.PaymentDto.RegisteredUser != null ? cybersourceResponse.PaymentDto.RegisteredUser.Email :
                            cybersourceResponse.PaymentDto.AnonymousUser != null ? cybersourceResponse.PaymentDto.AnonymousUser.Email : "",
                        UserType = cybersourceResponse.PaymentDto.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered,
                        UserId = cybersourceResponse.PaymentDto.AnonymousUserId.HasValue ? cybersourceResponse.PaymentDto.AnonymousUserId.Value : cybersourceResponse.PaymentDto.RegisteredUserId.Value,
                        Token = cybersourceResponse.PaymentDto.Card.PaymentToken,
                        RequestId = cybersourceResponse.PaymentData.PaymentRequestId,
                        IdTransaccion = cybersourceResponse.PaymentData.PaymentRequestId,
                        IdOperation = cybersourceResponse.CyberSourceMerchantDefinedData.OperationId
                    });
                    throw new BusinessException(CodeExceptions.BILL_ALREADY_PAID);
                }

                result.NewPaymentDto.IdOperation = !csDictionaryData.ContainsKey("req_merchant_defined_data27") ? string.Empty : csDictionaryData["req_merchant_defined_data27"];

                var notifyResult = NotifyGateways(result.NewPaymentDto);

                if (notifyResult.CyberSourceOperationData.VoidData != null)
                {
                    result.CyberSourceOperationData.VoidData = notifyResult.CyberSourceOperationData.VoidData;
                    result.CyberSourceOperationData.ReversalData = notifyResult.CyberSourceOperationData.ReversalData;
                    result.NewPaymentDto = null;
                }
                if (notifyResult.CyberSourceOperationData.RefundData != null)
                {
                    result.CyberSourceOperationData.RefundData = notifyResult.CyberSourceOperationData.RefundData;
                    result.NewPaymentDto = null;
                }

                if (notifyResult.NewPaymentDto != null)
                {
                    result.NewPaymentDto = notifyResult.NewPaymentDto;
                }
                return result;
            }
            catch (BusinessException ex)
            {
                result.ExceptionCapture = ex;
            }
            catch (ProviderWithoutConectionException ex)
            {
                result.ExceptionCapture = ex;
            }
            catch (Exception e)
            {
                result.ExceptionCapture = e;
            }
            result.NewPaymentDto = null;
            return result;
        }

        /// <summary>
        /// Marca como pagas las facturas ante los entes por medio de las pasarelas de pago.
        /// </summary>
        /// <param name="notify"></param>
        /// <returns>Retorna las facturas si se pudieron notificar</returns>
        public ICollection<BillDto> MakePayment(NotifyPaymentDto notify)
        {
            var bills = _serviceBill.PayBills(notify);
            return bills;
        }

        private Guid GetBestGateway(ServiceDto service, string mask)
        {
            if (service.ServiceGatewaysDto.Count(g => g.Active) == 1)
                return service.ServiceGatewaysDto.FirstOrDefault(g => g.Active).GatewayId;

            //Si hay mas de una gateway, debo chequear el BIN
            var value = Int32.Parse(mask.Substring(0, 6));
            var bin = _serviceBin.AllNoTracking(null, b => b.Value.Equals(value), b => b.Gateway).FirstOrDefault();

            return bin != null ? bin.GatewayId : Guid.Empty;
        }

        public IEnumerable<PaymentBillDto> ReportsTransactionsDataFromDbView(ReportsTransactionsFilterDto filters)
        {
            using (var context = new AppContext())
            {
                var result = context.Database.SqlQuery<PaymentBillDto>(
                   "StoreProcedure_VisaNet_ReportTransaction " +
                   "@inputDateFrom = {0}, " +
                   "@inputDateTo = {1}, " +
                   "@inputPaymentTransactionNumber = {2}, " +
                   "@inputPaymentUniqueIdentifier = {3}, " +
                   "@inputServiceId = {4}, " +
                   "@inputClientEmail = {5}, " +
                   "@inputClientName = {6}, " +
                   "@inputClientSurname = {7}, " +
                   "@inputGatewayId = {8}, " +
                   "@inputServiceCategoryId = {9}, " +
                   "@inputServiceAssociatedId = {10} , " +
                   "@inputPaymentStatus = {11}, " +
                   "@inputPlatform = {12}, " +
                   "@inputPaymentType = {13}, " +
                   "@inputOrderBy = {14}, " +
                   "@inputOrder = {15}, " +
                   "@inputDisplayLength = {16}, " +
                   "@inputDisplayStart = {17}, " +
                   "@inputCount = {18}",
                   DateTime.Parse(filters.DateFromString, new CultureInfo("es-UY")),
                   DateTime.Parse(filters.DateToString, new CultureInfo("es-UY")),
                   filters.PaymentTransactionNumber,
                   filters.PaymentUniqueIdentifier,
                   filters.ServiceId.HasValue && filters.ServiceId != Guid.Empty ? filters.ServiceId : null,
                   filters.ClientEmail,
                   filters.ClientName,
                   filters.ClientSurname,
                   filters.GatewayId.HasValue && filters.GatewayId != Guid.Empty ? filters.GatewayId : null,
                   filters.ServiceCategoryId.HasValue && filters.ServiceCategoryId != Guid.Empty ? filters.ServiceCategoryId : null,
                   filters.ServiceAssociatedId.HasValue && filters.ServiceAssociatedId != Guid.Empty ? filters.ServiceAssociatedId : null,
                   filters.PaymentStatus.HasValue ? filters.PaymentStatus : 0,
                   filters.Platform,
                   filters.PaymentType,
                   filters.OrderBy,
                   filters.SortDirection.ToString(),
                   filters.DisplayLength,
                   filters.DisplayStart,
                   false);

                var list = result.ToList();
                return list;
            }
        }

        public int ReportsTransactionsDataCount(ReportsTransactionsFilterDto filters)
        {
            using (var context = new AppContext())
            {
                var countResult = context.Database.SqlQuery<int>(
                    "StoreProcedure_VisaNet_ReportTransaction " +
                    "@inputDateFrom = {0}, " +
                    "@inputDateTo = {1}, " +
                    "@inputPaymentTransactionNumber = {2}, " +
                    "@inputPaymentUniqueIdentifier = {3}, " +
                    "@inputServiceId = {4}, " +
                    "@inputClientEmail = {5}, " +
                    "@inputClientName = {6}, " +
                    "@inputClientSurname = {7}, " +
                    "@inputGatewayId = {8}, " +
                    "@inputServiceCategoryId = {9}, " +
                    "@inputServiceAssociatedId = {10}, " +
                    "@inputPaymentStatus = {11}, " +
                    "@inputPlatform = {12}, " +
                    "@inputPaymentType = {13}, " +
                    "@inputOrderBy = {14}, " +
                    "@inputOrder = {15}, " +
                    "@inputDisplayLength = {16}, " +
                    "@inputDisplayStart = {17}, " +
                    "@inputCount = {18}",
                    DateTime.Parse(filters.DateFromString, new CultureInfo("es-UY")),
                    DateTime.Parse(filters.DateToString, new CultureInfo("es-UY")),
                    filters.PaymentTransactionNumber,
                    filters.PaymentUniqueIdentifier,
                    filters.ServiceId.HasValue && filters.ServiceId != Guid.Empty ? filters.ServiceId : null,
                    filters.ClientEmail,
                    filters.ClientName,
                    filters.ClientSurname,
                    filters.GatewayId.HasValue && filters.GatewayId != Guid.Empty ? filters.GatewayId : null,
                    filters.ServiceCategoryId.HasValue && filters.ServiceCategoryId != Guid.Empty ? filters.ServiceCategoryId : null,
                    filters.ServiceAssociatedId.HasValue && filters.ServiceAssociatedId != Guid.Empty ? filters.ServiceAssociatedId : null,
                    filters.PaymentStatus.HasValue ? filters.PaymentStatus : 0,
                    filters.Platform,
                    filters.PaymentType,
                    filters.OrderBy,
                    filters.SortDirection.ToString(),
                    filters.DisplayLength,
                    filters.DisplayStart,
                    true);

                var count = countResult.First();
                return count;
            }
        }

        public CyberSourceOperationData CancelPaymentCybersource(CancelPayment cancel)
        {
            try
            {
                var csOperationData = _cyberSourceAccess.VoidPayment(cancel);
                var result = AnalizeCsData(csOperationData, cancel.RequestId);
                return csOperationData;
            }
            catch (Exception e)
            {
                //EN ESTE PUNTO, NO SE PUDO CANCELAR NI REALIZAR UN REFUND. SE DEBE NOTIFICAR
                //TODO: agregar usuario al mail
                var parameter = _repositoryParameters.AllNoTracking().First();
                _serviceNotificationMessage.SendPaymentCancellationError(cancel.UserEmail, cancel.UserId, cancel.UserType, parameter, cancel.RequestId, DateTime.Now);
                //_repositoryNotificationMessage.Create(new EmailMessage
                //{
                //    CreationDateTime = DateTime.Now,
                //    EmailType = EmailType.PaymentCancellationError,
                //    DataByType = JsonConvert.SerializeObject(new
                //    {
                //        RequestId = cancel.RequestId,
                //        Amount = cancel.Amount,
                //        MerchandId = cancel.ServiceDto.MerchantId,
                //        UserEmail = cancel.UserEmail,
                //        UserId = cancel.UserId
                //    }),
                //});
                //_repositoryNotificationMessage.Save();
                NLogLogger.LogEvent(NLogType.Info, "ServicePayment - CancelPaymentCybersource Excepcion");
                NLogLogger.LogEvent(e);
            }
            return null;
        }

        private CyberSourceOperationData RefundPaymentCybersource(RefundPayment refund)
        {
            var result = _cyberSourceAccess.RefundPayment(refund);
            if (result.RefundData != null && result.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
            {
                var changed = ChangePaymentStatus(PaymentStatus.Refunded, refund.RequestId);
                if (!changed) NLogLogger.LogEvent(NLogType.Info, string.Format("Service payment - CancelPaymentCybersource - No se pudo actualizar al estado {0} el pago {1}", PaymentStatus.Refunded, refund.RequestId));
            }
            return result;
        }

        public CyberSourceOperationData ReversePayment(RefundPayment reverse)
        {
            var result = _cyberSourceAccess.ReversePayment(reverse);
            //EL REVERSO SOLO ES POR 481. SI HAY UN VOID ES AUTOMATICO. NO ACTUALIZO ESTADO DE PAYMENT PORQUE NO EXISTE TAL
            return result;
        }

        public void GeneratePaymentTicket(Guid id, string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType)
        {
            try
            {
                _servicePaymentTicket.GeneratePaymentTicket(transactionNumber, userId, out renderedBytes, out mimeType);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("GeneratePaymentTicket - Excepcion. transactionNumber {0}", transactionNumber));
                NLogLogger.LogEvent(exception);

                renderedBytes = new byte[] { };
                mimeType = null;
            }
        }

        public void SendPaymentTicketByEmail(Guid id, string transactionNumber, Guid userId)
        {
            var payment =
                AllNoTracking(null, p => p.Id == id && p.TransactionNumber == transactionNumber, x => x.AnonymousUser, x => x.RegisteredUser).FirstOrDefault();

            if (payment == null)
                throw new BusinessException(CodeExceptions.PAYMENT_NOT_FOUND);

            #region Mail

            byte[] arrBytes;
            string mimeType;

            GeneratePaymentTicket(id, transactionNumber, userId, out arrBytes, out mimeType);

            _serviceNotificationMessage.SendCopyPayment(payment.AnonymousUserId != null, transactionNumber, payment.AnonymousUser, payment.RegisteredUser, arrBytes, mimeType);

            #endregion
        }

        public IEnumerable<PaymentDto> GetPaymentBatch(DateTime date)
        {
            var query = Repository.AllNoTracking(p => p.Gateway.Enum == (int)GatewayEnum.Sistarbanc
                && p.Date.Day.CompareTo(date.Day) == 0 && p.Date.Month.CompareTo(date.Month) == 0 && p.Date.Year.CompareTo(date.Year) == 0 &&
                (p.PaymentStatus == PaymentStatus.Done || p.PaymentStatus == PaymentStatus.Processed),
                p => p.PaymentIdentifier, p => p.Service, p => p.Bills, p => p.RegisteredUser, p => p.AnonymousUser);

            var list = query.Select(t => new PaymentDto
            {
                Id = t.Id,
                Date = t.Date,
                ServiceId = t.ServiceId,
                PaymentIdentifierDto = new PaymentIdentifierDto()
                {
                    UniqueIdentifier = t.PaymentIdentifier.UniqueIdentifier
                },
                ServiceDto = new ServiceDto
                {
                    Id = t.Service.Id,
                    Name = t.Service.Name,
                    Description = t.Service.Description,
                    MerchantId = t.Service.MerchantId,
                    ImageName = t.Service.ImageName,
                    ServiceContainerImageName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.ImageName : string.Empty,
                    ServiceCategoryId = t.Service.ServiceCategoryId,
                    ServiceGatewaysDto = t.Service.ServiceGateways.Where(s => s.Gateway.Name.Equals("Sistarbanc")).Select(sg => new ServiceGatewayDto()
                    {
                        ReferenceId = sg.ReferenceId,
                        ServiceType = sg.ServiceType,
                        GatewayId = sg.GatewayId,
                        Gateway = new GatewayDto()
                        {
                            Enum = sg.Gateway.Enum
                        }
                    }).ToList()
                },
                RegisteredUserId = t.RegisteredUserId,
                RegisteredUser = new ApplicationUserDto()
                {
                    SistarbancUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.RegisteredUserId == null ? -1 : t.RegisteredUser.SistarbancUser.UniqueIdentifier
                    },
                    SistarbancBrouUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.RegisteredUserId == null ? -1 : t.RegisteredUser.SistarbancBrouUser.UniqueIdentifier
                    }
                },
                AnonymousUserId = t.AnonymousUserId,
                AnonymousUser = new AnonymousUserDto()
                {
                    SistarbancUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.AnonymousUserId == null ? -1 : t.AnonymousUser.SistarbancUser.UniqueIdentifier
                    },
                    SistarbancBrouUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.AnonymousUserId == null ? -1 : t.AnonymousUser.SistarbancBrouUser.UniqueIdentifier
                    }
                },
                TransactionNumber = t.TransactionNumber,
                //ServiceAssociatedId = t.ServiceAssosiatedId,
                //ServiceAssociatedDto = new ServiceAssociatedDto
                //{
                //    ServiceDto = new ServiceDto
                //    {
                //        Description = t.ServiceAssosiated.Service.Description,
                //        Image = new ImageDto
                //        {
                //            InternalName = t.ServiceAssosiated.Service.Image != null ? t.ServiceAssosiated.Service.Image.InternalName : ""
                //        },
                //        MerchantId = t.ServiceAssosiated.Service.MerchantId,
                //        Name = t.ServiceAssosiated.Service.Name
                //    }
                //},
                Bills = t.Bills.Select(b => new BillDto
                {
                    Currency = b.Currency,
                    Amount = b.Amount,
                    GatewayTransactionId = b.GatewayTransactionId,
                    BillExternalId = b.BillExternalId,
                    ExpirationDate = b.ExpirationDate,
                    PaymentId = b.PaymentId,
                    DiscountAmount = b.DiscountAmount,

                }).ToList(),
                Card = new CardDto
                {
                    MaskedNumber = t.Card.MaskedNumber,
                    Description = t.Card.Description
                }
            }).ToList();

            foreach (var paymentDto in list)
            {
                paymentDto.ServiceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.Id, paymentDto.ServiceDto.ImageName);
                if (paymentDto.ServiceDto.ServiceContainerId.HasValue)
                {
                    paymentDto.ServiceDto.ServiceContainerImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.ServiceContainerId.Value, paymentDto.ServiceDto.ServiceContainerImageName);
                }
            }

            return list;
        }

        public bool IsPaymentDoneWithServiceAssosiated(Guid serviceAssosiatedId)
        {
            return Repository.AllNoTracking().Any(p => p.ServiceAssosiatedId == serviceAssosiatedId);
        }

        public int CountPaymentsDone(Guid registredUserId, Guid anonymousUserId, Guid serviceId)
        {
            var result = 0;
            result = Guid.Empty != registredUserId ?
                Repository.AllNoTracking().Count(p => p.ServiceId == serviceId && p.RegisteredUserId == registredUserId)
                : Repository.AllNoTracking().Count(p => p.ServiceId == serviceId && p.AnonymousUserId == anonymousUserId);

            return result;
        }

        public IEnumerable<PaymentDto> GetDashboardData(ReportsDashboardFilterDto filters)
        {

            var query = Repository.AllNoTracking(null, x => x.Bills);

            if (filters.From != default(DateTime))
            {
                query = query.Where(p => p.Date >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(p => p.Date < filters.To);
            }

            var payments = query.Select(t => new PaymentDto
            {
                PaymentType = (PaymentTypeDto)(int)t.PaymentType,
                Date = t.Date,
                Bills = t.Bills.Select(b => new BillDto
                {
                    Currency = b.Currency,
                    Amount = b.Amount
                }).ToList()
            }).ToList();

            //if (payments.Any(x => x.Bills == null))
            //{
            //    NLogLogger.LogEvent(NLogType.Error, "Transacciones con bills = null: " + string.Join(";", payments.Where(x => x.Bills == null).SelectMany(x => x.TransactionNumber).ToList()));    
            //}
            //if (payments.Any(x => x.Bills.Any()))
            //{
            //    NLogLogger.LogEvent(NLogType.Error, "Transacciones con bill empty: " + string.Join(";", payments.Where(x => x.Bills.Any()).SelectMany(x => x.TransactionNumber).ToList()));
            //}


            //Convierto el monto para sumar en la misma moneda en el reporte
            foreach (var payment in payments)
            {
                var quotationDollars = _serviceQuotation.GetQuotationForDate(payment.Date, CurrencyDto.USD);
                if (quotationDollars != null)
                {
                    if (filters.Currency == (int)CurrencyDto.UYU)
                    {
                        if (payment.Bills.FirstOrDefault().Currency != Currency.PESO_URUGUAYO)
                        {
                            //En este caso tengo que pasar de dolares a pesos
                            payment.Bills.FirstOrDefault().Amount = payment.Bills.FirstOrDefault().Amount *
                                                                    quotationDollars.ValueInPesos;
                        }
                    }
                    else
                    {
                        if (payment.Bills.FirstOrDefault().Currency != Currency.DOLAR_AMERICANO)
                        {
                            //Tengo que pasar de pesos a dolares
                            payment.Bills.FirstOrDefault().Amount = payment.Bills.FirstOrDefault().Amount /
                                                                    quotationDollars.ValueInPesos;
                        }
                    }
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Error, "No pude obtener quotationDollars para al fecha " + payment.Date.ToString("dd-MM-yyyy"));
                }
            }

            return payments;
        }

        public IEnumerable<PaymentDto> GetPaymentBatch(DateTime date, GatewayEnum gatewayEnum, Guid serviceId, int departament)
        {
            var query = Repository.AllNoTracking(p => p.Gateway.Enum == (int)gatewayEnum
                && p.Date.Day.CompareTo(date.Day) == 0 && p.Date.Month.CompareTo(date.Month) == 0 && p.Date.Year.CompareTo(date.Year) == 0
                && (p.PaymentStatus == PaymentStatus.Done || p.PaymentStatus == PaymentStatus.Processed),
                p => p.PaymentIdentifier, p => p.Service, p => p.Bills, p => p.RegisteredUser, p => p.AnonymousUser, p => p.Gateway, p => p.ServiceAssosiated
                //, p => p.Service.ServiceGateways, p => p.Service.ServiceGateways.Select(s => s.Gateway)
                );

            if (serviceId != null && serviceId != Guid.Empty)
            {
                query = query.Where(x => x.ServiceId == serviceId);
            }
            if (departament > 0)
            {
                query = query.Where(x => x.Service.Departament == (DepartamentType)departament);
            }

            var list = query.Select(t => new PaymentDto
            {
                Id = t.Id,
                Date = t.Date,
                ServiceId = t.ServiceId,
                PaymentIdentifierDto = new PaymentIdentifierDto()
                {
                    UniqueIdentifier = t.PaymentIdentifier.UniqueIdentifier
                },
                TransactionNumber = t.TransactionNumber,
                Bills = t.Bills.Select(b => new BillDto
                {
                    Currency = b.Currency,
                    Amount = b.Amount,
                    GatewayTransactionId = b.GatewayTransactionId,
                    BillExternalId = b.BillExternalId,
                    ExpirationDate = b.ExpirationDate,
                    PaymentId = b.PaymentId,
                    DiscountAmount = b.DiscountAmount,
                    TaxedAmount = b.TaxedAmount,
                    SucivePreBillNumber = b.SucivePreBillNumber,

                }).ToList(),
                ReferenceNumber = t.ReferenceNumber,
                ReferenceNumber2 = t.ReferenceNumber2,
                ReferenceNumber3 = t.ReferenceNumber3,
                ReferenceNumber6 = t.ReferenceNumber6, //idpadron

                Card = new CardDto
                {
                    MaskedNumber = t.Card.MaskedNumber,
                    Description = t.Card.Description
                },
                RegisteredUserId = t.RegisteredUserId,
                RegisteredUser = new ApplicationUserDto()
                {
                    SistarbancUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.RegisteredUserId == null ? -1 : t.RegisteredUser.SistarbancUser.UniqueIdentifier
                    },
                    SistarbancBrouUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.RegisteredUserId == null ? -1 : t.RegisteredUser.SistarbancBrouUser.UniqueIdentifier
                    }
                },
                AnonymousUserId = t.AnonymousUserId,
                AnonymousUser = new AnonymousUserDto()
                {
                    SistarbancUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.AnonymousUserId == null ? -1 : t.AnonymousUser.SistarbancUser.UniqueIdentifier
                    },
                    SistarbancBrouUser = new SistarbancUserDto()
                    {
                        UniqueIdentifier = t.AnonymousUserId == null ? -1 : t.AnonymousUser.SistarbancBrouUser.UniqueIdentifier
                    }
                },
                ServiceDto = new ServiceDto
                {
                    Id = t.Service.Id,
                    Name = t.Service.Name,
                    Description = t.Service.Description,
                    MerchantId = t.Service.MerchantId,
                    ImageName = t.Service.ImageName,
                    ServiceContainerImageName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.ImageName : string.Empty,
                    ServiceCategoryId = t.Service.ServiceCategoryId,
                    Departament = (DepartamentDtoType)t.Service.Departament,
                    ServiceGatewaysDto = t.Service.ServiceGateways.Select(sg => new ServiceGatewayDto()
                    {
                        ReferenceId = sg.ReferenceId,
                        ServiceType = sg.ServiceType,
                        GatewayId = sg.GatewayId,
                        Gateway = new GatewayDto()
                        {
                            Enum = sg.Gateway.Enum
                        }
                    }).ToList()
                },
                ServiceAssociatedId = t.ServiceAssosiatedId,
                ServiceAssociatedDto = t.ServiceAssosiated != null ?
                    new ServiceAssociatedDto()
                    {
                        ReferenceNumber6 = t.ServiceAssosiated.ReferenceNumber6
                    } :
                    new ServiceAssociatedDto()
                    {
                        ReferenceNumber6 = string.Empty
                    },
                GatewayDto = new GatewayDto()
                {
                    Enum = t.Gateway.Enum
                }
            }).OrderByDescending(x => x.Date).ToList();

            foreach (var paymentDto in list)
            {
                paymentDto.ServiceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.Id, paymentDto.ServiceDto.ImageName);
                if (paymentDto.ServiceDto.ServiceContainerId.HasValue)
                {
                    paymentDto.ServiceDto.ServiceContainerImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.ServiceContainerId.Value, paymentDto.ServiceDto.ServiceContainerImageName);
                }
            }

            return list;
        }

        public void NotifyError(string data)
        {
            var parameters = _repositoryParameters.AllNoTracking().First();
            _serviceNotificationMessage.SendCybersourceError(parameters, string.Empty, data);
        }

        public IEnumerable<PaymentDto> GetDataForUserReports(ReportFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.ServiceAssosiated.Service.Name.Contains(filters.GenericSearch.ToLower()));

            if (filters.UserId != default(Guid))
                query = query.Where(sc => sc.RegisteredUserId == filters.UserId);

            if (filters.From != null && filters.From != default(DateTime))
            {
                filters.From = filters.From.Value.Date;
                query = query.Where(sc => sc.Date >= filters.From.Value);
            }

            if (filters.To != null && filters.To != default(DateTime))
            {
                filters.To = filters.To.Value.Date.AddDays(1);
                query = query.Where(sc => sc.Date < filters.To.Value);
            }

            //por ahora siempre se devuelven los de estado Done o Processed
            query = query.Where(sc =>
                        (int)sc.PaymentStatus == (int)PaymentStatusDto.Done ||
                        (int)sc.PaymentStatus == (int)PaymentStatusDto.Processed);

            var list = query.Select(t => new PaymentDto
            {
                Id = t.Id,
                Date = t.Date,
                ServiceId = t.ServiceId,
                ServiceDto = new ServiceDto
                {
                    Id = t.Service.Id,
                    Name = t.Service.Name,
                    Description = t.Service.Description,
                    MerchantId = t.Service.MerchantId,
                    ImageName = t.Service.ImageName,
                    ServiceContainerImageName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.ImageName : string.Empty,
                    ServiceCategoryId = t.Service.ServiceCategoryId,
                    ServiceCategoryName = t.Service.ServiceCategory.Name
                },
                ServiceAssociatedId = t.ServiceAssosiatedId,
                ServiceAssociatedDto = new ServiceAssociatedDto
                {
                    Description = t.ServiceAssosiated != null ? t.ServiceAssosiated.Description : ""
                },
                CardId = t.CardId,
                Card = new CardDto
                {
                    MaskedNumber = t.Card.MaskedNumber,
                    Description = t.Card.Description
                },
                TransactionNumber = t.TransactionNumber,
                TotalAmount = t.TotalAmount,
                PaymentType = (PaymentTypeDto)(int)t.PaymentType,
                Bills = t.Bills.Select(b => new BillDto { Currency = b.Currency, Amount = b.Amount }).ToList()
            }).ToList();

            foreach (var paymentDto in list)
            {
                paymentDto.ServiceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.Id, paymentDto.ServiceDto.ImageName);
                if (paymentDto.ServiceDto.ServiceContainerId.HasValue)
                {
                    paymentDto.ServiceDto.ServiceContainerImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.ServiceContainerId.Value, paymentDto.ServiceDto.ServiceContainerImageName);
                }
            }

            return list;
        }

        private bool ChangePaymentStatus(PaymentStatus status, string transactionId)
        {
            try
            {
                Repository.ContextTrackChanges = true;
                var payments = Repository.All(x => x.TransactionNumber.Equals(transactionId)).ToList();
                if (!payments.Any() || payments.Count > 1) return true;

                var payment = payments.First();
                payment.PaymentStatus = status;
                Repository.Edit(payment);
                Repository.Save();
                Repository.ContextTrackChanges = false;

                return true;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Service payment - ChangePaymentStatus - Excepcion capturada");
                NLogLogger.LogEvent(exception);
            }
            return false;
        }

        public List<TransactionHistory> TransactionsHistoryForWebService(WsBillQueryDto dto)
        {
            var result = _repositoryPayment.HistoryTrans(dto.Date, dto.IdApp, dto.CodBranch == 0 ? null : dto.CodBranch.ToString(),
                dto.CodCommerce == 0 ? null : dto.CodCommerce.ToString(), dto.RefClient, dto.RefClient2, dto.RefClient3, dto.RefClient4, dto.RefClient5, dto.RefClient6,
                dto.BillNumber, string.IsNullOrWhiteSpace(dto.IdMerchant) ? null : dto.IdMerchant);
            return result;
        }

        public CyberSourceOperationData TestCyberSourcePayment(Guid serviceId)
        {
            return _cyberSourceAccess.TestPayment(serviceId);
        }
        public CyberSourceOperationData TestCyberSourceReversal(Guid serviceId)
        {
            return _cyberSourceAccess.TestReversePayment(serviceId);
        }
        public CyberSourceOperationData TestCyberSourceCancelPayment(Guid serviceId, CyberSourceOperationData cSourceOperationData)
        {
            return _cyberSourceAccess.TestCancelPayment(serviceId, cSourceOperationData);
        }

        public bool TestCyberSourceReports(Guid serviceId)
        {
            return _cyberSourceAccess.TestReports(serviceId);
        }

        private CardDto CreateNewCard(CardDto cardDto, Guid applicationUserId)
        {
            if (applicationUserId != Guid.Empty && cardDto != null && cardDto.Id == Guid.Empty)
                return _serviceApplicationUser.AddCard(cardDto, applicationUserId);

            return null;
        }

        public List<PaymentDto> GetDataForFromList(PaymentFilterDto filters)
        {
            var query = Repository.AllNoTracking(null, x => x.ServiceAssosiated, x => x.Service, x => x.Service.ServiceContainer, x => x.Bills);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.ServiceAssosiated.Service.Name.Contains(filters.GenericSearch.ToLower()));

            if (filters.UserId != default(Guid))
                query = query.Where(sc => sc.RegisteredUserId == filters.UserId);

            if (filters.From != null && filters.From != default(DateTime))
            {
                filters.From = filters.From.Value.Date;
                query = query.Where(sc => sc.Date >= filters.From.Value);
            }

            if (filters.To != null && filters.To != default(DateTime))
            {
                filters.To = filters.To.Value.Date.AddDays(1);
                query = query.Where(sc => sc.Date < filters.To.Value);
            }

            if (!string.IsNullOrEmpty(filters.ServiceAssociatedDto))
                query = query.Where(sc => sc.ServiceAssosiated.Service.Name.ToLower().Contains(filters.ServiceAssociatedDto.ToLower()));

            if (!string.IsNullOrEmpty(filters.TrnsNumber))
                query = query.Where(sc => sc.TransactionNumber.Equals(filters.TrnsNumber));

            if (filters.PaymentTypeFilterDto > 0)
            {
                switch (filters.PaymentTypeFilterDto)
                {
                    case PaymentTypeFilterDto.Portal:
                        query = query.Where(sc => ((int)sc.PaymentType) == (int)PaymentTypeDto.Manual);
                        break;
                    case PaymentTypeFilterDto.Mobile:
                        query = query.Where(sc => (int)sc.PaymentPlatform == (int)PaymentPlatformDto.Mobile);
                        break;
                    case PaymentTypeFilterDto.AutomaticPayment:
                        query = query.Where(sc => ((int)sc.PaymentType) == (int)PaymentTypeDto.Automatic);
                        break;
                    case PaymentTypeFilterDto.Apps:
                        query = query.Where(sc => ((int)sc.PaymentType) == (int)PaymentTypeDto.App);
                        break;
                }
            }

            //por ahora siempre se devuelven los de estado Done o Processed
            query = query.Where(sc =>
                        (int)sc.PaymentStatus == (int)PaymentStatusDto.Done ||
                        (int)sc.PaymentStatus == (int)PaymentStatusDto.Processed);


            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);

            if (filters.DisplayLength.HasValue)
            {
                query = query.Skip(filters.DisplayStart * filters.DisplayLength.Value);
                query = query.Take(filters.DisplayLength.Value);
            }
            else
            {
                query = query.Skip(filters.DisplayStart);
            }

            var list = query.Select(t => new PaymentDto
            {
                Id = t.Id,
                Date = t.Date,
                ServiceId = t.ServiceId,
                ServiceDto = new ServiceDto
                {
                    Id = t.Service.Id,
                    Name = t.Service.Name,
                    Description = t.Service.Description,
                    ImageName = t.Service.ImageName,
                    ImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, t.Service.Id, t.Service.ImageName, false),
                    ServiceContainerName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.Name : string.Empty,

                    ServiceContainerImageUrl = t.Service.ServiceContainer == null
                        ? string.Empty
                        : FileStorage.Instance.GetImageUrl(_serviceFolderBlob, t.Service.ServiceContainerId.Value, t.Service.ServiceContainer.ImageName, false),
                    ServiceContainerImageName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.ImageName : string.Empty
                },
                ServiceAssociatedDto = new ServiceAssociatedDto
                {
                    Description = t.ServiceAssosiated != null ? t.ServiceAssosiated.Description : ""
                },
                Card = new CardDto
                {
                    MaskedNumber = t.Card.MaskedNumber,
                    Description = t.Card.Description
                },
                TransactionNumber = t.TransactionNumber,
                TotalAmount = t.TotalAmount,
                Currency = t.Currency,
                Quotas = t.Quotas,
                Bills = t.Bills.Select(b => new BillDto { Currency = b.Currency, Amount = b.Amount }).ToList(),

            }).ToList();

            foreach (var paymentDto in list)
            {
                paymentDto.ServiceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.Id, paymentDto.ServiceDto.ImageName);
                if (paymentDto.ServiceDto.ServiceContainerId.HasValue)
                {
                    paymentDto.ServiceDto.ServiceContainerImageUrl = FileStorage.Instance.GetImageUrl(_serviceFolderBlob, paymentDto.ServiceDto.ServiceContainerId.Value, paymentDto.ServiceDto.ServiceContainerImageName);
                }
            }

            return list;
        }

        public CyberSourceOperationData CancelPaymentDone(Guid id, string transactionNumber, bool notify)
        {
            var payment = id != Guid.Empty
                ? _repositoryPayment.GetById(id, x => x.Service, x => x.Card, x => x.RegisteredUser, x => x.AnonymousUser) :
                _repositoryPayment.AllNoTracking(x => x.TransactionNumber == transactionNumber, x => x.Service, x => x.Card, x => x.RegisteredUser, x => x.AnonymousUser).FirstOrDefault();

            if (string.IsNullOrEmpty(transactionNumber))
            {
                transactionNumber = payment.TransactionNumber;
            }
            if (payment == null)
                throw new BusinessException(CodeExceptions.PAYMENT_NOT_FOUND_NROTRNS, new[] { transactionNumber });

            if (payment.PaymentStatus != PaymentStatus.Done && payment.PaymentStatus != PaymentStatus.Processed)
            {
                throw new BusinessException(CodeExceptions.PAYMENT_STATUS_NOT_VALID, new[] { transactionNumber, payment.PaymentStatus.ToString() });
            }

            var cancel = new CancelPayment();
            cancel.UserId = payment.RegisteredUserId.HasValue ? payment.RegisteredUserId.Value : payment.AnonymousUserId.Value;
            cancel.Token = payment.Card.PaymentToken;
            cancel.RequestId = payment.TransactionNumber;
            cancel.UserType = payment.AnonymousUserId.HasValue ? LogUserType.NoRegistered : LogUserType.Registered;
            cancel.Amount = payment.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"));
            cancel.UserEmail = payment.RegisteredUser != null ? payment.RegisteredUser.Email : payment.AnonymousUser != null ? payment.AnonymousUser.Email : "";
            cancel.IdTransaccion = payment.TransactionNumber;
            cancel.Currency = payment.Currency;
            cancel.PaymentPlatform = (PaymentPlatformDto)payment.PaymentPlatform;
            cancel.ServiceId = payment.ServiceId;

            var csOperationData = this.CancelPaymentCybersource(cancel);

            if (notify)
            {
                if (csOperationData.VoidData != null)
                {
                    if (csOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        //SI NO HAY REVERSO IGUAL SE CANCELO LA TRNS
                        NotifyCustomer(csOperationData, transactionNumber);
                    }
                    if (csOperationData.VoidData.PaymentResponseCode != (int)CybersourceMsg.Accepted)
                    {
                        //SI NO SE HIZO EL VOID, SE INTENTA EL REFUND
                        if (csOperationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            NotifyCustomer(csOperationData, transactionNumber);
                        }
                    }
                }
            }
            return csOperationData;
        }

        public CyberSourceOperationData DeleteCardInCybersource(Guid userId, Guid cardId, string transactionNumber)
        {
            try
            {
                var card = _serviceCard.GetById(cardId);

                if (card == null)
                    throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH);

                if (string.IsNullOrEmpty(transactionNumber))
                {
                    transactionNumber = card.CybersourceTransactionId;
                }

                if (!string.IsNullOrEmpty(card.PaymentToken))
                {
                    var delete = new DeleteCardDto
                    {
                        UserId = userId,
                        RequestId = cardId.ToString(),
                        Token = card.PaymentToken,
                        IdTransaccion = transactionNumber
                    };

                    var csOperationData = _cyberSourceAccess.DeleteCard(delete);

                    if (csOperationData.DeleteData.DeleteResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        _serviceCard.CardDeletedFromCS(card.Id, true);
                    }

                    return csOperationData;
                }

            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
            }
            return null;
        }

        private void NotifyCustomer(CyberSourceOperationData operationData, string transactionNumber)
        {
            var now = DateTime.Now;
            var payment = _repositoryPayment.AllNoTracking(x => x.TransactionNumber == transactionNumber,
                x => x.Service, x => x.Card, x => x.RegisteredUser, x => x.AnonymousUser).FirstOrDefault();
            var service = payment.Service;
            ServiceDto serviceContainer = null;

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
                        "Se realizo la cancelación de la transacción {0} realizada el {1} por el servicio {2}", payment.TransactionNumber, dateString, serviceName);

            #region Mail

            var email = payment.RegisteredUser != null ? payment.RegisteredUser.Email : payment.AnonymousUser.Email;

            _serviceNotificationMessage.SendPaymentDoneCancellation(email, mailDesc, payment.TransactionNumber, dateString,
                payment.AmountTocybersource.ToString("##,#0.00", CultureInfo.CurrentCulture),
                cancellationTransaccionId, now.ToString("dd/MM/yyyy hh:mm"), cancellationAmount);
            #endregion

            #region AppNotification
            _serviceNotification.Create(new NotificationDto
            {
                Date = now,
                Message = mailDesc,
                NotificationPrupose = NotificationPruposeDto.SueccessNotification,
                RegisteredUserId = payment.RegisteredUserId.Value,
                ServiceId = payment.ServiceId,
            });
            #endregion
        }

        public bool AnalizeCsData(CyberSourceOperationData csOperationData, string transactionNumber)
        {
            var result = false;
            if (csOperationData.VoidData != null)
            {
                if (csOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                {
                    //SI HAY VOID, TIENE QUE TENER REFERSE
                    if (csOperationData.ReversalData != null && csOperationData.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        result = ChangePaymentStatus(PaymentStatus.Reversed, transactionNumber);
                        if (!result)
                            NLogLogger.LogEvent(NLogType.Info,
                                string.Format("Service payment - CancelPaymentCybersource - No se pudo actualizar al estado {0} el pago {1}", PaymentStatus.Reversed, transactionNumber));
                    }
                    else
                    {
                        result = ChangePaymentStatus(PaymentStatus.Voided, transactionNumber);
                        if (!result)
                            NLogLogger.LogEvent(NLogType.Info,
                                string.Format("Service payment - CancelPaymentCybersource - No se pudo actualizar al estado {0} el pago {1}", PaymentStatus.Voided, transactionNumber));
                    }
                }
                if (csOperationData.VoidData.PaymentResponseCode != (int)CybersourceMsg.Accepted)
                {
                    //SI NO SE HIZO EL VOID, SE INTENTA EL REFUND
                    if (csOperationData.RefundData != null)
                    {
                        if (csOperationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            result = ChangePaymentStatus(PaymentStatus.Refunded, transactionNumber);
                            if (!result)
                                NLogLogger.LogEvent(NLogType.Info,
                                    string.Format("Service payment - CancelPaymentCybersource - No se pudo actualizar al estado {0} el pago {1}", PaymentStatus.Refunded, transactionNumber));
                        }
                    }
                }
            }
            return result;
        }

        public bool BillAlreadyPaid(CheckBillInsertedDto checkBillInsertedDto)
        {
            Payment paymentIn = null;

            switch (checkBillInsertedDto.GatewayEnum)
            {
                case GatewayEnumDto.Carretera:
                case GatewayEnumDto.Apps:
                case GatewayEnumDto.Sistarbanc:
                case GatewayEnumDto.Banred:
                    paymentIn = _repositoryPayment.AllNoTracking(x =>
                            (x.PaymentStatus == PaymentStatus.Done || x.PaymentStatus == PaymentStatus.Processed)
                            && x.Bills.Any(y => y.BillExternalId == checkBillInsertedDto.BillExternalId)
                            && x.ServiceId == checkBillInsertedDto.ServiceId
                        ).FirstOrDefault();
                    break;
                case GatewayEnumDto.Sucive:
                case GatewayEnumDto.Geocom:
                    paymentIn = _repositoryPayment.AllNoTracking(x =>
                            (x.PaymentStatus == PaymentStatus.Done || x.PaymentStatus == PaymentStatus.Processed)
                            && x.Bills.Any(y => y.SucivePreBillNumber == checkBillInsertedDto.SucivePreBillNumber && x.ServiceId == checkBillInsertedDto.ServiceId)
                        ).FirstOrDefault();
                    break;
            }

            return paymentIn != null;
        }

        public CybersourceTransactionsDataDto CybersourceAnalyze(IDictionary<string, string> csDictionaryData)
        {
            return _serviceAnalyzeCsCall.ProcessCybersourceOperation(csDictionaryData);
        }

        public void LogCybersourceData(PaymentDto payment, CybersourceTransactionsDataDto csTransactionsDataDto)
        {
            var strLog = payment.AnonymousUser != null ?
                string.Format(LogStrings.Payment_Init, payment.AnonymousUser.Email, payment.AnonymousUser.Name, payment.AnonymousUser.Surname, payment.ServiceDto == null ? "" : payment.ServiceDto.Name) :
                string.Format(LogStrings.Payment_Init, payment.RegisteredUser.Email, payment.RegisteredUser.Name, payment.RegisteredUser.Surname, payment.ServiceDto == null ? "" : payment.ServiceDto.Name);

            Guid tempGuid;
            Guid? tempGuid2 = null;
            tempGuid2 = Guid.TryParse(csTransactionsDataDto.CyberSourceMerchantDefinedData.TemporaryTransactionIdentifier, out tempGuid) ? tempGuid : tempGuid2;

            var cyberSourceData = csTransactionsDataDto.CyberSourceData;
            var verifyByVisaData = csTransactionsDataDto.VerifyByVisaData;
            if (payment.AnonymousUser != null)
            {
                _loggerService.CreateLogForAnonymousUser(
                    LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet, payment.AnonymousUser.Id,
                    strLog, strLog, null,
                    new CyberSourceLogDataDto
                    {
                        AuthAmount = cyberSourceData.AuthAmount,
                        AuthAvsCode = cyberSourceData.AuthAvsCode,
                        AuthCode = cyberSourceData.AuthCode,
                        AuthResponse = cyberSourceData.AuthResponse,
                        AuthTime = cyberSourceData.AuthTime,
                        AuthTransRefNo = cyberSourceData.AuthTransRefNo,
                        BillTransRefNo = cyberSourceData.BillTransRefNo,
                        Decision = cyberSourceData.Decision,
                        Message = cyberSourceData.Message,
                        PaymentToken = cyberSourceData.PaymentToken,
                        ReasonCode = cyberSourceData.ReasonCode,
                        ReqAmount = cyberSourceData.ReqAmount,
                        ReqCardExpiryDate = cyberSourceData.ReqCardExpiryDate,
                        ReqCardNumber = cyberSourceData.ReqCardNumber,
                        ReqCardType = cyberSourceData.ReqCardType,
                        ReqCurrency = cyberSourceData.ReqCurrency,
                        ReqPaymentMethod = cyberSourceData.ReqPaymentMethod,
                        ReqProfileId = cyberSourceData.ReqProfileId,
                        ReqReferenceNumber = cyberSourceData.ReqReferenceNumber,
                        ReqTransactionType = cyberSourceData.ReqTransactionType,
                        ReqTransactionUuid = cyberSourceData.ReqTransactionUuid,
                        TransactionId = cyberSourceData.TransactionId,
                        PaymentPlatform = PaymentPlatform.VisaNet,
                        TransactionType = TransactionType.Payment
                    },
                    new CyberSourceVerifyByVisaDataDto
                    {
                        PayerAuthenticationXid = verifyByVisaData.PayerAuthenticationXid,
                        PayerAuthenticationProofXml = verifyByVisaData.PayerAuthenticationProofXml,
                        PayerAuthenticationCavv = verifyByVisaData.PayerAuthenticationCavv,
                        PayerAuthenticationEci = verifyByVisaData.PayerAuthenticationEci,
                    },
                    tempGuid2
                    );
            }

            if (payment.RegisteredUser != null)
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                    payment.RegisteredUser.Id, strLog, strLog, null,
                    new CyberSourceLogDataDto
                    {
                        AuthAmount = cyberSourceData.AuthAmount,
                        AuthAvsCode = cyberSourceData.AuthAvsCode,
                        AuthCode = cyberSourceData.AuthCode,
                        AuthResponse = cyberSourceData.AuthResponse,
                        AuthTime = cyberSourceData.AuthTime,
                        AuthTransRefNo = cyberSourceData.AuthTransRefNo,
                        BillTransRefNo = cyberSourceData.BillTransRefNo,
                        Decision = cyberSourceData.Decision,
                        Message = cyberSourceData.Message,
                        PaymentToken = cyberSourceData.PaymentToken,
                        ReasonCode = cyberSourceData.ReasonCode,
                        ReqAmount = cyberSourceData.ReqAmount,
                        ReqCardExpiryDate = cyberSourceData.ReqCardExpiryDate,
                        ReqCardNumber = cyberSourceData.ReqCardNumber,
                        ReqCardType = cyberSourceData.ReqCardType,
                        ReqCurrency = cyberSourceData.ReqCurrency,
                        ReqPaymentMethod = cyberSourceData.ReqPaymentMethod,
                        ReqProfileId = cyberSourceData.ReqProfileId,
                        ReqReferenceNumber = cyberSourceData.ReqReferenceNumber,
                        ReqTransactionType = cyberSourceData.ReqTransactionType,
                        ReqTransactionUuid = cyberSourceData.ReqTransactionUuid,
                        TransactionId = cyberSourceData.TransactionId,
                        TransactionType = TransactionType.Payment,
                        PaymentPlatform = PaymentPlatform.VisaNet
                    },
                    new CyberSourceVerifyByVisaDataDto
                    {
                        PayerAuthenticationXid = verifyByVisaData.PayerAuthenticationXid,
                        PayerAuthenticationProofXml = verifyByVisaData.PayerAuthenticationProofXml,
                        PayerAuthenticationCavv = verifyByVisaData.PayerAuthenticationCavv,
                        PayerAuthenticationEci = verifyByVisaData.PayerAuthenticationEci,
                    },
                    tempGuid2
                    );
            }
        }

        public bool NotifyExternalSourceNewPayment(PaymentDto paymentDto)
        {
            try
            {
                if (paymentDto.PaymentType == PaymentTypeDto.AnonymousUser)
                {
                    return _serviceExternalNotification.NotifyExternalSourceNewPaymentAnonymous(paymentDto);
                }

                const bool withAssociation = false; // withAssociation debe ir siempre en false?
                var idUserExternal = paymentDto.IdUserExternal;
                var idCardExternal = paymentDto.Card != null && paymentDto.Card.ExternalId != null ? paymentDto.Card.ExternalId.ToString() : string.Empty;

                //Si el PaymentDto no tiene el IdUserExternal, lo voy a buscar al servicio asociado
                if (string.IsNullOrEmpty(idUserExternal) && paymentDto.ServiceAssociatedId.HasValue)
                {
                    var serviceAssociated = _serviceServiceAssosiate.GetById(paymentDto.ServiceAssociatedId.Value);
                    idUserExternal = serviceAssociated.IdUserExternal.HasValue ? serviceAssociated.IdUserExternal.ToString() : string.Empty;
                }

                //Si el PaymentDto no tiene el IdCardExternal, lo voy a buscar a la tabla Cards
                if (string.IsNullOrEmpty(idCardExternal))
                {
                    var card = _serviceCard.GetById(paymentDto.CardId);
                    idCardExternal = card.ExternalId.HasValue ? card.ExternalId.ToString() : string.Empty;
                }

                return _serviceExternalNotification.NotifyExternalSourceNewPayment(paymentDto, idUserExternal, idCardExternal, withAssociation);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServicePayment - NotifyExternalSourceNewPayment - Exception");
            }
            return false;
        }

    }
}