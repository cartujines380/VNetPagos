using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Models
{
    public class LogOnModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }
        [Display(Name = "Security_RememberMe", ResourceType = typeof(PresentationCoreMessages))]
        public bool RememberMe { get; set; }
    }

    public class CallCenterLogOnModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_UserName", ResourceType = typeof(PresentationCoreMessages))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }
        [Display(Name = "Security_RememberMe", ResourceType = typeof(PresentationCoreMessages))]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_OldPassword", ResourceType = typeof(PresentationCoreMessages))]
        public string OldPassword { get; set; }

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


    public class ChangePasswordWebModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_OldPassword", ResourceType = typeof(PresentationCoreMessages))]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_NewPassword", ResourceType = typeof(PresentationCoreMessages))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_ConfirmPassword", ResourceType = typeof(PresentationCoreMessages))]
        [Compare("NewPassword", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class SetPasswordFromTokenModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_ConfirmPassword", ResourceType = typeof(PresentationCoreMessages))]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class ForgetMyPasswordModel
    {
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }
    }
}