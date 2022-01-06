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

    public class ServiceCategoryController : ApiController
    {
        private readonly IServiceServiceCategory _serviceServiceCategory;

        public ServiceCategoryController(IServiceServiceCategory serviceServiceCategory)
        {
            _serviceServiceCategory = serviceServiceCategory;
        }

        [System.Web.Http.HttpGet]
        [WebApiAuthentication(Actions.ServiceCategoryList)]
        public HttpResponseMessage Get([FromUri] ServiceCategoryFilterDto filterDto)
        {
            var servicesCategories = _serviceServiceCategory.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, servicesCategories);
        }

        [System.Web.Http.HttpGet]
        [WebApiAuthentication(Actions.ServiceCategoryList)]
        public HttpResponseMessage Get(Guid id)
        {
            var cat = _serviceServiceCategory.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        [System.Web.Http.HttpPut]
        [WebApiAuthentication(Actions.ServiceCategoryCreate)]
        public HttpResponseMessage Put([FromBody] ServiceCategoryDto entity)
        {

            var dto = _serviceServiceCategory.Create(new ServiceCategoryDto { Name = entity.Name });
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [System.Web.Http.HttpPost]
        [WebApiAuthentication(Actions.ServiceCategoryEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] ServiceCategoryDto dto)
        {
            dto.Id = id;
            _serviceServiceCategory.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [System.Web.Http.HttpDelete]
        [WebApiAuthentication(Actions.ServiceCategoryDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceServiceCategory.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

    }
}
