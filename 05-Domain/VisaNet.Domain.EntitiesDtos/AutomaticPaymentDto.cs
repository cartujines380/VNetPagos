using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class AutomaticPaymentDto
    {
        public Guid Id { get; set; }
        public Guid ServiceAssosiateId { get; set; }

        public int DaysBeforeDueDate { get; set; }
        public double Maximum { get; set; }
        public int Quotas { get; set; }
        public int QuotasDone { get; set; }

        public bool UnlimitedQuotas { get; set; }
        public bool UnlimitedAmount { get; set; }
        public bool SuciveAnnualPatent { get; set; }

        public DateTime? LastRunDate { get; set; }
        public PaymentResultTypeDto? LastRunResult { get; set; }

        public DateTime? LastSuccessfulPaymentDate { get; set; }
        public int? LastSuccessfulPaymentIteration { get; set; }

        public DateTime? LastErrorDate { get; set; }
        public PaymentResultTypeDto? LastErrorResult { get; set; }

    }
}
