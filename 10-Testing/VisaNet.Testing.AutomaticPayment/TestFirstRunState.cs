using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Testing.AutomaticPayment
{
    public class TestFirstRunState : FirstRunState
    {
        private readonly IServiceProcessHistory _processHistoryService;
        private readonly Object _servicesResultLock = new Object();

        public TestFirstRunState(IServiceServiceAssosiate serviceServiceAssosiate, IServiceProcessHistory processHistoryService)
            : base(serviceServiceAssosiate, processHistoryService)
        {
            _processHistoryService = processHistoryService;
        }

        public override void StartProcess()
        {
            var processHistory = _processHistoryService.Create(new ProcessHistoryDto
            {
                Count = 1,
                Process = ProcessTypeDto.AutomaticPayment,
                Status = ProcessStatusDto.Start,
                PendingAutomaticPayments = new List<PendingAutomaticPaymentDto>(),
                Additional = "TEST_AUTOMATIC_PAYMENT"
            }, true);

            ProcessHistoryId = processHistory.Id;
            ServicesToRetry = new List<PendingAutomaticPaymentDto>();
            ProcessStatistics = new AutomaticPaymentStatisticsDto
            {
                ProcessRunNumber = 1,
                ServicesResult = new Dictionary<Guid, PaymentResultTypeDto>()
            };
            ShouldNotifyUser = false;
            ShouldNotifySystem = true;
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return _serviceServiceAssosiate.GetServicesActiveAutomaticPaymentOrNotification().Where(x =>
                x.Description != null && x.Description.ToUpper() == "TEST_AUTOMATIC_PAYMENT").ToList();
        }

        public override void UpdateProcessHistory(bool fatalError = false)
        {
            const bool firstRun = true;
            _processHistoryService.UpdateProcessHistory(ProcessHistoryId, ServicesToRetry, ProcessStatistics, firstRun,
                fatalError);

            //En FirstRunState no se esta actualizando esto pero para los Tests se necesita
            _serviceServiceAssosiate.UpdateAutomaticPaymentRunsData(ProcessHistoryId, ProcessStatistics.ServicesResult);
        }

        protected override void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

    }
}