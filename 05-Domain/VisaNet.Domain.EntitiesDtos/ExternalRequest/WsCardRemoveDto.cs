using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{

    public class WsCardRemoveDto
    {
        public Guid Id { get; set; }
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public string IdCard { get; set; }
        public string IdUser { get; set; }

        public int Codresult { get; set; }

        public DateTime CreationDate { get; set; }
        public string WcfVersion { get; set; }
    }
}