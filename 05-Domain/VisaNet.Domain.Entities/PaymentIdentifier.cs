using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("PaymentsIdentifiers")]
    public class PaymentIdentifier : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string CyberSourceTransactionIdentifier { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 UniqueIdentifier { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
