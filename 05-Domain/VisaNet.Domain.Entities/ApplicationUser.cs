using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ApplicationUsers")]
    [TrackChanges]
    public class ApplicationUser : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 2)]
        public string Email { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 1)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string IdentityNumber { get; set; }
        [MaxLength(50)]
        public string MobileNumber { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string Address { get; set; }
        [MaxLength(6)]
        public string CallCenterKey { get; set; }

        public virtual ICollection<ServiceAssociated> ServicesAssociated { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<NotificationConfig> NotificationConfigs { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

        public Platform Platform { get; set; }

        public Guid SistarbancUserId { get; set; }
        [ForeignKey("SistarbancUserId")]
        public virtual SistarbancUser SistarbancUser { get; set; }

        public Guid SistarbancBrouUserId { get; set; }
        [ForeignKey("SistarbancBrouUserId")]
        public virtual SistarbancUser SistarbancBrouUser { get; set; }

        public long CyberSourceIdentifier { get; set; }

        public Guid MembershipIdentifier { get; set; }
        [ForeignKey("MembershipIdentifier")]
        public virtual MembershipUser MembershipIdentifierObj { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        [NotMapped]
        public string FullName { get { return string.Format("{0} {1}", Name, Surname); } }

    }
}
