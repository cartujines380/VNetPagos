using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ServiceAssociateNotificationModel
    {
        public ServiceAssociateNotificationModel()
        {
            DaysBeforeDueDate = 5;
        }

        //data from first step
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceImageUrl { get; set; }
        public string TooltipeDesc { get; set; }

        //[RegexValidation("ReferenceRegex")]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        //[RegexValidation("ReferenceRegex2")]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        //[RegexValidation("ReferenceRegex3")]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        //[RegexValidation("ReferenceRegex4")]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        //[RegexValidation("ReferenceRegex5")]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        //[RegexValidation("ReferenceRegex6")]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }

        public string ReferenceRegex { get; set; }
        public string ReferenceRegex2 { get; set; }
        public string ReferenceRegex3 { get; set; }
        public string ReferenceRegex4 { get; set; }
        public string ReferenceRegex5 { get; set; }
        public string ReferenceRegex6 { get; set; }

        //notification step
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Range(1, 5, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int DaysBeforeDueDate { get; set; }


        public bool DaysBeforeDueDateConfigEmail { get; set; }
        public bool DaysBeforeDueDateConfigSms { get; set; }
        public bool DaysBeforeDueDateConfigWeb { get; set; }
        public bool DaysBeforeDueDateConfigActive { get; set; }
        
        public bool SuccessPaymentEmail { get; set; }
        public bool SuccessPaymentSms { get; set; }
        public bool SuccessPaymentWeb { get; set; }
        public bool SuccessPaymentActive { get; set; }

        public bool FailedAutomaticPaymentEmail { get; set; }
        public bool FailedAutomaticPaymentSms { get; set; }
        public bool FailedAutomaticPaymentWeb { get; set; }
        public bool FailedAutomaticPaymentActive { get; set; }

        public bool NewBillEmail { get; set; }
        public bool NewBillSms { get; set; }
        public bool NewBillWeb { get; set; }
        public bool NewBillActive { get; set; }


        public bool ExpiredBillEmail { get; set; }
        public bool ExpiredBillSms { get; set; }
        public bool ExpiredBillWeb { get; set; }
        public bool ExpiredBillActive { get; set; }

        public bool Sucive { get; set; }

        public string MerchantId { get; set; }
    }
}