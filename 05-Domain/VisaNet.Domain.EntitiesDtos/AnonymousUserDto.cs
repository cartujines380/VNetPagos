using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Domain.EntitiesDtos
{
    public class AnonymousUserDto : IUserDto
    {
        public Guid Id { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }
        public Guid SistarbancUserId { get; set; }
        public virtual SistarbancUserDto SistarbancUser { get; set; }
        public long CyberSourceIdentifier { get; set; }
        public Guid SistarbancBrouUserId { get; set; }
        public virtual SistarbancUserDto SistarbancBrouUser { get; set; }

        public bool IsPortalUser { get; set; }
        public bool IsVonUser { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsRegisteredUser
        {
            get { return false; }
        }

        public string Password { get; set; }
    }
}
