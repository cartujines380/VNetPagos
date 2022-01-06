using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceModel
    {
        public Guid Id { get; set; }
        [CustomDisplay("Service_CategoryName")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; }
        [CustomDisplay("Service_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        public int Departament { get; set; }

        [CustomDisplay("Service_Description")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }
        [CustomDisplay("Service_DescriptionTooltip")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string DescriptionTooltip { get; set; }
        [CustomDisplay("Service_LinkId")]
        public string LinkId { get; set; }
        [CustomDisplay("Service_Tags")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Tags { get; set; }

        [CustomDisplay("Service_Image")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Image { get; set; }
        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(Image) ? string.Empty : string.Format("{0}{1}", Id.ToString(), Image.Substring(Image.LastIndexOf("."))); }
        }
        public bool DeleteImage { get; set; }
        public string ImagePath { get; set; }

        [CustomDisplay("Service_Image_tooltip")]
        public string ImageTooltip { get; set; }
        public string ImageTooltipBlobName
        {
            get { return string.IsNullOrEmpty(ImageTooltip) ? string.Empty : string.Format("{0}_tooltip{1}", Id.ToString(), ImageTooltip.Substring(ImageTooltip.LastIndexOf("."))); }
        }
        public bool DeleteImageTooltip { get; set; }
        public string ImageTooltipPath { get; set; }

        [CustomDisplay("Service_Active")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public bool Active { get; set; }

        [CustomDisplay("Service_CybersourceId")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MerchantId { get; set; }
        [CustomDisplay("CybersourceProfileId")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceProfileId { get; set; }
        [CustomDisplay("CybersourceAccessKey")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceAccessKey { get; set; }
        [CustomDisplay("CybersourceSecretKey")]
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceSecretKey { get; set; }
        [CustomDisplay("CybersourceTransactionKey")]
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceTransactionKey { get; set; }


        [CustomDisplay("Service_ReferenceParamName")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName { get; set; }
        [CustomDisplay("Service_ReferenceParamName2")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName2 { get; set; }
        [CustomDisplay("Service_ReferenceParamName3")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName3 { get; set; }
        [CustomDisplay("Service_ReferenceParamName4")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName4 { get; set; }
        [CustomDisplay("Service_ReferenceParamName5")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName5 { get; set; }
        [CustomDisplay("Service_ReferenceParamName6")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName6 { get; set; }

        [CustomDisplay("Service_ReferenceRegex")]
        public string ReferenceParamRegex { get; set; }
        [CustomDisplay("Service_ReferenceRegex2")]
        public string ReferenceParamRegex2 { get; set; }
        [CustomDisplay("Service_ReferenceRegex3")]
        public string ReferenceParamRegex3 { get; set; }
        [CustomDisplay("Service_ReferenceRegex4")]
        public string ReferenceParamRegex4 { get; set; }
        [CustomDisplay("Service_ReferenceRegex5")]
        public string ReferenceParamRegex5 { get; set; }
        [CustomDisplay("Service_ReferenceRegex6")]
        public string ReferenceParamRegex6 { get; set; }

        //Referencias del servicio contenedor
        public bool ContainerHasReferences { get; set; }
        [CustomDisplay("Service_ReferenceParamName")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName { get; set; }
        [CustomDisplay("Service_ReferenceParamName2")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName2 { get; set; }
        [CustomDisplay("Service_ReferenceParamName3")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName3 { get; set; }
        [CustomDisplay("Service_ReferenceParamName4")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName4 { get; set; }
        [CustomDisplay("Service_ReferenceParamName5")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName5 { get; set; }
        [CustomDisplay("Service_ReferenceParamName6")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ContainerReferenceParamName6 { get; set; }

        [CustomDisplay("Service_ReferenceRegex")]
        public string ContainerReferenceParamRegex { get; set; }
        [CustomDisplay("Service_ReferenceRegex2")]
        public string ContainerReferenceParamRegex2 { get; set; }
        [CustomDisplay("Service_ReferenceRegex3")]
        public string ContainerReferenceParamRegex3 { get; set; }
        [CustomDisplay("Service_ReferenceRegex4")]
        public string ContainerReferenceParamRegex4 { get; set; }
        [CustomDisplay("Service_ReferenceRegex5")]
        public string ContainerReferenceParamRegex5 { get; set; }
        [CustomDisplay("Service_ReferenceRegex6")]
        public string ContainerReferenceParamRegex6 { get; set; }


        public ICollection<ServiceGatewayModel> ServiceGateways { get; set; }

        [CustomDisplay("Service_EnableMultipleBills")]
        public bool EnableMultipleBills { get; set; }

        [CustomDisplay("Service_EnableAutomaticPayment")]
        public bool EnableAutomaticPayment { get; set; }
        [CustomDisplay("Service_EnablePublicPayment")]
        public bool EnablePublicPayment { get; set; }
        [CustomDisplay("Service_EnablePrivatePayment")]
        public bool EnablePrivatePayment { get; set; }
        [CustomDisplay("Service_EnablePartialPayment")]
        public bool EnablePartialPayment { get; set; }
        [CustomDisplay("Service_EnableAssociation")]
        public bool EnableAssociation { get; set; }
        [CustomDisplay("Service_AskUserForReferences")]
        public bool AskUserForReferences { get; set; }
        [CustomDisplay("AllowMultipleCards")]
        public bool AllowMultipleCards { get; set; }
        [CustomDisplay("MaxQuotaAllow")]
        public int MaxQuotaAllow { get; set; }

        public ICollection<ServiceEnableEmailModel> ServiceEnableEmailModel { get; set; }
        [CustomDisplay("ExtractEmail")]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ExtractEmail { get; set; }

        [CustomDisplay("Service_PostAssociationDesc")]
        public string PostAssociationDesc { get; set; }

        [CustomDisplay("Service_TermsAndConditions")]
        public string TermsAndConditions { get; set; }

        [CustomDisplay("Service_UrlName")]
        public string UrlName { get; set; }

        [CustomDisplay("Service_DiscountType")]
        public int DiscountType { get; set; }

        [CustomDisplay("Service_ServiceContainer")]
        public string ServiceContainerId { get; set; }

        [CustomDisplay("Service_ExternalUrlAdd")]
        public string ExternalUrlAdd { get; set; }
        [CustomDisplay("Service_ExternalUrlRemove")]
        public string ExternalUrlRemove { get; set; }

        [CustomDisplay("CertificatePublicThumbprintExternal")]
        public string CertificateThumbprintExternal { get; set; }
        [CustomDisplay("CertificatePrivateThumbprintVisa")]
        public string CertificateThumbprintVisa { get; set; }

        public bool Container { get; set; }

        [AllowHtml]
        public string ContentIntro { get; set; }

        [CustomDisplay("Service_BinGroups")]
        public List<Guid> BinGroups { get; set; }

        [CustomDisplay("InterpreterId")]
        public string InterpreterId { get; set; }
        [CustomDisplay("InterpreterAuxParam")]
        public string InterpreterAuxParam { get; set; }

        [CustomDisplay("InformCardBank")]
        public bool InformCardBank { get; set; }
        [CustomDisplay("InformCardType")]
        public bool InformCardType { get; set; }
        [CustomDisplay("UrlIntegrationVersion")]
        public int UrlIntegrationVersion { get; set; }
        [CustomDisplay("AllowVon")]
        public bool AllowVon { get; set; }
        [CustomDisplay("AllowWcfPayment")]
        public bool AllowWcfPayment { get; set; }

        [CustomDisplay("Service_Sort")]
        [Range(0, 99, ErrorMessageResourceName = "RangeExceeded", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int Sort { get; set; }

        public bool UploadImageDisabled { get; set; }

        [CustomDisplay("InformAffiliationCard")]
        public bool InformAffiliationCard { get; set; }

        public bool IsDetailsView { get; set; }
        public bool IsEditView { get; set; }


        //Select Lists
        public ICollection<SelectListItem> BinGroupsSelectList { get; set; }
    }
}