using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankPaymentInputDto
    {
        //Factura
        public string BillId { get; set; }
        public string BillNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string GatewayTransactionId { get; set; }
        //public string GatewayTransactionBrouId { get; set; }
        //public string BanredTransactionId { get; set; }
        public bool Payable { get; set; }
        public bool FinalConsumer { get; set; }
        public bool DiscountApplyed { get; set; }
        public double TotalTaxedAmount { get; set; }
        public double TotalAmount { get; set; }
        public double Discount { get; set; }
        public double AmountToCybersource { get; set; }
        public string Gateway { get; set; }
        //GUID 
        public string MerchantReferenceCode { get; set; }
        public string SucivePreBillNumber { get; set; }
        public string TransactionCreationDate { get; set; }

        //CyberSource
        public string AuthAmount { get; set; }
        public string AuthTime { get; set; }
        public string AuthCode { get; set; }
        public string AuthAvsCode { get; set; }
        public string AuthResponse { get; set; }
        public string AuthTransRefNo { get; set; }
        public string Decision { get; set; }
        public string BillTransRefNo { get; set; }
        public string PaymentToken { get; set; }
        public string ReasonCode { get; set; }
        public string ReqAmount { get; set; }
        public string ReqCurrency { get; set; }
        public string TransactionId { get; set; }
        public string ReqTransactionUuid { get; set; }
        public string ReqReferenceNumber { get; set; }
        public string ReqTransactionType { get; set; }

        //Usuario
        public string Email { get; set; }
        public string Ci { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        //Tarjeta
        public int CardBinNumbers { get; set; }
        public string CardMaskedNumber { get; set; }
        public string CardDueDate { get; set; }
        public string CardName { get; set; }
        public string CardPhone { get; set; }

        public string ServiceId { get; set; }
        public string PaymentPlatform { get; set; }

        public string ServiceReferenceNumber { get; set; }
        public string ServiceReferenceNumber2 { get; set; }
        public string ServiceReferenceNumber3 { get; set; }
        public string ServiceReferenceNumber4 { get; set; }
        public string ServiceReferenceNumber5 { get; set; }
        public string ServiceReferenceNumber6 { get; set; }

        //NUEVO
        public string DiscountObjId { get; set; }
    }
}
