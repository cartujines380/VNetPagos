using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
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
    public class BankController : BaseController
    {
        private readonly IBankClientService _bankClientService;
        private readonly IAffiliationCardClientService _affiliationCardClientService;

        public BankController(IBankClientService bankClientService, IAffiliationCardClientService affiliationCardClientService)
        {
            _bankClientService = bankClientService;
            _affiliationCardClientService = affiliationCardClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.BankList)]
        public async Task<ActionResult> Index()
        {
            ViewBag.BankName = Request["BankName"] ?? "";
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.BankCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                LoadViewBags();
                return View(new BankModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Index");
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.BankCreate)]
        public async Task<ActionResult> Create(BankModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBags(model: model);
                    return View(model);
                }

                var quotes = Request["QuotesPermited"];
                if (!quotes.Contains("1"))
                {
                    await LoadViewBags(model: model);
                    ShowNotification(PresentationAdminStrings.Bank_FirstQuota_NotSelected, NotificationType.Alert);
                    return View(model);
                }

                model.QuotesPermited = quotes;

                await _bankClientService.Create(model.ToDto());
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
            await LoadViewBags();
            return View(model);
        }

        [CustomAuthentication(Actions.BankDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _bankClientService.Delete(id);
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
        [CustomAuthentication(Actions.BankEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _bankClientService.Find(id);
                var model = dto.ToModel();
                await LoadViewBags(dto, model);
                return View(model);
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
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.BankEdit)]
        public async Task<ActionResult> Edit(BankModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBags(model: model);
                    return View(model);
                }

                var quotes = Request["QuotesPermited"];
                if (!quotes.Contains("1"))
                {
                    await LoadViewBags(model: model);
                    ShowNotification(PresentationAdminStrings.Bank_FirstQuota_NotSelected, NotificationType.Alert);
                    return View(model);
                }

                model.QuotesPermited = quotes;

                await _bankClientService.Edit(model.ToDto());
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
            await LoadViewBags(model: model);
            return View(model);
        }

        [CustomAuthentication(Actions.BankList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerBank(Request, param);

            var data = await _bankClientService.FindAll(filter);
            var totalRecords = await _bankClientService.GetDataForBankCount(filter);

            var dataModel = data.Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.QuotesPermited
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);            
        }

        [HttpGet]
        [CustomAuthentication(Actions.BankDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _bankClientService.Find(id);
                var model = dto.ToModel();
                await LoadViewBags(model: model);
                return View(model);
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

        private async Task LoadViewBags(BankDto dto = null, BankModel model = null )
        {
            var list = new List<SelectListItem>();
            var quotaConf = ConfigurationManager.AppSettings["MaxNumberQuotas"];
            var maxQuotas = !string.IsNullOrEmpty(quotaConf) ? int.Parse(quotaConf) : 12;
            for (int i = 1; i <= maxQuotas; i++)
            {
                list.Add(new SelectListItem()
                         {
                             Selected = (i == 1),
                             Text = i.ToString(),
                             Value = i.ToString()
                         });
            }
            ViewBag.Quotas = list;

            if (dto != null)
                ViewBag.SelectedQuotas = JsonConvert.SerializeObject(dto.QuotesPermited.Split(',')).Replace("\"", "'");

            ViewBag.AfiliationCard = new List<AffiliationCardDto>();

            if (model != null)
            {
                var affiliation = await _affiliationCardClientService.FindAll();
                var allAffiliationCards = affiliation.Where(w => w.BankId == model.Id).ToList();                

                ViewBag.AfiliationCard = allAffiliationCards;            
            }
            
        }

    }
}