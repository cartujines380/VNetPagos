using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class PageController : ApiController
    {
        private readonly IServicePage _servicePage;

        public PageController(IServicePage servicePage)
        {
            _servicePage = servicePage;
        }


        [HttpGet]
        //[WebApiAuthentication(Actions.InstitutionalContentDetails)]
        public HttpResponseMessage Get()
        {
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType });
            return Request.CreateResponse(HttpStatusCode.OK, pag.ToList());
        }

        [HttpGet]
        //[WebApiAuthentication(Actions.InstitutionalContentDetails)]
        public HttpResponseMessage Get(PageTypeDto type)
        {
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType }, s => (int)s.PageType == (int)type, s => s.Content);
            return Request.CreateResponse(HttpStatusCode.OK, pag.FirstOrDefault());
        }

        [HttpGet]
        //[WebApiAuthentication(Actions.InstitutionalContentDetails)]
        public HttpResponseMessage Get(string id)
        {
            var page = _servicePage.GetById(Guid.Parse(id));
            return Request.CreateResponse(HttpStatusCode.OK, page);
        }

        [HttpGet]
        //[WebApiAuthentication(Actions.InstitutionalContentDetails)]
        public HttpResponseMessage Get(string nombre, int? from, int? length)
        {
            var pages = _servicePage.GetDataForTable();
            var pag = _servicePage.AllNoTracking(s => new PageDto { Id = s.Id, Content = s.Content, PageType = (PageTypeDto)(int)s.PageType });
            return Request.CreateResponse(HttpStatusCode.OK, pages);
        }

        [HttpPost]
        //[WebApiAuthentication(Actions.InstitutionalContentEdit)]
        public HttpResponseMessage Post(string id, PageDto dto)
        {
            try
            {
                dto.Id = new Guid(id);
                _servicePage.Edit(dto);
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
