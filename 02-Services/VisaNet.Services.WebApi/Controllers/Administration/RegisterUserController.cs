using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class RegisterUserController : ApiController
    {
        private readonly IServiceApplicationUser _serviceApplicationUser;

        public RegisterUserController(IServiceApplicationUser serviceApplicationUser)
        {
            _serviceApplicationUser = serviceApplicationUser;
        }

        [HttpPut]
        //[WebApiAuthentication(Actions.application)]
        public HttpResponseMessage Put([FromBody] ApplicationUserCreateEditDto entity)
        {
             _serviceApplicationUser.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}
