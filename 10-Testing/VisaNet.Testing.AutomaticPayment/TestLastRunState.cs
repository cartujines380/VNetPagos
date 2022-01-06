using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Testing.AutomaticPayment
{
    public class TestLastRunState : LastRunState
    {
        private readonly IServiceProcessHistory _processHistoryService;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly Object _servicesResultLock = new Object();

        public TestLastRunState(IServiceServiceAssosiate serviceServiceAssosiate, IServiceProcessHistory processHistoryService)
            : base(serviceServiceAssosiate, processHistoryService)
        {
            _processHistoryService = processHistoryService;
            _serviceServiceAssosiate = serviceServiceAssosiate;
        }

        public override void StartProcess()
        {
            //TODO: es la cantidad que tienen el Additional
            var processCount = _processHistoryService.GetProcessCountForDate(DateTime.Now);
            var processHistory = _processHistoryService.Create(new ProcessHistoryDto
            {
                Count = processCount + 1,
                Process = ProcessTypeDto.AutomaticPayment,
                Status = ProcessStatusDto.Start,
                PendingAutomaticPayments = new List<PendingAutomaticPaymentDto>(),
                Additional = "TEST_AUTOMATIC_PAYMENT"
            }, true);

            ProcessHistoryId = processHistory.Id;
            ServicesToRetry = new List<PendingAutomaticPaymentDto>();
            ProcessStatistics = new AutomaticPaymentStatisticsDto
            {
                ProcessRunNumber = processCount + 1,
                ServicesResult = new Dictionary<Guid, PaymentResultTypeDto>()
            };
            ShouldNotifyUser = true;
            ShouldNotifySystem = true;
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return _processHistoryService.GetPendingAutomaticPayments(DateTime.Now).Where(x =>
                x.Description != null && x.Description.ToUpper() == "TEST_AUTOMATIC_PAYMENT").ToList();
        }

        public override void UpdateProcessHistory(bool fatalError = false)
        {
            const bool firstRun = false;
            _processHistoryService.UpdateProcessHistory(ProcessHistoryId, ServicesToRetry, ProcessStatistics, firstRun,
                fatalError);

            //En LastRunState no se esta actualizando esto pero para los Tests se necesita
            _serviceServiceAssosiate.UpdateAutomaticPaymentRunsData(ProcessHistoryId, ProcessStatistics.ServicesResult);
        }

        protected override void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
            _processHistoryService.ChangePendingAutomaticPaymentStatus(serviceAssociatedDto.Id,
                        ProcessHistoryId, serviceResult);
        }

    }
}