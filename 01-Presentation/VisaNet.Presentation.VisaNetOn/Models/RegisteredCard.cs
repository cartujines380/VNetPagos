using System;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class RegisteredCard
    {
        public Guid Id { get; set; }
        public string MaskedNumber { get; set; }
        public DateTime DueDate { get; set; }
        public bool Active { get; set; }
        public bool Expired { get; set; }
    }
}