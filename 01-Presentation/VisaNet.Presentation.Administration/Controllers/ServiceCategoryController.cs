using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ServiceCategoryController : BaseController
    {
        private readonly IServiceCategoryClientService _serviceCategoryClientService;

        public ServiceCategoryController(IServiceCategoryClientService serviceCategoryClientService)
        {
            _serviceCategoryClientService = serviceCategoryClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCategoryList)]
        public ActionResult Index()
        {
            try
            {
                //var categories = await _serviceCategoryClientService.FindAll();
                //var listado = categories.Select(dto => new ServiceCategoryModel()
                //{
                //    Name = dto.Name,
                //    Id = dto.Id
                //}).ToList();

                //ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);

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
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.ServiceCategoryCreate)]
        public async Task<ActionResult> Create(ServiceCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid) { return View(model); }
                await _serviceCategoryClientService.Create(new ServiceCategoryDto() { Name = model.Name });
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return View(model);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return View(model);
            }

            return RedirectToAction("Index", "ServiceCategory");
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCategoryCreate)]
        public async Task<ActionResult> Create()
        {
            return View(new ServiceCategoryModel());
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCategoryEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _serviceCategoryClientService.Find(id);
                return View(new ServiceCategoryModel() { Id = dto.Id, Name = dto.Name });
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

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCategoryDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _serviceCategoryClientService.Find(id);
                return View(new ServiceCategoryModel() { Id = dto.Id, Name = dto.Name });
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
        [CustomAuthentication(Actions.ServiceCategoryDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _serviceCategoryClientService.Delete(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.ServiceCategoryEdit)]
        public async Task<ActionResult> Edit(ServiceCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid) { return RedirectToAction("Create"); }
                await _serviceCategoryClientService.Edit(new ServiceCategoryDto() { Name = model.Name, Id = model.Id });
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

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCategoryList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerServiceCategory(Request, param);

            var data = await _serviceCategoryClientService.FindAll(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
            });

            if (filter.SortDirection == SortDirection.Desc) //invertido porque ordenaba primero al reves
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? p.Name : "");
            else
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? p.Name : "");

            var dataToShow = dataModel.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

    }
}