using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class DiscountModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("DiscountDto_CardType")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int CardTypeId { get; set; }

        [CustomDisplay("DiscountDto_From")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public DateTime From { get; set; }

        [CustomDisplay("DiscountDto_To")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public DateTime To { get; set; }

        [CustomDisplay("DiscountDto_Fixed")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int Fixed { get; set; }

        [CustomDisplay("DiscountDto_Additional")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int Additional { get; set; }

        [CustomDisplay("DiscountDto_MaximumAmount")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int MaximumAmount { get; set; }

        [CustomDisplay("DiscountDto_DiscountType")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int DiscountType { get; set; }

        [CustomDisplay("DiscountDto_DiscountLabel")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int DiscountLabel { get; set; }
    }
}
