using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebLogController : ApiController
    {
        private readonly ILoggerService _loggerService;

        public WebLogController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var log = _loggerService.All(null, l => l.Id == id).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new LogDto
            {
                Id = log.Id,
                //UserName = !String.IsNullOrEmpty(log.SystemUserName) ? log.SystemUserName : log.ApplicationUserName,
                LogType = (LogType)((int)log.LogType),
                DateTime = log.DateTime,
                Message = log.Message
            });
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get([FromUri] LogFilterDto filterDto)
        {
            if (filterDto.To != default(DateTime))
            {
                filterDto.To = filterDto.To.AddDays(1);
            }

            var logs = _loggerService.All(null, l => (l.ApplicationUserId.HasValue && l.ApplicationUserId.Value == filterDto.SelectedUserId)
                                                  && (l.IncludeCallCenterMessage)
                //                                      && (filterDto.LogType == null || (int)l.LogType == (filterDto.LogType ))
                                                  && (filterDto.From == default(DateTime) || l.DateTime > filterDto.From)
                                                  && (filterDto.To == default(DateTime) || l.DateTime < filterDto.To));

            //var logs = _loggerService.All(null, l => (l.ApplicationUserId.HasValue && l.ApplicationUserId.Value == filterDto.SelectedUserId));
            return Request.CreateResponse(HttpStatusCode.OK, logs.OrderBy(l => l.DateTime).Select(l => new LogDto
            {
                Id = l.Id,
                LogType = (LogType)((int)l.LogType),
                DateTime = l.DateTime,
                CallCenterMessage = l.CallCenterMessage,
            }));
        }


        [System.Web.Http.HttpPost]
        public HttpResponseMessage GetForTable([FromBody] LogFilterDto filterDto)
        {
            if (filterDto.To != default(DateTime))
            {
                filterDto.To = filterDto.To.AddDays(1);
            }

            var logs = _loggerService.All(null, l => (l.ApplicationUserId.HasValue && l.ApplicationUserId.Value == filterDto.SelectedUserId)
                                                      && (l.IncludeCallCenterMessage)
                                                      && (filterDto.LogType == null || (int)l.LogType == (filterDto.LogType))
                                                  && (filterDto.From == default(DateTime) || l.DateTime >= filterDto.From)
                                                  && (filterDto.To == default(DateTime) || l.DateTime < filterDto.To));

            return Request.CreateResponse(HttpStatusCode.OK, logs.OrderByDescending(l => l.DateTime).Select(l => new LogDto
            {
                Id = l.Id,
                LogType = l.LogType,
                LogUserType = l.LogUserType,
                LogCommunicationType = l.LogCommunicationType,
                DateTime = l.DateTime,
                Message = l.Message,
                CallCenterMessage = l.CallCenterMessage,
            }));
        }


        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put([FromBody] LogModel entity)
        {
            if (entity.ApplicationUserId.HasValue)
            {
                _loggerService.CreateLog(entity.LogType, entity.LogOperationType, entity.LogCommunicationType,
                                    entity.ApplicationUserId.Value,
                                    entity.Message, entity.CallCenterMessage, null,
                                    entity.CyberSourceLogData, entity.CyberSourceVerifyByVisaData, entity.TemporaryId);
            }
            else
            {
                _loggerService.CreateLog(entity.LogType, entity.LogOperationType, entity.LogCommunicationType,
                                    entity.Message, entity.CallCenterMessage, null,
                                    entity.CyberSourceLogData, entity.CyberSourceVerifyByVisaData, entity.TemporaryId);
            }
            

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        [System.Web.Http.HttpPut]
        public HttpResponseMessage CreateWithTemporaryId([FromBody] LogModel entity)
        {
            _loggerService.CreateLogWithTemporaryId(entity.TemporaryId.Value, entity.LogType, entity.LogOperationType, entity.LogCommunicationType,
                                     entity.Message, entity.CallCenterMessage);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage AnonymousLog([FromBody] LogAnonymousModel entity)
        {
            if (string.IsNullOrEmpty(entity.CallCenterMessage))
                _loggerService.CreateLogForAnonymousUser(entity.LogType, entity.LogOperationType, entity.LogCommunicationType, entity.AnonymousUserId, entity.Message, null, entity.CyberSourceLogData, entity.CyberSourceVerifyByVisaData, entity.TemporaryId);
            else
                _loggerService.CreateLogForAnonymousUser(entity.LogType, entity.LogOperationType, entity.LogCommunicationType, entity.AnonymousUserId, entity.Message, entity.CallCenterMessage, null, entity.CyberSourceLogData, entity.CyberSourceVerifyByVisaData, entity.TemporaryId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
