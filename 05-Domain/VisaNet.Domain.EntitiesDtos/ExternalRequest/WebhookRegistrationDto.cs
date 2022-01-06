using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WebhookRegistrationDto
    {
        public Guid Id { get; set; }
        public UserDataInputDto UserData { get; set; }
        public string IdApp { get; set; }
        public string EnableEmailChange { get; set; }
        public string IdOperation { get; set; }
        public string UrlCallback { get; set; }
        public string MerchantId { get; set; }
        public string IdUsuario { get; set; }
        public string EnableRememberUser { get; set; }

        public WebhookRegistrationActionEnumDto Action { get; set; }

        public BillDataInputDto Bill { get; set; }
        public IList<WebhookRegistrationLineDto> BillLines { get; set; }

        public DateTime CreationDate { get; set; }

        public WebhookAccessTokenDto WebhookAccessTokenDto { get; set; }

        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }

        public string FirmaDigital { get; set; }

        public Guid? PaymentId { get; set; }
        public PaymentDto PaymentDto { get; set; }

        public bool? SendEmail { get; set; }
    }

    public class WebhookRegistrationLineDto
    {
        public string Order { get; set; }
        public string Amount { get; set; }
        public string Concept { get; set; }
    }

    public enum WebhookRegistrationActionEnumDto
    {
        Payment = 1,
        Tokenization = 2,
        Association = 3
    }

}
