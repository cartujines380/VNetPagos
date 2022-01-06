using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("FixedNotifications")]
    [TrackChanges]
    public class FixedNotification : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public FixedNotificationLevel Level { get; set; }
        public FixedNotificationCategory Category { get; set; }
        public DateTime DateTime { get; set; }
        [TrackChangesAditionalInfo(Index = 0)]
        public string Description { get; set; }
        public string Detail { get; set; }
        public bool Resolved { get; set; }
        public string Comment { get; set; }
        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}