using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ApplicationUserModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("ApplicationUser_Email")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
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
        public int PlatformId { get; set; }

        [CustomDisplay("ApplicationUser_FailLogInCount")]
        public int FailLogInCount { get; set; }

        [CustomDisplay("ApplicationUser_Status")]
        public string Status { get; set; }

        [CustomDisplay("ApplicationUser_Active")]
        public DateTime CreationDate { get; set; }

    }
}
