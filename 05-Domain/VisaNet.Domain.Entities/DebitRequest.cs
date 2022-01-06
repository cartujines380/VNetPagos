using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("DebitRequests")]
    [TrackChanges]
    public class DebitRequest : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public DebitRequestType Type { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid CardId { get; set; }
        public virtual Card Card { get; set; }
        public int DebitProductId { get; set; }
        public DebitRequestState State { get; set; }
        public int? DebitRequestEventId { get; set; }
        public DateTime? SynchronizationDate { get; set; }
        public string SynchronizationMessage { get; set; }

        public int? SynchronizationLimitOfAttemps { get; set; }

        [Index(IsUnique = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReferenceNumber { get; set; }

        public Guid? AssociatedDebitRequestId { get; set; }
        public virtual DebitRequest AssociatedDebitRequest { get; set; }

        public virtual ICollection<DebitRequestReference> References { get; set; }

        #region Auditable Fields
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        #endregion Auditable Fields
    }
}
