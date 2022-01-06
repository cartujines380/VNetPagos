using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class AutomaticPaymentModel
    {

        public Guid ServiceId { get; set; }
        public String ServiceName { get; set; }

        public string ServiceReferenceName { get; set; }
        public string ServiceReferenceName2 { get; set; }
        public string ServiceReferenceName3 { get; set; }
        public string ServiceReferenceName4 { get; set; }
        public string ServiceReferenceName5 { get; set; }
        public string ServiceReferenceName6 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue2 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue3 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue4 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue5 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceReferenceValue6 { get; set; }

        [CustomDisplay("Automatic_Payment_Max_Amount")]
        public double MaxAmount { get; set; }

        [CustomDisplay("Automatic_Payment_Max_Count")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Range(0, 24, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int MaxCountPayments { get; set; }

        [CustomDisplay("Automatic_Payment_Day")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Range(1, 5, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int DayBeforeExpiration { get; set; }

        public int QuotasDone { get; set; }
        public bool UnlimitedQuotas { get; set; }
        public bool UnlimitedAmount { get; set; }

        public bool FromConfiguration { get; set; }
        [CustomDisplay("Card")]
        public String DefaultCardMask { get; set; }

        [CustomDisplay("Automatic_Payment_SuciveAnnualPatent")]
        public bool SuciveAnnualPatent { get; set; }
        public bool Sucive { get; set; }

        public bool MaxAmountIsNullOrZero { get; set; }
    }

}