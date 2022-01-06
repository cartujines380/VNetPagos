using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Common.Exceptions;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceCategoryClientService _serviceCategoryClientService;
        private readonly IServiceClientService _serviceClientService;
        private readonly IGatewayClientService _gatewayClientService;
        private readonly IBillClientService _billClientService;
        private readonly IPaymentClientService _paymentClientService;
        private readonly IApplicationUserClientService _applicationUserClientService;
        private readonly ILogClientService _logClientService;
        private readonly ICyberSourceAccessClientService _cyberSourceAccessClientService;
        private readonly IInterpreterClientService _interpreterClientService;
        private readonly IBinGroupClientService _binGroupsService;
        private readonly string _imageFolder = ConfigurationManager.AppSettings["AzureServicesImageFolder"];

        public ServiceController(
            IServiceClientService serviceClientService,
            IServiceCategoryClientService serviceCategoryClientService,
            IGatewayClientService gatewayClientService,
            IBillClientService billClientService,
            IPaymentClientService paymentClientService,
            IApplicationUserClientService applicationUserClientService,
            ILogClientService logClientService,
            ICyberSourceAccessClientService cyberSourceAccessClientService,
            IBinGroupClientService binGroupsService,
            IInterpreterClientService interpreterClientService)
        {
            _serviceClientService = serviceClientService;
            _serviceCategoryClientService = serviceCategoryClientService;
            _gatewayClientService = gatewayClientService;
            _billClientService = billClientService;
            _paymentClientService = paymentClientService;
            _applicationUserClientService = applicationUserClientService;
            _logClientService = logClientService;
            _binGroupsService = binGroupsService;
            _cyberSourceAccessClientService = cyberSourceAccessClientService;
            _interpreterClientService = interpreterClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceList)]
        public async Task<ActionResult> Index()
        {
            ViewBag.Categories = await GenerateCategoriesList();
            ViewBag.ServicesLigthList = await GenerateServiceList(Guid.Empty);

            var gateways = await _gatewayClientService.FindAll();
            var gatewaysSelectList = gateways.Select(dto => new SelectListItem() { Value = dto.Enum.ToString(), Text = dto.Name }).ToList();
            var final = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "Todos" } };
            final.AddRange(gatewaysSelectList);
            ViewBag.Gateways = final;
            var serviceName = Request["ServiceName"];
            return View("Index", new ServiceFilterDto()
            {
                Name = serviceName
            });
        }

        [HttpGet]
        [CustomAuthentication(Actions.ServiceCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                await LoadViewBag();

                var model = new ServiceModel
                {
                    ServiceGateways = new Collection<ServiceGatewayModel>(),
                    Active = true,
                    EnableAutomaticPayment = true,
                    EnablePrivatePayment = true,
                    EnablePublicPayment = true,
                    EnableAssociation = true,
                    CertificateThumbprintVisa = ConfigurationManager.AppSettings["ActualThumbprintVNP"],
                    UrlIntegrationVersion = (int)(Enum.GetValues(typeof(UrlIntegrationVersionEnumDto)).Cast<UrlIntegrationVersionEnumDto>().Max()),
                    BinGroupsSelectList = await GenerateBinGroups()
                };

                var gateways = await _gatewayClientService.FindAll();
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
        [CustomAuthentication(Actions.ServiceCreate)]
        public async Task<ActionResult> Create(ServiceModel service)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag(null, service.BinGroups, null, null, service);
                    return View(service);
                }

                #region Validations
                //Validar que si pide referencias, que tenga referencias él o su contenedor
                if (service.AskUserForReferences && !service.ContainerHasReferences && string.IsNullOrEmpty(service.ReferenceParamName))
                {
                    await LoadViewBag(null, service.BinGroups, null, null, service);
                    ShowNotification(PresentationAdminStrings.Service_Needs_References, NotificationType.Error);
                    return View(service);
                }
                //Validar que si permite facutras múltiples tenga la pasarela Sucive o Geocom
                if (service.EnableMultipleBills &&
                    !service.ServiceGateways.Any(g => g.Active && (g.GatewayEnum == (int)GatewayEnumDto.Sucive || g.GatewayEnum == (int)GatewayEnumDto.Geocom)))
                {
                    await LoadViewBag(null, service.BinGroups, null, null, service);
                    ShowNotification(PresentationAdminStrings.Only_sucive, NotificationType.Error);
                    return View(service);
                }
                //Validar parámetros de Pasarelas
                if (!ServiceGatewaysParamsAreValid(service.ServiceGateways))
                {
                    await LoadViewBag(null, service.BinGroups, null, null, service);
                    ShowNotification(PresentationAdminStrings.Apps_CodComercio_CodSucursal_numeric, NotificationType.Error);
                    return View(service);
                }
                //Validar que si tiene pasarela Apps, entonces tenga IdApp, Urls y Thumbprint
                if (service.ServiceGateways.Any(g => g.Active && (g.GatewayEnum == (int)GatewayEnumDto.Apps)) && (
                    string.IsNullOrEmpty(service.UrlName) || string.IsNullOrEmpty(service.ExternalUrlAdd) || string.IsNullOrEmpty(service.ExternalUrlRemove) ||
                    string.IsNullOrEmpty(service.CertificateThumbprintExternal) || string.IsNullOrEmpty(service.CertificateThumbprintVisa))
                    && service.ServiceCategoryId.Equals(Guid.Empty.ToString()))
                {
                    if (string.IsNullOrEmpty(service.UrlName))
                    {
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        ShowNotification(PresentationAdminStrings.UrlNameEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.ExternalUrlAdd))
                    {
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        ShowNotification(PresentationAdminStrings.ExternalUrlAddEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.ExternalUrlRemove))
                    {
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        ShowNotification(PresentationAdminStrings.ExternalUrlRemoveEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.CertificateThumbprintExternal))
                    {
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintExternalEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(service.CertificateThumbprintVisa))
                    {
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintVisaEmpty, NotificationType.Error);
                    }
                    return View(service);
                }
                //Validar que tenga al menos un grupo de bin seleccionado
                var binGroupCount = service.BinGroups == null ? 0 : service.BinGroups.Count(x => x != Guid.Empty);
                if (binGroupCount == 0)
                {
                    await LoadViewBag(null, service.BinGroups, null, null, service);
                    ShowNotification(PresentationAdminStrings.BinGroup_Required, NotificationType.Error);
                    return View(service);
                }

                if (!string.IsNullOrEmpty(service.ServiceContainerId) && service.ServiceContainerId != Guid.Empty.ToString())
                {
                    var serviceContainer = await _serviceClientService.Find(new Guid(service.ServiceContainerId));
                    if (serviceContainer != null)
                    {
                        //Validar que si tiene servicio contenedor, entonces los grupos de bines del hijo no sean distintos a los que permite el contenedor
                        var serviceBinGroups = service.BinGroups.Where(x => x != Guid.Empty).ToList();
                        var serviceContainerBinGroups = serviceContainer.BinGroups.Select(x => x.Id).ToList();
                        if (serviceBinGroups.Intersect(serviceContainerBinGroups).Count() != serviceBinGroups.Count())
                        {
                            await LoadViewBag(null, service.BinGroups, null, null, service);
                            ShowNotification(PresentationAdminStrings.BinGroup_Container_NotMatch, NotificationType.Error);
                            return View(service);
                        }
                    }
                }

                #endregion

                var serviceId = Guid.NewGuid();
                HttpPostedFileBase imageService = null;
                HttpPostedFileBase imageTooltipService = null;
                string imageName = null;
                string imageTooltipName = null;

                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image == null || string.IsNullOrEmpty(image.FileName))
                    {
                        continue;
                    }

                    if (!IsImage(image))
                    {
                        ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        return View(service);
                    }
                    if (!IsSizeCorrect(image))
                    {
                        ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                        await LoadViewBag(null, service.BinGroups, null, null, service);
                        return View(service);
                    }
                    if (file.Equals("Image"))
                    {
                        service.Image = image.FileName;
                        imageService = image;
                    }
                    if (file.Equals("ImageTooltip"))
                    {
                        service.ImageTooltip = image.FileName;
                        imageTooltipService = image;
                    }
                }

                service.ServiceGateways = service.ServiceGateways.Where(x => x.Active).ToList();
                var dto = await Convert(service);
                dto.Id = serviceId;
                await _serviceClientService.Create(dto);

                if (imageService != null)
                {
                    CreateImageAzure(service.ImageBlobName, imageService.InputStream, imageService.ContentType, _imageFolder);
                }
                if (imageTooltipService != null)
                {
                    CreateImageAzure(service.ImageTooltipBlobName, imageTooltipService.InputStream, imageTooltipService.ContentType, _imageFolder);
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
            await LoadViewBag(null, service.BinGroups, null, null, service);
            return View(service);
        }

        [HttpPost]
        [CustomAuthentication(Actions.ServiceDelete)]
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
                    if (!string.IsNullOrEmpty(service.ImageTooltipName))
                    {
                        DeleteImageAzure(service.ImageTooltipName, _imageFolder);
                    }
                }
                catch (Exception)
                {
                }
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
        [CustomAuthentication(Actions.ServiceEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var model = new ServiceModel();
            try
            {
                var dto = await _serviceClientService.Find(id);
                int? maxQuota = null;
                if (dto.ServiceContainerDto != null)
                {
                    maxQuota = dto.ServiceContainerDto.MaxQuotaAllow;
                }

                await LoadViewBag(id, dto.BinGroups.Select(x => x.Id).ToList(), dto.MaxQuotaAllow, maxQuota);
                var gateways = await _gatewayClientService.FindAll();

                model = Convert(dto, gateways, true);
                model.IsEditView = true;
                model.BinGroupsSelectList = await GenerateBinGroups(dto.ServiceContainerId);
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
            catch (BusinessException e)
            {
                if (e.Code == CodeExceptions.CONNECTION_FAILED)
                {
                    model.UploadImageDisabled = true;
                    model.IsEditView = true;
                    ShowNotification(e.Message, NotificationType.Error);
                    return View(model);
                }
                ShowNotification(e.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CustomAuthentication(Actions.ServiceEdit)]
        public async Task<ActionResult> Edit(ServiceModel model)
        {
            var serviceContainerId = !string.IsNullOrEmpty(model.ServiceContainerId) && model.ServiceContainerId != Guid.Empty.ToString() ? new Guid(model.ServiceContainerId) : (Guid?)null;

            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                    return View(model);
                }

                #region Validations
                //Validar que si pide referencias, que tenga referencias él o su contenedor
                if (model.AskUserForReferences && !model.ContainerHasReferences && string.IsNullOrEmpty(model.ReferenceParamName))
                {
                    await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                    ShowNotification(PresentationAdminStrings.Service_Needs_References, NotificationType.Error);
                    return View(model);
                }
                //Validar parámetros de Pasarelas
                if (!ServiceGatewaysParamsAreValid(model.ServiceGateways))
                {
                    await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                    ShowNotification(PresentationAdminStrings.Apps_CodComercio_CodSucursal_numeric, NotificationType.Error);
                    return View(model);
                }
                //Validar que si permite facutras múltiples tenga la pasarela Sucive o Geocom
                if (model.EnableMultipleBills &&
                    !model.ServiceGateways.Any(g => g.Active && (g.GatewayEnum == (int)GatewayEnumDto.Sucive || g.GatewayEnum == (int)GatewayEnumDto.Geocom)))
                {
                    await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                    ShowNotification(PresentationAdminStrings.Only_sucive, NotificationType.Error);
                    return View(model);
                }
                //Validar que si tiene pasarela Apps, entonces tenga IdApp, Urls y Thumbprint
                if (model.ServiceGateways.Any(g => g.Active && (g.GatewayEnum == (int)GatewayEnumDto.Apps)) && (
                    string.IsNullOrEmpty(model.UrlName) || string.IsNullOrEmpty(model.ExternalUrlAdd) || string.IsNullOrEmpty(model.ExternalUrlRemove) ||
                    string.IsNullOrEmpty(model.CertificateThumbprintExternal) || string.IsNullOrEmpty(model.CertificateThumbprintVisa))
                    && model.ServiceCategoryId.Equals(Guid.Empty.ToString()))
                {
                    if (string.IsNullOrEmpty(model.UrlName))
                    {
                        await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                        ShowNotification(PresentationAdminStrings.UrlNameEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.ExternalUrlAdd))
                    {
                        await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                        ShowNotification(PresentationAdminStrings.ExternalUrlAddEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.ExternalUrlRemove))
                    {
                        await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                        ShowNotification(PresentationAdminStrings.ExternalUrlRemoveEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.CertificateThumbprintExternal))
                    {
                        await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintExternalEmpty, NotificationType.Error);
                    }
                    if (string.IsNullOrEmpty(model.CertificateThumbprintVisa))
                    {
                        await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                        ShowNotification(PresentationAdminStrings.CertificateThumbprintVisaEmpty, NotificationType.Error);
                    }

                    return View(model);
                }
                //Validar que tenga al menos un grupo de bin seleccionado
                var binGroupCount = model.BinGroups == null ? 0 : model.BinGroups.Count(x => x != Guid.Empty);
                if (binGroupCount == 0)
                {
                    await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                    ShowNotification(PresentationAdminStrings.BinGroup_Required, NotificationType.Error);
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.ServiceContainerId) && model.ServiceContainerId != Guid.Empty.ToString())
                {
                    var serviceContainer = await _serviceClientService.Find(new Guid(model.ServiceContainerId));
                    if (serviceContainer != null)
                    {
                        //Validar que si tiene servicio contenedor, entonces los grupos de bines del hijo no sean distintos a los que permite el contenedor
                        var serviceBinGroups = model.BinGroups.Where(x => x != Guid.Empty).ToList();
                        var serviceContainerBinGroups = serviceContainer.BinGroups.Select(x => x.Id).ToList();
                        if (serviceBinGroups.Intersect(serviceContainerBinGroups).Count() != serviceBinGroups.Count())
                        {
                            await LoadViewBag(null, model.BinGroups, null, null, model);
                            ShowNotification(PresentationAdminStrings.BinGroup_Container_NotMatch, NotificationType.Error);
                            return View(model);
                        }
                    }
                }
                #endregion

                if (model.DeleteImage)
                {
                    DeleteImageAzure(model.Image, _imageFolder);
                    model.Image = null;
                }
                if (model.DeleteImageTooltip)
                {
                    DeleteImageAzure(model.ImageTooltip, _imageFolder);
                    model.ImageTooltip = null;
                }

                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !String.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBag(model.Id, model.BinGroups, null, null, model);
                            return View(model);
                        }

                        if (file.Equals("Image"))
                        {
                            model.Image = image.FileName;
                            DeleteImageAzure(model.ImageBlobName, _imageFolder);
                            CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _imageFolder);
                        }
                        if (file.Equals("ImageTooltip"))
                        {
                            model.ImageTooltip = image.FileName;
                            DeleteImageAzure(model.ImageTooltipBlobName, _imageFolder);
                            CreateImageAzure(model.ImageTooltipBlobName, image.InputStream, image.ContentType, _imageFolder);
                        }
                    }
                }

                var dto = await Convert(model);
                await _serviceClientService.Edit(dto);

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
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }

            await LoadViewBag(model.Id, model.BinGroups, null, null, model);
            return View(model);
        }

        [CustomAuthentication(Actions.ServiceList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerService(Request, param);
            filter.IsContainer = false;

            var data = await _serviceClientService.FindAll(filter);
            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Status = d.Active ? "Activo" : "Inactivo",
                StatusActive = d.Active,
                Name = d.Name,
                ServiceCategoryName = d.ServiceCategoryName,
                Description = d.Description,
                ServiceContainer = d.ServiceContainerName,
                Gateways = d.ServiceGatewaysDto.Any(x => x.Active) ? string.Join(", ", d.ServiceGatewaysDto.Where(x => x.Active).Select(x => x.Gateway.Name)) : string.Empty
            });

            if (filter.SortDirection == SortDirection.Desc)
            {
                dataModel = dataModel.OrderByDescending(p =>
                    filter.OrderBy == "0" ? p.Name :
                    filter.OrderBy == "1" ? p.ServiceCategoryName :
                    filter.OrderBy == "2" ? p.Status : "");
            }
            else
            {
                dataModel = dataModel.OrderBy(p =>
                    filter.OrderBy == "0" ? p.Name :
                    filter.OrderBy == "1" ? p.ServiceCategoryName :
                    filter.OrderBy == "2" ? p.Status : "");
            }

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
        [CustomAuthentication(Actions.ServiceDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _serviceClientService.Find(id);
                await LoadViewBag(id, dto.BinGroups.Select(x => x.Id).ToList());

                var gateways = await _gatewayClientService.FindAll();
                var model = Convert(dto, gateways, false);
                model.IsDetailsView = true;
                model.BinGroupsSelectList = await GenerateBinGroups(dto.ServiceContainerId);

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
        [CustomAuthentication(Actions.ServiceEdit)]
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

        //TEST PASARELAS
        [CustomAuthentication(Actions.ServiceTestGateways)]
        public async Task<ActionResult> LoadTestGatewaysData(Guid serviceId)
        {
            try
            {
                var service = await _serviceClientService.Find(serviceId);

                if (service.ServiceGatewaysDto.Any())
                {
                    var gateways = service.ServiceGatewaysDto.Where(x => x.Active &&
                        !String.Equals(x.Gateway.Name, "Apps", StringComparison.InvariantCultureIgnoreCase) &&
                        !String.Equals(x.Gateway.Name, "Carretera", StringComparison.InvariantCultureIgnoreCase) &&
                        !String.Equals(x.Gateway.Name, "Importe", StringComparison.InvariantCultureIgnoreCase)).ToList();

                    service.ServiceGatewaysDto = gateways;
                }

                var content = RenderPartialViewToString("_TestGatewaysLightBox", service);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null, "Error", "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthentication(Actions.ServiceTestGateways)]
        public async Task<ActionResult> TestGatewaysAjax(ServiceAssociatedDto model)
        {
            var service = await _serviceClientService.Find(model.ServiceId);

            if (service.ServiceGatewaysDto.Any())
            {
                var gateways = service.ServiceGatewaysDto.Where(x => x.Active &&
                    !String.Equals(x.Gateway.Name, "Apps", StringComparison.InvariantCultureIgnoreCase) &&
                    !String.Equals(x.Gateway.Name, "Carretera", StringComparison.InvariantCultureIgnoreCase) &&
                    !String.Equals(x.Gateway.Name, "Importe", StringComparison.InvariantCultureIgnoreCase)).ToList();

                service.ServiceGatewaysDto = gateways;
            }

            var viewModel = new List<TestGatewaysModel>();

            //Para cada pasarela pruebo obtener facturas
            foreach (var gateway in service.ServiceGatewaysDto)
            {
                var gwyResult = await GetBillsForGateway((GatewayEnumDto)gateway.Gateway.Enum, service, model);
                viewModel.Add(gwyResult);
            }

            var content = RenderPartialViewToString("_TestGatewaysResponseTable", viewModel);

            return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
        }

        //TESTS CYBERSOURCE
        [CustomAuthentication(Actions.ServiceTestCybersource)]
        public async Task<ActionResult> LoadTestCybersourceData(Guid serviceId)
        {
            try
            {
                var service = await _serviceClientService.Find(serviceId);

                var content = RenderPartialViewToString("_TestCybersourceLightBox", service);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null, "Error", "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthentication(Actions.ServiceTestCybersource)]
        public async Task<ActionResult> TestCybersourceWebApi(Guid serviceId)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "Inicio proceso de TestCybersourceWebApi para servicio " + serviceId);
                var response = await _paymentClientService.TestCyberSourcePayment(serviceId);
                var msg = PresentationAdminStrings.TestCybersource_ConnectionError;

                if (response == null || response.PaymentData == null)
                {
                    msg = PresentationAdminStrings.TestCybersource_ConnectionError + " No se pudo ejecutar el pago.";
                    return Json(new JsonResponse(AjaxResponse.Error, "", msg, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                if (response.PaymentData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                {
                    msg = PresentationAdminStrings.TestCybersource_ConnectionSuccessful + "<br/>Pago: OK . Request id: " +
                          response.PaymentData.PaymentRequestId;

                    #region void

                    if (response.VoidData != null)
                    {
                        if (response.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            msg += "<br/> Void: OK.";
                        }
                        else
                        {
                            msg += "<br/> Void con error. Codigo: " + response.VoidData.PaymentResponseCode;
                        }
                        msg += " Request Id: " + response.VoidData.PaymentRequestId;
                    }
                    else
                    {
                        msg += "<br/> Void no executado. -Error";
                    }

                    #endregion

                    #region reversal

                    if (response.ReversalData != null)
                    {
                        if (response.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            msg += "<br/> Reverso: OK.";
                        }
                        else
                        {
                            msg += "<br/> Reverso con error. Codigo: " + response.ReversalData.PaymentResponseCode;
                        }
                        msg += " Request Id: " + response.ReversalData.PaymentRequestId;
                    }
                    else
                    {
                        msg += "<br/> Reverso no executado. -Error";
                    }

                    #endregion

                    return Json(new JsonResponse(AjaxResponse.Success, "", msg, "Correcto", NotificationType.Success),
                        JsonRequestBehavior.AllowGet);
                }
                else
                {
                    msg = PresentationAdminStrings.TestCybersource_ConnectionError + "Pago con error. Codigo: " + response.PaymentData.PaymentResponseCode;
                    if (response.PaymentData.PaymentResponseCode == (int)CybersourceMsg.DecisionManager)
                    {
                        #region reversal

                        if (response.ReversalData != null)
                        {
                            if (response.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                            {
                                msg += "<br/> Reverso OK.";
                            }
                            else
                            {
                                msg += "<br/> Reverso con error. Codigo: " + response.ReversalData.PaymentResponseCode;
                            }
                            msg += " Request Id: " + response.ReversalData.PaymentRequestId;
                        }
                        else
                        {
                            msg += "<br/> Reverso no executado. -Error";
                        }

                        #endregion
                    }
                }

                return Json(new JsonResponse(AjaxResponse.Error, "", msg, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Exception generada en proceso de TestCybersourceWebApi para servicio " + serviceId);
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.TestCybersource_UnexpectedError, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthentication(Actions.ServiceTestCybersource)]
        public async Task<ActionResult> TestCybersourceReports(Guid serviceId)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "Inicio proceso de TestCybersourceReports para servicio " + serviceId);
                var connectionOk = await _paymentClientService.TestCyberSourceReports(serviceId);
                if (connectionOk)
                {
                    return Json(new JsonResponse(AjaxResponse.Success, "", PresentationAdminStrings.TestCybersource_ConnectionSuccessful, "Correcto", NotificationType.Success), JsonRequestBehavior.AllowGet);
                }
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.TestCybersource_ConnectionError, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Exception generada en proceso de TestCybersourceWebApi para servicio " + serviceId);
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.TestCybersource_UnexpectedError, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> TestCybersourceSecureAcceptance(Guid serviceId, string fpProfiler)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "Inicio proceso de TestCybersourceSecureAcceptance para servicio " + serviceId);
                var userEmailTest = ConfigurationManager.AppSettings["AppUserForTest"];

                var service = await _serviceClientService.Find(serviceId);
                var user = await _applicationUserClientService.Find(userEmailTest);
                var card = user.CardDtos.FirstOrDefault();

                var token = new KeysInfoForPaymentRegisteredUser
                {
                    FingerPrint = fpProfiler,
                    TransactionReferenceNumber = Guid.NewGuid().ToString(),
                    RedirectTo = RedirectEnums.TestCsSecureAcceptance.ToString("D"),
                    CardId = card.Id,
                    NameTh = card.Name,
                    UserId = user.Id,
                    CardBin = card.MaskedNumber.Substring(0, 6),
                    ServiceId = service.Id,
                    OperationTypeDto = OperationTypeDto.UniquePayment,
                    Platform = PaymentPlatformDto.VisaNet.ToString()
                };

                token.Bill = new BillForToken
                {
                    Currency = "UYU",
                    Amount = 1,
                    DiscountType = 0,
                    DiscountAmount = 0,
                    TaxedAmount = 0,
                    Quota = 1
                };

                var keys = await _cyberSourceAccessClientService.GenerateKeys(token);

                var cyberSourceKey = new CyberSourceKeyModel
                {
                    Keys = keys,
                    Currency = token.Bill.Currency,
                    Discount = token.Bill.DiscountAmount,
                    DiscountApplyed = token.Bill.DiscountAmount > 0,
                    TotalAmount = token.Bill.Amount,
                    TotalTaxedAmount = token.Bill.TaxedAmount,
                };

                Session[SessionConstants.PAYMENT_TEST] = token;

                await _logClientService.Put(Guid.Parse(keys["merchant_defined_data29"]), new LogDto
                {
                    LogCommunicationType = LogCommunicationType.CyberSource,
                    LogType = LogType.Info,
                    LogOperationType = LogOperationType.BillPayment,
                    ApplicationUserId = user.Id,
                    LogUserType = LogUserType.Registered,
                    CallCenterMessage = string.Format("Inicia comunicación a CS para pago de servicio {0}", service.Name),
                    Message = string.Format("Inicia comunicación a CS para pago de servicio {0}", service.Name)
                });

                var content = RenderPartialViewToString("_CybersourceKeys", cyberSourceKey.Keys);

                return Json(new JsonResponse(AjaxResponse.Success, new { keys = content }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Exception generada en proceso de TestCybersourceWebApi para servicio " + serviceId);
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.TestCybersource_UnexpectedError, "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        //Callback de TestSecureAcceptance
        public async Task<ActionResult> TestCybersourceSecureAcceptanceCallback()
        {
            CyberSourceDataDto cyberSourceData = null;
            var token = (KeysInfoForPaymentRegisteredUser)Session[SessionConstants.PAYMENT_TEST];
            ServiceDto service = null;
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "Admin/ServiceController TestCybersourceSecureAcceptanceCallback");
                service = await _serviceClientService.Find(token.ServiceId);
                cyberSourceData = (CyberSourceDataDto)TempData["CyberSourceData"];
                var verifyByVisaData = (VerifyByVisaDataDto)TempData["VerifyByVisaData"];

                #region Registro Log (pendiente)

                var mdd29 = TempData["CsMerchantDefinedData29"].ToString();
                Guid tempGuid;
                Guid? tempGuid2 = null;
                tempGuid2 = Guid.TryParse(mdd29, out tempGuid) ? tempGuid : tempGuid2;
                var user = await _applicationUserClientService.Find(token.UserId);
                var strLog = string.Format(LogStrings.PaymentTest_Loged_Init, user.Email, service.Name);

                await _logClientService.Put(new LogModel
                {
                    LogType = LogType.Info,
                    LogUserType = LogUserType.Registered,
                    LogCommunicationType = LogCommunicationType.VisaNet,
                    Message = strLog,
                    CallCenterMessage = strLog,
                    CyberSourceLogData = new CyberSourceLogDataDto
                    {
                        AuthAmount = cyberSourceData.AuthAmount,
                        AuthAvsCode = cyberSourceData.AuthAvsCode,
                        AuthCode = cyberSourceData.AuthCode,
                        AuthResponse = cyberSourceData.AuthResponse,
                        AuthTime = cyberSourceData.AuthTime,
                        AuthTransRefNo = cyberSourceData.AuthTransRefNo,
                        BillTransRefNo = cyberSourceData.BillTransRefNo,
                        Decision = cyberSourceData.Decision,
                        Message = cyberSourceData.Message,
                        PaymentToken = cyberSourceData.PaymentToken,
                        ReasonCode = cyberSourceData.ReasonCode,
                        ReqAmount = cyberSourceData.ReqAmount,
                        ReqCardExpiryDate =
                            cyberSourceData.ReqCardExpiryDate,
                        ReqCardNumber = cyberSourceData.ReqCardNumber,
                        ReqCardType = cyberSourceData.ReqCardType,
                        ReqCurrency = cyberSourceData.ReqCurrency,
                        ReqPaymentMethod =
                            cyberSourceData.ReqPaymentMethod,
                        ReqProfileId = cyberSourceData.ReqProfileId,
                        ReqReferenceNumber =
                            cyberSourceData.ReqReferenceNumber,
                        ReqTransactionType =
                            cyberSourceData.ReqTransactionType,
                        ReqTransactionUuid =
                            cyberSourceData.ReqTransactionUuid,
                        TransactionId = cyberSourceData.TransactionId,
                        TransactionType = TransactionType.Payment,
                        PaymentPlatform = PaymentPlatform.VisaNet
                    },
                    CyberSourceVerifyByVisaData = new CyberSourceVerifyByVisaDataDto
                    {
                        PayerAuthenticationXid =
                            verifyByVisaData
                            .PayerAuthenticationXid,
                        PayerAuthenticationProofXml =
                            verifyByVisaData
                            .PayerAuthenticationProofXml,
                        PayerAuthenticationCavv =
                            verifyByVisaData
                            .PayerAuthenticationCavv,
                        PayerAuthenticationEci =
                            verifyByVisaData
                            .PayerAuthenticationEci,
                    },
                    TemporaryId = tempGuid2
                });

                #endregion

                var msg = PresentationAdminStrings.TestCybersource_ConnectionError + " No se pudo ejecutar el pago.";
                var msgTitle = "ERROR";
                if (cyberSourceData != null)
                {
                    var cyberSourceOperationData = new CyberSourceOperationData()
                    {
                        PaymentData = new CsResponseData()
                        {
                            PaymentResponseCode = int.Parse(cyberSourceData.ReasonCode),
                            PaymentRequestId =
                                                                             cyberSourceData.TransactionId,
                        }
                    };

                    if (cyberSourceData.ReasonCode.Equals("100"))
                    {
                        msgTitle = "SUCCESS";
                        msg = "Pago: OK . Request id: " + cyberSourceData.TransactionId;
                        cyberSourceOperationData =
                            await
                                _paymentClientService.TestCyberSourceCancelPayment(service.Id, cyberSourceOperationData);

                        #region void

                        if (cyberSourceOperationData.VoidData != null)
                        {
                            if (cyberSourceOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                            {
                                msg += "| Void: OK.";
                            }
                            else
                            {
                                msg += "| Void con error. Codigo: " +
                                       cyberSourceOperationData.VoidData.PaymentResponseCode;
                            }
                            msg += " Request Id: " + cyberSourceOperationData.VoidData.PaymentRequestId;
                        }
                        else
                        {
                            msg += "| Void no executado. -Error";
                        }

                        #endregion

                        #region reversal

                        if (cyberSourceOperationData.ReversalData != null)
                        {
                            if (cyberSourceOperationData.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                            {
                                msg += "| Reverso: OK.";
                            }
                            else
                            {
                                msg += "| Reverso con error. Codigo: " +
                                       cyberSourceOperationData.ReversalData.PaymentResponseCode;
                            }
                            msg += " Request Id: " + cyberSourceOperationData.ReversalData.PaymentRequestId;
                        }
                        else
                        {
                            msg += "| Reverso no executado. -Error";
                        }

                        #endregion
                    }
                    else
                    {
                        msg = PresentationAdminStrings.TestCybersource_ConnectionError + "Pago con error. Codigo: " +
                              cyberSourceData.ReasonCode;
                        if (cyberSourceData.ReasonCode.Equals("481"))
                        {
                            var result = await _paymentClientService.TestCyberSourceReversal(service.Id);

                            #region reversal

                            if (result.ReversalData != null)
                            {
                                if (result.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                                {
                                    msg += "| Reverso OK.";
                                }
                                else
                                {
                                    msg += "| Reverso con error. Codigo: " + result.ReversalData.PaymentResponseCode;
                                }
                                msg += " Request Id: " + result.ReversalData.PaymentRequestId;
                            }
                            else
                            {
                                msg += "| Reverso no executado. -Error";
                            }

                            #endregion
                        }
                    }
                }
                TempData["ResponseTestSecureAcceptanceServiceName"] = service.Name;
                TempData["ResponseTestSecureAcceptance"] = msgTitle;
                TempData["ResponseTestSecureAcceptanceMessage"] = msg;
                TempData["ResponseTestSecureAcceptanceServiceId"] = service.Id.ToString();
                return View("Index");
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info,
                    "ServiceController - TestCybersourceSecureAcceptanceCallback - Exception");
                NLogLogger.LogEvent(exception);

                TempData["ResponseTestSecureAcceptanceServiceName"] = service != null ? service.Name : "";
                TempData["ResponseTestSecureAcceptance"] = "ERROR";
                TempData["ResponseTestSecureAcceptanceMessage"] = PresentationAdminStrings.TestCybersource_UnexpectedError;
                TempData["ResponseTestSecureAcceptanceServiceId"] = service != null ? service.Id.ToString() : "";
            }

            return RedirectToAction("Index");
        }

        //Cargar modal despues de callback
        [CustomAuthentication(Actions.ServiceTestCybersource)]
        public async Task<ActionResult> LoadTestCybersourceCallbackData(string serviceName, string result, string message)
        {
            try
            {
                var model = new TestCybersourceModel
                {
                    ServiceName = serviceName,
                    SuccessfulConnection = result.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase),
                    ResponseMessage = message
                };

                var content = RenderPartialViewToString("_TestCybersourceCallbackLightBox", model);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null, "Error", "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CustomAuthentication(Actions.ServiceEdit)]
        public async Task<JsonResult> GetContainerConf(Guid containerId)
        {
            var serviceContainer = await _serviceClientService.Find(containerId);
            return
                Json(new JsonResponse(AjaxResponse.Success,
                    new
                    {
                        AllowQuotas = serviceContainer.MaxQuotaAllow,
                        BinsGroup = serviceContainer.BinGroups,
                    }
                    , "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ServiceEdit)]
        public async Task<JsonResult> GetContainerConfiguration(Guid containerId, bool isCreateView)
        {
            var serviceContainer = await _serviceClientService.Find(containerId);

            //Quotas
            int? maxQuota = null;
            if (serviceContainer != null)
            {
                maxQuota = serviceContainer.MaxQuotaAllow == 0 ? 1 : serviceContainer.MaxQuotaAllow;
            }
            ViewBag.Quotas = GenerateQuotas(1, maxQuota);
            var quotasView = RenderPartialViewToString("_QuotasCombo", new ServiceModel());

            //References
            var containerModel = new ServiceModel
            {
                ContainerReferenceParamName = serviceContainer.ReferenceParamName,
                ContainerReferenceParamName2 = serviceContainer.ReferenceParamName2,
                ContainerReferenceParamName3 = serviceContainer.ReferenceParamName3,
                ContainerReferenceParamName4 = serviceContainer.ReferenceParamName4,
                ContainerReferenceParamName5 = serviceContainer.ReferenceParamName5,
                ContainerReferenceParamName6 = serviceContainer.ReferenceParamName6,
                ContainerReferenceParamRegex = serviceContainer.ReferenceParamRegex,
                ContainerReferenceParamRegex2 = serviceContainer.ReferenceParamRegex2,
                ContainerReferenceParamRegex3 = serviceContainer.ReferenceParamRegex3,
                ContainerReferenceParamRegex4 = serviceContainer.ReferenceParamRegex4,
                ContainerReferenceParamRegex5 = serviceContainer.ReferenceParamRegex5,
                ContainerReferenceParamRegex6 = serviceContainer.ReferenceParamRegex6
            };
            var referencesView = RenderPartialViewToString("_ContainerReferencesLightbox", containerModel);

            //BIN Groups
            var binGroups = await GenerateBinGroups(containerId);
            var model = new ServiceModel
            {
                IsDetailsView = false,
                BinGroupsSelectList = binGroups
            };
            var binGroupsView = RenderPartialViewToString("_BinGroupsDropDown", model);

            //Result
            var content = new
            {
                QuotasView = quotasView,
                ReferencesView = referencesView,
                ContainerHasReferences = !string.IsNullOrEmpty(serviceContainer.ReferenceParamName),
                BinGroupsView = binGroupsView,
                SelectAllBinGroups = isCreateView && containerId != Guid.Empty
            };

            return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
        }

        #region Private methods

        private async Task LoadViewBag(Guid? serviceId = null, List<Guid> binGroups = null, int? quotaSelected = null, int? maxQuota = null, ServiceModel model = null)
        {
            //Cargar siempre
            ViewBag.Categories = await GenerateCategoriesList();
            ViewBag.Departaments = GenerateDepartamentList();
            ViewBag.Discounts = GenerateDiscountTypeList();
            ViewBag.Quotas = GenerateQuotas(quotaSelected, maxQuota);
            ViewBag.IntegrationVersion = GenerateUrlIntegrationVersionList();
            ViewBag.Interpreters = await GenerateInterpretersList();

            if (model != null)
            {
                var serviceContainerId = !string.IsNullOrEmpty(model.ServiceContainerId) && model.ServiceContainerId != Guid.Empty.ToString() ? new Guid(model.ServiceContainerId) : (Guid?)null;
                model.BinGroupsSelectList = await GenerateBinGroups(serviceContainerId);
            }

            if (binGroups != null)
            {
                ViewBag.SelectedBinGroups = JsonConvert.SerializeObject(binGroups).Replace("\"", "'");
            }

            if (serviceId == null)
            {
                serviceId = Guid.Empty;
            }
            ViewBag.ServicesLigthList = await GenerateServiceList(serviceId.Value);
        }

        private List<SelectListItem> GenerateDepartamentList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(DepartamentDtoType)).Cast<DepartamentDtoType>();
            return list.Select(departamentDtoType => new SelectListItem
            {
                Text = rm.GetString(departamentDtoType.ToString()),
                Value = (int)departamentDtoType + "",
            }).ToList();
        }

        private List<SelectListItem> GenerateDiscountTypeList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(DiscountType)).Cast<DiscountType>();
            return list.Select(discountType => new SelectListItem()
            {
                Text = rm.GetString(string.Concat("DiscountType_", discountType.ToString())),
                Value = (int)discountType + "",
            }).Where(x => !x.Value.Equals("5")).ToList();
        }

        private async Task<List<SelectListItem>> GenerateInterpretersList()
        {
            var interpreters = await _interpreterClientService.FindAll();
            var list = interpreters.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name }).ToList();
            var final = new List<SelectListItem>() { new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Ninguno" } };
            final.AddRange(list);
            return final;
        }

        private async Task<List<SelectListItem>> GenerateBinGroups(Guid? serviceContainerId = null)
        {
            if (serviceContainerId == null || serviceContainerId == Guid.Empty)
            {
                var allBinGroups = await _binGroupsService.FindAll();
                allBinGroups = allBinGroups != null ? allBinGroups.OrderBy(x => x.Name).ToList() : new List<BinGroupDto>();
                return allBinGroups.Select(binGroup => new SelectListItem
                {
                    Text = binGroup.Name,
                    Value = binGroup.Id.ToString()
                }).ToList();
            }

            var serviceContainer = await _serviceClientService.Find(serviceContainerId.Value);
            var serviceContainerBinGroups = serviceContainer.BinGroups != null ? serviceContainer.BinGroups.OrderBy(x => x.Name).ToList() : new List<BinGroupDto>();
            return serviceContainerBinGroups.Select(binGroup => new SelectListItem
            {
                Text = binGroup.Name,
                Value = binGroup.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GenerateServiceList(Guid serviceId)
        {
            var services = await _serviceClientService.GetDataForList(serviceId, true);
            var aux = services.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name });
            var list = new List<SelectListItem>() { new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Ninguno" } };
            list.AddRange(aux);
            return list;
        }

        private async Task<List<SelectListItem>> GenerateCategoriesList()
        {
            var categories = await _serviceCategoryClientService.FindAll();
            var list = categories.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name }).ToList();
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

        private List<SelectListItem> GenerateQuotas(int? selected, int? maxQuota)
        {
            var list = new List<SelectListItem>();
            var quotaConf = maxQuota.HasValue ? maxQuota.Value.ToString() : ConfigurationManager.AppSettings["MaxNumberQuotas"];
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

        private bool ServiceGatewaysParamsAreValid(ICollection<ServiceGatewayModel> serviceGateways)
        {
            if (serviceGateways != null && serviceGateways.Count > 0)
            {
                foreach (var gt in serviceGateways)
                {
                    //Para la pasarla apps ServiceType y ReferenceId deben ser numeros
                    if (gt.GatewayEnum == (int)GatewayEnumDto.Apps && gt.Active)
                    {
                        int aux;
                        var condition1 = int.TryParse(gt.ServiceType, out aux);
                        var condition2 = int.TryParse(gt.ReferenceId, out aux);
                        if (!condition1 || !condition2)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private ServiceModel Convert(ServiceDto dto, ICollection<GatewayDto> gateways, bool edit)
        {
            if (dto.IntroContent != null) dto.IntroContent = dto.IntroContent.Replace(Environment.NewLine, string.Empty);
            var model = new ServiceModel()
            {
                Name = dto.Name,
                Description = dto.Description,
                DescriptionTooltip = dto.DescriptionTooltip,
                Active = dto.Active,
                LinkId = dto.LinkId,
                MerchantId = dto.MerchantId,
                CybersourceAccessKey = !edit ? dto.CybersourceAccessKey : "",
                CybersourceProfileId = dto.CybersourceProfileId,
                CybersourceSecretKey = !edit ? dto.CybersourceSecretKey : "",
                CybersourceTransactionKey = !edit ? dto.CybersourceTransactionKey : "",
                Tags = dto.Tags,
                ReferenceParamName = dto.ReferenceParamName,
                ReferenceParamName2 = dto.ReferenceParamName2,
                ReferenceParamName3 = dto.ReferenceParamName3,
                ReferenceParamName4 = dto.ReferenceParamName4,
                ReferenceParamName5 = dto.ReferenceParamName5,
                ReferenceParamName6 = dto.ReferenceParamName6,
                ReferenceParamRegex = dto.ReferenceParamRegex,
                ReferenceParamRegex2 = dto.ReferenceParamRegex2,
                ReferenceParamRegex3 = dto.ReferenceParamRegex3,
                ReferenceParamRegex4 = dto.ReferenceParamRegex4,
                ReferenceParamRegex5 = dto.ReferenceParamRegex5,
                ReferenceParamRegex6 = dto.ReferenceParamRegex6,
                ServiceCategoryId = dto.ServiceCategoryId.ToString(),
                Departament = (int)dto.Departament,
                ServiceGateways = new List<ServiceGatewayModel>(),
                ExtractEmail = dto.ExtractEmail,
                CertificateThumbprintExternal = dto.CertificateThumbprintExternal,
                EnableAutomaticPayment = dto.EnableAutomaticPayment,
                EnablePartialPayment = dto.EnablePartialPayment,
                EnableMultipleBills = dto.EnableMultipleBills,
                EnableAssociation = dto.EnableAssociation,
                EnablePrivatePayment = dto.EnablePrivatePayment,
                EnablePublicPayment = dto.EnablePublicPayment,
                PostAssociationDesc = dto.PostAssociationDesc,
                TermsAndConditions = dto.TermsAndConditions,
                UrlName = !string.IsNullOrEmpty(dto.UrlName) ? dto.UrlName.ToLower() : "",
                DiscountType = (int)dto.DiscountType,
                ServiceContainerId = dto.ServiceContainerId.ToString(),
                ExternalUrlAdd = dto.ExternalUrlAdd,
                ExternalUrlRemove = dto.ExternalUrlRemove,
                CertificateThumbprintVisa = dto.CertificateThumbprintVisa,
                AskUserForReferences = dto.AskUserForReferences,
                AllowMultipleCards = dto.AllowMultipleCards,
                MaxQuotaAllow = dto.MaxQuotaAllow,
                ContentIntro = dto.IntroContent,
                InterpreterId = dto.InterpreterId.ToString(),
                InterpreterAuxParam = dto.InterpreterAuxParam,
                BinGroups = dto.BinGroups.Select(bg => bg.Id).ToList(),
                InformCardBank = dto.InformCardBank,
                InformCardType = dto.InformCardType,
                InformAffiliationCard = dto.InformCardAffiliation,
                UrlIntegrationVersion = (int)dto.UrlIntegrationVersion,
                AllowWcfPayment = dto.AllowWcfPayment,
                AllowVon = dto.AllowVon,
                Sort = dto.Sort,
                Image = dto.ImageName,
                ImagePath = dto.ImageUrl,
                ImageTooltip = dto.ImageTooltipName,
                ImageTooltipPath = dto.ImageTooltipUrl,
                ContainerHasReferences = dto.ServiceContainerDto != null && !string.IsNullOrEmpty(dto.ServiceContainerDto.ReferenceParamName),
                ContainerReferenceParamName = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName : null,
                ContainerReferenceParamName2 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName2 : null,
                ContainerReferenceParamName3 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName3 : null,
                ContainerReferenceParamName4 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName4 : null,
                ContainerReferenceParamName5 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName5 : null,
                ContainerReferenceParamName6 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamName6 : null,
                ContainerReferenceParamRegex = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex : null,
                ContainerReferenceParamRegex2 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex2 : null,
                ContainerReferenceParamRegex3 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex3 : null,
                ContainerReferenceParamRegex4 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex4 : null,
                ContainerReferenceParamRegex5 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex5 : null,
                ContainerReferenceParamRegex6 = dto.ServiceContainerDto != null ? dto.ServiceContainerDto.ReferenceParamRegex6 : null,
            };

            //Gateways
            if (dto.ServiceGatewaysDto != null && dto.ServiceGatewaysDto.Count > 0)
            {
                foreach (var gt in gateways)
                {
                    var gActive = dto.ServiceGatewaysDto.FirstOrDefault(x => x.GatewayId == gt.Id);
                    model.ServiceGateways.Add(new ServiceGatewayModel()
                    {
                        Id = gActive != null ? gActive.Id : new Guid(),
                        GatewayName = gt.Name,
                        Active = gActive != null && gActive.Active,
                        SendExtract = gActive != null && gActive.SendExtract,
                        GatewayId = gt.Id,
                        ReferenceId = gActive != null ? gActive.ReferenceId : "",
                        ServiceType = gActive != null ? gActive.ServiceType : "",
                        GatewayEnum = gt.Enum,
                        AuxiliarData = gActive != null ? gActive.AuxiliarData : "",
                        AuxiliarData2 = gActive != null ? gActive.AuxiliarData2 : "",
                    });
                }

            }
            else
            {
                foreach (var gt in gateways)
                {
                    model.ServiceGateways.Add(new ServiceGatewayModel()
                    {
                        Active = false,
                        GatewayId = gt.Id,
                        GatewayName = gt.Name,
                        GatewayEnum = gt.Enum,
                    });
                }
            }

            //Highway Emails
            if (dto.HighwayEnableEmails != null && dto.HighwayEnableEmails.Count > 0)
            {
                model.ServiceEnableEmailModel = new List<ServiceEnableEmailModel>();
                foreach (var gt in dto.HighwayEnableEmails)
                {
                    model.ServiceEnableEmailModel.Add(new ServiceEnableEmailModel()
                    {
                        Id = gt.Id,
                        Email = gt.Email
                    });
                }
            }

            return model;
        }

        private async Task<ServiceDto> Convert(ServiceModel model)
        {
            var introcontent = !string.IsNullOrEmpty(model.ContentIntro)
                ? model.ContentIntro.Replace("\"", "'")
                : string.Empty;
            var dto = new ServiceDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                DescriptionTooltip = model.DescriptionTooltip,
                Active = model.Active,
                LinkId = model.LinkId,
                MerchantId = model.MerchantId == null ? null : model.MerchantId.Trim(),
                CybersourceAccessKey = model.CybersourceAccessKey == null ? null : model.CybersourceAccessKey.Trim(),
                CybersourceProfileId = model.CybersourceProfileId == null ? null : model.CybersourceProfileId.Trim(),
                CybersourceSecretKey = model.CybersourceSecretKey == null ? null : model.CybersourceSecretKey.Trim(),
                CybersourceTransactionKey = model.CybersourceTransactionKey == null ? null : model.CybersourceTransactionKey.Trim(),
                Tags = model.Tags,
                ReferenceParamName = model.ReferenceParamName,
                ReferenceParamName2 = model.ReferenceParamName2,
                ReferenceParamName3 = model.ReferenceParamName3,
                ReferenceParamName4 = model.ReferenceParamName4,
                ReferenceParamName5 = model.ReferenceParamName5,
                ReferenceParamName6 = model.ReferenceParamName6,
                ReferenceParamRegex = model.ReferenceParamRegex,
                ReferenceParamRegex2 = model.ReferenceParamRegex2,
                ReferenceParamRegex3 = model.ReferenceParamRegex3,
                ReferenceParamRegex4 = model.ReferenceParamRegex4,
                ReferenceParamRegex5 = model.ReferenceParamRegex5,
                ReferenceParamRegex6 = model.ReferenceParamRegex6,
                ServiceCategoryId = new Guid(model.ServiceCategoryId),
                ImageDeleted = model.DeleteImage,
                ImageTooltipDeleted = model.DeleteImageTooltip,
                Departament = (DepartamentDtoType)model.Departament,
                ExtractEmail = model.ExtractEmail,
                CertificateThumbprintExternal = model.CertificateThumbprintExternal,
                PostAssociationDesc = model.PostAssociationDesc,
                TermsAndConditions = model.TermsAndConditions,
                DiscountType = (DiscountTypeDto)model.DiscountType,
                UrlName = string.IsNullOrEmpty(model.UrlName) ? string.Empty : model.UrlName.ToLower(),
                ExternalUrlAdd = model.ExternalUrlAdd,
                ExternalUrlRemove = model.ExternalUrlRemove,
                CertificateThumbprintVisa = model.CertificateThumbprintVisa,
                EnableAutomaticPayment = model.EnableAutomaticPayment,
                EnablePartialPayment = model.EnablePartialPayment,
                EnableMultipleBills = model.EnableMultipleBills,
                EnableAssociation = model.EnableAssociation,
                EnablePrivatePayment = model.EnablePrivatePayment,
                EnablePublicPayment = model.EnablePublicPayment,
                AskUserForReferences = model.AskUserForReferences,
                AllowMultipleCards = model.AllowMultipleCards,
                InterpreterId = string.IsNullOrEmpty(model.InterpreterId) ? Guid.Empty : Guid.Parse(model.InterpreterId),
                InterpreterAuxParam = model.InterpreterAuxParam,
                MaxQuotaAllow = model.MaxQuotaAllow,
                IntroContent = introcontent,
                BinGroups = model.BinGroups.Select(bg => new BinGroupDto { Id = bg }).ToList(),
                InformCardBank = model.InformCardBank,
                InformCardType = model.InformCardType,
                InformCardAffiliation = model.InformAffiliationCard,
                UrlIntegrationVersion = (UrlIntegrationVersionEnumDto)model.UrlIntegrationVersion,
                AllowWcfPayment = model.AllowWcfPayment,
                AllowVon = model.AllowVon,
                Sort = model.Sort,
                ImageName = model.Image,
                ImageTooltipName = model.ImageTooltip
            };

            var aux = new Guid(model.ServiceContainerId);
            if (aux != Guid.Empty)
            {
                dto.ServiceContainerId = new Guid(model.ServiceContainerId);

                //TODO: Agregar controles de servicio contenedor
                //Si el servicio contenedor tiene una cuota mayor a la seleccionada, se setea al hijo la cuota del contenedor
                var serviceContainer = await _serviceClientService.Find((Guid)dto.ServiceContainerId);
                dto.MaxQuotaAllow = model.MaxQuotaAllow > serviceContainer.MaxQuotaAllow ? serviceContainer.MaxQuotaAllow : model.MaxQuotaAllow;
                if (dto.MaxQuotaAllow == 0)
                {
                    dto.MaxQuotaAllow = 1;
                }
            }

            if (model.ServiceGateways != null && model.ServiceGateways.Count > 0)
            {
                dto.ServiceGatewaysDto = new List<ServiceGatewayDto>();
                foreach (var gt in model.ServiceGateways)
                {
                    dto.ServiceGatewaysDto.Add(new ServiceGatewayDto()
                    {
                        Active = gt.Active,
                        GatewayId = gt.GatewayId,
                        ServiceType = gt.ServiceType,
                        ReferenceId = gt.ReferenceId,
                        Id = gt.Id,
                        SendExtract = gt.SendExtract,
                        AuxiliarData = gt.AuxiliarData,
                        AuxiliarData2 = gt.AuxiliarData2
                    });
                }
            }
            if (model.ServiceEnableEmailModel != null && model.ServiceEnableEmailModel.Count > 0)
            {
                dto.HighwayEnableEmails = new List<ServiceEnableEmailDto>();
                foreach (var gt in model.ServiceEnableEmailModel)
                {
                    dto.HighwayEnableEmails.Add(new ServiceEnableEmailDto()
                    {
                        Email = gt.Email,
                        Id = gt.Id
                    });
                }
            }
            return dto;
        }

        private static bool IsImage(HttpPostedFileBase file)
        {
            var formats = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return file.ContentType.Contains("image") && formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsSizeCorrect(HttpPostedFileBase file)
        {
            var byteCount = file.ContentLength;
            return byteCount < 5242880; //5MB
        }

        private async Task<TestGatewaysModel> GetBillsForGateway(GatewayEnumDto gatewayEnum, ServiceDto service, ServiceAssociatedDto refs)
        {
            var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)gatewayEnum);

            var result = new TestGatewaysModel
            {
                GatewayName = gateway.Gateway.Name,
                Bills = new List<BillDto>(),
                SuccessfulConnection = false
            };

            var filterDto = new TestGatewaysFilterDto
            {
                ServiceId = service.Id,
                GatewayDto = gateway,
                References = new[] { refs.ReferenceNumber, refs.ReferenceNumber2, refs.ReferenceNumber3,
                    refs.ReferenceNumber4, refs.ReferenceNumber5, refs.ReferenceNumber6 }
            };

            try
            {
                var bills = await _billClientService.TestGatewayGetBills(filterDto);

                foreach (var bill in bills)
                {
                    result.Bills.Add(bill);
                }
                result.SuccessfulConnection = true;
            }
            catch (WebApiClientBillBusinessException e)
            {
                result.SuccessfulConnection = true;
                result.ErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
            }

            return result;
        }

        #endregion Private methods

    }
}