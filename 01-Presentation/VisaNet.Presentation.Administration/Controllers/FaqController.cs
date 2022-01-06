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
    public class FaqController : BaseController
    {
        private readonly IFaqClientService _faqClientService;

        public FaqController(IFaqClientService faqClientService)
        {
            _faqClientService = faqClientService;
        }

        //
        // GET: /Faq/
        [CustomAuthentication(Actions.FaqList)]
        public async Task<ActionResult> Index()
        {
            try
            {
                //var faqs = await _faqClientService.FindAll();

                //return View(faqs.Select(f => f.ToModel()));
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

        //
        // GET: /Faq/Details/5
        [CustomAuthentication(Actions.FaqDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            var faq = await _faqClientService.Find(id);

            return View(faq.ToModel());
        }

        //
        // GET: /Faq/Create
        [CustomAuthentication(Actions.FaqCreate)]
        public ActionResult Create()
        {
            return View(new FaqModel());
        }

        //
        // POST: /Faq/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.FaqCreate)]
        public async Task<ActionResult> Create(FaqModel faq)
        {
            try
            {
                await _faqClientService.Create(faq.ToDto());
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Create");
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Create");
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Faq/Edit/5
        [CustomAuthentication(Actions.FaqEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var faq = await _faqClientService.Find(id);

                return View(faq.ToModel());
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

        //
        // POST: /Faq/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.FaqEdit)]
        public async Task<ActionResult> Edit(FaqModel faq)
        {
            try
            {
                await _faqClientService.Edit(faq.ToDto());
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
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
        [CustomAuthentication(Actions.FaqDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _faqClientService.Delete(id);
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

        [CustomAuthentication(Actions.FaqList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerFaq(Request, param);

            var data = await _faqClientService.FindAll(filter);

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
