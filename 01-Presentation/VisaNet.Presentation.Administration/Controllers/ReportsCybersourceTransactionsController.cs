using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsCybersourceTransactionsController : BaseController
    {
        private readonly IReportsClientService _reportsClientService;

        public ReportsCybersourceTransactionsController(IReportsClientService reportsClientService)
        {
            _reportsClientService = reportsClientService;
        }

        //
        // GET: /ReportsCybersourceTransactions/
        [CustomAuthentication(Actions.ReportsCybersourceTransactionsDetails)]
        public ActionResult Index()
        {
            return View(new ReportsCybersourceTransactionsFilterDto()
                        {
                            SortDirection = SortDirection.Desc,
                        });
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsCybersourceTransactionsDetails)]
        public async Task<ActionResult> GetTable(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            try
            {
                filtersDto.Sale = false;
                var data = await _reportsClientService.GetCybersourceTransactionsData(filtersDto);
                ViewBag.TransactionsName = PresentationAdminStrings.Cybersource_Transactions_CreateToken;
                ViewBag.Index = 1;
                var content = RenderPartialViewToString("_CybersourceTransactionsList", data);

                filtersDto.Sale = true;
                var data2 = await _reportsClientService.GetCybersourceTransactionsData(filtersDto);
                ViewBag.TransactionsName = PresentationAdminStrings.Cybersource_Transactions_Payment;
                ViewBag.Index = 4;
                var content2 = RenderPartialViewToString("_CybersourceTransactionsList", data2);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success, null, content2));
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

        [CustomAuthentication(Actions.ReportsCybersourceTransactionsDetails)]
        public async Task<ActionResult> ExcelExport(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            var items = await _reportsClientService.GetCybersourceTransactionsDetails(filtersDto);

            var data = from p in items
                       orderby p.TransactionDateTime
                       select new
                       {
                           TransactionDateTime = p.TransactionDateTime.ToString(),
                           p.CyberSourceLogData.Decision,
                           p.CyberSourceLogData.ReasonCode,
                           p.CyberSourceLogData.Message,
                           p.CyberSourceLogData.TransactionId,
                           TransactionType = !String.IsNullOrEmpty(p.CyberSourceLogData.ReqTransactionType) ? p.CyberSourceLogData.ReqTransactionType : p.CyberSourceLogData.TransactionType.ToString().ToLower(),
                           p.CyberSourceLogData.ReqCardNumber,
                           p.CyberSourceLogData.ReqCardExpiryDate,
                           p.CyberSourceLogData.ReqCurrency,
                           p.CyberSourceLogData.ReqAmount
                       };

            var headers = new[]
                {
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_TransactionDateTime,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_Decision,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReasonCode,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_Message,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_TransactionId,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReqTransactionType,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReqCardNumber,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReqCardExpiryDate,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReqCurrency,
                    EntitiesDtosStrings.LogPaymentCyberSourceDto_ReqAmount
                };

            var memoryStream = ExcelExporter.ExcelExport("Transacciones", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"], string.Format("{0}.{1}", "Reporte_Transacciones", "xlsx"));
        }
    }
}