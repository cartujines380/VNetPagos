using System;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WebhookDownDto
    {
        public Guid Id { get; set; }
        public string IdApp { get; set; }
        public string IdUser { get; set; }
        public string IdCard { get; set; }
        public string IdOperation { get; set; }

        public string HttpResponseCode { get; set; }

        public DateTime CreationDate { get; set; }
    }
}