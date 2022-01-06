using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class NotificationController : BaseController
    {
        private readonly IWebNotificationClientService _notificationClientService;

        public NotificationController(IWebNotificationClientService notificationClientService)
        {
            _notificationClientService = notificationClientService;
        }

        //
        // GET: /Private/Notification/
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetNotificationsAjax(NotificationFilterDto filter)
        {
            try
            {
                filter.UserId = (await CurrentSelectedUser()).Id;
                ViewBag.DisplayStart = filter.DisplayStart;
                ViewBag.DisplayLength = filter.DisplayLength;
                var notifications = await _notificationClientService.FindAll(filter);
                //Obtengo solo los servicios asociados activos

                var list = notifications.Select(noti => new NotificationModel()
                                                        {
                                                            Date = noti.Date,
                                                            Message = noti.Message,
                                                            RegisteredUserEmail = noti.RegisteredUser.Email,
                                                            NotificationPrupose = noti.NotificationPrupose,
                                                            //TODO: mostrar descripcion del serv asociado (no del servicio)
                                                            ServiceDesc = noti.Service.Description,
                                                            ServiceName = noti.Service.Name,
                                                            RegisteredUserId = noti.RegisteredUserId
                                                        }).ToList();

                ViewBag.IsSearch = filter.From != default(DateTime) || filter.To != default(DateTime) || !String.IsNullOrEmpty(filter.Service);

                return PartialView("_NotificationList", list);
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

    }
}