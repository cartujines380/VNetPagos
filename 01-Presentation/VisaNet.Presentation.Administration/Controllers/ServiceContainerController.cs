using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ServiceContainerController : BaseController
    {
        private readonly IServiceCategoryClientService _serviceCategoryClientService;
        private readonly IServiceClientService _serviceClientService;
        private readonly IGatewayClientService _gatewayClientService;
        private readonly IBinGroupClientService _binGroupsService;
        private readonly string _imageFolder = ConfigurationManager.AppSettings["AzureServicesImageFolder"];

        public ServiceContainerController(IServiceClientService serviceClientService, IServiceCategoryClientService serviceCategoryClientService,
            IGatewayClientService gatewayClientService, IBinGroupClientService binGroupsService)
        {
            _serviceClientService = serviceClientService;
            _serviceCategoryClientService = serviceCategoryClientService;
            _gatewayClientService = gatewayClientService;
            _binGroupsService = binGroupsService;
        }

        private async Task LoadViewBag(List<Guid> binGroups = null, int? quotaSelected = null)
        {
            //cargar siempre
            var categories = await _serviceCategoryClientService.FindAll();
            ICollection<SelectListItem> list = categories.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name }).ToList();
            ViewBag.Categories = list;
            ViewBag.BinGroups = await GenerateBinGroups();
            if (binGroups != null) ViewBag.SelectedBinGroups = JsonConvert.SerializeObject(binGroups).Replace("\"", "'");

            ViewBag.IntegrationVersion = GenerateUrlIntegrationVersionList();
            ViewBag.Quotas = GenerateQuotas(quotaSelected);
        }

        private async Task<List<SelectListItem>> GenerateBinGroups()
        {
            var allBinGroups = await _binGroupsService.FindAll();

            return allBinGroups.Select(binGroup => new SelectListItem
            {
                Text = binGroup.Name,
                Value = binGroup.Id.ToString()
            }).ToList();
        }
        private List<SelectListItem> GenerateQuotas(int? selected)
        {
            var list = new List<SelectListItem>();
            var quotaConf = ConfigurationManager.AppSettings["MaxNumberQuotas"];
            var maxQuotas = !string.IsNullOrEmpty(quotaConf) ? int.Parse(quotaConf) : 12;
            for (int i = 1; i <= maxQuotas; i++)
            {
                list.Add(new SelectListItem()
                {
                    Selected = (i == (selected.HasValue ? selected.Value : 1)),
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            return list;
        }
        private List<SelectListItem> GenerateUrlIntegrationVersionList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(UrlIntegrationVersionEnumDto)).Cast<UrlIntegrationVersionEnumDto>();
            return list.Select(version => new SelectListItem()
            {
                Text = rm.GetString(string.Concat("UrlIntegrationVersionEnumDto_", version.ToString())),
                Value = (int)version + "",
            }).ToList();
        }
        private async Task<List<SelectListItem>> GenerateCategoriesList()
        {
            var categories = await _serviceCategoryClientService.FindAll();
            var list = categories.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name }).ToList();
            return list;
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceContainerList)]
        public async Task<ActionResult> Index()
        {
            ViewBag.Categories = await GenerateCategoriesList();
            var serviceName = Request["ServiceName"];
            ViewBag.ServiceContainerName = Request["ServiceContainerName"] ?? "";
            return View("Index", new ServiceFilterDto()
            {
                Name = serviceName
            });
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceContainerCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                await LoadViewBag();

                var model = new ServiceContainerModel
                {
                    Active = true,
                    Container = true,
                    ServiceGateways = new Collection<ServiceGatewayModel>(),
                    UrlIntegrationVersion = (int)(Enum.GetValues(typeof(UrlIntegrationVersionEnumDto)).Cast<UrlIntegrationVersionEnumDto>().Max()),
                    CertificateThumbprintVisa = ConfigurationManager.AppSettings["ActualThumbprintVNP"],
                };
                var gatewaysAll = await _gatewayClientService.FindAll();
                var gateways = gatewaysAll.Where(x => x.Enum == (int)GatewayEnum.Apps || x.Enum == (int)GatewayEnum.PagoLink).ToList();
                foreach (var gt in gateways)
                {
                    model.ServiceGateways.Add(new ServiceGatewayModel()
                    {
                        Active = false,
                        GatewayId = gt.Id,
                        GatewayName = gt.Name,
                        GatewayEnum = gt.Enum
                    });
                }
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
        [CustomAuthentication(Actions.ServiceContainerCreate)]
        public async Task<ActionResult> Create(ServiceContainerModel service)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag(service.BinGroups);
                    return View(service);
                }

                #region validations
                if ((string.IsNullOrEmpty(service.UrlName) || string.IsNullOrEmpty(service.ExternalUrlAdd) || string.IsNullOrEmpty(service.ExternalUrlRemove) ||
                    string.IsNullOrEmpty(service.CertificateThumbprintExternal) || string.IsNullOrEmpty(service.CertificateThumbprintVisa))
                    && service.ServiceCategoryId.Equals(Guid.Empty.ToString()))
                {
                    if (string.IsNullOrEmpty(service.UrlName))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.UrlNameEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.ExternalUrlAdd))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.ExternalUrlAddEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.ExternalUrlRemove))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.ExternalUrlRemoveEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.CertificateThumbprintExternal))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintExternalEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.CertificateThumbprintVisa))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintVisaEmpty, NotificationType.Error);
                    }
                    return View(service);
                }

                var binGroupCount = service.BinGroups == null ? 0 : service.BinGroups.Count(x => x != Guid.Empty);
                if (binGroupCount == 0)
                {
                    await LoadViewBag(service.BinGroups);
                    ShowNotification(PresentationAdminStrings.BinGroup_Required, NotificationType.Error);
                    return View(service);
                }

                #endregion

                var serviceId = Guid.NewGuid();
                HttpPostedFileBase imageService = null;

                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !String.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBag();
                            return View(service);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBag();
                            return View(service);
                        }
                        if (file.Equals("Image"))
                        {
                            service.Image = image.FileName;
                            imageService = image;
                        }
                    }
                }

                var dto = service.ToDto();
                dto.Id = serviceId;
                await _serviceClientService.Create(dto);

                if (imageService != null)
                {
                    CreateImageAzure(service.ImageBlobName, imageService.InputStream, imageService.ContentType, _imageFolder);
                }

                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - WebApiClientBusinessException");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - WebApiClientFatalException");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - Exception");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            await LoadViewBag();
            return View(service);
        }

        [HttpPost]
        [CustomAuthentication(Actions.ServiceContainerDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var service = await _serviceClientService.Find(id);
                await _serviceClientService.Delete(id);
                try
                {
                    if (!string.IsNullOrEmpty(service.ImageName))
                    {
                        DeleteImageAzure(service.ImageName, _imageFolder);
                    }
                }
                catch (Exception)
                {
                }
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_DeleteSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - WebApiClientBusinessException");
                NLogLogger.LogEvent(ex);
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - WebApiClientFatalException");
                NLogLogger.LogEvent(ex);
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Create - Exception");
                NLogLogger.LogEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, "", e.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceContainerEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _serviceClientService.Find(id);
                await LoadViewBag(dto.BinGroups.Select(x => x.Id).ToList(), dto.MaxQuotaAllow);
                var gatewaysAll = await _gatewayClientService.FindAll();
                var gateways = gatewaysAll.Where(x => x.Enum == (int)GatewayEnum.Apps || x.Enum == (int)GatewayEnum.PagoLink).ToList();
                var model = dto.ToModel(gateways);
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
        [CustomAuthentication(Actions.ServiceContainerEdit)]
        public async Task<ActionResult> Edit(ServiceContainerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag();
                    return View(model);
                }

                #region validations

                if ((string.IsNullOrEmpty(model.UrlName) || string.IsNullOrEmpty(model.ExternalUrlAdd) ||
                     string.IsNullOrEmpty(model.ExternalUrlRemove) ||
                     string.IsNullOrEmpty(model.CertificateThumbprintExternal) ||
                     string.IsNullOrEmpty(model.CertificateThumbprintVisa))
                    && model.ServiceCategoryId.Equals(Guid.Empty.ToString()))
                {
                    if (string.IsNullOrEmpty(model.UrlName))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.UrlNameEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.ExternalUrlAdd))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.ExternalUrlAddEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.ExternalUrlRemove))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.ExternalUrlRemoveEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.CertificateThumbprintExternal))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintExternalEmpty,
                            NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.CertificateThumbprintVisa))
                    {
                        await LoadViewBag();
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintVisaEmpty, NotificationType.Error);
                    }
                    return View(model);
                }

                var binGroupCount = model.BinGroups == null ? 0 : model.BinGroups.Count(x => x != Guid.Empty);
                if (binGroupCount == 0)
                {
                    await LoadViewBag(model.BinGroups);
                    ShowNotification(PresentationAdminStrings.BinGroup_Required, NotificationType.Error);
                    return View(model);
                }

                #endregion

                //si quiso borrar la imagen
                if (model.DeleteImage)
                {
                    DeleteImageAzure(model.Image, _imageFolder);
                    model.Image = null;
                }

                //si hay imagenes nuevas
                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !String.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBag();
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBag();
                            return View(model);
                        }
                        if (file.Equals("Image"))
                        {
                            model.Image = image.FileName;
                            DeleteImageAzure(model.ImageBlobName, _imageFolder);
                            CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _imageFolder);
                        }
                    }
                }

                var dto = model.ToDto();
                await _serviceClientService.Edit(dto);

                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Edit - WebApiClientBusinessException");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Edit - WebApiClientFatalException");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepction - ServiceController - Edit - Exception");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }

            await LoadViewBag(model.BinGroups);
            return View(model);
        }

        [CustomAuthentication(Actions.ServiceContainerList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerServiceContainer(Request, param);
            filter.IsContainer = true;
            var data = await _serviceClientService.FindAll(filter);
            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Status = d.Active ? "Activo" : "Inactivo",
                StatusActive = d.Active,
                Name = d.Name,
                ServiceCategoryName = d.ServiceCategoryName,
                Description = d.Description,
            });

            if (filter.SortDirection == SortDirection.Desc)
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? p.Name :
                                        filter.OrderBy == "1" ? p.ServiceCategoryName :
                                        filter.OrderBy == "2" ? p.Status :
                                        "");
            else
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? p.Name :
                                        filter.OrderBy == "1" ? p.ServiceCategoryName :
                                        filter.OrderBy == "2" ? p.Status :
                                        "");

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

        [HttpGet]
        [CustomAuthentication(Actions.ServiceContainerDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _serviceClientService.Find(id);
                await LoadViewBag(dto.BinGroups.Select(x => x.Id).ToList(), dto.MaxQuotaAllow);
                var gatewaysAll = await _gatewayClientService.FindAll();
                var gateways = gatewaysAll.Where(x => x.Enum == (int)GatewayEnum.Apps || x.Enum == (int)GatewayEnum.PagoLink).ToList();
                var model = dto.ToModel(gateways);
                model.IsDetailsView = true;
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

        [HttpPost]
        [CustomAuthentication(Actions.ServiceContainerEdit)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _serviceClientService.ChangeStatus(id);
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

    }
}