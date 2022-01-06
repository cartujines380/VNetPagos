using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Administration.Models
{
    public class CustomerSiteCommerceModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("CustomerSite_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [CustomDisplay("CustomerSite_ContactEmail")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
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

        [CustomDisplay("CustomerSite_SystemUser")]
        public Guid CustomerSiteSystemUser { get; set; }

        public bool CreateBranch { get; set; }

        [CustomDisplay("CustomerSite_ImageName")]
        public string ImageName { get; set; }

        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(ImageName) ? string.Empty : string.Format("{0}{1}", Id.ToString(), ImageName.Substring(ImageName.LastIndexOf("."))); }
        }
        public string ImageUrl { get; set; }
        public bool DeleteImage { get; set; }

    }
}