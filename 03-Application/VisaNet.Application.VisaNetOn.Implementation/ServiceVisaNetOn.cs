using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Application.VisaNetOn.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public abstract class ServiceVisaNetOn : IServiceVisaNetOn
    {
        protected readonly IServicePayment ServicePayment;
        protected readonly IServiceServiceAssosiate ServiceServiceAssosiate;
        protected readonly IServiceVonData ServiceVonData;
        protected readonly IServiceService ServiceService;
        protected readonly IServiceExternalNotification ServiceExternalNotification;
        protected readonly IServiceWebhookRegistration ServiceWebhookRegistration;

        protected ServiceVisaNetOn(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration
            )
        {
            ServicePayment = servicePayment;
            ServiceServiceAssosiate = serviceServiceAssosiate;
            ServiceVonData = serviceVonData;
            ServiceService = serviceService;
            ServiceExternalNotification = serviceExternalNotification;
            ServiceWebhookRegistration = serviceWebhookRegistration;
        }

        public abstract ResultDto ProcessOperation(IDictionary<string, string> formData);


        //PAGOS
        protected CybersourceCreatePaymentDto GeneratePayment(CybersourceTransactionsDataDto cybersourceResponse)
        {
            var service = ServiceService.GetById(cybersourceResponse.PaymentDto.ServiceId, x => x.ServiceContainer);
            var idApp = !string.IsNullOrEmpty(service.UrlName) ? service.UrlName : service.ServiceContainerDto.UrlName;
            var idOperation = cybersourceResponse.PaymentDto.IdOperation;

            var result = new CybersourceCreatePaymentDto
            {
                CyberSourceOperationData = new CyberSourceOperationData()
            };
            result.CyberSourceOperationData.PaymentData = cybersourceResponse.PaymentData;

            var paymentDto = cybersourceResponse.PaymentDto;
            paymentDto.IdOperation = idOperation;


            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GeneratePayment - IdApp: {0}, IdOperation: {1}", idApp, idOperation));

            try
            {
                //ERROR EN CYBERSOURCE
                if (cybersourceResponse.PaymentData.PaymentResponseCode != (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GeneratePayment - Pago NO realizado - Codigo CS: {0}, IdApp: {1}, IdOperation: {2}", cybersourceResponse.PaymentData.PaymentResponseCode, idApp, idOperation));
                    if (cybersourceResponse.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.DECISIONMANAGER)
                    {
                        var reverseResult = ServicePayment.ReversePayment(new RefundPayment
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

                var gateway = ServiceService.GetGateways().FirstOrDefault(x => x.Id == cybersourceResponse.CyberSourceMerchantDefinedData.GatewayId);
                cybersourceResponse.PaymentDto.GatewayEnum = (GatewayEnumDto)gateway.Enum;

                cybersourceResponse.PaymentDto.ServiceDto = service;

                ServicePayment.LogCybersourceData(cybersourceResponse.PaymentDto, cybersourceResponse);

                //check que factura no este marcada como paga en el portal
                var info = new CheckBillInsertedDto()
                {
                    ServiceId = cybersourceResponse.CyberSourceMerchantDefinedData.ServiceId,
                    SucivePreBillNumber = cybersourceResponse.CyberSourceMerchantDefinedData.BillSucivePreBillNumber,
                    BillExternalId = cybersourceResponse.CyberSourceMerchantDefinedData.BillNumber,
                    GatewayEnum = (GatewayEnumDto)gateway.Enum,
                };
                if (ServicePayment.BillAlreadyPaid(info))
                {
                    var cancelPayment = new CancelPayment
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
                    };
                    ServicePayment.CancelPaymentCybersource(cancelPayment);
                    throw new BusinessException(CodeExceptions.BILL_ALREADY_PAID);
                }

                var notifyResult = ServicePayment.NotifyGateways(paymentDto);
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
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GeneratePayment BusinessException - IdApp: {0}, IdOperation: {1}, Message: {2}", idApp, idOperation, ex.Message));
                NLogLogger.LogEvent(ex);
                result.ExceptionCapture = ex;
            }
            catch (ProviderWithoutConectionException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GeneratePayment ProviderWithoutConectionException - IdApp: {0}, IdOperation: {1}, Message: {2}", idApp, idOperation, ex.Message));
                NLogLogger.LogEvent(ex);
                result.ExceptionCapture = ex;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GeneratePayment Exception - IdApp: {0}, IdOperation: {1}, Message: {2}", idApp, idOperation, e.Message));
                NLogLogger.LogEvent(e);
                result.ExceptionCapture = e;
            }
            result.NewPaymentDto = null;
            return result;
        }

        protected ResultDto ProcessPaymentResult(CybersourceCreatePaymentDto paymentResult, string idApp, string idOperation)
        {
            var result = new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error general."
            };

            if (paymentResult.ExceptionCapture != null)
            {
                //Ocurrio un error
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProcessPaymentResult ExceptionCapture - " +
                    "IdApp: {0}, IdOperation: {1}, Message: {2}", idApp, idOperation, paymentResult.ExceptionCapture.Message));
                result.ResultCode = "1";
                result.ResultDescription = "Error general.";

                if (paymentResult.ExceptionCapture.GetType() == typeof(BusinessException))
                {
                    var exception = (BusinessException)paymentResult.ExceptionCapture;
                    if (exception.Code == CodeExceptions.BILL_ALREADY_PAID)
                    {
                        result.ResultCode = "8";
                        result.ResultDescription = "Factura ya paga en el sistema.";
                    }
                }
            }
            else
            {
                if (paymentResult.CyberSourceOperationData.PaymentData != null)
                {
                    if (paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                    {
                        if (paymentResult.NewPaymentDto == null)
                        {
                            //Ocurrio un error
                            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProcessPaymentResult - Ocurrio un error al registrar el pago (NewPaymentDto null). " +
                                "IdApp: {0}, IdOperation: {1}", idApp, idOperation));
                            result.ResultCode = "1";
                            result.ResultDescription = "Error general.";
                        }
                        else
                        {
                            //Pago exitoso
                            result.ResultCode = "0";
                            result.ResultDescription = "Pago realizado.";

                            //ACTUALIZAR WEBHOOKREGISTRATION
                            ServiceWebhookRegistration.UpdatewithPaymentId(idApp, idOperation, paymentResult.NewPaymentDto.Id);
                        }
                    }
                    else
                    {   //Ocurrio un error
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProcessPaymentResult - Ocurrio un error al registrar el pago (PaymentResponseCode: {0}). " +
                            "IdApp: {1}, IdOperation: {2}", paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode, idApp, idOperation));

                        if (Enum.IsDefined(typeof(ErrorCodeDto),
                            paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode))
                        {
                            result.ResultCode = paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode.ToString();
                            result.ResultDescription = paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseMsg;
                        }
                        else
                        {
                            result.ResultCode = "1";
                            result.ResultDescription = "Error al realizar el pago (Código CS: " + paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode + ").";
                        }
                    }
                }
                else
                {
                    //Ocurrio un error
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProcessPaymentResult - Ocurrio un error al registrar el pago (PaymentData null). " +
                        "IdApp: {0}, IdOperation: {1}", idApp, idOperation));

                    result.ResultCode = "1";
                    result.ResultDescription = "Error general.";
                }
            }

            return result;
        }


        //ASOCIACIONES
        protected Tuple<CybersourceCreateAppAssociationDto, VonDataDto> GenerateAssociationForRecurrentUser(CybersourceTransactionsDataDto cybersourceResponse, bool isNewUser)
        {
            var associationResponse = new CybersourceCreateAppAssociationDto
            {
                CybersourceCreateServiceAssociatedDto = new CybersourceCreateServiceAssociatedDto
                {
                    CybersourceCreateCardDto = new CybersourceCreateCardDto()
                }
            };
            VonDataDto vonUserData = null;

            var userCreated = false;
            var cardCreated = false;
            try
            {
                associationResponse.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData = cybersourceResponse.TokenizationData;
                associationResponse.CybersourceCreateServiceAssociatedDto.CyberSourceMerchantDefinedData = cybersourceResponse.CyberSourceMerchantDefinedData;

                var service = ServiceService.GetById(cybersourceResponse.PaymentDto.ServiceId, x => x.ServiceContainer);
                var idApp = !string.IsNullOrEmpty(service.UrlName) ? service.UrlName : service.ServiceContainerDto.UrlName;

                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GenerateAssociationForRecurrentUser - IdApp: {0}, IdOperation: {1}", idApp, cybersourceResponse.PaymentDto.IdOperation));

                if (cybersourceResponse.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (isNewUser)
                    {
                        //Se crea el registro del nuevo usuario recurrente
                        vonUserData = RegisterVONUser(cybersourceResponse);
                        userCreated = true;
                        cardCreated = true;
                    }
                    else
                    {
                        //Se crea el registro de nueva tarjeta para el mismo usuario recurrente
                        vonUserData = AddCardToVONUser(cybersourceResponse);
                        cardCreated = true;
                    }

                    associationResponse.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData = cybersourceResponse.TokenizationData;
                    return new Tuple<CybersourceCreateAppAssociationDto, VonDataDto>(associationResponse, vonUserData);
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - GenerateAssociationForRecurrentUser - Cybersource devolvio distinto de Ok - idApp: {0}, Operación: {1}, Codigo CS {2}",
                        idApp, cybersourceResponse.PaymentDto.IdOperation, cybersourceResponse.TokenizationData.PaymentResponseCode));
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = cybersourceResponse.TokenizationData.PaymentResponseCode;
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = cybersourceResponse.TokenizationData.PaymentResponseMsg;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProccesDataFromCybersource - Exception. Operación {0}",
                    associationResponse.WebhookRegistrationDto != null ? cybersourceResponse.PaymentDto.IdOperation : string.Empty));
                NLogLogger.LogEvent(e);
                associationResponse.CybersourceCreateServiceAssociatedDto.ExceptionCapture = e;
            }

            if (userCreated)
            {
                var email = vonUserData != null && vonUserData.AnonymousUserDto != null ? vonUserData.AnonymousUserDto.Email : string.Empty;
                if (cardCreated)
                {
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = 14;
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = string.Format("Ha ocurrido un error. " +
                        "Se creo el usuario {0} y tambien se creo la tarjeta pero no se pudo terminar con el proceso de asociación. " +
                        "Por favor intente nuevamente y si el error persiste comuníquese con el call center", email);
                }
                else
                {
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = 15;
                    associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = string.Format("Ha ocurrido un error. " +
                        "Se creo el usuario {0} pero no se pudo terminar con el proceso de asociación. " +
                        "Por favor intente nuevamente y si el error persiste comuníquese con el call center", email);
                }
            }
            else
            {
                associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = 1;
                associationResponse.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = "Error general.";
            }

            return new Tuple<CybersourceCreateAppAssociationDto, VonDataDto>(associationResponse, vonUserData);
        }

        protected ResultDto ProcessAssociationResult(CybersourceCreateAppAssociationDto associationResult, bool isRegisteredUser, string idApp, string idOperation)
        {
            var result = new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error general."
            };

            var createAppAssociation = associationResult.CybersourceCreateServiceAssociatedDto;

            if (createAppAssociation != null)
            {
                if (createAppAssociation.ExceptionCapture != null)
                {
                    //Ocurrio un error
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOn - ProcessAssociationResult ExceptionCapture - " +
                        "IdApp: {0}, IdOperation: {1}, Message: {2}", idApp, idOperation, createAppAssociation.ExceptionCapture.Message));
                    result.ResultCode = "1";
                    result.ResultDescription = "Error general.";
                }
                else
                {
                    var tokenizationData = createAppAssociation.CybersourceCreateCardDto.TokenizationData;
                    if (createAppAssociation.AssociationInternalErrorCode == (int)ErrorCodeDto.OK)
                    {
                        if (tokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                        {
                            if (isRegisteredUser && createAppAssociation.ServiceAssociatedDto == null)
                            {
                                //Ocurrio un error
                                result.ResultCode = "1";
                                result.ResultDescription = "Error general.";
                            }
                            else
                            {
                                //Asociacion exitosa
                                result.ResultCode = "0";
                                result.ResultDescription = "Asociación exitosa.";
                            }
                        }
                        else
                        {
                            //Ocurrio un error
                            if (Enum.IsDefined(typeof(ErrorCodeDto), tokenizationData.PaymentResponseCode))
                            {
                                result.ResultCode = tokenizationData.PaymentResponseCode.ToString();
                                result.ResultDescription = tokenizationData.PaymentResponseMsg;
                            }
                            else
                            {
                                result.ResultCode = "1";
                                result.ResultDescription = "Error al realizar la asociación (Código CS: " + tokenizationData.PaymentResponseCode + ").";
                            }
                        }
                    }
                    else
                    {
                        //Ocurrio un error
                        result.ResultCode = createAppAssociation.AssociationInternalErrorCode.ToString();
                        result.ResultDescription = createAppAssociation.AssociationInternalErrorDesc;
                    }

                    var invalidCardMsg = InvalidCardData((CybersourceMsg)tokenizationData.PaymentResponseCode);

                    if (!string.IsNullOrEmpty(invalidCardMsg))
                    {
                        //Ocurrio un error
                        result.ResultCode = tokenizationData.PaymentResponseCode.ToString();
                        result.ResultDescription = invalidCardMsg;
                    }
                }
            }
            else
            {
                //Ocurrio un error
                result.ResultCode = "1";
                result.ResultDescription = "Error general.";
            }
            return result;
        }


        //AUXILIARES
        protected VonDataDto RegisterVONUser(CybersourceTransactionsDataDto cybersourceResponse)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - Registrando nuevo usuario recurrente.");

                var service = ServiceService.GetById(cybersourceResponse.PaymentDto.ServiceId);
                var appId = service.UrlName;
                if (string.IsNullOrEmpty(appId))
                {
                    var containerService = ServiceService.GetById(service.ServiceContainerId.Value);
                    appId = containerService.UrlName;
                }

                var vonData = new VonDataDto
                {
                    AppId = appId,
                    AnonymousUserId = cybersourceResponse.PaymentDto.AnonymousUserId.Value,
                    UserExternalId = Guid.NewGuid().ToString(),
                    CardExternalId = cybersourceResponse.PaymentDto.Card.ExternalId.ToString(),
                    CardName = cybersourceResponse.PaymentDto.Card.Name,
                    CardMaskedNumber = cybersourceResponse.PaymentDto.Card.MaskedNumber,
                    CardToken = cybersourceResponse.PaymentDto.Card.PaymentToken,
                    CardDueDate = cybersourceResponse.PaymentDto.Card.DueDate,
                    ReferenceNumber = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber1,
                    ReferenceNumber2 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber2,
                    ReferenceNumber3 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber3,
                    ReferenceNumber4 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber4,
                    ReferenceNumber5 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber5,
                    ReferenceNumber6 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber6,
                    CreationDate = DateTime.Now
                };

                return ServiceVonData.Create(vonData, true);
            }
            catch (BusinessException e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - " + e.Message);
                throw e;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - Error al registrar nuevo usuario recurrente.");
                throw e;
            }
        }

        protected VonDataDto AddCardToVONUser(CybersourceTransactionsDataDto cybersourceResponse)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - Agregando tarjeta a usuario recurrente.");

                var service = ServiceService.GetById(cybersourceResponse.PaymentDto.ServiceId);
                var appId = service.UrlName;
                if (string.IsNullOrEmpty(appId))
                {
                    var containerService = ServiceService.GetById(service.ServiceContainerId.Value);
                    appId = containerService.UrlName;
                }

                var vonDataRegister = ServiceVonData.Find(appId, cybersourceResponse.PaymentDto.AnonymousUserId.Value).FirstOrDefault();

                var vonData = new VonDataDto
                {
                    AppId = appId,
                    AnonymousUserId = cybersourceResponse.PaymentDto.AnonymousUserId.Value,
                    UserExternalId = vonDataRegister.UserExternalId,
                    CardExternalId = cybersourceResponse.PaymentDto.Card.ExternalId.ToString(),
                    CardName = cybersourceResponse.PaymentDto.Card.Name,
                    CardMaskedNumber = cybersourceResponse.PaymentDto.Card.MaskedNumber,
                    CardToken = cybersourceResponse.PaymentDto.Card.PaymentToken,
                    CardDueDate = cybersourceResponse.PaymentDto.Card.DueDate,
                    ReferenceNumber = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber1,
                    ReferenceNumber2 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber2,
                    ReferenceNumber3 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber3,
                    ReferenceNumber4 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber4,
                    ReferenceNumber5 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber5,
                    ReferenceNumber6 = cybersourceResponse.CyberSourceMerchantDefinedData.ReferenceNumber6,
                    CreationDate = DateTime.Now
                };

                return ServiceVonData.Create(vonData, true);
            }
            catch (BusinessException e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - " + e.Message);
                throw e;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOn - ProcessOperation - Error al agregar tarjeta a usuario recurrente.");
                throw e;
            }
        }

        protected ServiceDto GetServiceForIntegration(Guid serviceId)
        {
            try
            {
                //Si el servicio tiene contenedor, entonces se devuelve el contenedor, sino a el mismo (porque los datos de integracion los posee el contenedor) 
                var service = ServiceService.GetById(serviceId, x => x.ServiceContainer);
                var integrationService = service.ServiceContainerDto ?? service;
                return integrationService;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        protected void GetIdAppAndIdOperation(IDictionary<string, string> formData, out string idApp, out string idOperation)
        {
            idOperation = formData["req_merchant_defined_data27"];
            var integrationService = GetServiceForIntegration(new Guid(formData["req_merchant_defined_data30"]));
            idApp = integrationService != null ? integrationService.UrlName : "";
        }

        private string InvalidCardData(CybersourceMsg reasonCode)
        {
            var msg = string.Empty;
            if (reasonCode != CybersourceMsg.Accepted)
            {
                switch (reasonCode)
                {
                    case CybersourceMsg.InvalidFields:
                        msg = PresentationWebStrings.Card_InvalidFields;
                        break;
                    case CybersourceMsg.ExpiredCard:
                        msg = PresentationWebStrings.Card_ExpiriedCard;
                        break;
                    case CybersourceMsg.InsufficientFunds:
                        msg = PresentationWebStrings.Card_InsufficientFunds;
                        break;
                    case CybersourceMsg.StolenLostCard:
                        msg = PresentationWebStrings.Card_StolenLostCard;
                        break;
                    case CybersourceMsg.CreditLimitReached:
                        msg = PresentationWebStrings.Card_CreditLimitReached;
                        break;
                    case CybersourceMsg.InvalidCVN:
                        msg = PresentationWebStrings.Card_InvalidCVN;
                        break;
                    case CybersourceMsg.AccountFrozen:
                        msg = PresentationWebStrings.Card_AccountFrozen;
                        break;
                    case CybersourceMsg.InvalidAccountNumber:
                        msg = PresentationWebStrings.Card_InvalidAccountNumber;
                        break;
                    case CybersourceMsg.CardTypeNotAccepted:
                        msg = PresentationWebStrings.Card_CardTypeNotAccepted;
                        break;
                    case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                        msg = PresentationWebStrings.Card_InvalidCardTypeOrNotCorrelateWithCardNumber;
                        break;
                    case CybersourceMsg.CVNCheckInvalid:
                        msg = PresentationWebStrings.Card_CVNCheckInvalid;
                        break;
                    default:
                        msg =
                            "No hemos podido procesar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.";
                        break;
                }
            }
            return msg;
        }

    }
}