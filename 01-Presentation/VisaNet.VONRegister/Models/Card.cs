using System.ComponentModel.DataAnnotations;

namespace VisaNet.VONRegister.Models
{
    public class Card
    {
        [Required(AllowEmptyStrings = false)]
        public string Number { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Cvv { get; set; }
    }
}