using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DiscountQueryDto
    {
        public int BinNumber { get; set; }
        public Guid ServiceId { get; set; }
        public IList<BillDto> Bills { get; set; }
    }
}