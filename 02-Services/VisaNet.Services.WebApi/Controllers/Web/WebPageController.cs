using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebPageController : ApiController
    {
        private readonly IServicePage _servicePage;

        public WebPageController(IServicePage servicePage)
        {
            _servicePage = servicePage;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var pages = _servicePage.GetDataForTable();
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType });
            return Request.CreateResponse(HttpStatusCode.OK, pages);
        }

        [HttpGet]
        public HttpResponseMessage Get(PageType type)
        {
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType }, s => s.PageType == type, s => s.Content);
            return Request.CreateResponse(HttpStatusCode.OK, pag.FirstOrDefault());
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var page = _servicePage.GetById(Guid.Parse(id));
            return Request.CreateResponse(HttpStatusCode.OK, page);
        }

        [HttpGet]
        public HttpResponseMessage Get(string nombre, int? from, int? length)
        {
            var pages = _servicePage.GetDataForTable();
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType });
            return Request.CreateResponse(HttpStatusCode.OK, pages);
        }

    }
}