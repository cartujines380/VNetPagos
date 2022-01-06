namespace VisaNet.LIF.Domain
{
    public class Bin
    {
        public string Value { get; set; }
        public char CardType { get; set; }
        public bool National { get; set; }
        public string IssuingCompany { get; set; }
        public int[] Installments { get; set; }
    }
}
