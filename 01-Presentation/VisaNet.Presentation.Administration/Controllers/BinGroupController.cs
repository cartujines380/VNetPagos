using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class BinGroupController : BaseController
    {
        private readonly IBinGroupClientService _binGroupService;
        private readonly IBinsClientService _binService;
        private readonly IServiceClientService _serviceService;
        public BinGroupController(IBinGroupClientService binGroupService, IBinsClientService binService, IServiceClientService serviceService)
        {
            _binGroupService = binGroupService;
            _binService = binService;
            _serviceService = serviceService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinGroupList)]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinGroupCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                var model = new BinGroupModel()
                {
                    Bins = new List<BinModel>()
                };

                await LoadsViewBags(null, model);

                return View(model);
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
        [CustomAuthentication(Actions.BinGroupCreate)]
        public async Task<ActionResult> Create(BinGroupModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //Validar que tenga al menos service seleccionado
                var servicesCount = model.Services == null ? 0 : model.Services.Count(x => x != Guid.Empty);
                if (servicesCount == 0)
                {
                    await LoadsViewBags(null, model);
                    ShowNotification(PresentationAdminStrings.Service_Required, NotificationType.Error);
                    return View(model);
                }

                await _binGroupService.Create(model.ToDto());
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

            return View(model);
        }

        [HttpPost]
        [CustomAuthentication(Actions.BinGroupDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _binGroupService.Delete(id);

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
        [CustomAuthentication(Actions.BinGroupEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _binGroupService.Find(id);

                var model = dto.ToModel();
                await LoadsViewBags(dto, model);

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
        [CustomAuthentication(Actions.BinGroupEdit)]
        public async Task<ActionResult> Edit(BinGroupModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //Validar que tenga al menos service seleccionado
                var servicesCount = model.Services == null ? 0 : model.Services.Count(x => x != Guid.Empty);
                if (servicesCount == 0)
                {
                    await LoadsViewBags(null, model);
                    ShowNotification(PresentationAdminStrings.Service_Required, NotificationType.Error);
                    return View(model);
                }
                


                await _binGroupService.Edit(model.ToDto());
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

            return View(model);
        }

        [CustomAuthentication(Actions.BinGroupList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerBinGroup(Request, param);

            var data = await _binGroupService.FindAll(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name
            });

            if (filter.SortDirection == SortDirection.Desc) //invertido porque ordenaba primero al reves
            {
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? p.Name : "");
                //filter.OrderBy == "3" ? p.CardType.ToString() : "");
            }
            else
            {
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? p.Name : "");
                //filter.OrderBy == "3" ? p.CardType.ToString() : "");
            }

            var dataToShow = dataModel.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(b => new
                {
                    b.Id,
                    b.Name
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinGroupDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _binGroupService.Find(id);

                var model = dto.ToModel();
                await LoadsViewBags(dto, model);
                
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

        public async Task<ActionResult> FilterBins(BinFilterDto filter)
        {
            var bins = await _binService.GetDataForTable(filter);
            var result = bins.Select(x => x.ToModel());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        private async Task LoadsViewBags(BinGroupDto dto, BinGroupModel model)
        {
            model.ServicesSelectList = new List<SelectListItem>();

            var allServices = await _serviceService.FindAll();

            foreach (var s in allServices)
            {
                model.ServicesSelectList.Add(new SelectListItem()
                {
                    Selected = dto != null ? dto.Services.Any(w => w.Id == s.Id) : false,
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
            }

            if (dto != null)
                ViewBag.ServicesJson = JsonConvert.SerializeObject(model.ServicesSelectList.Where(w => w.Selected).Select(s => s.Value)).Replace("\"", "'");
        }
    }
}