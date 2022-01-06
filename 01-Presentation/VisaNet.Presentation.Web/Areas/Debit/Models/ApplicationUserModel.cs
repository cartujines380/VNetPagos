using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Areas.Debit.Models
{
    public class ApplicationUserModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("ApplicationUser_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("ApplicationUser_Surname")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }

        [CustomDisplay("ApplicationUser_Email")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessageResourceName = "InvalidEmail",
                            ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }

        [CustomDisplay("ApplicationUser_IdentityNumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }

        [CustomDisplay("ApplicationUser_MobileNumber")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }

        [CustomDisplay("Profile_Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }

        [CustomDisplay("Profile_Confirmation")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PasswordConfirmation { get; set; }

        [CustomDisplay("Profile_CCKey")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterKey { get; set; }

        public bool AcceptTermsAndConditions { get; set; }

        public IList<CardModel> Cards { get; set; }
    }
}