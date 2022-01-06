using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WsPaymentCancellationDto
    {
        public Guid Id { get; set; }
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public string IdOperacionCobro { get; set; }
        public int Codresult { get; set; }

        public DateTime CreationDate { get; set; }
        public string WcfVersion { get; set; }
    }
}