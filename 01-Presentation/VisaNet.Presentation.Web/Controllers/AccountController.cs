using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Enums;
using VisaNet.Presentation.Web.Models;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IWebApplicationUserClientService _userService;
        private readonly IWebSystemUserClientService _userSystemService;

        public AccountController(IWebApplicationUserClientService userService, IWebSystemUserClientService userSystemService)
        {
            _userService = userService;
            _userSystemService = userSystemService;
        }

        public async Task<ActionResult> LogIn(string returnUrl)
        {
            var a = Session["TimeOut"];
            if (a != null && (bool)a)
            {
                //ShowNotification("Por favor inicie sesión nuevamente", NotificationType.Alert);
            }
            if (User.Identity.IsAuthenticated)
            {
                return await LoadUser(User.Identity.Name, returnUrl);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(LogOnModel model, string ReturnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _userService.ValidateUserWeb(new ValidateUserDto { UserName = model.UserName, Password = model.Password });
                    if (response == ValidateUserResponse.Valid)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return await LoadUser(model.UserName, ReturnUrl);
                    }
                    if (response == ValidateUserResponse.Valid_Change_Password)
                    {
                        return RedirectToAction("ChangePassword", "Account", new { email = model.UserName, returnUrl = ReturnUrl });
                    }

                    ClearSessionVariables();
                    ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                    return View();
                }

                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View(model);
            }
            catch (WebApiClientBusinessException)
            {
                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
        }

        public async Task<ActionResult> LogOff()
        {
            var isCallCenter = await _userSystemService.ValidateUserInRole(new ValidateUserInRoleDto
            {
                SystemUserTypeDto = SystemUserTypeDto.CallCenter,
                UserName = User.Identity.Name,
            });
            if (isCallCenter)
            {
                SetCurrentSelectedUser(null);
                ClearSessionVariablesFromCallCenter();
                return RedirectToAction("Index", "CallCenterDashboard", new RouteValueDictionary() { { "Area", "CallCenter" } });
            }
            FormsAuthentication.SignOut();
            ClearSessionVariables();
            return RedirectToAction("LogIn", "Account");
        }

        public ActionResult NotAllowed()
        {
            FormsAuthentication.SignOut();
            ClearSessionVariables();
            return View();
        }

        public ActionResult ChangePassword(string email, string returnUrl)
        {
            return View(new ChangePasswordWebModel
            {
                UserName = !string.IsNullOrWhiteSpace(email) ? email : "",
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordWebModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.ChangePasswordWeb(model.UserName, model.OldPassword, model.NewPassword);

                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return await LoadUser(model.UserName, model.ReturnUrl);
                }
                ShowNotification(PresentationCoreMessages.InvalidFields, NotificationType.Error);
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }

            model.NewPassword = string.Empty;
            model.OldPassword = string.Empty;
            model.ConfirmPassword = string.Empty;

            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public async Task<ActionResult> RegisterConfirmation(string email, string token)
        {
            try
            {
                if (await _userService.ConfirmUser(new ConfirmUserDto { UserName = email, Token = token }))
                    return View(true);

                return View(false);
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
                return View(false);
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
                return View(false);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult RegisterConfirmation(SetPasswordFromTokenModel model)
        //{
        //    try
        //    {
        //        //_userService.ResetUserAccountPassword(model.UserName, model.Password, model.Id);
        //        return RedirectToAction("SetPasswordFromTokenSuccess");
        //    }
        //    catch (WebApiClientBusinessException exception)
        //    {
        //        ShowNotification(exception.Message, NotificationType.Error);
        //        return View();
        //    }
        //    catch (WebApiClientFatalException exception)
        //    {
        //        ShowNotification(exception.Message, NotificationType.Error);
        //        return View();
        //    }
        //}

        public ActionResult ChangePasswordFromToken(string token)
        {
            return View(new SetPasswordFromTokenModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordFromToken(SetPasswordFromTokenModel model)
        {
            try
            {
                await
                    _userService.ResetPasswordFromToken(new ResetPasswordFromTokenDto
                    {
                        Password = model.Password,
                        UserName = model.Email,
                        Token = model.Token
                    });
                return RedirectToAction("SetPasswordFromTokenSuccess");
            }
            catch (WebApiClientBusinessException)
            {
                ShowNotification("Token inválido. Vuelva a solicitar un cambio de contraseña.", NotificationType.Error);
                return RedirectToAction("ForgetMyPassword");
            }
            catch (WebApiClientFatalException)
            {
                ShowNotification("Token inválido. Vuelva a solicitar un cambio de contraseña.", NotificationType.Error);
                return RedirectToAction("ForgetMyPassword");
            }
            catch (Exception)
            {
                ShowNotification("Token inválido. Vuelva a solicitar un cambio de contraseña.", NotificationType.Error);
                return RedirectToAction("ForgetMyPassword");
            }
        }

        public ActionResult ConfirmationFailure()
        {
            return View();
        }

        public ActionResult SetPasswordFromTokenSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgetMyPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetMyPassword(ForgetMyPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
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
                    //                NLogLogger.LogEvent(NLogType.Info, "ForgetMyPassword - Error validacion captcha usuario: " + model.Email + " - The secret parameter is missing.");
                    //                break;
                    //            case ("invalid-input-secret"):
                    //                NLogLogger.LogEvent(NLogType.Info, "ForgetMyPassword - Error validacion captcha usuario: " + model.Email + " - The secret parameter is invalid or malformed.");
                    //                break;
                    //            case ("missing-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info, "ForgetMyPassword - Error validacion captcha usuario: " + model.Email + " - The response parameter is missing.");
                    //                break;
                    //            case ("invalid-input-response"):
                    //                NLogLogger.LogEvent(NLogType.Info, "ForgetMyPassword - Error validacion captcha usuario: " + model.Email + " - The response parameter is invalid or malformed.");
                    //                break;
                    //            default:
                    //                NLogLogger.LogEvent(NLogType.Info, "ForgetMyPassword - Error validacion captcha usuario: " + model.Email + " - Error occured. Please try again");
                    //                break;
                    //        }
                    //        #endregion
                    //    }

                    //    ShowNotification(PresentationCoreMessages.Common_Captcha_Error, NotificationType.Error);
                    //    return View();
                    //}

                    var result = await _userService.ResetPassword(model.Email);
                    if (result == 1)
                    {
                        ShowNotification(PresentationCoreMessages.Security_ForgetMyPassword_Result, NotificationType.Info);
                        return View();
                        //return RedirectToAction("ForgetMyPasswordSuccess");
                    }
                    if (result == 2) //actualmente siempre devuelve 1
                    {
                        ShowNotification(PresentationCoreMessages.Security_ForgetMyPassword_Result, NotificationType.Info);
                        return View();
                        //return RedirectToAction("ActivateUser");
                    }
                }
                catch (WebApiClientBusinessException exception)
                {
                    ShowNotification(PresentationCoreMessages.Security_ForgetMyPassword_Result, NotificationType.Info);
                    NLogLogger.LogEvent(NLogType.Info, "Error en pagina de cambiar contraseña");
                    NLogLogger.LogEvent(exception);
                    return View();
                }
                catch (WebApiClientFatalException exception)
                {
                    ShowNotification(PresentationCoreMessages.NotificationFail, NotificationType.Error);
                    NLogLogger.LogEvent(NLogType.Info, "Error en pagina de cambiar contraseña");
                    NLogLogger.LogEvent(exception);
                    return View();
                }
                catch (Exception exception)
                {
                    ShowNotification(PresentationCoreMessages.NotificationFail, NotificationType.Error);
                    NLogLogger.LogEvent(NLogType.Info, "Error en pagina de cambiar contraseña");
                    NLogLogger.LogEvent(exception);
                    return View();
                }
            }
            ShowNotification(PresentationCoreMessages.NotificationFail, NotificationType.Error);

            return View();
        }

        [HttpGet]
        public ActionResult ForgetMyPasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ActivateUser()
        {
            return View();
        }

        private async Task<ActionResult> LoadUser(string userName, string returnUrl)
        {
            var user = await _userService.Find(userName);
            SetCurrentSelectedUser(user);

            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard", new RouteValueDictionary() { { "Area", "Private" } });
        }

    }
}