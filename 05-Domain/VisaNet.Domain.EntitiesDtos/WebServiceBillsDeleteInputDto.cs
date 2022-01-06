using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WebServiceBillsDeleteDto
    {
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }
        public List<string> NroFacturas{ get; set; }
    }
}
