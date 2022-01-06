using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebSubscriberController : ApiController
    {
        private readonly IServiceSubscriber _serviceSubscriber;

        public WebSubscriberController(IServiceSubscriber serviceSubscriber)
        {
            _serviceSubscriber = serviceSubscriber;
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] SubscriberDto entity)
        {
            try
            {
                var dto = _serviceSubscriber.Create(new SubscriberDto { Name = entity.Name, Surname = entity.Surname, Email = entity.Email });
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        public HttpResponseMessage DeleteByEmail(string email)
        {
            _serviceSubscriber.DeleteByEmail(email);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpGet]
        public HttpResponseMessage ExistsEmail(string email)
        {
            var exists = _serviceSubscriber.ExistsEmail(email);
            return Request.CreateResponse(HttpStatusCode.OK, exists);
        }
    }
}
