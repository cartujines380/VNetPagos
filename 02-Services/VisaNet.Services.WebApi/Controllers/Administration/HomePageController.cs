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
    public class HomePageController : ApiController
    {
        private readonly IServiceHomePage _serviceHomePage;

        public HomePageController(IServiceHomePage serviceHomePage)
        {
            _serviceHomePage = serviceHomePage;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.HomePageDetails)]
        public HttpResponseMessage Get()
        {
            var homePages = _serviceHomePage.GetDataForTable();

            return Request.CreateResponse(HttpStatusCode.OK, homePages);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.HomePageDetails)]
        public HttpResponseMessage Get(string id)
        {
            var homePage = _serviceHomePage.GetById(Guid.Parse(id), h => h.HomePageItem1, h => h.HomePageItem2, h => h.HomePageItem3);
            return Request.CreateResponse(HttpStatusCode.OK, homePage);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.HomePageEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] HomePageDto dto)
        {
            dto.Id = id;
            _serviceHomePage.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }
    }
}
