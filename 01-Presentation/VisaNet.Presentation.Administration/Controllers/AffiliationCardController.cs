using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
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
    public class AffiliationCardController : BaseController
    {
        private readonly IAffiliationCardClientService _affiliationCardClientService;
        private readonly IBankClientService _bankClientService;

        public AffiliationCardController(IAffiliationCardClientService affiliationCardClientService, IBankClientService bankClientService)
        {
            _affiliationCardClientService = affiliationCardClientService;
            _bankClientService = bankClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.AffiliationCardList)]
        public async Task<ActionResult> Index()
        {
            await LoadViewBags();
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.AffiliationCardCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                await LoadViewBags();
                return View(new AffiliationCardModel());
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
        [CustomAuthentication(Actions.AffiliationCardCreate)]
        public async Task<ActionResult> Create(AffiliationCardModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBags();
                    return View(model);
                }

                if (model.Code < 1)
                {
                    ShowNotification("El código del programa debe ser mayor a 0", NotificationType.Error);
                    await LoadViewBags();
                    return View(model);
                }

                await _affiliationCardClientService.Create(model.ToDto());
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
            catch (Exception e)
            {
                ShowNotification("Error general", NotificationType.Error);
            }
            await LoadViewBags();
            return View(model);
        }

        [CustomAuthentication(Actions.AffiliationCardDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _affiliationCardClientService.Delete(id);
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
        [CustomAuthentication(Actions.AffiliationCardEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _affiliationCardClientService.Find(id);
                await LoadViewBags(dto);
                return View(dto.ToModel());
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
        [CustomAuthentication(Actions.AffiliationCardEdit)]
        public async Task<ActionResult> Edit(AffiliationCardModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBags();
                    return View(model);
                }

                if (model.Code < 1)
                {
                    ShowNotification("El código del programa debe ser mayor a 0", NotificationType.Error);
                    await LoadViewBags();
                    return View(model);
                }

                await _affiliationCardClientService.Edit(model.ToDto());
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
            catch (Exception e)
            {
                ShowNotification("Error general", NotificationType.Error);
            }
            await LoadViewBags();
            return View(model);
        }

        [CustomAuthentication(Actions.AffiliationCardList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerAffiliationCard(Request, param);

            var data = await _affiliationCardClientService.GetDataForTable(filter);
            var totalRecords = await _affiliationCardClientService.GetDataForTableCount(filter);

            var dataModel = data.Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.BankId,
                BankName = d.BankDto != null ? d.BankDto.Name : string.Empty,
                Active = d.Active ? "Activo" : "Inactivo",
                StatusActive = d.Active,
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
        [CustomAuthentication(Actions.AffiliationCardDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _affiliationCardClientService.Find(id);
                return View(dto.ToModel());
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
        [CustomAuthentication(Actions.AffiliationCardDisable)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _affiliationCardClientService.ChangeStatus(id);
                return
                    Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_Status_Success,
                        PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
            catch (Exception ex)
            {
                return
                   Json(new JsonResponse(AjaxResponse.Error, "", "Error general.", PresentationCoreMessages.NotificationFail,
                       NotificationType.Error));
            }
        }

        private async Task LoadViewBags(AffiliationCardDto dto = null)
        {
            var banks = await GetBanks();
            var listBanks = banks.Select(d => new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = dto != null ? d.Id == dto.BankId : false }).ToList();
            ViewBag.Banks = listBanks;
            ViewBag.ServiceStatus = GenerateServiceStatusList();
        }

        private async Task<List<BankDto>> GetBanks()
        {
            var banks = new List<BankDto>(){new BankDto()
            {
                Name = "Todos"
            }};
            banks.AddRange(await _bankClientService.FindAll());
            return banks;
        }

        private List<SelectListItem> GenerateServiceStatusList()
        {
            var list = Enum.GetValues(typeof(StatusEnumDto)).Cast<StatusEnumDto>();
            return list.Select(serviceStatus => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(StatusEnumDto), (int)serviceStatus, EnumsStrings.ResourceManager),
                Value = (int)serviceStatus + ""
            }).ToList();
        }

    }
}