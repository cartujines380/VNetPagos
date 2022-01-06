using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class QuotationDto
    {
        public Guid Id { get; set; }

        public DateTime DateFrom { get; set; }

        public string Currency { get; set; }

        public double ValueInPesos { get; set; }
    }
}
