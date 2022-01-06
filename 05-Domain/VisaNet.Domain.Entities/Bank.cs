using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Bank")]
    [TrackChanges]
    public class Bank : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        public int Code { get; set; }
        public string QuotesPermited { get; set; }

        public virtual ICollection<Bin> Bins { get; set; }
        
        
        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
