using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsUserController : BaseController
    {
        private readonly IApplicationUserClientService _applicationUserClientService;

        public ReportsUserController(IApplicationUserClientService applicationUserClientService)
        {
            _applicationUserClientService = applicationUserClientService;
        }

        [CustomAuthentication(Actions.ReportsUsersList)]
        public ActionResult Index()
        {
            var email = Request["UserEmail"];

            try
            {
                return View(new ReportsUserFilterDto()
                {
                    DateTo = DateTime.Now,
                    DateFrom = new DateTime(2014, 01, 01),
                    Email = string.IsNullOrEmpty(email) ? string.Empty : email,
                });
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return null;
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsUsersDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var userDto = await _applicationUserClientService.Find(id);
                return View(userDto.ToModel());
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
        [CustomAuthentication(Actions.ReportsUsersEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {

                var userDto = await _applicationUserClientService.Find(id);
                return View(userDto.ToModel());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.ReportsUsersEdit)]
        public async Task<ActionResult> Edit(ApplicationUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                await _applicationUserClientService.Edit(model.ToDto());
                ShowNotification(PresentationAdminStrings.ApplicationUser_Edit_Sueccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException)
            {
                ShowNotification(PresentationAdminStrings.ApplicationUser_Edit_Error, NotificationType.Error);
            }

            return View(model);
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsUsersDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _applicationUserClientService.Delete(id);
                ShowNotification(PresentationAdminStrings.ApplicationUser_Edit_Sueccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException)
            {
                ShowNotification(PresentationAdminStrings.ApplicationUser_Edit_Error, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [CustomAuthentication(Actions.ReportsUsersList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsUser(Request, param);

            var data = await _applicationUserClientService.GetDataForReportsUser(filter);
            var totalRecords = await _applicationUserClientService.GetDataForReportsUserCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                CreationDate = d.CreationDate.ToString("yyyy/MM/dd hh:mm"),
                Email = d.Email,
                Name = d.Name,
                Surname = d.Surname,
                CallCenterKey = d.CallCenterKey,
                PaymentCount = d.PaymentDtos != null ? d.PaymentDtos.Count : 0,
                CardsCount = d.CardDtos != null ? d.CardDtos.Count : 0,
                ServiceAsociatedCount = d.ServicesAssociated != null ? d.ServicesAssociated.Count : 0,
                Status = d.MembershipIdentifierObj.Blocked ? EnumsStrings.ActiveOrInactiveEnumDto_Inactive :
                    d.MembershipIdentifierObj.Active ? EnumsStrings.ActiveOrInactiveEnumDto_Active : EnumsStrings.ActiveOrInactiveEnumDto_Inactive,
                StatusValue = d.MembershipIdentifierObj.Blocked ? (int)ActiveOrInactiveEnumDto.Blocked :
                d.MembershipIdentifierObj.Active ? (int)ActiveOrInactiveEnumDto.Active : (int)ActiveOrInactiveEnumDto.Inactive,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsUsersEdit)]
        public async Task<ActionResult> ChangeBlockStatusUser(Guid id)
        {
            try
            {
                var result = await _applicationUserClientService.ChangeBlockStatusUser(id);
                if (result)
                {
                    return Json(new JsonResponse(AjaxResponse.Success, string.Empty, string.Empty, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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
            return Json(new JsonResponse(AjaxResponse.Error, "", "ERROR", PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsUsersChangePassword)]
        public async Task<ActionResult> ChangePassword(ChangeWebUserPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", PresentationCoreMessages.InvalidFields, PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
                await _applicationUserClientService.ChangePassword(model.UserId, model.NewPassword);
                return Json(new JsonResponse(AjaxResponse.Success, string.Empty, string.Empty, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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

    }
}