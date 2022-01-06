using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("HighwayEmailErrors")]
    public class HighwayEmailError : EntityBase
    {
        [Key]
        public override Guid Id{ get; set; }
        public string Data{ get; set; }
        
        public virtual HighwayEmail HighwayEmail { get; set; }
        public Guid HighwayEmailId { get; set; }

    }
}
