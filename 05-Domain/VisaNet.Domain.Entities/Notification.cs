using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Notifications")]
    public class Notification : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public Guid? ServiceId { get; set; }
        
        public virtual Service Service { get; set; }

        public DateTime Date { get; set; }
        
        //[MaxLength(500)]
        public string Message { get; set; }


        public NotificationPrupose NotificationPrupose { get; set; }

        public Guid RegisteredUserId { get; set; }
        public virtual ApplicationUser RegisteredUser { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
