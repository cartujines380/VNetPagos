using System.Net;

namespace VisaNet.Domain.Entities
{
    public class MailgunResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Description { get; set; }
    }
}