
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class TransactionCommerceResult
    {
        public string CsTransactionNumber { get; set; }
        public int OperationResult { get; set; }

        public List<ServiceDto> Commerces { get; set; }
    }
}
