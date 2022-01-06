using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using HXCaptcha;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Controllers
{
    public class SubscriberController : BaseController
    {
        private readonly IWebSubscriberClientService _subscriberClientService;

        public SubscriberController(IWebSubscriberClientService subscriberClientService)
        {
            _subscriberClientService = subscriberClientService;
        }

        public ActionResult Index()
        {
            return View(new SubscriberModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SubscriberModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = Request["g-recaptcha-response"];
                    var secretKey = ConfigurationManager.AppSettings["RecaptchaSecretKey"];

                    var postUrl = string.Format(ConfigurationManager.AppSettings["RecaptchaUrl"], secretKey, response);

                    var client = new WebClient();
                    //var reply = client.DownloadString(postUrl);

                    //var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponseModel>(reply);

                    //if (!captchaResponse.Success)
                    //{
                    //    if (captchaResponse.ErrorCodes.Count > 0)
                    //    {
                    //        #region logeo error

                    //        var error = captchaResponse.ErrorCodes[0].ToLower();
                    //        switch (error)
                    //        {
                    //            case ("missing-input-secret"):
                    //                NLogLogger.LogEvent(NLogType.Info,
                    //                    "Newsletter - Error validacion captcha usuario: " + model.Email +
                    //                    " - The secret parameter is missing.");
                    //                break;
                    //            case ("invalid-input-secret"):
                    //                NLogLogger.LogEvent(NLogType.Info,
                    //                    "Newsletter - Error validacion captcha usuario: " + model.Email +
                    //                    " - The secret parameter is invalid or malformed.");
                    //                break;
                    //            case ("missing-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info,
                    //                    "Newsletter - Error validacion captcha usuario: " + model.Email +
                    //                    " - The response parameter is missing.");
                    //                break;
                    //            case ("invalid-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info,
                    //                    "Newsletter - Error validacion captcha usuario: " + model.Email +
                    //                    " - The response parameter is invalid or malformed.");
                    //                break;
                    //            default:
                    //                NLogLogger.LogEvent(NLogType.Info,
                    //                    "Newsletter - Error validacion captcha usuario: " + model.Email +
                    //                    " - Error occured. Please try again");
                    //                break;
                    //        }

                    //        #endregion
                    //    }

                    //    ShowNotification(PresentationCoreMessages.Common_Captcha_Error, NotificationType.Error);
                    //    return View("Index", model);
                    //}

                    await _subscriberClientService.Create(model.ToDto());
                    ShowNotification(PresentationCoreMessages.Newsletter_correct, NotificationType.Success);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en newsletter");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en newsletter");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en newsletter");
                NLogLogger.LogEvent(exception);
                ShowNotification("No pudimos procesar tu solicitud. Intentá más tarde.", NotificationType.Error);
            }


            return View("Index", model);
        }
    }
}