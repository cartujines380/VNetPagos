using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Tc33Transactions")]
    public class Tc33Transaction: EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string RequestId { get; set; }

        public Guid Tc33Id { get; set; }

        public virtual Tc33 Tc33 { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
