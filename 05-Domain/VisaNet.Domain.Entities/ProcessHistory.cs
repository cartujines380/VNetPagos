using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ProcessHistory")]
    public class ProcessHistory : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public ProcessType Process { get; set; }
        public int Count { get; set; }
        public ProcessStatus Status { get; set; }
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

        public ICollection<PendingAutomaticPayment> PendingAutomaticPayments { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }

    public class PendingAutomaticPayment
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public Guid PendingServiceAssociateId { get; set; }
        public PaymentResultType Result { get; set; }
        public string ErrorMessage { get; set; }

        public Guid ProcessHistoryId { get; set; }
        public virtual ProcessHistory ProcessHistory { get; set; }

        public Guid LastProcessHistoryId { get; set; }
    }

}