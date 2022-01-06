using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("CybersourceErrors")]
    public class CybersourceError : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        public int ReasonCode { get; set; }

        public Guid CybersourceErrorGroupId { get; set; }
        public virtual CybersourceErrorGroup CybersourceErrorGroup { get; set; }
    }
}
