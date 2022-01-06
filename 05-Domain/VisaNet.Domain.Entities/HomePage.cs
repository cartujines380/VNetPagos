using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("HomePage")]
    [TrackChanges]
    public class HomePage: EntityBase, IAuditable
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }

        public Guid HomePageItem1Id { get; set; }
        public virtual HomePageItem HomePageItem1 { get; set; }

        public Guid HomePageItem2Id { get; set; }
        public virtual HomePageItem HomePageItem2 { get; set; }

        public Guid HomePageItem3Id { get; set; }
        public virtual HomePageItem HomePageItem3 { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
