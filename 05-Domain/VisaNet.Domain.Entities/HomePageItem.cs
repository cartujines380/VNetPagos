using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("HomePageItems")]
    [TrackChanges]
    public class HomePageItem: EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string Headline1 { get; set; }

        public string Headline2 { get; set; }

        [TrackChangesAditionalInfo(Index = 0)]
        public string Description { get; set; }

        public Guid? ImageId { get; set; }
        public virtual Image Image { get; set; }

        public string LinkUrl { get; set; }

        public Guid? FileId { get; set; }
        public virtual Image File { get; set; }

        public string LinkName { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
