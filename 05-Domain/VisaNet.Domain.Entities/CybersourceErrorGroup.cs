using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("CybersourceErrorGroups")]
    public class CybersourceErrorGroup : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public virtual ICollection<CybersourceError> CybersourceErrors { get; set; }
    }
}
