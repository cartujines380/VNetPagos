using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ProfileModel
    {

        public Guid Id { get; set; }

        [CustomDisplay("Profile_PasswordNew")]
        public string PasswordNew { get; set; }

        [CustomDisplay("Profile_Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }

        [CustomDisplay("Profile_Confirmation")]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PasswordConfirmation { get; set; }

        [CustomDisplay("Profile_identitynumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }
        [CustomDisplay("Profile_Nombre")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [CustomDisplay("Profile_Surname")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }
        [CustomDisplay("Profile_Email")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }
        public string OldEmail { get; set; }

        [CustomDisplay("Profile_Mobile")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }
        [CustomDisplay("Profile_Phone")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }
        [CustomDisplay("Profile_Address")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }
        [CustomDisplay("Profile_CCKey")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterKey { get; set; }

        [CustomDisplay("Profile_RecieveNewsletter")]
        public bool RecieveNewsletter { get; set; }
    }
}