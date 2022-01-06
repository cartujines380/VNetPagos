namespace VisaNet.LIF.Domain.Entities
{
    public class Bill
    {
        public double TaxedAmount { get; set; }
        public double Amount { get; set; }
        public bool IsFinalConsumer { get; set; }
        public int Currency { get; set; }
        public int LawId { get; set; }
    }
}