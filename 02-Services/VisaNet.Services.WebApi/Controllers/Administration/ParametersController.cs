using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ParametersController : ApiController
    {
        private readonly IServiceParameters _serviceParameters;

        public ParametersController(IServiceParameters serviceParameters)
        {
            _serviceParameters = serviceParameters;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ParametersDetails)]
        public HttpResponseMessage Get()
        {
            var parameters = _serviceParameters.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, parameters);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ParametersDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var parameters = _serviceParameters.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, parameters);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ParametersEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] ParametersDto dto)
        {
            dto.Id = id;
            _serviceParameters.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }
    }
}
