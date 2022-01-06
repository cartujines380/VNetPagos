using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("AnonymousUsers")]
    [TrackChanges]
    public class AnonymousUser : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }

        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 1)]
        public string Surname { get; set; }

        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 2)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string IdentityNumber { get; set; }
        [MaxLength(50)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string MobileNumber { get; set; }

        public Guid SistarbancUserId { get; set; }
        [ForeignKey("SistarbancUserId")]
        public virtual SistarbancUser SistarbancUser { get; set; }

        public Guid SistarbancBrouUserId { get; set; }
        [ForeignKey("SistarbancBrouUserId")]
        public virtual SistarbancUser SistarbancBrouUser { get; set; }

        public long CyberSourceIdentifier { get; set; }

        public bool IsPortalUser { get; set; }
        public bool IsVonUser { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
