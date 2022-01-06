using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class BillController : BaseController
    {
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebBillClientService _webBillClientService;
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebFixedNotificationClientService _webFixedNotificationClientService;

        public BillController(IWebServiceAssosiateClientService webServiceAssosiateClientService, IWebBillClientService webBillClientService, IWebServiceClientService webServiceClientService, IWebFixedNotificationClientService webFixedNotificationClientService)
        {
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _webBillClientService = webBillClientService;
            _webServiceClientService = webServiceClientService;
            _webFixedNotificationClientService = webFixedNotificationClientService;
        }

        //
        // GET: /Private/Bill/
        public ActionResult Index()
        {
            return View("Index", new ServiceFilterAssosiateDto());
        }

        public ActionResult Bills(int payment)
        {
            return View("Index", new ServiceFilterAssosiateDto()
            {
                WithAutomaticPaymentsInt = payment
            });
        }

        [HttpPost]
        public async Task<ActionResult> Bills(FormCollection frm)
        {
            var serviceAssociatedId = Guid.Parse(frm["ServiceAssociatedId"]);

            var service = await _webServiceAssosiateClientService.Find(serviceAssociatedId);

            return View("Index", new ServiceFilterAssosiateDto
            {
                ServiceAssociatedId = serviceAssociatedId,
                Service = service.ServiceDto.Name,
                ReferenceNumber = service.ReferenceNumber
            });
        }

        public async Task<ActionResult> GetBillsAjax(ServiceFilterAssosiateDto filter)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            var servicesToPay = new List<BillToPayModel>();
            filter.UserId = (await CurrentSelectedUser()).Id;

            filter.DisplayLength = 100000;

            var serviceAssociate = await _webServiceAssosiateClientService.Get(filter);
            //Obtengo solo los servicios asociados activos
            serviceAssociate = serviceAssociate.Where(s => s.Active).ToList();

            var tasks = new List<Task<ApplicationUserBillDto>>();
            var dic = new Dictionary<int, ServiceAssociatedDto>();

            var currentSelectedUser = await CurrentSelectedUser();

            foreach (var serviceAssociated in serviceAssociate)
            {
                //sucive y geocom, guardan idpadron en numero de referencia 6
                var references = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName, serviceAssociated.ReferenceNumber);
                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName2))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName2, serviceAssociated.ReferenceNumber2);
                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName3))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName3, serviceAssociated.ReferenceNumber3);
                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName4))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName4, serviceAssociated.ReferenceNumber4);
                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName5))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName5, serviceAssociated.ReferenceNumber5);
                if (!string.IsNullOrEmpty(serviceAssociated.ServiceDto.ReferenceParamName6))
                    references.Add(serviceAssociated.ServiceDto.ReferenceParamName6, serviceAssociated.ReferenceNumber6);

                var t = _webBillClientService.GetBillsForRegisteredUser(new RegisteredUserBillFilterDto()
                {
                    ServiceId = serviceAssociated.ServiceDto.Id,
                    References = references,
                    UserId = filter.UserId,
                    ScheduledPayment = false,
                });
                tasks.Add(t);
                dic.Add(t.Id, serviceAssociated);
            }

            while (tasks.Count > 0)
            {
                try
                {
                    var task = await Task.WhenAny(tasks);

                    tasks.Remove(task);
                    var serviceAssociated = dic[task.Id];

                    var userBills = await task;
                    var bills = userBills.Bills;
                    if (bills == null || !bills.Any())
                        continue;


                    bool multipleBills = serviceAssociated.ServiceDto.EnableMultipleBills && bills.Count() > 1;
                    //Agrego cada factura por pagar al listado de proximos vencimientos y pagos programados
                    foreach (var bill in bills)
                    {
                        if (filter.From == null || filter.From != null && DateTime.Compare(bill.ExpirationDate, (DateTime)filter.From) > -1)
                        {
                            if (filter.To == null || filter.To != null && DateTime.Compare(bill.ExpirationDate, (DateTime)filter.To) < 1)
                            {
                                var toAdd = new BillToPayModel
                                {
                                    ServiceAssociatedId = serviceAssociated.Id,
                                    ServiceId = serviceAssociated.ServiceId,
                                    ServiceName = serviceAssociated.ServiceDto.Name,
                                    ServiceImageName = GetImageForService(serviceAssociated.ServiceDto),
                                    ServiceDesc = serviceAssociated.Description,
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
                                    Amount = bill.Bills != null && bill.Bills.Any() ? bill.Bills.Sum(x => x.Amount) + bill.Amount : bill.Amount,
                                    Currency = bill.Currency,
                                    //DueDate = bills.Min(b => b.ExpirationDate),
                                    DueDate = bill.ExpirationDate,
                                    DefaultCardMask = serviceAssociated.DefaultCard.MaskedNumber,
                                    GatewayEnumDto = bill.Gateway,
                                    AllowsAutomaticPayment = serviceAssociated.ServiceDto.EnableAutomaticPayment,
                                    ServiceContainerName = serviceAssociated.ServiceDto != null ? serviceAssociated.ServiceDto.ServiceContainerName : string.Empty,
                                    BillExternalId = bill.BillExternalId,
                                    Line = bill.Line,
                                    Payable = bill.Payable,
                                    DashboardDescription = bill.DashboardDescription,
                                    MultipleBills = multipleBills,
                                };

                                if (serviceAssociated.AutomaticPaymentDtoId != null)
                                {
                                    var date = bill.ExpirationDate.Subtract(new TimeSpan(serviceAssociated.AutomaticPaymentDto.DaysBeforeDueDate, 0, 0, 0));
                                    toAdd.AutomaticPaymentDateString = date.ToString("dd/MM/yyyy");
                                }

                                if (filter.WithAutomaticPaymentsInt == 0)
                                {
                                    servicesToPay.Add(toAdd);
                                }

                                if (filter.WithAutomaticPaymentsInt == 1 && serviceAssociated.AutomaticPaymentDtoId != Guid.Empty && serviceAssociated.AutomaticPaymentDtoId != null)
                                {
                                    servicesToPay.Add(toAdd);
                                }
                                if (filter.WithAutomaticPaymentsInt == 2 && (serviceAssociated.AutomaticPaymentDtoId == Guid.Empty || serviceAssociated.AutomaticPaymentDtoId == null))
                                {
                                    servicesToPay.Add(toAdd);
                                }
                            }
                        }
                    }
                }
                catch (WebApiClientBusinessException)
                {
                    message = PresentationWebStrings.Bills_General_Error;
                    title = PresentationWebStrings.Action_Error;
                }
                catch (WebApiClientFatalException)
                {
                    message = PresentationWebStrings.Bills_General_Error;
                    title = PresentationWebStrings.Action_Error;
                }
                catch (WebApiClientBillBusinessException e)
                {
                    message = e.Message;
                    title = PresentationWebStrings.Action_Error;
                }
            }
            if (servicesToPay != null && servicesToPay.Any())
            {
                message = PresentationWebStrings.Service_Activate;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;
            }
            if (String.IsNullOrEmpty(message))
            {
                //se produjo un error en alguna parte
            }

            ViewBag.IsSearch = filter.From != null || filter.To != null || !String.IsNullOrEmpty(filter.Service) ||
                               !String.IsNullOrEmpty(filter.ReferenceNumber);

            servicesToPay = servicesToPay.OrderBy(x => x.DueDate).ToList();

            var content = RenderPartialViewToString("_BillList", servicesToPay);

            if (servicesToPay.Any())
            {
                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            else
            {
                response = AjaxResponse.Success;
                return Json(new JsonResponse(response, content, null, null, null), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> PaySelectedBill(string billExternalId, Guid serviceAssociatedId, int gateway, string line)
        {
            try
            {
                var serviceAssociated = await _webServiceAssosiateClientService.Find(serviceAssociatedId);

                if (serviceAssociated == null)
                    throw new Exception("No se ha encontrado el servicio asociado");

                var service = await _webServiceClientService.Find(serviceAssociated.ServiceDto.Id);
                var currentUser = await CurrentSelectedUser();
                var payment = new PaymentModel
                {
                    ServiceId = serviceAssociated.ServiceDto.Id,
                    Service = service,
                    ServiceName = service.Name,
                    ServiceAssociated = serviceAssociated,
                    ServicesAssosiatedId = serviceAssociated.Id,
                    ReferenceValue = serviceAssociated.ReferenceNumber,
                    ReferenceValue2 = serviceAssociated.ReferenceNumber2,
                    ReferenceValue3 = serviceAssociated.ReferenceNumber3,
                    ReferenceValue4 = serviceAssociated.ReferenceNumber4,
                    ReferenceValue5 = serviceAssociated.ReferenceNumber5,
                    ReferenceValue6 = serviceAssociated.ReferenceNumber6,
                    ReferenceName = service.ReferenceParamName,
                    ReferenceName2 = service.ReferenceParamName2,
                    ReferenceName3 = service.ReferenceParamName3,
                    ReferenceName4 = service.ReferenceParamName4,
                    ReferenceName5 = service.ReferenceParamName5,
                    ReferenceName6 = service.ReferenceParamName6,
                    Description = serviceAssociated.Description,
                    GatewayEnum = (GatewayEnumDto)gateway,
                    BillExternalId = billExternalId,
                    Line = line,
                    IdPadron = string.IsNullOrEmpty(serviceAssociated.ReferenceNumber6) ? 0 : Convert.ToInt32(serviceAssociated.ReferenceNumber6),
                    RegisteredUserId = currentUser.Id,
                    RegisteredUser = currentUser,
                    EnableMultipleBills = service.EnableMultipleBills,
                    ServiceType = ServiceType(service),
                };
                Session[SessionConstants.PAYMENT_DATA] = payment;
                NLogLogger.LogEvent(NLogType.Info, string.Format("Prviado - BillController llamo a pagar factura. Pasarela {0}, Params {1}",
                    payment.GatewayEnum, payment.ReferenceValue + (payment.GatewayEnum == GatewayEnumDto.Sucive || payment.GatewayEnum == GatewayEnumDto.Geocom ? ", id padron " +
                    payment.ReferenceValue6 : "")));

                return RedirectToAction("SelectCard", "Pay", new RouteValueDictionary() { { "Area", "Pay" } });

            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Info);
                return RedirectToAction("Index", "Dashboard");
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<ActionResult> PayMultipleBills(string billExternalId, Guid serviceAssociatedId, int gateway, string line)
        {
            try
            {
                var serviceAssociated = await _webServiceAssosiateClientService.Find(serviceAssociatedId);

                if (serviceAssociated == null)
                    throw new Exception("No se ha encontrado el servicio asociado");

                var service = await _webServiceClientService.Find(serviceAssociated.ServiceDto.Id);
                var currentUser = await CurrentSelectedUser();
                var payment = new PaymentModel
                {
                    ServiceId = serviceAssociated.ServiceDto.Id,
                    Service = service,
                    ServiceName = service.Name,
                    ServiceAssociated = serviceAssociated,
                    ServicesAssosiatedId = serviceAssociated.Id,
                    ReferenceValue = serviceAssociated.ReferenceNumber,
                    ReferenceValue2 = serviceAssociated.ReferenceNumber2,
                    ReferenceValue3 = serviceAssociated.ReferenceNumber3,
                    ReferenceValue4 = serviceAssociated.ReferenceNumber4,
                    ReferenceValue5 = serviceAssociated.ReferenceNumber5,
                    ReferenceValue6 = serviceAssociated.ReferenceNumber6,
                    ReferenceName = service.ReferenceParamName,
                    ReferenceName2 = service.ReferenceParamName2,
                    ReferenceName3 = service.ReferenceParamName3,
                    ReferenceName4 = service.ReferenceParamName4,
                    ReferenceName5 = service.ReferenceParamName5,
                    ReferenceName6 = service.ReferenceParamName6,
                    Description = serviceAssociated.Description,
                    GatewayEnum = (GatewayEnumDto)gateway,
                    BillExternalId = billExternalId,
                    Line = line,
                    IdPadron = string.IsNullOrEmpty(serviceAssociated.ReferenceNumber6) ? 0 : Convert.ToInt32(serviceAssociated.ReferenceNumber6),
                    RegisteredUserId = currentUser.Id,
                    RegisteredUser = currentUser,
                    EnableMultipleBills = service.EnableMultipleBills,
                    ServiceType = ServiceType(service),
                };

                Session[SessionConstants.PAYMENT_DATA] = payment;

                return RedirectToAction("SelectBills", "Pay", new RouteValueDictionary() { { "Area", "Pay" } });

            }
            catch (WebApiClientBusinessException)
            {
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Info);
                return RedirectToAction("Index", "Dashboard");
            }
            catch (WebApiClientFatalException)
            {
                ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                return RedirectToAction("Index", "Dashboard");
            }
        }

    }
}