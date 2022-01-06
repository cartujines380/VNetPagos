using System;

namespace VisaNet.Presentation.Administration.Models
{
    public class WebhookRegistrationModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public string UrlCallback { get; set; }
        public string MerchantId { get; set; }
        public string Action { get; set; }
        public string EnableEmailChange { get; set; }
        public string EnableRememberUser { get; set; }
        public string SendEmail { get; set; }

        public string IdUsuario { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserIdentityNumber { get; set; }
        public string UserMobileNumber { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserAddress { get; set; }

        public string BillExternalId { get; set; }
        public string BillAmount { get; set; }
        public string BillTaxedAmount { get; set; }
        public string BillCurrency { get; set; }
        public string BillFinalConsumer { get; set; }
        public string BillGenerationDate { get; set; }
        public string BillQuota { get; set; }
        public string BillDescription { get; set; }
        public string BillExpirationDate { get; set; }

        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }

        public Guid? PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTransactionNumber { get; set; }
    }
}