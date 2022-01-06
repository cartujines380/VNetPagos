using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface ISystemNotificationFactory
    {
        void BillExceedsQuotasLoggerNotification(ServiceAssociatedDto serviceAssociatedDto);
        void BillExceedsAmountLoggerNotification(ServiceAssociatedDto serviceAssociatedDto, BillDto bill);
        void NotifyProcessResult(AutomaticPaymentStatisticsDto processStatistics);
    }
}
