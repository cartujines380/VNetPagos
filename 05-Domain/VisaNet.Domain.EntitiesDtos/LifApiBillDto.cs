using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class LifApiBillDto
    {
        public Guid Id { get; set; }

        public string AppId { get; set; }
        public string OperationId { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }
        public double TaxedAmount { get; set; }
        public bool IsFinalConsumer { get; set; }
        public int LawId { get; set; }

        public string BinValue { get; set; }
        public string CardType { get; set; }
        public string IssuingCompany { get; set; }
        public double DiscountAmount { get; set; }
        public double AmountToCyberSource { get; set; }

        public string Error { get; set; }

        public DateTime CreationDate { get; set; }
    }
}