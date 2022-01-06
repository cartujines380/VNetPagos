using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ParametersDto
    {
        public Guid Id { get; set; }

        public EmailDto Contact { get; set; }

        public EmailDto ErrorNotification { get; set; }

        public EmailDto SendingEmail { get; set; }

        public BankCodeDto Banred { get; set; }
        public BankCodeDto Sistarbanc { get; set; }
        public BankCodeDto SistarbancBrou { get; set; }
        public BankCodeDto Cybersource { get; set; }
        public BankCodeDto Sucive { get; set; }
        public BankCodeDto Geocom { get; set; }

        public int LoginNumberOfTries { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MerchantId { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceProfileId { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceAccessKey { get; set; }
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceSecretKey { get; set; }
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceTransactionKey { get; set; }
    }
}
