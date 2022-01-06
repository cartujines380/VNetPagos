using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NLog.LogReceiverService;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Pay.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using VisaNet.Utilities.JsonResponse;
using BillModel = VisaNet.Presentation.Web.Models.BillModel;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;
using PaymentBillModel = VisaNet.Presentation.Web.Models.PaymentBillModel;
using PaymentServiceModel = VisaNet.Presentation.Web.Models.PaymentServiceModel;

namespace VisaNet.Presentation.Web.Areas.Pay.Controllers
{
    public class PayController : BaseController
    {
        private readonly IWebPaymentClientService _paymentClientService;
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebBillClientService _billClientService;
        private readonly IWebLogClientService _logClientService;
        private readonly IWebLocationClientService _locationClientService;
        private readonly IWebDiscountClientService _discountClientService;
        private readonly IWebApplicationUserClientService _applicationUserClientService;
        private readonly IWebServiceAssosiateClientService _serviceAssosiateClientService;
        private readonly IWebCardClientService _cardClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;

        public PayController(IWebPaymentClientService paymentClientService, IWebServiceClientService serviceClientService, IWebBillClientService billClientService,
            IWebLogClientService logClientService, IWebDiscountClientService discountClientService, IWebLocationClientService locationClientService,
            IWebApplicationUserClientService applicationUserClientService, IWebServiceAssosiateClientService serviceAssosiateClientService,
            IWebCardClientService cardClientService, IWebCyberSourceAccessClientService cyberSourceAccessClientService)
        {
            _paymentClientService = paymentClientService;
            _serviceClientService = serviceClientService;
            _billClientService = billClientService;
            _logClientService = logClientService;
            _discountClientService = discountClientService;
            _locationClientService = locationClientService;
            _applicationUserClientService = applicationUserClientService;
            _serviceAssosiateClientService = serviceAssosiateClientService;
            _cardClientService = cardClientService;
            _webCyberSourceAccessClientService = cyberSourceAccessClientService;
        }

        public ActionResult NewPayment()
        {
            Session[SessionConstants.PAYMENT_DATA] = null;
            return RedirectToAction("Service");
        }

        //Este metodo es para cuando vuelve hacia atras desde el paso 2
        public async Task<ActionResult> Service()
        {

            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
            var user = await CurrentSelectedUser();
            var model = new PaymentServiceModel();

            model.RegisteredUserId = user != null ? user.Id : Guid.Empty;

            var services = user == null ? await _serviceClientService.GetServicesPaymentPublic() : await _serviceClientService.GetServicesPaymentPrivate();
            var dptos = GenerateDepartamentList();

            model.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);

            if (payment == null)
            {
                return View(model);
            }

            //Session[SessionConstants.PAYMENT_DATA] = null;

            var service = await _serviceClientService.Find(payment.ServiceId);
            //ViewBag.CreditCard = service.CreditCard;
            //ViewBag.DebitCard = service.DebitCard;
            //ViewBag.CreditCardInternational = service.CreditCardInternational;
            //ViewBag.DebitCardInternational = service.DebitCardInternational;

            var locations = await LoadLocation(service);

            //contenedor
            ServiceDto firstCombo = null;
            List<ServiceDto> list = null;
            if (service.ServiceContainerId.HasValue)
            {
                firstCombo = await _serviceClientService.Find(service.ServiceContainerId.Value);
                list = await _serviceClientService.FindAll(new ServiceFilterDto()
                {
                    ServiceContainerId =
                                                                   service.ServiceContainerId.ToString(),
                }) as List<ServiceDto>;

            }
            else
            {
                firstCombo = service;
            }

            model.ServiceFirstComboId = firstCombo != null ? firstCombo.Id : Guid.Empty;
            model.ServiceSecondcomboId = service.ServiceContainerId.HasValue ? service.Id : Guid.Empty;
            model.ServicesInContainer = list;
            model.AnonymousUser = payment.AnonymousUser.ToModel();
            model.RegisteredUser = payment.RegisteredUser;
            model.ReferenceName = payment.Service.ReferenceParamName;
            model.ReferenceName2 = payment.Service.ReferenceParamName2;
            model.ReferenceName3 = payment.Service.ReferenceParamName3;
            model.ReferenceName4 = payment.Service.ReferenceParamName4;
            model.ReferenceName5 = payment.Service.ReferenceParamName5;
            model.ReferenceName6 = payment.Service.ReferenceParamName6;
            model.ReferenceValue = payment.ReferenceValue;
            model.ReferenceValue2 = payment.ReferenceValue2;
            model.ReferenceValue3 = payment.ReferenceValue3;
            model.ReferenceValue4 = payment.ReferenceValue4;
            model.ReferenceValue5 = payment.ReferenceValue5;
            model.ReferenceValue6 = payment.ReferenceValue6;
            model.Description = payment.Description;
            model.TooltipeImage = payment.Service != null
                ? !string.IsNullOrEmpty(payment.Service.ImageTooltipUrl) ? payment.Service.ImageTooltipUrl : ""
                : "";
            model.TooltipeDesc = payment.Service.DescriptionTooltip;
            model.Departaments = dptos;
            model.Sucive =
                service.ServiceGatewaysDto.Any(
                    s =>
                        s.Gateway.Enum == (int)GatewayEnum.Sucive ||
                        s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active);
            model.LocationsCiu = locations;
            model.ServiceType = ServiceType(service);


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Service(Guid? serviceId)
        {
            Session[SessionConstants.PAYMENT_DATA] = null;
            var user = await CurrentSelectedUser();

            var services = user == null ? await _serviceClientService.GetServicesPaymentPublic() : await _serviceClientService.GetServicesPaymentPrivate();
            var deptos = GenerateDepartamentList();
            Guid id = serviceId == null ? default(Guid) : serviceId.Value;

            var model = new PaymentServiceModel
            {
                RegisteredUserId = user != null ? user.Id : Guid.Empty,
                AnonymousUser = user != null ? new AnonymousUserModel() : null,
                Services =
                    services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags })
                    .ToList()
                    .OrderBy(s => s.Name),
                ServiceFirstComboId = id,
                Departaments = deptos,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PayImport(FormCollection frm)
        {
            var serviceAssociatedId = Guid.Parse(frm["SerAssociatedIdToPay"]);

            Session[SessionConstants.PAYMENT_DATA] = null;
            var selectedUser = await CurrentSelectedUser();
            var nextpage = true;

            var serviceAssociated = await _serviceAssosiateClientService.Find(serviceAssociatedId);
            var service = await _serviceClientService.Find(serviceAssociated.ServiceDto.Id);
            var payment = new PaymentModel
            {
                ServiceId = serviceAssociated.ServiceDto.Id,
                Service = service,
                ReferenceName = serviceAssociated.ServiceDto.ReferenceParamName,
                ReferenceName2 = serviceAssociated.ServiceDto.ReferenceParamName2,
                ReferenceName3 = serviceAssociated.ServiceDto.ReferenceParamName3,
                ReferenceName4 = serviceAssociated.ServiceDto.ReferenceParamName4,
                ReferenceName5 = serviceAssociated.ServiceDto.ReferenceParamName5,
                ReferenceName6 = serviceAssociated.ServiceDto.ReferenceParamName6,
                ReferenceValue = serviceAssociated.ReferenceNumber,
                ReferenceValue2 = serviceAssociated.ReferenceNumber2,
                ReferenceValue3 = serviceAssociated.ReferenceNumber3,
                ReferenceValue4 = serviceAssociated.ReferenceNumber4,
                ReferenceValue5 = serviceAssociated.ReferenceNumber5,
                ReferenceValue6 = serviceAssociated.ReferenceNumber6,
                Description = serviceAssociated.Description,
                EnableMultipleBills = service.EnableMultipleBills,
                ServiceType = ServiceType(service),
                EnableBills = EnableBills(service),
                EnableImporte = EnableImporte(service)
            };
            payment.RegisteredUser = selectedUser;
            payment.RegisteredUserId = selectedUser.Id;

            var gateway = GetBestGateway(serviceAssociated.ServiceDto.ServiceGatewaysDto);

            if (payment.EnableImporte && !payment.EnableBills)
            {
                payment.GatewayEnum = GatewayEnumDto.Importe;
                payment.PaymentMethod = 2;
                ViewBag.CurrencyList = GenerateCurrencyPaymentStatusList();
            }

            if (payment.EnableBills)
            {
                payment.PaymentMethod = 1; // muestro las facturas primero

                if (gateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                {
                    serviceAssociated.ReferenceNumber = serviceAssociated.ReferenceNumber.Replace(" ", "");
                }
                var model = new PaymentServiceModel()
                {
                    ReferenceValue = serviceAssociated.ReferenceNumber,
                    ReferenceValue2 = serviceAssociated.ReferenceNumber2,
                    ReferenceValue3 = serviceAssociated.ReferenceNumber3,
                    ReferenceValue4 = serviceAssociated.ReferenceNumber4,
                    ReferenceValue5 = serviceAssociated.ReferenceNumber5,
                    ReferenceValue6 = serviceAssociated.ReferenceNumber6,
                };
                var bills = await GetBillsFromGateways(model, serviceAssociated.ServiceDto, payment);

                if (bills != null && bills.Any())
                {
                    bills.First().HasAnnualPatent = CheckSucivePatent(bills);
                }

                var allBills = bills != null ? bills.ToList() : new List<BillDto>();
                payment.BillsGatewayDto = gateway;
                payment.GatewayEnum = (GatewayEnumDto)gateway.Gateway.Enum;
                payment.Bills = allBills;
                payment.Description = serviceAssociated.Description;
                payment.AllBills = allBills;
                if (allBills.Any())
                {
                    payment.IdPadron = allBills.FirstOrDefault().IdPadron;
                    payment.ReferenceValue6 = allBills.FirstOrDefault().IdPadron.ToString();
                }

                var payable = allBills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();
                nextpage = payable.FirstOrDefault() != null ? payable.FirstOrDefault().Payable : false;
            }

            Session[SessionConstants.PAYMENT_DATA] = payment;

            ViewBag.Service = serviceAssociated.ServiceDto.Name;

            ViewBag.ReferenceValue = serviceAssociated.ReferenceNumber;
            ViewBag.ReferenceName = serviceAssociated.ServiceDto.ReferenceParamName;

            ViewBag.ReferenceValue2 = serviceAssociated.ReferenceNumber2;
            ViewBag.ReferenceName2 = serviceAssociated.ServiceDto.ReferenceParamName2;

            ViewBag.ReferenceValue3 = serviceAssociated.ReferenceNumber3;
            ViewBag.ReferenceName3 = serviceAssociated.ServiceDto.ReferenceParamName3;

            ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, serviceAssociated.ReferenceNumber4) : serviceAssociated.ReferenceNumber4;
            ViewBag.ReferenceName4 = serviceAssociated.ServiceDto.ReferenceParamName4;

            ViewBag.ReferenceValue5 = serviceAssociated.ReferenceNumber5;
            ViewBag.ReferenceName5 = serviceAssociated.ServiceDto.ReferenceParamName5;

            ViewBag.ReferenceValue6 = serviceAssociated.ReferenceNumber6;
            ViewBag.ReferenceName6 = serviceAssociated.ServiceDto.ReferenceParamName6;

            ViewBag.EnableBills = serviceAssociated.ServiceDto.EnableMultipleBills;

            //si hay pago por importe. cargo los valores ingresados, min y max
            var gateImporte = service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
            var dto = new PaymentBillModel()
            {
                Bills = payment.Bills != null ? payment.AllBills.Select(b => BillMapper.ToModel(b)).OrderBy(b => b.ExpirationDate).ToList() : null,
                EnablePartialPayment = payment.Service.EnablePartialPayment,
                EnableBills = payment.EnableBills,
                EnableImporte = payment.EnableImporte,
                EnableMultipleBills = payment.EnableMultipleBills,
                MinPeso = gateImporte != null && gateImporte.ReferenceId != null ? int.Parse(gateImporte.ReferenceId) : 0,
                MaxPeso = gateImporte != null && gateImporte.ServiceType != null ? int.Parse(gateImporte.ServiceType) : 0,
                MinD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData) : 0,
                MaxD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData2) : 0,
                NextPage = nextpage,
                PaymentMethod = payment.PaymentMethod,
            };

