using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ApplicationUserController : ApiController
    {
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceCard _serviceCard;

        public ApplicationUserController(IServiceApplicationUser serviceApplicationUser, IServiceCard serviceCard)
        {
            _serviceApplicationUser = serviceApplicationUser;
            _serviceCard = serviceCard;
        }

        [HttpGet]
        //[WebApiAuthentication(Actions.SystemUsersList)]
        public HttpResponseMessage Get([FromUri] ApplicationUserFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        //[WebApiAuthentication(Actions.SystemUsersList)]
        public HttpResponseMessage Get(Guid id)
        {
            var user = _serviceApplicationUser.GetById(id, x => x.MembershipIdentifierObj);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage GetUserByUserName(string username)
        {

            var user = _serviceApplicationUser.GetUserByUserName(username);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [HttpGet]
        public HttpResponseMessage ResetPassword(string username)
        {
            var result = _serviceApplicationUser.ResetPassword(username);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage ChangePassword(Guid id, string password)
        {
            _serviceApplicationUser.ChangePasswordFromBO(id, password);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        public HttpResponseMessage ResetPasswordFromToken([FromUri]ResetPasswordFromTokenDto entity)
        {
            _serviceApplicationUser.ResetPasswordFromToken(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[HttpPut]
        //[WebApiAuthentication(Actions.SystemUsersCreate)]
        //public HttpResponseMessage Put([FromBody] SystemUserDto entity)
        //{
        //    _serviceApplicationUser.Create(entity);
        //    return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        //}

        [HttpPost]
        //[WebApiAuthentication(Actions.SystemUsersEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] ApplicationUserDto entity)
        {
            entity.Id = id;
            _serviceApplicationUser.EditFromBO(entity);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        public HttpResponseMessage ValidateUser([FromBody]ValidateUserDto entity)
        {
            if (_serviceApplicationUser.ValidateUser(entity.UserName, entity.Password))
                return Request.CreateResponse(HttpStatusCode.OK, true);

            return Request.CreateResponse(HttpStatusCode.OK, false);
        }


        //[HttpPost]
        //public HttpResponseMessage ValidateUserInRole([FromBody]ValidateUserInRoleDto entity)
        //{
        //    if (_serviceApplicationUser.ValidateUserInRole(entity.UserName, (SystemUserType)entity.SystemUserTypeDto))
        //        return Request.CreateResponse(HttpStatusCode.OK, true);

        //    return Request.CreateResponse(HttpStatusCode.OK, false);
        //}

        //[HttpPost]
        //public HttpResponseMessage GetPermissionsFromRoles(IEnumerable<Guid> rolesIds)
        //{
        //    var functionalitiesGroups = _serviceRole.GetFunctionalityGroupsFromRoles(rolesIds);

        //    var items = functionalitiesGroups.Select(fg => new FunctionalityGroup
        //    {
        //        Id = fg.Id,
        //        Name = fg.Name,
        //        Order = fg.Order,
        //        IconClass = fg.IconClass,
        //        Functionalities = fg.Functionalities.Select(f => new Functionality
        //        {
        //            Id = f.Id,
        //            IconClass = f.IconClass,
        //            Name = f.Name,
        //            Order = f.Order,
        //            MemberOfFunctionalityId = f.MemberOfFunctionalityId,
        //            FunctionalityGroupId = f.FunctionalityGroupId,
        //            Actions = f.Actions.Select(a => new Action
        //            {
        //                Id = a.Id,
        //                Name = a.Name,
        //                ActionRequiredId = a.ActionRequiredId,
        //                ActionType = a.ActionType,
        //                FunctionalityId = a.FunctionalityId,
        //                IsDefaultAction = a.IsDefaultAction,
        //                MvcAction = a.MvcAction,
        //                MvcController = a.MvcController,
        //            }).ToList()

        //        }).OrderBy(f => f.Order).ToList()
        //    }).OrderBy(fg => fg.Order).ToList();

        //    return Request.CreateResponse(HttpStatusCode.OK, items);
        //}

        [HttpDelete]
        //[WebApiAuthentication(Actions.SystemUsersDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceApplicationUser.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        [HttpPost]
        public HttpResponseMessage GetDashboardData([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDashboardData(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPost]
        public HttpResponseMessage GetDashboardDataCount([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDashboardDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPost]
        public HttpResponseMessage GetDataForReportsUser([FromBody] ReportsUserFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDataForReportsUser(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPost]
        public HttpResponseMessage GetDataForReportsUserCount([FromBody] ReportsUserFilterDto filterDto)
        {
            var users = _serviceApplicationUser.GetDataForReportsUserCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage ChangeBlockStatusUser([FromUri] Guid id)
        {
            var users = _serviceApplicationUser.ChangeBlockStatusUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage ReportsCardsData([FromUri] ReportsCardsFilterDto filters)
        {
            var data = _serviceCard.ReportsCardsData(filters);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage ReportsCardsDataCount([FromUri] ReportsCardsFilterDto filters)
        {
            var count = _serviceCard.ReportsCardsDataCount(filters);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

    }
}