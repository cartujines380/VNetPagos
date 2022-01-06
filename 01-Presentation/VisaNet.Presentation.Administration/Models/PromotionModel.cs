using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class PromotionModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("Promotion_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("Promotion_State")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public bool Active { get; set; }

        [CustomDisplay("Promotion_Image")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ImageName { get; set; }

        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(ImageName) ? string.Empty : string.Format("{0}{1}", Id.ToString(), ImageName.Substring(ImageName.LastIndexOf("."))); }
        }

        public string ImageUrl { get; set; }

    }
}
