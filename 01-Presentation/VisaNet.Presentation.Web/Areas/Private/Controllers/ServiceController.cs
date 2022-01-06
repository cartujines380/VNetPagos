using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Mappers;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class ServiceController : BaseController
    {
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebLogClientService _logClientService;
        private readonly IWebBinClientService _binClientService;
        private readonly IWebCardClientService _webCardClientService;
        private readonly IWebPaymentClientService _webPaymentClientService;
        private readonly IWebBillClientService _webBillClientService;
        private readonly IWebLocationClientService _webLocationClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;
        private readonly IWebParameterClientService _webParameterClientService;

        public ServiceController(IWebServiceClientService serviceClientService, IWebApplicationUserClientService aplApplicationUserClientService,
            IWebServiceAssosiateClientService webServiceAssosiateClientService, IWebLogClientService logClientService, IWebBinClientService binClientService,
            IWebCardClientService webCardClientService, IWebPaymentClientService webPaymentClientService, IWebBillClientService webBillClientService,
            IWebLocationClientService webLocationClientService, IWebCyberSourceAccessClientService webCyberSourceAccessClientService,
            IWebParameterClientService webParameterClientService)
        {
            _webServiceClientService = serviceClientService;
            _webApplicationUserClientService = aplApplicationUserClientService;
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _logClientService = logClientService;
            _binClientService = binClientService;
            _webCardClientService = webCardClientService;
            _webPaymentClientService = webPaymentClientService;
            _webBillClientService = webBillClientService;
            _webLocationClientService = webLocationClientService;
            _webCyberSourceAccessClientService = webCyberSourceAccessClientService;
            _webParameterClientService = webParameterClientService;
        }

        public ActionResult NewAssociate()
        {
            CleanSession();
            return RedirectToAction("Associate");
        }

        public async Task<ActionResult> Associate()
        {
            try
            {
                var services = await _webServiceClientService.GetServicesEnableAssociation();
                var hasModel = Session[SessionConstants.SERVICES_ASSOSIATION] != null
                    ? (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION]
                    : new ServiceAssociateModel()
                    {
                        Departaments = GenerateDepartamentList(),
                        DisableEdition = false,
                    };

                hasModel.Services =
                    services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags })
                        .ToList()
                        .OrderBy(s => s.Name);

                if (Session[SessionConstants.SERVICES_ASSOSIATION] != null)
                {
                    //contenedor
                    ServiceDto firstCombo = null;
                    List<ServiceDto> list = null;
                    var service = await _webServiceClientService.Find(hasModel.ServiceToPayId);
                    if (service.ServiceContainerId.HasValue)
                    {
                        firstCombo = await _webServiceClientService.Find(service.ServiceContainerId.Value);
                        list = await _webServiceClientService.FindAll(new ServiceFilterDto()
                        {
                            ServiceContainerId =
                                service.ServiceContainerId.ToString(),
                        }) as List<ServiceDto>;

                    }
                    else
                    {
                        firstCombo = service;
                    }

                    hasModel.ServiceFirstComboId = firstCombo != null ? firstCombo.Id : Guid.Empty;
                    hasModel.ServiceSecondcomboId = service.ServiceContainerId.HasValue ? service.Id : Guid.Empty;
                    hasModel.ServicesInContainer = list;
                }

                return View("Associate", hasModel);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");

        }

        public ActionResult Index()
        {
            return View("Index", new ServiceFilterAssosiateDto());
        }

        public ActionResult Service(int payment)
        {
            return View("Index", new ServiceFilterAssosiateDto()
            {
                WithAutomaticPaymentsInt = payment
            });
        }

        public async Task<ActionResult> GetService(ServiceAssociateModel model)
        {
            ModelState.Clear();
            try
            {
                ServiceDto serviceToUse = null;
                var askForReferences = false;

                if (model.ServiceFirstComboId != Guid.Empty)
                {
                    ServiceDto firstCombo = await _webServiceClientService.Find(model.ServiceFirstComboId);
                    ServiceDto secondCombo = null;
                    askForReferences = firstCombo.AskUserForReferences;

                    if (model.ServiceSecondcomboId != Guid.Empty)
                    {
                        secondCombo = await _webServiceClientService.Find(model.ServiceSecondcomboId);
                        if (secondCombo.ServiceContainerId == model.ServiceFirstComboId)
                        {
                            serviceToUse = secondCombo;
                            askForReferences = askForReferences || secondCombo.AskUserForReferences;
                        }
                        else
                        {
                            serviceToUse = firstCombo;
                            model.ServiceSecondcomboId = Guid.Empty;
                        }
                    }
                    else
                    {
                        serviceToUse = firstCombo;
                    }

                    if (serviceToUse != null)
                    {
                        var service = serviceToUse;
                        model.AskReferences = askForReferences;

                        //Si permite seleccionar contenedor ya marco el primero
                        if (service.Container && service.AllowSelectContentAssociation)
                        {
                            model.ServiceSecondcomboId = Guid.Empty;
                            var servicesAux = await _webServiceClientService.FindAll(new ServiceFilterDto()
                            {
                                ServiceContainerId =
                                    service.Id.ToString()
                            });
                            if (servicesAux != null)
                                model.ServicesInContainer = servicesAux.OrderBy(x => x.Name).ToList();

                            if (model.ServicesInContainer != null && model.ServicesInContainer.Any())
                            {
                                model.ServiceToPay = model.ServicesInContainer.First();
                                model.ServiceToPayId = model.ServiceToPay.Id;
                            }
                        }
                        else
                        {
                            model.ServiceToPay = service;
                        }
                        model.ServiceToPayId = model.ServiceToPay.Id;
                        model.ServiceName = model.ServiceToPay.Name;

                        LoadServiceModelReferences(model, firstCombo, secondCombo);

                        model.TooltipeImage = !string.IsNullOrEmpty(model.ServiceToPay.ImageTooltipUrl) ? model.ServiceToPay.ImageTooltipUrl : string.Empty;
                        model.TooltipeDesc = model.ServiceToPay.DescriptionTooltip;

                        model.Departaments = GenerateDepartamentList();
                        model.Sucive = model.ServiceToPay.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive && s.Active);

                        model.LocationsCiu = await LoadLocation(service);
                        model.ServiceType = ServiceType(service);

                        var newcombo = "";
                        var refcontent = "";
                        if (service.Container)
                        {
                            if (service.AllowSelectContentPayment)
                            {
                                model.AskChildService = true;
                                newcombo = RenderPartialViewToString("_ServicesInContainer", model);
                                refcontent = "clean";
                            }
                            else
                            {
                                refcontent = RenderPartialViewToString("_References", model);
                                newcombo = "clean";
                            }
                        }
                        else
                        {
                            refcontent = RenderPartialViewToString("_References", model);
                            if (model.ServiceSecondcomboId == Guid.Empty)
                            {
                                newcombo = "clean";
                            }
                        }
                        var obj = new { newcombo, refcontent };
                        return Json(new JsonResponse(AjaxResponse.Success, obj, "", ""), JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogEvent(ex);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogEvent(ex);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error); ShowNotification(ex.Message, NotificationType.Error);
            }
            return Json(new JsonResponse(AjaxResponse.Error, "", "", ""), JsonRequestBehavior.AllowGet);
        }

        private void LoadServiceModelReferences(ServiceAssociateModel model, ServiceDto firstComboService, ServiceDto secondComboService)
        {
            var askForReferences = firstComboService.AskUserForReferences;
            if (askForReferences == false && secondComboService != null)
            {
                askForReferences = secondComboService.AskUserForReferences;
            }

            if (askForReferences == true)
            {
                var takeSecondServiceRefs = secondComboService != null && !string.IsNullOrEmpty(secondComboService.ReferenceParamName);

                model.ReferenceName = takeSecondServiceRefs ? secondComboService.ReferenceParamName : firstComboService.ReferenceParamName;
                model.ReferenceName2 = takeSecondServiceRefs ? secondComboService.ReferenceParamName2 : firstComboService.ReferenceParamName2;
                model.ReferenceName3 = takeSecondServiceRefs ? secondComboService.ReferenceParamName3 : firstComboService.ReferenceParamName3;
                model.ReferenceName4 = takeSecondServiceRefs ? secondComboService.ReferenceParamName4 : firstComboService.ReferenceParamName4;
                model.ReferenceName5 = takeSecondServiceRefs ? secondComboService.ReferenceParamName5 : firstComboService.ReferenceParamName5;
                model.ReferenceName6 = takeSecondServiceRefs ? secondComboService.ReferenceParamName6 : firstComboService.ReferenceParamName6;

                model.ReferenceRegex = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex : firstComboService.ReferenceParamRegex;
                model.ReferenceRegex2 = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex2 : firstComboService.ReferenceParamRegex2;
                model.ReferenceRegex3 = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex3 : firstComboService.ReferenceParamRegex3;
                model.ReferenceRegex4 = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex4 : firstComboService.ReferenceParamRegex4;
                model.ReferenceRegex5 = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex5 : firstComboService.ReferenceParamRegex5;
                model.ReferenceRegex6 = takeSecondServiceRefs ? secondComboService.ReferenceParamRegex6 : firstComboService.ReferenceParamRegex6;
            }
        }

        #region Agregar Servicio

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StepNotification(ServiceAssociateModel model)
        {
            ServiceDto service = null;
            try
            {
                if (model.AskChildService)
                {
                    if (model.ServiceSecondcomboId == Guid.Empty)
                        throw new WebApiClientBillBusinessException("Debe seleccionar un servicio para poder continuar con el proceso de pago");
                }
                CheckReferences(model);
                if (ModelState.IsValid)
                {
                    var serviceDto = await _webServiceClientService.Find(model.ServiceToPayId);

                    model.ReferenceValue = model.ReferenceValue != null ? model.ReferenceValue.ToLower() : null;
                    model.ReferenceValue2 = model.ReferenceValue2 != null ? model.ReferenceValue2.ToLower() : null;
                    model.ReferenceValue3 = model.ReferenceValue3 != null ? model.ReferenceValue3.ToLower() : null;
                    model.ReferenceValue4 = model.ReferenceValue4 != null ? model.ReferenceValue4.ToLower() : null;
                    model.ReferenceValue5 = model.ReferenceValue5 != null ? model.ReferenceValue5.ToLower() : null;
                    model.ReferenceValue6 = model.ReferenceValue6 != null ? model.ReferenceValue6.ToLower() : null;

                    model.ReferenceName = serviceDto.ReferenceParamName;
                    model.ReferenceName2 = serviceDto.ReferenceParamName2;
                    model.ReferenceName3 = serviceDto.ReferenceParamName3;
                    model.ReferenceName4 = serviceDto.ReferenceParamName4;
                    model.ReferenceName5 = serviceDto.ReferenceParamName5;
                    model.ReferenceName6 = serviceDto.ReferenceParamName6;
                    model.ReferenceRegex = serviceDto.ReferenceParamRegex;
                    model.ReferenceRegex2 = serviceDto.ReferenceParamRegex2;
                    model.ReferenceRegex3 = serviceDto.ReferenceParamRegex3;
                    model.ReferenceRegex4 = serviceDto.ReferenceParamRegex4;
                    model.ReferenceRegex5 = serviceDto.ReferenceParamRegex5;
                    model.ReferenceRegex6 = serviceDto.ReferenceParamRegex6;
                    model.DisableEdition = false;
                    #region TrimValues
                    model.TrimAllStringsProperties();
                    #endregion

                    //Si es app, no me importa el nro de referencia
                    var refs = new string[6];
                    if (serviceDto.AskUserForReferences)
                    {
                        refs = new string[]
                               {
                                   model.ReferenceValue, model.ReferenceValue2,
                                   model.ReferenceValue3, model.ReferenceValue4,
                                   model.ReferenceValue5, model.ReferenceValue6
                               };
                    }
                    var currentUser = await CurrentSelectedUser();

                    var serviceAssosiatedId =
                        await
                            _webServiceAssosiateClientService.IsServiceAssosiatedToUser(currentUser.Id, model.ServiceToPayId,
                                refs);

                    var error = false;

                    if (serviceAssosiatedId != Guid.Empty)
                    {
                        //servicio ya asociado con Active = true
                        ShowNotification("Servicio ya asociado", NotificationType.Error);
                        error = true;
                    }

                    if (serviceDto.AskUserForReferences && ValidateReferences(model))
                    {
                        ShowNotification("Faltan parametros", NotificationType.Error);
                        error = true;
                    }

                    var isSucive = serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Sucive || s.Gateway.Enum == (int)GatewayEnum.Geocom) && s.Active);
                    var isApp = serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Apps) && s.Active);

                    var fullNotifications = serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Banred && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active) ||
                        (s.Gateway.Enum == (int)GatewayEnum.Sistarbanc && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Sucive && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Carretera && s.Active));

                    var result = isApp ? 1 : await CheckAccount(serviceDto, model.ReferenceValue, model.ReferenceValue2, model.ReferenceValue3, model.ReferenceValue4, model.ReferenceValue5, model.ReferenceValue6);

                    if (result < 1)
                    {
                        ShowNotification("No se pudo confirmar la cuenta. Chequear los datos e intente nuevamente", NotificationType.Error);
                        error = true;
                    }
                    if (!error)
                    {
                        model.Sucive = isSucive;
                        model.Departaments = GenerateDepartamentList();

                        if (model.Sucive)
                        {
                            model.IdPadron = result;
                            model.ReferenceValue6 = result.ToString();
                        }

                        var notiModel = new ServiceAssociateNotificationModel()
                        {
                            DaysBeforeDueDateConfigEmail = fullNotifications,
                            DaysBeforeDueDateConfigSms = fullNotifications,
                            DaysBeforeDueDateConfigWeb = fullNotifications,
                            DaysBeforeDueDateConfigActive = fullNotifications,

                            ExpiredBillEmail = fullNotifications,
                            ExpiredBillSms = fullNotifications,
                            ExpiredBillWeb = fullNotifications,
                            ExpiredBillActive = fullNotifications,

                            NewBillEmail = fullNotifications,
                            NewBillSms = fullNotifications,
                            NewBillWeb = fullNotifications,
                            NewBillActive = fullNotifications,

                            SuccessPaymentEmail = true,
                            SuccessPaymentSms = true,
                            SuccessPaymentWeb = true,
                            SuccessPaymentActive = true,

                            FailedAutomaticPaymentEmail = fullNotifications,
                            FailedAutomaticPaymentSms = fullNotifications,
                            FailedAutomaticPaymentWeb = fullNotifications,
                            FailedAutomaticPaymentActive = fullNotifications,

                            ServiceName = model.ServiceName,
                            ServiceImageUrl = model.TooltipeImage,
                            TooltipeDesc = serviceDto.DescriptionTooltip,
                        };

                        if (serviceDto.AskUserForReferences)
                        {
                            notiModel.ReferenceName = serviceDto.ReferenceParamName;
                            notiModel.ReferenceName2 = serviceDto.ReferenceParamName2;
                            notiModel.ReferenceName3 = serviceDto.ReferenceParamName3;
                            notiModel.ReferenceName4 = serviceDto.ReferenceParamName4;
                            notiModel.ReferenceName5 = serviceDto.ReferenceParamName5;
                            notiModel.ReferenceName6 = serviceDto.ReferenceParamName6;
                            notiModel.ReferenceValue = model.ReferenceValue;
                            notiModel.ReferenceValue2 = model.ReferenceValue2;
                            notiModel.ReferenceValue3 = model.ReferenceValue3;
                            notiModel.ReferenceValue4 = model.ReferenceValue4;
                            notiModel.ReferenceValue5 = model.ReferenceValue5;
                            notiModel.ReferenceValue6 = model.ReferenceValue6;
                            notiModel.ReferenceRegex = serviceDto.ReferenceParamRegex;
                            notiModel.ReferenceRegex2 = serviceDto.ReferenceParamRegex2;
                            notiModel.ReferenceRegex3 = serviceDto.ReferenceParamRegex3;
                            notiModel.ReferenceRegex4 = serviceDto.ReferenceParamRegex4;
                            notiModel.ReferenceRegex5 = serviceDto.ReferenceParamRegex5;
                            notiModel.ReferenceRegex6 = serviceDto.ReferenceParamRegex6;
                        }

                        model.NotificationConfig = notiModel;
                        model.ServiceToPay = serviceDto;
                        Session[SessionConstants.SERVICES_ASSOSIATION] = model;
                        return View("Associate_Step_Notification", notiModel);
                    }
                }

            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }
            catch (WebApiClientBillBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }
            var services = await _webServiceClientService.GetServicesEnableAssociation();
            model.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);
            var deptos = GenerateDepartamentList();
            model.Departaments = deptos;

            if (model.ServiceToPayId != Guid.Empty)
            {
                service = await _webServiceClientService.Find(model.ServiceToPayId);
                model.Sucive = service.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive || s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active);
                //ViewBag.CreditCard = service.CreditCard;
                //ViewBag.DebitCard = service.DebitCard;
                //ViewBag.CreditCardInternational = service.CreditCardInternational;
                //ViewBag.DebitCardInternational = service.DebitCardInternational;
                model.ServiceType = ServiceType(service);
                model.LocationsCiu = await LoadLocation(service);
            }
            return View("Associate", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StepCard(ServiceAssociateNotificationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelo = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
                    modelo.NotificationConfig = model;
                    Session[SessionConstants.SERVICES_ASSOSIATION] = modelo;

                    var paramsObj = await _webParameterClientService.Get();
                    var merchantId = paramsObj.MerchantId;

                    var user = await CurrentSelectedUser();
                    var cards = await _webServiceClientService.GetEnableCards(user.Id, modelo.ServiceToPayId);

                    if (cards != null && cards.Any())
                    {
                        ViewBag.Cards = cards.Where(c => c.Active).ToList();
                    }

                    return View("Associate_Step_Card", new ServiceAssociateCardModel()
                    {
                        ServiceName = model.ServiceName,
                        ReferenceName = modelo.ReferenceName,
                        ReferenceName2 = modelo.ReferenceName2,
                        ReferenceName3 = modelo.ReferenceName3,
                        ReferenceName4 = modelo.ReferenceName4,
                        ReferenceName5 = modelo.ReferenceName5,
                        ReferenceName6 = modelo.ReferenceName6,
                        ReferenceValue = modelo.ReferenceValue,
                        ReferenceValue2 = modelo.ReferenceValue2,
                        ReferenceValue3 = modelo.ReferenceValue3,
                        ReferenceValue4 = modelo.ReferenceValue4,
                        ReferenceValue5 = modelo.ReferenceValue5,
                        ReferenceValue6 = modelo.ReferenceValue6,
                        ServiceImageUrl = model.ServiceImageUrl,
                        MerchantId = merchantId,
                    });
                }
            }

            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }

            return View("Associate_Step_Notification", model);

        }

        public ActionResult StepNotification()
        {
            var model = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
            var notiModel = new ServiceAssociateNotificationModel()
            {
                DaysBeforeDueDate = model.NotificationConfig.DaysBeforeDueDate,
                DaysBeforeDueDateConfigEmail = model.NotificationConfig.DaysBeforeDueDateConfigEmail,
                DaysBeforeDueDateConfigSms = model.NotificationConfig.DaysBeforeDueDateConfigSms,
                DaysBeforeDueDateConfigWeb = model.NotificationConfig.DaysBeforeDueDateConfigWeb,
                DaysBeforeDueDateConfigActive = model.NotificationConfig.DaysBeforeDueDateConfigActive,
                SuccessPaymentEmail = model.NotificationConfig.SuccessPaymentEmail,
                SuccessPaymentSms = model.NotificationConfig.SuccessPaymentSms,
                SuccessPaymentWeb = model.NotificationConfig.SuccessPaymentWeb,
                SuccessPaymentActive = model.NotificationConfig.SuccessPaymentActive,
                FailedAutomaticPaymentEmail = model.NotificationConfig.FailedAutomaticPaymentEmail,
                FailedAutomaticPaymentSms = model.NotificationConfig.FailedAutomaticPaymentSms,
                FailedAutomaticPaymentWeb = model.NotificationConfig.FailedAutomaticPaymentWeb,
                FailedAutomaticPaymentActive = model.NotificationConfig.FailedAutomaticPaymentActive,
                NewBillEmail = model.NotificationConfig.NewBillEmail,
                NewBillSms = model.NotificationConfig.NewBillSms,
                NewBillWeb = model.NotificationConfig.NewBillWeb,
                NewBillActive = model.NotificationConfig.NewBillActive,
                ExpiredBillEmail = model.NotificationConfig.ExpiredBillEmail,
                ExpiredBillSms = model.NotificationConfig.ExpiredBillSms,
                ExpiredBillWeb = model.NotificationConfig.ExpiredBillWeb,
                ExpiredBillActive = model.NotificationConfig.ExpiredBillActive,
                ServiceName = model.ServiceName,
                //ReferenceName = model.ServiceType.Equals(GatewayEnum.Apps.ToString()) ? "" : model.ReferenceName,
                ReferenceName = model.ReferenceName,
                ReferenceName2 = model.ReferenceName2,
                ReferenceName3 = model.ReferenceName3,
                ReferenceName4 = model.ReferenceName4,
                ReferenceName5 = model.ReferenceName5,
                ReferenceName6 = model.ReferenceName6,
                //ReferenceValue = model.ServiceType.Equals(GatewayEnum.Apps.ToString()) ? "" : model.ReferenceValue,
                ReferenceValue = model.ReferenceValue,
                ReferenceValue2 = model.ReferenceValue2,
                ReferenceValue3 = model.ReferenceValue3,
                ReferenceValue4 = model.ReferenceValue4,
                ReferenceValue5 = model.ReferenceValue5,
                ReferenceValue6 = model.ReferenceValue6,
                ServiceImageUrl = model.TooltipeImage,
                ReferenceRegex = model.ReferenceRegex,
                ReferenceRegex2 = model.ReferenceRegex2,
                ReferenceRegex3 = model.ReferenceRegex3,
                ReferenceRegex4 = model.ReferenceRegex4,
                ReferenceRegex5 = model.ReferenceRegex5,
                ReferenceRegex6 = model.ReferenceRegex6,

                Sucive = model.Sucive,

            };
            return View("Associate_Step_Notification", notiModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServiceWithToken(Guid card)
        {
            ServiceAssociateModel model = null;
            ApplicationUserDto user;
            try
            {
                model = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];

                if (card == Guid.Empty)
                {
                    ShowNotification("Debe seleccionar una tarjeta", NotificationType.Info);

                    return View("Associate_Step_Card", new ServiceAssociateCardModel()
                    {
                        ServiceName = model.ServiceName,
                        ReferenceName = model.ServiceToPay.AskUserForReferences ? model.ReferenceName : string.Empty,
                        ReferenceName2 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName2 : string.Empty,
                        ReferenceName3 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName3 : string.Empty,
                        ReferenceName4 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName4 : string.Empty,
                        ReferenceName5 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName5 : string.Empty,
                        ReferenceName6 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName6 : string.Empty,
                        ReferenceValue = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue : string.Empty,
                        ReferenceValue2 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue2 : string.Empty,
                        ReferenceValue3 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue3 : string.Empty,
                        ReferenceValue4 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue4 : string.Empty,
                        ReferenceValue5 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue5 : string.Empty,
                        ReferenceValue6 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue6 : string.Empty,
                        Sucive = model.Sucive
                    });
                }
                user = await CurrentSelectedUser();
                var dto = new ServiceAssociatedDto()
                {
                    UserId = user.Id,
                    ServiceId = model.ServiceToPayId,
                    DefaultCardId = card,
                    Description = model.Description,
                    ReferenceNumber = model.ReferenceValue,
                    ReferenceNumber2 = model.ReferenceValue2,
                    ReferenceNumber3 = model.ReferenceValue3,
                    ReferenceNumber4 = model.ReferenceValue4,
                    ReferenceNumber5 = model.ReferenceValue5,
                    ReferenceNumber6 = model.ReferenceValue6,
                    Enabled = true,
                    Active = true,
                };

                if (model.NotificationConfig != null)
                {
                    dto.NotificationConfigDto = ToNotificationConfigDto(model.NotificationConfig);
                }

                var result = await _webServiceAssosiateClientService.AssociateServiceToUserFromCardCreated(dto);

                if (result != null)
                {
                    if (result.ServiceDto.EnableAutomaticPayment)
                    {
                        return View("SuccessfulAssosiation", new SuccessfulAssosiationModel()
                        {
                            ServiceName = model.ServiceName,
                            ServiceId = result.Id,
                            CardMask = result.DefaultCard.MaskedNumber,
                            CardId = result.DefaultCardId,
                            DayBeforeExpiration = result.AutomaticPaymentDto != null ? result.AutomaticPaymentDto.DaysBeforeDueDate : 0,
                            MaxAmount = result.AutomaticPaymentDto != null ? result.AutomaticPaymentDto.Maximum : 0,
                            MaxCountPayments = result.AutomaticPaymentDto != null ? result.AutomaticPaymentDto.Quotas : 0,
                            QuotasDone = result.AutomaticPaymentDto != null ? result.AutomaticPaymentDto.QuotasDone : 0,
                            UnlimitedQuotas = result.AutomaticPaymentDto == null || result.AutomaticPaymentDto.UnlimitedQuotas,
                            UnlimitedAmount = result.AutomaticPaymentDto == null || result.AutomaticPaymentDto.UnlimitedAmount,
                            Sucive = model.Sucive,
                            EnableAutomaticPayment = true
                        });
                    }
                    return View("SuccessfulAssosiation", new SuccessfulAssosiationModel()
                    {
                        ServiceName = model.ServiceName,
                        CardId = dto.DefaultCardId,
                        EnableAutomaticPayment = false
                    });
                }

            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Info);
                NLogLogger.LogEvent(NLogType.Error, ex.Message);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
                NLogLogger.LogEvent(NLogType.Error, ex.Message);
            }
            ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            if (model == null)
            {
                return Index();
            }
            user = await CurrentSelectedUser();
            var cards = await _webServiceClientService.GetEnableCards(user.Id, model.ServiceToPayId);

            if (cards != null && cards.Any())
            {
                var today = DateTime.Now.Date;
                ViewBag.Cards = cards.Where(c => c.Active && ((c.DueDate.Year.CompareTo(today.Year) == 0 && c.DueDate.Month.CompareTo(today.Month) >= 0) ||
                        c.DueDate.Year.CompareTo(today.Year) > 0)).ToList();
            }
            return View("Associate_Step_Card", new ServiceAssociateCardModel()
            {
                ServiceName = model.ServiceName,
                ReferenceName = model.ServiceToPay.AskUserForReferences ? model.ReferenceName : string.Empty,
                ReferenceName2 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName2 : string.Empty,
                ReferenceName3 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName3 : string.Empty,
                ReferenceName4 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName4 : string.Empty,
                ReferenceName5 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName5 : string.Empty,
                ReferenceName6 = model.ServiceToPay.AskUserForReferences ? model.ReferenceName6 : string.Empty,
                ReferenceValue = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue : string.Empty,
                ReferenceValue2 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue2 : string.Empty,
                ReferenceValue3 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue3 : string.Empty,
                ReferenceValue4 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue4 : string.Empty,
                ReferenceValue5 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue5 : string.Empty,
                ReferenceValue6 = model.ServiceToPay.AskUserForReferences ? model.ReferenceValue6 : string.Empty,
                Sucive = model.Sucive
            });
        }

        #endregion

        public ActionResult CancelAssosiation()
        {
            CleanSession();
            return RedirectToAction("Index", "Service");
        }

        public async Task<ActionResult> AddAutomaticPayment(SuccessfulAssosiationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.UnlimitedAmount)
                    {
                        if (model.MaxAmount <= 0)
                        {
                            model.MaxAmountIsNullOrZero = true;
                            return View("SuccessfulAssosiation", model);
                        }
                    }

                    var automaticPaymentDto = new AutomaticPaymentDto()
                    {
                        DaysBeforeDueDate = model.DayBeforeExpiration,
                        Maximum = model.MaxAmount,
                        Quotas = model.MaxCountPayments,
                        ServiceAssosiateId = model.ServiceId,
                        UnlimitedQuotas = model.UnlimitedQuotas,
                        UnlimitedAmount = model.UnlimitedAmount,
                        SuciveAnnualPatent = model.SuciveAnnualPatent,
                    };

                    await _webServiceAssosiateClientService.AddPayment(automaticPaymentDto);
                    ShowNotification(PresentationWebStrings.Automatic_Payment_Added, NotificationType.Success);
                    return RedirectToAction("Index");
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return View("SuccessfulAssosiation", model);
        }

        public async Task<ActionResult> GetServicesAssosiated(String name, bool all, int payments)
        {
            try
            {
                var currentUser = await CurrentSelectedUser();
                var models = all ?
                    await LoadServices(name, new ServiceFilterAssosiateDto()
                    {
                        UserId = currentUser.Id,
                        Service = name,
                        DisplayLength = 500,
                        IncludeDeleted = false,
                        WithAutomaticPaymentsInt = payments
                    })
                        : await LoadServices(name, new ServiceFilterAssosiateDto()
                        {
                            UserId = currentUser.Id,
                            Service = name,
                            IncludeDeleted = false,
                            WithAutomaticPaymentsInt = payments
                        });

                ViewBag.IsSearch = !String.IsNullOrEmpty(name);

                return PartialView("_ServiceList", models);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return null;
        }

        #region Editar Datos de servicio

        public async Task<ActionResult> EditServiceAssosiated(Guid serviceId)
        {
            CleanSession();
            try
            {

                var service = await _webServiceAssosiateClientService.Find(serviceId);
                var serviceDto = await _webServiceClientService.Find(service.ServiceId);
                var hasPayments = await _webPaymentClientService.IsPaymentDoneWithServiceAssosiated(service.Id);
                var services = await _webServiceClientService.GetServicesEnableAssociation();
                //contenedor
                ServiceDto firstCombo = null;
                if (serviceDto.ServiceContainerId.HasValue)
                {
                    firstCombo = await _webServiceClientService.Find(serviceDto.ServiceContainerId.Value);
                }

                var isApp =
                    serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Apps) && s.Active);

                var model = new ServiceAssociateModel()
                {
                    DisableEdition = hasPayments || isApp,
                    Description = service.Description,
                    ServiceName = service.ServiceDto.Name,

                    Id = service.Id.ToString(),
                    Sucive = serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Sucive || s.Gateway.Enum == (int)GatewayEnum.Geocom) && s.Active),
                    Departaments = GenerateDepartamentList(),
                    LocationsCiu = await LoadLocation(serviceDto),
                    ServiceType = ServiceType(serviceDto),
                    ServiceFirstComboName = firstCombo == null ? serviceDto.Name : firstCombo.Name,
                    ServiceFirstComboId = firstCombo == null ? Guid.Empty : firstCombo.Id,
                    ServiceSecondcomboId = firstCombo == null ? Guid.Empty : serviceDto.Id,
                    Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name),
                    AskReferences = serviceDto.AskUserForReferences,
                    ReferenceValue = service.ReferenceNumber,
                    ReferenceValue2 = service.ReferenceNumber2,
                    ReferenceValue3 = service.ReferenceNumber3,
                    ReferenceValue4 = service.ReferenceNumber4,
                    ReferenceValue5 = service.ReferenceNumber5,
                    ReferenceValue6 = service.ReferenceNumber6,
                    ReferenceName = serviceDto.ReferenceParamName,
                    ReferenceName2 = serviceDto.ReferenceParamName2,
                    ReferenceName3 = serviceDto.ReferenceParamName3,
                    ReferenceName4 = serviceDto.ReferenceParamName4,
                    ReferenceName5 = serviceDto.ReferenceParamName5,
                    ReferenceName6 = serviceDto.ReferenceParamName6,
                    ReferenceRegex = serviceDto.ReferenceParamRegex,
                    ReferenceRegex2 = serviceDto.ReferenceParamRegex2,
                    ReferenceRegex3 = serviceDto.ReferenceParamRegex3,
                    ReferenceRegex4 = serviceDto.ReferenceParamRegex4,
                    ReferenceRegex5 = serviceDto.ReferenceParamRegex5,
                    ReferenceRegex6 = serviceDto.ReferenceParamRegex6,
                };

                //ViewBag.CreditCard = serviceDto.CreditCard;
                //ViewBag.DebitCard = serviceDto.DebitCard;
                //ViewBag.CreditCardInternational = serviceDto.CreditCardInternational;
                //ViewBag.DebitCardInternational = serviceDto.DebitCardInternational;

                //Session[SessionConstants.SERVICES_ASSOSIATION] = model;

                return View("EditServiceAssosiated", model);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditNotification(ServiceAssociateModel model)
        {
            try
            {
                var serviceAssosiated = await _webServiceAssosiateClientService.Find(Guid.Parse(model.Id));
                model.ReferenceName = serviceAssosiated.ServiceDto.ReferenceParamName;
                model.ReferenceName2 = serviceAssosiated.ServiceDto.ReferenceParamName2;
                model.ReferenceName3 = serviceAssosiated.ServiceDto.ReferenceParamName3;
                model.ReferenceName4 = serviceAssosiated.ServiceDto.ReferenceParamName4;
                model.ReferenceName5 = serviceAssosiated.ServiceDto.ReferenceParamName5;
                model.ReferenceName6 = serviceAssosiated.ServiceDto.ReferenceParamName6;
                model.ServiceToPayId = serviceAssosiated.ServiceId;
                model.ServiceAssosiatedId = serviceAssosiated.Id;
                model.ReferenceValue6 = serviceAssosiated.ReferenceNumber6; // id padron de sucive
                model.AskReferences = serviceAssosiated.ServiceDto.AskUserForReferences;

                var service = await _webServiceClientService.Find(serviceAssosiated.ServiceId);
                var fullNotifications = service.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Banred && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active) ||
                        (s.Gateway.Enum == (int)GatewayEnum.Sistarbanc && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Sucive && s.Active) || (s.Gateway.Enum == (int)GatewayEnum.Carretera && s.Active));

                CheckReferences(model);
                if (ModelState.IsValid)
                {
                    #region TrimValues

                    model.TrimAllStringsProperties();

                    #endregion

                    var refs = new string[]
                               {
                                   model.ReferenceValue, model.ReferenceValue2,
                                   model.ReferenceValue3, model.ReferenceValue4,
                                   model.ReferenceValue5, model.ReferenceValue6
                               };
                    var currentUser = await CurrentSelectedUser();
                    var serviceAssosiatedId = await _webServiceAssosiateClientService.IsServiceAssosiatedToUser(currentUser.Id, model.ServiceToPayId, refs);
                    var error = false;

                    if (serviceAssosiatedId != Guid.Empty && serviceAssosiatedId != Guid.Parse(model.Id))
                    {
                        ShowNotification("Servicio ya asociado", NotificationType.Error);
                        error = true;
                    }

                    if (ValidateReferences(model))
                    {
                        ShowNotification("Faltan parametros", NotificationType.Error);
                        error = true;
                    }

                    if (!error)
                    {

                        model.Sucive =
                            service.ServiceGatewaysDto.Any(
                                s =>
                                    (s.Gateway.Enum == (int)GatewayEnum.Sucive ||
                                     s.Gateway.Enum == (int)GatewayEnum.Geocom) && s.Active);
                        var isApp = service.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Apps) && s.Active);

                        var result = isApp ? 1 : await CheckAccount(service, model.ReferenceValue, model.ReferenceValue2, model.ReferenceValue3,
                                    model.ReferenceValue4, model.ReferenceValue5, model.ReferenceValue6);

                        if (result < 1)
                        {
                            ShowNotification("No se pudo confirmar la cuenta. Chequear los datos e intente nuevamente",
                                NotificationType.Error);
                            error = true;
                        }
                        if (!error)
                        {
                            model.Departaments = GenerateDepartamentList();
                            if (model.Sucive)
                            {
                                model.ReferenceValue6 = string.IsNullOrEmpty(model.ReferenceValue6)
                                    ? result.ToString()
                                    : model.ReferenceValue6;
                            }

                            var notiModel = new ServiceAssociateNotificationModel()
                            {
                                DaysBeforeDueDateConfigEmail = serviceAssosiated.NotificationConfigDto.BeforeDueDateConfigDto.Email,
                                DaysBeforeDueDateConfigSms = serviceAssosiated.NotificationConfigDto.BeforeDueDateConfigDto.Sms,
                                DaysBeforeDueDateConfigWeb = serviceAssosiated.NotificationConfigDto.BeforeDueDateConfigDto.Web,
                                DaysBeforeDueDateConfigActive = fullNotifications,

                                ExpiredBillEmail = serviceAssosiated.NotificationConfigDto.ExpiredBillDto.Email,
                                ExpiredBillSms = serviceAssosiated.NotificationConfigDto.ExpiredBillDto.Sms,
                                ExpiredBillWeb = serviceAssosiated.NotificationConfigDto.ExpiredBillDto.Web,
                                ExpiredBillActive = fullNotifications,

                                NewBillEmail = serviceAssosiated.NotificationConfigDto.NewBillDto.Email,
                                NewBillSms = serviceAssosiated.NotificationConfigDto.NewBillDto.Sms,
                                NewBillWeb = serviceAssosiated.NotificationConfigDto.NewBillDto.Web,
                                NewBillActive = fullNotifications,

                                SuccessPaymentEmail = serviceAssosiated.NotificationConfigDto.SuccessPaymentDto.Email,
                                SuccessPaymentSms = serviceAssosiated.NotificationConfigDto.SuccessPaymentDto.Sms,
                                SuccessPaymentWeb = serviceAssosiated.NotificationConfigDto.SuccessPaymentDto.Web,
                                SuccessPaymentActive = true,

                                FailedAutomaticPaymentEmail = serviceAssosiated.NotificationConfigDto.FailedAutomaticPaymentDto.Email,
                                FailedAutomaticPaymentSms = serviceAssosiated.NotificationConfigDto.FailedAutomaticPaymentDto.Sms,
                                FailedAutomaticPaymentWeb = serviceAssosiated.NotificationConfigDto.FailedAutomaticPaymentDto.Web,
                                FailedAutomaticPaymentActive = fullNotifications,

                                DaysBeforeDueDate = serviceAssosiated.NotificationConfigDto.DaysBeforeDueDate,

                                ServiceName = model.ServiceName,


                            };

                            if (service.AskUserForReferences)
                            {
                                notiModel.ReferenceName = serviceAssosiated.ServiceDto.ReferenceParamName;
                                notiModel.ReferenceName2 = serviceAssosiated.ServiceDto.ReferenceParamName2;
                                notiModel.ReferenceName3 = serviceAssosiated.ServiceDto.ReferenceParamName3;
                                notiModel.ReferenceName4 = serviceAssosiated.ServiceDto.ReferenceParamName4;
                                notiModel.ReferenceName5 = serviceAssosiated.ServiceDto.ReferenceParamName5;
                                notiModel.ReferenceName6 = serviceAssosiated.ServiceDto.ReferenceParamName6;
                                notiModel.ReferenceValue = model.ReferenceValue;
                                notiModel.ReferenceValue2 = model.ReferenceValue2;
                                notiModel.ReferenceValue3 = model.ReferenceValue3;
                                notiModel.ReferenceValue4 = model.ReferenceValue4;
                                notiModel.ReferenceValue5 = model.ReferenceValue5;
                                notiModel.ReferenceValue6 = model.ReferenceValue6;
                                notiModel.ReferenceRegex = serviceAssosiated.ServiceDto.ReferenceParamRegex;
                                notiModel.ReferenceRegex2 = serviceAssosiated.ServiceDto.ReferenceParamRegex2;
                                notiModel.ReferenceRegex3 = serviceAssosiated.ServiceDto.ReferenceParamRegex3;
                                notiModel.ReferenceRegex4 = serviceAssosiated.ServiceDto.ReferenceParamRegex4;
                                notiModel.ReferenceRegex5 = serviceAssosiated.ServiceDto.ReferenceParamRegex5;
                                notiModel.ReferenceRegex6 = serviceAssosiated.ServiceDto.ReferenceParamRegex6;
                            }


                            model.NotificationConfig = notiModel;
                            var hasPayments = await _webPaymentClientService.IsPaymentDoneWithServiceAssosiated(service.Id);

                            model.DisableEdition = hasPayments || isApp;

                            Session[SessionConstants.SERVICES_ASSOSIATION] = model;


                            return View("EditServiceAssociatedNotification", notiModel);
                        }
                    }
                }
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }
            catch (WebApiClientBillBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
            }

            ViewBag.Services =
                (await _webServiceClientService.GetServicesEnableAssociation()).Select(
                    s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);

            if (model.ServiceToPayId != Guid.Empty)
            {
                var service = await _webServiceClientService.Find(model.ServiceToPayId);
                //ViewBag.CreditCard = service.CreditCard;
                //ViewBag.DebitCard = service.DebitCard;
                //ViewBag.CreditCardInternational = service.CreditCardInternational;
                //ViewBag.DebitCardInternational = service.DebitCardInternational;
            }

            return View("EditServiceAssosiated", model);
        }

        public async Task<ActionResult> BackEditServiceAssosiated()
        {
            var model = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
            var service = await _webServiceClientService.Find(model.ServiceToPayId);

            //ViewBag.CreditCard = service.CreditCard;
            //ViewBag.DebitCard = service.DebitCard;
            //ViewBag.CreditCardInternational = service.CreditCardInternational;
            //ViewBag.DebitCardInternational = service.DebitCardInternational;

            return View("EditServiceAssosiated", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditServiceAssosiated(ServiceAssociateNotificationModel model)
        {
            if (ModelState.IsValid)
            {
                var modelo = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];

                //Estos campos se ocultaron de la página porque se van a notificar siempre (por el momento se hardcodean en true)     
                model.FailedAutomaticPaymentEmail = true;

                modelo.NotificationConfig = model;

                Session[SessionConstants.SERVICES_ASSOSIATION] = modelo;

                await _webServiceAssosiateClientService.Edit(modelo.ToDto());
                ShowNotification(PresentationWebStrings.Service_Updated, NotificationType.Success);

                return RedirectToAction("Index");
            }
            return View("Associate_Step_Notification", model);
        }

        #endregion

        private async Task<List<ServiceListModel>> LoadServices(String name, ServiceFilterAssosiateDto filter)
        {
            var currentUser = await CurrentSelectedUser();
            var list = await _webServiceAssosiateClientService.GetDataForFrontList(filter);
            ViewBag.DisplayStart = filter.DisplayStart;
            ViewBag.DisplayLength = filter.DisplayLength;

            var models = new List<ServiceListModel>();
            currentUser = await _webApplicationUserClientService.GetUserWithCards(currentUser.Id);

            foreach (var serviceAssociatedDto in list)
            {
                try
                {
                    var model = new ServiceListModel();
                    model.ServiceId = serviceAssociatedDto.Id;
                    model.ServiceName = serviceAssociatedDto.ServiceDto.Name;
                    model.ServiceDesc = serviceAssociatedDto.Description;
                    model.ServiceRefName = serviceAssociatedDto.ServiceDto.ReferenceParamName;
                    model.ServiceRefName2 = serviceAssociatedDto.ServiceDto.ReferenceParamName2;
                    model.ServiceRefName3 = serviceAssociatedDto.ServiceDto.ReferenceParamName3;
                    model.ServiceRefValue = serviceAssociatedDto.ReferenceNumber;
                    model.ServiceRefValue2 = serviceAssociatedDto.ReferenceNumber2;
                    model.ServiceRefValue3 = serviceAssociatedDto.ReferenceNumber3;
                    model.ServiceRefName4 = serviceAssociatedDto.ServiceDto.ReferenceParamName4;
                    model.ServiceRefName5 = serviceAssociatedDto.ServiceDto.ReferenceParamName5;
                    model.ServiceRefName6 = serviceAssociatedDto.ServiceDto.ReferenceParamName6;
                    model.ServiceRefValue4 = serviceAssociatedDto.ReferenceNumber4;
                    model.ServiceRefValue5 = serviceAssociatedDto.ReferenceNumber5;
                    model.ServiceRefValue6 = serviceAssociatedDto.ReferenceNumber6;
                    model.ServiceContainerName = serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.ServiceContainerName : string.Empty;
                    model.ServiceImageName = GetImageForService(serviceAssociatedDto.ServiceDto);
                    model.CardsMask = currentUser.CardDtos.Select(t => t.MaskedNumber).ToList();
                    model.Active = serviceAssociatedDto.Active;
                    model.DefaultMask = serviceAssociatedDto.DefaultCard.MaskedNumber;
                    model.ServiceAutomaticPaymentId = serviceAssociatedDto.AutomaticPaymentDtoId;
                    model.EnableAutomaticPayment = serviceAssociatedDto.ServiceDto.EnableAutomaticPayment;
                    model.Cards = serviceAssociatedDto.CardDtos.Select(c => new CardModel()
                    {
                        Number = c.MaskedNumber,
                        Description = c.Description
                    }).ToList();
                    model.AskUserForReferences = serviceAssociatedDto.ServiceDto.AskUserForReferences;

                    model.AllowGetBills = serviceAssociatedDto.ServiceDto.AllowGetBills;
                    model.AllowInputAmount = serviceAssociatedDto.ServiceDto.AllowInputAmount;

                    models.Add(model);
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Error en la lectura de servicios asociados!");
                    NLogLogger.LogEvent(NLogType.Error, exception.Message);
                }
            }
            return models;
        }

        private bool ValidateReferences(ServiceAssociateModel model)
        {
            //check si falta alguna referencia. Es dinamico segun el servicio

            if (!String.IsNullOrEmpty(model.ReferenceName))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue))
                {

                    ShowNotification(
                        PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName,
                        NotificationType.Error);
                    return true;
                }
            }
            //Si es tupe CIU. solo quiero chequear el primer valor
            if (!string.IsNullOrEmpty(model.ServiceType) && model.ServiceType.Equals("CIU"))
                return false;

            if (!String.IsNullOrEmpty(model.ReferenceName2))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue2))
                {
                    ShowNotification(
                        PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName2,
                        NotificationType.Error);
                    return true;
                }
            }

            if (!String.IsNullOrEmpty(model.ReferenceName3))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue3))
                {
                    ShowNotification(PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName3,
                        NotificationType.Error);
                    return true;
                }
            }
            if (!String.IsNullOrEmpty(model.ReferenceName4))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue4))
                {

                    ShowNotification(
                        PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName4,
                        NotificationType.Error);
                }
            }
            if (!String.IsNullOrEmpty(model.ReferenceName5))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue5))
                {

                    ShowNotification(
                        PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName5,
                        NotificationType.Error);
                }
            }
            if (!String.IsNullOrEmpty(model.ReferenceName6))
            {
                if (String.IsNullOrEmpty(model.ReferenceValue6))
                {

                    ShowNotification(
                        PresentationWebStrings.Error_Missing_Reference + " " + model.ReferenceName6,
                        NotificationType.Error);
                }
            }
            return false;
        }

        [HttpPost]
        public async Task<ActionResult> DeleteService(Guid serviceId, string name, int payments, string all)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            var currentUser = await CurrentSelectedUser();
            try
            {
                await _webServiceAssosiateClientService.DeleteService(serviceId);

                message = PresentationWebStrings.Service_Delete;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;
            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadServices(name, new ServiceFilterAssosiateDto()
            {
                UserId = currentUser.Id,
                Service = string.IsNullOrEmpty(name) ? null : name,
                DisplayLength = all.Equals("true") ? 500 : 10,
                IncludeDeleted = false,
                WithAutomaticPaymentsInt = payments
            });

            ViewBag.IsSearch = !String.IsNullOrEmpty(name);

            var content = RenderPartialViewToString("_ServiceList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        private void CleanSession()
        {
            Session[SessionConstants.SERVICES_ASSOSIATION] = null;
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(RedirectEnums redirectTo, string nameTh, string cardBin, ServiceAssociateNotificationModel notificationmodel, string fpProfiler)
        {
            var currentUser = await CurrentSelectedUser();
            var noti = ToNotificationConfigDto(notificationmodel);
            var url = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            var model = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
            var keys = await _webCyberSourceAccessClientService.GenerateKeys(new KeysInfoForTokenRegisteredUser
            {
                UserId = currentUser.Id,
                TransactionReferenceNumber = Guid.NewGuid().ToString(),
                RedirectTo = redirectTo.ToString("D"),
                NameTh = nameTh,
                CardBin = cardBin,
                CallcenterUser = String.Empty,
                Platform = PaymentPlatformDto.VisaNet.ToString(),
                PaymentTypeDto = PaymentTypeDto.Manual,
                NotificationsConfig = noti,
                UrlReturn = url,
                ServiceId = model.ServiceToPayId,
                ReferenceNumber1 = model.ReferenceValue,
                ReferenceNumber2 = model.ReferenceValue2,
                ReferenceNumber3 = model.ReferenceValue3,
                ReferenceNumber4 = model.ReferenceValue4,
                ReferenceNumber5 = model.ReferenceValue5,
                ReferenceNumber6 = model.ReferenceValue6,
                FingerPrint = fpProfiler
            });
            Session[SessionConstants.TEMPORARY_ID] = keys["merchant_defined_data29"];
            return keys;
        }

        //CALLBACK DE CYBERSOURCE
        public async Task<ActionResult> TokengenerationCallBack()
        {
            try
            {

                var formData = GenerateDictionary(Request.Form);
                var result = await _webServiceAssosiateClientService.ProccesDataFromCybersource(formData);

                if (result.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (result.ServiceAssociatedDto != null)
                    {
                        Session[SessionConstants.SERVICES_ASSOSIATION] = null;
                        if (result.ServiceAssociatedDto.ServiceDto.EnableAutomaticPayment)
                        {

                            var service = await _webServiceClientService.Find(result.ServiceAssociatedDto.ServiceDto.Id);

                            var sucive = service.ServiceGatewaysDto.Any(
                                    s =>
                                        (s.Gateway.Enum == (int)GatewayEnum.Sucive ||
                                         s.Gateway.Enum == (int)GatewayEnum.Geocom) && s.Active);

                            return View("SuccessfulAssosiation", new SuccessfulAssosiationModel()
                            {
                                ServiceName = result.ServiceAssociatedDto.ServiceDto.Name,
                                ServiceId = result.ServiceAssociatedDto.Id,
                                CardMask = result.ServiceAssociatedDto.DefaultCard.MaskedNumber,
                                CardId = result.ServiceAssociatedDto.DefaultCardId,
                                DayBeforeExpiration = result.ServiceAssociatedDto.AutomaticPaymentDto != null ? result.ServiceAssociatedDto.AutomaticPaymentDto.DaysBeforeDueDate : 0,
                                MaxAmount = result.ServiceAssociatedDto.AutomaticPaymentDto != null ? result.ServiceAssociatedDto.AutomaticPaymentDto.Maximum : 0,
                                MaxCountPayments = result.ServiceAssociatedDto.AutomaticPaymentDto != null ? result.ServiceAssociatedDto.AutomaticPaymentDto.Quotas : 0,
                                QuotasDone = result.ServiceAssociatedDto.AutomaticPaymentDto != null ? result.ServiceAssociatedDto.AutomaticPaymentDto.QuotasDone : 0,
                                UnlimitedQuotas = result.ServiceAssociatedDto.AutomaticPaymentDto == null || result.ServiceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas,
                                UnlimitedAmount = result.ServiceAssociatedDto.AutomaticPaymentDto == null || result.ServiceAssociatedDto.AutomaticPaymentDto.UnlimitedAmount,
                                Sucive = sucive,
                                EnableAutomaticPayment = true
                            });
                        }
                        return View("SuccessfulAssosiation", new SuccessfulAssosiationModel()
                        {
                            ServiceName = result.ServiceAssociatedDto.ServiceDto.Name,
                            CardId = result.ServiceAssociatedDto.DefaultCardId,
                            EnableAutomaticPayment = false
                        });
                    }
                    else
                    {
                        ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Info);
                        return Index();
                    }
                }
                return await EvaluateErrors(result.CybersourceCreateCardDto.TokenizationData);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Info);
                NLogLogger.LogEvent(NLogType.Error, ex.Message);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
                NLogLogger.LogEvent(NLogType.Error, ex.Message);
            }
            return Index();
        }

        private async Task<ActionResult> EvaluateErrors(CsResponseData csResponseData)
        {
            var reasonCode = (CybersourceMsg)csResponseData.PaymentResponseCode;

            if (reasonCode != CybersourceMsg.Accepted)
            {
                #region ErrorCodes

                /* 100 : ok
                 * 102 : [msj datos inválidos] error de campos             
                 * 202 : [msj datos inválidos] tarjeta vencida
                 * 204 : [msj datos inválidos] fondos insuficientes 
                 * 205 : [msj datos inválidos] tarjeta declarada como robada o perdida             
                 * 208 : [msj datos inválidos] tarjeta inactiva / no autorizada
                 * 210 : [msj datos inválidos] limite de crédito excedido
                 * 211 : [msj datos inválidos] CVN inválido
                 * 222 : [msj datos inválidos] cuenta congelada
                 * 231 : [msj datos inválidos] número de cuenta inválido
                 * 232 : [msj datos inválidos] el tipo de tarjeta no es aceptada por el procesador             
                 * 240 : [msj datos inválidos] El tipo de tarjeta es inválido o no correpsonde con el nro de cuenta
                 * 230 : [msj datos inválidos] Autorización aceptada, declinada por cybersource (CVN Check)
                 * 104 : [página error] error de keys
                 * 200 : [página error] Autorización aceptada, rechazada por cybersource por no pasar el AVS
                 * 201 : [página error] Autorización automatica rechazada
                 * 207 : [página error] error de banco             
                 * 233 : [página error] Negación general
                 * 234 : [página error] Error en datos del usuario en cybersource
                 * 236 : [página error] Error general del procesador
                 * 475 : [página error] Payer authentication error
                 * 476 : [página error] Payer authentication error
                 * 520 : [página error] Autorización aceptada, declinada por cybersource
                 */

                #endregion

                switch (reasonCode)
                {
                    case CybersourceMsg.InvalidFields:
                    case CybersourceMsg.CardIssuer:
                    case CybersourceMsg.AuthorizationRejected:
                    case CybersourceMsg.ExpiredCard:
                    case CybersourceMsg.GeneralDecline:
                    case CybersourceMsg.InsufficientFunds:
                    case CybersourceMsg.StolenLostCard:
                    case CybersourceMsg.BankUnavailable:
                    case CybersourceMsg.InactiveUnAuthorizedCard:
                    case CybersourceMsg.CreditLimitReached:
                    case CybersourceMsg.InvalidCVN:
                    case CybersourceMsg.AccountFrozen:
                    case CybersourceMsg.CVNCheckInvalid:
                    case CybersourceMsg.InvalidAccountNumber:
                    case CybersourceMsg.CardTypeNotAccepted:
                    case CybersourceMsg.GeneralDeclineByProcessor:
                    case CybersourceMsg.ProcessorFailure:
                    case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    case CybersourceMsg.PayerAuthenticationError:
                        return await InvalidCardData(csResponseData);

                    case CybersourceMsg.AVSCheckInvalid:
                    case CybersourceMsg.UserCybersourceError:
                    case CybersourceMsg.PayerAuthenticationNotAuthenticated:
                    case CybersourceMsg.AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings:
                    case CybersourceMsg.ConfigurationKeysInvalids:
                        return RedirectToAction("NotificationError");
                }
            }

            return null;
        }

        private async Task<ViewResult> InvalidCardData(CsResponseData csResponseData)
        {
            ShowNotification(csResponseData.PaymentResponseMsg, NotificationType.Info);
            var model = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
            var user = await CurrentSelectedUser();
            var cards = await _webServiceClientService.GetEnableCards(user.Id, model.ServiceToPayId);

            if (cards != null && cards.Any())
            {
                var today = DateTime.Now.Date;
                ViewBag.Cards = cards.Where(c => c.Active && ((c.DueDate.Year.CompareTo(today.Year) == 0 && c.DueDate.Month.CompareTo(today.Month) >= 0) ||
                        c.DueDate.Year.CompareTo(today.Year) > 0)).ToList();
            }
            return View("Associate_Step_Card", new ServiceAssociateCardModel()
            {
                ServiceName = model.ServiceName,
                ReferenceName = model.ReferenceName,
                ReferenceName2 = model.ReferenceName2,
                ReferenceName3 = model.ReferenceName3,
                ReferenceName4 = model.ReferenceName4,
                ReferenceName5 = model.ReferenceName5,
                ReferenceName6 = model.ReferenceName6,
                ReferenceValue = model.ReferenceValue,
                ReferenceValue2 = model.ReferenceValue2,
                ReferenceValue3 = model.ReferenceValue3,
                ReferenceValue4 = model.ReferenceValue4,
                ReferenceValue5 = model.ReferenceValue5,
                ReferenceValue6 = model.ReferenceValue6,
                ServiceImageUrl = model.TooltipeImage,
            });
        }

        public ActionResult NotificationError()
        {
            Session[SessionConstants.SERVICES_ASSOSIATION] = null;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ValidateCardType(bool newAssociation, int maskedNumber, string nameTh, string fpProfiler)
        {
            try
            {
                ServiceDto service;
                ServiceAssociateNotificationModel notificationModel = null;
                if (newAssociation)
                {
                    var modelo = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
                    notificationModel = modelo.NotificationConfig;
                    service = await _webServiceClientService.Find(modelo.ServiceToPayId);
                }
                else
                {
                    var modelo = (ServiceAssociateCardModel)Session[SessionConstants.SERVICES_ASSOSIATE_NEW_CARD];
                    var serviceAssociated = await _webServiceAssosiateClientService.Find(modelo.ServiceAssosiateId);
                    service = serviceAssociated.ServiceDto;
                }

                var bin = await _binClientService.Find(maskedNumber);

                if (bin != null && !bin.Active)
                {
                    return
                       Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid,
                           PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                //si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = bin == null || (await _webServiceClientService.IsBinAssociatedToService(bin.Value, service.Id));
                if (!isBinAssociatedToService)
                {
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid_For_Service,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
                var cybersourceData = await LoadKeysForCybersource(RedirectEnums.PrivateAssosiate, nameTh, maskedNumber.ToString(), notificationModel, fpProfiler);
                var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData);

                return Json(new JsonResponse(AjaxResponse.Success, new { keys = content, }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);

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
        }

        private async Task<Collection<CardListModel>> GetCardList(Guid serviceId, bool multipleCards)
        {
            var saDto = await _webServiceAssosiateClientService.Find(serviceId);
            var list = new Collection<CardListModel>();

            var user = await CurrentSelectedUser();
            user = await _webApplicationUserClientService.GetUserWithCards(user.Id);

            var listMask = user.CardDtos.Where(c => c.Active).Select(c => Int32.Parse(c.MaskedNumber.Substring(0, 6))).ToList();
            var bins = await _binClientService.GetBinsFromMask(listMask);

            foreach (var dto in user.CardDtos.Where(c => c.Active))
            {
                var today = DateTime.Now.Date;
                var bin = bins.FirstOrDefault(b => b.Value == Int32.Parse(dto.MaskedNumber.Substring(0, 6)));
                var cardDto = await _webCardClientService.FindWithServices(dto.Id);
                list.Add(new CardListModel()
                {
                    Expired = (dto.DueDate.Year.CompareTo(today.Year) == 0 && dto.DueDate.Month.CompareTo(today.Month) >= 0) || dto.DueDate.Year.CompareTo(today.Year) > 0,
                    Active = ((dto.DueDate.Year.CompareTo(today.Year) == 0 && dto.DueDate.Month.CompareTo(today.Month) >= 0) || dto.DueDate.Year.CompareTo(today.Year) > 0) && dto.Active,
                    Mask = dto.MaskedNumber,
                    Description = dto.Description,
                    DueDate = dto.DueDate.ToString("MM-yyyy"),
                    Default = saDto.DefaultCardId == dto.Id,
                    ServiceId = serviceId,
                    Id = dto.Id,
                    CardImage = bin == null || string.IsNullOrEmpty(bin.ImageUrl) ? null : bin.ImageUrl,
                    MultipleCards = multipleCards,
                    Used = cardDto.ServicesAssociatedDto != null && cardDto.ServicesAssociatedDto.Any(x => x.Id == saDto.Id)
                });
            }
            return list;
        }

        public async Task<ViewResult> CardList(Guid serviceId, string serviceName)
        {
            var saDto = await _webServiceAssosiateClientService.Find(serviceId);
            var serviceDto = await _webServiceClientService.Find(saDto.ServiceId);
            //var isApp = serviceDto.ServiceGatewaysDto.Any(s => (s.Gateway.Enum == (int)GatewayEnum.Apps) && s.Active);
            var askReferences = serviceDto.AskUserForReferences;
            var list = await GetCardList(serviceId, serviceDto.AllowMultipleCards);

            var model = new ServiceCardListModel()
            {
                Cards = list,
                ServiceId = serviceDto.Id,
                ServiceName = saDto.ServiceDto.Name,
                ServiceDesc = saDto.Description,
                ServiceRefName = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName,
                ServiceRefName2 = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName2,
                ServiceRefName3 = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName3,
                ServiceRefName4 = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName4,
                ServiceRefName5 = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName5,
                ServiceRefName6 = !askReferences ? "" : saDto.ServiceDto.ReferenceParamName6,
                ServiceRefValue = !askReferences ? "" : saDto.ReferenceNumber,
                ServiceRefValue2 = !askReferences ? "" : saDto.ReferenceNumber2,
                ServiceRefValue3 = !askReferences ? "" : saDto.ReferenceNumber3,
                ServiceRefValue4 = !askReferences ? "" : saDto.ReferenceNumber4,
                ServiceRefValue5 = !askReferences ? "" : saDto.ReferenceNumber5,
                ServiceRefValue6 = !askReferences ? "" : saDto.ReferenceNumber6,
                ServiceNameFiltered = serviceName
            };

            return View("ListCard", model);
        }

        public async Task<int> CheckPaymentModel()
        {
            try
            {
                var service = (ServiceAssociateModel)Session[SessionConstants.SERVICES_ASSOSIATION];
                if (User.Identity.IsAuthenticated && service != null)
                {
                    return (int)AjaxResponse.Success;
                }
            }
            catch (Exception e)
            {

            }
            Session[SessionConstants.CURRENT_SELECTED_USER] = null;
            return (int)AjaxResponse.Error;
        }

        public List<SelectListItem> GenerateDepartamentList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(DepartamentDtoType)).Cast<DepartamentDtoType>();
            return list.Select(departamentDtoType => new SelectListItem()
            {
                Text = rm.GetString(departamentDtoType.ToString()),
                Value = (int)departamentDtoType + "",
            }).ToList();
        }

        public async Task<int> CheckAccount(ServiceDto service, string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4, string referenceNumber5, string referenceNumber6)
        {
            var references = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(service.ReferenceParamName))
                references.Add(service.ReferenceParamName, referenceNumber);

            if (!string.IsNullOrEmpty(service.ReferenceParamName2))
                references.Add(service.ReferenceParamName2, referenceNumber2);
            if (!string.IsNullOrEmpty(service.ReferenceParamName3))
                references.Add(service.ReferenceParamName3, referenceNumber3);
            if (!string.IsNullOrEmpty(service.ReferenceParamName4))
                references.Add(service.ReferenceParamName4, referenceNumber4);
            if (!string.IsNullOrEmpty(service.ReferenceParamName5))
                references.Add(service.ReferenceParamName5, referenceNumber5);
            if (!string.IsNullOrEmpty(service.ReferenceParamName6))
                references.Add(service.ReferenceParamName6, referenceNumber6);

            var result = await _webBillClientService.CheckAccount(new RegisteredUserBillFilterDto()
            {
                References = references,
                ServiceId = service.Id,
                ScheduledPayment = false,
            });
            return result;
        }

        private async Task<List<SelectListItem>> LoadLocation(ServiceDto service)
        {
            if (service != null && service.ServiceGatewaysDto != null && service.ServiceGatewaysDto.Any())
            {
                var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Active && x.Gateway.Enum == (int)GatewayEnum.Geocom);
                if (gateway != null && gateway.ReferenceId.Equals("CIU"))
                {
                    if (string.IsNullOrEmpty(service.ReferenceParamName2))
                        return null;//ESTA POR ID PADRON

                    var locations = await _webLocationClientService.GetList(new LocationFilterDto()
                    {
                        DepartamentDtoType = service.Departament,
                        GatewayEnumDto =
                            (GatewayEnumDto)
                            gateway.Gateway.Enum,
                    });
                    return locations.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Value,
                    }).ToList();
                }
            }
            return null;
        }

        public ActionResult ConfirmDeleteAjax(Guid id, string name)
        {
            var model = new ServiceAssociateModel
            {
                ServiceName = name,
                ServiceAssosiatedId = id
            };

            return PartialView("_ConfirmDelete", model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAutomaticPayment(Guid serviceId, String name, int payments, string all)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            var currentUser = await CurrentSelectedUser();
            try
            {

                await _webServiceAssosiateClientService.DeleteAutomaticPayment(serviceId);

                message = PresentationWebStrings.Automatic_Payment_Deleted;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;
            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            var list = await LoadServices(name, new ServiceFilterAssosiateDto()
            {
                UserId = currentUser.Id,
                Service = string.IsNullOrEmpty(name) ? null : name,
                DisplayLength = all.Equals("true") ? 500 : 10,
                IncludeDeleted = false,
                WithAutomaticPaymentsInt = payments
            });

            ViewBag.IsSearch = !String.IsNullOrEmpty(name);

            var content = RenderPartialViewToString("_ServiceList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteCardFromService(Guid serviceId, Guid cardId)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();
                var result = await _webServiceAssosiateClientService.DeleteCardFromService(new CardServiceDataDto()
                {
                    CardId = cardId,
                    ServiceId = serviceId,
                    UserId = user.Id,
                    OperationId = string.Empty,
                });
                if (result)
                {
                    message = PresentationWebStrings.Service_Card_Deactivate;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = "No pudimos desactivar tu tarjeta. Intentá nuevamente o comunicate con el CallCenter";
                    title = "";
                }

            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            var list = await GetCardList(serviceId, true);
            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        //SE ASOCIA AL SERVICIO UNA TARJETA YA EXISTENTE
        public async Task<ActionResult> AddCardToService(Guid serviceId, Guid cardId)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();
                var result = await _webServiceAssosiateClientService.AddCardToService(new CardServiceDataDto()
                {
                    CardId = cardId,
                    ServiceId = serviceId,
                    UserId = user.Id,
                    OperationId = string.Empty,
                });
                if (result)
                {
                    message = PresentationWebStrings.Service_Card_Activate;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = PresentationWebStrings.Add_card_To_Service;
                    title = "";
                }

            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (Exception e)
            {
                message = e.Message;
                title = PresentationWebStrings.Action_Error;
            }
            var sa = await _webServiceAssosiateClientService.Find(serviceId);
            var list = await GetCardList(serviceId, sa.ServiceDto.AllowMultipleCards);

            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> LogCSInvocation()
        {
            try
            {
                //Creo nuevo log
                await _logClientService.Put(Guid.Parse((string)Session[SessionConstants.TEMPORARY_ID]), new LogDto
                {
                    LogCommunicationType = LogCommunicationType.CyberSource,
                    LogType = LogType.Info,
                    LogOperationType = LogOperationType.ServiceAssociated,
                    CallCenterMessage = string.Format("Inicia comunicación a CS para asociar tarjeta"),
                    Message = string.Format("Inicia comunicación a CS para asociar tarjeta")
                });
                return Json(new JsonResponse(AjaxResponse.Success, "", "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        private void CheckReferences(ServiceAssociateModel model)
        {
            if (string.IsNullOrEmpty(model.ReferenceName) && string.IsNullOrEmpty(model.ReferenceValue))
            {
                ModelState.Remove("ReferenceValue");
            }
            if (string.IsNullOrEmpty(model.ReferenceName2) && string.IsNullOrEmpty(model.ReferenceValue2))
            {
                ModelState.Remove("ReferenceValue2");
            }
            if (string.IsNullOrEmpty(model.ReferenceName3) && string.IsNullOrEmpty(model.ReferenceValue3))
            {
                ModelState.Remove("ReferenceValue3");
            }
            if (string.IsNullOrEmpty(model.ReferenceName4) && string.IsNullOrEmpty(model.ReferenceValue4))
            {
                ModelState.Remove("ReferenceValue4");
            }
            if (string.IsNullOrEmpty(model.ReferenceName5) && string.IsNullOrEmpty(model.ReferenceValue5))
            {
                ModelState.Remove("ReferenceValue5");
            }
            if (string.IsNullOrEmpty(model.ReferenceName6) && string.IsNullOrEmpty(model.ReferenceValue6))
            {
                ModelState.Remove("ReferenceValue6");
            }

        }

        public NotificationConfigDto ToNotificationConfigDto(ServiceAssociateNotificationModel dto)
        {
            var notificationConfigDto = new NotificationConfigDto()
            {
                DaysBeforeDueDate = dto.DaysBeforeDueDate,
                BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                {
                    Email = dto.DaysBeforeDueDateConfigEmail,
                    Sms = dto.DaysBeforeDueDateConfigSms,
                    Web = dto.DaysBeforeDueDateConfigWeb
                },
                ExpiredBillDto = new ExpiredBillDto()
                {
                    Email = dto.ExpiredBillEmail,
                    Sms = dto.ExpiredBillSms,
                    Web = dto.ExpiredBillWeb
                },
                NewBillDto = new NewBillDto()
                {
                    Email = dto.NewBillEmail,
                    Sms = dto.NewBillSms,
                    Web = dto.NewBillWeb
                },
                FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                {
                    Email = dto.FailedAutomaticPaymentEmail,
                    Sms = dto.FailedAutomaticPaymentSms,
                    Web = dto.FailedAutomaticPaymentWeb
                },
                SuccessPaymentDto = new SuccessPaymentDto()
                {
                    Email = dto.SuccessPaymentEmail,
                    Sms = dto.SuccessPaymentSms,
                    Web = dto.SuccessPaymentWeb
                }
            };

            //Estos campos se ocultaron de la página porque se van a notificar siempre (por el momento se hardcodean en true)
            notificationConfigDto.FailedAutomaticPaymentDto.Email = true;

            return notificationConfigDto;
        }

    }
}