            return View("Bills", dto);
        }

        //Llamado desde afuera
        [HttpGet]
        public async Task<ActionResult> PaymentService(string serviceName)
        {
            try
            {
                var serviceId = Guid.Empty;
                Guid.TryParse(serviceName, out serviceId);

                var service = serviceId == Guid.Empty ? await _serviceClientService.GetServiceByUrlName(serviceName) : await _serviceClientService.Find(serviceId);
                if (service != null)
                {
                    Session[SessionConstants.PAYMENT_DATA] = new PaymentModel()
                    {
                        ServiceId = service.Id,
                        Service = service
                    };
                }

                return RedirectToAction("Service");
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return View("Service", new PaymentServiceModel
                {
                    AnonymousUser = new AnonymousUserModel(),
                    Departaments = GenerateDepartamentList()
                });
                throw;
            }
        }

        //[HttpGet]
        //public async Task<ActionResult> PaymentService(string service)
        //{
        //    try
        //    {
        //        Session[SessionConstants.PAYMENT_DATA] = null;
        //        //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER] = null;
        //        //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER_ID] = null;

        //        var serviceId = new Guid(service);

        //        var services = await _serviceClientService.ServicesForPayment(new ServiceFilterDto { DisplayLength = null, OnlyToPay = true });
        //        var deptos = GenerateDepartamentList();
        //        ServiceDto serviceDto = null;
        //        if (serviceId != default(Guid))
        //        {
        //            serviceDto = await _serviceClientService.Find(serviceId);
        //            ViewBag.CreditCard = serviceDto.CreditCard;
        //            ViewBag.DebitCard = serviceDto.DebitCard;
        //            ViewBag.CreditCardInternational = serviceDto.CreditCardInternational;
        //            ViewBag.DebitCardInternational = serviceDto.DebitCardInternational;

        //            ViewBag.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);
        //        }
        //        return View("Service", new PaymentServiceModel
        //        {
        //            ServiceFirstComboId = service == null ? Guid.Empty : serviceDto.Id,
        //            //Service = service == null ? null : serviceDto.Name,
        //            AnonymousUser = new AnonymousUserModel(),
        //            ReferenceName = service != null ? serviceDto.ReferenceParamName : "",
        //            ReferenceName2 = service != null ? serviceDto.ReferenceParamName2 : "",
        //            ReferenceName3 = service != null ? serviceDto.ReferenceParamName3 : "",
        //            ReferenceName4 = service != null ? serviceDto.ReferenceParamName4 : "",
        //            ReferenceName5 = service != null ? serviceDto.ReferenceParamName5 : "",
        //            ReferenceName6 = service != null ? serviceDto.ReferenceParamName6 : "",
        //            TooltipeImage = service != null ? serviceDto.ImageTooltip != null ? serviceDto.ImageTooltip.InternalName : "" : "",
        //            TooltipeDesc = service != null ? serviceDto.DescriptionTooltip : "",
        //            Sucive = service != null && serviceDto.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive || s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active),
        //            Departaments = deptos
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.LogEvent(e);
        //        return View("Service", new PaymentServiceModel
        //        {
        //            ServiceFirstComboId = Guid.Empty,
        //            //Service = service == null ? null : serviceDto.Name,
        //            AnonymousUser = new AnonymousUserModel(),
        //            Departaments = GenerateDepartamentList()
        //        });
        //        throw;
        //    }

        //}


        //Este metodo es para cuando vuelve desde el paso 3
        public async Task<ActionResult> Bills()
        {
            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

            if (payment == null)
                return RedirectToAction("Index", "Home");


            payment.Card = new CardDto();
            ViewBag.Service = payment.Service.Name;

            ViewBag.ReferenceValue = payment.ReferenceValue;
            ViewBag.ReferenceName = payment.ReferenceName;

            ViewBag.ReferenceValue2 = payment.ReferenceValue2;
            ViewBag.ReferenceName2 = payment.ReferenceName2;

            ViewBag.ReferenceValue3 = payment.ReferenceValue3;
            ViewBag.ReferenceName3 = payment.ReferenceName3;

            ViewBag.ReferenceValue4 = ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
            ViewBag.ReferenceName4 = payment.ReferenceName4;

            ViewBag.ReferenceValue5 = payment.ReferenceValue5;
            ViewBag.ReferenceName5 = payment.ReferenceName5;

            ViewBag.ReferenceValue6 = payment.ReferenceValue6;
            ViewBag.ReferenceName6 = payment.ReferenceName6;


            //if (payment.AllBills != null && payment.AllBills.Any())
            //{
            //    payment.AllBills.First().HasAnnualPatent = CheckSucivePatent(payment.AllBills);
            //}

            if (payment.AllBills == null)
            {
                var service = await _serviceClientService.Find(payment.ServiceId);
                var bills = await GetBillsFromGateways(new PaymentServiceModel()
                {
                    ReferenceValue = payment.ReferenceValue,
                    ReferenceValue2 = payment.ReferenceValue2,
                    ReferenceValue3 = payment.ReferenceValue3,
                    ReferenceValue4 = payment.ReferenceValue4,
                    ReferenceValue5 = payment.ReferenceValue5,
                    ReferenceValue6 = payment.ReferenceValue6
                }, service, payment);

                if (bills != null && bills.Any())
                {
                    bills.First().HasAnnualPatent = CheckSucivePatent(bills);
                }

                var allBills = bills != null ? bills.ToList() : new List<BillDto>();

                if (bills != null && bills.Any())
                {
                    bills.First().HasAnnualPatent = CheckSucivePatent(bills);
                    var billGatewayInt = (int)bills.FirstOrDefault().Gateway;
                    var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == billGatewayInt);
                    payment.BillsGatewayDto = gateway;
                    payment.GatewayEnum = (GatewayEnumDto)gateway.Gateway.Enum;
                }

                payment.Bills = allBills;
                payment.AllBills = allBills;
                if (allBills.Any())
                {
                    payment.IdPadron = allBills.FirstOrDefault().IdPadron;
                    payment.ReferenceValue6 = allBills.FirstOrDefault().IdPadron.ToString();
                }
            }


            //si hay pago por importe. cargo los valores ingresados, min y max
            var gateImporte = payment.Service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
            var payable = payment.AllBills == null ? true : (payment.AllBills.Any() ? payment.AllBills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).FirstOrDefault().Payable : false);

            //payment.AllBills es cargado por pasarelas distitnas a importe. si hay facturas, es porque hay mas de una pasarela
            var dto = new PaymentBillModel()
            {
                Bills = payment.AllBills != null && payment.AllBills.Any() ? payment.AllBills.Select(b => BillMapper.ToModel(b)).OrderBy(b => b.ExpirationDate).ToList() : null,
                EnablePartialPayment = payment.Service.EnablePartialPayment,
                EnableBills = EnableBills(payment.Service),
                EnableImporte = EnableImporte(payment.Service),
                EnableMultipleBills = payment.Service.EnableMultipleBills,
                MinPeso = gateImporte != null && gateImporte.ReferenceId != null ? int.Parse(gateImporte.ReferenceId) : 0,
                MaxPeso = gateImporte != null && gateImporte.ServiceType != null ? int.Parse(gateImporte.ServiceType) : 0,
                MinD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData) : 0,
                MaxD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData2) : 0,
                NextPage = payment.EnableImporte || payable,
                Currency = payment.PaymentMethod == 2 && payment.Bills != null && payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().Currency.Equals("UYU") ? 1 : 2 : 0,
                ImporteAmount = payment.PaymentMethod == 2 && payment.Bills != null && payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().Amount : 0,
                PaymentMethod = payment.PaymentMethod,
                DisableEditServicePage = payment.DisableEditServicePage
            };

            ViewBag.CurrencyList = GenerateCurrencyPaymentStatusList();

            Session[SessionConstants.PAYMENT_DATA] = payment;
            return View("Bills", dto);
        }

        private async Task<ICollection<BillDto>> GetBillsFromGateways(PaymentServiceModel model, ServiceDto service, PaymentModel payment)
        {
            //sucive y geocom, guardan idpadron en numero de referencia 6
            var user = await CurrentSelectedUser();

            var references = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(service.ReferenceParamName))
                references.Add(service.ReferenceParamName, model.ReferenceValue);

            if (!string.IsNullOrEmpty(service.ReferenceParamName2))
                references.Add(service.ReferenceParamName2, model.ReferenceValue2);

            if (!string.IsNullOrEmpty(service.ReferenceParamName3))
                references.Add(service.ReferenceParamName3, model.ReferenceValue3);

            if (!string.IsNullOrEmpty(service.ReferenceParamName4))
                references.Add(service.ReferenceParamName4, model.ReferenceValue4);

            if (!string.IsNullOrEmpty(service.ReferenceParamName5))
                references.Add(service.ReferenceParamName5, model.ReferenceValue5);

            if (!string.IsNullOrEmpty(service.ReferenceParamName6))
                references.Add(service.ReferenceParamName6, model.ReferenceValue6);

            if (user != null)
            {
                var userbills = await _billClientService.GetBillsForRegisteredUser(new RegisteredUserBillFilterDto
                {
                    ServiceId = service.Id,
                    References = references,
                    UserId = user.Id,
                    ScheduledPayment = false,
                });
                SetUpPaymentUser(payment, null);
                return userbills.Bills;
            }
            var auserbills = await _billClientService.GetBillsForAnonymousUser(new AnonymousUserBillFilterDto()
            {
                ServiceId = service.Id,
                References = references,
                AnonymousUserDto = new AnonymousUserDto()
                {
                    Id = model.AnonymousUser.Id,
                    Email = model.AnonymousUser.Email,
                    Name = model.AnonymousUser.Name,
                    Surname = model.AnonymousUser.Surname,
                    Address = model.AnonymousUser.Address,
                    IsPortalUser = true
                }
            });
            SetUpPaymentUser(payment, auserbills.User);
            return auserbills.Bills;
        }

        private bool CheckSucivePatent(IEnumerable<BillDto> bills)
        {
            var now = DateTime.Today;
            var patente = bills.Where(b => !string.IsNullOrEmpty(b.Description) && b.Description.Contains("PATENTE") && b.ExpirationDate.CompareTo(now) >= 0).OrderBy(b => b.ExpirationDate).GroupBy(b => b.Discount).ToList();
            //cuotas de patentes no vencidas

            foreach (var pat in patente)
            {
                //el año de la patente anual a pagar no puede ser menor a este año
                if (pat.Key >= now.Year)
                {
                    //para ser anual tienen que ser 6 cuotas
                    if (pat.Count() == 6)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async void SetUpPaymentUser(PaymentModel payment, AnonymousUserDto user)
        {
            var selectedUser = await CurrentSelectedUser();
            if (selectedUser != null)
            {
                payment.RegisteredUser = selectedUser;
                payment.RegisteredUserId = selectedUser.Id;
            }
            else
            {
                payment.AnonymousUserId = user.Id;
                payment.AnonymousUser = user;
                SetCurrentAnonymousUser(user);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Bills(PaymentServiceModel model)
        {
            ServiceDto service = null;
            ApplicationUserDto selectedUser = null;
            try
            {
                service = await _serviceClientService.Find(model.ServiceToPayId);

                if (service == null)
                    throw new WebApiClientBillBusinessException("Debe seleccionar un servicio para poder continuar con el proceso de pago");

                var nextpage = true;

                if (model.AskChildService)
                {
                    if (model.ServiceSecondcomboId == Guid.Empty)
                        throw new WebApiClientBillBusinessException("Debe seleccionar un servicio para poder continuar con el proceso de pago");
                }

                if (!service.AskUserForReferences || ModelState.IsValid)
                {
                    if (model.ServiceType != null && model.ServiceType.Equals("CIU") && !String.IsNullOrEmpty(model.ReferenceValue) ||
                         (String.IsNullOrEmpty(model.ReferenceName) || !String.IsNullOrEmpty(model.ReferenceValue)) &&
                         (String.IsNullOrEmpty(model.ReferenceName2) || !String.IsNullOrEmpty(model.ReferenceValue2)) &&
                         (String.IsNullOrEmpty(model.ReferenceName3) || !String.IsNullOrEmpty(model.ReferenceValue3)) &&
                         (String.IsNullOrEmpty(model.ReferenceName4) || !String.IsNullOrEmpty(model.ReferenceValue4)) &&
                         (String.IsNullOrEmpty(model.ReferenceName5) || !String.IsNullOrEmpty(model.ReferenceValue5)) &&
                         (String.IsNullOrEmpty(model.ReferenceName6) || !String.IsNullOrEmpty(model.ReferenceValue6)))
                    {
                        #region TrimValues
                        model.TrimAllStringsProperties();
                        model.AnonymousUser.TrimAllStringsProperties();
                        model.LowerAllStringsProperties();
                        #endregion

                        if (model.AnnualSucive)
                        {
                            var cardModel = await GenerateAnnualPatent(service, model);
                            if (cardModel != null)
                            {
                                if (cardModel.Bills.FirstOrDefault().SucivePreBillNumber.Equals("-1"))
                                {
                                    TempData["BillsInvalid"] = cardModel.Bills.FirstOrDefault().Description;
                                }
                                else
                                {
                                    ViewBag.Cards = new List<CardDto>();
                                    return View("Card", cardModel);
                                }
                            }
                        }

                        selectedUser = await CurrentSelectedUser();
                        var payment = new PaymentModel
                        {
                            ServiceId = model.ServiceToPayId,
                            Service = service,
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
                            Description = model.Description,
                            EnableMultipleBills = service.EnableMultipleBills,
                            ServiceType = model.ServiceType,
                            EnableBills = EnableBills(service),
                            EnableImporte = EnableImporte(service)
                        };

                        var bills = await GetBillsFromGateways(model, service, payment);

                        if (payment.EnableImporte && !payment.EnableBills)
                        {
                            payment.GatewayEnum = GatewayEnumDto.Importe;
                            payment.PaymentMethod = 2;
                            ViewBag.CurrencyList = GenerateCurrencyPaymentStatusList();
                        }

                        if (payment.EnableBills)
                        {
                            payment.PaymentMethod = 1; // muestro las facturas primero

                            if (bills != null && bills.Any())
                            {
                                bills.First().HasAnnualPatent = CheckSucivePatent(bills);
                                var billGatewayInt = (int)bills.FirstOrDefault().Gateway;
                                var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == billGatewayInt);
                                payment.BillsGatewayDto = gateway;
                                payment.GatewayEnum = (GatewayEnumDto)gateway.Gateway.Enum;
                            }

                            var allBills = bills != null ? bills.ToList() : new List<BillDto>();
                            payment.Bills = allBills;
                            payment.Description = model.Description;
                            payment.AllBills = allBills;
                            if (allBills.Any())
                            {
                                payment.IdPadron = allBills.FirstOrDefault().IdPadron;
                                payment.ReferenceValue6 = allBills.FirstOrDefault().IdPadron.ToString();
                            }

                            var payable = allBills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();
                            nextpage = payable.FirstOrDefault() != null ? payable.FirstOrDefault().Payable : false;
                        }

                        Session[SessionConstants.PAYMENT_DATA] = payment;

                        ViewBag.Service = service.Name;

                        ViewBag.ReferenceValue = model.ReferenceValue;
                        ViewBag.ReferenceName = service.ReferenceParamName;

                        ViewBag.ReferenceValue2 = model.ReferenceValue2;
                        ViewBag.ReferenceName2 = service.ReferenceParamName2;

                        ViewBag.ReferenceValue3 = model.ReferenceValue3;
                        ViewBag.ReferenceName3 = service.ReferenceParamName3;

                        ViewBag.ReferenceValue4 = model.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, model.ReferenceValue4) : model.ReferenceValue4;
                        ViewBag.ReferenceName4 = service.ReferenceParamName4;

                        ViewBag.ReferenceValue5 = model.ReferenceValue5;
                        ViewBag.ReferenceName5 = service.ReferenceParamName5;

                        ViewBag.ReferenceValue6 = model.ReferenceValue6;
                        ViewBag.ReferenceName6 = service.ReferenceParamName6;

                        ViewBag.EnableBills = service.EnableMultipleBills;

                        //si hay pago por importe. cargo los valores ingresados, min y max
                        var gateImporte = payment.Service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
                        var dto = new PaymentBillModel()
                        {
                            Bills = payment.Bills != null ? payment.AllBills.Select(b => BillMapper.ToModel(b)).OrderBy(b => b.ExpirationDate).ToList() : null,
                            EnablePartialPayment = payment.Service.EnablePartialPayment,
                            EnableBills = payment.EnableBills,
                            EnableImporte = payment.EnableImporte,
                            EnableMultipleBills = service.EnableMultipleBills,
                            MinPeso = gateImporte != null && gateImporte.ReferenceId != null ? int.Parse(gateImporte.ReferenceId) : 0,
                            MaxPeso = gateImporte != null && gateImporte.ServiceType != null ? int.Parse(gateImporte.ServiceType) : 0,
                            MinD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData) : 0,
                            MaxD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData2) : 0,
                            NextPage = nextpage,
                            PaymentMethod = payment.PaymentMethod,
                        };

                        return View("Bills", dto);
                    }

                    //Falta ingresar algun numero de referencia
                    ViewBag.ReferenceNumbersInvalid = PresentationWebStrings.Bills_All_References;
                }
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
            }
            catch (WebApiClientBillBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
            }

            var services = selectedUser == null ? await _serviceClientService.GetServicesPaymentPublic() : await _serviceClientService.GetServicesPaymentPrivate();
            model.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);
            if (model.ServiceToPayId != Guid.Empty)
            {
                service = await _serviceClientService.Find(model.ServiceToPayId);
                model.Sucive = service.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive || s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active);
                //ViewBag.CreditCard = service.CreditCard;
                //ViewBag.DebitCard = service.DebitCard;
                //ViewBag.CreditCardInternational = service.CreditCardInternational;
                //ViewBag.DebitCardInternational = service.DebitCardInternational;

                var locations = await LoadLocation(service);
                model.LocationsCiu = locations;
                model.ServiceType = ServiceType(service);

                ViewBag.EnableBills = service.EnableMultipleBills;
            }

            var deptos = GenerateDepartamentList();
            model.Departaments = deptos;

            var firstCombo = await _serviceClientService.Find(model.ServiceFirstComboId);
            if (firstCombo != null && firstCombo.Container)
            {
                var servicesAux = await _serviceClientService.FindAll(new ServiceFilterDto()
                {
                    ServiceContainerId = firstCombo.Id.ToString()
                });
                if (servicesAux != null)
                    model.ServicesInContainer = servicesAux.OrderBy(x => x.Name).ToList();
            }

            var user = await CurrentSelectedUser();
            model.RegisteredUserId = user != null ? user.Id : Guid.Empty;

            return View("Service", model);
        }

        public async Task<ActionResult> BillDetail(Guid id)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var title = "Error";
            var message = "Error al obtener el detalle de la factura.";

            try
            {
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

                var bill = payment.AllBills.FirstOrDefault(x => x.Id == id);

                if (bill == null)
                    Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);

                var billaux = new List<BillDto> { bill };
                billaux.AddRange(bill.Bills);

                var content = RenderPartialViewToString("_DetailBill", billaux);

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                title = "Correcto";
                message = "Correcto";

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "Paymentcontroller - BillDetail - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Metodo que indica si se esta pagando las facturas mas viejas
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool CheckBillsOrder(IEnumerable<BillModel> list)
        {
            if (list.Count() == 1) return true;

            bool spectedResult = false;
            foreach (var billModel in list.OrderByDescending(b => b.ExpirationDate))
            {
                if (billModel.Pay)
                {
                    spectedResult = true;
                }
                else
                {
                    if (spectedResult)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Las facturas tiene que estar filtradas entre las marcadas a pagar.
        /// Comparo las marcadas a pagar contra las facturas en la seasion y veo si estan habilitadas para pagar
        /// </summary>
        /// <param name="list"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool CheckBillsSelected(IEnumerable<BillModel> list, IEnumerable<BillDto> memory)
        {
            if (!list.Any()) return false;
            if (list.Count() == 1)
            {
                var bill = list.First();
                var billMemory = memory.FirstOrDefault(b => bill.Line == b.Line);
                if (billMemory != null && billMemory.Payable)
                    return true;
            }

            foreach (var billModel in list)
            {
                var billMemory = memory.FirstOrDefault(b => billModel.Line == b.Line);
                if (billMemory == null || !billMemory.Payable)
                    return false;
            }

            return true;

        }

        //Este metodo es para cuando fallan los datos de la tarjeta
        public async Task<ActionResult> Card()
        {
            try
            {
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
                if (payment == null)
                    return RedirectToAction("Index", "Home");
                ViewBag.Service = payment.Service.Name;

                ViewBag.ReferenceValue = payment.ReferenceValue;
                ViewBag.ReferenceName = payment.ReferenceName;

                ViewBag.ReferenceValue2 = payment.ReferenceValue2;
                ViewBag.ReferenceName2 = payment.ReferenceName2;

                ViewBag.ReferenceValue3 = payment.ReferenceValue3;
                ViewBag.ReferenceName3 = payment.ReferenceName3;

                ViewBag.ReferenceValue4 = ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
                ViewBag.ReferenceName4 = payment.ReferenceName4;

                ViewBag.ReferenceValue5 = payment.ReferenceValue5;
                ViewBag.ReferenceName5 = payment.ReferenceName5;

                ViewBag.ReferenceValue6 = payment.ReferenceValue6;
                ViewBag.ReferenceName6 = payment.ReferenceName6;

                var dolar = payment.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount);
                ViewBag.TotalDolars = dolar;
                var pesos = payment.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount);
                ViewBag.TotalPesos = pesos;

                ViewBag.MerchantId = payment.Service.MerchantId.Trim();
                ViewBag.CsEnvironment = ConfigurationManager.AppSettings["CsEnvironment"];

                //Se tiene que realizar luego de aplicar el descuento, en el momento de pagar.
                //LoadKeysForCybersource(payment.Service, RedirectEnums.PublicPayment.ToString("D"), pesos.ToString());

                Session[SessionConstants.PAYMENT_DATA] = payment;

                await LoadCardViewBag(payment);

                var model = new CardModel()
                {
                    Bills = payment.Bills.ToList(),
                };

                return View("Card", model);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Card(PaymentBillModel dataModel)
        {
            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
            payment.Bills = null;
            var correct = false;
            var exit = false;
            var errorString = "";
            try
            {
                if (dataModel.PaymentMethod == 2)
                {
                    if (!ModelState.IsValid)
                    {
                        exit = true;
                    }
                }
                if (!exit)
                {
                    payment.PaymentMethod = dataModel.PaymentMethod;

                    if (dataModel.PaymentMethod == 1)
                    {
                        if (payment.Service != null)
                        {
                            if (!payment.Service.EnableMultipleBills)
                            {
                                if (dataModel.Bills.Count(m => m.Pay) > 1)
                                {
                                    TempData["BillsInvalid"] = "El servicio no permite seleccion multiple de facturas";
                                    return RedirectToAction("Bills");
                                }
                            }
                        }
                        if (!dataModel.Bills.Any(b => b.Pay))
                        {
                            TempData["BillsInvalid"] = "Debe seleccionar al menos una factura";
                            return RedirectToAction("Bills");
                        }
                        var currency = dataModel.Bills.Where(b => b.Pay).Select(b => b.Currency).Distinct();
                        if (currency.Count() > 1)
                        {
                            TempData["BillsInvalid"] = "Debe seleccionar facturas de misma moneda";
                            return RedirectToAction("Bills");
                        }
                    }

                    //var refId = payment.Service.ServiceGatewaysDto.First(x => x.Gateway.Enum == (int)payment.GatewayEnum).ReferenceId;
                    if (dataModel.PaymentMethod == 1)
                    {
                        var selected = dataModel.Bills.Where(b => b.Pay).ToList();

                        if (payment.GatewayEnum == GatewayEnumDto.Sucive || payment.GatewayEnum == GatewayEnumDto.Geocom)
                        {
                            //var lines = dataModel.Bills.Where(m => m.Pay).Select(m => m.Line).ToList();
                            //var line = String.Join("", lines);
                            //var preBill = await _billClientService.ChekBills(line, payment.IdPadron, (int)payment.Service.Departament, payment.GatewayEnum, refId);
                            var preBill = await _billClientService.GeneratePreBill(new GeneratePreBillDto()
                            {
                                SelectedBills = selected.Select(x => new BillDto()
                                {
                                    Line = x.Line,
                                    IdPadron = payment.IdPadron,
                                    Gateway = x.Gateway
                                }).ToList(),
                                ServiceId = payment.ServiceId
                            });
                            //si tiene SucivePreBillNumber -1 hay un error
                            if (preBill.SucivePreBillNumber.Equals("-1"))
                            {
                                errorString = preBill.Description;
                            }
                            else
                            {
                                correct = true;
                                payment.Bills = new List<BillDto>() { preBill };
                            }
                        }
                        else
                        {

                            correct = CheckBillsSelected(selected, payment.AllBills.Where(b => b.Payable));
                            selected.ForEach(x => x.Description = HttpUtility.HtmlDecode(x.Description));
                            if (payment.Service.EnableMultipleBills)
                            {
                                payment.Bills = payment.AllBills.Where(b => selected.Select(x => x.Id).Contains(b.Id)).ToList();
                            }
                            else
                            {
                                payment.Bills = payment.AllBills.Where(b => b.Id == selected.First().Id).ToList();
                            }
                        }
                    }
                    else if (dataModel.PaymentMethod == 2)
                    {
                        payment.Bills = new List<BillDto>()
                                                        {
                                            await _billClientService.GetInputAmountBill(new InputAmountBillFilterDto()
                                                                                        {
                                                                                            Amount =
                                                                                                dataModel.ImporteAmount,
                                                                                            Currency =
                                                                                                (CurrencyDto)
                                                                                                dataModel.Currency
                                                                                        }
                                                )
                                        };
                        payment.BillsGatewayDto = payment.Service.ServiceGatewaysDto.First(x => x.Gateway.Enum == (int)payment.GatewayEnum);
                        correct = true;
                    }

                    if (correct)
                    {
                        ViewBag.Service = payment.Service.Name;

                        ViewBag.ReferenceValue = payment.ReferenceValue;
                        ViewBag.ReferenceName = payment.ReferenceName;

                        ViewBag.ReferenceValue2 = payment.ReferenceValue2;
                        ViewBag.ReferenceName2 = payment.ReferenceName2;

                        ViewBag.ReferenceValue3 = payment.ReferenceValue3;
                        ViewBag.ReferenceName3 = payment.ReferenceName3;

                        ViewBag.ReferenceValue4 = ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
                        ViewBag.ReferenceName4 = payment.ReferenceName4;

                        ViewBag.ReferenceValue5 = payment.ReferenceValue5;
                        ViewBag.ReferenceName5 = payment.ReferenceName5;

                        ViewBag.ReferenceValue6 = payment.ReferenceValue6;
                        ViewBag.ReferenceName6 = payment.ReferenceName6;

                        var dolar = payment.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount);
                        ViewBag.TotalDolars = dolar;
                        var pesos = payment.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount);
                        ViewBag.TotalPesos = pesos;

                        await LoadCardViewBag(payment);

                        ViewBag.MerchantId = payment.Service.MerchantId.Trim();
                        ViewBag.CsEnvironment = ConfigurationManager.AppSettings["CsEnvironment"];

                        Session[SessionConstants.PAYMENT_DATA] = payment;

                        var model = new CardModel()
                        {
                            Bills = payment.Bills.ToList(),
                        };

                        return View("Card", model);
                    }
                    TempData["BillsInvalid"] = String.IsNullOrEmpty(errorString) ? "Debe pagar la factura mas vieja primero." : errorString;
                }
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }

            ViewBag.Service = payment.Service.Name;

            ViewBag.ReferenceValue = payment.ReferenceValue;
            ViewBag.ReferenceName = payment.Service.ReferenceParamName;

            ViewBag.ReferenceValue2 = payment.ReferenceValue2;
            ViewBag.ReferenceName2 = payment.Service.ReferenceParamName2;

            ViewBag.ReferenceValue3 = payment.ReferenceValue3;
            ViewBag.ReferenceName3 = payment.Service.ReferenceParamName3;

            ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
            ViewBag.ReferenceName4 = payment.Service.ReferenceParamName4;

            ViewBag.ReferenceValue5 = payment.ReferenceValue5;
            ViewBag.ReferenceName5 = payment.Service.ReferenceParamName5;

            ViewBag.ReferenceValue6 = payment.ReferenceValue6;
            ViewBag.ReferenceName6 = payment.Service.ReferenceParamName6;

            ViewBag.EnableBills = payment.Service.EnableMultipleBills;

            dataModel.EnableBills = payment.EnableBills;
            dataModel.EnableImporte = payment.EnableImporte;
            dataModel.EnableMultipleBills = payment.Service.EnableMultipleBills;
            dataModel.NextPage = true;
            dataModel.DisableEditServicePage = payment.DisableEditServicePage;
            ViewBag.CurrencyList = GenerateCurrencyPaymentStatusList();
            return View("Bills", dataModel);
        }

        public async Task<ActionResult> FinalBillDetail(Guid id)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var title = "Error";
            var message = "Error al obtener el detalle de la factura.";

            try
            {
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

                var bill = payment.Bills.FirstOrDefault(x => x.Id == id);

                if (bill == null)
                    Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);


                var content = RenderPartialViewToString("_DetailFinalBill", bill);

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                title = "Correcto";
                message = "Correcto";

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "Paymentcontroller - BillDetail - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetService(PaymentServiceModel model)
        {
            ModelState.Clear();
            try
            {
                var paymentSession = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
                ServiceDto serviceToUse = null;
                var askForReferences = false;

                if (model.ServiceFirstComboId != Guid.Empty)
                {
                    ServiceDto firstCombo = await _serviceClientService.Find(model.ServiceFirstComboId);
                    ServiceDto secondCombo = null;
                    askForReferences = firstCombo.AskUserForReferences;

                    if (model.ServiceSecondcomboId != Guid.Empty)
                    {
                        secondCombo = await _serviceClientService.Find(model.ServiceSecondcomboId);
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

                        if (service.Container && service.AllowSelectContentPayment)
                        {
                            model.ServiceSecondcomboId = Guid.Empty;
                            var servicesAux = await _serviceClientService.FindAll(new ServiceFilterDto()
                            {
                                ServiceContainerId =
                                    service.Id.ToString()
                            });
                            if (servicesAux != null)
                            {
                                model.ServicesInContainer = servicesAux.OrderBy(x => x.Name).ToList();
                            }

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

                        LoadServiceModelReferences(model, firstCombo, secondCombo);

                        model.TooltipeImage = !string.IsNullOrEmpty(model.ServiceToPay.ImageTooltipUrl) ? model.ServiceToPay.ImageTooltipUrl : "";
                        model.TooltipeDesc = model.ServiceToPay.DescriptionTooltip;

                        model.Departaments = GenerateDepartamentList();
                        model.Sucive = model.ServiceToPay.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive && s.Active);

                        model.LocationsCiu = await LoadLocation(service);
                        model.ServiceType = ServiceType(service);

                        if (paymentSession != null)
                        {
                            model.ReferenceValue = paymentSession.ReferenceValue;
                            model.ReferenceValue2 = paymentSession.ReferenceValue2;
                            model.ReferenceValue3 = paymentSession.ReferenceValue3;
                            model.ReferenceValue4 = paymentSession.ReferenceValue4;
                            model.ReferenceValue5 = paymentSession.ReferenceValue5;
                            model.ReferenceValue6 = paymentSession.ReferenceValue6;
                        }

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

        private void LoadServiceModelReferences(PaymentServiceModel model, ServiceDto firstComboService, ServiceDto secondComboService)
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

        public async Task<ActionResult> TokengenerationCallBack()
        {

            try
            {
                var dictionaryData = GenerateDictionary(Request.Form);
                //VER: se está perdiendo la Descripcion que completa el usuario que serviría para cuando es registrado
                //y no tiene el servicio asociado para que se asocie con esa descripción (solucion provisoria: ocultar el campo)
                var paymentResult = await _paymentClientService.NotifyPayment(dictionaryData);

                if (paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    Session[SessionConstants.PAYMENT_DATA] = null;
                    if (paymentResult.NewPaymentDto != null)
                    {
                        _paymentClientService.NotifyExternalSourceNewPayment(paymentResult.NewPaymentDto);
                        return await ConfirmationView(paymentResult.NewPaymentDto);
                    }
                    else
                    {
                        return RedirectToAction("NotificationError");
                    }
                }

                return EvaluateErrors(paymentResult.CyberSourceOperationData.PaymentData);
            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "TokengenerationCallBack - PaymenController - WebApiClientBusinessException");
                NLogLogger.LogEvent(ex);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "TokengenerationCallBack - PaymenController - WebApiClientFatalException");
                NLogLogger.LogEvent(ex);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "TokengenerationCallBack - PaymenController - Exception");
                NLogLogger.LogEvent(ex);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            return RedirectToAction("NotificationError");
        }

        private ActionResult EvaluateErrors(CsResponseData csResponseData)
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
                        return InvalidCardData(csResponseData);

                    case CybersourceMsg.AVSCheckInvalid:
                    case CybersourceMsg.UserCybersourceError:
                    case CybersourceMsg.PayerAuthenticationNotAuthenticated:
                    case CybersourceMsg.AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings:
                    case CybersourceMsg.ConfigurationKeysInvalids:
                        return RedirectToAction("NotificationError");
                }
            }

            return RedirectToAction("NotificationError");
        }

        private ActionResult InvalidCardData(CsResponseData csResponseData)
        {
            ShowNotification(csResponseData.PaymentResponseMsg, NotificationType.Info);
            return RedirectToAction("Card");
        }

        public ActionResult NotificationError()
        {
            Session[SessionConstants.PAYMENT_DATA] = null;
            return View();
        }

        private ActionResult GetError(CybersourceMsg reasonCode, PaymentModel payment)
        {
            NLogLogger.LogEvent(NLogType.Error, "Payment controller - GetError - Reason code : " + (int)reasonCode);

            var strLog = string.Format(LogStrings.Payment_cybersourceCallback_error,
                reasonCode + " " + (int)reasonCode,
                payment.AnonymousUser.Email,
                payment.AnonymousUser.Name,
                payment.AnonymousUser.Surname,
                payment.Service.Name,
                payment.Service.MerchantId);


            _logClientService.Put(new LogModel
            {
                LogType = LogType.Error,
                LogUserType = LogUserType.NoRegistered,
                LogCommunicationType = LogCommunicationType.VisaNet,
                LogOperationType = LogOperationType.Cybersource,
                Message = strLog,
            });

            return RedirectToAction("NotificationError");
        }

        private async Task<ViewResult> ConfirmationView(PaymentDto dto)
        {
            var discountMessage = "Descuento: ";
            if (dto.DiscountObj != null)
            {
                discountMessage =
                    String.Concat(
                        EnumHelpers.GetName(typeof(DiscountLabelTypeDto), (int)dto.DiscountObj.DiscountLabel,
                            EnumsStrings.ResourceManager), ": ");
            }

            //_logClientService.Put(new LogAnonymousModel
            //{
            //    AnonymousUserId = dto.AnonymousUserId.Value,
            //    LogType = LogType.Info,
            //    LogCommunicationType = LogCommunicationType.VisaNet,
            //    Message = string.Format(LogStrings.Payment_Done,
            //                                dto.AnonymousUser.Email,
            //                                dto.AnonymousUser.Name,
            //                                dto.AnonymousUser.Surname,
            //                                dto.ServiceDto.Name,
            //    dto.TransactionNumber)
            //});

            var confModel = new ConfirmationModel();

            if (dto.RegisteredUserId.HasValue && dto.RegisteredUserId != Guid.Empty)
            {
                confModel = await ConfirmationRegistered(dto);
            }
            else
            {
                confModel = new ConfirmationModel()
                {
                    Id = dto.Id,
                    AnonymousUserId = dto.AnonymousUserId.HasValue ? dto.AnonymousUserId.Value : Guid.Empty,
                    Email = dto.AnonymousUser != null ? dto.AnonymousUser.Email : "",
                    Transaction = dto.TransactionNumber,
                    Date = dto.Date.ToString("dd/MM/yyyy"),
                    Hrs = dto.Date.ToString("t"),
                    Amount = dto.Bills != null ? dto.Bills.Any() ? dto.Bills.Sum(b => b.Amount).ToString("##,#0.00", CultureInfo.CurrentCulture) : "" : "",
                    Currency = dto.Currency,
                    Discount = dto.Discount,
                    DiscountApplyed = dto.DiscountApplyed,
                    TotalAmount = dto.TotalAmount,
                    TotalTaxedAmount = dto.TotalTaxedAmount,
                    Mask = dto.Card != null ? dto.Card.MaskedNumber : "",
                    ServiceName = dto.ServiceDto != null ? dto.ServiceDto.Name : "",
                    References = new Dictionary<string, string>(),
                    AllowsAutomaticPayment = false,
                    Quotas = dto.Quotas
                };
                NLogLogger.LogEvent(NLogType.Info, "confModel anonimo " + (dto.AnonymousUserId.HasValue ? dto.AnonymousUserId.Value : Guid.Empty));
            }

            confModel.DiscountTypeText = dto.DiscountApplyed ? discountMessage : "No aplica descuento Ley de Inclusión Financiera (19.210).";

            if (confModel.Currency != null && confModel.Currency.Equals(Currency.PESO_URUGUAYO))
                confModel.Currency = "$";
            if (confModel.Currency != null && confModel.Currency.Equals(Currency.DOLAR_AMERICANO))
                confModel.Currency = "U$S";

            if (dto.ServiceDto != null)
            {
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName, dto.ReferenceNumber);
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName2))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName2, dto.ReferenceNumber2);
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName3))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName3, dto.ReferenceNumber3);
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName4))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName4, dto.ReferenceNumber4);
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName5))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName5, dto.ReferenceNumber5);
                if (!String.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName6))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName6, dto.ReferenceNumber6);
            }
            return View("PaymentConfirm", confModel);
        }

        private async Task<ConfirmationModel> ConfirmationRegistered(PaymentDto dto)
        {
            var hasAutPay = false;
            if (dto.ServiceAssociatedId.HasValue)
            {
                var serviceAssociatedDto = await _serviceAssosiateClientService.Find(dto.ServiceAssociatedId.Value);
                hasAutPay = serviceAssociatedDto != null && serviceAssociatedDto.AutomaticPaymentDtoId.HasValue;
            }
            var confModel = new ConfirmationModel()
            {
                Id = dto.Id,
                Email = dto.RegisteredUser != null ? dto.RegisteredUser.Email : "",
                Transaction = dto.TransactionNumber,
                Date = dto.Date.ToString("dd/MM/yyyy"),
                Hrs = dto.Date.ToString("t"),
                Amount = dto.Bills != null ? dto.Bills.Any() ? dto.Bills.Sum(b => b.Amount).ToString("##,#0.00", CultureInfo.CurrentCulture) : "" : "",
                Currency = dto.Currency,
                Discount = dto.Discount,
                DiscountApplyed = dto.DiscountApplyed,
                TotalAmount = dto.TotalAmount,
                TotalTaxedAmount = dto.TotalTaxedAmount,
                Mask = dto.Card != null ? dto.Card.MaskedNumber : "",
                ServiceName = dto.ServiceDto != null ? dto.ServiceDto.Name : "",
                References = new Dictionary<string, string>(),
                ServiceAssociatedId = dto.ServiceAssociatedId,

                AllowsAutomaticPayment = dto.ServiceDto != null && dto.ServiceDto.EnableAutomaticPayment,
                AlreadyHasAutomaticPayment = hasAutPay,

                Quotas = dto.Quotas,
            };

            return confModel;
        }

        public async Task<ActionResult> DownloadTicket(Guid id, string transactionNumber)
        {

            var anonymousUser = CurrentAnonymousUser();
            var applicationUser = await CurrentSelectedUser();

            var arrbytes = await _paymentClientService.DownloadTicket(id, transactionNumber, applicationUser != null ?
                applicationUser.Id : anonymousUser.Id);

            return File(arrbytes, "application/PDF", string.Format("Ticket_{0}.pdf", transactionNumber));
        }

        [HttpGet]
        public async Task<ActionResult> ValidationBIN(int maskedNumber)
        {
            try
            {
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

                var result = await _discountClientService.ValidateBin(maskedNumber, payment.ServiceId);

                var msg = result ? PresentationWebStrings.Bin_Not_Defined : string.Empty;
                var respType = result ? AjaxResponse.Success : AjaxResponse.BusinessError;

                return Json(new JsonResponse(respType, result, msg, string.Empty,
                                NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                NLogLogger.LogEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, false, PresentationWebStrings.Payment_General_Error, string.Empty, NotificationType.Info), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public async Task<ActionResult> NewCardValidationAndDiscount(int maskedNumber, string fpProfiler, string nameTh)
        {
            try
            {
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

                CyberSourceKeyModel cybersourceData = null;
                try
                {
                    cybersourceData = await LoadKeysForCybersource(payment, RedirectEnums.Payment, maskedNumber, Guid.Empty, fpProfiler, nameTh);
                    //error en bin
                    if (cybersourceData == null)
                    {
                        return Json(new JsonResponse(AjaxResponse.BusinessError, "", ExceptionMessages.BIN_NOTVALID_FOR_SERVICE, PresentationCoreMessages.Notification_Title_Alert, NotificationType.Info));
                    }
                    if (cybersourceData.TotalAmount <= 0)
                    {
                        return
                               Json(new JsonResponse(AjaxResponse.Error, "", "El monto a pagar no puede ser igual o menor a 0.",
                                   PresentationCoreMessages.NotificationFail, NotificationType.Error));
                    }
                }
                catch (WebApiClientBusinessException exception)
                {
                    NLogLogger.LogEvent(exception);
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", exception.Message,//PresentationWebStrings.Payment_General_Error,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
                catch (WebApiClientFatalException exception)
                {
                    NLogLogger.LogEvent(exception);
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(e);
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", PresentationCoreMessages.Discount_Error,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
                var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData.Keys);
                Session[SessionConstants.PAYMENT_DATA] = payment;

                var discountType = "Descuento: ";
                if (payment.DiscountObj != null)
                {
                    discountType = payment.DiscountObj.DiscountLawDescription + ": ";
                }

                return
                    Json(new JsonResponse(AjaxResponse.Success,
                        new
                        {
                            keys = content,
                            Service = payment.Service.Name,
                            cybersourceData.Currency,
                            TotalAmount = cybersourceData.TotalAmount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                            Discount = cybersourceData.Discount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                            TotalAfterDiscount = (cybersourceData.TotalAmount - cybersourceData.Discount).SignificantDigits(2).ToString("##,#0.00", CultureInfo.CurrentCulture),
                            //DiscountType = payment.DiscountApplyed ? discountType : "No aplica descuento Ley de Inclusión Financiera (19.210).",
                            DiscountType = payment.DiscountApplyed ? discountType : string.Empty,
                        }, "", "", NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogEvent(ex);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogEvent(ex);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        public async Task<int> CheckPaymentModel()
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "Metodo CheckPaymentModel - Llamada");
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
                var user = await CurrentSelectedUser();
                var tempId = Guid.Parse((string)Session[SessionConstants.TEMPORARY_ID]);
                if (payment != null)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Metodo CheckPaymentModel - Hay un pago en sesion. Devuelvo 1");
                    //Creo nuevo log
                    await _logClientService.Put(tempId, new LogDto
                    {
                        LogCommunicationType = LogCommunicationType.CyberSource,
                        LogType = LogType.Info,
                        LogOperationType = LogOperationType.BillPayment,
                        CallCenterMessage = string.Format("Inicia comunicación a CS para pago de servicio {0}", payment.Service.Name),
                        Message = string.Format("Inicia comunicación a CS para pago de servicio {0}", payment.Service.Name),
                        ApplicationUserId = user != null ? user.Id : Guid.Empty,
                        AnonymousUserId = user == null ? payment.AnonymousUserId : Guid.Empty,
                        LogUserType = user != null ? LogUserType.Registered : LogUserType.NoRegistered,
                    });
                    return (int)AjaxResponse.Success;
                }
            }
            catch (Exception e)
            {

            }
            NLogLogger.LogEvent(NLogType.Info, "Metodo CheckPaymentModel - NO HAY PAGO. DEVUELVO 0");
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

        private void NotifyError(int reasonCode, string merchantId)
        {
            var msg = String.Format("Se produjo un error en cybersouce. Reason code :{0}, MerchantId: {1}", reasonCode, merchantId);
            _paymentClientService.NotifyError(JsonConvert.SerializeObject(new
            {
                Message = msg,
                Title = "Error en pago de Cybersource",
            }));

        }

        private void NotifyErrorCantCancel(string requestId)
        {
            var msg = String.Format("Se produjo un error en el portal, no se pudo notificar al ente ni cancelar el pago en Cybersource. Request ID:{0}, Fecha: {1}", requestId, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            _paymentClientService.NotifyError(JsonConvert.SerializeObject(new
            {
                Message = msg,
                Title = "Error en PORTAL. NO SE PUDO CANCELAR EN CYBERSOURCE",
            }));

        }
        private void NotifyErrorCsWithData()
        {
            var msg = String.Format("Se produjo un error en el portal, no se pudo cargar los datos de Cybersource. No se puede determinar si se realizo una transacción Fecha: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            _paymentClientService.NotifyError(JsonConvert.SerializeObject(new
            {
                Message = msg,
                Title = "Error en PORTAL. NO SE PUDO CARGAR LOS DATOS DE CYBERSOURCE",
            }));

        }
        private async Task<List<SelectListItem>> LoadLocation(ServiceDto service)
        {
            if (service != null && service.ServiceGatewaysDto != null && service.ServiceGatewaysDto.Any())
            {
                var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Active && x.Gateway.Enum == (int)GatewayEnum.Geocom);
                if (gateway != null && gateway.ReferenceId.Equals("CIU"))
                {
                    var locations = await _locationClientService.GetList(new LocationFilterDto()
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

        private async Task<String> LoadLocationName(ServiceDto service, string value)
        {
            if (service != null && service.ServiceGatewaysDto != null && service.ServiceGatewaysDto.Any())
            {
                var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Active && x.Gateway.Enum == (int)GatewayEnum.Geocom);
                if (gateway != null && gateway.ReferenceId.Equals("CIU"))
                {
                    var locations = await _locationClientService.GetList(new LocationFilterDto()
                    {
                        DepartamentDtoType = service.Departament,
                        GatewayEnumDto = (GatewayEnumDto)gateway.Gateway.Enum,
                    });
                    var firstOrDefault = locations.FirstOrDefault(x => x.Value.Equals(value));
                    if (firstOrDefault != null)
                        return firstOrDefault.Name;
                }
            }
            return null;
        }

        private bool EnableImporte(ServiceDto service)
        {
            if (service.ServiceGatewaysDto != null && service.ServiceGatewaysDto.Any())
            {
                foreach (var gate in service.ServiceGatewaysDto.Where(c => c.Active))
                {
                    if (gate.Gateway != null)
                    {
                        if (gate.Gateway.Enum == (int)GatewayEnum.Importe)
                            return true;
                    }
                }
            }
            return false;
        }
        private bool EnableBills(ServiceDto service)
        {
            if (service.ServiceGatewaysDto != null && service.ServiceGatewaysDto.Any())
            {
                foreach (var gate in service.ServiceGatewaysDto.Where(c => c.Active))
                {
                    if (gate.Gateway != null)
                    {
                        if (gate.Gateway.Enum == (int)GatewayEnum.Banred || gate.Gateway.Enum == (int)GatewayEnum.Carretera || gate.Gateway.Enum == (int)GatewayEnum.Sistarbanc ||
                            gate.Gateway.Enum == (int)GatewayEnum.Sucive || gate.Gateway.Enum == (int)GatewayEnum.Geocom)
                            return true;
                    }
                }
            }
            return false;
        }

        public async Task<IList<ServiceDto>> GetServicesFromContainer(ServiceDto service)
        {
            var services = await _serviceClientService.GetServicesFromContainer(service.ServiceContainerId.HasValue ? service.ServiceContainerId.Value : Guid.Empty);
            return services.ToList();
        }

        [HttpPost]
        public async Task<ActionResult> LoadKeysWithTokenAjax(Guid cardId, string fpProfiler)
        {
            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

            var card = await _cardClientService.Find(cardId);
            payment.Card = card;

            //obtengo la moneda de las facturas que se van a pagar
            //las facturas deben ser todas de la misma moneda.
            var binNumber = Convert.ToInt32(card.MaskedNumber.Substring(0, 6));
            CyberSourceKeyModel cybersourceData = null;
            try
            {
                if (payment.Bills == null || !payment.Bills.Any() || payment.Bills.Any(b => b.Amount <= 0))
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "El monto a pagar no puede ser igual o menor a 0.", PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                cybersourceData = await LoadKeysForCybersource(payment, RedirectEnums.Payment, binNumber, cardId, fpProfiler, "");

                //error en bin
                if (cybersourceData == null)
                {
                    return Json(new JsonResponse(AjaxResponse.BusinessError, "", ExceptionMessages.BIN_NOTVALID_FOR_SERVICE, PresentationCoreMessages.Notification_Title_Alert, NotificationType.Info));
                }

                if (cybersourceData.CybersourceAmount <= 0)
                {
                    var serviceName = payment.Service == null ? "" : payment.Service.Name;
                    var msj = string.Format("Error al calcular el monto a enviar a CS. Servicio {0} - Nro de referencia {1} - Facturas ", serviceName, payment.ReferenceValue);
                    msj += string.Join(",", payment.Bills.Select(x => x.BillExternalId));
                    NLogLogger.LogEvent(NLogType.Error, msj);
                    return Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error, PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }
            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationCoreMessages.Discount_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }

            Session[SessionConstants.PAYMENT_DATA] = payment;
            var discountType = "Descuento: ";
            if (payment.DiscountObj != null)
            {
                discountType = payment.DiscountObj.DiscountLawDescription + ": ";
            }

            var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData.Keys);

            return
                Json(new JsonResponse(AjaxResponse.Success,
                    new
                    {
                        keys = content,
                        Service = payment.Service.Name,
                        cybersourceData.Currency,
                        TotalAmount = cybersourceData.TotalAmount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                        Discount = cybersourceData.Discount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                        TotalAfterDiscount =
                            (cybersourceData.TotalAmount - cybersourceData.Discount).SignificantDigits(2)
                                .ToString("##,#0.00", CultureInfo.CurrentCulture),
                        //DiscountType = payment.DiscountApplyed ? discountType : "No aplica descuento Ley de Inclusión Financiera (19.210).",
                        DiscountType = payment.DiscountApplyed ? discountType : string.Empty,
                    }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
        }

        private async Task<CyberSourceKeyModel> LoadKeysForCybersource(PaymentModel payment, RedirectEnums redirectTo, int binNumber, Guid cardId, string fingerPrint, string nameTh)
        {
            var discount = await GetDiscountData(payment.Bills.ToList(), binNumber, payment.ServiceId);

            if (discount == null)
                return null;

            payment.AmountTocybersource = discount.CybersourceAmount.SignificantDigits(2);

            payment.Bills = new List<BillDto>() { discount.BillDto };

            payment.DiscountObj = discount.DiscountDto;
            payment.DiscountObjId = discount.DiscountDto != null ? discount.DiscountDto.Id : default(Guid);

            var token = await GenerateToken(payment, redirectTo, binNumber, cardId, fingerPrint, nameTh, discount);

            var keys = await _webCyberSourceAccessClientService.GenerateKeys(token);

            var cyberSourceKey = new CyberSourceKeyModel
            {
                Keys = keys,
                Currency = token.Bill.Currency,
                Discount = token.Bill.DiscountAmount,
                DiscountApplyed = token.Bill.DiscountAmount > 0,
                TotalAmount = token.Bill.Amount,
                TotalTaxedAmount = token.Bill.TaxedAmount,
                CybersourceAmount = payment.AmountTocybersource
            };

            payment.Currency = token.Bill.Currency;
            payment.Discount = token.Bill.DiscountAmount;//se debe ingresar el discountamount
            payment.DiscountApplyed = token.Bill.DiscountAmount > 0;
            payment.TotalAmount = token.Bill.Amount;
            payment.TotalTaxedAmount = token.Bill.TaxedAmount;

            Session[SessionConstants.PAYMENT_DATA] = payment;
            Session[SessionConstants.TEMPORARY_ID] = keys["merchant_defined_data29"];

            return cyberSourceKey;
        }

        private async Task<CyberSourceExtraDataDto> GetDiscountData(List<BillDto> bills, int binNumber, Guid serviceId)
        {
            var discountQuery = new DiscountQueryDto
            {
                Bills = bills,
                BinNumber = binNumber,
                ServiceId = serviceId
            };

            //obtengo los valores con los descuentos correspondientes
            var discountList = await _discountClientService.GetDiscount(discountQuery);
            CyberSourceExtraDataDto discount = discountList.FirstOrDefault();

            return discount;
        }

        private async Task<KeysInfoForPayment> GenerateToken(PaymentModel payment, RedirectEnums redirectTo, int binNumber, Guid cardId, string fingerPrint, string nameTh, CyberSourceExtraDataDto discount)
        {
            KeysInfoForPayment token = null;
            Guid userId = Guid.Empty;
            if (payment.RegisteredUserId.HasValue)
            {
                token = new KeysInfoForPaymentRegisteredUser();
                userId = payment.RegisteredUserId.Value;
            }
            else
            {
                token = new KeysInfoForPaymentAnonymousUser();
                userId = payment.AnonymousUserId.Value;
            }

            token.ServiceId = payment.Service.Id;
            token.FingerPrint = fingerPrint;
            token.TransactionReferenceNumber = Guid.NewGuid().ToString();
            token.RedirectTo = redirectTo.ToString("D");
            token.CybersourceAmount = discount.CybersourceAmount;
            token.ReferenceNumber1 = payment.ReferenceValue;
            token.ReferenceNumber2 = payment.ReferenceValue2;
            token.ReferenceNumber3 = payment.ReferenceValue3;
            token.ReferenceNumber4 = payment.ReferenceValue4;
            token.ReferenceNumber5 = payment.ReferenceValue5;
            token.ReferenceNumber6 = payment.ReferenceValue6;
            token.NameTh = nameTh;
            token.CardBin = binNumber.ToString();
            token.Platform = PaymentPlatformDto.VisaNet.ToString();
            token.CardId = cardId;
            token.UserId = userId;
            token.GatewayId = payment.BillsGatewayDto.GatewayId;
            token.DiscountObjId = payment.DiscountObjId.HasValue ? payment.DiscountObjId.Value : Guid.Empty;
            token.UrlReturn = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            token.OperationTypeDto = OperationTypeDto.UniquePayment;
            token.PaymentTypeDto = payment.RegisteredUserId.HasValue ? PaymentTypeDto.Manual : PaymentTypeDto.AnonymousUser;
            //token.DescriptionService = payment.Description;
            token.Quotas = 1;
            token.CardTypeDto = discount.DiscountDto.CardType;

            token.Bill = new BillForToken
            {
                Currency = discount.BillDto.Currency,
                DiscountAmount = discount.BillDto.DiscountAmount,
                Amount = discount.BillDto.Amount,
                TaxedAmount = discount.BillDto.TaxedAmount,
                BillNumber = payment.Bills.First().BillExternalId,
                DiscountType = discount.DiscountDto != null ? (int)discount.DiscountDto.DiscountLabel : 0,
                BillExpirationDate = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().ExpirationDate.ToString("dd-MM-yyyy") : string.Empty,
                BillDescription = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().Description : string.Empty,
                BillGatewayTransactionId = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().GatewayTransactionId : string.Empty,
                BillSucivePreBillNumber = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().SucivePreBillNumber : string.Empty,
                BillFinalConsumer = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().FinalConsumer.ToString() : string.Empty,
                BillDiscount = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().Discount.ToString() : string.Empty,
                BillDateInitTransaccion = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().DateInitTransaccion : string.Empty,
                BillGatewayTransactionBrouId = payment.Bills.FirstOrDefault() != null ? payment.Bills.FirstOrDefault().GatewayTransactionBrouId : string.Empty,
                Quota = 1
            };

            return token;
        }

        public async Task<ActionResult> SelectBills()
        {
            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
            payment.PaymentMethod = 1;
            var nextpage = true;
            ServiceDto service = payment.Service;
            ApplicationUserDto currentUser = null;
            var model = new PaymentServiceModel()
            {
                ReferenceValue = payment.ReferenceValue,
                ReferenceValue2 = payment.ReferenceValue2,
                ReferenceValue3 = payment.ReferenceValue3,
                ReferenceValue4 = payment.ReferenceValue4,
                ReferenceValue5 = payment.ReferenceValue5,
                ReferenceValue6 = payment.ReferenceValue6,
            };

            try
            {
                currentUser = await CurrentSelectedUser();
                var gateway = GetBestGateway(service.ServiceGatewaysDto);
                payment.PaymentMethod = 1; // muestro las facturas primero
                if (gateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                {
                    model.ReferenceValue = model.ReferenceValue.Replace(" ", "");
                }

                var bills = await GetBillsFromGateways(model, payment.Service, payment);
                if (bills != null && bills.Any())
                {
                    NLogLogger.LogEvent(NLogType.Info, bills.Count() + " facturas");
                    bills.First().HasAnnualPatent = CheckSucivePatent(bills);
                }

                var allBills = bills != null ? bills.ToList() : new List<BillDto>();

                payment.BillsGatewayDto = gateway;
                payment.GatewayEnum = (GatewayEnumDto)gateway.Gateway.Enum;
                //payment.Bills = allBills;
                payment.Description = model.Description;
                payment.AllBills = allBills;
                if (allBills.Any()) { payment.IdPadron = allBills.FirstOrDefault().IdPadron; }

                var payable = allBills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();
                nextpage = payable.FirstOrDefault() != null ? payable.FirstOrDefault().Payable : false;


                NLogLogger.LogEvent(NLogType.Info, (payment.AllBills != null ? payment.AllBills.Count() : 0) + " facturas");

                ViewBag.Service = service.Name;
                ViewBag.ReferenceValue = payment.ReferenceValue;
                ViewBag.ReferenceName = payment.Service.ReferenceParamName;

                ViewBag.ReferenceValue2 = payment.ReferenceValue2;
                ViewBag.ReferenceName2 = payment.Service.ReferenceParamName2;

                ViewBag.ReferenceValue3 = payment.ReferenceValue3;
                ViewBag.ReferenceName3 = payment.Service.ReferenceParamName3;

                ViewBag.ReferenceValue4 = payment.ServiceType != null && payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
                ViewBag.ReferenceName4 = payment.Service.ReferenceParamName4;

                ViewBag.ReferenceValue5 = payment.ReferenceValue5;
                ViewBag.ReferenceName5 = payment.Service.ReferenceParamName5;

                ViewBag.ReferenceValue6 = payment.ReferenceValue6;
                ViewBag.ReferenceName6 = payment.Service.ReferenceParamName6;

                ViewBag.EnableBills = service.EnableMultipleBills;

                //si hay pago por importe. cargo los valores ingresados, min y max
                var gateImporte = payment.Service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);

                var dto = new PaymentBillModel()
                {
                    Bills = payment.AllBills != null && payment.AllBills.Any() ? payment.AllBills.Select(b => BillMapper.ToModel(b)).OrderBy(b => b.ExpirationDate).ToList() : null,
                    EnablePartialPayment = payment.Service.EnablePartialPayment,
                    EnableBills = EnableBills(payment.Service),
                    EnableImporte = EnableImporte(payment.Service),
                    EnableMultipleBills = service.EnableMultipleBills,
                    MinPeso = gateImporte != null && gateImporte.ReferenceId != null ? int.Parse(gateImporte.ReferenceId) : 0,
                    MaxPeso = gateImporte != null && gateImporte.ServiceType != null ? int.Parse(gateImporte.ServiceType) : 0,
                    MinD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData) : 0,
                    MaxD = gateImporte != null && gateImporte.AuxiliarData != null ? int.Parse(gateImporte.AuxiliarData2) : 0,
                    NextPage = nextpage,
                    PaymentMethod = payment.PaymentMethod,
                    DisableEditServicePage = true,
                };

                //no puedo pre seleccionar la factura por bill external id, ya que en geocom y sucive usan mismo codigo para todas las facturas
                if ((gateway.Gateway.Enum == (int)GatewayEnumDto.Sucive || gateway.Gateway.Enum == (int)GatewayEnumDto.Geocom)
                    && !string.IsNullOrEmpty(payment.Line))
                {
                    CollectionExtensionMethods.ForEach(dto.Bills.Where(x => x.Line.Equals(payment.Line)), x => x.Pay = true);
                }

                payment.EnableBills = EnableBills(service);
                payment.EnableImporte = EnableImporte(service);
                payment.EnableMultipleBills = service.EnableMultipleBills;
                payment.DisableEditServicePage = true;
                Session[SessionConstants.PAYMENT_DATA] = payment;
                return View("Bills", dto);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
            }
            catch (WebApiClientBillBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }

            var services = currentUser == null ? await _serviceClientService.GetServicesPaymentPublic() : await _serviceClientService.GetServicesPaymentPrivate();

            ViewBag.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);

            service = await _serviceClientService.Find(payment.ServiceId);

            model.Sucive =
                service.ServiceGatewaysDto.Any(
                    s =>
                       s.Gateway.Enum == (int)GatewayEnum.Sucive ||
                        s.Gateway.Enum == (int)GatewayEnum.Geocom && s.Active);
            var deptos = GenerateDepartamentList();

            model.Departaments = deptos;

            var serviceDto = await _serviceClientService.Find(payment.ServiceId);
            if (serviceDto != null)
            {
                //ViewBag.CreditCard = serviceDto.CreditCard;
                //ViewBag.DebitCard = serviceDto.DebitCard;
                //ViewBag.CreditCardInternational = serviceDto.CreditCardInternational;
                //ViewBag.DebitCardInternational = serviceDto.DebitCardInternational;
            }
            var locations = await LoadLocation(service);
            var st = ServiceType(service);
            model.ServiceType = st;
            model.LocationsCiu = locations;

            ViewBag.EnableBills = service.EnableMultipleBills;

            return View("Service", model);
        }

        public async Task<ActionResult> SelectCard()
        {
            try
            {
                var currentUser = await CurrentSelectedUser();
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];

                payment.EnableBills = EnableBills(payment.Service);
                payment.EnableImporte = EnableImporte(payment.Service);

                var correct = false;
                payment.PaymentMethod = 1;
                var referenceId = payment.Service.ServiceGatewaysDto.First(x => x.Gateway.Enum == (int)payment.GatewayEnum).ReferenceId;

                ServiceGatewayDto gateway = GetBestGateway(payment.Service.ServiceGatewaysDto);
                payment.BillsGatewayDto = gateway;

                if (payment.GatewayEnum == GatewayEnumDto.Sucive || payment.GatewayEnum == GatewayEnumDto.Geocom)
                {
                    var lines = payment.Line.ToList();
                    var line = String.Join("", lines);
                    var preBill = await _billClientService.ChekBills(line, payment.IdPadron, (int)payment.Service.Departament,
                                payment.GatewayEnum, referenceId);
                    //si tiene SucivePreBillNumber -1 hay un error
                    if (preBill.SucivePreBillNumber.Equals("-1"))
                    {
                        TempData["BillsInvalid"] = preBill.Description;
                    }
                    else
                    {
                        correct = true;
                        payment.Bills = new List<BillDto>() { preBill };
                    }
                }
                else
                {
                    var payModel = new PaymentServiceModel
                    {
                        ReferenceValue = payment.ReferenceValue,
                        ReferenceValue2 = payment.ReferenceValue2,
                        ReferenceValue3 = payment.ReferenceValue3,
                        ReferenceValue4 = payment.ReferenceValue4,
                        ReferenceValue5 = payment.ReferenceValue5,
                        ReferenceValue6 = payment.ReferenceValue6,
                    };

                    if (gateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                    {
                        payModel.ReferenceValue = payModel.ReferenceValue.Replace(" ", "");
                    }

                    var bills = await GetBillsFromGateways(payModel, payment.Service, payment);
                    var allBills = bills != null ? bills.ToList() : new List<BillDto>();

                    payment.AllBills = allBills;

                    var billmodel = new BillModel
                    {
                        BillExternalId = payment.BillExternalId,
                        Pay = true,
                        Line = payment.Line,
                    };

                    var selected = new List<BillModel> { billmodel };
                    correct = CheckBillsSelected(selected, payment.AllBills.Where(b => b.Payable));
                    payment.Bills = payment.AllBills.Where(b => b.BillExternalId == selected.First().BillExternalId).ToList();

                    payment.ServiceType = ServiceType(payment.Service);
                    payment.BillsGatewayDto = gateway;
                }
                if (correct)
                {
                    ViewBag.Service = payment.Service.Name;

                    ViewBag.ReferenceValue = payment.ReferenceValue;
                    ViewBag.ReferenceName = payment.Service.ReferenceParamName;

                    ViewBag.ReferenceValue2 = payment.ReferenceValue2;
                    ViewBag.ReferenceName2 = payment.Service.ReferenceParamName2;

                    ViewBag.ReferenceValue3 = payment.ReferenceValue3;
                    ViewBag.ReferenceName3 = payment.Service.ReferenceParamName3;

                    ViewBag.ReferenceValue4 = payment.ServiceType != null && payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
                    ViewBag.ReferenceName4 = payment.Service.ReferenceParamName4;

                    ViewBag.ReferenceValue5 = payment.ReferenceValue5;
                    ViewBag.ReferenceName5 = payment.Service.ReferenceParamName5;

                    ViewBag.ReferenceValue6 = payment.ReferenceValue6;
                    ViewBag.ReferenceName6 = payment.Service.ReferenceParamName6;

                    var dolar = payment.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount);
                    ViewBag.TotalDolars = dolar;

                    var pesos = payment.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount);
                    ViewBag.TotalPesos = pesos;

                    ViewBag.Cards = new List<CardDto>();

                    var user = await CurrentSelectedUser();

                    payment.RegisteredUser = user;
                    payment.RegisteredUserId = user.Id;

                    ViewBag.DefaultCardId = payment.ServiceAssociated.DefaultCardId;

                    user = await _applicationUserClientService.Find(user.Id);

                    var cards = await _serviceClientService.GetEnableCards(user.Id, payment.ServiceId);
                    ViewBag.Cards = cards.Where(c => c.Active).ToList();


                    ViewBag.MerchantId = payment.Service.MerchantId.Trim();
                    ViewBag.CsEnvironment = ConfigurationManager.AppSettings["CsEnvironment"];
                    payment.DisableEditServicePage = true;
                    Session[SessionConstants.PAYMENT_DATA] = payment;

                    ViewBag.ReturnButton = "ReturnToBills";

                    var paymentcardmodel = new CardModel()
                    {
                        Bills = payment.Bills.ToList(),
                    };

                    return View("Card", paymentcardmodel);
                }
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationWebStrings.Payment_General_Error, NotificationType.Error);
            }
            return RedirectToAction("Bills");
        }

        public ActionResult ServiceDetails()
        {
            var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
            Session[SessionConstants.PAYMENT_DATA] = null;

            return View(new PaymentServiceModel
            {
                //ServiceId = payment.ServiceId,
                //Service = payment.Service.Name,
                ReferenceName = payment.Service.ReferenceParamName,
                ReferenceName2 = payment.Service.ReferenceParamName2,
                ReferenceName3 = payment.Service.ReferenceParamName3,
                ReferenceName4 = payment.Service.ReferenceParamName4,
                ReferenceName5 = payment.Service.ReferenceParamName5,
                ReferenceName6 = payment.Service.ReferenceParamName6,
                ReferenceValue = payment.ReferenceValue,
                ReferenceValue2 = payment.ReferenceValue2,
                ReferenceValue3 = payment.ReferenceValue3,
                ReferenceValue4 = payment.ReferenceValue4,
                ReferenceValue5 = payment.ReferenceValue5,
                ReferenceValue6 = payment.ReferenceValue6,
                Description = payment.Description,
                Sucive = payment.GatewayEnum == GatewayEnumDto.Sucive,
                ServiceType = ServiceType(payment.Service)

            });
        }

        public async Task<bool> LoadCardViewBag(PaymentModel payment)
        {
            ViewBag.Cards = new List<CardDto>();
            var user = await CurrentSelectedUser();
            if (user != null)
            {
                var serviceAsociated = await _serviceAssosiateClientService.ServiceAssosiatedToUser(user.Id, payment.ServiceId, new string[]
                                    {
                                        payment.ReferenceValue, payment.ReferenceValue2, payment.ReferenceValue3,
                                        payment.ReferenceValue4, payment.ReferenceValue5, payment.ReferenceValue6
                                    });

                if (serviceAsociated != null)
                {
                    payment.ServicesAssosiatedId = serviceAsociated.Id;
                    payment.ServiceAssociated = serviceAsociated;
                    ViewBag.DefaultCardId = serviceAsociated.DefaultCardId;
                }

                var cards = await _serviceClientService.GetEnableCards(user.Id, payment.ServiceId);
                ViewBag.Cards = cards.Where(c => c.Active).ToList();

                //if (user.CardDtos == null || !user.CardDtos.Any())
                //    user = await _applicationUserClientService.GetUserWithCards(user.Id);

                ////user = await _applicationUserClientService.GetUserWithCards(user.Id);
                //if (user.CardDtos != null && user.CardDtos.Any())
                //{
                //    var today = DateTime.Now.Date;
                //    ViewBag.Cards =
                //        user.CardDtos.Where(
                //            c =>
                //                c.Active &&
                //                ((c.DueDate.Year.CompareTo(today.Year) == 0 &&
                //                  c.DueDate.Month.CompareTo(today.Month) >= 0) ||
                //                 c.DueDate.Year.CompareTo(today.Year) > 0)).ToList();
                //}
            }

            return true;

        }

        private List<SelectListItem> GenerateCurrencyPaymentStatusList()
        {
            var list = new List<CurrencyDto> { CurrencyDto.UYU, CurrencyDto.USD };
            return list.Select(currencyDto => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(CurrencyDto), (int)currencyDto, EnumsStrings.ResourceManager),
                Value = (int)currencyDto + "",
            }).ToList();
        }

        [HttpGet]
        public async Task<ActionResult> ValidateSelectedCard(Guid cardId)
        {
            try
            {
                var card = await _cardClientService.Find(cardId);
                var payment = (PaymentModel)Session[SessionConstants.PAYMENT_DATA];
                var isValid = await _serviceClientService.IsBinAssociatedToService(card.BIN, payment.ServiceId);
                var today = DateTime.Now;
                if (!((card.DueDate.Year.CompareTo(today.Year) == 0 && card.DueDate.Month.CompareTo(today.Month) >= 0) || card.DueDate.Year.CompareTo(today.Year) > 0))
                {
                    return Json(new JsonResponse(AjaxResponse.Error, string.Empty, ExceptionMessages.CARD_EXPIRED, string.Empty, NotificationType.Info), JsonRequestBehavior.AllowGet);
                }

                if (isValid)
                {
                    return
                        Json(
                            new JsonResponse(AjaxResponse.Success, string.Empty, string.Empty, string.Empty,
                                NotificationType.Success), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Error, string.Empty, ExceptionMessages.BIN_NOTVALID_FOR_SERVICE, string.Empty, NotificationType.Info), JsonRequestBehavior.AllowGet);

            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationCoreMessages.Discount_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        #region Cybersource Fingerprint
        [HttpGet]
        public ActionResult FingerprintRedirectPng1(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/clear.png?org_id=" + orgId + "&session_id=" + sessionId + "&m=1";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectPng2(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/clear.png?org_id=" + orgId + "&session_id=" + sessionId + "&m=2";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectJs(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/check.js?org_id=" + orgId + "&session_id=" + sessionId;
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectSwf(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/fp.swf?org_id=" + orgId + "&session_id=" + sessionId;
            return Redirect(redirectUrl);
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult> ValidateAnnualPatent(Guid serviceId)
        {
            var content = string.Empty;
            try
            {
                var serviceIdApp = ConfigurationManager.AppSettings["SuciveServiceAnnualIdApp"];
                var service = await _serviceClientService.Find(serviceId);

                if (service != null && service.UrlName.Equals(serviceIdApp))
                {
                    var now = DateTime.Now;

                    //SI NO ESTOY EN ENERO, NO HAY FACTURA ANUAL CON DESCUENTO
                    if (now.Month != 1)
                        return Json(new JsonResponse(AjaxResponse.Success, content, "", ""),
                            JsonRequestBehavior.AllowGet);

                    var lastDayInt =
                        int.Parse(ConfigurationManager.AppSettings["SuciveServiceAnnualLastDayJanuaryInclusive"]);
                    var lastday = new DateTime(now.Year, 1, lastDayInt);

                    if (now > lastday)
                        return Json(new JsonResponse(AjaxResponse.Success, content, "", ""),
                            JsonRequestBehavior.AllowGet);

                    TempData["AnnualLastDay"] = lastDayInt;
                    TempData["AnnualYear"] = now.Year;
                    content = RenderPartialViewToString("_SelectSuciveAnnual", null);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
            }
            return Json(new JsonResponse(AjaxResponse.Success, content, "", ""), JsonRequestBehavior.AllowGet);
        }

        private async Task<CardModel> GenerateAnnualPatent(ServiceDto dto, PaymentServiceModel model)
        {
            var references = new Dictionary<string, string>()
            {
                {dto.ReferenceParamName, model.ReferenceValue},
                {dto.ReferenceParamName2, model.ReferenceValue2}
            };

            var user = await CurrentSelectedUser();
            var userId = Guid.Empty;
            List<BillDto> bills = null;
            if (user == null)
            {
                var filter = new AnonymousUserBillFilterDto()
                {
                    ServiceId = dto.Id,
                    References = references,
                    AnonymousUserDto = new AnonymousUserDto()
                    {
                        Name = model.AnonymousUser.Name,
                        Surname = model.AnonymousUser.Surname,
                        Email = model.AnonymousUser.Email,
                    }
                };
                var preBill = await _billClientService.GenerateAnnualPatenteForAnonymousUser(filter);
                userId = preBill != null ? preBill.User.Id : Guid.Empty;
                bills = preBill != null && preBill.Bills != null ? preBill.Bills.ToList() : null;
            }
            else
            {
                var filter = new RegisteredUserBillFilterDto()
                {
                    ServiceId = dto.Id,
                    References = references,
                    UserId = user.Id,
                };
                var preBill = await _billClientService.GenerateAnnualPatenteForRegisteredUser(filter);
                userId = preBill != null ? preBill.User.Id : Guid.Empty;
                bills = preBill != null && preBill.Bills != null ? preBill.Bills.ToList() : null;
            }

            CardModel pageModel = null;

            if (bills != null && bills.Any() && userId != Guid.Empty)
            {
                pageModel = new CardModel()
                {
                    Bills = bills
                };
                //ERROR DEVUELTO POR SUCIVE
                if (bills.FirstOrDefault().SucivePreBillNumber.Equals("-1"))
                {
                    return pageModel;
                }

                var gateway = GetBestGateway(dto.ServiceGatewaysDto);

                var payment = new PaymentModel
                {
                    ServiceId = model.ServiceToPayId,
                    Service = dto,
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
                    Description = model.Description,
                    EnableMultipleBills = dto.EnableMultipleBills,
                    ServiceType = model.ServiceType,
                    EnableBills = EnableBills(dto),
                    EnableImporte = EnableImporte(dto),
                    Bills = pageModel.Bills,
                    GatewayEnum = GatewayEnumDto.Sucive,
                    PaymentMethod = 1,
                    BillsGatewayDto = gateway,
                };

                if (user != null)
                {
                    payment.RegisteredUserId = userId;
                }
                else
                {
                    payment.AnonymousUserId = userId;
                }

                ViewBag.Service = payment.Service.Name;

                ViewBag.ReferenceValue = payment.ReferenceValue;
                ViewBag.ReferenceName = payment.ReferenceName;

                ViewBag.ReferenceValue2 = payment.ReferenceValue2;
                ViewBag.ReferenceName2 = payment.ReferenceName2;

                ViewBag.ReferenceValue3 = payment.ReferenceValue3;
                ViewBag.ReferenceName3 = payment.ReferenceName3;

                ViewBag.ReferenceValue4 = ViewBag.ReferenceValue4 = payment.ServiceType.Equals("CIU") ? await LoadLocationName(payment.Service, payment.ReferenceValue4) : payment.ReferenceValue4;
                ViewBag.ReferenceName4 = payment.ReferenceName4;

                ViewBag.ReferenceValue5 = payment.ReferenceValue5;
                ViewBag.ReferenceName5 = payment.ReferenceName5;

                ViewBag.ReferenceValue6 = payment.ReferenceValue6;
                ViewBag.ReferenceName6 = payment.ReferenceName6;

                var dolar = payment.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount);
                ViewBag.TotalDolars = dolar;
                var pesos = payment.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount);
                ViewBag.TotalPesos = pesos;

                await LoadCardViewBag(payment);

                ViewBag.MerchantId = payment.Service.MerchantId.Trim();
                ViewBag.CsEnvironment = ConfigurationManager.AppSettings["CsEnvironment"];

                Session[SessionConstants.PAYMENT_DATA] = payment;

            }
            return pageModel;
        }
    }
}