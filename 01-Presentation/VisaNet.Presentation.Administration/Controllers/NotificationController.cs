using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;


namespace VisaNet.Presentation.Administration.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly IFixedNotificationClientService _fixedNotificationService;

        public NotificationController(IFixedNotificationClientService fixedNotificationService)
        {
            _fixedNotificationService = fixedNotificationService;
        }

        [CustomAuthentication(Actions.NotificationsList)]
        public async Task<ActionResult> GetNotificationsForMenu()
        {
            var notifications = await _fixedNotificationService.FindAll();
            var model = new List<FixedNotificationGroup>();
            var groupedNotifications = notifications.GroupBy(x => x.Description);
            foreach (var group in groupedNotifications)
            {
                if (group.Count() != 1)
                {
                    model.Add(new FixedNotificationGroup
                    {
                        DateTime = group.OrderBy(x => x.DateTime).First().DateTime,
                        QueryString = group.Key,
                        Description = group.First().Description,
                        Level = group.First().Level,
                        Count = group.Count()
                    });
                }
                else
                {
                    model.Add(new FixedNotificationGroup
                    {
                        DateTime = group.OrderBy(x => x.DateTime).First().DateTime,
                        Description = group.First().Description,
                        Id = group.OrderBy(x => x.DateTime).First().Id,
                        Level = group.First().Level,
                        Count = 1
                    });
                }
            }

            return PartialView("_FixedNotifications", model);
        }

        [CustomAuthentication(Actions.NotificationsList)]
        public ActionResult Index(string description, DateTime? from)
        {
            return View(new NotificationFilterDto
            {
                From = from ?? DateTime.Now.AddDays(-1),
                To = DateTime.Now,
                Description = description
            });
        }

        [CustomAuthentication(Actions.NotificationsDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var notif = await _fixedNotificationService.GetById(id);

                return View(new NotificationModel
                {
                    Category = notif.Category,
                    Comments = notif.Comment,
                    DateTime = notif.DateTime,
                    Description = notif.Description,
                    Detail = notif.Detail,
                    Id = notif.Id,
                    Level = notif.Level,
                    Resolved = notif.Resolved
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
            return RedirectToAction("Index");
        }

        [CustomAuthentication(Actions.NotificationsEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var notif = await _fixedNotificationService.GetById(id);

                return View(new NotificationModel
                {
                    Category = notif.Category,
                    Comments = notif.Comment,
                    DateTime = notif.DateTime,
                    Description = notif.Description,
                    Detail = notif.Detail,
                    Id = notif.Id,
                    Level = notif.Level,
                    Resolved = notif.Resolved
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.NotificationsEdit)]
        public async Task<ActionResult> Edit(NotificationModel page)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _fixedNotificationService.Edit(new FixedNotificationDto
                    {
                        Id = page.Id,
                        Comment = page.Comments,
                        Resolved = page.Resolved
                    });
                }
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }

            return View(page);
        }

        [CustomAuthentication(Actions.NotificationsDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerFixedNotification(Request, param);

            var data = await _fixedNotificationService.FindAll(filter);

            var dataToShow = data.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(d => new
                {
                    d.Id,
                    DateTime = d.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    d.Description,
                    d.Detail,
                    Resolved = d.Resolved ? "Si" : "No",
                    Level = EnumHelpers.GetName(typeof(FixedNotificationLevelDto), (int)d.Level, EnumsStrings.ResourceManager),
                    Category = EnumHelpers.GetName(typeof(FixedNotificationCategoryDto), (int)d.Category, EnumsStrings.ResourceManager)
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ResolveAll(JQueryDataTableParamModel param)
        {
            try
            {
                var filter = AjaxHandlers.AjaxHandlerFixedNotification(Request, param);
                var comment = Request.QueryString["Comment"];
                await _fixedNotificationService.ResolveAll(filter, comment);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationWebStrings.Payment_General_Error, PresentationCoreMessages.NotificationFail), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", e.Message, PresentationCoreMessages.NotificationFail), JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}
