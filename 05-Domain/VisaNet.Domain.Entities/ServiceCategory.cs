using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ServicesCategories")]
    [TrackChanges]
    public class ServiceCategory : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        [MaxLength(100)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        [SkipTracking]
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [SkipTracking]
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }
    }
}
