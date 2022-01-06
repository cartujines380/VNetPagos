using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ServiceCategoryDto
    {
        public Guid Id { get; set; }

        [CustomDisplay("ServiceCategoryDto_Name")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
    }
}
