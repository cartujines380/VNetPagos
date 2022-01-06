using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor
{
    public class AutomaticPaymentHandler
    {
        private readonly IValidationHelper _validationHelper;
        private readonly IStateFactory _stateFactory;
        private readonly ILoggerHelper _loggerHelper;
        private readonly ISystemNotificationFactory _systemNotificationFactory;
        private RunState _state;
        private List<ServiceAssociatedDto> _servicesAssociated;

        public AutomaticPaymentHandler(IStateFactory stateFactory, ILoggerHelper loggerHelper,
            IValidationHelper validationHelper, ISystemNotificationFactory systemNotificationFactory)
        {
            _stateFactory = stateFactory;
            _loggerHelper = loggerHelper;
            _validationHelper = validationHelper;
            _systemNotificationFactory = systemNotificationFactory;
        }

        public void Start()
        {
            var billsCount = 0;
            var billsToPayCount = 0;
            var billsNotReadyToPayCount = 0;
            var billsPayedCount = 0;
            var billsFailedAmountCount = 0;
            var billsFailedQuotasCount = 0;
            var billsFailedCardCount = 0;
            var billsFailedServiceValidationCount = 0;
            var billsFailedCybersourceCount = 0;
            var billsFailedGatewayCount = 0;
            var billsFailedDiscountCalculationCount = 0;
            var billsFailedBinNotValidForServiceCount = 0;
            var billsFailedExpiredCount = 0;
            var billsFailedExceptionsCount = 0;
            var servicesGetBillsExceptionsCount = 0;
            var actualServiceNumber = 0;

            try
            {
                _state = _stateFactory.GetState();
                _state.StartProcess();
                _servicesAssociated = _state.GetServices();

                _loggerHelper.LogServicesCount(_servicesAssociated.Count);
                _state.ProcessStatistics.ServicesCount = _servicesAssociated.Count;

                var maxParallelism = GetDegreeOfParallelism();

                Parallel.ForEach(_servicesAssociated, new ParallelOptions { MaxDegreeOfParallelism = maxParallelism }, serviceAssociatedDto =>
                {
                    LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);

                    Interlocked.Increment(ref actualServiceNumber);
                    _loggerHelper.LogActualServiceBeingProcessed(actualServiceNumber);

                    //Es necesario setear la cultura para el cálculo del descuento
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-UY");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-UY");

                    var billHelper = NinjectRegister.Get<IBillHelper>();
                    var paymentHandler = NinjectRegister.Get<IPaymentHandler>();
                    var userNotificationFactory = NinjectRegister.Get<IUserNotificationFactory>();

                    var serviceResult = PaymentResultTypeDto.Success;
                    ICollection<BillDto> bills = new Collection<BillDto>();
                    ICollection<BillDto> filteredBills = new Collection<BillDto>();
                    var billResultDictionary = new Dictionary<Guid, PaymentResultTypeDto>();

                    try
                    {
                        bills = TryToGetBills(serviceAssociatedDto, billHelper, ref servicesGetBillsExceptionsCount,
                            ref serviceResult);

                        if (bills.Any())
                        {
                            _loggerHelper.LogServiceProcessStarted(serviceAssociatedDto);

                            Interlocked.Add(ref billsCount, bills.Count);
                            InitializeBillsResultDictionary(bills, ref billResultDictionary);

                            //Validar datos del servicio asociado
                            serviceResult = _validationHelper.ValidateService(serviceAssociatedDto);

                            if (_state.ShouldPay)
                            {
                                if (serviceResult == PaymentResultTypeDto.Success)
                                {
                                    filteredBills = billHelper.FilterBills(bills, serviceAssociatedDto, ref billResultDictionary);
                                    Interlocked.Add(ref billsToPayCount, filteredBills.Count);

                                    foreach (var bill in filteredBills.ToList())
                                    {
                                        //Se intenta pagar
                                        var billResult = paymentHandler.Pay(bill, serviceAssociatedDto);
                                        AddBillResult(bill, billResult, billResultDictionary);

                                        //Si hay varias facturas se decide qué código devolver para el servicio
                                        serviceResult = (bills.Count > 1)
                                            ? ServiceResultFromBill(billResult, serviceResult, billResultDictionary)
                                            : billResult;
                                    }
                                }
                                else
                                {
                                    AssignServiceValidationResultToBills(bills, serviceResult, ref billResultDictionary);
                                }
                            }
                            else
                            {
                                //Setear BillExternalId a facturas de sucive y geocom
                                bills = AssignBillExternalIdToBillsWithout(bills, serviceAssociatedDto);

                                //En el caso de solo notificaciones se debe asginar AutomaticPaymentDisabled a las bills
                                AssignServiceValidationResultToBills(bills, serviceResult, ref billResultDictionary);
                            }

                            _loggerHelper.LogServiceProcessFinished(serviceAssociatedDto);
                        }
                        else if (serviceResult != PaymentResultTypeDto.GetBillsException)
                        {
                            serviceResult = PaymentResultTypeDto.NoBills;
                        }

                        _state.InterpretResult(serviceResult, serviceAssociatedDto);

                        NotifyUser(userNotificationFactory, serviceAssociatedDto, serviceResult, bills, billResultDictionary);
                    }
                    catch (Exception e)
                    {
                        NLogLogger.LogEvent(NLogType.Info, "ERROR AutomaticPaymentHandler Procesamiento servicioasociado id: " + serviceAssociatedDto.Id);
                        NLogLogger.LogEvent(e);

                        serviceResult = PaymentResultTypeDto.UnhandledException;
                        _loggerHelper.LogException(e);

                        _state.InterpretResult(serviceResult, serviceAssociatedDto);

                        AssignUnhandledExceptionResultToBills(bills, filteredBills, ref billResultDictionary);

                        NotifyUser(userNotificationFactory, serviceAssociatedDto, serviceResult, bills, billResultDictionary);
                    }

                    BillsResultCount(billResultDictionary, ref billsFailedQuotasCount, ref billsFailedAmountCount,
                        ref billsFailedCardCount, ref billsFailedServiceValidationCount, ref billsPayedCount,
                        ref billsFailedCybersourceCount, ref billsFailedGatewayCount, ref billsFailedDiscountCalculationCount,
                        ref billsFailedExceptionsCount, ref billsNotReadyToPayCount, ref billsFailedBinNotValidForServiceCount,
                        ref billsFailedExpiredCount);
                });
                _loggerHelper.LogFinishedProcessingServices();

                _state.ProcessStatistics.BillsCount = billsCount;
                _state.ProcessStatistics.BillsToPayCount = billsToPayCount;
                _state.ProcessStatistics.BillsNotReadyToPayCount = billsNotReadyToPayCount;
                _state.ProcessStatistics.BillsFailedQuotasCount = billsFailedQuotasCount;
                _state.ProcessStatistics.BillsFailedAmountCount = billsFailedAmountCount;
                _state.ProcessStatistics.BillsFailedCardCount = billsFailedCardCount;
                _state.ProcessStatistics.BillsFailedServiceValidationCount = billsFailedServiceValidationCount;
                _state.ProcessStatistics.BillsPayedCount = billsPayedCount;
                _state.ProcessStatistics.BillsFailedCybersourceCount = billsFailedCybersourceCount;
                _state.ProcessStatistics.BillsFailedGatewayCount = billsFailedGatewayCount;
                _state.ProcessStatistics.BillsFailedDiscountCalculationCount = billsFailedDiscountCalculationCount;
                _state.ProcessStatistics.BillsFailedBinNotValidForServiceCount = billsFailedBinNotValidForServiceCount;
                _state.ProcessStatistics.BillsFailedExpiredCount = billsFailedExpiredCount;
                _state.ProcessStatistics.BillsFailedExceptionsCount = billsFailedExceptionsCount;
                _state.ProcessStatistics.GetBillsExceptionsCount = servicesGetBillsExceptionsCount;

                //Se actualiza el resultado de la corrida del proceso
                NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Comienza UpdateProcessHistory");
                _state.UpdateProcessHistory();
                NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Termina UpdateProcessHistory");
            }
            catch (Exception e)
            {
                _loggerHelper.LogException(e);
                const bool fatalError = true;

                //Se actualiza el resultado de la corrida del proceso indicando error
                try
                {
                    NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Comienza UpdateProcessHistory Fatal");
                    _state.UpdateProcessHistory(fatalError);
                    NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Termina UpdateProcessHistory Fatal");
                }
                catch (Exception)
                {
                    _loggerHelper.LogException(e);
                    NLogLogger.LogEvent(NLogType.Info, "ERROR AutomaticPaymentHandler Fallo UpdateProcessHistory Fatal");
                }
            }
            finally
            {
                _loggerHelper.LogProcessFinished(_state.ProcessStatistics.ProcessRunNumber);

                //Notificar resultado al sistema
                if (_state.ShouldNotifySystem)
                {
                    NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Comienza NotifyProcessResult");
                    _systemNotificationFactory.NotifyProcessResult(_state.ProcessStatistics);
                    NLogLogger.LogEvent(NLogType.Info, "AutomaticPaymentHandler Termina NotifyProcessResult");
                }
            }
        }

        #region Auxilares

        private int GetDegreeOfParallelism()
        {
            int maxParallelism;
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxParallelism"], out maxParallelism))
            {
                maxParallelism = 1;
            }
            return maxParallelism;
        }

        private ICollection<BillDto> TryToGetBills(ServiceAssociatedDto serviceAssociatedDto, IBillHelper billHelper,
            ref int servicesGetBillsExceptionsCount, ref PaymentResultTypeDto serviceResult)
        {
            ICollection<BillDto> bills = new Collection<BillDto>();
            try
            {
                bills = billHelper.ObtainBillsForService(serviceAssociatedDto);
            }
            catch (Exception)
            {
                Interlocked.Increment(ref servicesGetBillsExceptionsCount);
                serviceResult = PaymentResultTypeDto.GetBillsException;
            }
            return bills;
        }

        private void NotifyUser(IUserNotificationFactory userNotificationFactory, ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult,
            IEnumerable<BillDto> bills, Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            _state.SetNotificationFlag(billResultDictionary);
            if (_state.ShouldNotifyUser)
            {
                userNotificationFactory.NotifyServiceResultToUser(serviceAssociatedDto, serviceResult, bills,
                    billResultDictionary);
            }
        }

        private void AssignServiceValidationResultToBills(IEnumerable<BillDto> bills, PaymentResultTypeDto serviceResult,
            ref Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            foreach (var billDto in bills.ToList())
            {
                AddBillResult(billDto, serviceResult, billResultDictionary);
            }
        }

        private void AssignUnhandledExceptionResultToBills(IEnumerable<BillDto> bills, IEnumerable<BillDto> filteredBills,
            ref Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            //Si sucede una UnhandledException, se le asigna ese resultado a las facturas que no hayan sido procesadas
            const PaymentResultTypeDto unhandledException = PaymentResultTypeDto.UnhandledException;
            var controlledErrorCodes = RunState.ControlledErrorCodes();
            var retryErrorCodes = RunState.RetryErrorCodes();

            var billList = bills.ToList();
            if (filteredBills != null && filteredBills.Any())
            {
                billList = filteredBills.ToList();
            }

            foreach (var billDto in billList)
            {
                if (billResultDictionary.ContainsKey(billDto.Id))
                {
                    if (!controlledErrorCodes.Contains(billResultDictionary[billDto.Id]) &&
                        !retryErrorCodes.Contains(billResultDictionary[billDto.Id]) &&
                        billResultDictionary[billDto.Id] != PaymentResultTypeDto.Success)
                    {
                        billResultDictionary[billDto.Id] = unhandledException;
                    }
                }
                else
                {
                    billResultDictionary.Add(billDto.Id, unhandledException);
                }
            }
        }

        private void InitializeBillsResultDictionary(IEnumerable<BillDto> bills, ref Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            foreach (var billDto in bills.ToList())
            {
                if (billDto.ExpirationDate.CompareTo(DateTime.Today.Date) >= 0)
                {
                    billResultDictionary.Add(billDto.Id, PaymentResultTypeDto.BillOk);
                }
                else
                {
                    billResultDictionary.Add(billDto.Id, PaymentResultTypeDto.BillExpired);
                }
            }
        }

        private void AddBillResult(BillDto bill, PaymentResultTypeDto billResult, Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            if (billResultDictionary.ContainsKey(bill.Id))
            {
                billResultDictionary[bill.Id] = billResult;
            }
            else
            {
                billResultDictionary.Add(bill.Id, billResult);
            }
        }

        private PaymentResultTypeDto ServiceResultFromBill(PaymentResultTypeDto billResult, PaymentResultTypeDto serviceResult,
            Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            //Resultado del servicio en base al resultado de pago de la factura
            if (billResult != PaymentResultTypeDto.Success && serviceResult != PaymentResultTypeDto.ServiceErrorRetry)
            {
                var retryCodes = RunState.RetryErrorCodes();
                var controlledErrorCodes = RunState.ControlledErrorCodes();
                var deleteAutomaticPaymentCodes = RunState.DeleteAutomaticPaymentErrorCodes();

                if (retryCodes.Contains(billResult))
                {
                    serviceResult = PaymentResultTypeDto.ServiceErrorRetry;
                }
                else if (deleteAutomaticPaymentCodes.Contains(billResult))
                {
                    serviceResult = PaymentResultTypeDto.ServiceErrorDeleteAutomaticPayment;
                }
                else if (controlledErrorCodes.Contains(billResult))
                {
                    serviceResult = PaymentResultTypeDto.ServiceErrorControlled;
                }
            }

            //Si se excedió la cantidad de pagos entonces lo indico en el resultado del servicio
            if (serviceResult != PaymentResultTypeDto.ServiceErrorRetry &&
                billResultDictionary.ContainsValue(PaymentResultTypeDto.ExceededPaymentsCount))
            {
                serviceResult = PaymentResultTypeDto.ServiceErrorDeleteAutomaticPayment;
            }

            return serviceResult;
        }

        private void BillsResultCount(Dictionary<Guid, PaymentResultTypeDto> billResultDictionary,
            ref int billsFailedQuotasCount, ref int billsFailedAmountCount, ref int billsFailedCardCount,
            ref int billsFailedServiceValidationCount, ref int billsPayedCount, ref int billsFailedCybersourceCount,
            ref int billsFailedGatewayCount, ref int billsFailedDiscountCalculationCount, ref int billsFailedExceptionsCount,
            ref int billsNotReadyToPayCount, ref int billsFailedBinNotValidForServiceCount, ref int billsFailedExpiredCount)
        {
            foreach (var billResult in billResultDictionary)
            {
                switch (billResult.Value)
                {
                    case PaymentResultTypeDto.ExceededPaymentsCount:
                        Interlocked.Increment(ref billsFailedQuotasCount);
                        break;
                    case PaymentResultTypeDto.ExceededPaymentsAmount:
                        Interlocked.Increment(ref billsFailedAmountCount);
                        break;
                    case PaymentResultTypeDto.Success:
                        Interlocked.Increment(ref billsPayedCount);
                        break;
                    case PaymentResultTypeDto.BillOk:
                        Interlocked.Increment(ref billsNotReadyToPayCount);
                        break;
                    case PaymentResultTypeDto.BillExpired:
                        Interlocked.Increment(ref billsFailedExpiredCount);
                        break;
                    case PaymentResultTypeDto.UnhandledException:
                        Interlocked.Increment(ref billsFailedExceptionsCount);
                        break;
                    case PaymentResultTypeDto.InvalidCardToken:
                    case PaymentResultTypeDto.InvalidCardDueDate:
                    case PaymentResultTypeDto.InvalidCardBin:
                        Interlocked.Increment(ref billsFailedCardCount);
                        break;
                    case PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment:
                    case PaymentResultTypeDto.AutomaticPaymentDisabled:
                        Interlocked.Increment(ref billsFailedServiceValidationCount);
                        break;
                    case PaymentResultTypeDto.BinNotValidForService:
                        Interlocked.Increment(ref billsFailedBinNotValidForServiceCount);
                        break;
                    case PaymentResultTypeDto.AmountIsZeroError:
                    case PaymentResultTypeDto.DiscountCalculationError:
                    case PaymentResultTypeDto.InvalidModel:
                        Interlocked.Increment(ref billsFailedDiscountCalculationCount);
                        break;
                    case PaymentResultTypeDto.GatewayNotificationError:
                        Interlocked.Increment(ref billsFailedGatewayCount);
                        break;
                    default:
                        var cybersourceErrorCodes = RunState.CybersourceErrorCodes();
                        if (cybersourceErrorCodes.Contains(billResult.Value))
                        {
                            Interlocked.Increment(ref billsFailedCybersourceCount);
                        }
                        break;
                }
            }
        }

        private ICollection<BillDto> AssignBillExternalIdToBillsWithout(IEnumerable<BillDto> bills, ServiceAssociatedDto serviceAssociatedDto)
        {
            //Para el caso de notificaciones, si es Sucive o Geocom se debe generar un BillExternalId porque viene uno fijo
            var resultBills = bills;

            if (serviceAssociatedDto.AutomaticPaymentDto == null)
            {
                if (bills.FirstOrDefault().Gateway == GatewayEnumDto.Sucive ||
                    bills.FirstOrDefault().Gateway == GatewayEnumDto.Geocom)
                {
                    foreach (var b in resultBills)
                    {
                        b.BillExternalId = b.ExpirationDate.ToString("yyyyMMdd") + "_" + b.BillExternalId;
                    }
                }
            }
            return resultBills.ToList();
        }

        private static void LoadServiceReferenceParams(ServiceDto serviceDto)
        {
            //Si el servicio hijo pide referencias y no tiene, se usan las del contenedor
            if (serviceDto != null
                && string.IsNullOrEmpty(serviceDto.ReferenceParamName)
                && serviceDto.ServiceContainerDto != null
                && !string.IsNullOrEmpty(serviceDto.ServiceContainerDto.ReferenceParamName))
            {
                serviceDto.ReferenceParamName = serviceDto.ServiceContainerDto.ReferenceParamName;
                serviceDto.ReferenceParamName2 = serviceDto.ServiceContainerDto.ReferenceParamName2;
                serviceDto.ReferenceParamName3 = serviceDto.ServiceContainerDto.ReferenceParamName3;
                serviceDto.ReferenceParamName4 = serviceDto.ServiceContainerDto.ReferenceParamName4;
                serviceDto.ReferenceParamName5 = serviceDto.ServiceContainerDto.ReferenceParamName5;
                serviceDto.ReferenceParamName6 = serviceDto.ServiceContainerDto.ReferenceParamName6;
                serviceDto.ReferenceParamRegex = serviceDto.ServiceContainerDto.ReferenceParamRegex;
                serviceDto.ReferenceParamRegex2 = serviceDto.ServiceContainerDto.ReferenceParamRegex2;
                serviceDto.ReferenceParamRegex3 = serviceDto.ServiceContainerDto.ReferenceParamRegex3;
                serviceDto.ReferenceParamRegex4 = serviceDto.ServiceContainerDto.ReferenceParamRegex4;
                serviceDto.ReferenceParamRegex5 = serviceDto.ServiceContainerDto.ReferenceParamRegex5;
                serviceDto.ReferenceParamRegex6 = serviceDto.ServiceContainerDto.ReferenceParamRegex6;
            }
        }

        #endregion

    }
}