using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Interpreters")]
    [TrackChanges]
    public class Interpreter : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Service> Services { get; set; }
            
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
