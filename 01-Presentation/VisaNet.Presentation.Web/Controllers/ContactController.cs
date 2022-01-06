using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IWebContactClientService _contactClientService;

        public ContactController(IWebContactClientService contactClientService)
        {
            _contactClientService = contactClientService;
        }

        public ActionResult Index()
        {
            return View(new ContactModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContactModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = Request["g-recaptcha-response"];
                    var secretKey = ConfigurationManager.AppSettings["RecaptchaSecretKey"];

                    var postUrl = string.Format(ConfigurationManager.AppSettings["RecaptchaUrl"], secretKey, response);

                    var client = new WebClient();
                    var reply = client.DownloadString(postUrl);

                    var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponseModel>(reply);

                    if (!captchaResponse.Success)
                    {
                        if (captchaResponse.ErrorCodes.Count > 0)
                        {
                            #region logeo error
                            var error = captchaResponse.ErrorCodes[0].ToLower();
                            switch (error)
                            {
                                case ("missing-input-secret"):
                                    NLogLogger.LogEvent(NLogType.Info, "Contact Create - Error validacion captcha correo: " + model.Email + " - The secret parameter is missing.");
                                    break;
                                case ("invalid-input-secret"):
                                    NLogLogger.LogEvent(NLogType.Info, "Contact Create - Error validacion captcha correo: " + model.Email + " - The secret parameter is invalid or malformed.");
                                    break;
                                case ("missing-input-response"):
                                    NLogLogger.LogEvent(NLogType.Info, "Contact Create - Error validacion captcha correo: " + model.Email + " - The response parameter is missing.");
                                    break;
                                case ("invalid-input-response"):
                                    NLogLogger.LogEvent(NLogType.Info, "Contact Create - Error validacion captcha correo: " + model.Email + " - The response parameter is invalid or malformed.");
                                    break;
                                default:
                                    NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha correo: " + model.Email + " - Error occured. Please try again");
                                    break;
                            }
                            #endregion
                        }

                        ShowNotification(PresentationCoreMessages.Common_Captcha_Error, NotificationType.Error);
                        return View("Index", model);
                    }

                    await _contactClientService.Create(model.ToDto());
                    ShowNotification(PresentationCoreMessages.Contact_done, NotificationType.Success);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogEvent(exception);
                ShowNotification(exception.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogEvent(exception);
                ShowNotification(exception.Message, NotificationType.Error);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en contacto");
                NLogLogger.LogEvent(exception);
                ShowNotification("No pudimos procesar tu solicitud. Intentá más tarde.", NotificationType.Error);
            }
            return View("Index", model);
        }
    }
}