using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class AuditController : ApiController
    {
        private readonly IServiceAudit _serviceAudit;
        private readonly IServiceSystemUser _serviceSystemUser;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceChangeTracker _serviceChangeTracker;


        public AuditController(IServiceAudit serviceAudit,
                               IServiceSystemUser serviceSystemUser,
                               IServiceAnonymousUser serviceAnonymousUser,
                               IServiceApplicationUser serviceApplicationUser, IServiceChangeTracker serviceChangeTracker)
        {
            _serviceAudit = serviceAudit;
            _serviceSystemUser = serviceSystemUser;
            _serviceAnonymousUser = serviceAnonymousUser;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceChangeTracker = serviceChangeTracker;
        }

        [HttpPost]
        [WebApiAuthentication(Actions.AuditDetails)]
        public HttpResponseMessage GetDetails([FromBody] Guid id)
        {
            var auditLogDtos = _serviceAudit.GetDetails(id)
                                 .Select(a => new AuditLogDto
                                 {
                                     Id = a.Id,

                                     IP = a.IP,
                                     TransactionIdentifier = a.TransactionIdentifier,

                                     SystemUserId = a.SystemUserId,
                                     AnonymousUserId = a.AnonymousUserId,
                                     ApplicationUserId = a.ApplicationUserId,

                                     DateTime = a.DateTime,
                                     LogType = a.LogType,
                                     LogOperationType = a.LogOperationType,
                                     LogUserType = a.LogUserType,
                                     LogCommunicationType = a.LogCommunicationType,

                                     Message = a.Message,
                                     InnerException = a.InnerException,
                                     ExceptionMessage = a.ExceptionMessage
                                 })
                                 .ToList();


            var systemUsersIds = auditLogDtos.Where(s => s.SystemUserId != null).Select(s => s.SystemUserId.Value).Distinct().ToList();
            var applicationUsersIds = auditLogDtos.Where(s => s.ApplicationUserId != null).Select(s => s.ApplicationUserId.Value).Distinct().ToList();
            var anonymousUsersIds = auditLogDtos.Where(s => s.AnonymousUserId != null).Select(s => s.AnonymousUserId.Value).Distinct().ToList();

            var systemsUsers = new List<SystemUserDto>();
            if (systemUsersIds.Any())
            {
                systemsUsers = _serviceSystemUser.AllNoTracking(u => new SystemUserDto
                {
                    Id = u.Id,
                    LDAPUserName = u.LDAPUserName,
                    SystemUserType = (SystemUserTypeDto)u.SystemUserType,
                }, u => systemUsersIds.Contains(u.Id)).ToList();
            }

            var applicationUsers = new List<ApplicationUserDto>();
            if (applicationUsersIds.Any())
            {
                applicationUsers = _serviceApplicationUser.AllNoTracking(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                }, u => applicationUsersIds.Contains(u.Id)).ToList();
            }

            var anonymousUsers = new List<AnonymousUserDto>();
            if (anonymousUsersIds.Any())
            {
                anonymousUsers = _serviceAnonymousUser.AllNoTracking(u => new AnonymousUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                }, u => anonymousUsersIds.Contains(u.Id)).ToList();
            }

            Parallel.ForEach(auditLogDtos, a =>
            {
                if (a.SystemUserId.HasValue)
                    a.SystemUser = systemsUsers.FirstOrDefault(u => u.Id == a.SystemUserId.Value);

                if (a.ApplicationUserId.HasValue)
                    a.ApplicationUser = applicationUsers.FirstOrDefault(u => u.Id == a.ApplicationUserId.Value);

                if (a.AnonymousUserId.HasValue)
                    a.AnonymousUser = anonymousUsers.FirstOrDefault(u => u.Id == a.AnonymousUserId.Value);
            });

            return Request.CreateResponse(HttpStatusCode.OK, auditLogDtos);
        }


        [HttpPost]
        [WebApiAuthentication(Actions.AuditList)]
        public HttpResponseMessage ExcelExport([FromBody] AuditFilterDto filterDto)
        {
            var t = _serviceAudit.ExcelExport(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, t);
        } 
        
        [HttpPost]
        [WebApiAuthentication(Actions.AuditList)]
        public HttpResponseMessage GetDataForTable([FromBody] AuditFilterDto filterDto)
        {
            var t = _serviceAudit.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, t);







            //var auditLogDtos = _serviceAudit.GetDataForTable(filterDto)
            //                     .Select(a => new AuditLogDto
            //                     {
            //                         //Id = a.Id,

            //                         IP = a.IP,
            //                         TransactionIdentifier = a.TransactionIdentifier,

            //                         SystemUserId = a.SystemUserId,
            //                         AnonymousUserId = a.AnonymousUserId,
            //                         ApplicationUserId = a.ApplicationUserId,

            //                         DateTime = a.DATE,
            //                         //LogType = a.LogType,
            //                         //LogUserType = a.LogUserType,
            //                         //LogCommunicationType = a.LogCommunicationType,
            //                         //Message = a.Message,
            //                     })
            //                     .ToList();


            //var systemUsersIds = auditLogDtos.Where(s => s.SystemUserId != null).Select(s => s.SystemUserId.Value).Distinct().ToList();
            //var applicationUsersIds = auditLogDtos.Where(s => s.ApplicationUserId != null).Select(s => s.ApplicationUserId.Value).Distinct().ToList();
            //var anonymousUsersIds = auditLogDtos.Where(s => s.AnonymousUserId != null).Select(s => s.AnonymousUserId.Value).Distinct().ToList();

            //var systemsUsers = _serviceSystemUser.AllNoTracking(u => new SystemUserDto
            //{
            //    Id = u.Id,
            //    LDAPUserName = u.LDAPUserName,
            //    SystemUserType = (SystemUserTypeDto)u.SystemUserType,
            //}, u => systemUsersIds.Contains(u.Id)).ToList();

            //var applicationUsers = _serviceApplicationUser.AllNoTracking(u => new ApplicationUserDto
            //{
            //    Id = u.Id,
            //    Name = u.Name,
            //    Surname = u.Surname,
            //    Email = u.Email,
            //}, u => applicationUsersIds.Contains(u.Id)).ToList();


            //var anonymousUsers = _serviceAnonymousUser.AllNoTracking(u => new AnonymousUserDto
            //{
            //    Id = u.Id,
            //    Name = u.Name,
            //    Surname = u.Surname,
            //    Email = u.Email,
            //}, u => anonymousUsersIds.Contains(u.Id)).ToList();

            //Parallel.ForEach(auditLogDtos, a =>
            //{
            //    if (a.SystemUserId.HasValue)
            //        a.SystemUser = systemsUsers.FirstOrDefault(u => u.Id == a.SystemUserId.Value);

            //    if (a.ApplicationUserId.HasValue)
            //        a.ApplicationUser = applicationUsers.FirstOrDefault(u => u.Id == a.ApplicationUserId.Value);

            //    if (a.AnonymousUserId.HasValue)
            //        a.AnonymousUser = anonymousUsers.FirstOrDefault(u => u.Id == a.AnonymousUserId.Value);
            //});

            //auditLogDtos.ForEach(a =>
            //{
            //    if (a.SystemUserId.HasValue)
            //        a.SystemUser = systemsUsers.FirstOrDefault(u => u.Id == a.SystemUserId.Value);

            //    if (a.ApplicationUserId.HasValue)
            //        a.ApplicationUser = applicationUsers.FirstOrDefault(u => u.Id == a.ApplicationUserId.Value);

            //    if (a.AnonymousUserId.HasValue)
            //        a.AnonymousUser = anonymousUsers.FirstOrDefault(u => u.Id == a.AnonymousUserId.Value);
            //});

            //return Request.CreateResponse(HttpStatusCode.OK, auditLogDtos);
        }


        [HttpPost]
        [WebApiAuthentication(Actions.AuditList)]
        public HttpResponseMessage ChangeLogExcelExport([FromBody] ChangeTrackerFilterDto filterDto)
        {
            var t = _serviceChangeTracker.ChangeLogExcelExport(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, t);
        } 
    }
}
