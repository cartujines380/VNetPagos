using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Images")]
    [TrackChanges]
    public class Image: EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        [MaxLength(250)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string OriginalName { get; set; }
        [MaxLength(250)]
        public string InternalName { get; set; }
    }
}
