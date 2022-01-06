using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebApplicationUserController : ApiController
    {
        private readonly IServiceApplicationUser _serviceApplicationUser;

        public WebApplicationUserController(IServiceApplicationUser serviceApplicationUser)
        {
            _serviceApplicationUser = serviceApplicationUser;
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri] ApplicationUserFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var user = _serviceApplicationUser.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id, string identityNumber)
        {
            var user = _serviceApplicationUser.SearchUser(id, identityNumber);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage GetUserByUserName(string username)
        {
            var user = _serviceApplicationUser.GetUserByUserName(username);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage ResetPassword(string username)
        {
            var result = _serviceApplicationUser.ResetPassword(username);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage ChangePassword(Guid id, string email, string oldPassword, string newPassword)
        {
            _serviceApplicationUser.ChangePassword(id, email, oldPassword, newPassword);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage ChangePasswordWeb(string email, string oldPassword, string newPassword)
        {
            _serviceApplicationUser.ChangePasswordWeb(email, oldPassword, newPassword);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage ResetPasswordFromToken([FromUri]ResetPasswordFromTokenDto entity)
        {
            _serviceApplicationUser.ResetPasswordFromToken(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage Post(Guid id, [FromBody] ApplicationUserDto entity)
        {
            entity.Id = id;
            _serviceApplicationUser.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUser([FromBody]ValidateUserDto entity)
        {
            if (_serviceApplicationUser.ValidateUser(entity.UserName, entity.Password))
                return Request.CreateResponse(HttpStatusCode.OK, true);

            return Request.CreateResponse(HttpStatusCode.OK, false);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUserWeb([FromBody]ValidateUserDto entity)
        {
            var response = _serviceApplicationUser.ValidateUserWeb(entity.UserName, entity.Password);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage ConfirmUser([FromBody]ConfirmUserDto entity)
        {
            if (_serviceApplicationUser.ConfirmUser(entity.UserName, entity.Token))
                return Request.CreateResponse(HttpStatusCode.OK, true);

            return Request.CreateResponse(HttpStatusCode.OK, false);
        }

        [HttpGet]
        public HttpResponseMessage ResetPasswordForUser(string username)
        {
            var token = _serviceApplicationUser.ResetPasswordForUser(username);
            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [HttpGet]
        public HttpResponseMessage InactivateUser(Guid id)
        {
            _serviceApplicationUser.InactivateUser(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage AddCard(Guid id, [FromBody] CardDto cardDto)
        {
            var card = _serviceApplicationUser.AddCard(cardDto, id);
            return Request.CreateResponse(HttpStatusCode.OK, card);
        }

        [HttpPost]
        public HttpResponseMessage AddCard([FromBody] IDictionary<string, string> cybersourceData)
        {
            var card = _serviceApplicationUser.AddCard(cybersourceData);
            return Request.CreateResponse(HttpStatusCode.OK, card);
        }

        [HttpGet]
        public HttpResponseMessage GetUserWithCards(Guid id)
        {
            var user = _serviceApplicationUser.GetById(id, s => s.Cards, s => s.MembershipIdentifierObj);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage GetNextCyberSourceIdentifier()
        {
            var cybersourceId = _serviceApplicationUser.GetNextCyberSourceIdentifier();
            return Request.CreateResponse(HttpStatusCode.OK, cybersourceId);
        }

    }
}