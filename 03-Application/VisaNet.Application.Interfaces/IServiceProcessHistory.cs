using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceProcessHistory : IService<ProcessHistory, ProcessHistoryDto>
    {
        int GetProcessCountForDate(DateTime date);
        bool ProcessExecutedSuccessfully(DateTime date);
        IEnumerable<ServiceAssociatedDto> GetPendingAutomaticPayments(DateTime date);
        void ChangePendingAutomaticPaymentStatus(Guid serviceAssociateId, Guid lastProcessId, PaymentResultTypeDto result);
        void UpdateProcessHistory(Guid id, List<PendingAutomaticPaymentDto> servicesToRetry,
            AutomaticPaymentStatisticsDto processStatistics, bool firstRun, bool fatalError);
    }
}
