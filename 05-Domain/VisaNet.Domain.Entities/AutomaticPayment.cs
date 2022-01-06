using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("AutomaticPayments")]
    [TrackChanges]
    public class AutomaticPayment : EntityBase
    {
        [Key]
        [TrackChangesAditionalInfo]
        public override Guid Id { get; set; }

        public int DaysBeforeDueDate { get; set; }
        public double Maximum { get; set; }
        public int Quotas { get; set; }

        public int QuotasDone { get; set; }

        public bool UnlimitedQuotas { get; set; }
        public bool UnlimitedAmount { get; set; }
        public bool SuciveAnnualPatent { get; set; }

        public DateTime? LastRunDate { get; set; }
        public PaymentResultType? LastRunResult { get; set; }

        public DateTime? LastSuccessfulPaymentDate { get; set; }
        public int? LastSuccessfulPaymentIteration { get; set; }

        public DateTime? LastErrorDate { get; set; }
        public PaymentResultType? LastErrorResult { get; set; }

    }
}
