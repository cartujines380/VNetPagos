namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceTransactionsDataDto
    {
        public CsResponseData PaymentData { get; set; }
        public CsResponseData VoidData { get; set; }
        public CsResponseData RefundData { get; set; }
        public CsResponseData ReversalData { get; set; }
        public CsResponseData TokenizationData { get; set; }

        public PaymentDto PaymentDto { get; set; }
        
        public CyberSourceDataDto CyberSourceData{ get; set; }
        public VerifyByVisaDataDto VerifyByVisaData { get; set; }

        public CyberSourceMerchantDefinedDataDto CyberSourceMerchantDefinedData { get; set; }

        public DebitRequestDto DebitRequestDto { get; set; }
    }
}
