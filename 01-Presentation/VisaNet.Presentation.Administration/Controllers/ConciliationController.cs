using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ConciliationController : BaseController
    {
        private readonly IConciliationDailySummaryClientService _conciliationDailyService;
        private readonly IConciliationVNPClientService _conciliationVnpService;

        private readonly string _banredBlobFolder = ConfigurationManager.AppSettings["AzureConciliationBanredUnprocessedFolder"];
        private readonly string _visanetCallbackBlobFolder = ConfigurationManager.AppSettings["AzureConciliationVisanetCallbackUnprocessedFolder"];
        private readonly string _sistarbancBlobFolder = ConfigurationManager.AppSettings["AzureConciliationSistarbancUnprocessedFolder"];

        public ConciliationController(IConciliationDailySummaryClientService conciliationDailyService,
            IConciliationVNPClientService conciliationVnpService)
        {
            _conciliationDailyService = conciliationDailyService;
            _conciliationVnpService = conciliationVnpService;
        }

        [CustomAuthentication(Actions.ConciliationDailyList)]
        public ActionResult Index()
        {
            return View(new DailyConciliationFilterDto
            {
                From = DateTime.Now.AddDays(-7),
                To = DateTime.Now,
                SortDirection = SortDirection.Desc
            });
        }

        [CustomAuthentication(Actions.ConciliationDailyList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerDailyConciliation(Request, param);

            var data = await _conciliationDailyService.GetConciliationDailySummary(filter);

            var dataModel = data.OrderByDescending(d => d.Date).Select(d => new
            {
                Date = d.Date.ToString("dd/MM/yyyy"),
                d.TotalPortal,
                d.TotalExternas,
                d.PortalState,
                d.CybersourceState,
                d.BanredState,
                d.SistarbancState,
                d.SuciveState,
                d.Tc33State,
                d.BatchState,

                d.SiteRojas,
                d.SiteAmarillas,
                d.SiteVerdes,
                d.SiteAzules,
                d.SiteNoaplica,

                d.CsRojas,
                d.CsAmarillas,
                d.CsVerdes,
                d.CsAzules,
                d.CsDetailVNP,
                d.CsDetailTotalCs,
                d.CsDetailOk,
                d.CsDetailDif,
                d.CsDetailRev,
                d.CsDetailSiteNoCs,
                d.CsDetailCsNoSite,

                d.BrRojas,
                d.BrAmarillas,
                d.BrVerdes,
                d.BrAzules,
                d.BrDetailVNP,
                d.BrDetailTotalBr,
                d.BrDetailOk,
                d.BrDetailDif,
                d.BrDetailRev,
                d.BrDetailSiteNoBr,
                d.BrDetailBrNoSite,

                d.SbRojas,
                d.SbAmarillas,
                d.SbVerdes,
                d.SbAzules,
                d.SbDetailVNP,
                d.SbDetailTotalSb,
                d.SbDetailOk,
                d.SbDetailDif,
                d.SbDetailRev,
                d.SbDetailSiteNoSb,
                d.SbDetailSbNoSite,

                d.SuRojas,
                d.SuAmarillas,
                d.SuVerdes,
                d.SuAzules,
                d.SuDetailVNP,
                d.SuDetailTotalSu,
                d.SuDetailOk,
                d.SuDetailDif,
                d.SuDetailRev,
                d.SuDetailSiteNoSu,
                d.SuDetailSuNoSite,

                d.TcRojas,
                d.TcAmarillas,
                d.TcVerdes,
                d.TcAzules,
                d.TcDetailVNP,
                d.TcDetailTotalTc,
                d.TcDetailOk,
                d.TcDetailDif,
                d.TcDetailRev,
                d.TcDetailSiteNoTc,
                d.TcDetailTcNoSite,
                d.TcDetailExtVNP,
                d.TcDetailExtTotalTc,
                d.TcDetailExtOk,
                d.TcDetailExtDif,
                d.TcDetailExtRev,
                d.TcDetailExtSiteNoTc,
                d.TcDetailTcExtNoSite,

                d.BatRojas,
                d.BatAmarillas,
                d.BatVerdes,
                d.BatAzules,
                d.BatDetailVNP,
                d.BatDetailTotalBat,
                d.BatDetailOk,
                d.BatDetailDif,
                d.BatDetailRev,
                d.BatDetailSiteNoBat,
                d.BatDetailBatNoSite,

            }).ToList();

            var dataToShow = dataModel.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = dataModel.Count,
                iTotalDisplayRecords = dataModel.Count,
                aaData = dataToShow.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ConciliationDailyList)]
        [HttpPost]
        public ActionResult DailyDetail(DailyConciliationModel model)
        {
            var vm = new DailyConciliationDetailModel
            {
                ConciliationApp = model.App
            };
            switch (model.App)
            {
                case ConciliationAppDto.Sistarbanc:
                    vm.TotalVnp = model.Detail.SbDetailVNP;
                    vm.TotalGetaway = model.Detail.SbDetailTotalSb;
                    vm.TotalOk = model.Detail.SbDetailOk;
                    vm.TotalDiff = model.Detail.SbDetailDif;
                    vm.TotalChecked = model.Detail.SbDetailRev;
                    vm.GetawayNoSite = model.Detail.SbDetailSbNoSite;
                    vm.SiteNoGetaway = model.Detail.SbDetailSiteNoSb;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Sistarbanc;
                    return PartialView("_Getaway", vm);
                case ConciliationAppDto.Batch:
                    vm.TotalVnp = model.Detail.BatDetailVNP;
                    vm.TotalGetaway = model.Detail.BatDetailTotalBat;
                    vm.TotalOk = model.Detail.BatDetailOk;
                    vm.TotalDiff = model.Detail.BatDetailDif;
                    vm.TotalChecked = model.Detail.BatDetailRev;
                    vm.GetawayNoSite = model.Detail.BatDetailBatNoSite;
                    vm.SiteNoGetaway = model.Detail.BatDetailSiteNoBat;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Batch;
                    return PartialView("_Getaway", vm);
                case ConciliationAppDto.Banred:
                    vm.TotalVnp = model.Detail.BrDetailVNP;
                    vm.TotalGetaway = model.Detail.BrDetailTotalBr;
                    vm.TotalOk = model.Detail.BrDetailOk;
                    vm.TotalDiff = model.Detail.BrDetailDif;
                    vm.TotalChecked = model.Detail.BrDetailRev;
                    vm.GetawayNoSite = model.Detail.BrDetailBrNoSite;
                    vm.SiteNoGetaway = model.Detail.BrDetailSiteNoBr;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Banred;
                    return PartialView("_Getaway", vm);
                case ConciliationAppDto.CyberSource:
                    vm.TotalVnp = model.Detail.CsDetailVNP;
                    vm.TotalGetaway = model.Detail.CsDetailVNP;
                    vm.TotalOk = model.Detail.CsDetailOk;
                    vm.TotalDiff = model.Detail.CsDetailDif;
                    vm.TotalChecked = model.Detail.CsDetailRev;
                    vm.GetawayNoSite = model.Detail.CsDetailCsNoSite;
                    vm.SiteNoGetaway = model.Detail.CsDetailSiteNoCs;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Cybersource;
                    return PartialView("_Getaway", vm);
                case ConciliationAppDto.Sucive:
                    vm.TotalVnp = model.Detail.SuDetailVNP;
                    vm.TotalGetaway = model.Detail.SuDetailVNP;
                    vm.TotalOk = model.Detail.SuDetailOk;
                    vm.TotalDiff = model.Detail.SuDetailDif;
                    vm.TotalChecked = model.Detail.SuDetailRev;
                    vm.GetawayNoSite = model.Detail.SuDetailSuNoSite;
                    vm.SiteNoGetaway = model.Detail.SuDetailSiteNoSu;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Sucive;
                    return PartialView("_Getaway", vm);
                case ConciliationAppDto.Tc33:
                    vm.TotalVnp = model.Detail.TcDetailVNP;
                    vm.TotalGetaway = model.Detail.TcDetailVNP;
                    vm.TotalOk = model.Detail.TcDetailOk;
                    vm.TotalDiff = model.Detail.TcDetailDif;
                    vm.TotalChecked = model.Detail.TcDetailRev;
                    vm.GetawayNoSite = model.Detail.TcDetailTcNoSite;
                    vm.SiteNoGetaway = model.Detail.TcDetailSiteNoTc;
                    vm.GetawayName = PresentationAdminStrings.Reports_Conciliation_Tc33;
                    return PartialView("_Getaway", vm);
                default:
                    return PartialView("_Getaway", vm);
            }
        }

        [HttpGet]
        [CustomAuthentication(Actions.ConciliationVNP)]
        public ActionResult VNP()
        {
            var model = new RunConciliationModel
            {
                Date = DateTime.Today.AddDays(-1),
                DateTo = DateTime.Today.AddDays(-1)
            };
            return View(model);
        }

        [HttpPost]
        [CustomAuthentication(Actions.ConciliationVNP)]
        public async Task<ActionResult> RunConciliation(RunConciliationModel model)
        {
            try
            {
                //Validations
                if (model.App == null)
                {
                    ShowNotification("Debe seleccionar una aplicación.", NotificationType.Alert, "¡Atención!");
                    return RedirectToAction("VNP");
                }
                var appsWithFileNeeded = new List<ConciliationAppModel> { ConciliationAppModel.Batch, ConciliationAppModel.Banred, ConciliationAppModel.Sistarbanc };
                if (appsWithFileNeeded.Contains(model.App.Value) && string.IsNullOrEmpty(model.FileName))
                {
                    ShowNotification("La aplicación seleccionada require que indique un archivo.", NotificationType.Alert, "¡Atención!");
                    return RedirectToAction("VNP");
                }

                if (appsWithFileNeeded.Contains(model.App.Value))
                {
                    UploadFileToBlob(model.App.Value, model.FileName);
                }

                //Execute Conciliation async
                var dto = new RunConciliationDto
                {
                    App = (ConciliationAppDto)model.App,
                    Date = model.Date,
                    DateTo = model.DateTo,
                    FileName = model.FileName
                };
                _conciliationVnpService.RunConciliation(dto);

                NLogLogger.LogEvent(NLogType.Info, string.Format("ConciliationController - RunConciliation - " +
                    "Se ejecutó la conciliación manual para App: {0}, Fecha Desde: {1}, Fecha Hasta: {2}, Archivo: {3}",
                    model.App.ToString(), model.Date.ToString("dd/MM/yyyy"), model.DateTo.ToString("dd/MM/yyyy"), model.FileName));

                ShowNotification(PresentationAdminStrings.ProcessStarted, NotificationType.Success, "Éxito");
                return RedirectToAction("VNP");
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ConciliationController - RunConciliation - Error");
                NLogLogger.LogEvent(e);
                ShowNotification(e.Message, NotificationType.Error, "Error");
                return RedirectToAction("VNP");
            }
        }

        private void UploadFileToBlob(ConciliationAppModel app, string filename)
        {
            byte[] fileBuffer;
            var input = Request.Files[0];
            using (var reader = new BinaryReader(input.InputStream))
            {
                fileBuffer = reader.ReadBytes(input.ContentLength);
            }

            var folder = string.Empty;
            var contentType = string.Empty;
            switch (app)
            {
                case ConciliationAppModel.Banred:
                    folder = _banredBlobFolder;
                    contentType = "text/plain";
                    break;
                case ConciliationAppModel.Batch:
                    folder = _visanetCallbackBlobFolder;
                    contentType = "text/plain";
                    break;
                case ConciliationAppModel.Sistarbanc:
                    folder = _sistarbancBlobFolder;
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
            }

            if (folder != string.Empty)
            {
                DeleteFileAzure(filename, folder);
                CreateFileAzure(folder, filename, fileBuffer, contentType);
            }
        }

    }
}