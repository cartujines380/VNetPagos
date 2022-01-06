using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Services;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class LogController : ApiController
    {
        private readonly ILoggerService _loggerService;

        public LogController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(string transactionId)
        {
            var log = _loggerService.All(l =>new LogDto
            {
                LogPaymentCyberSource = new LogPaymentCyberSourceDto
                {
                    TransactionDateTime = l.LogPaymentCyberSource.TransactionDateTime,
                    CyberSourceLogData = new CyberSourceLogDataDto
                    {
                        TransactionId = l.LogPaymentCyberSource.CyberSourceLogData.TransactionId,
                        ReqCurrency = l.LogPaymentCyberSource.CyberSourceLogData.ReqCurrency,
                        AuthAmount = l.LogPaymentCyberSource.CyberSourceLogData.AuthAmount,
                        ReqAmount = l.LogPaymentCyberSource.CyberSourceLogData.ReqAmount
                    }
                }
            }, l => l.LogPaymentCyberSource.CyberSourceLogData.TransactionId.Equals(transactionId), l => l.LogPaymentCyberSource).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, log);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put([FromBody] LogModel entity)
        {
            _loggerService.CreateLog(entity.LogType, entity.LogOperationType, entity.LogCommunicationType,
                                     entity.Message, entity.CallCenterMessage, null,
                                     entity.CyberSourceLogData, entity.CyberSourceVerifyByVisaData, entity.TemporaryId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage CreateWithTemporaryId([FromBody] LogModel entity)
        {
            _loggerService.CreateLogWithTemporaryId(entity.TemporaryId.Value, entity.LogType, entity.LogOperationType, entity.LogCommunicationType,
                                     entity.Message, entity.CallCenterMessage);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
