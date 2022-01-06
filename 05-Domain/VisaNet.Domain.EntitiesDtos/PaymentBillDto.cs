using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class PaymentBillDto
    {
        public Guid PaymentId { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Guid GatewayId { get; set; }
        public string GatewayName { get; set; }
        public int GatewayEnum { get; set; }
        public Guid? CardId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentType { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; }
        public string CSTransactionIdentifier { get; set; }
        public Int64 PaymentUniqueIdentifier { get; set; }

        public double BillTaxedAmount { get; set; }
        public double BillAmount { get; set; }
        public string BillCurrency { get; set; }
        public int BillDiscount { get; set; }
        public double BillDiscountAmount { get; set; }

        public DateTime CardDueDate { get; set; }
        public string CardMaskedNumber { get; set; }

        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }

        public int PaymentStatus { get; set; }
        public string TransactionNumber { get; set; }
        public string PaymentCSRequestCurrency { get; set; }
        public string PaymentCSRequestAmount { get; set; }
        public string PaymentCSAuthCode { get; set; }
        public string PaymentCSAuthAmount { get; set; }
        public string PaymentCSAuthTime { get; set; }
        public double PaymentTotalAmount { get; set; }
        public double PaymentTaxedAmount { get; set; }
        public double PaymentDiscount { get; set; }
        public double PaymentAmountToCS { get; set; }

        public DateTime BillExpirationDate { get; set; }
        public string BillExternalId { get; set; }
        public string GatewayTransactionId { get; set; }
        public bool BillFinalConsumer { get; set; }
        public string BillSucivePreBillNumber { get; set; }

        public Guid? PaymentServiceAssociatedId { get; set; }
        public string ServiceAssociatedDescription { get; set; }

        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }

        public int PaymentPlatform { get; set; }

        public string WsBillPaymentOnlinesOperationId { get; set; }
        public string WebhookRegistrationsOperationId { get; set; }
    }
}
