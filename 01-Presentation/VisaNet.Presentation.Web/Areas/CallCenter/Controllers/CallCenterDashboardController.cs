using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.CallCenter.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Controllers
{
    [Authorize]
    public class CallCenterDashboardController : BaseController
    {
        private readonly IWebApplicationUserClientService _applicationUserClientService;
        private readonly IAutoCompleteClientService _autoCompleteClientService;

        public CallCenterDashboardController(IWebApplicationUserClientService applicationUserClientService,
                                             IAutoCompleteClientService autoCompleteClientService)
        {
            _applicationUserClientService = applicationUserClientService;
            _autoCompleteClientService = autoCompleteClientService;
        }

        //
        // GET: /CallCenter/CallCenterDashboard/
        public async Task<ActionResult> Index()
        {

            if (Session[SessionConstants.CURRENT_CALLCENTER_USER] == null)
            {
                return RedirectToAction("Index","CallCenterAccount");
            }

            //var currentSelectedUser = CurrentSelectedUser;
            //if (currentSelectedUser != null)
            //{
            //    return RedirectToAction("Index", "Dashboard");
            //}

            var applicationUsers = await _applicationUserClientService.FindAll();
            ViewBag.Id = new SelectList(applicationUsers, "Id", "Email");

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SearchUser(string id, string identityNumber)
        {
            try
            {
                if (!String.IsNullOrEmpty(id) || !String.IsNullOrEmpty(identityNumber))
                {
                    var userId = !String.IsNullOrEmpty(id) ? Guid.Parse(id) : default(Guid);
                    var applicationUser = await _applicationUserClientService.Find(userId, identityNumber);

                    if (applicationUser != null)
                    {
                        var content = RenderPartialViewToString("_ConfirmUser", new ApplicationUserModel
                        {
                            Id = applicationUser.Id,
                            Email = applicationUser.Email,
                            CallCenterKey = applicationUser.CallCenterKey,
                            Name = applicationUser.Name,
                            Surname = applicationUser.Surname,
                            IdentityNumber = applicationUser.IdentityNumber,
                            PhoneNumber = applicationUser.PhoneNumber,
                            MobileNumber = applicationUser.MobileNumber,
                            Address = applicationUser.Address
                        });

                        return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success));
                    }
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            return Json(new JsonResponse(AjaxResponse.Error, "", PresentationCallCenterStrings.Application_User_Not_Exist, PresentationCoreMessages.NotificationFail, NotificationType.Error));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUser(ApplicationUserModel model)
        {
            try
            {
                var applicationUser = await _applicationUserClientService.Find(model.Id, model.IdentityNumber);
                SetCurrentSelectedUser(applicationUser);

                return RedirectToAction("Index", "Dashboard", new RouteValueDictionary() { { "Area", "Private" } });
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult LogOffUser()
        {
            SetCurrentSelectedUser(null);

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> AutoCompleteUsers(string name_startsWith, int maxRows = 10)
        {
            var data = await _autoCompleteClientService.AutoCompleteApplicationUsers(name_startsWith);

            if (data.Any())
                return Json(data.Select(d => new { d.Id, d.Email }), JsonRequestBehavior.AllowGet);


            return Json(new List<dynamic> { new { Id = Guid.Empty, Name = "No se encontraron elementos" } }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckUser()
        {
            var user = CurrentUser();
            return user == null;
        }
    }
}