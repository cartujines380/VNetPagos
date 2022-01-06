using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class FinishedState : RunState
    {
        public override sealed Guid ProcessHistoryId { get; set; }
        public override sealed List<PendingAutomaticPaymentDto> ServicesToRetry { get; set; }
        public override sealed AutomaticPaymentStatisticsDto ProcessStatistics { get; set; }
        public override bool ShouldPay { get; set; }
        public override bool ShouldNotifyUser { get; set; }
        public override bool ShouldNotifySystem { get; set; }

        public override void StartProcess()
        {
            ShouldPay = false;
            ShouldNotifyUser = false;
            ShouldNotifySystem = false;
            ProcessStatistics = new AutomaticPaymentStatisticsDto
            {
                ProcessRunNumber = 0,
                ServicesResult = new Dictionary<Guid, PaymentResultTypeDto>()
            };
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return new List<ServiceAssociatedDto>();
        }

        public override void UpdateProcessHistory(bool fatalError = false)
        {
        }

        public override void SetNotificationFlag(Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            ShouldNotifyUser = false;
        }

        protected override void SuccessResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            return;
        }

        protected override void ControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            return;
        }

        protected override void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            return;
        }

        protected override void RetryErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            return;
        }

    }
}
