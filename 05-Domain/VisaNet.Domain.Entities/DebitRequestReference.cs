using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("DebitRequestReferences")]
    [TrackChanges]
    public class DebitRequestReference : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        public int Index { get; set; }
        public int ProductPropertyId { get; set; }
        public string Value { get; set; }
        
    }
}
