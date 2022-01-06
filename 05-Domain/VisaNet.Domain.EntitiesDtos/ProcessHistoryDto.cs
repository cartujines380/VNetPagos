using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ProcessHistoryDto
    {
        public Guid Id { get; set; }

        public ProcessTypeDto Process { get; set; }
        public DateTime CreationDate { get; set; }
        public int Count { get; set; }
        public ProcessStatusDto Status { get; set; }
        public string ErrorMessages { get; set; }
        public string Additional { get; set; }

        public int ServicesCount { get; set; }
        public int ServicesWithBillsCount { get; set; }
        public int ServicesFailedGetBillsCount { get; set; }
        public int BillsCount { get; set; }
        public int BillsToPayCount { get; set; }
        public int BillsPayedCount { get; set; }
        public int BillsFailedAmountCount { get; set; }
        public int BillsFailedQuotasCount { get; set; }
        public int ControlledErrorsCount { get; set; }
        public int RetryErrorsCount { get; set; }

        public ICollection<PendingAutomaticPaymentDto> PendingAutomaticPayments { get; set; }
    }

    public class PendingAutomaticPaymentDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public Guid PendingServiceAssociateId { get; set; }
        public PaymentResultTypeDto Result { get; set; }
        public string ErrorMessage { get; set; }

        public Guid ProcessHistoryId { get; set; }
        public virtual ProcessHistoryDto ProcessHistoryDto { get; set; }

        public Guid LastProcessHistoryId { get; set; }
    }

}