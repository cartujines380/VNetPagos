using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleClientService _roleClientService;

        public RoleController(IRoleClientService roleClientService)
        {
            _roleClientService = roleClientService;
        }

        //
        // GET: /Role/
        [HttpGet]
        [CustomAuthentication(Actions.RolesList)]
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.RolesCreate)]
        public async Task<ActionResult> Create()
        {
            ViewBag.FunctionalityGroups = await _roleClientService.GetFunctionalityGroups();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.RolesCreate)]
        public async Task<ActionResult> Create(RoleModel model, FormCollection formCollection)
        {
            var actions = (await _roleClientService.GetActions()).ToList();

            #region Validation of Required Actions
            foreach (var formControl in formCollection.AllKeys)
            {
                if (formControl.Contains("chkAction_"))
                {
                    if (formCollection[formControl].Equals("on"))
                    {
                        var id = int.Parse(formControl.Replace("chkAction_", ""));
                        model.ActionsIds.Add(id);
                    }
                }
            }

            foreach (var id in model.ActionsIds)
            {
                var actionAux = actions.First(a => a.Id == id);
                if (actionAux.ActionRequiredId != null)
                {
                    if (model.ActionsIds.All(actionId => actionId != actionAux.ActionRequiredId))
                    {
                        ModelState.AddModelError(string.Empty,
                                                 string.Format(PresentationAdminStrings.Roles_ActionRequired,
                                                               actions.First(a => a.Id == actionAux.ActionRequiredId).
                                                                   Name, actionAux.Name));

                        ShowNotification(string.Format(PresentationAdminStrings.Roles_ActionRequired,
                                                               actions.First(a => a.Id == actionAux.ActionRequiredId).
                                                                   Name, actionAux.Name), NotificationType.Alert);
                    }
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    await _roleClientService.Create(model.ToDto());
                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                    return RedirectToAction("Index");
                }
                catch (WebApiClientBusinessException exception)
                {
                    ShowNotification(exception.Message, NotificationType.Error);
                }
                catch (WebApiClientFatalException exception)
                {
                    ShowNotification(exception.Message, NotificationType.Error);
                }
            }

            ViewBag.FunctionalityGroups = await _roleClientService.GetFunctionalityGroups();
            return View(model);
        }


        [HttpGet]
        [CustomAuthentication(Actions.RolesEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            ViewBag.FunctionalityGroups = await _roleClientService.GetFunctionalityGroups();

            var roleDto = await _roleClientService.Find(id);

            return View(roleDto.ToModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.RolesEdit)]
        public async Task<ActionResult> Edit(RoleModel model, FormCollection formCollection)
        {
            var actions = (await _roleClientService.GetActions()).ToList();

            #region Validation of Required Actions
            foreach (var formControl in formCollection.AllKeys)
            {
                if (formControl.Contains("chkAction_"))
                {
                    if (formCollection[formControl].Equals("on"))
                    {
                        var id = int.Parse(formControl.Replace("chkAction_", ""));
                        model.ActionsIds.Add(id);
                    }
                }
            }

            foreach (var id in model.ActionsIds)
            {
                var actionAux = actions.First(a => a.Id == id);
                if (actionAux.ActionRequiredId != null)
                {
                    if (model.ActionsIds.All(actionId => actionId != actionAux.ActionRequiredId))
                    {
                        ModelState.AddModelError(string.Empty,
                                                 string.Format(PresentationAdminStrings.Roles_ActionRequired,
                                                               actions.First(a => a.Id == actionAux.ActionRequiredId).
                                                                   Name, actionAux.Name));

                        ShowNotification(string.Format(PresentationAdminStrings.Roles_ActionRequired,
                                                               actions.First(a => a.Id == actionAux.ActionRequiredId).
                                                                   Name, actionAux.Name), NotificationType.Alert);
                    }
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    await _roleClientService.Edit(model.ToDto());
                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                    return RedirectToAction("Index");
                }
                catch (WebApiClientBusinessException exception)
                {
                    ShowNotification(exception.Message, NotificationType.Error);
                }
                catch (WebApiClientFatalException exception)
                {
                    ShowNotification(exception.Message, NotificationType.Error);
                }
            }

            ViewBag.FunctionalityGroups = await _roleClientService.GetFunctionalityGroups();
            return View(model);
        }

        [HttpPost]
        [CustomAuthentication(Actions.RolesDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _roleClientService.Delete(id);
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
        [CustomAuthentication(Actions.RolesList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerRole(Request, param);

            var data = await _roleClientService.FindAll(filter);

            var dataToShow = data.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(u => new
                {
                    u.Id,
                    u.Name,
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}