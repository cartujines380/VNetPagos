using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationVisanet")]
    public class ConciliationVisanet: EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string RequestId { get; set; }
        public string BillExternalId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public ConciliationType ConciliationType { get; set; }
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
