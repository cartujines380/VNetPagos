using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class FaqController : ApiController
    {
        private readonly IServiceFaq _serviceFaq;

        public FaqController(IServiceFaq serviceFaq)
        {
            _serviceFaq = serviceFaq;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.FaqList)]
        public HttpResponseMessage Get([FromUri] FaqFilterDto filterDto)
        {
            var faqs = _serviceFaq.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, faqs);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.FaqDetails)]
        public HttpResponseMessage Get(string id)
        {
            var faq = _serviceFaq.GetById(Guid.Parse(id));
            return Request.CreateResponse(HttpStatusCode.OK, faq);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.FaqCreate)]
        public HttpResponseMessage Put([FromBody] FaqDto entity)
        {

            var dto = _serviceFaq.Create(new FaqDto { Order = entity.Order, Question = entity.Question, Answer = entity.Answer });


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.FaqEdit)]
        public HttpResponseMessage Post(string id, FaqDto dto)
        {
            try
            {
                dto.Id = new Guid(id);
                _serviceFaq.Edit(dto);
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.FaqDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                _serviceFaq.Delete(id);
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

    }
}
