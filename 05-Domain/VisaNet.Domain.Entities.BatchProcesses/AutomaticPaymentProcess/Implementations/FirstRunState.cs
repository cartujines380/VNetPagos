using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class FirstRunState : RunState
    {
        protected readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceProcessHistory _processHistoryService;
        private readonly Object _servicesResultLock = new Object();

        public FirstRunState(IServiceServiceAssosiate serviceServiceAssosiate, IServiceProcessHistory processHistoryService)
        {
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _processHistoryService = processHistoryService;
        }

        public override sealed Guid ProcessHistoryId { get; set; }
        public override sealed List<PendingAutomaticPaymentDto> ServicesToRetry { get; set; }
        public override sealed AutomaticPaymentStatisticsDto ProcessStatistics { get; set; }
        public override bool ShouldPay { get; set; }
        public override bool ShouldNotifyUser { get; set; }
        public override bool ShouldNotifySystem { get; set; }

        public override void StartProcess()
        {
            var processHistory = _processHistoryService.Create(new ProcessHistoryDto
            {
                Count = 1,
                Process = ProcessTypeDto.AutomaticPayment,
                Status = ProcessStatusDto.Start,
                PendingAutomaticPayments = new List<PendingAutomaticPaymentDto>()
            }, true);

            ProcessHistoryId = processHistory.Id;
            ServicesToRetry = new List<PendingAutomaticPaymentDto>();
            ProcessStatistics = new AutomaticPaymentStatisticsDto
            {
                ProcessRunNumber = 1,
                ServicesResult = new Dictionary<Guid, PaymentResultTypeDto>()
            };
            ShouldPay = true;
            ShouldNotifyUser = false;
            ShouldNotifySystem = true;
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return _serviceServiceAssosiate.GetServicesActiveAutomaticPaymentOrNotification().ToList();
        }

        public override void UpdateProcessHistory(bool fatalError = false)
        {
            const bool firstRun = true;
            _processHistoryService.UpdateProcessHistory(ProcessHistoryId, ServicesToRetry, ProcessStatistics, firstRun,
                fatalError);

            //TODO: por ahora no se va a usar (demora mucho)
            //_serviceServiceAssosiate.UpdateAutomaticPaymentRunsData(ProcessHistoryId, ProcessStatistics.ServicesResult);
        }

        public override void SetNotificationFlag(Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            //Si sucedio hay algun error de reintento entonces no se notifica hasta la ultima corrida
            ShouldNotifyUser = false;

            var retryErrorCodes = RetryErrorCodes();
            var billsResultValues = billResultDictionary.Values.ToList();
            var billsResultsContainsRetryError = billsResultValues.Any(x => retryErrorCodes.Any(y => y == x));
            if (!billsResultsContainsRetryError)
            {
                ShouldNotifyUser = true;
            }
        }

        protected override void SuccessResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

        protected override void ControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

        protected override void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
            _serviceServiceAssosiate.DeleteAutomaticPayment(serviceAssociatedDto.Id);
        }

        protected override void RetryErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                //Verifico si este servicio ya fue agregado por otro error y luego hubo una excepcion
                if (ProcessStatistics.ServicesResult.ContainsKey(serviceAssociatedDto.Id) &&
                    serviceResult == PaymentResultTypeDto.UnhandledException)
                {
                    ProcessStatistics.ServicesResult[serviceAssociatedDto.Id] = serviceResult;
                    if (ServicesToRetry.Any(x => x.PendingServiceAssociateId == serviceAssociatedDto.Id))
                    {
                        ServicesToRetry.FirstOrDefault(x => x.PendingServiceAssociateId == serviceAssociatedDto.Id).Result =
                            serviceResult;
                    }
                }
                else
                {
                    ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
                    ServicesToRetry.Add(new PendingAutomaticPaymentDto
                    {
                        Date = DateTime.Now,
                        PendingServiceAssociateId = serviceAssociatedDto.Id,
                        Result = serviceResult,
                        ProcessHistoryId = ProcessHistoryId,
                        LastProcessHistoryId = ProcessHistoryId
                    });
                }
            }
        }

    }
}