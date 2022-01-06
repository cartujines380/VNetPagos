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
    public class RegistrationController : BaseController
    {
        private readonly IWebRegisterUserClientService _registerUserClientService;
        private readonly IWebAnonymousUserClientService _anonymousUserClientService;

        public RegistrationController(IWebRegisterUserClientService registerUserClientService, IWebAnonymousUserClientService anonymousUserClientService)
        {
            _registerUserClientService = registerUserClientService;
            _anonymousUserClientService = anonymousUserClientService;
        }

        //
        // GET: /Public/Registration/
        public ActionResult Index()
        {
            return View(new RegistrationModel());
        }

        public async Task<ActionResult> RegisterAnonymousUser(Guid anonymousUserId)
        {
            var user = await _anonymousUserClientService.Find(anonymousUserId);

            return View("Index", new RegistrationModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Address = user.Address
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegistrationModel model)
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
                    //                NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha usuario: " + model.Email + " - The secret parameter is missing.");
                    //                break;
                    //            case ("invalid-input-secret"):
                    //                NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha usuario: " + model.Email + " - The secret parameter is invalid or malformed.");
                    //                break;
                    //            case ("missing-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha usuario: " + model.Email + " - The response parameter is missing.");
                    //                break;
                    //            case ("invalid-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha usuario: " + model.Email + " - The response parameter is invalid or malformed.");
                    //                break;
                    //            default:
                    //                NLogLogger.LogEvent(NLogType.Info, "Registration Index - Error validacion captcha usuario: " + model.Email + " - Error occured. Please try again");
                    //                break;
                    //        }
                    //        #endregion
                    //    }

                    //    ShowNotification(PresentationCoreMessages.Common_Captcha_Error, NotificationType.Error);
                    //    return View(model);
                    //}

                    if (!model.AcceptTermsAndConditions)
                    {
                        ShowNotification(PresentationWebStrings.Registration_Conditions_Validation, NotificationType.Error);
                        return View(model);
                    }
                    await _registerUserClientService.Create(model.ToDto());
                    ShowNotification(PresentationWebStrings.Register_Success, NotificationType.Success);
                    TempData["FromRegistration"] = true;
                    return RedirectToAction("LogIn", "Account");
                }

                ShowNotification(PresentationCoreMessages.NotificationFail, NotificationType.Error);
                return View(model);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }

            return View(model);
        }

        public ActionResult Conditions()
        {
            return RedirectToAction("Index", "LegalPages");
        }
    }
}