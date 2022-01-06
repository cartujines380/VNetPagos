using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CardDto
    {
        public Guid Id { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [StringLength(25, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MaskedNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PaymentToken { get; set; }

        [StringLength(150, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceTransactionId { get; set; }

        //public Guid ApplicationUser_Id { get; set; }

        public DateTime DueDate { get; set; }

        public bool Active { get; set; }
        public bool Deleted { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public Guid? ExternalId { get; set; }

        public Guid? ApplicationUser_Id { get; set; }
        public virtual ICollection<ServiceAssociatedDto> ServicesAssociatedDto { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardDto)obj);
        }

        protected bool Equals(CardDto other)
        {
            return string.Equals(MaskedNumber, other.MaskedNumber);
        }

        public override int GetHashCode()
        {
            return (MaskedNumber != null ? MaskedNumber.GetHashCode() : 0);
        }

        public CardStateDto State { get; set; }

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

    public class CardDeleteCSDto:CardDto
    {
        public Guid? AnonymousUserId { get; set; }
    }
}
