using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface IUserNotificationFactory
    {
        void BillExceedsQuotasNotification(ServiceAssociatedDto serviceAssociatedDto);
        void BillExceedsAmountNotification(ServiceAssociatedDto serviceAssociatedDto, BillDto bill);
        void NotifyServiceResultToUser(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult,
            IEnumerable<BillDto> bills, Dictionary<Guid, PaymentResultTypeDto> billResultDictionary);
    }
}
