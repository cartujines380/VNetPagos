using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WebServiceBillsInputDto
    {
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }
        public bool ReplaceBills { get; set; }
        public ICollection<HighwayBillDto> HighwayBillDtos { get; set; }
        public string DigitalFirm { get; set; }
    }
}
