using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Administration.Models
{

    public class LogOnModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "UserName", ResourceType = typeof(PresentationCoreMessages))]
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

    public class SetPasswordFromTokenModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_UserName", ResourceType = typeof(PresentationCoreMessages))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_ConfirmPassword", ResourceType = typeof(PresentationCoreMessages))]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ConfirmPassword { get; set; }
    }

    public class ForgetMyPasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_UserName", ResourceType = typeof(PresentationCoreMessages))]
        public string UserName { get; set; }

        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }
    }

}