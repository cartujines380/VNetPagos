using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Cards")]
    [TrackChanges]
    public class Card : EntityBase
    {

        public Card()
        {
            Active = true;
        }

        [Key]
        public override Guid Id { get; set; }

        [MaxLength(25)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string MaskedNumber { get; set; }
        [MaxLength(50)]
        public string PaymentToken { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime DueDate { get; set; }

        public bool Active { get; set; }

        public bool Deleted { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string CybersourceTransactionId { get; set; }

        //public Guid ApplicationUser_Id { get; set; }

        public Guid? ExternalId { get; set; }

        public virtual ICollection<ServiceAssociated> ServicesAssociated { get; set; }

        public int BIN
        {
            get
            {
                //Si no tiene número enmascarado retorna 0, sino los primeros 6 digitos
                return string.IsNullOrWhiteSpace(MaskedNumber) ? 0 : int.Parse(MaskedNumber.Substring(0, 6));
            }
        }

        public bool DeletedFromCs { get; set; }

    }
}
