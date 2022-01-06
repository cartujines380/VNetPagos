using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class TestController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "OnLine");
        }
                
        public HttpResponseMessage Get(int p)
        {
            return Request.CreateResponse(HttpStatusCode.OK, p);
        }
    }
}
