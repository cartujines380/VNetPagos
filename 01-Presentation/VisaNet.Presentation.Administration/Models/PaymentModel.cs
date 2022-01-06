using System;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class PaymentModel
    {
        [CustomDisplay("PaymentDto_Date")]
        public string Date { get; set; }

        [CustomDisplay("PaymentDto_ClientEmail")]
        public string ClientEmail { get; set; }
        [CustomDisplay("PaymentDto_ClientName")]
        public string ClientName { get; set; }
        [CustomDisplay("PaymentDto_ClientSurname")]
        public string ClientSurname { get; set; }
        [CustomDisplay("PaymentDto_ClientAddress")]
        public string ClientAddress { get; set; }

        [CustomDisplay("PaymentDto_Gateway")]
        public string Gateway { get; set; }
        [CustomDisplay("PaymentDto_UniqueIdentifier")]
        public string UniqueIdentifier{ get; set; }
        [CustomDisplay("PaymentDto_TransactionNumber")]
        public string TransactionNumber { get; set; }
        [CustomDisplay("PaymentDto_PaymentType")]
        public string PaymentType { get; set; }

        [CustomDisplay("PaymentDto_ServiceName")]
        public string ServiceName { get; set; }
        [CustomDisplay("PaymentDto_ServiceCategoryName")]
        public string ServiceCategoryName { get; set; }

        [CustomDisplay("PaymentDto_ReferenceNumber")]
        public string ReferenceNumber { get; set; }
        [CustomDisplay("PaymentDto_PaymentDescription")]
        public string Description { get; set; }

        [CustomDisplay("PaymentDto_CardMaskedNumber")]
        public string CardMaskedNumber { get; set; }
        [CustomDisplay("PaymentDto_CardDueDate")]
        public string CardDueDate { get; set; }
        [CustomDisplay("PaymentDto_CardBin")]
        public string CardBin { get; set; }
        [CustomDisplay("PaymentDto_CardType")]
        public string CardType { get; set; }

        [CustomDisplay("PaymentDto_BillExternalId")]
        public string BillExternalId { get; set; }
        [CustomDisplay("PaymentDto_BillExpirationDate")]
        public string BillExpirationDate { get; set; }
        [CustomDisplay("PaymentDto_BillFinalConsumer")]
        public string BillFinalConsumer { get; set; }
        [CustomDisplay("PaymentDto_BillCurrency")]
        public string BillCurrency { get; set; }
        [CustomDisplay("PaymentDto_BillAmount")]
        public string BillAmount { get; set; }
        [CustomDisplay("PaymentDto_BillTaxedAmount")]
        public string BillTaxedAmount { get; set; }
        [CustomDisplay("PaymentDto_BillDiscountApplied")]
        public string BillDiscountApplied { get; set; }
        [CustomDisplay("PaymentDto_BillDiscountAmount")]
        public string BillDiscountAmount { get; set; }

        [CustomDisplay("PaymentDto_PaymentStatus")]
        public string PaymentStatus { get; set; }

        public int PaymentStatusVal { get; set; }

        public Guid Id { get; set; }

        public string WsBillPaymentOnlinesOperationId { get; set; }
        public string WebhookRegistrationsOperationId { get; set; }
    }
}