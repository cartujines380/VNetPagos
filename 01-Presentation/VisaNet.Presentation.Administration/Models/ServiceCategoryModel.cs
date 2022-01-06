using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceCategoryModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [CustomDisplay("ServiceCategoryModel_Name")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
    }
}