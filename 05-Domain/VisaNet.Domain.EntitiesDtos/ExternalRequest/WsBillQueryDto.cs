using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WsBillQueryDto
    {
        public Guid Id { get; set; }
        public string IdOperation { get; set; }
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }
        public string IdMerchant { get; set; }
        public string RefClient { get; set; }
        public string RefClient2 { get; set; }
        public string RefClient3 { get; set; }
        public string RefClient4 { get; set; }
        public string RefClient5 { get; set; }
        public string RefClient6 { get; set; }
        public string BillNumber { get; set; }
        public DateTime Date { get; set; }
        public string IdApp { get; set; }
        public int Codresult { get; set; }

        public DateTime CreationDate { get; set; }
        public string WcfVersion { get; set; }
    }

}
