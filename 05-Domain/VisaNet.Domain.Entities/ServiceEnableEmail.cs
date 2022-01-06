using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ServiceEnableEmails")]
    [TrackChanges]
    public class ServiceEnableEmail : EntityBase
    {
        [Key]
        public override Guid Id{ get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Email{ get; set; }
        public string RouteId { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; }

    }
}
