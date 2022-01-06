using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
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
    public class CustomerSiteCommerceController : BaseController
    {
        private readonly IServiceClientService _serviceClientService;
        private readonly ICustomerSiteCommerceClientService _customerSiteCommerceClientService;
        private readonly IServiceValidatorClientService _serviceValidatorClientService;
        private readonly string _ImageFolder = ConfigurationManager.AppSettings["AzureCommerceImageFolder"];

        public CustomerSiteCommerceController(IServiceClientService serviceClientService, ICustomerSiteCommerceClientService customerSiteCommerceClientService,
            IServiceValidatorClientService serviceValidatorClientService)
        {
            _serviceClientService = serviceClientService;
            _customerSiteCommerceClientService = customerSiteCommerceClientService;
            _serviceValidatorClientService = serviceValidatorClientService;
        }

        [CustomAuthentication(Actions.CustomerSiteCommerceList)]
        public async Task<ActionResult> Index()
        {
            try
            {
                await LoadViewBagData(null);
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

        [CustomAuthentication(Actions.CustomerSiteCommerceList)]
        public async Task<ActionResult> IndexWithId(string commerce)
        {
            try
            {
                await LoadViewBagData(null);
                return View("Index", new CustomerSiteCommerceFilterDto()
                {
                    Name = commerce,
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
            return null;
        }

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteCommerceCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                var model = new CustomerSiteCommerceModel();
                await LoadViewBagData(null);
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
        [CustomAuthentication(Actions.CustomerSiteCommerceCreate)]
        public async Task<ActionResult> Create(CustomerSiteCommerceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(model.ServiceId);
                    return View(model);
                }

                if (model.ServiceId == null || string.Equals(model.ServiceId, Guid.Empty.ToString()))
                {
                    ShowNotification("No se selecciono un servicio VNP", NotificationType.Error);
                    await LoadViewBagData(model.ServiceId);
                    return View(model);
                }

                var isValidService = await _serviceValidatorClientService.ValidateLinkService(Guid.Parse(model.ServiceId));
                if (!isValidService)
                {
                    ShowNotification("El servicio seleccionado no tiene la configuración adecuada para PagoLink", NotificationType.Error);
                    await LoadViewBagData(model.ServiceId);
                    return View(model);
                }

                var service = await _serviceClientService.Find(Guid.Parse(model.ServiceId));
                var dto = model.ToDto();
                //ES UN COMERCIO UNICO. YA CREO LA SUCURSAL
                if (model.CreateBranch)
                {
                    if (service.Container)
                    {
                        var servicesFromContainer = await _serviceClientService.GetServicesFromContainer(service.Id);
                        dto.BranchesDto = new List<CustomerSiteBranchDto>();
                        foreach (var serviceDto in servicesFromContainer)
                        {
                            dto.BranchesDto.Add(
                                new CustomerSiteBranchDto()
                                {
                                    ContactAddress = dto.ContactAddress,
                                    ContactEmail = dto.ContactEmail,
                                    ContactPhoneNumber = dto.ContactPhoneNumber,
                                    Name = serviceDto.Name,
                                    Disabled = false,
                                    ServiceId = serviceDto.Id.ToString(),
                                }
                            );
                        }
                    }
                    else
                    {
                        dto.BranchesDto = new List<CustomerSiteBranchDto>()
                        {
                            new CustomerSiteBranchDto()
                            {
                                ContactAddress = dto.ContactAddress,
                                ContactEmail = dto.ContactEmail,
                                ContactPhoneNumber = dto.ContactPhoneNumber,
                                Name = dto.Name,
                                Disabled = false,
                                ServiceId = service.Id.ToString()
                            }
                        };
                    }
                }

                var commerceId = Guid.NewGuid();
                HttpPostedFileBase imageCommerce = null;
                string imageName = null;

                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !string.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBagData(model.ServiceId);
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBagData(model.ServiceId);
                            return View(model);
                        }
                        if (file.Equals("Image"))
                        {
                            dto.ImageName = image.FileName;
                            imageCommerce = image;
                        }
                    }
                }

                dto.Id = commerceId;
                await _customerSiteCommerceClientService.Create(dto);

                if (imageCommerce != null)
                {
                    CreateImageAzure(dto.ImageBlobName, imageCommerce.InputStream, imageCommerce.ContentType, _ImageFolder);
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
            catch (Exception e)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model != null ? model.ServiceId : null);
            return View(model);
        }

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteCommerceDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var commerce = await _customerSiteCommerceClientService.Find(id);
                await LoadViewBagData(commerce.ServiceId);
                return View(commerce.ToModel());
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
        [CustomAuthentication(Actions.CustomerSiteCommerceEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var commerce = await _customerSiteCommerceClientService.Find(id);
                await LoadViewBagData(commerce.ServiceId);
                return View(commerce.ToModel());
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
        [CustomAuthentication(Actions.CustomerSiteCommerceEdit)]
        public async Task<ActionResult> Edit(CustomerSiteCommerceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(model.ServiceId);
                    return View(model);
                }

                if (model.ServiceId == null || string.Equals(model.ServiceId, Guid.Empty.ToString()))
                {
                    ShowNotification("No se selecciono un servicio VNP", NotificationType.Success);
                    await LoadViewBagData(model.ServiceId);
                    return View(model);
                }

                //si quiso borrar la imagen
                if (model.DeleteImage)
                {
                    DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                    model.ImageName = null;
                }

                //si hay imagenes nuevas
                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !string.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBagData(model.ServiceId);
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBagData(model.ServiceId);
                            return View(model);
                        }
                        if (file.Equals("ImageName"))
                        {
                            model.ImageName = image.FileName;
                            DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                            CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _ImageFolder);
                        }
                    }
                }

                var dto = model.ToDto();
                await _customerSiteCommerceClientService.Edit(dto);

                ShowNotification(PresentationAdminStrings.CustomerSite_Success, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (Exception exception)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model != null ? model.ServiceId : null);
            return View(model);
        }

        [HttpPost]
        [CustomAuthentication(Actions.CustomerSiteCommerceDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _customerSiteCommerceClientService.Delete(id);
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
            catch (Exception exception)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Error_General_Model, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [CustomAuthentication(Actions.CustomerSiteCommerceList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCustomerSiteCommerce(Request, param);
            var data = await _customerSiteCommerceClientService.GetDataForCommerceTable(filter);
            var totalRecords = await _customerSiteCommerceClientService.GetDataForCommerceTableCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Disabled = d.Disabled ? "1" : "0",
                BranchesCount = d.BranchesDto != null ? d.BranchesDto.Count : 0
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
        [CustomAuthentication(Actions.CustomerSiteCommerceEnable)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _customerSiteCommerceClientService.ChangeState(id);
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

        public async Task<bool> LoadViewBagData(string serviceId)
        {
            ViewBag.ServicesLigthList = await GetServicesLightWithoutChildens(serviceId);
            return true;
        }

        private async Task<List<SelectListItem>> GetServicesLightWithoutChildens(string serviceId)
        {
            var services = await _serviceClientService.GetServicesLigthWithoutChildens(Guid.Empty, GatewayEnumDto.PagoLink);
            var id = string.IsNullOrEmpty(serviceId) ? Guid.Empty : Guid.Parse(serviceId);
            var aux = services.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == id });
            var list = new List<SelectListItem>();
            list.AddRange(aux);
            return list;
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            var formats = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return file.ContentType.Contains("image") && formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsSizeCorrect(HttpPostedFileBase file)
        {
            int byteCount = file.ContentLength;
            return byteCount < 5242880; //5MB
        }

    }
}