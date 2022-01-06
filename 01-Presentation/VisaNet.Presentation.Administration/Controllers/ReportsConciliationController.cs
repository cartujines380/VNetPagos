using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsConciliationController : BaseController
    {
        private readonly IConciliationSummaryClientService _conciliationSummaryClientService;
        private readonly IPaymentClientService _paymentClientService;
        private readonly ILogClientService _logClientService;

        public ReportsConciliationController(IConciliationSummaryClientService conciliationSummaryClientService,
            IPaymentClientService paymentClientService, ILogClientService logClientService)
        {
            _conciliationSummaryClientService = conciliationSummaryClientService;
            _paymentClientService = paymentClientService;
            _logClientService = logClientService;
        }

        [CustomAuthentication(Actions.ReportsConciliationDetails)]
        public ActionResult Index()
        {
            return View(new ReportsConciliationFilterDto()
                        {
                            From = new DateTime(2015, 01, 01),
                            To = DateTime.Now.AddDays(-2),
                            SortDirection = SortDirection.Desc,
                            DisplayLength = 15,
                            DisplayStart = 0,
                        });
        }

        [CustomAuthentication(Actions.ReportsConciliationDetails)]
        [HttpPost]
        public ActionResult Index(ReportsConciliationFilterDto model)
        {
            ViewBag.Apps = model.Applications;
            ViewBag.States = model.State;
            return View(model);
        }

        [CustomAuthentication(Actions.ReportsConciliationDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsConciliation(Request, param);

            filter.DisplayLength = 100;

            var data = await _conciliationSummaryClientService.GetConciliationSummary(filter);

            var dataModel = data.OrderByDescending(d => d.Date).Select(d => new
            {
                ConciliationSummaryId = d.Id,
                Date = d.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                TransactionNumber = d.TransactionNumber,
                UniqueIdenfifier = d.VisaTransactionId,
                State = (int)d.State,
                ConciliationType = (int)d.Type,
                CybersourceState = (int)d.ConciliationCybersourceSummaryState,
                Gateway = d.ConciliationGatewaySummaryGateway,
                GatewayState = (int)d.ConciliationGatewaySummaryState,
                VisaNetState = (int)d.ConciliationVisaNetSummaryState,
                BatchState = (int)d.ConciliationBatchSummaryState,
                ConciliationPortalState = (int)d.ConciliationPortalState,
                RowsCount = d.RowsCount,
                TransactionType = d.TransactionType
            }).ToList();

            var rowsCount = dataModel.FirstOrDefault() != null ? dataModel.FirstOrDefault().RowsCount : 0;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = rowsCount,
                iTotalDisplayRecords = rowsCount,
                aaData = dataModel
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsConciliationDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var conciliation = await _conciliationSummaryClientService.GetConciliationSummary(id);

                PaymentDto payment = null;
                if (!String.IsNullOrEmpty(conciliation.TransactionNumber))
                {
                    payment = await _paymentClientService.GetByTransactionNumber(conciliation.TransactionNumber);
                }

                LogDto log = null;
                //TODO: se saca porque da timeout
                //if (!String.IsNullOrEmpty(conciliation.TransactionNumber))
                //{
                //    log = await _logClientService.Find(conciliation.TransactionNumber);
                //}

                ConciliationCybersourceDto conciliationCybersource = null;
                if (conciliation.ConciliationCybersourceSummary.ConciliationCybersourceId != default(Guid))
                {
                    conciliationCybersource = await _conciliationSummaryClientService.GetConciliationCybersource(conciliation.ConciliationCybersourceSummary.ConciliationCybersourceId);
                }

                ConciliationBanredDto conciliationBanred = null;
                ConciliationSistarbancDto conciliationSistarbanc = null;
                ConciliationSuciveDto conciliationSucive = null;
                if (conciliation.ConciliationGatewaySummary.ConciliationGatewayId != Guid.Empty)
                {
                    if (conciliation.ConciliationGatewaySummary.Gateway == "Banred")
                    {
                        conciliationBanred = await _conciliationSummaryClientService.GetConciliationBanred(conciliation.ConciliationGatewaySummary.ConciliationGatewayId);
                    }
                    if (conciliation.ConciliationGatewaySummary.Gateway == "Sistarbanc")
                    {
                        conciliationSistarbanc = await _conciliationSummaryClientService.GetConciliationSistarbanc(conciliation.ConciliationGatewaySummary.ConciliationGatewayId);
                    }
                    if (conciliation.ConciliationGatewaySummary.Gateway == "Sucive")
                    {
                        conciliationSucive = await _conciliationSummaryClientService.GetConciliationSucive(conciliation.ConciliationGatewaySummary.ConciliationGatewayId);
                    }
                }

                ConciliationVisanetDto conciliationVisanet = null;
                if (conciliation.ConciliationVisaNetSummary.ConciliationVisaNetId != default(Guid))
                {
                    conciliationVisanet = await _conciliationSummaryClientService.GetConciliationVisanet(conciliation.ConciliationVisaNetSummary.ConciliationVisaNetId);
                }

                ConciliationVisanetCallbackDto conciliationBatch = null;
                if (conciliation.ConciliationBatchSummary.ConciliationVisaNetCallbackId != default(Guid))
                {
                    conciliationBatch = await _conciliationSummaryClientService.GetConciliationBatch(conciliation.ConciliationBatchSummary.ConciliationVisaNetCallbackId);
                }


                var model = new ConciliationSummaryModel
                {
                    ConciliationSummaryId = id,
                    Payment = payment,
                    Log = log,
                    ConciliationType = conciliation.Type,
                    ConciliationCybersource = conciliationCybersource,
                    CybersourceState = conciliation.ConciliationCybersourceSummary.State,
                    ConciliationBanred = conciliationBanred,
                    ConciliationSistarbanc = conciliationSistarbanc,
                    GatewayState = conciliation.ConciliationGatewaySummary.State,
                    TransactionType = conciliation.TransactionType,
                    ConciliationSucive = conciliationSucive,
                    ConciliationVisanet = conciliationVisanet,
                    VisanetState = conciliation.ConciliationVisaNetSummary.State,
                    GeneralComment = conciliation.GeneralComment,
                    PortalState = conciliation.ConciliationPortalState,
                    ConciliationVisanetCallback = conciliationBatch,
                    BatchState = conciliation.ConciliationBatchSummary.State,
                    ConciliationSummeryGatewayName = conciliation.ConciliationGatewaySummary.Gateway
                };

                var content = RenderPartialViewToString("_Details", model);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success),
                    JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        [CustomAuthentication(Actions.ReportsConciliationDetails)]
        public async Task<ActionResult> CheckConciliationSummary(Guid id, bool portal, bool cybersource, bool gateway,
            bool visanet, bool batch, string comment)
        {
            try
            {
                var conciliation = await _conciliationSummaryClientService.GetConciliationSummary(id);

                if (portal)
                {
                    if (conciliation.ConciliationPortalState == ConciliationStateDto.NotFound)
                    {
                        conciliation.ConciliationPortalState = ConciliationStateDto.Checked;
                    }
                }

                //Si los parametros estan en true cambio el estado a checked. Sino comparo los datos para evaluar si es ok, error o not found.
                if (cybersource)
                {
                    if (conciliation.ConciliationCybersourceSummary.State == ConciliationStateDto.NotFound ||
                        conciliation.ConciliationCybersourceSummary.State == ConciliationStateDto.Difference)
                    {
                        conciliation.ConciliationCybersourceSummary.State = ConciliationStateDto.Checked;
                    }
                }

                if (gateway)
                {
                    if (conciliation.ConciliationGatewaySummary.State == ConciliationStateDto.NotFound ||
                        conciliation.ConciliationGatewaySummary.State == ConciliationStateDto.Difference)
                    {
                        conciliation.ConciliationGatewaySummary.State = ConciliationStateDto.Checked;
                    }
                }

                if (visanet)
                {
                    if (conciliation.ConciliationVisaNetSummary.State == ConciliationStateDto.NotFound ||
                        conciliation.ConciliationVisaNetSummary.State == ConciliationStateDto.Difference)
                    {
                        conciliation.ConciliationVisaNetSummary.State = ConciliationStateDto.Checked;
                    }
                }

                if (batch)
                {
                    if (conciliation.ConciliationBatchSummary.State == ConciliationStateDto.NotFound ||
                        conciliation.ConciliationBatchSummary.State == ConciliationStateDto.Difference)
                    {
                        conciliation.ConciliationBatchSummary.State = ConciliationStateDto.Checked;
                    }
                }

                //Actualizo estado general
                if (conciliation.ConciliationPortalState == ConciliationStateDto.NotFound ||
                    conciliation.ConciliationCybersourceSummary.State == ConciliationStateDto.NotFound ||
                    conciliation.ConciliationGatewaySummary.State == ConciliationStateDto.NotFound ||
                    conciliation.ConciliationBatchSummary.State == ConciliationStateDto.NotFound ||
                    conciliation.ConciliationVisaNetSummary.State == ConciliationStateDto.NotFound)
                {
                    conciliation.State = ConciliationStateDto.NotFound;
                }
                else
                {
                    if (conciliation.ConciliationPortalState == ConciliationStateDto.Difference ||
                        conciliation.ConciliationCybersourceSummary.State == ConciliationStateDto.Difference ||
                        conciliation.ConciliationGatewaySummary.State == ConciliationStateDto.Difference ||
                        conciliation.ConciliationBatchSummary.State == ConciliationStateDto.Difference ||
                        conciliation.ConciliationVisaNetSummary.State == ConciliationStateDto.Difference)
                    {
                        conciliation.State = ConciliationStateDto.Difference;
                    }
                    else
                    {
                        if (conciliation.ConciliationPortalState == ConciliationStateDto.Checked ||
                            conciliation.ConciliationCybersourceSummary.State == ConciliationStateDto.Checked ||
                            conciliation.ConciliationGatewaySummary.State == ConciliationStateDto.Checked ||
                            conciliation.ConciliationBatchSummary.State == ConciliationStateDto.Checked ||
                            conciliation.ConciliationVisaNetSummary.State == ConciliationStateDto.Checked)
                        {
                            conciliation.State = ConciliationStateDto.Checked;
                        }
                        else
                        {
                            conciliation.State = ConciliationStateDto.Ok;
                        }
                    }
                }

                conciliation.GeneralComment = comment;
                await _conciliationSummaryClientService.Edit(conciliation);

                return Json(new JsonResponse(AjaxResponse.Success, null, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
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