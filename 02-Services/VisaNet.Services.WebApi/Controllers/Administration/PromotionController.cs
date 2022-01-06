using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class PromotionController : ApiController
    {
        private readonly IServicePromotion _servicePromotion;

        public PromotionController(IServicePromotion servicePromotion)
        {
            _servicePromotion = servicePromotion;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.PromotionList)]
        public HttpResponseMessage Get([FromUri] PromotionFilterDto filterDto)
        {
            var promotions = _servicePromotion.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, promotions);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.PromotionList)]
        public HttpResponseMessage Get(Guid id)
        {
            var prom = _servicePromotion.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, prom);
        }
        [HttpPut]
        [WebApiAuthentication(Actions.PromotionCreate)]
        public HttpResponseMessage Put([FromBody] PromotionDto entity)
        {

            var dto = _servicePromotion.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.PromotionEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] PromotionDto dto)
        {
            dto.Id = id;
            _servicePromotion.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.PromotionDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _servicePromotion.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

    }
}
