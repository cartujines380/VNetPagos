using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsTransactionsController : BaseController
    {
        private readonly IPaymentClientService _paymentClientService;
        private readonly IServiceClientService _serviceClientService;
        private readonly IServiceCategoryClientService _serviceCategoryClientService;
        private readonly IBinsClientService _binsClientService;
        private readonly IGatewayClientService _gatewayClientService;
        private readonly IServiceAssociatedClientService _serviceAssociatedClientService;

        public ReportsTransactionsController(IPaymentClientService paymentClientService,
            IServiceClientService serviceClientService,
            IServiceCategoryClientService serviceCategoryClientService,
            IBinsClientService binsClientService,
            IGatewayClientService gatewayClientService, IServiceAssociatedClientService serviceAssociatedClientService)
        {
            _paymentClientService = paymentClientService;
            _serviceClientService = serviceClientService;
            _serviceCategoryClientService = serviceCategoryClientService;
            _binsClientService = binsClientService;
            _gatewayClientService = gatewayClientService;
            _serviceAssociatedClientService = serviceAssociatedClientService;
        }

        [CustomAuthentication(Actions.ReportsTransactionsDetails)]
        public async Task<ActionResult> Index()
        {
            var filters = new ReportsTransactionsFilterDto()
            {
                DateFrom = DateTime.Today.AddMonths(-1),
                DateTo = DateTime.Today,
            };
            var applyFilters = false;

            var gateways = await _gatewayClientService.FindAll();
            ViewBag.Gateways = new SelectList(gateways, "Id", "Name").OrderBy(item => item.Text);

            var services = await _serviceClientService.FindAll();
            ViewBag.Services = new SelectList(services, "Id", "Name").OrderBy(item => item.Text);

            var serviceCategories = await _serviceCategoryClientService.FindAll();
            ViewBag.ServiceCategories = new SelectList(serviceCategories, "Id", "Name").OrderBy(item => item.Text);

            ViewBag.PaymentStatus = GeneratePaymentStatusList();
            ViewBag.PaymentPlataform = GeneratePaymentPlataformList();

            var serviceId = Request["ServiceId"];
            var serviceAssociatedId = Request["ServiceAssociatedId"];
            var paymentType = Request["PaymentType"];
            var paymentTransactionNumber = Request["PaymentTransactionNumber"];
            var paymentGatewayTransactionNumber = Request["PaymentGatewayTransactionNumber"];
            var clientEmail = Request["Email"];
            var dateTo = Request["DateFrom"];
            var dateFrom = Request["DateTo"];
            var platform = Request["Platform"];
            var exec = Request["Exec"];

            ViewBag.AutoExecute = !string.IsNullOrEmpty(exec) && exec == "auto";

            if (!string.IsNullOrEmpty(dateTo))
            {
                filters.DateFrom = DateTime.ParseExact(dateTo, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                filters.DateTo = DateTime.ParseExact(dateFrom, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(paymentTransactionNumber))
            {
                filters.PaymentTransactionNumber = paymentTransactionNumber;
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(paymentGatewayTransactionNumber))
            {
                filters.PaymentUniqueIdentifier = Int64.Parse(paymentGatewayTransactionNumber);
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(clientEmail))
            {
                filters.ClientEmail = clientEmail;
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(serviceId))
            {
                filters.ServiceId = new Guid(serviceId);
                applyFilters = true;
            }
            if (!string.IsNullOrEmpty(platform))
            {
                filters.Platform = int.Parse(platform);
                applyFilters = true;
            }

            if (!string.IsNullOrEmpty(serviceAssociatedId))
            {
                applyFilters = true;
                var serviceAssociated = await _serviceAssociatedClientService.Find(new Guid(serviceAssociatedId));

                var email = serviceAssociated.RegisteredUserDto.Email;
                var serviceNameAndDesc = !String.IsNullOrEmpty(serviceAssociated.Description) ? serviceAssociated.ServiceDto.Name + " - " + serviceAssociated.Description : serviceAssociated.ServiceDto.Name;

                filters.ServiceAssociatedId = new Guid(serviceAssociatedId);
                ViewBag.ServiceNameAndDesc = serviceNameAndDesc;
                filters.ClientEmail = email;
                filters.ServiceId = serviceAssociated.ServiceId;
                if (!String.IsNullOrEmpty(paymentType))
                    filters.PaymentType = Convert.ToInt32(paymentType);

                filters.DateFrom = DateTime.Today.AddMonths(-1);
                filters.DateTo = DateTime.Today;
            }

            if (applyFilters)
            {
                return View(filters);
            }
            return View(new ReportsTransactionsFilterDto()
            {
                DateFrom = DateTime.Today.AddMonths(-1),
                DateTo = DateTime.Today,
            });
        }

        [CustomAuthentication(Actions.ReportsTransactionsDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerTransactions(Request, param);

            var transactions = await _paymentClientService.ReportsTransactionsDataFromDbView(filter);

            var totalRecords = await _paymentClientService.ReportsTransactionsDataCount(filter);

            var bins = await _binsClientService.FindAll();

            var items = transactions.Select(d => d.ToModel(bins));

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = items.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsTransactionsDetails)]
        public async Task<ActionResult> ExcelExport(FormCollection collection)
        {
            try
            {
                var filterNamesDictionary = await FindFilterNames(collection);
                string[] filterHeaders;
                string[] filterData;

                var filter = LoadFiltersForExcelExport(collection, filterNamesDictionary, out filterHeaders, out filterData);

                var transactions = await _paymentClientService.ReportsTransactionsDataFromDbView(filter);

                var bins = await _binsClientService.FindAll();

                var items = transactions.Select(d => d.ToExcelModel(bins));

                var data = from p in items
                           select new
                           {
                               p.PaymentDate,

                               p.UserEmail,
                               p.UserName,
                               p.UserSurname,

                               p.GatewayName,
                               p.PaymentUniqueIdentifier,
                               p.TransactionNumber,
                               p.PaymentType,

                               p.ServiceName,
                               p.ServiceCategoryName,
                               p.ReferenceNumbers,
                               p.ServiceAssociatedDescription,

                               p.CardMaskedNumber,
                               p.CardDueDate,
                               p.CardType,

                               p.BillExternalId,
                               p.BillExpirationDate,
                               p.BillFinalConsumer,
                               p.BillCurrency,
                               p.BillAmount,
                               p.BillTaxedAmount,
                               p.BillDiscount,
                               p.BillDiscountAmount,

                               p.PaymentStatus,

                               p.BillSucivePreBillNumber,
                               p.CSTransactionIdentifier,
                               p.GatewayTransactionId,
                               p.PaymentCSRequestCurrency,
                               p.PaymentCSAuthCode,
                               p.PaymentCSAuthTime,
                               p.PaymentAmountToCS,

                               p.PaymentTotalAmount,
                               p.PaymentTaxedAmount,
                               p.PaymentDiscount,
                           };

                var headers = new[]
                    {
                        EntitiesDtosStrings.PaymentDto_Date,

                        EntitiesDtosStrings.PaymentDto_ClientEmail,
                        EntitiesDtosStrings.PaymentDto_ClientName,
                        EntitiesDtosStrings.PaymentDto_ClientSurname,

                        EntitiesDtosStrings.PaymentDto_Gateway,
                        EntitiesDtosStrings.PaymentDto_UniqueIdentifier,
                        EntitiesDtosStrings.PaymentDto_TransactionNumber,
                        EntitiesDtosStrings.PaymentDto_PaymentType,

                        EntitiesDtosStrings.PaymentDto_ServiceName,
                        EntitiesDtosStrings.PaymentDto_ServiceCategoryName,
                        EntitiesDtosStrings.PaymentDto_ReferenceNumbers,
                        EntitiesDtosStrings.PaymentDto_ServiceAssociatedDescription,

                        EntitiesDtosStrings.PaymentDto_CardMaskedNumber,
                        EntitiesDtosStrings.PaymentDto_CardDueDate,
                        EntitiesDtosStrings.PaymentDto_CardType,

                        EntitiesDtosStrings.PaymentDto_BillExternalId,
                        EntitiesDtosStrings.PaymentDto_BillExpirationDate,
                        EntitiesDtosStrings.PaymentDto_BillFinalConsumer,
                        EntitiesDtosStrings.PaymentDto_BillCurrency,
                        EntitiesDtosStrings.PaymentDto_BillAmount,
                        EntitiesDtosStrings.PaymentDto_BillTaxedAmount,
                        EntitiesDtosStrings.PaymentDto_BillDiscountApplied,
                        EntitiesDtosStrings.PaymentDto_BillDiscountAmount,

                        EntitiesDtosStrings.PaymentDto_PaymentStatus,

                        EntitiesDtosStrings.PaymentDto_SucivePreBillNumber,
                        EntitiesDtosStrings.PaymentDto_CSTransactionIdentifier,
                        EntitiesDtosStrings.PaymentDto_GatewayTransactionId,
                        EntitiesDtosStrings.PaymentDto_CSRequestCurrency,
                        EntitiesDtosStrings.PaymentDto_CSAuthCode,
                        EntitiesDtosStrings.PaymentDto_CSAuthTime,
                        EntitiesDtosStrings.PaymentDto_AmountToCS,

                        EntitiesDtosStrings.PaymentDto_TotalAmount,
                        EntitiesDtosStrings.PaymentDto_TaxedAmount,
                        EntitiesDtosStrings.PaymentDto_Discount,
                    };

                var memoryStream = ExcelExporter.ExcelExportTransactions("Transacciones", data, headers, filterHeaders, filterData);

                var filePath = ConfigurationManager.AppSettings["TransReports"];
                var fileName = string.Format("{0}_{1}.{2}", DateTime.Now.ToString("yyyyMMdd"), "Reporte_Transacciones", "xlsx");

                string fullPath = Path.Combine(filePath, fileName);
                System.IO.File.WriteAllBytes(fullPath, memoryStream);

                return Json(new JsonResponse(AjaxResponse.Success, fileName, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null, "", "", NotificationType.Error), JsonRequestBehavior.AllowGet);
                throw;
            }
        }

        [CustomAuthentication(Actions.ReportsTransactionsDetails)]
        public async Task<ActionResult> DownloadTicket(Guid id, string transactionNumber)
        {
            var arrbytes = await _paymentClientService.DownloadTicket(id, transactionNumber, Guid.Empty);

            return File(arrbytes, "application/PDF", string.Format("Ticket_{0}.pdf", transactionNumber));
        }

        private List<SelectListItem> GeneratePaymentStatusList()
        {
            var list = Enum.GetValues(typeof(PaymentStatusDto)).Cast<PaymentStatusDto>();
            return list.Select(paymentStatus => new SelectListItem()
            {
                Text = paymentStatus.ToString(),
                Value = (int)paymentStatus + "",
            }).ToList();
        }
        private List<SelectListItem> GeneratePaymentPlataformList()
        {
            var list = Enum.GetValues(typeof(PaymentPlatformDto)).Cast<PaymentPlatformDto>();
            return list.Select(paymentStatus => new SelectListItem()
            {
                Text = paymentStatus.ToString(),
                Value = (int)paymentStatus + "",
            }).ToList();
        }

        private ReportsTransactionsFilterDto LoadFiltersForExcelExport(FormCollection collection, IDictionary<string, string> filterNamesDictionary, out string[] filterHeaders, out string[] filterData)
        {
            var dateFrom = !String.IsNullOrEmpty(collection["DateFrom"])
                ? Convert.ToDateTime(collection["DateFrom"])
                : DateTime.MinValue;
            var dateTo = !String.IsNullOrEmpty(collection["DateTo"])
                ? Convert.ToDateTime(collection["DateTo"])
                : DateTime.MinValue;

            var filter = new ReportsTransactionsFilterDto
            {
                DisplayLength = 10,
                DateFrom = dateFrom,
                DateFromString = dateFrom != DateTime.MinValue ? dateFrom.ToString("dd/MM/yyyy") : "",
                DateTo = dateTo,
                DateToString = dateTo != DateTime.MinValue ? dateTo.ToString("dd/MM/yyyy") : "",
                ClientEmail = collection["ClientEmail"],
                ClientName = collection["ClientName"],
                ClientSurname = collection["ClientSurname"],
                PaymentTransactionNumber = collection["PaymentTransactionNumber"],
                PaymentUniqueIdentifier = !String.IsNullOrEmpty(collection["PaymentUniqueIdentifier"]) ? Convert.ToInt64(collection["PaymentUniqueIdentifier"]) : (long?)null,
                GatewayId = !String.IsNullOrEmpty(collection["GatewayId"]) ? Guid.Parse(collection["GatewayId"]) : (Guid?)null,//Guid.Empty,
                PaymentType = !String.IsNullOrEmpty(collection["PaymentType"]) ? Convert.ToInt32(collection["PaymentType"]) : 0,
                ServiceId = !String.IsNullOrEmpty(collection["ServiceId"]) ? Guid.Parse(collection["ServiceId"]) : (Guid?)null,//Guid.Empty,
                ServiceCategoryId = !String.IsNullOrEmpty(collection["ServiceCategoryId"]) ? Guid.Parse(collection["ServiceCategoryId"]) : (Guid?)null,// Guid.Empty,
                ServiceAssociatedId = !String.IsNullOrEmpty(collection["ServiceAssociatedId"]) ? Guid.Parse(collection["ServiceAssociatedId"]) : (Guid?)null,//Guid.Empty,
                PaymentStatus = !String.IsNullOrEmpty(collection["PaymentStatus"]) ? Convert.ToInt32(collection["PaymentStatus"]) : (int?)null,
                //ordenar por PaymentDate Descendiente
                OrderBy = "0",
                SortDirection = SortDirection.Desc
            };

            filterHeaders = new[]
            {
                PresentationAdminStrings.ExcelTransactions_FilterDateFrom,
                PresentationAdminStrings.ExcelTransactions_FilterDateTo,
                PresentationAdminStrings.ExcelTransactions_FilterUserEmail,
                PresentationAdminStrings.ExcelTransactions_FilterUserName,
                PresentationAdminStrings.ExcelTransactions_FilterUserSurname,
                PresentationAdminStrings.ExcelTransactions_FilterTransactionNumber,
                PresentationAdminStrings.ExcelTransactions_FilterUniqueIdentifier,
                PresentationAdminStrings.ExcelTransactions_FilterGateway,
                PresentationAdminStrings.ExcelTransactions_FilterPaymentType,
                PresentationAdminStrings.ExcelTransactions_FilterServiceName,
                PresentationAdminStrings.ExcelTransactions_FilterServiceCategory,
                PresentationAdminStrings.ExcelTransactions_FilterPaymentStatus
            };

            filterData = new[]
            {
                filter.DateFromString,
                filter.DateToString,
                filter.ClientEmail,
                filter.ClientName,
                filter.ClientSurname,
                filter.PaymentTransactionNumber,
                filter.PaymentUniqueIdentifier != 0 ? filter.PaymentUniqueIdentifier.ToString() : "",
                filterNamesDictionary["GatewayName"],
                filter.PaymentType != 0 ?
                    EnumHelpers.GetName(typeof(PaymentTypeDto), filter.PaymentType, EnumsStrings.ResourceManager) : "",
                filterNamesDictionary["ServiceName"],
                filterNamesDictionary["ServiceCategoryName"],
                filter.PaymentStatus != null ? ((PaymentStatusDto)filter.PaymentStatus).ToString() : "",
            };

            return filter;
        }

        private async Task<IDictionary<string, string>> FindFilterNames(FormCollection collection)
        {
            var filtersNames = new Dictionary<string, string>();

            var gatewayId = !String.IsNullOrEmpty(collection["GatewayId"])
                ? Guid.Parse(collection["GatewayId"])
                : Guid.Empty;

            var serviceId = !String.IsNullOrEmpty(collection["ServiceId"])
                ? Guid.Parse(collection["ServiceId"])
                : Guid.Empty;

            var serviceCategoryId = !String.IsNullOrEmpty(collection["ServiceCategoryId"])
                ? Guid.Parse(collection["ServiceCategoryId"])
                : Guid.Empty;

            var gatewayName = "";
            if (gatewayId != Guid.Empty)
            {
                var gateways = await _gatewayClientService.FindAll();
                var gateway = gateways.FirstOrDefault(x => x.Id == gatewayId);
                if (gateway != null)
                {
                    gatewayName = gateway.Name;
                }
            }
            filtersNames.Add("GatewayName", gatewayName);

            var serviceName = "";
            if (serviceId != Guid.Empty)
            {
                var service = await _serviceClientService.Find(serviceId);
                if (service != null)
                {
                    serviceName = service.Name;
                }
            }
            filtersNames.Add("ServiceName", serviceName);

            var serviceCategoryName = "";
            if (serviceCategoryId != Guid.Empty)
            {
                var serviceCategory = await _serviceCategoryClientService.Find(serviceCategoryId);
                if (serviceCategory != null)
                {
                    serviceCategoryName = serviceCategory.Name;
                }
            }
            filtersNames.Add("ServiceCategoryName", serviceCategoryName);

            return filtersNames;
        }

        public ActionResult DownloadExcel(string fileName)
        {
            var filePath = ConfigurationManager.AppSettings["TransReports"];
            var fullName = Path.Combine(filePath, fileName);
            var data = System.IO.File.OpenRead(fullName);
            return File(data, WebConfigurationManager.AppSettings["ExcelResponseType"], fileName);
        }

        [CustomAuthentication(Actions.ReportsTransactionCancellation)]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "No se pudo cancelar la transacción.";
            var title = "Cancelación fallida.";

            try
            {
                var notify = Boolean.Parse(Request.Form["Notify"]);
                var csOperationData = await _paymentClientService.CancelPayment(new CancelTrnsDto()
                {
                    PaymentId = id,
                    Notify = notify,
                });

                if (csOperationData.VoidData != null)
                {
                    if (csOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        if (csOperationData.ReversalData != null && csOperationData.ReversalData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            response = AjaxResponse.Success;
                            notification = NotificationType.Success;
                            title = "Cancelación exitosa.";
                            message = string.Format(
                                "Se realizo la cancelación de la transacción. Quedo en estado {0}", PaymentStatus.Reversed.ToString());
                        }
                        else
                        {
                            response = AjaxResponse.Success;
                            notification = NotificationType.Success;
                            title = "Cancelación exitosa.";
                            message = string.Format(
                                "Se realizo la cancelación de la transacción. Quedo en estado {0}. El Reverso fallo con codigo de error: {1}", PaymentStatus.Voided.ToString(),
                                csOperationData.ReversalData != null ? csOperationData.ReversalData.PaymentResponseCode.ToString() : string.Empty);
                        }
                    }
                    if (csOperationData.VoidData.PaymentResponseCode != (int)CybersourceMsg.Accepted)
                    {
                        //SI NO SE HIZO EL VOID, SE INTENTA EL REFUND
                        if (csOperationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                        {
                            response = AjaxResponse.Success;
                            notification = NotificationType.Success;
                            title = "Cancelación exitosa.";
                            message = string.Format(
                                "Se realizo la cancelación de la transacción. Quedo en estado {0}",
                                PaymentStatus.Refunded.ToString());
                        }
                        else
                        {
                            message = message + string.Format(" El void dio codigo de error: {0}", csOperationData.VoidData.PaymentResponseCode);
                            message = message + string.Format(" El refund dio codigo de error: {0}", csOperationData.RefundData.PaymentResponseCode);
                        }
                    }
                }

                return
                Json(new JsonResponse(response, string.Empty, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException exception)
            {
                message = message + string.Format(" " + exception.Message);
                NLogLogger.LogEvent(exception);
            }
            catch (Exception exception)
            {
                message = message + "Se ha producido un error. ";
                NLogLogger.LogEvent(exception);
            }
            return
                Json(new JsonResponse(response, string.Empty, message, title, notification), JsonRequestBehavior.AllowGet);
        }

    }
}