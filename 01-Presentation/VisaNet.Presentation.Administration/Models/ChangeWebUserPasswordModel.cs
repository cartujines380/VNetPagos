using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.Administration.Models
{
    public class ChangeWebUserPasswordModel
    {
        public Guid UserId{ get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_NewPassword", ResourceType = typeof(PresentationCoreMessages))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_ConfirmPassword", ResourceType = typeof(PresentationCoreMessages))]
        [Compare("NewPassword", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ConfirmPassword { get; set; } 
    }
}