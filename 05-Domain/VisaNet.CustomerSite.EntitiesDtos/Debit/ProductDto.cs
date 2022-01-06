using System;
using System.Collections.Generic;

namespace VisaNet.CustomerSite.EntitiesDtos.Debit
{
    public class ProductDto 
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
        public int? DebitProductid { get; set; }
        public int? DebitMerchantId { get; set; }
        
        public ICollection<ProductPropertyDto> ProductPropertyList { get; set; }
        
    }
}