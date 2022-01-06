using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class SystemVersionsController : ApiController
    {
        private readonly IServiceSystemVersions _serviceSystemVersions;

        public SystemVersionsController(IServiceSystemVersions serviceSystemVersions)
        {
            _serviceSystemVersions = serviceSystemVersions;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsVersionDetails)]
        public HttpResponseMessage GetSystemVersions()
        {
            var versions = _serviceSystemVersions.GetSystemVersions();
            return Request.CreateResponse(HttpStatusCode.OK, versions);
        }

    }
}