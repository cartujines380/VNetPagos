using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationBanred")]
    public class ConciliationBanred : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Int64 VisaTransactionId { get; set; }

        public string ReferenceNumber { get; set; }

        public string BillExternalId { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
