using System;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationCybersourceDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string RequestId { get; set; }
        public string MerchantReferenceNumber { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }

        public int Correct { get; set; }
        public string Source { get; set; }
        public string IcsApplications { get; set; }
        public string MerchandId { get; set; }
        public string DateString { get; set; }
        public bool PaymentDone { get; set; }

        public int CreditRcode { get; set; }
        public int AuthReversalRcode { get; set; }

        public TransactionType TransactionType { get; set; }

    }
}
