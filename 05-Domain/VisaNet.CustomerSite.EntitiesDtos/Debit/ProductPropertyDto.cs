using System;

namespace VisaNet.CustomerSite.EntitiesDtos.Debit
{
    public class ProductPropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public int MaxSize { get; set; }
        public int InputSequence { get; set; }
        public bool Requiered { get; set; }
        
        public int? DebitProductPropertyId { get; set; }
        public int? DebitProductId { get; set; }
    }
}