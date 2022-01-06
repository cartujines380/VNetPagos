using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class SystemUserController : BaseController
    {
        private readonly ISystemUserClientService _systemUserClientService;
        private readonly IRoleClientService _roleClientService;


        public SystemUserController(ISystemUserClientService systemUserClientService, IRoleClientService roleClientService)
        {
            _systemUserClientService = systemUserClientService;
            _roleClientService = roleClientService;
        }
        
        private async Task LoadViewBags()
        {
            var roles = await _roleClientService.FindAll();

            ViewBag.Roles = roles.Select(r => r.ToModel());
        }
        
        [HttpGet]
        [CustomAuthentication(Actions.SystemUsersList)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.SystemUsersCreate)]
        public async Task<ActionResult> Create()
        {
            await LoadViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.SystemUsersCreate)]
        public async Task<ActionResult> Create(SystemUserModel entity, FormCollection formCollection)
        {
            try
            {
                #region Se mapean los roles seleccionados
                entity.Roles = new List<RoleModel>();
                foreach (var controlKey in formCollection.AllKeys)
                {
                    if (controlKey.Contains("chkSelectedRole_"))
                    {
                        if (formCollection[controlKey].Equals("on"))
                        {
                            var roleId = new Guid(controlKey.Replace("chkSelectedRole_", string.Empty));
                            entity.Roles.Add(new RoleModel { Id = roleId });
                        }
                    }
                }
                #endregion

                if (ModelState.IsValid)
                {
                    await _systemUserClientService.Create(entity.ToDto());
                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);

                    return RedirectToAction("Index");
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Alert);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }

            await LoadViewBags();
            return View(entity);
        }
        
        [HttpGet]
        [CustomAuthentication(Actions.SystemUsersEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                await LoadViewBags();
                var user = await _systemUserClientService.Find(id);

                return View(user.ToModel());
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Alert);
                return RedirectToAction("Index");
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.SystemUsersEdit)]
        public async Task<ActionResult> Edit(SystemUserModel entity, FormCollection formCollection)
        {
            try
            {
                #region Se mapean los roles seleccionados
                entity.Roles = new List<RoleModel>();
                foreach (var controlKey in formCollection.AllKeys)
                {
                    if (controlKey.Contains("chkSelectedRole_"))
                    {
                        if (formCollection[controlKey].Equals("on"))
                        {
                            var roleId = new Guid(controlKey.Replace("chkSelectedRole_", string.Empty));
                            entity.Roles.Add(new RoleModel { Id = roleId });
                        }
                    }
                }
                #endregion

                if (ModelState.IsValid)
                {
                    await _systemUserClientService.Edit(entity.ToDto());
                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);

                    return RedirectToAction("Index");
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Alert);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }

            await LoadViewBags();
            return View(entity);
        }

        [HttpPost]
        [CustomAuthentication(Actions.SystemUsersDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _systemUserClientService.Delete(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_DeleteSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpGet]
        [CustomAuthentication(Actions.SystemUsersList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerSystemUser(Request, param);

            var data = await _systemUserClientService.FindAll(filter);

            var dataToShow = data.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            var resourceManager = EnumsStrings.ResourceManager;
            var enumType = typeof(SystemUserTypeDto);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(u => new
                {
                    u.Id,
                    UserName = u.LDAPUserName,
                    SystemUserType = EnumHelpers.GetName(enumType, (int)u.SystemUserType, resourceManager),
                    Roles = string.Join(" / ", u.Roles.Select(r => r.Name)),
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthentication(Actions.SystemUsersList)]
        public async Task<ActionResult> AjaxRolesPartial(Guid id)
        {
            SystemUserModel user;
            if (id == Guid.Empty)
                user = new SystemUserModel();
            else
            {
                var userDto = await _systemUserClientService.Find(id);
                user = userDto.ToModel();
            }

            var roles = await _roleClientService.FindAll();
            ViewBag.Roles = roles.Select(r => r.ToModel());

            if (user == null)
            {
                return
                    Json(new JsonResponse(AjaxResponse.Success, RenderPartialViewToString("_RoleSelection"),
                        string.Empty, string.Empty, NotificationType.Success));
            }

            return
                Json(new JsonResponse(AjaxResponse.Success, RenderPartialViewToString("_RoleSelection", user),
                    string.Empty, string.Empty, NotificationType.Success));

        }
    }
}