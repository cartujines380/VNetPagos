using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class DashboardController : BaseController
    {
        private readonly IWebBillClientService _billClientService;
        private readonly IWebServiceAssosiateClientService _serviceAssosiateClientService;
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebPaymentClientService _paymentClientService;
        private readonly IWebDebitClientService _webDebitClientService;

        public DashboardController(IWebBillClientService billClientService, IWebServiceAssosiateClientService serviceAssosiateClientService,
            IWebServiceClientService serviceClientService, IWebPaymentClientService paymentClientService, IWebDebitClientService webDebitClientService)
        {
            _billClientService = billClientService;
            _serviceAssosiateClientService = serviceAssosiateClientService;
            _serviceClientService = serviceClientService;
            _paymentClientService = paymentClientService;
            _webDebitClientService = webDebitClientService;
        }

        //
        // GET: /Private/Dashboard/
        public ActionResult Index()
        {
            //limpio sesion si llego al dashboard
            Session[SessionConstants.PAYMENT_DATA] = null;
            return View();
        }

        public async Task<ActionResult> GetBillsAndAutomaticPaymentsAjax()
        {
            var error = false;
            var servicesToPay = new List<BillToPayModel>();
            var nextAutomaticPayments = new List<NextAutomaticPaymentModel>();

            var user = await CurrentSelectedUser();
            var serviceAssociate = await _serviceAssosiateClientService.GetServicesForBills(user.Id);

            if (serviceAssociate == null || !serviceAssociate.Any())
            {
                var hasServices = await _serviceAssosiateClientService.HasAsosiatedService(user.Id);
                if (hasServices)
                {
                    TempData["ErrorLoadingBill"] = PresentationWebStrings.Dashboard_Without_ActiveService;
                    TempData["ErrorLoadingAutomatic"] = PresentationWebStrings.Dashboard_AutomaticPayment_ServiceNotActive;
                }
                else
                {
                    TempData["ErrorLoadingBill"] = PresentationWebStrings.Dashboard_Without_Services;
                    TempData["ErrorLoadingAutomatic"] = PresentationWebStrings.Dashboard_AutomaticPayment_WithOutService;
                }

                return PartialView("_BillsAndAutoPayments", new DashBoardModel()
                {
                    AutoPayments = new List<NextAutomaticPaymentModel>(nextAutomaticPayments),
                    Bills = new List<BillToPayModel>(servicesToPay)
                });
            }

            var tasks = new List<Task<ApplicationUserBillDto>>();
            var dic = new Dictionary<int, ServiceAssociatedDto>();

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

                var t = _billClientService.GetBillsForRegisteredUser(new RegisteredUserBillFilterDto()
                    {
                        ServiceId = serviceAssociated.ServiceDto.Id,
                        References = references,
                        UserId = user.Id,
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
                        var add = new BillToPayModel
                        {
                            ServiceAssociatedId = serviceAssociated.Id,
                            ServiceId = serviceAssociated.ServiceDto.Id,
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
                            DefaultCardMask = serviceAssociated.DefaultCard != null ? serviceAssociated.DefaultCard.MaskedNumber : "",
                            DefaultCardDescription = serviceAssociated.DefaultCard != null ? serviceAssociated.DefaultCard.Description : "",
                            DueDate = bill.ExpirationDate,
                            GatewayEnumDto = bill.Gateway,
                            AllowsAutomaticPayment = serviceAssociated.ServiceDto.EnableAutomaticPayment,
                            BillExternalId = bill.BillExternalId,
                            Line = bill.Line,
                            Payable = bill.Payable,
                            DashboardDescription = bill.DashboardDescription,
                            MultipleBills = multipleBills,
                            ServiceContainerName = serviceAssociated.ServiceDto.ServiceContainerName
                        };

                        if (serviceAssociated.AutomaticPaymentDtoId != null)
                        {
                            var date = bill.ExpirationDate.Subtract(new TimeSpan(serviceAssociated.AutomaticPaymentDto.DaysBeforeDueDate, 0, 0, 0));
                            add.AutomaticPaymentDateString = date.ToString("dd/MM/yyyy");
                        }
                        servicesToPay.Add(add);
                    }

                    //Muestro solo las cuatro facturas mas proximas
                    servicesToPay = servicesToPay.OrderBy(x => x.DueDate).Take(4).ToList();
                }
                catch (WebApiClientBusinessException ex)
                {
                    ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                    error = true;
                    NLogLogger.LogEvent(ex);
                }
                catch (WebApiClientFatalException ex)
                {
                    ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                    error = true;
                    NLogLogger.LogEvent(ex);
                }
                catch (WebApiClientBillBusinessException ex)
                {
                    ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                    error = false;
                    NLogLogger.LogEvent(ex);
                }
                catch (Exception ex)
                {
                    ShowNotification(PresentationWebStrings.Bills_General_Error, NotificationType.Error);
                    error = true;
                    NLogLogger.LogEvent(ex);
                }
            }
            if (error)
            {
                TempData["ErrorBills"] = PresentationWebStrings.Dashboard_Bills_Error;
                TempData["ErrorAtuomaticPayments"] = PresentationWebStrings.Dashboard_AutomaticPayment_Error;
            }
            if (!nextAutomaticPayments.Any())
            {
                var has = await _serviceAssosiateClientService.HasAutomaticPaymentCreated(user.Id);
                if (has)
                {
                    TempData["ErrorLoadingAutomatic"] = PresentationWebStrings.Dashboard_AutomaticPayment_ServiceNotActive;
                }
                else
                {
                    TempData["ErrorLoadingAutomatic"] = PresentationWebStrings.Dashboard_AutomaticPayment_NotActive;
                }

            }
            return PartialView("_BillsAndAutoPayments", new DashBoardModel()
            {
                AutoPayments = new List<NextAutomaticPaymentModel>(nextAutomaticPayments),
                Bills = new List<BillToPayModel>(servicesToPay)
            });
        }

        public async Task<ActionResult> PaySelectedBill(string billExternalId, Guid serviceAssociatedId, int gateway, string line)
        {
            try
            {
                var serviceAssociated = await _serviceAssosiateClientService.Find(serviceAssociatedId);

                if (serviceAssociated == null)
                    throw new Exception("No se ha encontrado el servicio asociado");

                var service = await _serviceClientService.Find(serviceAssociated.ServiceDto.Id);

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
                    IdPadron = !string.IsNullOrEmpty(serviceAssociated.ReferenceNumber6) ? Convert.ToInt32(serviceAssociated.ReferenceNumber6) : 0,
                    RegisteredUserId = currentUser.Id,
                    RegisteredUser = currentUser,
                    EnableMultipleBills = service.EnableMultipleBills,
                    ServiceType = ServiceType(service),
                };

                Session[SessionConstants.PAYMENT_DATA] = payment;

                NLogLogger.LogEvent(NLogType.Info, string.Format("Prviado - DashboardController llamo a pagar factura. Pasarela {0}, Params {1}",
                    payment.GatewayEnum, payment.ReferenceValue + (payment.GatewayEnum == GatewayEnumDto.Sucive || payment.GatewayEnum == GatewayEnumDto.Geocom ? ", id padron " +
                    payment.ReferenceValue6 : "")));

                return RedirectToAction("SelectCard", "Pay", new RouteValueDictionary() { { "Area", "Pay" } });

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

        public async Task<ActionResult> PayMultipleBills(string billExternalId, Guid serviceAssociatedId, int gateway, string line)
        {
            try
            {
                var serviceAssociated = await _serviceAssosiateClientService.Find(serviceAssociatedId);

                if (serviceAssociated == null)
                    throw new Exception("No se ha encontrado el servicio asociado");

                var service = await _serviceClientService.Find(serviceAssociated.ServiceDto.Id);

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

        public async Task<ActionResult> GetPaymentsAjax()
        {
            var user = await CurrentSelectedUser();

            var payments = await _paymentClientService.FindAll(new PaymentFilterDto
            {
                UserId = user.Id,
                From = default(DateTime),
                To = default(DateTime),
                ServiceAssociatedDto = "",
                Status = 0
            });

            //Muestro solo los dos pagos mas proximos
            payments = payments.OrderByDescending(x => x.Date).Take(2).ToList();

            //Agrego cada pago realizado al historial de pagos
            var paymentHistory = payments.Select(payment => new PaymentHistoryModel
            {
                Id = payment.Id,
                Date = payment.Date,
                TransactionNumber = payment.TransactionNumber,
                ServiceName = payment.ServiceDto.Name,
                ServiceDesc = payment.ServiceAssociatedDto.Description,
                ServiceImageUrl = GetImageForService(payment.ServiceDto),
                CardMask = payment.Card.MaskedNumber,
                ServiceAssosiatedId = payment.ServiceAssociatedId,
                AmountDolars = payment.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount),
                AmountPesos = payment.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount),
                ServiceContainerName = payment.ServiceDto.ServiceContainerName,
                Quotas = payment.Quotas,
            }).ToList();
            return PartialView("_PaymentHistory", paymentHistory);
        }

        public async Task<ActionResult> GetDebitAjax()
        {
            var user = await CurrentSelectedUser();

            var requests = await _webDebitClientService.GetDataForFromList(new DebitRequestsFilterDto()
            {
                DisplayLength = 4,
                DateFrom = default(DateTime),
                DateTo = DateTime.Now,
                UserId = user.Id,
            });

            requests = requests.OrderByDescending(x => x.CreationDate).ToList();

            //Muestro solo los dos pagos mas proximos
            //payments = payments.OrderByDescending(x => x.Date).Take(2).ToList();

            //Agrego cada pago realizado al historial de pagos
            var debitsHistory = requests.Select(x => x.ToModel()).ToList();

            return PartialView("_DebitHistory", debitsHistory);
        }

        [HttpPost]
        public async Task<ActionResult> CancelDebitRequest(DebitRequestsViewModel viewModel)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            try
            {
                var result = await _webDebitClientService.CancelDebitRequest(viewModel.DebitRequestIdToCancel);

                if (result)
                {
                    //message = PresentationWebStrings.DebitRequest_Cancellation;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = "No pudimos cancelar su solicitud. Intentá nuevamente o comunicate con el CallCenter";
                    title = "";
                    response = AjaxResponse.Error;
                    notification = NotificationType.Error;
                }

                return Json(new JsonResponse(response, new object(), message, title, notification), JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public async Task<ActionResult> DeleteDebitRequest(DebitRequestsViewModel viewModel)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            try
            {
                var result = await _webDebitClientService.CancelDebitRequest(viewModel.DebitRequestIdToCancel);

                if (result)
                {
                    //message = PresentationWebStrings.DebitRequest_Removed;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = "No pudimos eliminar el débito automático. Intentá nuevamente o comunicate con el CallCenter";
                    title = "";
                    response = AjaxResponse.Error;
                    notification = NotificationType.Error;
                }

                return Json(new JsonResponse(response, new object(), message, title, notification), JsonRequestBehavior.AllowGet);
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
    }
}