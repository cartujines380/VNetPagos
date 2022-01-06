namespace VisaNet.Presentation.Administration.Models
{
    public class PaymentExcelModel
    {
        public string ServiceName { get; set; }
        public string GatewayName { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public string ServiceCategoryName { get; set; }
        public string CSTransactionIdentifier { get; set; }
        public string PaymentUniqueIdentifier { get; set; }

        public string BillTaxedAmount { get; set; }
        public string BillAmount { get; set; }
        public string BillCurrency { get; set; }
        public string BillDiscount { get; set; }
        public string BillDiscountAmount { get; set; }

        public string CardDueDate { get; set; }
        public string CardMaskedNumber { get; set; }
        public string CardType { get; set; }

        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }

        public string PaymentStatus { get; set; }
        public string TransactionNumber { get; set; }
        public string PaymentCSRequestCurrency { get; set; }
        public string PaymentCSAuthCode { get; set; }
        public string PaymentCSAuthTime { get; set; }
        public string PaymentTotalAmount { get; set; }
        public string PaymentTaxedAmount { get; set; }
        public string PaymentDiscount { get; set; }
        public string PaymentAmountToCS { get; set; }

        public string BillExpirationDate { get; set; }
        public string BillExternalId { get; set; }
        public string GatewayTransactionId { get; set; }
        public string BillFinalConsumer { get; set; }
        public string BillSucivePreBillNumber { get; set; }

        public string ServiceAssociatedDescription { get; set; }
        public string ReferenceNumbers { get; set; }
    }
}