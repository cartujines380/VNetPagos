using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("AffiliationCard")]
    [TrackChanges]
    public class AffiliationCard : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        [Index("IX_AffiliationCard_Code", 1, IsUnique = true)]
        public int Code { get; set; }
        public bool Active { get; set; }

        public Guid? BankId { get; set; }
        public virtual Bank Bank{get;set;}
        
        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
