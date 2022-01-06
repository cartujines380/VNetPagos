namespace VisaNet.Domain.EntitiesDtos
{
    public class TransactionsAmountDto
    {
        public string Name { get; set; }

        public double ValuePesos { get; set; }
        public double ValueDollars { get; set; }
        public double ValueTotal { get; set; }
    }
}