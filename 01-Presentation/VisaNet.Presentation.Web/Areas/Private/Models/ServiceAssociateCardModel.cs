using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ServiceAssociateCardModel
    {

        //data from first step
        public string ServiceName { get; set; }
        public string ServiceImageUrl { get; set; }

        public Guid ServiceAssosiateId { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }


        [CustomDisplay("CardDto_Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        [StringLength(25, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Number { get; set; }

        [CustomDisplay("CardDto_SecurityCode")]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string SecurityCode { get; set; }
        
        [CustomDisplay("CardDto_DueDate")]
        public string DueDate { get; set; }

        public bool Sucive { get; set; }

        public string MerchantId { get; set; }

        [CustomDisplay("CardDto_Description")]
        public string Description { get; set; }
    }
}