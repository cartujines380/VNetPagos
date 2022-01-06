using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class BinModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("Bin_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("Bin_Description")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        [CustomDisplay("Bin_Value")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [MaxLength(6, ErrorMessageResourceName = "ExactStringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [MinLength(6, ErrorMessageResourceName = "ExactStringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Value { get; set; }

        [CustomDisplay("Bin_Gateway")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public Guid GatewayId { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string GatewayName { get; set; }

        [CustomDisplay("Bin_Image")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ImageName { get; set; }

        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(ImageName) ? string.Empty : string.Format("{0}{1}", Id.ToString(), ImageName.Substring(ImageName.LastIndexOf("."))); }
        }
        public bool DeleteImage { get; set; }
        public string ImageUrl { get; set; }

        [CustomDisplay("Bin_CardType")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int CardTypeId { get; set; }
        public string CardTypeName { get; set; }

        [CustomDisplay("Bin_AuthorizationAmountType")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int AuthorizationAmountTypeId { get; set; }

        [CustomDisplay("Bin_Bank")]
        public Guid? BankId { get; set; }
        public string BankName { get; set; }

        [CustomDisplay("Bin_Active")]
        public bool Active { get; set; }

        [CustomDisplay("Bin_Country")]
        [StringLength(2, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Country { get; set; }

        public ICollection<BinGroupModel> BinGroups { get; set; }

        public ICollection<BinAuthorizationAmountModel> BinAuthorizationAmountModelList { get; set; }

        public ICollection<ServiceModel> Services { get; set; }

        [CustomDisplay("AffiliationCardId")]
        public Guid? AffiliationCardId { get; set; }
        public virtual AffiliationCardModel AffiliationCardModel { get; set; }
    }
}
