using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Models
{
    public class PaymentServiceModel
    {
        [CustomDisplay("PaymentDto_ServiceId")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public Guid ServiceFirstComboId { get; set; }
        
        public string TooltipeImage { get; set; }
        public string TooltipeDesc { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        [RegexValidation("ReferenceRegex2")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        [RegexValidation("ReferenceRegex3")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        [RegexValidation("ReferenceRegex4")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        [RegexValidation("ReferenceRegex5")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        [RegexValidation("ReferenceRegex6")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }

        [CustomDisplay("PaymentDto_Description")]
        [StringLength(45, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }
        public string ServiceType { get; set; }
        public AnonymousUserModel AnonymousUser { get; set; }
        public bool Sucive { get; set; }
        public List<SelectListItem> Departaments { get; set; }
        public IEnumerable<SelectListItem> LocationsCiu { get; set; }
        public string ReferenceRegex { get; set; }
        public string ReferenceRegex2 { get; set; }
        public string ReferenceRegex3 { get; set; }
        public string ReferenceRegex4 { get; set; }
        public string ReferenceRegex5 { get; set; }
        public string ReferenceRegex6 { get; set; }

        public IOrderedEnumerable<ServiceDto> Services { get; set; }
        public ICollection<ServiceDto> ServicesInContainer { get; set; }
        
        public Guid ServiceSecondcomboId { get; set; }

        public ServiceDto ServiceToPay { get; set; }
        public Guid ServiceToPayId { get; set; }

        public Guid? RegisteredUserId { get; set; }
        public ApplicationUserDto RegisteredUser { get; set; }

        public bool AssociateService { get; set; }
        public bool AskReferences { get; set; }

        public bool AskChildService { get; set; }
        public bool AnnualSuciveYes { get; set; }
        public bool AnnualSuciveNo { get; set; }
        public bool AnnualSucive { get; set; }
        

        public PaymentServiceModel()
        {
            AnonymousUser = new AnonymousUserModel();
            RegisteredUser = new ApplicationUserDto();
        }
    }

}
