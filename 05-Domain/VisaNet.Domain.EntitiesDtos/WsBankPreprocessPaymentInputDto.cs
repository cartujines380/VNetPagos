using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankPreprocessPaymentInputDto
    {
        public string ServiceId { get; set; }

        public string ServiceType { get; set; }

        public bool MultipleBillsAllowed { get; set; }

        public string ServiceReferenceNumber { get; set; }
        public string ServiceReferenceNumber2 { get; set; }
        public string ServiceReferenceNumber3 { get; set; }
        public string ServiceReferenceNumber4 { get; set; }
        public string ServiceReferenceNumber5 { get; set; }
        public string ServiceReferenceNumber6 { get; set; }

        public int CardBinNumbers { get; set; }

        public string MerchantReferenceCode { get; set; }

        public List<BillDto> Bills { get; set; }
    }
}
