using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Enums;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Controllers
{
    public class CallCenterAccountController : BaseController
    {
        private readonly IWebSystemUserClientService _systemUserClientService;

        public CallCenterAccountController(IWebSystemUserClientService systemUserClientService)
        {
            _systemUserClientService = systemUserClientService;
        }


        public ActionResult Index()
        {
            return RedirectToAction("LogIn");
        }

        public async Task<ActionResult> LogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return await LoadUser(User.Identity.Name, "");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(CallCenterLogOnModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var result = await _systemUserClientService.ValidateUserInRole(new ValidateUserInRoleDto
                    //    {
                    //        SystemUserTypeDto = SystemUserTypeDto.CallCenter,
                    //        UserName = model.UserName
                    //    });

                    //if (!result)
                    //{
                    //    ClearSessionVariables();
                    //    ShowNotification(PresentationCoreMessages.Security_CallCenterUserNameOrPasswordNotValid, NotificationType.Error);
                    //    return View();
                    //}

                    var result = await _systemUserClientService.ValidateUser(new ValidateUserDto
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                    });

                    if (result)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return await LoadUser(model.UserName, returnUrl);
                    }
                }

                ShowNotification(PresentationCoreMessages.Security_CallCenterUserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(PresentationCoreMessages.Security_CallCenterUserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(PresentationCoreMessages.Security_CallCenterUserNameOrPasswordNotValid, NotificationType.Error);
                //CustomLogger.LogEvent(LogType.Error,
                //                      string.Format("Type: {0} | Message: {1} | UserName: {2} | IP: {3}",
                //                                    exception.GetType(), exception.GetMessage, model.UserName,
                //                                    Request.GetIPAddress()));

                return View();
            }
        }

        public ActionResult LogOff()
        {
            ClearSessionVariables();
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn");
        }

        private async Task<ActionResult> LoadUser(string userName, string returnUrl)
        {
            var user = await _systemUserClientService.Find(userName);
            Session[SessionConstants.CURRENT_CALLCENTER_USER] = user;
            Session[SessionConstants.CURRENT_CALLCENTER_USER_ID] = user.Id;
            //Session[SessionConstants.CURRENT_USER_TYPE] = CurrentUserType.CallCenter;

            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "CallCenterDashboard", new RouteValueDictionary() { { "Area", "CallCenter" } });
        }

        public ActionResult NotAllowed()
        {
            ClearSessionVariables();
            FormsAuthentication.SignOut();
            return View();
        }

        public ActionResult Timeout()
        {
            Session[SessionConstants.PAYMENT_DATA] = null;
            //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER] = null;
            //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER_ID] = null;
            Session[SessionConstants.SERVICES_ASSOSIATION] = null;
            return View();
        }
    }
}