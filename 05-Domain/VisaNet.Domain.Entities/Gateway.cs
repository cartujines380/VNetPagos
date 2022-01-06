using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Gateway")]
    public class Gateway : EntityBase
    {
        [Key]
        public override Guid Id{ get; set; }

        [MaxLength(50)]
        public string Name{ get; set; }

        public int Enum { get; set; }
    }
}
