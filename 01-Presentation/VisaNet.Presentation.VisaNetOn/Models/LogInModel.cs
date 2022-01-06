using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class LogInModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string LogInUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        public string LogInPassword { get; set; }

        public Guid WebhookRegistrationId { get; set; }
    }
}