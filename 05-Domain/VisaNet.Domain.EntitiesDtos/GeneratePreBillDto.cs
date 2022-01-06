using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class GeneratePreBillDto
    {
        public IList<BillDto> SelectedBills { get; set; }
        public Guid ServiceId { get; set; }
    }
}
