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
    public class DiscountController : ApiController
    {
        private readonly IServiceDiscount _serviceDiscount;

        public DiscountController(IServiceDiscount serviceDiscount)
        {
            _serviceDiscount = serviceDiscount;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DiscountList)]
        public HttpResponseMessage Get()
        {
            var discounts = _serviceDiscount.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, discounts);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DiscountDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var discount = _serviceDiscount.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, discount);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.DiscountEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] DiscountDto dto)
        {
            dto.Id = id;
            _serviceDiscount.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }
    }
}
