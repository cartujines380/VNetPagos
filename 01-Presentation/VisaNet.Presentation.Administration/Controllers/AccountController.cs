using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ISystemUserClientService _systemUserClientService;

        public AccountController(ISystemUserClientService systemUserClientService)
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
        public async Task<ActionResult> LogIn(LogOnModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _systemUserClientService.ValidateUserInRole(new ValidateUserInRoleDto
                        {
                            SystemUserTypeDto = SystemUserTypeDto.Administration,
                            UserName = model.UserName
                        });

                    if (!result)
                    {
                        ClearSessionVariables();
                        ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                        return View();
                    }

                    result = await _systemUserClientService.ValidateUser(new ValidateUserDto
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

                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View();
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
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
            var functionalitiesGroups = (await
                _systemUserClientService.GetPermissionsFromRoles(user.Roles.Select(r => r.Id).ToList())).ToList();

            var functionalities = functionalitiesGroups.SelectMany(fg => fg.Functionalities).Distinct().ToList();

            var actions = functionalities.SelectMany(u => u.Actions).Distinct().ToList();

            Session[SessionConstants.CURRENT_USER] = user;
            Session[SessionConstants.CURRENT_SYS_USER_ID] = user.Id;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITY_GROUP] = functionalitiesGroups;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES] = functionalities;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL] = null;
            Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS] = actions;
            
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }

            if (!functionalitiesGroups.Any())
                return RedirectToAction("NotAllowed");

            return FunctionalityGroup(functionalitiesGroups.First().Id);
        }

        [Authorize]
        public ActionResult FunctionalityGroup(int id)
        {
            try
            {
                Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL] = null;

                var totalFunctionalities = (IList<Functionality>)Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES];
                var filteredFunctionalities = totalFunctionalities.Where(f => f.FunctionalityGroupId == id).ToList();

                Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL] = filteredFunctionalities;

                var defaultFunctionality = filteredFunctionalities.FirstOrDefault();
                var totalActions = (IList<Action>)Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS];

                var defaultAction =
                    totalActions.FirstOrDefault(a => a.FunctionalityId == defaultFunctionality.Id && a.IsDefaultAction);

                Session[SessionConstants.FUNCTIONALITY_GROUP_SELECTED] = id;
                Session[SessionConstants.FUNCTIONALITY_SELECTED] = defaultFunctionality.Id;

                return RedirectToAction(defaultAction.MvcAction, defaultAction.MvcController);
            }
            catch (Exception)
            {
                return RedirectToAction("LogIn");
            }
        }

        [Authorize]
        public ActionResult Functionality(int id)
        {
            try
            {
                var selectedFunctionalities = (IList<Functionality>)Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL];

                var selectedFunctionality = selectedFunctionalities.FirstOrDefault(f => f.Id == id);

                var totalActions = (IList<Action>)Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS];

                var defaultAction = totalActions.First(a => a.FunctionalityId == selectedFunctionality.Id && a.IsDefaultAction);

                Session[SessionConstants.FUNCTIONALITY_SELECTED] = selectedFunctionality.Id;
                return RedirectToAction(defaultAction.MvcAction, defaultAction.MvcController);
            }
            catch (Exception)
            {
                return RedirectToAction("LogIn");
            }
            
        }

        public ActionResult NotAllowed()
        {
            ClearSessionVariables();
            FormsAuthentication.SignOut();
            return View();
        }
    }
}