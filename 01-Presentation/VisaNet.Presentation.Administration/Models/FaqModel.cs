using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class FaqModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("FaqDto_Order")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Range(0,250,ErrorMessageResourceName = "Range",ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int Order { get; set; }

        [CustomDisplay("FaqDto_Question")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Question { get; set; }

        [CustomDisplay("FaqDto_Answer")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(1024, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Answer { get; set; }
    }
}
