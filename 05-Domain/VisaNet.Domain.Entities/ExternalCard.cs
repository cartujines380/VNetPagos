using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ExternalCards")]
    public class ExternalCard : EntityBase, IAuditable
    {
        //TODO: Key por las 3 cosas? o solo por ExternalId? (si ya se tienen casos de mismo ExternalId para diferentes servicios no se va a poder)
        [Key]
        public Guid ExternalId { get; set; }

        [Key]
        public Guid CardId { get; set; }
        [ForeignKey("CardId")]
        public virtual Card Card { get; set; }

        [Key]
        public Guid ServicesAssociatedId { get; set; }
        [ForeignKey("ServicesAssociatedId")]
        public virtual ServiceAssociated ServicesAssociated { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}