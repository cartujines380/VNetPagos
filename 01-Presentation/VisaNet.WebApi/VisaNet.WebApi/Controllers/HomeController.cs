using System.Net;
using System.Net.Http;

namespace VisaNet.WebApi.Controllers
{
    public class HomeController : MailgunController
    {
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);     
        }

    }
}