using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServiceIntegration : IServiceIntegration
    {
        private readonly IServiceDiscountCalculator _serviceDiscountCalculator;
        private readonly IServiceService _serviceService;
        private readonly IServicePayment _servicePayment;
        private readonly ICyberSourceAccess _serviceCyberSourceAccess;
        private readonly IServiceCard _serviceCard;
        private readonly IServicePaymentIdentifier _paymentIdentifier;
        private readonly IServiceWsBillPaymentOnline _serviceWsBillPaymentOnline;
        private readonly IServiceWsPaymentCancellation _serviceWsPaymentCancellation;
        private readonly IServiceWsCardRemove _serviceWsCardRemove;
        private readonly ILoggerService _loggerService;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceNotification _serviceNotification;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceAssociationSelector _serviceAssociationSelector;
        private readonly IServiceWebhookNewAssociation _serviceWebhookNewAssociation;

        public ServiceIntegration(IServiceDiscountCalculator serviceDiscountCalculator, IServiceService serviceService, IServicePayment servicePayment,
            ICyberSourceAccess serviceCyberSourceAccess, IServiceCard serviceCard, IServicePaymentIdentifier paymentIdentifier,
            IServiceWsBillPaymentOnline serviceWsBillPaymentOnline, IServiceWsPaymentCancellation serviceWsPaymentCancellation, ILoggerService loggerService,
            IServiceFixedNotification serviceFixedNotification, IServiceNotification serviceNotification, IServiceWsCardRemove serviceWsCardRemove,
            IServiceEmailMessage serviceNotificationMessage, IServiceBin serviceBin, IServiceAssociationSelector serviceAssociationSelector, IServiceWebhookNewAssociation serviceWebhookNewAssociation)
        {
            _serviceDiscountCalculator = serviceDiscountCalculator;
            _serviceService = serviceService;
            _servicePayment = servicePayment;
            _serviceCyberSourceAccess = serviceCyberSourceAccess;
            _serviceCard = serviceCard;
            _paymentIdentifier = paymentIdentifier;
            _serviceWsBillPaymentOnline = serviceWsBillPaymentOnline;
            _serviceWsPaymentCancellation = serviceWsPaymentCancellation;
            _loggerService = loggerService;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceNotification = serviceNotification;
            _serviceWsCardRemove = serviceWsCardRemove;
            _serviceNotificationMessage = serviceNotificationMessage;
            _serviceBin = serviceBin;
            _serviceAssociationSelector = serviceAssociationSelector;
            _serviceWebhookNewAssociation = serviceWebhookNewAssociation;
        }

        public TransactionResult MakePayment(WsBillPaymentOnlineDto data)
        {
            var resultData = new TransactionResult();
            var paymentdone = false;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration MakePayment: idOperacion {0}, id app {1}", data.IdOperation, data.IdApp));

                var strLog = string.Format(LogStrings.IntegrationService_Payment_Init, data.IdApp, data.IdOperation);
                _loggerService.CreateLog(LogType.Info, LogOperationType.WebServicePayment, LogCommunicationType.WebService, strLog);

                var services = !string.IsNullOrEmpty(data.IdMerchant) ? _serviceService.GetServicesFromMerchand(data.IdApp, data.IdMerchant, GatewayEnumDto.Apps).ToList()
                                    : _serviceService.GetServices(data.IdApp, data.CodCommerce, data.CodBranch, GatewayEnumDto.Apps).ToList();

                //Si no obtengo el servicio
                if (!services.Any())
                {
                    var errorLog = string.Format(LogStrings.Log_CommerceNotFound, data.CodCommerce, data.CodBranch, data.IdApp, data.IdMerchant);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);

                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.PaymentError,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Error,
                        Id = Guid.NewGuid(),
                        Description = "Comercio No Encontrado",
                        Detail = errorLog,
                    });

                    resultData.OperationResult = 56;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                //Si hay mas de un servicio con los datos pasados (merchantId o codComercio/codSucursal)
                if (services.Count() > 1)
                {
                    var errorLog = string.Format(LogStrings.Log_CommerceDuplicated, data.CodCommerce, data.CodBranch, data.IdApp, data.IdMerchant);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);

                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.PaymentError,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Error,
                        Id = Guid.NewGuid(),
                        Description = "Comercio Duplicado",
                        Detail = errorLog,
                    });

                    resultData.OperationResult = 58;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                var serviceToPay = services.First();

                if (!serviceToPay.Active)
                {
                    var errorLog = string.Format(LogStrings.Log_CommerceNotFound, data.CodCommerce, data.CodBranch, data.IdApp, data.IdMerchant);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 57;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                if (serviceToPay.ServiceContainerDto != null && !serviceToPay.ServiceContainerDto.AllowWcfPayment ||
                    serviceToPay.ServiceContainerDto == null && !serviceToPay.AllowWcfPayment)
                {
                    var errorLog = string.Format(LogStrings.Log_ServiceNotAllow_Wcf, data.IdApp, data.CodCommerce, data.CodBranch, data.IdMerchant);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 76;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                var correct = _serviceService.IsFatherOrHim(data.IdApp, data.IdMerchant, data.CodCommerce.ToString(), data.CodBranch.ToString());
                if (!correct)
                {
                    var errorLog = string.Format(LogStrings.Log_ServiceFather_Error, data.IdApp, data.CodCommerce, data.CodBranch);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 55;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }
                if ((int)serviceToPay.DiscountType != data.Indi)
                {
                    var serviceIndi = ((int)serviceToPay.DiscountType).ToString();
                    var errorLog = string.Format(LogStrings.Service_DifDiscount, serviceToPay.Name, serviceIndi, data.IdApp, data.Indi);
                    _loggerService.CreateLog(LogType.Info, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    //resultData.OperationResult = 17;
                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.ServiceConfiguration,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Info,
                        Id = Guid.NewGuid(),
                        Description = "Servicio Ley Diferente",
                        Detail = errorLog + Environment.NewLine + "Esto genero que se procesara la transacción pero puede haber problemas de conciliación y si no se modifica la configuración seguramente ocurra el mismo problema con todas las posibles futuras transacciones asociadas con este servicio.",
                    });
                }

                if (data.AmountTotal <= 0)
                {
                    var errorLog = string.Format(LogStrings.Log_Amount_Error, data.IdOperation, data.AmountTotal);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 71;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                if (data.AmountTaxed < 0 || data.AmountTaxed > data.AmountTotal)
                {
                    var errorLog = string.Format(LogStrings.Log_AmountTaxed_Error, data.IdOperation, data.AmountTotal);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 72;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                var bill = new BillDto()
                {
                    Amount = data.AmountTotal,
                    TaxedAmount = data.AmountTaxed,
                    BillExternalId = data.BillNumber,
                    Description = data.Description,
                    Currency = data.Currency.Equals("N", StringComparison.InvariantCultureIgnoreCase) ? "UYU" : "USD",
                    FinalConsumer = data.ConsFinal,
                    Gateway = GatewayEnumDto.Apps,
                    GeneratedDate = data.DateBill,
                    ExpirationDate = DateTime.Now,
                    Payable = true,
                    GatewayTransactionId = data.IdOperation,
                    DontApplyDiscount = data.Indi < 1,
                };

                //Se obtiene la asociacion que puede ser de usuario recurrente (VonData) o usuario registrado (ServiceAssociated)
                IAssociationInfoDto associationDto = _serviceAssociationSelector.FindServiceByIdAppAndExternalId(data.IdApp, data.IdUser, data.IdCard);

                if (associationDto == null)
                {
                    var errorLog = string.Format(LogStrings.Log_ServiceNotFound, data.IdUser);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 63;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                bool isRegisteredUser = associationDto.GetType() == typeof(ServiceAssociatedDto);

                if (isRegisteredUser)
                {
                    //Usuario registrado
                    if (associationDto.RegisteredUserDto == null || associationDto.RegisteredUserDto.MembershipIdentifierObj == null)
                    {
                        var errorLog = string.Format(LogStrings.User_ErrorLoadin, data.IdUser, data.IdOperation);
                        _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                        resultData.OperationResult = 66;
                        EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                        return resultData;
                    }

                    if (associationDto.RegisteredUserDto.MembershipIdentifierObj.Blocked)
                    {
                        var errorLog = string.Format(LogStrings.User_Blocked, data.IdUser, data.IdOperation);
                        _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                        resultData.OperationResult = 67;
                        EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                        return resultData;
                    }
                }
                else
                {
                    //Usuario recurrente
                    var vonAssociation = (VonDataAssociationDto)associationDto;
                    if (vonAssociation.AnonymousUserDto == null)
                    {
                        var errorLog = string.Format(LogStrings.User_ErrorLoadin, data.IdUser, data.IdOperation);
                        _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                        resultData.OperationResult = 66;
                        EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                        return resultData;
                    }
                }

                var card = associationDto.GetCardFromExternalId(data.IdCard);

                if (card == null)
                {
                    //NO SE ENCONTRO TARJETA ENVIADA PARA COBRAR
                    var errorLog = string.Format(LogStrings.Log_Card_Not_Found, data.IdCard);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 64;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                if (isRegisteredUser)
                {
                    //No se por que donde estaba antes que para usuario registrado iba a buscar a la BD asi que lo deje
                    card = _serviceCard.GetById(card.Id);
                }

                var info = new CheckBillInsertedDto()
                {
                    ServiceId = serviceToPay.Id,
                    SucivePreBillNumber = string.Empty,
                    BillExternalId = bill.BillExternalId,
                    GatewayEnum = GatewayEnumDto.Apps,
                };

                if (_servicePayment.BillAlreadyPaid(info))
                {
                    //FACTURA YA PAGA
                    var errorLog = string.Format(LogStrings.Bill_Already_Paid, bill.BillExternalId);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 8;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                var bin = _serviceBin.Find(card.BIN);

                var discountQuery = new DiscountQueryDto
                {
                    Bills = new List<BillDto>() { bill },
                    BinNumber = int.Parse(card.MaskedNumber.Substring(0, 6)),
                    ServiceId = associationDto.ServiceDto.Container ? serviceToPay.Id : associationDto.ServiceDto.Id,
                };

                //Obtengo los valores con los descuentos correspondientes
                List<CyberSourceExtraDataDto> discountValuesList = null;
                try
                {
                    discountValuesList = _serviceDiscountCalculator.Calculate(discountQuery);
                }
                catch (BusinessException exception)
                {
                    var errorLog = string.Format(exception.Message + ". Tarjeta: {0}", data.IdCard);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    //Si el servicio no acepta el tipo de tarjeta ahora devuelve 59: Error en calculo de descuento, pero no hay codigo especifico
                    resultData.OperationResult = 59;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }
                catch (FatalException exception)
                {
                    var errorLog = string.Format(exception.Message + ". Tarjeta: {0}", data.IdCard);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 59;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }
                catch (Exception exception)
                {
                    var errorLog = string.Format(exception.Message + ". Tarjeta: {0}", data.IdCard);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 59;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                var discountValues = discountValuesList.FirstOrDefault();

                if (discountValues == null)
                {
                    var errorLog = string.Format(LogStrings.Log_DiscountError);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 1;
                    EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                    return resultData;
                }

                //agrego el id operacion para enviarlo como MDD
                discountValues.OperationId = data.IdOperation;

                var serviceGateway = _serviceService.GetGateway(bill.Gateway);

                //TENGO QUE GENERAR UN ID PARA EL PAYMENT PARTICULAR PARA SISTARBANC
                var paymentDto = new PaymentDto
                {
                    Bills = new List<BillDto>() { discountValues.BillDto },
                    CardId = card.Id,
                    Card = card,
                    Date = DateTime.Now,
                    ReferenceNumber = associationDto.ReferenceNumber,
                    ReferenceNumber2 = associationDto.ReferenceNumber2,
                    ReferenceNumber3 = associationDto.ReferenceNumber3,
                    ReferenceNumber4 = associationDto.ReferenceNumber4,
                    ReferenceNumber5 = associationDto.ReferenceNumber5,
                    ReferenceNumber6 = associationDto.ReferenceNumber6,
                    ServiceDto = serviceToPay,
                    ServiceId = serviceToPay.Id,
                    PaymentType = PaymentTypeDto.App,
                    Currency = bill.Currency,
                    Discount = discountValues.BillDto.DiscountAmount, //se debe ingresar el discountamount
                    DiscountApplyed = discountValues.BillDto.DiscountAmount > 0,
                    TotalAmount = discountValues.BillDto.Amount,
                    TotalTaxedAmount = discountValues.BillDto.TaxedAmount,
                    AmountTocybersource = discountValues.CybersourceAmount,
                    PaymentPlatform = PaymentPlatformDto.Apps,
                    DiscountObjId = discountValues.DiscountDto != null ? discountValues.DiscountDto.Id : Guid.Empty,
                    DiscountObj = discountValues.DiscountDto,
                    GatewayId = serviceGateway.Id,
                    GatewayEnum = bill.Gateway,
                    Quotas = data.Quota < 1 ? 1 : data.Quota,
                };

                paymentDto = associationDto.FillPaymentServiceAndUserData(paymentDto);

                if (paymentDto.AmountTocybersource > 0)
                {
                    var paymentCsData = new GeneratePayment
                    {
                        ApplicationUserId = associationDto.UserId,
                        Currency = bill.Currency,
                        Token = card.PaymentToken,
                        TransaccionId = Guid.NewGuid().ToString(),
                        GrandTotalAmount = paymentDto.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        MerchandId = serviceToPay.MerchantId,
                        Key = serviceToPay.CybersourceTransactionKey,
                        UserType = LogUserType.Registered,
                        PaymentPlatform = PaymentPlatformDto.Apps,
                        CustomerIp = data.CustomerIp,
                        DeviceFingerprint = data.DeviceFingerprint,
                        CustomerShippingAddresDto = data.CustomerShippingAddresDto,
                        Quota = paymentDto.Quotas,
                        AdditionalInfo = new AdditionalInfo()
                        {
                            DiscountLabelTypeDto = discountValues.DiscountDto != null ? discountValues.DiscountDto.DiscountLabel : DiscountLabelTypeDto.NoDiscount,
                            CardTypeDto = bin.CardType,
                            BinValue = bin.Value,
                        }
                    };

                    associationDto.DefaultCard = card; //esto no se actualiza. Es para el los MDD
                    associationDto.ServiceDto = serviceToPay; //esto no se actualiza. Es para los MDD

                    var paymentsCount = GetPaymentsCount(associationDto.UserId, associationDto.ServiceId, isRegisteredUser);

                    var merchantDefinedData = _serviceCyberSourceAccess.LoadMerchantDefinedData(associationDto, discountValues, paymentsCount);

                    if (!string.IsNullOrEmpty(paymentCsData.Token))
                    {
                        CyberSourceDataDto paymentData = null;
                        try
                        {
                            paymentData = _serviceCyberSourceAccess.GeneratePayment(paymentCsData, merchantDefinedData);
                        }
                        catch (QuotaNotAllowWithCardTypeException exception)
                        {
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment,
                                LogCommunicationType.WebService, exception.Message);
                            resultData.OperationResult = int.Parse(exception.ErrorCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }
                        catch (QuotaNotAllowInBankException exception)
                        {
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment,
                                LogCommunicationType.WebService, exception.Message);
                            resultData.OperationResult = int.Parse(exception.ErrorCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }
                        catch (QuotaNotAllowInServiceException exception)
                        {
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment,
                                LogCommunicationType.WebService, exception.Message);
                            resultData.OperationResult = int.Parse(exception.ErrorCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }
                        catch (BillAmountNotAllowException exception)
                        {
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment,
                            LogCommunicationType.WebService, exception.Message);
                            resultData.OperationResult = int.Parse(exception.ErrorCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }
                        catch (BillTaxedAmountNotAllow exception)
                        {
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment,
                            LogCommunicationType.WebService, exception.Message);
                            resultData.OperationResult = int.Parse(exception.ErrorCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }

                        if (paymentData == null)
                        {
                            resultData.OperationResult = 1;
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }

                        if (!paymentData.ReasonCode.Equals("100"))
                        {
                            resultData.OperationResult = int.Parse(paymentData.ReasonCode);
                            EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
                            return resultData;
                        }

                        //SI HAY ERROR AL GUARDAR, ROLLBACK EN CYBERSOURCE
                        try
                        {
                            paymentDto.TransactionNumber = paymentData.TransactionId;
                            paymentDto.CyberSourceData = paymentData;

                            var pIdenfifierDto = _paymentIdentifier.Create(new PaymentIdentifierDto { CyberSourceTransactionIdentifier = paymentData.TransactionId }, true);
                            paymentDto.PaymentIdentifierId = pIdenfifierDto.Id;
                            paymentDto.PaymentIdentifierDto = pIdenfifierDto;

                            var updatedDto = _servicePayment.Create(paymentDto, true);
                            paymentdone = true;
                            resultData.OperationResult = 0;
                            resultData.CsTransactionNumber = updatedDto.TransactionNumber;

                            //Se notifica al comercio a su url_transaccion
                            _servicePayment.NotifyExternalSourceNewPayment(updatedDto);

                            if (updatedDto != null)
                            {
                                data.Codresult = 0;
                                data.PaymentId = updatedDto.Id;
                                _serviceWsBillPaymentOnline.Edit(data);
                            }
                        }
                        catch (Exception exception)
                        {
                            NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration Exception - Despues de notificar a cybersource");
                            NLogLogger.LogAppsEvent(exception);
                            var errorLog = string.Format(LogStrings.Log_WebService_Integration_AfterCs_Exception, data.IdOperation, paymentData.TransactionId);
                            _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog, exception);
                            var cancellationResult = _servicePayment.CancelPaymentCybersource(new CancelPayment()
                            {
                                Amount = paymentCsData.GrandTotalAmount,
                                Currency = paymentCsData.Currency,
                                UserId = paymentCsData.ApplicationUserId,
                                IdTransaccion = paymentData.TransactionId,
                                PaymentPlatform = paymentDto.PaymentPlatform,
                                RequestId = paymentData.TransactionId,
                                UserType = paymentCsData.UserType,
                                Token = card.PaymentToken,
                                ServiceId = serviceToPay.Id,
                                ServiceDto = serviceToPay
                            });
                            if (!paymentdone) resultData.OperationResult = 1;

                        }
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration Exception");
                NLogLogger.LogAppsEvent(exception);
                var errorLog = string.Format(LogStrings.Log_WebService_Integration_Exception, data.IdOperation);
                _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog, exception);
                resultData.OperationResult = 1;
                if (!paymentdone) resultData.OperationResult = 1;

                EditWsBillPaymentOnlineDto(data, resultData.OperationResult);
            }
            return resultData;
        }

        public TransactionResult CancelPayment(WsPaymentCancellationDto data)
        {
            var resultData = new TransactionResult();
            PaymentDto payment = null;
            CyberSourceOperationData cyberSourceOperationData = null;
            var now = DateTime.Now;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration CancelPayment: idOperacion {0}, id app {1}", data.IdOperation, data.IdApp));

                var strLog = string.Format(LogStrings.IntegrationService_Cancel_Init, data.IdApp, data.IdOperation);
                _loggerService.CreateLog(LogType.Info, LogOperationType.WebServicePayment, LogCommunicationType.WebService, strLog);

                //Si ya hay una operacion de cancelacion con resultado 0 no se hace nada
                var cancelationOp = _serviceWsPaymentCancellation.AllNoTracking(null, x => x.IdOperacionCobro.Equals(data.IdOperacionCobro)
                    && x.IdApp.Equals(data.IdApp)).OrderByDescending(x => x.CreationDate);
                if (cancelationOp.Any() && cancelationOp.Any(x => x.Codresult == 0))
                {
                    resultData.OperationResult = 62;
                    EditWsPaymentCancellationDto(data, resultData.OperationResult);
                    return resultData;
                }

                var operation = _serviceWsBillPaymentOnline.GetByIdOperation(data.IdOperacionCobro, data.IdApp);
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
                var errorLog = string.Format(LogStrings.Log_WebService_Integration_Exception, data.IdOperation);
                _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog, exception);
                resultData.OperationResult = 1;
            }
            EditWsPaymentCancellationDto(data, resultData.OperationResult);
            return resultData;
        }

        public WebServiceTransactionHistoryDto TransactionsHistory(WsBillQueryDto dto)
        {
            var result = new WebServiceTransactionHistoryDto()
            {
                ResumenPagos = new ResumenPagosDto(),
                EstadoFacturas = new List<TransactionHistoryDto>()
            };

            var list = _servicePayment.TransactionsHistoryForWebService(dto);

            result.EstadoFacturas = list.Select(x => new TransactionHistoryDto()
            {
                Estado = (x.Estado == 0 || x.Estado == (int)PaymentStatus.Processed) ? 0 : 1,
                NroFactura = x.NroFactura,
                Moneda = x.Moneda,
                RefCliente1 = x.RefCliente,
                RefCliente2 = x.RefCliente2,
                RefCliente3 = x.RefCliente3,
                RefCliente4 = x.RefCliente4,
                RefCliente5 = x.RefCliente5,
                RefCliente6 = x.RefCliente6,
                MontoTotal = x.MontoTotal,
                MontoDescIVA = x.MontoDescIVA,
                CantCuotas = x.CantCuotas,
                FchPago = x.FchPago,
                AuxiliarData = new List<HighwayAuxiliarDataDto>()
            }).ToList();

            result.ResumenPagos.CantFacturas = list.Count;
            result.ResumenPagos.SumaDolaresPagados = list.Where(x => x.Moneda.Equals("USD") && (x.Estado == (int)PaymentStatus.Done || x.Estado == (int)PaymentStatus.Processed)).Sum(x => x.MontoTotal);
            result.ResumenPagos.CantDolaresPagados = list.Count(x => x.Moneda.Equals("USD") && (x.Estado == (int)PaymentStatus.Done || x.Estado == (int)PaymentStatus.Processed));
            result.ResumenPagos.SumaPesosPagados = list.Where(x => x.Moneda.Equals("UYU") && (x.Estado == (int)PaymentStatus.Done || x.Estado == (int)PaymentStatus.Processed)).Sum(x => x.MontoTotal);
            result.ResumenPagos.CantPesosPagados = list.Count(x => x.Moneda.Equals("UYU") && (x.Estado == (int)PaymentStatus.Done || x.Estado == (int)PaymentStatus.Processed));

            return result;
        }

        public TransactionCommerceResult GetServices(WsCommerceQueryDto dto)
        {
            return _serviceService.GetServicesFromFather(dto.IdApp);
        }

        public TransactionResult RemoveCard(WsCardRemoveDto data)
        {
            var resultData = new TransactionResult();
            var result = false;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info,
                    string.Format("Service Integration RemoveCard: idOperacion {0}, id app {1}", data.IdOperation,
                        data.IdApp));

                var strLog = string.Format(LogStrings.IntegrationService_CardRemove_Init, data.IdApp, data.IdOperation);
                _loggerService.CreateLog(LogType.Info, LogOperationType.WebServiceCardRemove,
                    LogCommunicationType.WebService, strLog);

                //Se obtiene la asociacion que puede ser de usuario recurrente (VonData) o usuario registrado (ServiceAssociated)
                IAssociationInfoDto associationDto =
                    _serviceAssociationSelector.FindServiceByIdAppAndExternalId(data.IdApp, data.IdUser, data.IdCard);

                if (associationDto == null)
                {
                    var errorLog = string.Format(LogStrings.Log_ServiceNotFound, data.IdUser);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServiceCardRemove,
                        LogCommunicationType.WebService, errorLog);
                    resultData.OperationResult = 63;
                    EditWsCardRemoveDto(data, resultData.OperationResult);
                    return resultData;
                }

                if (!string.IsNullOrEmpty(data.IdCard))
                {
                    //ELIMINO TARJETA
                    NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration RemoveCard - Eliminar tarjeta");
                    var card = associationDto.GetCardFromExternalId(data.IdCard);

                    if (card == null)
                    {
                        //NO SE ENCONTRO TARJETA ENVIADA PARA COBRAR
                        var errorLog = string.Format(LogStrings.Log_Card_Not_Found, data.IdCard);
                        _loggerService.CreateLog(LogType.Error, LogOperationType.WebServiceCardRemove,
                            LogCommunicationType.WebService, errorLog);
                        resultData.OperationResult = 64;
                        EditWsCardRemoveDto(data, resultData.OperationResult);
                        return resultData;
                    }

                    result = _serviceAssociationSelector.DeleteAssociationCard(associationDto, card);
                }
                else
                {
                    //ELIMINO ASOCIACION
                    NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration RemoveCard - Eliminar asociacion");
                    result = _serviceAssociationSelector.DeleteAssociation(associationDto);
                }

                //Si viene false en result significa que ocurrio un error por lo que mando la excepcion para que se procese en el catch de abajo
                if (!result)
                {
                    throw new Exception();
                }

                resultData.OperationResult = 0;
                EditWsCardRemoveDto(data, resultData.OperationResult);
            }
            catch (ServiceAssociatedWithoutCardException exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration Exception");
                NLogLogger.LogAppsEvent(exception);
                var errorLog = string.Format(LogStrings.Log_WebService_Integration_Exception, data.IdOperation);
                _loggerService.CreateLog(LogType.Error, LogOperationType.WebServiceCardRemove, LogCommunicationType.WebService, errorLog, exception);
                resultData.OperationResult = int.Parse(exception.ErrorCode);
                EditWsCardRemoveDto(data, resultData.OperationResult);
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration Exception");
                NLogLogger.LogAppsEvent(exception);
                var errorLog = string.Format(LogStrings.Log_WebService_Integration_Exception, data.IdOperation);
                _loggerService.CreateLog(LogType.Error, LogOperationType.WebServiceCardRemove, LogCommunicationType.WebService, errorLog, exception);
                resultData.OperationResult = 1;
                EditWsCardRemoveDto(data, resultData.OperationResult);
            }
            return resultData;
        }

        public IList<WebhookNewAssociationDto> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto)
        {
            return _serviceWebhookNewAssociation.GetUrlTransacctionPosts(dto);
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
                        "El servicio {0} realizo la cancelación de la transacción {1} realizada el {2}", serviceName, payment.TransactionNumber, dateString);

            //Email notification
            if (payment.PaymentPlatform == PaymentPlatformDto.Apps)
            {
                _serviceNotificationMessage.SendPaymentDoneCancellation(email, mailDesc, payment.TransactionNumber,
                    dateString, payment.AmountTocybersource.ToString("##,#0.00", CultureInfo.CurrentCulture), cancellationTransaccionId,
                    now.ToString("dd/MM/yyyy hh:mm"), cancellationAmount);
            }

            //Portal notification
            if (isRegisteredUser)
            {
                _serviceNotification.Create(new NotificationDto
                {
                    Date = DateTime.Now,
                    Message = mailDesc,
                    NotificationPrupose = NotificationPruposeDto.SueccessNotification,
                    RegisteredUserId = payment.RegisteredUserId.Value,
                    ServiceId = payment.ServiceId,
                });
            }
        }

        private void EditWsBillPaymentOnlineDto(WsBillPaymentOnlineDto data, int codResult)
        {
            try
            {
                data.Codresult = codResult;
                _serviceWsBillPaymentOnline.Edit(data);
                //NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration - MakePayment - Error {0}, Idapp {1}. Operacion: {2}, Monto total: {3}, Monto gravado: {4}",
                //    codResult, data.IdApp, data.IdOperation, data.AmountTotal, data.AmountTaxed));
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration Payment LOG - Exception");
                NLogLogger.LogAppsEvent(e);
            }
        }

        private void EditWsPaymentCancellationDto(WsPaymentCancellationDto data, int codResult)
        {
            try
            {
                data.Codresult = codResult;
                _serviceWsPaymentCancellation.Edit(data);
                //NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration - MakePayment - Error {0}, Idapp {1}. Operacion: {2}", 
                //    codResult, data.IdApp, data.IdOperation));
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration CancelPayment LOG - Exception");
                NLogLogger.LogAppsEvent(e);
            }
        }

        private void EditWsCardRemoveDto(WsCardRemoveDto data, int codResult)
        {
            try
            {
                data.Codresult = codResult;
                _serviceWsCardRemove.Edit(data);
                //NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Service Integration - MakePayment - Error {0}, Idapp {1}. Operacion: {2}",
                //    codResult, data.IdApp, data.IdOperation));
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "Service Integration RemoveCard LOG - Exception");
                NLogLogger.LogAppsEvent(e);
            }
        }

        private int GetPaymentsCount(Guid userId, Guid serviceId, bool isRegisteredUser)
        {
            var paymentsCount = isRegisteredUser ?
                _servicePayment.CountPaymentsDone(userId, Guid.Empty, serviceId) :
                _servicePayment.CountPaymentsDone(Guid.Empty, userId, serviceId);
            return paymentsCount;
        }

    }
}