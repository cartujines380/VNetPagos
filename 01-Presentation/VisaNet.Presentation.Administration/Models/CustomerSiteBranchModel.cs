using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Administration.Models
{
    public class CustomerSiteBranchModel
    {
        
        public Guid Id { get; set; }

        [CustomDisplay("CustomerSite_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [CustomDisplay("CustomerSite_ContactEmail")]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContactEmail { get; set; }
        [CustomDisplay("CustomerSite_ContactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }
        [CustomDisplay("CustomerSite_ContactAddress")]
        public string ContactAddress { get; set; }
        [CustomDisplay("CustomerSite_Disabled")]
        public bool Disabled { get; set; }

        [CustomDisplay("CustomerSite_ServiceId")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceId { get; set; }

        [CustomDisplay("CustomerSite_Commerce")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CustomerSiteCommerce { get; set; }
        
    }
}