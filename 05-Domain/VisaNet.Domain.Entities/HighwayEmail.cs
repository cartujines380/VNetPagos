using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    //HOY NO SOLO SE GENERAN FACTURAS POR EMAIL. SINO TAMBIEN POR WEB SERVICE. ESTOS SE GUARDAN ACA TMB
    [Table("HighwayEmails")]
    public class HighwayEmail : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string Sender { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string AttachmentInputName { get; set; }
        public string AttachmentOutputName { get; set; }
        public string TimeStampSeconds { get; set; }

        public DateTime AttachmentCreationDate { get; set; }

        public Int32 CodCommerce { get; set; }
        public Int32 CodBranch { get; set; }

        public Guid? ServiceId { get; set; } //me interesa tener id nullos. Por eso quito referencia a servicio

        public HighwayEmailStatus Status { get; set; }
        
        public virtual ICollection<HighwayEmailError> Errors { get; set; }
        public virtual ICollection<HighwayBill> Bills { get; set; }

        public HighwayEmailType Type { get; set; }

        public Int64 Transaccion { get; set; }

        public string ClientIp { get; set; }

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
