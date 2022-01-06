using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberClientService _subscriberClientService;

        public SubscriberController(ISubscriberClientService subscriberClientService)
        {
            _subscriberClientService = subscriberClientService;
        }

        //
        // GET: /Subscriber/
        [CustomAuthentication(Actions.SubscriberList)]
        public async Task<ActionResult> Index()
        {
            try
            {
                //var subscribers = await _subscriberClientService.FindAll();

                //return View(subscribers.Select(s => s.ToModel()));
                return View();
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

        [HttpPost]
        [CustomAuthentication(Actions.SubscriberDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _subscriberClientService.Delete(id);
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

        [CustomAuthentication(Actions.SubscriberList)]
        public async Task<ActionResult> ExcelExport()
        {
            var items = await _subscriberClientService.FindAll();

            var data = from p in items
                       select new
                       {
                           p.Name,
                           p.Surname,
                           p.Email,
                       };

            var headers = new[]
                {
                    EntitiesDtosStrings.SubscriberDto_Name,
                    EntitiesDtosStrings.SubscriberDto_Surname,
                    EntitiesDtosStrings.SubscriberDto_Email
                };

            var memoryStream = ExcelExporter.ExcelExport("Registros", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"],
                        string.Format("{0}.{1}", "Newsletter", "xlsx"));
        }

        [CustomAuthentication(Actions.SubscriberList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerSubscriber(Request, param);

            var data = await _subscriberClientService.FindAll(filter);

            var dataToShow = data.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            var dataModel = dataToShow.Select(d => d.ToModel());

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
