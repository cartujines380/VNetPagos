using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Logging.NLog;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.WebApi.ModelBinders;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.Controllers
{
    public class MailgunBouncesController : MailgunController
    {
        private readonly ApiMethodEnum _apiMethodEnum = ApiMethodEnum.MailgunBounce;
        private readonly IMailgunWebhookService _mailgunWebhookService;

        public MailgunBouncesController(IMailgunWebhookService mailgunWebhookService)
        {
            _mailgunWebhookService = mailgunWebhookService;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([ModelBinder(typeof(MailgunBounceBinder))]MailgunBounce model)
        {
            //Se devuelve codigo 200 (Ok) o 406 (Not Acceptable) para que Mailgun no reintente el envío
            try
            {
                if (model == null)
                {
                    NLogLogger.LogEvent(NLogType.Error, "MailgunBouncesController - Post - Falló el ModelBinder: model = null");
                    return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                }

                LogEmailNotificationReception(model, _apiMethodEnum);

                if (!ValidateCredentials(model))
                {
                    LogEmailNotificationForbbiden(model);
                    return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                }

                await _mailgunWebhookService.RegisterBounce(model.MessageId, model.Recipient);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Error, e.Message);
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }
        }

    }
}