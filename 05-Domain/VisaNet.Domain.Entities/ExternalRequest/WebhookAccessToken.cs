using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WebhookAccessTokens")]
    public class WebhookAccessToken : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        public string AccessToken { get; set; }
        public DateTime CreationDate { get; set; }

        public WebhookAccessState State { get; set; }
    }

    public enum WebhookAccessState
    {
        New = 1,
        Expired = 2,
        Renewed = 3,
        Send = 4,
        Cancelled = 5,
        Paid = 6,
    }

}