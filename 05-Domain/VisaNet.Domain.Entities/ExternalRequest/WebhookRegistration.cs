using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WebhookRegistrations")]
    public class WebhookRegistration : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public UserDataInput UserData { get; set; }
        [Index("IX_IdApp_IdOperation", 1, IsUnique = true)]
        [MaxLength(100)]
        public string IdApp { get; set; }
        [Index("IX_IdApp_IdOperation", 2, IsUnique = true)]
        [MaxLength(100)]
        public string IdOperation { get; set; }
        public string EnableEmailChange { get; set; }
        public string UrlCallback { get; set; }
        public string IdUsuario { get; set; }
        public string MerchantId { get; set; }
        public string EnableRememberUser { get; set; }

        public WebhookRegistrationActionEnum Action { get; set; }

        public BillDataInput Bill { get; set; }
        public ICollection<WebhookRegistrationLine> BillLines { get; set; }

        public Guid? WebhookAccessTokenId { get; set; }
        public virtual WebhookAccessToken WebhookAccessToken { get; set; }

        [MaxLength(100)]
        public string ReferenceNumber { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber2 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber3 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber4 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber5 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber6 { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public Guid? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        public bool SendEmail { get; set; }
    }

    [Table("WebhookRegistrationsLines")]
    public class WebhookRegistrationLine
    {
        [Key]
        public Guid Id { get; set; }
        public string Order { get; set; }
        public string Amount { get; set; }
        public string Concept { get; set; }
    }

    public enum WebhookRegistrationActionEnum
    {
        Payment = 1,
        Tokenization = 2,
        Association = 3
    }

}