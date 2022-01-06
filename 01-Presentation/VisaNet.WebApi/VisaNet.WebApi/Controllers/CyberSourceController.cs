using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Logging.NLog;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.WebApi.ModelBinders;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.Controllers
{
    public class CyberSourceController : ApiController
    {
        private readonly ICyberSourceAcknowledgementClientService _cyberSourceAcknowledgementClientService;
        private readonly ApiMethodEnum _apiMethodEnum = ApiMethodEnum.CyberSourceAcknowledgement;

        public CyberSourceController(ICyberSourceAcknowledgementClientService cyberSourceAcknowledgementClientService)
        {
            _cyberSourceAcknowledgementClientService = cyberSourceAcknowledgementClientService;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([ModelBinder(typeof(CyberSourceBinder))]CyberSourcePostModel model)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Metodo: {0}, Llego post desde Cs. TransactionId: {1}", _apiMethodEnum.ToString(), model.TransactionId));
            try
            {
                await _cyberSourceAcknowledgementClientService.Process(model.ToDto());
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("Metodo: {0}, Exception. TransactionId: {1}", _apiMethodEnum.ToString(), model.TransactionId));
                NLogLogger.LogEvent(exception);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}