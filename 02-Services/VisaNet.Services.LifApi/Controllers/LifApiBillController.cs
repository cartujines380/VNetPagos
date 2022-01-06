using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Services.LifApi.Models;

namespace VisaNet.Services.LifApi.Controllers
{
    public class LifApiBillController : ApiController
    {
        private readonly IServiceLifApiBill _lifApiBillService;

        public LifApiBillController(IServiceLifApiBill lifApiBillService)
        {
            _lifApiBillService = lifApiBillService;
        }

        [HttpPost]
        public HttpResponseMessage Create(LifApiBillModel model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _lifApiBillService.Create(model.LifApiBill));
        }
    }
}