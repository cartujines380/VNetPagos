using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public abstract class KeysInfoBasic : IGenerateToken
    {
        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }
        public string RedirectTo { get; set; }
        public string UrlReturn { get; set; }
        public string CallcenterUser { get; set; }
        public string OperationId { get; set; }
        public string Platform { get; set; }
        public PaymentTypeDto PaymentTypeDto { get; set; }
        public CardTypeDto CardTypeDto { get; set; }
        public string TemporaryTransactionIdentifier { get; set; }
        public string CyberSourceIdentifier { get; set; }
        public OperationTypeDto OperationTypeDto { get; set; }
        public string NameTh { get; set; }
        public string CardBin { get; set; }
        public string TransactionReferenceNumber { get; set; }
        public string FingerPrint { get; set; }
    }
}