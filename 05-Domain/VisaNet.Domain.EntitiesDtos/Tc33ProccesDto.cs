using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class Tc33ProccesDto
    {
        public string AuthorizationDate { get; set; }
        public string SourceAmount { get; set; }
        public string SourceCurrencyCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string CardAcceptorId { get; set; }
        public string TerminalId { get; set; }
        public string CommercePaymentIndicator { get; set; }
        public string RequestId { get; set; }
        public string TransactionType { get; set; }

        public DateTime? TransactionDate { get; set; }

    }
}
