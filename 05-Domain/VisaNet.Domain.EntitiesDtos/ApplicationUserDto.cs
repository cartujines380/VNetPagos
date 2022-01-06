using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ApplicationUserDto : IUserDto
    {
        public Guid Id { get; set; }

        [CustomDisplay("ApplicationUser_Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [CustomDisplay("ApplicationUser_Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("ApplicationUser_Surname")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }

        [CustomDisplay("ApplicationUser_IdentityNumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }

        [CustomDisplay("ApplicationUser_MobileNumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }

        [CustomDisplay("ApplicationUser_PhoneNumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }

        [CustomDisplay("ApplicationUser_Address")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }

        [CustomDisplay("ApplicationUser_CallCenterKey")]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterKey { get; set; }


        [CustomDisplay("ApplicationUser_Platform")]
        public PlatformDto Platform { get; set; }

        public Guid MembreshipIdentifier { get; set; }
        public MembershipUserDto MembershipIdentifierObj { get; set; }
        public long CyberSourceIdentifier { get; set; }
        public Guid SistarbancUserId { get; set; }
        public virtual SistarbancUserDto SistarbancUser { get; set; }
        public Guid SistarbancBrouUserId { get; set; }
        public virtual SistarbancUserDto SistarbancBrouUser { get; set; }

        public virtual ICollection<ServiceAssociatedDto> ServicesAssociated { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<CardDto> CardDtos { get; set; }
        public virtual ICollection<PaymentDto> PaymentDtos { get; set; }

        public bool RecieveNewsletter { get; set; }

        public bool IsRegisteredUser
        {
            get { return true; }
        }

        public string Password { get; set; }
    }
}
