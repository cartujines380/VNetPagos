using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebAnonymousUserController : ApiController
    {
        private readonly IServiceAnonymousUser _serviceAnonymousUser;

        public WebAnonymousUserController(IServiceAnonymousUser serviceAnonymousUser)
        {
            _serviceAnonymousUser = serviceAnonymousUser;
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var user = _serviceAnonymousUser.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage GetByEmail(string email)
        {
            var user = _serviceAnonymousUser.GetUserByEmailIdentityNumber(email, null);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpPut]
        public HttpResponseMessage Put(AnonymousUserDto entity)
        {
            var anonymousUser = _serviceAnonymousUser.Create(entity, true);
            return Request.CreateResponse(HttpStatusCode.OK, anonymousUser);
        }

        [HttpPost]
        public HttpResponseMessage Post(AnonymousUserDto entity)
        {
            _serviceAnonymousUser.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage CreateOrEditAnonymousUser(AnonymousUserDto entity)
        {
            var user = _serviceAnonymousUser.CreateOrEditAnonymousUser(entity);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

    }
}
