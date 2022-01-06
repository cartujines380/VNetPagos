using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.VisaNetOn.Models;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IWebApplicationUserClientService _applicationUserClientService;

        public AccountController(IWebApplicationUserClientService applicationUserClientService)
        {
            _applicationUserClientService = applicationUserClientService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetMyPassword(LogInModel model)
        {
            try
            {
                await _applicationUserClientService.ResetPassword(model.LogInUserName);
                return Json(new JsonResponse(AjaxResponse.Success, null,
                    PresentationCoreMessages.Security_ForgetMyPassword_Result, "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException)
            {
                //Si no encuentra el email
                return Json(new JsonResponse(AjaxResponse.Success, null,
                    PresentationCoreMessages.Security_ForgetMyPassword_Result, "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "AccountController - ForgetMyPassword - Error en pagina de cambiar contraseña - Username: " + model.LogInUserName);
                NLogLogger.LogEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    "Error en pagina de cambiar contraseña.", PresentationCoreMessages.NotificationFail, NotificationType.Alert), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "AccountController - ForgetMyPassword - Error en pagina de cambiar contraseña - Username: " + model.LogInUserName);
                NLogLogger.LogEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    "Error en pagina de cambiar contraseña.", PresentationCoreMessages.NotificationFail, NotificationType.Alert), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ValidateUserCredentials(LogInModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _applicationUserClientService.ValidateUserWeb(new ValidateUserDto { UserName = model.LogInUserName, Password = model.LogInPassword });
                    if (response == ValidateUserResponse.Valid)
                    {
                        return Json(new JsonResponse(AjaxResponse.Success, null,
                            "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationCoreMessages.Security_UserNameOrPasswordNotValid, "", NotificationType.Alert), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationCoreMessages.Security_UserNameOrPasswordNotValid, "", NotificationType.Alert), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _applicationUserClientService.ValidateUserWeb(new ValidateUserDto { UserName = model.LogInUserName, Password = model.LogInPassword });
                    if (response == ValidateUserResponse.Valid)
                    {
                        var user = await _applicationUserClientService.Find(model.LogInUserName);
                        TempData["ApplicationUser"] = user;
                        return RedirectToAction("SuccessfulLogIn", "Home", new { webhookRegistrationId = model.WebhookRegistrationId });
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "AccountController - LogIn - Exception - Username: " + model.LogInUserName);
            }
            return RedirectToAction("End", "Home", new End { WebhookRegistrationId = model.WebhookRegistrationId });
        }

    }
}