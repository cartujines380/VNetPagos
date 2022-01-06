using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationCybersource")]
    public class ConciliationCybersource : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string DateString { get; set; }

        public string RequestId { get; set; }
        public string MerchantReferenceNumber { get; set; }
        
        public string Currency { get; set; }
        public double Amount { get; set; }

        public string Source { get; set; }
        public string IcsApplications { get; set; }
        public string MerchandId { get; set; }

        //True si hay registro en tabla payments
        public bool PaymentDone { get; set; }

        public TransactionType TransactionType { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
