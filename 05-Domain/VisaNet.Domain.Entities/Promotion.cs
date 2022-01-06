using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Promotion")]
    [TrackChanges]
    public class Promotion : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public bool Active { get; set; }
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string ImageName { get; set; }

        //TODO: Las imagenes como refereencias se van a eliminar en un proximo release
        public Guid? ImageId { get; set; }
        public virtual Image Image { get; set; }

        [SkipTracking]
        public string CreationUser { get; set; }

        [SkipTracking]
        public string LastModificationUser { get; set; }

        [SkipTracking]
        public DateTime CreationDate { get; set; }

        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

    }
}
