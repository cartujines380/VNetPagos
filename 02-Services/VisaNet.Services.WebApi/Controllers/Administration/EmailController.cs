using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Notifications;
using VisaNet.Common.Exceptions;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class EmailController : ApiController
    {
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly IServiceSystemUser _serviceSystemUser;

        public EmailController(IServiceEmailMessage serviceEmailMessage, IServiceSystemUser serviceSystemUser)
        {
            _serviceEmailMessage = serviceEmailMessage;
            _serviceSystemUser = serviceSystemUser;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsList)]
        public HttpResponseMessage ReportsEmailsData([FromUri] ReportsEmailsFilterDto filterDto)
        {
            var services = _serviceEmailMessage.GetEmailsForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsList)]
        public HttpResponseMessage ReportsEmailsDataCount([FromUri] ReportsEmailsFilterDto filterDto)
        {
            var services = _serviceEmailMessage.GetEmailsForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.ReportsMailsCancel)]
        public HttpResponseMessage CancelEmail(Guid id)
        {
            _serviceEmailMessage.CancelEmail(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var email = _serviceEmailMessage.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, email);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsResend)]
        public HttpResponseMessage ResendEmail(Guid id)
        {
            _serviceEmailMessage.ResendEmail(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsResendAll)]
        public HttpResponseMessage CheckStatus()
        {
            _serviceEmailMessage.CheckAllEmailStatus();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsResendAll)]
        public HttpResponseMessage ResendAll()
        {
            var count = _serviceEmailMessage.SendAllPendingEmails();
            return Request.CreateResponse(HttpStatusCode.OK,count);
        }
        
        [HttpGet]
        [WebApiAuthentication(Actions.ReportsMailsDetails)]
        public HttpResponseMessage DownloadAttachment(Guid id)
        {
            FileDto file;
            _serviceEmailMessage.GenerateAttachment(id, out file);
            return Request.CreateResponse(HttpStatusCode.OK, file);
        }
        
        [HttpPost]
        public HttpResponseMessage RegisterDelivery([FromBody]MailgunDeliveryDto model)
        {
            _serviceEmailMessage.RegisterDelivery(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        [HttpPost]
        public HttpResponseMessage RegisterFailure([FromBody]MailgunFailureDto model)
        {
            _serviceEmailMessage.RegisterFailure(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage RegisterBounce([FromBody]MailgunBounceDto model)
        {
            _serviceEmailMessage.RegisterBounce(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage SendCustomerSiteSystemUserCreationEmail(CustomerSiteSystemUserDto user)
        {
            _serviceEmailMessage.SendCustomerSiteSystemUserCreationEmail(user);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage SendNotificationManualSynchronization(DebitManualSyncNotificationDto dto)
        {
            try
            {
                var email = _serviceSystemUser.GetEmailByUserName(dto.UserName);
                if (dto.Type == NotificationType.Error)
                {
                    _serviceEmailMessage.SendFileManualSynchronizationNotificationError(email, dto.ExpectionError);
                }
                else
                {
                    _serviceEmailMessage.SendFileManualSynchronizationNotification(email, dto.Message);
                }
            }
            catch(BusinessException e)
            {
                NLogLogger.LogEvent(e);
            }
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage SendNotificationTc33Synchronization(Tc33SyncNotificationDto dto)
        {
            try
            {
                var email = _serviceSystemUser.GetEmailByUserName(dto.UserName);
                if (dto.Type == NotificationType.Error)
                {
                    _serviceEmailMessage.SendTc33SynchronizationNotificationError(email, dto.ExpectionError);
                }
                else
                {
                    _serviceEmailMessage.SendTc33SynchronizationNotification(email, dto.Message);
                }
            }
            catch (BusinessException e)
            {
                NLogLogger.LogEvent(e);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}