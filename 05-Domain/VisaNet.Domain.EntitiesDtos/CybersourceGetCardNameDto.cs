using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceGetCardNameDto
    {
        public string Token { get; set; }
        public string MerchantId { get; set; }
        public string MerchantReferenceCode { get; set; }
        public string TransactionKey { get; set; }
        
        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Token", Token },
                {"MerchantId", MerchantId },
                {"MerchantReferenceCode", MerchantReferenceCode },
                {"TransactionKey", TransactionKey }
            };
        }
    }
}
