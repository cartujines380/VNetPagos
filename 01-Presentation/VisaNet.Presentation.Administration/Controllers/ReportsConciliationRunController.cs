using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
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
    public class ReportsConciliationRunController : BaseController
    {
        private readonly IConciliationVNPClientService _conciliationVNPClientService;

        public ReportsConciliationRunController(IConciliationVNPClientService conciliationVNPClientService)
        {
            _conciliationVNPClientService = conciliationVNPClientService;
        }

        [CustomAuthentication(Actions.ReportsConciliationRun)]
        public ActionResult Index()
        {
            var filter = new ReportsConciliationRunFilterDto()
            {
                CreationDateFrom = DateTime.Today.AddMonths(-1),
                CreationDateTo = DateTime.Today,
                SortDirection = SortDirection.Desc,
            };
            LoadViewBags();
            return View(filter);
        }

        [CustomAuthentication(Actions.ReportsConciliationRun)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsConciliationRun(Request, param);

            var dataModel = await _conciliationVNPClientService.GetConciliationRunReport(filter);
            var totalRecords = await _conciliationVNPClientService.GetConciliationRunReportCount(filter);

            var data = dataModel.Select(s => new ReportsConciliationRunViewModel
            {
                Id = s.Id,
                CreationDate = s.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                LastModificationDate = s.LastModificationDate != s.CreationDate ? s.LastModificationDate.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                App = EnumHelpers.GetName(typeof(ConciliationAppDto), (int)s.App, EnumsStrings.ResourceManager),
                RunType = s.IsManualRun ? "Manual" : "Automática",
                State = EnumHelpers.GetName(typeof(ConciliationRunStateDto), (int)s.State, EnumsStrings.ResourceManager),
                InputFileName = s.InputFileName,
                ConciliationDateFrom = s.ConciliationDateFrom.HasValue ? s.ConciliationDateFrom.Value.ToString("dd/MM/yyyy") : string.Empty,
                ConciliationDateTo = s.ConciliationDateTo.HasValue ? s.ConciliationDateTo.Value.ToString("dd/MM/yyyy") : string.Empty,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsConciliationRun)]
        public async Task<ActionResult> DetailsModal(Guid id)
        {
            var dto = await _conciliationVNPClientService.GetConciliationRun(id);

            var model = new ReportsConciliationRunViewModel
            {
                Id = dto.Id,
                CreationDate = dto.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                LastModificationDate = dto.LastModificationDate != dto.CreationDate ? dto.LastModificationDate.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                App = EnumHelpers.GetName(typeof(ConciliationAppDto), (int)dto.App, EnumsStrings.ResourceManager),
                RunType = dto.IsManualRun ? "Manual" : "Automática",
                State = EnumHelpers.GetName(typeof(ConciliationRunStateDto), (int)dto.State, EnumsStrings.ResourceManager),
                InputFileName = dto.InputFileName,
                ConciliationDateFrom = dto.ConciliationDateFrom.HasValue ? dto.ConciliationDateFrom.Value.ToString("dd/MM/yyyy") : string.Empty,
                ConciliationDateTo = dto.ConciliationDateTo.HasValue ? dto.ConciliationDateTo.Value.ToString("dd/MM/yyyy") : string.Empty,
                ResultDescription = dto.ResultDescription,
                ExceptionMessage = dto.ExceptionMessage
            };

            return PartialView("_LbConciliationRunDetails", model);
        }

        private void LoadViewBags()
        {
            var apps = Enum.GetValues(typeof(ConciliationAppDto)).Cast<ConciliationAppDto>();
            ViewBag.Apps = apps.Select(s => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(ConciliationAppDto), (int)s, EnumsStrings.ResourceManager),
                Value = (int)s + ""
            }).ToList();

            ViewBag.IsManualRun = new SelectList(
                new List<SelectListItem>
                {
                    //new SelectListItem { Selected = true, Text = "Todas", Value = null},
                    new SelectListItem { Selected = false, Text = "Manual", Value = true.ToString()},
                    new SelectListItem { Selected = false, Text = "Automática", Value = false.ToString()},
                }, "Value", "Text"/*, 1*/);

            var states = Enum.GetValues(typeof(ConciliationRunStateDto)).Cast<ConciliationRunStateDto>();
            ViewBag.States = states.Select(s => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(ConciliationRunStateDto), (int)s, EnumsStrings.ResourceManager),
                Value = (int)s + ""
            }).ToList();

        }

    }
}