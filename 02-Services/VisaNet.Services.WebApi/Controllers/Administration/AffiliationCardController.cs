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
    public class AffiliationCardController : ApiController
    {
        private readonly IServiceAffiliationCard _serviceAffiliationCard;

        public AffiliationCardController(IServiceAffiliationCard serviceAffiliationCard)
        {
            _serviceAffiliationCard = serviceAffiliationCard;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.AffiliationCardList)]
        public HttpResponseMessage Get([FromUri] AffiliationCardFilterDto filterDto)
        {
            var bins = _serviceAffiliationCard.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.AffiliationCardDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceAffiliationCard.GetById(id, x => x.Bank);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.AffiliationCardCreate)]
        public HttpResponseMessage Put([FromBody] AffiliationCardDto entity)
        {
            var dto = _serviceAffiliationCard.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.AffiliationCardEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] AffiliationCardDto dto)
        {
            dto.Id = id;
            _serviceAffiliationCard.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.AffiliationCardDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceAffiliationCard.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        [WebApiAuthentication(Actions.AffiliationCardList)]
        public HttpResponseMessage GetDataForTableCount([FromUri] AffiliationCardFilterDto filterDto)
        {
            var users = _serviceAffiliationCard.GetDataForAffiliationCardCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.AffiliationCardEdit)]
        public HttpResponseMessage ChangeStatus(Guid id)
        {
            _serviceAffiliationCard.ChangeStatus(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
