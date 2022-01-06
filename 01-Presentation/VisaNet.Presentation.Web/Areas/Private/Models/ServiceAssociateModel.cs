using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ServiceAssociateModel
    {
        [CustomDisplay("Service_Description")]
        [StringLength(45, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        [CustomDisplay("Service_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public Guid ServiceFirstComboId { get; set; }
        public string ServiceName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex2")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex3")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex4")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex5")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [RegexValidation("ReferenceRegex6")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }


        public string Id { get; set; }

        public string TooltipeImage { get; set; }
        public string TooltipeDesc { get; set; }
        public bool DisableEdition { get; set; }

        public ServiceAssociateNotificationModel NotificationConfig { get; set; }
        
        //es con lo q obtengo los datos de sesion luego del post a cybersource
        public string reference_number { get; set; }

        public bool Sucive { get; set; }
        public IEnumerable<SelectListItem> Departaments { get; set; }
        public IEnumerable<SelectListItem> LocationsCiu { get; set; }

        public int IdPadron { get; set; }
        public Guid ServiceAssosiatedId { get; set; }
        public string ServiceType { get; set; }

        public string ReferenceRegex { get; set; }
        public string ReferenceRegex2 { get; set; }
        public string ReferenceRegex3 { get; set; }
        public string ReferenceRegex4 { get; set; }
        public string ReferenceRegex5 { get; set; }
        public string ReferenceRegex6 { get; set; }

        public IOrderedEnumerable<ServiceDto> Services { get; set; }
        public ICollection<ServiceDto> ServicesInContainer { get; set; }

        public Guid ServiceSecondcomboId { get; set; }
        public string ServiceFirstComboName { get; set; } 

        public ServiceDto ServiceToPay { get; set; }
        public Guid ServiceToPayId { get; set; }

        public bool AskReferences { get; set; }
        public bool AskChildService { get; set; }
    }
}