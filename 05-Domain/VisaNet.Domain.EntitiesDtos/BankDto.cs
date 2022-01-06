using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BankDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int Code { get; set; }

        public string QuotesPermited { get; set; }
        public virtual ICollection<BinDto> BinsDto { get; set; }
        public virtual ICollection<AffiliationCardDto> AffiliationCardDto { get; set; }
    }
}
