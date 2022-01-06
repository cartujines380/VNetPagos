using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.VONRegister.Models
{
    public class Payment
    {
        [RegexValidation("Reference1Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference1Value { get; set; }
        public string Reference1Name { get; set; }
        public string Reference1Regex { get; set; }

        [RegexValidation("Reference2Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference2Value { get; set; }
        public string Reference2Name { get; set; }
        public string Reference2Regex { get; set; }

        [RegexValidation("Reference3Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference3Value { get; set; }
        public string Reference3Name { get; set; }
        public string Reference3Regex { get; set; }

        [RegexValidation("Reference4Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference4Value { get; set; }
        public string Reference4Name { get; set; }
        public string Reference4Regex { get; set; }

        [RegexValidation("Reference5Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference5Value { get; set; }
        public string Reference5Name { get; set; }
        public string Reference5Regex { get; set; }

        [RegexValidation("Reference6Regex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Reference6Value { get; set; }
        public string Reference6Name { get; set; }
        public string Reference6Regex { get; set; }

        [CustomDisplay("PaymentDto_Description")]
        [StringLength(45, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public string ToolTipImage { get; set; }
    }
}