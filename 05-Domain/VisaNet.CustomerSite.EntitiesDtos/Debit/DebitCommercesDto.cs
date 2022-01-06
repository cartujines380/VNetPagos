using System;
using System.Collections.Generic;

namespace VisaNet.CustomerSite.EntitiesDtos.Debit
{
    public class DebitCommercesDto
    {
        public List<CustomerSiteCommerceDto> Commerces { get; set; }
        public DateTime Date { get; set; }
    }
}
