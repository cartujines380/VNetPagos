using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CyberSource;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class PaymentHandler : IPaymentHandler
    {
        private readonly IServiceDiscountCalculator _serviceDiscountCalculator;
        private readonly IServicePayment _servicePayment;
        private readonly IWebApiTransactionContext _webApiTransactionContext;
        private readonly ILoggerService _loggerService;
        private readonly IServiceService _serviceService;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly IServiceParameters _serviceParameters;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IServiceCard _serviceCard;
        private readonly IRepositoryConciliationCybersource _repositoryConciliationCybersource;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IServiceBank _serviceBank;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceVonData _serviceVonData;
        private readonly IRepositoryCard _repositoryCard;

        public PaymentHandler(IServiceDiscountCalculator serviceDiscountCalculator, IServicePayment servicePayment,
            IWebApiTransactionContext webApiTransactionContext, ILoggerService loggerService, IServiceService serviceService,
            IServiceFixedNotification serviceFixedNotification, IServiceApplicationUser serviceApplicationUser,
            IServiceAnonymousUser serviceAnonymousUser, IServiceParameters serviceParameters, IRepositoryPayment repositoryPayment,
            IRepositoryConciliationCybersource repositoryConciliationCybersource,
            ILoggerHelper loggerHelper, IServiceBank serviceBank, IServiceBin serviceBin, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IRepositoryCard repositoryCard)
        {
            _serviceDiscountCalculator = serviceDiscountCalculator;
            _servicePayment = servicePayment;
            _webApiTransactionContext = webApiTransactionContext;
            _loggerService = loggerService;
            _serviceService = serviceService;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceAnonymousUser = serviceAnonymousUser;
            _serviceParameters = serviceParameters;
            _repositoryPayment = repositoryPayment;
            
            _repositoryConciliationCybersource = repositoryConciliationCybersource;
            _loggerHelper = loggerHelper;
            _serviceBank = serviceBank;
            _serviceBin = serviceBin;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _serviceVonData = serviceVonData;
            _repositoryCard = repositoryCard;
        }

        public PaymentResultTypeDto Pay(BillDto bill, ServiceAssociatedDto serviceAssociatedDto)
        {
            PaymentResultTypeDto result;
            try
            {
                RefreshTransactionData(serviceAssociatedDto.UserId);

                var cyberSourceExtraData = new CyberSourceExtraDataDto();

                //Intento calcular descuento
                PaymentDto payment = null;
                try
                {
                    _loggerHelper.LogCalculatingDiscount(serviceAssociatedDto, bill);
                    payment = CalculateDiscount(bill, serviceAssociatedDto, ref cyberSourceExtraData);
                }
                catch (BusinessException e)
                {
                    if (e.Code == CodeExceptions.DISCOUNT_INVALID_MODEL)
                    {
                        result = PaymentResultTypeDto.InvalidModel;
                        _loggerHelper.LogPayBillInvalidModel(serviceAssociatedDto, bill);
                    }
                    else if (e.Code == CodeExceptions.BIN_NOTVALID_FOR_SERVICE)
                    {
                        result = PaymentResultTypeDto.BinNotValidForService;
                        _loggerHelper.LogPayBillBinNotValidForService(serviceAssociatedDto, bill);
                    }
                    else
                    {
                        result = PaymentResultTypeDto.DiscountCalculationError;
                        _loggerHelper.LogDiscountCalculationException(e, serviceAssociatedDto);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    result = PaymentResultTypeDto.DiscountCalculationError;
                    _loggerHelper.LogDiscountCalculationException(e, serviceAssociatedDto);
                    return result;
                }

                //Se obtuvo descuento y continuo con el intento de pago
                if (payment.AmountTocybersource > 0)
                {
                    _loggerHelper.LogCallingCybersource(serviceAssociatedDto, bill);
                    var paymentData = CallCybersource(payment, serviceAssociatedDto, cyberSourceExtraData);
                    payment.TransactionNumber = paymentData != null ? paymentData.TransactionId : null;
                    payment.CyberSourceData = paymentData;

                    result = ProcessPaymentResult(payment, paymentData);

                    if (result == PaymentResultTypeDto.Success)
                    {
                        //Pago exitoso
                        _loggerHelper.LogPayBillSuccess(serviceAssociatedDto, bill);
                        serviceAssociatedDto.AutomaticPaymentDto.QuotasDone += 1;
                        _serviceServiceAssosiate.AutomaticPaymentAddQuotasDone(serviceAssociatedDto.AutomaticPaymentDto);
                    }
                }
                else
                {
                    result = PaymentResultTypeDto.AmountIsZeroError;
                }
            }
            catch (BusinessException e)
            {
                if (e.Code == CodeExceptions.BANK_DONOT_ALLOW_QUOTA)
                {
                    result = PaymentResultTypeDto.BankDontAllowQuota;
                    _loggerHelper.LogPayBillBankDontAllowQuota(serviceAssociatedDto, bill);
                }
                else
                {
                    result = PaymentResultTypeDto.UnhandledException;
                    _loggerHelper.LogException(e, serviceAssociatedDto.Id);
                }
            }
            catch (Exception e)
            {
                _loggerHelper.LogException(e, serviceAssociatedDto.Id);
                result = PaymentResultTypeDto.UnhandledException;
            }
            return result;
        }

        private PaymentDto CalculateDiscount(BillDto bill, ServiceAssociatedDto serviceAssociatedDto,
            ref CyberSourceExtraDataDto cyberSourceExtraData)
        {
            try
            {
                //Calculo y registro los descuentos aplicados, en el caso de tarjetas de debito
                var payBills = new List<BillDto> { bill };
                var binNumber = Int32.Parse(serviceAssociatedDto.DefaultCard.MaskedNumber.Substring(0, 6));

                //obtengo la moneda de las facturas que se van a pagar
                //las facturas deben ser todas de la misma moneda.

                var currency = bill.Currency;
                var discountQuery = new DiscountQueryDto
                {
                    Bills = payBills,
                    BinNumber = binNumber,
                    ServiceId = serviceAssociatedDto.ServiceDto.Id
                };

                //obtengo los valores con los descuentos correspondientes
                var cyberSourceExtraDataList = _serviceDiscountCalculator.Calculate(discountQuery);
                cyberSourceExtraData = cyberSourceExtraDataList.FirstOrDefault();

                var payment = new PaymentDto
                {
                    Bills = new List<BillDto>() { cyberSourceExtraData.BillDto },
                    CardId = serviceAssociatedDto.DefaultCardId,
                    Card = serviceAssociatedDto.DefaultCard,
                    Date = DateTime.Now,
                    ReferenceNumber = serviceAssociatedDto.ReferenceNumber,
                    ReferenceNumber2 = serviceAssociatedDto.ReferenceNumber2,
                    ReferenceNumber3 = serviceAssociatedDto.ReferenceNumber3,
                    ReferenceNumber4 = serviceAssociatedDto.ReferenceNumber4,
                    ReferenceNumber5 = serviceAssociatedDto.ReferenceNumber5,
                    ReferenceNumber6 = serviceAssociatedDto.ReferenceNumber6,
                    ServiceAssociatedId = serviceAssociatedDto.Id,
                    ServiceAssociatedDto = serviceAssociatedDto,
                    ServiceDto = serviceAssociatedDto.ServiceDto,
                    ServiceId = serviceAssociatedDto.ServiceId,
                    RegisteredUserId = serviceAssociatedDto.UserId,
                    RegisteredUser = serviceAssociatedDto.RegisteredUserDto,
                    PaymentType = PaymentTypeDto.Automatic,
                    Currency = currency,
                    Discount = cyberSourceExtraData.BillDto.DiscountAmount,//se debe ingresar el discountamount
                    DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0,
                    TotalAmount = cyberSourceExtraData.BillDto.Amount,
                    TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount,
                    AmountTocybersource = cyberSourceExtraData.CybersourceAmount,
                    PaymentPlatform = PaymentPlatformDto.VisaNet,
                    DiscountObjId = cyberSourceExtraData.DiscountDto != null ? cyberSourceExtraData.DiscountDto.Id : Guid.Empty,
                    DiscountObj = cyberSourceExtraData.DiscountDto,
                };

                cyberSourceExtraData.BinNumber = int.Parse(payment.Card.MaskedNumber.Substring(0, 6));
                cyberSourceExtraData.CallcenterUser = String.Empty;

                NLogLogger.LogEvent(NLogType.Info, "      Proceso Pago Automático -- Llamo a metodo para pagar por medio del WCF Cybersource, servicio asociado: " +
                    serviceAssociatedDto.Id);

                return payment;
            }
            catch (BusinessException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private CyberSourceDataDto CallCybersource(PaymentDto paymentDto, ServiceAssociatedDto serviceAssociatedDto,
            CyberSourceExtraDataDto cyberSourceExtraData)
        {
            var tryCounter = 5;
            var exit = false;
            CyberSourceDataDto data = null;
            var transactionId = Guid.NewGuid();

            var bin = _serviceBin.FindByGuid(serviceAssociatedDto.DefaultCardId);

            while (tryCounter > 0 && exit == false)
            {
                try
                {
                    var payment = new GeneratePayment
                    {
                        ApplicationUserId = serviceAssociatedDto.UserId,
                        Currency = paymentDto.Currency,
                        MerchandId = serviceAssociatedDto.ServiceDto.MerchantId,
                        Token = serviceAssociatedDto.DefaultCard.PaymentToken,
                        TransaccionId = transactionId.ToString(),
                        GrandTotalAmount = paymentDto.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Key = serviceAssociatedDto.ServiceDto.CybersourceTransactionKey,
                        UserType = LogUserType.Other,
                        PaymentPlatform = PaymentPlatformDto.VisaNet,
                        AdditionalInfo = new AdditionalInfo
                        {
                            BinValue = bin.Value,
                            CardTypeDto = bin.CardType,
                            DiscountLabelTypeDto = paymentDto.DiscountObj != null ? paymentDto.DiscountObj.DiscountLabel : DiscountLabelTypeDto.NoDiscount,
                        },
                        Quota = 1,
                    };

                    var merchantDefinedData = LoadMerchantDefinedData(serviceAssociatedDto, cyberSourceExtraData);

                    if (!String.IsNullOrEmpty(payment.Token))
                    {
                        var access = new CyberSourceAccess(_loggerService, _serviceService,
                            _serviceFixedNotification, _serviceApplicationUser,
                            _serviceAnonymousUser, _serviceParameters,
                            _repositoryPayment, _repositoryCard,
                            _repositoryConciliationCybersource, _serviceBank, _serviceVonData, _serviceBin);
                        data = access.GeneratePayment(payment, merchantDefinedData);
                        exit = true;
                    }
                    tryCounter--;
                }
                catch (CybersourceException e)
                {
                    tryCounter--;
                }
            }
            return data;
        }

        private PaymentResultTypeDto ProcessPaymentResult(PaymentDto payment, CyberSourceDataDto paymentData)
        {
            if (paymentData != null)
            {
                var paymentReasonCode = (PaymentResultTypeDto)int.Parse(paymentData.ReasonCode);
                if (paymentReasonCode == PaymentResultTypeDto.Success)
                {
                    _loggerHelper.LogCallCybersourceSuccess(payment, paymentData);

                    var paymentDto = NotifyGateway(payment);

                    //Si el payment es null entonces se produjo un error en la notificación por las pasarelas
                    //y se realizó el rollback de la transacción en cybersource
                    if (paymentDto.NewPaymentDto == null)
                    {
                        //Error al intentar notificar a las pasarelas
                        _loggerHelper.LogGatewayNotificationError((Guid)payment.ServiceAssociatedId);
                        return PaymentResultTypeDto.GatewayNotificationError;
                    }

                    //Se deja comentado que notifique al ente externo porque ahora no hay ni IdOperacion ni NroFactura, entonces no hace sentido la notificacion
                    //_servicePayment.NotifyExternalSourceNewPayment(paymentDto.NewPaymentDto);
                }
                return paymentReasonCode;
            }
            Console.WriteLine("No se pudo realizar el pago contra Cybresource");
            return PaymentResultTypeDto.PaymentGeneralError;
        }

        private CybersourceCreatePaymentDto NotifyGateway(PaymentDto payment)
        {
            return _servicePayment.NotifyGateways(payment);
        }

        private void RefreshTransactionData(Guid applicationUserId)
        {
            _webApiTransactionContext.TransactionIdentifier = Guid.NewGuid();
            _webApiTransactionContext.TransactionDateTime = DateTime.Now;
            _webApiTransactionContext.ApplicationUserId = applicationUserId.ToString();
        }

        private CyberSourceMerchantDefinedDataDto LoadMerchantDefinedData(ServiceAssociatedDto serviceAssociated,
            CyberSourceExtraDataDto cyberSourceExtraData)
        {
            var paymentsCount = _servicePayment.CountPaymentsDone(serviceAssociated.UserId, Guid.Empty, serviceAssociated.ServiceId);

            var merchantDefinedData = new CyberSourceMerchantDefinedDataDto
            {
                ServiceType = serviceAssociated.ServiceDto.ServiceCategory.Name,
                OperationTypeDto = OperationTypeDto.UniquePayment.ToString(),
                UserRegistered = "Y",
                UserRegisteredDays = (DateTime.Now.Date - serviceAssociated.RegisteredUserDto.CreationDate.Date).Days.ToString(),
                ReferenceNumber1 = serviceAssociated.ReferenceNumber,
                ReferenceNumber2 = serviceAssociated.ReferenceNumber2,
                ReferenceNumber3 = serviceAssociated.ReferenceNumber3,
                ReferenceNumber4 = serviceAssociated.ReferenceNumber4,
                ReferenceNumber5 = serviceAssociated.ReferenceNumber5,
                ReferenceNumber6 = serviceAssociated.ReferenceNumber6,
                RedirctTo = 0.ToString(),
                DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0 ? ((int)cyberSourceExtraData.DiscountDto.DiscountLabel).ToString() : "0",
                TotalAmount = cyberSourceExtraData.BillDto.Amount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                Discount = cyberSourceExtraData.BillDto.DiscountAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                BillNumber = cyberSourceExtraData.BillDto.BillExternalId,
                AditionalNumberElectornicBill = "",
                NameTh = serviceAssociated.DefaultCard.Name,
                PaymentCount = paymentsCount,
                UserCi = serviceAssociated.RegisteredUserDto.IdentityNumber,
                UserMobile = serviceAssociated.RegisteredUserDto.MobileNumber,
                UserRegisteredAddress = serviceAssociated.RegisteredUserDto.Address,
                MerchandId = serviceAssociated.ServiceDto.MerchantId,
                CardBin = cyberSourceExtraData.BinNumber.ToString(),
                ServiceName = serviceAssociated.ServiceDto.Name,
                CallcenterUser = cyberSourceExtraData.CallcenterUser,
                ServiceId = serviceAssociated.ServiceDto.Id,
                Plataform = PaymentPlatformDto.VisaNet.ToString(),
                PaymentTypeDto = PaymentTypeDto.Automatic,
                BillExpirationDate = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.ExpirationDate.ToString("dd-MM-yyyy") : string.Empty,
                BillDateInitTransaccion = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.DateInitTransaccion : string.Empty,
                BillDescription = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.Description : string.Empty,
                BillDiscount = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.Discount.ToString() : string.Empty,
                BillFinalConsumer = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.FinalConsumer.ToString() : string.Empty,
                BillGatewayTransactionBrouId = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.GatewayTransactionBrouId : string.Empty,
                BillGatewayTransactionId = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.GatewayTransactionId : string.Empty,
                BillSucivePreBillNumber = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.SucivePreBillNumber : string.Empty,
                DiscountObjId = cyberSourceExtraData.DiscountDto != null ? cyberSourceExtraData.DiscountDto.Id : Guid.Empty,
            };
            return merchantDefinedData;
        }

    }
}