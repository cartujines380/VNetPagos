using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Web.Models
{
    public class CardModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("CardDto_Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(25, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Number { get; set; }

        [CustomDisplay("CardDto_SecurityCode")]
        [StringLength(5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string SecurityCode { get; set; }

        public string PaymentToken { get; set; }

        [CustomDisplay("CardDto_DueDate")]
        public string DueDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public List<BillDto> Bills { get; set; }
        public bool Active { get; set; }
    }
}