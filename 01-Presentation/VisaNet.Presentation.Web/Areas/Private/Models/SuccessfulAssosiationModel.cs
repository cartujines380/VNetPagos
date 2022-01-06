using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class SuccessfulAssosiationModel
    {
        public Guid CardId { get; set; }
        public Guid ServiceId { get; set; }

        [CustomDisplay("Card")]
        public String CardMask { get; set; }

        public String ServiceName { get; set; }

        [CustomDisplay("Automatic_Payment_Max_Amount")]
        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
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

        [CustomDisplay("Automatic_Payment_SuciveAnnualPatent")]
        public bool SuciveAnnualPatent { get; set; }
        public bool Sucive { get; set; }

        public bool EnableAutomaticPayment { get; set; }

        public bool MaxAmountIsNullOrZero { get; set; }
    }
}