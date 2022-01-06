using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ApplicationUserCreateEditDto : IUserDto
    {
        public Guid Id { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }

        public DateTime CreationDate { get; set; }

        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterKey { get; set; }

        public bool IsRegisteredUser { get; private set; }
        public string Password { get; set; }
        public Guid MembreshipIdentifier { get; set; }

        public bool PasswordAlreadyHashed { get; set; }
    }
}
