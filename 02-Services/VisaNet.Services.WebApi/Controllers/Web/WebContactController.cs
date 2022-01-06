using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebContactController : ApiController
    {
        private readonly IServiceContact _serviceContact;

        public WebContactController(IServiceContact serviceContact)
        {
            _serviceContact = serviceContact;
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] ContactDto entity)
        {

            var dto = _serviceContact.Create(new ContactDto { Name = entity.Name, Surname = entity.Surname, Email = entity.Email, PhoneNumber = entity.PhoneNumber, QueryType = entity.QueryType, Subject = entity.Subject, Message = entity.Message });


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var dictionary = EnumHelpers.ConvertToDictionary(typeof(QueryTypeDto), EnumsStrings.ResourceManager);
            return Request.CreateResponse(HttpStatusCode.OK, dictionary);
        }

    }
}