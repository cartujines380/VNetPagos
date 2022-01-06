using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.CustomerSite.EntitiesDtos;
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
    public class CustomerSiteBranchController : BaseController
    {
        private readonly IServiceClientService _serviceClientService;
        private readonly ICustomerSiteCommerceClientService _customerSiteCommerceClientService;
        private readonly ICustomerSiteBranchClientService _customerSiteBranchClientService;
        private readonly IServiceValidatorClientService _serviceValidatorClientService;

        public CustomerSiteBranchController(IServiceClientService serviceClientService, ICustomerSiteCommerceClientService customerSiteCommerceClientService,
            ICustomerSiteBranchClientService customerSiteBranchClientService, IServiceValidatorClientService serviceValidatorClientService)
        {
            _serviceClientService = serviceClientService;
            _customerSiteCommerceClientService = customerSiteCommerceClientService;
            _customerSiteBranchClientService = customerSiteBranchClientService;
            _serviceValidatorClientService = serviceValidatorClientService;
        }

        [CustomAuthentication(Actions.CustomerSiteBranchList)]
        public async Task<ActionResult> Index()
        {
            try
            {
                await LoadViewBagData(null, null);
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

        [CustomAuthentication(Actions.CustomerSiteBranchList)]
        public async Task<ActionResult> IndexWithCommerce(Guid id)
        {
            try
            {
                await LoadViewBagData(id.ToString(), null);
                return View("Index");
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
        [CustomAuthentication(Actions.CustomerSiteBranchCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                var model = new CustomerSiteBranchModel();
                await LoadViewBagData(null, null);
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

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteBranchCreate)]
        public async Task<ActionResult> CreateWtihCommerce(Guid id)
        {
            //id del comercio
            try
            {
                var model = new CustomerSiteBranchModel();
                await LoadViewBagData(id.ToString(), null);
                return View("Create", model);
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
        [CustomAuthentication(Actions.CustomerSiteBranchCreate)]
        public async Task<ActionResult> Create(CustomerSiteBranchModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(model.CustomerSiteCommerce, model.ServiceId);
                    return View(model);
                }

                if (model.ServiceId == null || string.Equals(model.ServiceId, Guid.Empty.ToString()))
                {
                    ShowNotification("No se selecciono un servicio VNP", NotificationType.Success);
                    await LoadViewBagData(model.CustomerSiteCommerce, model.ServiceId);
                    return View(model);
                }

                if (model.CustomerSiteCommerce == null || string.Equals(model.CustomerSiteCommerce, Guid.Empty.ToString()))
                {
                    ShowNotification("No se selecciono un comercio", NotificationType.Error);
                    await LoadViewBagData(model.CustomerSiteCommerce, model.ServiceId);
                    return View(model);
                }

                var isValidService = await _serviceValidatorClientService.ValidateLinkService(Guid.Parse(model.ServiceId));
                if (!isValidService)
                {
                    ShowNotification("El servicio seleccionado no tiene la configuración adecuada para PagoLink", NotificationType.Error);
                    await LoadViewBagData(model.CustomerSiteCommerce, model.ServiceId);
                    return View(model);
                }

                var dto = model.ToDto();

                await _customerSiteBranchClientService.Create(dto);
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
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model != null ? model.CustomerSiteCommerce : null, model != null ? model.ServiceId : null);
            return View(model);
        }

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteBranchDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var branch = await _customerSiteBranchClientService.Find(id);
                await LoadViewBagData(branch.CommerceId, branch.ServiceId);
                return View(branch.ToModel());
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
        [CustomAuthentication(Actions.CustomerSiteBranchEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var branch = await _customerSiteBranchClientService.Find(id);
                await LoadViewBagData(branch.CommerceId, branch.ServiceId);
                return View(branch.ToModel());
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
        [CustomAuthentication(Actions.CustomerSiteBranchEdit)]
        public async Task<ActionResult> Edit(CustomerSiteBranchModel model)
        {
            try
            {
                var dto = model.ToDto();
                await _customerSiteBranchClientService.Edit(dto);
                ShowNotification(PresentationAdminStrings.CustomerSite_Success, NotificationType.Success);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            catch (Exception exception)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model != null ? model.CustomerSiteCommerce : null, model != null ? model.ServiceId : null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthentication(Actions.CustomerSiteBranchDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _customerSiteBranchClientService.Delete(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_DeleteSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Error_General_Model, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Error_General_Model, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception exception)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Error_General_Model, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [CustomAuthentication(Actions.CustomerSiteBranchList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCustomerSiteBranch(Request, param);
            var data = await _customerSiteBranchClientService.GetDataForBranchTable(filter);
            var totalRecords = await _customerSiteBranchClientService.GetDataForBranchTableCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                CustomerSiteUser = d.SystemUserDto != null ? d.SystemUserDto.Email : string.Empty,
                Disabled = d.Disabled ? "1" : "0",
                Commerce = d.CommerceDto != null ? d.CommerceDto.Name : string.Empty
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthentication(Actions.CustomerSiteBranchEnable)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _customerSiteBranchClientService.ChangeState(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_Status_Success, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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

        public async Task<JsonResult> GetServicesLists(string commerceId)
        {
            try
            {
                var commerce = await _customerSiteCommerceClientService.Find(Guid.Parse(commerceId));
                var service = await _serviceClientService.Find(Guid.Parse(commerce.ServiceId));
                var list = service.Container ? await _serviceClientService.GetServicesFromContainer(service.Id) : null;
                ViewBag.ServicesLigthList = list != null ? GetServicesLigthFromServiceList(list, string.Empty) : await GetServicesFromSelectedCommerce(commerce.ServiceId);
                var content = RenderPartialViewToString("_ServicesDropDownList");
                return Json(new JsonResponse(AjaxResponse.Success, content, PresentationCoreMessages.NotificationSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (Exception)
            {
            }
            return Json(new JsonResponse(AjaxResponse.Error, "", "", PresentationCoreMessages.NotificationFail, NotificationType.Error));
        }

        public async Task<bool> LoadViewBagData(string commerceId, string selectedServiceId)
        {
            var listCommerces = await GetCommercesLigthList();
            var commerce = string.IsNullOrEmpty(commerceId) ? null : listCommerces.FirstOrDefault(x => x.Id == Guid.Parse(commerceId));
            //var firstCommerce = listCommerces.FirstOrDefault();
            var list = GetCommercesLigth(listCommerces, string.IsNullOrEmpty(commerceId) ? Guid.Empty : Guid.Parse(commerceId));
            ViewBag.CommercesLigthList = list;
            if (string.IsNullOrEmpty(commerceId))
            {
                ViewBag.ServicesLigthList = new List<SelectListItem>();
            }
            else
            {
                ViewBag.ServicesLigthList = await GetServicesFromSelectedCommerce(commerce.ServiceId);
            }

            return true;
        }

        private async Task<ICollection<CustomerSiteCommerceDto>> GetCommercesLigthList()
        {
            return await _customerSiteCommerceClientService.GetCommercesLigth();
        }

        private async Task<List<SelectListItem>> GetServicesFromSelectedCommerce(string commerceServiceId)
        {
            var list = new List<SelectListItem>();
            var service = await _serviceClientService.Find(Guid.Parse(commerceServiceId));
            if (service.Container)
            {
                var servicesFromContainer = await _serviceClientService.GetServicesFromContainer(service.Id);
                list.AddRange(
                    servicesFromContainer.Select(
                        dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name }));
            }
            else
            {
                list.AddRange(new[] { new SelectListItem() { Value = service.Id.ToString(), Text = service.Name } });
            }

            return list;
        }

        private List<SelectListItem> GetServicesLigthFromServiceList(ICollection<ServiceDto> services, string serviceId)
        {
            var id = string.IsNullOrEmpty(serviceId) ? Guid.Empty : Guid.Parse(serviceId);
            var aux = services.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == id });
            var list = new List<SelectListItem>();
            list.AddRange(aux);
            return list;
        }

        private List<SelectListItem> GetCommercesLigth(ICollection<CustomerSiteCommerceDto> commerces, Guid commerceId)
        {
            var aux = commerces.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == commerceId });
            var list = new List<SelectListItem>() { };
            list.AddRange(aux);
            return list;
        }

    }
}