using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactClientService _contactClientService;
        private readonly ISystemUserClientService _systemUserClientService;

        public ContactController(IContactClientService contactClientService, ISystemUserClientService systemUserClientService)
        {
            _contactClientService = contactClientService;
            _systemUserClientService = systemUserClientService;
        }

        [CustomAuthentication(Actions.ContactList)]
        public ActionResult Index()
        {
            try
            {
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

        [HttpGet]
        [CustomAuthentication(Actions.ContactDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var contact = await _contactClientService.Find(id);
                var content = RenderPartialViewToString("_DetailsLightbox", contact.ToModel());
                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
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
        [CustomAuthentication(Actions.ContactEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var contact = await _contactClientService.Find(id);
                var content = RenderPartialViewToString("_EditLightbox", contact.ToModel());
                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
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
        [CustomAuthentication(Actions.ContactEdit)]
        public async Task<ActionResult> Edit(ContactModel model)
        {
            try
            {
                var user = await _systemUserClientService.Find(User.Identity.Name);
                await _contactClientService.Edit(new ContactDto()
                {
                    Comments = model.Comments,
                    Taken = true,
                    UserTookId = user.Id,
                    Id = model.Id
                });
                ShowNotification(PresentationAdminStrings.Contact_Sueccess, NotificationType.Success);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationAdminStrings.Contacts_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationAdminStrings.Contacts_Error, NotificationType.Error);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthentication(Actions.ContactDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _contactClientService.Delete(id);
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

        [CustomAuthentication(Actions.ContactList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerContact(Request, param);
            var data = await _contactClientService.FindAll(filter);
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