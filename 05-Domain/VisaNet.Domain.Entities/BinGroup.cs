using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("BinGroups")]
    [TrackChanges]
    public class BinGroup : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public virtual ICollection<Bin> Bins { get; set; }

        public string Name { get; set; }

        [SkipTracking]
        public string CreationUser { get; set; }
        [SkipTracking]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
