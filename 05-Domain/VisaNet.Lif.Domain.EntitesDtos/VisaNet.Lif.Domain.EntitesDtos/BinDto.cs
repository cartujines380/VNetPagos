namespace VisaNet.Lif.Domain.EntitesDtos
{
    public class BinDto
    {
        public string Value { get; set; }
        public char CardType { get; set; }
        public bool National { get; set; }
        public string IssuingCompany { get; set; }
        public int[] Installments { get; set; }
    }
}
