using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class SystemUserController : ApiController
    {
        private readonly IServiceSystemUser _serviceSystemUser;
        private readonly IServiceRole _serviceRole;

        public SystemUserController(IServiceSystemUser serviceSystemUser, IServiceRole serviceRole)
        {
            _serviceSystemUser = serviceSystemUser;
            _serviceRole = serviceRole;
        }


        [HttpGet]
        [WebApiAuthentication(Actions.SystemUsersList)]
        public HttpResponseMessage Get([FromUri] SystemUserFilterDto filterDto)
        {
            var users = _serviceSystemUser.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.SystemUsersList)]
        public HttpResponseMessage Get(Guid id)
        {
            var user = _serviceSystemUser.GetById(id, u => u.Roles);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage GetUserByUserName(string username)
        {
            var user = _serviceSystemUser.GetUserByUserName(username);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }
        
        [HttpPut]
        [WebApiAuthentication(Actions.SystemUsersCreate)]
        public HttpResponseMessage Put([FromBody] SystemUserDto entity)
        {
            _serviceSystemUser.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.SystemUsersEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] SystemUserDto entity)
        {
            entity.Id = id;
            _serviceSystemUser.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUser([FromBody]ValidateUserDto entity)
        {
            if (_serviceSystemUser.ValidateUser(entity.UserName, entity.Password))
                return Request.CreateResponse(HttpStatusCode.OK, true);

            return Request.CreateResponse(HttpStatusCode.OK, false);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUserInRole([FromBody]ValidateUserInRoleDto entity)
        {
            if (_serviceSystemUser.ValidateUserInRole(entity.UserName, (SystemUserType)entity.SystemUserTypeDto))
                return Request.CreateResponse(HttpStatusCode.OK, true);

            return Request.CreateResponse(HttpStatusCode.OK, false);
        }

        [HttpPost]
        public HttpResponseMessage GetPermissionsFromRoles(IEnumerable<Guid> rolesIds)
        {
            var functionalitiesGroups = _serviceRole.GetFunctionalityGroupsFromRoles(rolesIds);

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
                        MvcAction = a.MvcAction,
                        MvcController = a.MvcController,
                    }).ToList()

                }).OrderBy(f => f.Order).ToList()
            }).OrderBy(fg => fg.Order).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.SystemUsersDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceSystemUser.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

    }
}
