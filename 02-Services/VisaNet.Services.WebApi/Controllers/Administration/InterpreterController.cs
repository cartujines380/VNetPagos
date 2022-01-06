using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class InterpreterController : ApiController
    {
        private readonly IServiceInterpreter _serviceInterpreter;

        public InterpreterController(IServiceInterpreter serviceInterpreter)
        {
            _serviceInterpreter = serviceInterpreter;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.InterpreterList)]
        public HttpResponseMessage Get([FromUri] InterpreterFilterDto filterDto)
        {
            var bins = _serviceInterpreter.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.InterpreterDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceInterpreter.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.InterpreterCreate)]
        public HttpResponseMessage Put([FromBody] InterpreterDto entity)
        {
            var dto = _serviceInterpreter.Create(entity,true);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.InterpreterEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] InterpreterDto dto)
        {
            dto.Id = id;
            _serviceInterpreter.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.InterpreterDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceInterpreter.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage GetDataForInterpreterCount([FromBody] InterpreterFilterDto filterDto)
        {
            var users = _serviceInterpreter.GetDataForInterpreterCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }
    }
}
