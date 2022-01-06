using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CancelTrnsDto
    {
        public Guid PaymentId { get; set; }
        public bool Notify{ get; set; }
        public string TransactionNumber { get; set; }
    }
}
