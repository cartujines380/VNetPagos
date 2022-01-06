using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebAutoCompleteController : ApiController
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceApplicationUser _serviceApplicationUser;

        public WebAutoCompleteController(IServiceService serviceService,IServiceApplicationUser serviceApplicationUser)
        {
            _serviceService = serviceService;
            _serviceApplicationUser = serviceApplicationUser;
        }

        [HttpGet]
        public HttpResponseMessage AutoCompleteServices(string contains)
        {
            var result = _serviceService.GetServicesAutoComplete(contains);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage AutoCompleteApplicationUsers(string contains)
        {
            var result = _serviceApplicationUser.GetApplicationUserAutoComplete(contains);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
