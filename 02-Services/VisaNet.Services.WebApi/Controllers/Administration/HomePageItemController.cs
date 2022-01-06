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
    public class HomePageItemController : ApiController
    {
        private readonly IServiceHomePageItem _serviceHomePageItem;

        public HomePageItemController(IServiceHomePageItem serviceHomePageItem)
        {
            _serviceHomePageItem = serviceHomePageItem;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.HomePageDetails)]
        public HttpResponseMessage Get()
        {
            var homePageItems = _serviceHomePageItem.GetDataForTable();

            return Request.CreateResponse(HttpStatusCode.OK, homePageItems);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.HomePageDetails)]
        public HttpResponseMessage Get(string id)
        {
            var homePageItem = _serviceHomePageItem.GetById(Guid.Parse(id), h => h.Image, h => h.File);
            return Request.CreateResponse(HttpStatusCode.OK, homePageItem);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.HomePageEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] HomePageItemDto dto)
        {
            dto.Id = id;
            _serviceHomePageItem.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }
    }
}
