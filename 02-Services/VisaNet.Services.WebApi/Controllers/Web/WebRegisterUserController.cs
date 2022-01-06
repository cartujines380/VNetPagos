using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebRegisterUserController : ApiController
    {
        private readonly IServiceApplicationUser _serviceApplicationUser;

        public WebRegisterUserController(IServiceApplicationUser serviceApplicationUser)
        {
            _serviceApplicationUser = serviceApplicationUser;
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] ApplicationUserCreateEditDto entity)
        {
            _serviceApplicationUser.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPut]
        public HttpResponseMessage CreateUserWithoutPassword([FromBody] ApplicationUserCreateEditDto entity)
        {
            var dto = _serviceApplicationUser.CreateUserWithoutPassword(entity);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

    }
}