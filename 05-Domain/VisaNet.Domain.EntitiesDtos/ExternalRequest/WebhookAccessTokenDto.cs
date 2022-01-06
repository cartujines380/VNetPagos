using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WebhookAccessTokenDto
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }
        /// <summary>
        /// UTC TIME
        /// </summary>
        public DateTime CreationDate { get; set; }
        public string Url { get; set; }
        public WebhookAccessStateDto StateDto { get; set; }
    }

    public enum WebhookAccessStateDto
    {
        New = 1,
        Expired = 2,
        Renewed = 3,
        Send = 4,
        Cancelled = 5,
        Paid = 5,
    }

}