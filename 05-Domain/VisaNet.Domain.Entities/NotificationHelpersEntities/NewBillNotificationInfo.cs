using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities.NotificationHelpersEntities
{
    [Table("NewBillNotificationInfo")]
    public class NewBillNotificationInfo : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        [MaxLength(100)]
        public string BillNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public BillType BillType { get; set; }

        public Guid? ServiceId { get; set; }
        public virtual Service Service { get; set; }

        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
