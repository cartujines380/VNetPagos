using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceContainerModel
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

        [CustomDisplay("Service_Description")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

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

        [CustomDisplay("Service_Active")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public bool Active { get; set; }

        [CustomDisplay("Service_PostAssociationDesc")]
        public string PostAssociationDesc { get; set; }

        [CustomDisplay("Service_TermsAndConditions")]
        public string TermsAndConditions { get; set; }

        [CustomDisplay("Service_UrlName")]
        public string UrlName { get; set; }

        [CustomDisplay("Service_ExternalUrlAdd")]
        public string ExternalUrlAdd { get; set; }
        [CustomDisplay("Service_ExternalUrlRemove")]
        public string ExternalUrlRemove { get; set; }

        [CustomDisplay("CertificatePublicThumbprintExternal")]
        public string CertificateThumbprintExternal { get; set; }
        [CustomDisplay("CertificatePrivateThumbprintVisa")]
        public string CertificateThumbprintVisa { get; set; }

        public bool Container { get; set; }

        [CustomDisplay("AllowSelectContentAssociation")]
        public bool AllowSelectContentAssociation { get; set; }
        [CustomDisplay("AllowSelectContentPayment")]
        public bool AllowSelectContentPayment { get; set; }
        [CustomDisplay("AllowMultipleCards")]
        public bool AllowMultipleCards { get; set; }
        [CustomDisplay("MaxQuotaAllow")]
        public int MaxQuotaAllow { get; set; }

        [CustomDisplay("Service_EnablePublicPayment")]
        public bool EnablePublicPayment { get; set; }
        [CustomDisplay("Service_EnablePrivatePayment")]
        public bool EnablePrivatePayment { get; set; }
        [CustomDisplay("Service_EnableAssociation")]
        public bool EnableAssociation { get; set; }
        [CustomDisplay("Service_AskUserForReferences")]
        public bool AskUserForReferences { get; set; }

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

        public bool HasReferences { get; set; }

        public ICollection<ServiceGatewayModel> ServiceGateways { get; set; }

        public string ContentIntro { get; set; }

        [CustomDisplay("Service_BinGroups")]
        public List<Guid> BinGroups { get; set; }

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


        [CustomDisplay("InformAffiliationCard")]
        public bool InformAffiliationCard { get; set; }

        public bool IsDetailsView { get; set; }

        public bool PropagateChangesToChildServices { get; set; }

    }
}