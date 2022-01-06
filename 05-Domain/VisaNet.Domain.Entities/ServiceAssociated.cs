using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ServicesAssociated")]
    [TrackChanges]
    public class ServiceAssociated : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public Guid DefaultCardId { get; set; }
        [ForeignKey("DefaultCardId")]
        public virtual Card DefaultCard { get; set; }

        //valor que cambia el usuario asignado al servicio asociado
        public bool Active { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [MaxLength(100)]
        public string ReferenceNumber { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber2 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber3 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber4 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber5 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber6 { get; set; }

        [MaxLength(250)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Description { get; set; }

        public Guid? AutomaticPaymentId { get; set; }
        public virtual AutomaticPayment AutomaticPayment { get; set; }

        public Guid RegisteredUserId { get; set; }
        public virtual ApplicationUser RegisteredUser { get; set; }

        public Guid NotificationConfigId { get; set; }
        public virtual NotificationConfig NotificationConfig { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Card> Cards{ get; set; }

        //Representa el estado del servicio. si se desactiva el servicio este pasa a false.
        public bool Enabled { get; set; }
        public Guid? IdUserExternal { get; set; }
        //Si el usuario desea borrar el servicio asociado, pero habia hecho algun pago de ese servicio
        //este campo se marca con true (si no tenia pagos realizados se elimina)
        //public bool Deleted { get; set; }

        [SkipTracking]
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [SkipTracking]
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

    }
}
