using System;

namespace VisaNet.Domain.EntitiesDtos.ReportsModel
{
    public class AutomaticPaymentsViewDto
    {
        public Guid AutomaticPaymentId { get; set; }
        public Guid ServiceAssociatedId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid RegisteredUserId { get; set; }
        public bool ServiceActive { get; set; }
        public bool ServiceEnabled { get; set; }

        public string ClientEmail { get; set; }
        public string ServiceNameAndDesc { get; set; }

        public double Maximum { get; set; }
        public bool UnlimitedAmount { get; set; }
        public int DaysBeforeDueDate { get; set; }
        public int Quotas { get; set; }
        public bool UnlimitedQuotas { get; set; }
        public int QuotasDone { get; set; }
        public bool SuciveAnnualPatent { get; set; }

        public DateTime? LastRunDate { get; set; }
        public int? LastRunResult { get; set; }
        public DateTime? LastSuccessfulPaymentDate { get; set; }
        public int? LastSuccessfulPaymentIteration { get; set; }
        public DateTime? LastErrorDate { get; set; }
        public int? LastErrorResult { get; set; }

        public int PaymentsCount { get; set; }
        public double PaymentsAmountPesos { get; set; }
        public double PaymentsAmountDollars { get; set; }

        public DateTime CreationDate { get; set; }
    }
}