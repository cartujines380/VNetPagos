using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServiceWsBank : IServiceWsBank
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceBill _serviceBill;
        private readonly IServiceDiscountCalculator _serviceDiscountCalculator;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly IServicePayment _servicePayment;
        private readonly ICyberSourceAccess _cyberSourceAccess;
        private readonly IServicePaymentIdentifier _paymentIdentifier;
        private readonly ILoggerService _loggerService;

        public ServiceWsBank(IServiceBill serviceBill, IServiceService serviceService, IServiceDiscountCalculator serviceDiscountCalculator, IServiceBin serviceBin,
            IServiceAnonymousUser serviceAnonymousUser, IServicePayment servicePayment, ICyberSourceAccess cyberSourceAccess, IServicePaymentIdentifier paymentIdentifier,
            ILoggerService loggerService)
        {
            _serviceBill = serviceBill;
            _serviceService = serviceService;
            _serviceDiscountCalculator = serviceDiscountCalculator;
            _serviceBin = serviceBin;
            _serviceAnonymousUser = serviceAnonymousUser;
            _servicePayment = servicePayment;
            _cyberSourceAccess = cyberSourceAccess;
            _paymentIdentifier = paymentIdentifier;
            _loggerService = loggerService;
        }

        #region Public Methods

        public IEnumerable<ServiceDto> AllServices()
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank AllServices - " + DateTime.Now.ToString("G"));

            var result = _serviceService.AllNoTracking(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
                Active = t.Active,
                ServiceCategoryName = t.ServiceCategory.Name,
                ReferenceParamName = t.ReferenceParamName,
                ReferenceParamName2 = t.ReferenceParamName2,
                ReferenceParamName3 = t.ReferenceParamName3,
                ReferenceParamName4 = t.ReferenceParamName4,
                ReferenceParamName5 = t.ReferenceParamName5,
                ReferenceParamName6 = t.ReferenceParamName6,
                CybersourceTransactionKey = t.CybersourceTransactionKey,
                MerchantId = t.MerchantId,
                //CreditCard = t.CreditCard,
                //DebitCard = t.DebitCard,
                //CreditCardInternational = t.CreditCardInternational,
                //DebitCardInternational = t.DebitCardInternational,
                ServiceGatewaysDto = t.ServiceGateways.Select(g => new ServiceGatewayDto
                {
                    Active = g.Active,
                    GatewayId = g.GatewayId,
                    ReferenceId = g.ReferenceId,
                    ServiceType = g.ServiceType,
                    Gateway = new GatewayDto()
                    {
                        Enum = g.Gateway.Enum
                    }
                }).ToList()
            }, s => s.Active && !s.ServiceGateways.Any(g => g.Active && (g.Gateway.Enum == (int)GatewayEnum.Apps || g.Gateway.Enum == (int)GatewayEnum.Importe)), s => s.ServiceGateways);
            return result;
        }

        public WsBankBillsResultDto GetBills(WsBankBillsInputDto dto)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank GetBills - " + DateTime.Now.ToString("G"));

                InsertBillsLog(dto);

                Guid serviceId;
                var isServiceId = Guid.TryParse(dto.ServiceId, out serviceId);

                if (!isServiceId)
                {
                    var message = "Error al validar los campos requeridos: Servicio inexistente.";
                    NLogLogger.LogEvent(NLogType.Info, message);
                    return new WsBankBillsResultDto(ErrorCodeDto.ERRORES_EN_LOS_CAMPOS_ENVIADOS)
                    {
                        ErrorMessage = message
                    };
                }

                var service = _serviceService.All(null, x => x.Id == serviceId,
                        s => s.ServiceCategory, x => x.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

                if (service == null)
                {
                    var message = "Error al validar los campos requeridos: Servicio inexistente.";
                    NLogLogger.LogEvent(NLogType.Info, message);
                    return new WsBankBillsResultDto(ErrorCodeDto.ERRORES_EN_LOS_CAMPOS_ENVIADOS)
                    {
                        ErrorMessage = message
                    };
                }

                var gateways = _serviceService.GetGateways();

                Guid gateId = default(Guid);
                if (dto.GatewayEnumDto != 0)
                {
                    gateId = gateways.FirstOrDefault(x => x.Enum == (int)dto.GatewayEnumDto).Id;
                    if (service.ServiceGatewaysDto.Where(g => g.Active).All(g => g.GatewayId != gateId))
                    {
                        var message = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.LA_PASARELA_INGRESADA_NO_ESTA_ASOCIADA_AL_SERVICIO, EnumsStrings.ResourceManager);
                        NLogLogger.LogEvent(NLogType.Info, message);
                        return new WsBankBillsResultDto(ErrorCodeDto.LA_PASARELA_INGRESADA_NO_ESTA_ASOCIADA_AL_SERVICIO)
                        {
                            ErrorMessage = message
                        };
                    }
                }

                var references = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(service.ReferenceParamName))
                    references.Add(service.ReferenceParamName, dto.ServiceReferenceNumber);
                if (!string.IsNullOrEmpty(service.ReferenceParamName2))
                    references.Add(service.ReferenceParamName2, dto.ServiceReferenceNumber2);
                if (!string.IsNullOrEmpty(service.ReferenceParamName3))
                    references.Add(service.ReferenceParamName3, dto.ServiceReferenceNumber3);
                if (!string.IsNullOrEmpty(service.ReferenceParamName4))
                    references.Add(service.ReferenceParamName4, dto.ServiceReferenceNumber4);
                if (!string.IsNullOrEmpty(service.ReferenceParamName5))
                    references.Add(service.ReferenceParamName5, dto.ServiceReferenceNumber5);
                if (!string.IsNullOrEmpty(service.ReferenceParamName6))
                    references.Add(service.ReferenceParamName6, dto.ServiceReferenceNumber6);

                var bills =
                    _serviceBill.GetBillsForAnonymousUser(new AnonymousUserBillFilterDto()
                                          {
                                              ServiceId = service.Id,
                                              References = references,
                                              AnonymousUserDto = new AnonymousUserDto()
                                              {
                                                  Email = dto.UserEmail,
                                                  IdentityNumber = dto.UserCi,
                                                  Name = dto.UserName,
                                                  Surname = dto.UserSurname,
                                                  Address = dto.UserAddress
                                              }
                                          });

                return new WsBankBillsResultDto(ErrorCodeDto.OK)
                {
                    Bills = bills != null && bills.Bills != null ? bills.Bills.ToList() : new List<BillDto>(),
                    MerchantId = service.MerchantId,
                    ServiceType = service.ServiceCategory.Name,
                    MultipleBillsAllowed = service.EnableMultipleBills
                };
            }
            catch (BillException exception)
            {
                NLogLogger.LogEvent(exception);
                return new WsBankBillsResultDto(ErrorCodeDto.ERROR_DE_LA_PASARELA)
                {
                    ErrorMessage = exception.Message
                };
            }
            catch (ProviderFatalException exception)
            {
                NLogLogger.LogEvent(exception);
                return new WsBankBillsResultDto(ErrorCodeDto.ERROR_DE_LA_PASARELA)
                {
                    ErrorMessage = exception.Message
                };
            }
            catch (TimeoutException exception)
            {
                NLogLogger.LogEvent(exception);
                return new WsBankBillsResultDto(ErrorCodeDto.EL_TIEMPO_DE_RESPUESTA_DE_LA_PASARELA_SUPERO_EL_TIEMPO_DE_ESPERA_DEFINIDO)
                {
                    ErrorMessage = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.EL_TIEMPO_DE_RESPUESTA_DE_LA_PASARELA_SUPERO_EL_TIEMPO_DE_ESPERA_DEFINIDO, EnumsStrings.ResourceManager)
                };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                return new WsBankBillsResultDto(ErrorCodeDto.ERROR_GENERAL)
                {
                    ErrorMessage = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager)
                };
            }
        }

        public WsBankBillsResultDto PreprocessPayment(WsBankPreprocessPaymentInputDto dto)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank PreprocessPayment - " + DateTime.Now.ToString("G"));

            InsertPreprocessPaymentLog(dto);

            var gateway = dto.Bills.FirstOrDefault().Gateway;
            var serviceId = Guid.Parse(dto.ServiceId);
            var service = _serviceService.All(null, s => s.Id == serviceId, s => s.ServiceCategory, s => s.ServiceGateways,
                    s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

            //Si no es sucive o geocom devuelvo las mismas facturas que recibi
            if (gateway != GatewayEnumDto.Geocom && gateway != GatewayEnumDto.Sucive)
            {
                NLogLogger.LogEvent(NLogType.Info, "El servicio no es Sucive o Geocom, devuelvo las mismas facturas que recibi");
                return new WsBankBillsResultDto(ErrorCodeDto.OK)
                {
                    Bills = dto.Bills,
                    MerchantId = service.MerchantId,
                    ServiceType = service.ServiceCategory.Name,
                    MultipleBillsAllowed = service.EnableMultipleBills
                };
            }

            //Si es sucive o geocom, primero chequeo que tengan el mismo IdPadron
            var idPadron = dto.Bills.FirstOrDefault().IdPadron;
            if (dto.Bills.Any(b => b.IdPadron != idPadron))
            {
                NLogLogger.LogEvent(NLogType.Info, "El Id Padron de las facturas a pagar debe ser el mismo.");
                return new WsBankBillsResultDto(ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_TENER_EL_MISMO_ID_PADRON)
                {
                    ErrorMessage = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_TENER_EL_MISMO_ID_PADRON, EnumsStrings.ResourceManager)
                };
            }

            //Concateno las lineas
            var lines = dto.Bills.Select(m => m.Line).ToList();
            var line = String.Join("", lines);

            var serviceGateway =
                service.ServiceGatewaysDto.FirstOrDefault(g => g.Active && ((g.Gateway.Enum == (int)gateway)));

            //Llamo a CheckBills
            NLogLogger.LogEvent(NLogType.Info, "CheckBills");
            var billDto = _serviceBill.ChekBills(line, idPadron, (int)service.Departament, gateway, serviceGateway.ReferenceId);

            //Error de la pasarela
            if (billDto.SucivePreBillNumber.Equals("-1"))
            {
                NLogLogger.LogEvent(NLogType.Info, billDto.Description);
                return new WsBankBillsResultDto(ErrorCodeDto.ERROR_DE_LA_PASARELA_SUCIVE_O_GEOCOM)
                {
                    ErrorMessage = billDto.Description
                };
            }

            return new WsBankBillsResultDto(ErrorCodeDto.OK)
            {
                Bills = new List<BillDto> { billDto },
                MerchantId = service.MerchantId,
                ServiceType = service.ServiceCategory.Name,
                MultipleBillsAllowed = service.EnableMultipleBills
            };
        }

        public List<CyberSourceExtraDataDto> CalculateDiscount(WsBankPreprocessPaymentInputDto dto)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank CalculateDiscount - " + DateTime.Now.ToString("G"));

            var serviceId = Guid.Parse(dto.ServiceId);
            var service = _serviceService.All(null, s => s.Id == serviceId, s => s.ServiceGateways,
                    s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

            var result = _serviceDiscountCalculator.Calculate(new DiscountQueryDto()
            {
                Bills = dto.Bills,
                BinNumber = dto.CardBinNumbers,
                ServiceId = service.Id
            });

            return result;
        }

        public WsBankPaymentResultDto Payment(WsBankPaymentInputDto dto)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank Payment - " + DateTime.Now.ToString("G"));

            InsertPaymentLog(dto);

            var paymentDone = false;
            var userId = new Guid();
            var cybersourceTransactionKey = "";
            var merchantId = "";
            ServiceDto service = null;
            try
            {
                var bill = new BillDto
                {
                    Id = Guid.Parse(dto.BillId),
                    BillExternalId = dto.BillNumber,
                    ExpirationDate = DateTime.ParseExact(dto.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Currency = dto.Currency,
                    Description = dto.Description,
                    GatewayTransactionId = dto.GatewayTransactionId,
                    Payable = dto.Payable,
                    FinalConsumer = dto.FinalConsumer,
                    Amount = dto.TotalAmount,
                    TaxedAmount = dto.TotalTaxedAmount,
                    Discount = dto.Discount != 0.0 ? Convert.ToInt32(dto.TotalAmount / dto.Discount / 100) : 0,
                    DiscountAmount = dto.Discount,
                    Gateway = (GatewayEnumDto)Enum.Parse(typeof(GatewayEnumDto), dto.Gateway, true),
                    SucivePreBillNumber = dto.SucivePreBillNumber,
                    DateInitTransaccion = dto.TransactionCreationDate
                };

                var anonymousUser = _serviceAnonymousUser.All(null, x => x.Email == dto.Email).FirstOrDefault();
                if (anonymousUser == null)
                {
                    anonymousUser = _serviceAnonymousUser.Create(new AnonymousUserDto
                    {
                        Email = dto.Email,
                        IdentityNumber = dto.Ci,
                        Address = dto.Address,
                        Name = dto.Name,
                        Surname = dto.Surname
                    }, true);
                }
                userId = anonymousUser.Id;

                Guid serviceId;
                var isServiceId = Guid.TryParse(dto.ServiceId, out serviceId);

                if (!isServiceId)
                {
                    throw new ProviderWithoutConectionException("servicio null");
                }

                service = _serviceService.All(null, s => s.Id == serviceId, s => s.ServiceGateways,
                        s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

                if (service == null)
                    throw new ProviderWithoutConectionException("servicio null");
                if (service.ServiceGatewaysDto == null || !service.ServiceGatewaysDto.Any())
                    throw new ProviderWithoutConectionException("servicio sin pasarelas o nullas");
                if (service.ServiceGatewaysDto.First().Gateway == null || !service.ServiceGatewaysDto.Any())
                    throw new ProviderWithoutConectionException("servicio con pasarelas internas nulas");

                merchantId = service.MerchantId;
                cybersourceTransactionKey = service.CybersourceTransactionKey;

                //validar q pasarela q te pasa en la factura este activa en el servicio
                var serviceGateway =
                    service.ServiceGatewaysDto.FirstOrDefault(g => g.Active && ((g.Gateway.Enum == (int)bill.Gateway)));

                var payment = new PaymentDto
                {
                    Bills = new List<BillDto> { bill },
                    Date = DateTime.Now,
                    ReferenceNumber = dto.ServiceReferenceNumber,
                    ReferenceNumber2 = dto.ServiceReferenceNumber2,
                    ReferenceNumber3 = dto.ServiceReferenceNumber3,
                    ReferenceNumber4 = dto.ServiceReferenceNumber4,
                    ReferenceNumber5 = dto.ServiceReferenceNumber5,
                    ReferenceNumber6 = dto.ServiceReferenceNumber6,
                    AnonymousUserId = anonymousUser.Id,
                    AnonymousUser = anonymousUser,
                    PaymentType = PaymentTypeDto.AnonymousUser,
                    Currency = dto.Currency,
                    Discount = dto.Discount,
                    DiscountApplyed = dto.DiscountApplyed,
                    TotalAmount = dto.TotalAmount,
                    TotalTaxedAmount = dto.TotalTaxedAmount,
                    AmountTocybersource = dto.AmountToCybersource,
                    PaymentPlatform = PaymentPlatformDto.Itau,
                    Card = new CardDto
                    {
                        MaskedNumber = dto.CardMaskedNumber,
                        DueDate = DateTime.ParseExact(dto.CardDueDate, "MMyy", CultureInfo.InvariantCulture),
                        Name = dto.Name,
                        CybersourceTransactionId = dto.TransactionId
                    },
                    ServiceId = service.Id,
                    ServiceDto = service,
                    GatewayEnum = bill.Gateway,
                    TransactionNumber = dto.TransactionId,
                    CyberSourceData = new CyberSourceDataDto
                    {
                        AuthAmount = dto.AuthAmount,
                        AuthTime = dto.AuthTime,
                        AuthCode = dto.AuthCode,
                        AuthAvsCode = dto.AuthAvsCode,
                        AuthResponse = dto.AuthResponse,
                        AuthTransRefNo = dto.AuthTransRefNo,
                        Decision = dto.Decision,
                        BillTransRefNo = dto.BillTransRefNo,
                        PaymentToken = dto.PaymentToken,
                        ReasonCode = dto.ReasonCode,
                        ReqAmount = dto.ReqAmount,
                        ReqCurrency = dto.ReqCurrency,
                        TransactionId = dto.TransactionId,
                        ReqTransactionUuid = dto.ReqTransactionUuid,
                        ReqReferenceNumber = dto.ReqReferenceNumber,
                        ReqTransactionType = dto.ReqTransactionType,
                    },
                };

                if (dto.DiscountObjId != null && !dto.DiscountObjId.Equals(Guid.Empty.ToString()))
                {
                    payment.DiscountObjId = Guid.Parse(dto.DiscountObjId);
                }

                var pIdenfifierDto =
                    _paymentIdentifier.Create(
                        new PaymentIdentifierDto { CyberSourceTransactionIdentifier = dto.TransactionId }, true);
                payment.PaymentIdentifierId = pIdenfifierDto.Id;
                payment.PaymentIdentifierDto = pIdenfifierDto;

                var notify = new NotifyPaymentDto()
                {
                    UserType = LogUserType.NoRegistered,
                    GatewayEnum = bill.Gateway,
                    Name = payment.AnonymousUser.Name,
                    Surname = payment.AnonymousUser.Surname,
                    UserId = payment.AnonymousUserId.Value,
                    UserRegistred = false,
                    TransactionNumber = payment.PaymentIdentifierDto.UniqueIdentifier.ToString(),
                    ServiceType = serviceGateway.ServiceType,
                    ServiceGatewayReferenceId = serviceGateway.ReferenceId,
                    GatewayId = serviceGateway.GatewayId,
                    PossibleGateways = service.ServiceGatewaysDto,
                    Bills = payment.Bills,
                    References = new string[]
                    {
                        payment.ReferenceNumber, payment.ReferenceNumber2,
                        payment.ReferenceNumber3,
                        payment.ReferenceNumber4, payment.ReferenceNumber5,
                        payment.ReferenceNumber6,
                    },
                    Bin = dto.CardBinNumbers.ToString(),
                    ServiceDepartament = (int)service.Departament,
                    CybersourceTransactionNumber = dto.TransactionId
                };

                var billsUpdated = _servicePayment.MakePayment(notify);

                paymentDone = true;

                if (billsUpdated != null)
                {
                    payment.GatewayId = notify.GatewayId;
                    payment.Bills = billsUpdated;
                }
                else
                {
                    throw new Exception("Bills vacias");
                }

                payment = _servicePayment.Create(payment, true);

                return new WsBankPaymentResultDto(ErrorCodeDto.OK) { Payment = payment };
            }
            catch (ProviderWithoutConectionException ex)
            {

                NLogLogger.LogEvent(NLogType.Info, "Excepcion ServiceWsBank - WsBankPaymentResultDto");
                NLogLogger.LogEvent(ex);

                //Si se notifico al ente, solo envio mail
                if (paymentDone)
                {
                    var msg = String.Format("{0} Request ID:{1}, Fecha: {2}", EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_AL_INENTAR_GUARDAR_LA_TRANSACCION, EnumsStrings.ResourceManager),
                        dto.TransactionId, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    var data = JsonConvert.SerializeObject(new
                    {
                        Message = msg,
                        Title = "Error en PORTAL al guardar pago.",
                    });
                    _servicePayment.NotifyError(data);

                    NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank Payment - " + msg);

                    return new WsBankPaymentResultDto(ErrorCodeDto.ERROR_AL_INENTAR_GUARDAR_LA_TRANSACCION)
                    {
                        ErrorMessage = msg
                    };
                }
                else
                {
                    //SI O SI, HAY Q CANCELAR SIN IMPORTAR Q TIPO DE EXCEPTION SEA
                    var error = dto.TransactionId + " mascara:" + dto.CardMaskedNumber;
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Gateway,
                        LogUserType.NoRegistered,
                        userId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Must, error),
                        string.Format(LogStrings.CallCenter_Payment_Cancel, dto.TransactionId), ex);

                    var cancel = new CancelPayment();
                    cancel.UserId = userId;
                    //cancel.Token = entity.Card.PaymentToken;
                    cancel.RequestId = dto.TransactionId;
                    cancel.UserType = LogUserType.NoRegistered;
                    cancel.Amount = Double.Parse(dto.ReqAmount).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    cancel.UserEmail = dto.Email;
                    cancel.IdTransaccion = dto.TransactionId;
                    cancel.Currency = dto.Currency;
                    cancel.PaymentPlatform = PaymentPlatformDto.Itau;
                    cancel.ServiceId = Guid.Parse(dto.ServiceId);
                    cancel.ServiceDto = service;

                    if (Double.Parse(dto.ReqAmount) < 1)
                    {
                        if (dto.AmountToCybersource > 0)
                        {
                            cancel.Amount = dto.AmountToCybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")); ;
                        }
                    }

                    var result = _servicePayment.CancelPaymentCybersource(cancel);

                    return new WsBankPaymentResultDto(ErrorCodeDto.ERROR_AL_INENTAR_GENERAR_LA_TRANSACCION)
                    {
                        ErrorMessage = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_AL_INENTAR_GENERAR_LA_TRANSACCION, EnumsStrings.ResourceManager),
                        CyberSourceOperationData = result
                    };
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepcion ServiceWsBank - WsBankPaymentResultDto");
                NLogLogger.LogEvent(e);

                if (paymentDone)
                {
                    var msg =
                        String.Format(
                            "{0} Request ID:{1}, Fecha: {2}", EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_AL_INENTAR_GUARDAR_LA_TRANSACCION, EnumsStrings.ResourceManager),
                            dto.TransactionId, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    var data = JsonConvert.SerializeObject(new
                    {
                        Message = msg,
                        Title = "Error en PORTAL al guardar pago.",
                    });
                    _servicePayment.NotifyError(data);

                    NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank Payment - " + msg);

                    return new WsBankPaymentResultDto(ErrorCodeDto.ERROR_AL_INENTAR_GUARDAR_LA_TRANSACCION)
                    {
                        ErrorMessage = msg
                    };
                }
                else
                {
                    //SI O SI, HAY Q CANCELAR SIN IMPORTAR Q TIPO DE EXCEPTION SEA
                    var error = dto.TransactionId + " mascara:" + dto.CardMaskedNumber;
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Gateway,
                        LogUserType.NoRegistered,
                        userId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Must, error),
                        string.Format(LogStrings.CallCenter_Payment_Cancel, dto.TransactionId), e);

                    var cancel = new CancelPayment();
                    cancel.UserId = userId;
                    //cancel.Token = entity.Card.PaymentToken;
                    cancel.RequestId = dto.TransactionId;
                    cancel.UserType = LogUserType.NoRegistered;
                    cancel.Amount = Double.Parse(dto.ReqAmount).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    cancel.UserEmail = dto.Email;
                    cancel.IdTransaccion = dto.TransactionId;
                    cancel.Currency = dto.Currency;
                    cancel.PaymentPlatform = PaymentPlatformDto.Itau;
                    cancel.ServiceId = Guid.Parse(dto.ServiceId);
                    cancel.ServiceDto = service;

                    if (Double.Parse(dto.ReqAmount) < 1)
                    {
                        if (dto.AmountToCybersource > 0)
                        {
                            cancel.Amount = dto.AmountToCybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")); ;
                        }
                    }

                    var result = _servicePayment.CancelPaymentCybersource(cancel);

                    return new WsBankPaymentResultDto(ErrorCodeDto.ERROR_AL_INENTAR_GENERAR_LA_TRANSACCION)
                    {
                        ErrorMessage = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_AL_INENTAR_GENERAR_LA_TRANSACCION, EnumsStrings.ResourceManager),
                        CyberSourceOperationData = result
                    };
                }
            }
        }

        public WsBankReverseResultDto ReversePayment(WsBankReverseInputDto dto)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceWsBank ReversePayment - " + DateTime.Now.ToString("G"));

            var reverse = new RefundPayment
            {
                ServiceId = Guid.Parse(dto.ServiceId),
                Token = dto.Token,
                RequestId = dto.RequestId,
                Amount = dto.Amount,
                IdTransaccion = dto.IdTransaccion,
                Currency = dto.Currency,
                UserType = dto.UserType,
                PaymentPlatform = dto.PaymentPlatform,
            };

            var serviceId = reverse.ServiceId;
            var service = _serviceService.All(null, s => s.Id == serviceId, s => s.ServiceGateways,
                    s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();

            reverse.ServiceDto = service;

            var result = _cyberSourceAccess.ReversePayment(reverse);

            return new WsBankReverseResultDto
            {
                CyberSourceOperationData = result
            };
        }

        public IEnumerable<PaymentDto> GetPayments(WsBankSearchPaymentsInputDto dto)
        {
            var serviceId = !String.IsNullOrEmpty(dto.ServiceId) ? Guid.Parse(dto.ServiceId) : default(Guid);

            DateTime fromDate;
            var isFromDate = DateTime.TryParseExact(dto.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
            fromDate = fromDate.Date;
            DateTime toDate;
            var isToDate = DateTime.TryParseExact(dto.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
            toDate = toDate.AddDays(1).Date;

            return _servicePayment.AllNoTracking(null, p => p.PaymentPlatform == PaymentPlatform.Itau &&
                    (String.IsNullOrEmpty(dto.TransactionId) || p.TransactionNumber == dto.TransactionId) &&
                    (String.IsNullOrEmpty(dto.ServiceId) || p.ServiceId == serviceId) &&
                    (!isFromDate || p.Date >= fromDate) &&
                    (!isToDate || p.Date < toDate), p => p.Service, p => p.Service.ServiceCategory, p => p.Bills, p => p.Card, p => p.AnonymousUser, p => p.Gateway).ToList();
        }

        #endregion

        #region Private Methods

        private void InsertBillsLog(WsBankBillsInputDto dto)
        {
            var info =
                    string.Format(
                        "Datos: ServiceId '{0}', GatewayEnumDto '{1}', ServiceReferenceNumber '{2}', ServiceReferenceNumber2 '{3}', ServiceReferenceNumber3 '{4}', " +
                        "ServiceReferenceNumber4 '{5}', ServiceReferenceNumber5 '{6}', ServiceReferenceNumber6 '{7}'.",
                        dto.ServiceId, dto.GatewayEnumDto, dto.ServiceReferenceNumber, dto.ServiceReferenceNumber2,
                        dto.ServiceReferenceNumber3,
                        dto.ServiceReferenceNumber4, dto.ServiceReferenceNumber5, dto.ServiceReferenceNumber6);
            NLogLogger.LogEvent(NLogType.Info, info);
        }

        private void InsertPreprocessPaymentLog(WsBankPreprocessPaymentInputDto dto)
        {
            var infoBills = "";
            foreach (var bill in dto.Bills)
            {
                infoBills += "{ Gateway " + bill.Gateway + ", IdPadron " + bill.IdPadron + ", Lines " + bill.Line + "}, ";
            }
            var info = string.Format("Datos: ServiceId '{0}', Bills: '{1}'.", dto.ServiceId, infoBills);
            NLogLogger.LogEvent(NLogType.Info, info);
        }

        private void InsertPaymentLog(WsBankPaymentInputDto dto)
        {
            var info =
                string.Format(
                    "Datos: BillId '{0}', BillNumber '{1}', ExpirationDate '{2}', Currency '{3}', Description '{4}', GatewayTransactionId '{5}', " +
                    "Payable '{6}', FinalConsumer '{7}', TotalAmount '{8}', TotalTaxedAmount '{9}', ServiceId '{10}', Discount '{11}', " +
                    "DiscountApplyed '{12}', AmountToCybersource '{13}', Gateway '{14}', Email '{15}', Ci '{16}', Address '{17}', Name '{18}', " +
                    "Surname '{19}', ServiceReferenceNumber '{20}', ServiceReferenceNumber2 '{21}', ServiceReferenceNumber3 '{22}', " +
                    "ServiceReferenceNumber4 '{23}', ServiceReferenceNumber5 '{24}', ServiceReferenceNumber6 '{25}', CardMaskedNumber '{26}', CardDueDate '{27}', " +
                    "CardName '{28}', CardBinNumbers '{29}', TransactionId '{30}'.",
                    dto.BillId, dto.BillNumber, dto.ExpirationDate, dto.Currency, dto.Description, dto.GatewayTransactionId,
                    dto.Payable, dto.FinalConsumer, dto.TotalAmount, dto.TotalTaxedAmount, dto.ServiceId, dto.Discount,
                    dto.DiscountApplyed, dto.AmountToCybersource, dto.Gateway, dto.Email, dto.Ci, dto.Address, dto.Name,
                    dto.Surname, dto.ServiceReferenceNumber, dto.ServiceReferenceNumber2, dto.ServiceReferenceNumber3,
                    dto.ServiceReferenceNumber4, dto.ServiceReferenceNumber5, dto.ServiceReferenceNumber6, dto.CardMaskedNumber, dto.CardDueDate,
                    dto.CardName, dto.CardBinNumbers, dto.TransactionId);
            NLogLogger.LogEvent(NLogType.Info, info);
        }

        private ServiceGatewayDto GetBestGateway(IEnumerable<ServiceGatewayDto> list, Guid gatewayId)
        {
            var sublist = list.Where(g => g.Active).ToList();
            if (!sublist.Any()) return null;
            if (sublist.Count() == 1) return sublist.FirstOrDefault();
            if (sublist.Any(g => g.GatewayId == gatewayId)) return sublist.FirstOrDefault(g => g.GatewayId == gatewayId);

            return sublist.Any(g =>
                g.Gateway.Enum == (int)GatewayEnumDto.Banred) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Banred)
                : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
                    : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                        : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom)
                            : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera)
                                : null;
        }

        #endregion
    }
}
