using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class RoleController : ApiController
    {
        private readonly IServiceRole _serviceRole;

        public RoleController(IServiceRole serviceRole)
        {
            _serviceRole = serviceRole;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.RolesList)]
        public HttpResponseMessage Get([FromUri] RoleFilterDto filterDto)
        {
            var dtos = _serviceRole.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.RolesList)]
        public HttpResponseMessage Get(Guid id)
        {
            var dto = _serviceRole.GetById(id, r => r.Actions);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.RolesCreate)]
        public HttpResponseMessage Put([FromBody] RoleDto entity)
        {
            _serviceRole.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.RolesEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] RoleDto entity)
        {
            entity.Id = id;
            _serviceRole.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.RolesDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceRole.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpGet]
        public HttpResponseMessage GetFunctionalityGroups()
        {
            var functionalitiesGroups = _serviceRole.GetFunctionalityGroupsNoTracking();

            var items = functionalitiesGroups.Select(fg => new FunctionalityGroup
            {
                Id = fg.Id,
                Name = fg.Name,
                Order = fg.Order,
                IconClass = fg.IconClass,
                Functionalities = fg.Functionalities.Select(f => new Functionality
                {
                    Id = f.Id,
                    IconClass = f.IconClass,
                    Name = f.Name,
                    Order = f.Order,
                    MemberOfFunctionalityId = f.MemberOfFunctionalityId,
                    FunctionalityGroupId = f.FunctionalityGroupId,
                    Actions = f.Actions.Select(a => new Action
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ActionRequiredId = a.ActionRequiredId,
                        ActionType = a.ActionType,
                        FunctionalityId = a.FunctionalityId,
                        IsDefaultAction = a.IsDefaultAction,
                    }).ToList()

                }).ToList()
            });

            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpGet]
        public HttpResponseMessage GetActions()
        {
            var actions = _serviceRole.GetActionNoTracking();

            var items = actions.Select(a => new Action
            {
                Id = a.Id,
                Name = a.Name,
                ActionRequiredId = a.ActionRequiredId,
                ActionType = a.ActionType,
                FunctionalityId = a.FunctionalityId,
                IsDefaultAction = a.IsDefaultAction,
                MvcAction = a.MvcAction,
                MvcController = a.MvcController,
            });

            return Request.CreateResponse(HttpStatusCode.OK, items);
        }
    }
}
