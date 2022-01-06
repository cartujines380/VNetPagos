using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationBanredDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Int64 VisaTransactionId { get; set; }

        public string ReferenceNumber { get; set; }

        public string BillExternalId { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }
    }
}
