using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.CallCenter.Models;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Controllers
{
    [Authorize]
    public class LogController : BaseController
    {
        private readonly IWebLogClientService _logClientService;

        public LogController(IWebLogClientService logClientService)
        {
            _logClientService = logClientService;
        }

        //
        // GET: /CallCenter/Log/
        public async Task<ActionResult> Index()
        {
            var currentSelectedUser = await CurrentSelectedUser();
            if (currentSelectedUser == null)
                return RedirectToAction("Index", "CallCenterDashboard");

            return View(new LogFiltersModel
            {
                DateFrom = DateTime.Now.AddDays(-10).Date,
                DateTo = DateTime.Now.Date,
                UserName = currentSelectedUser.Email,
            });
        }

        public async Task<ActionResult> GetLogs(LogFiltersModel logFiltersModel)
        {
            try
            {
                var currentSelectedUser = await CurrentSelectedUser();
                var logs = await _logClientService.FindAll(new LogFilterDto
                {
                    SelectedUserId = currentSelectedUser.Id,
                    From = logFiltersModel.DateFrom.Date,
                    To = logFiltersModel.DateTo.Date,
                    LogType = (logFiltersModel.LogType.HasValue ? (int)logFiltersModel.LogType : (int?)null),
                });

                var logsModels = logs.Select(l => new CallCenterLogModel
                {
                    DateTime = l.DateTime,
                    LogType = l.LogType,
                    LogUserType = l.LogUserType,
                    LogCommunicationType = l.LogCommunicationType,
                    Message = l.CallCenterMessage,
                }).ToList();

                return PartialView("_LogList", logsModels);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var log = await _logClientService.Find(id);

                var content = RenderPartialViewToString("_DetailsLightbox", log);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success));
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