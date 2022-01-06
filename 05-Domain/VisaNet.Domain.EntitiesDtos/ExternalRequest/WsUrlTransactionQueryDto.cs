using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{

    public class WsUrlTransactionQueryDto
    {
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public DateTime QueryDate { get; set; }

        public DateTime CreationDate { get; set; }
        public string WcfVersion { get; set; }
    }
}