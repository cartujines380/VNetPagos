namespace VisaNet.Lif.Domain.EntitesDtos
{
    public class BillDto
    {
        public double TaxedAmount { get; set; }
        public double Amount { get; set; }
        public bool IsFinalConsumer { get; set; }
        public string Currency { get; set; }
        public int LawId { get; set; }
    }
}