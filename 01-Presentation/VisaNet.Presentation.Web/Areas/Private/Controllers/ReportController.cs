using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Utilities.Exportation.ExtensionMethods;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class ReportController : BaseController
    {
        private readonly IWebReportClientService _webReportClientService;


        public ReportController(IWebReportClientService webReportClientService)
        {
            _webReportClientService = webReportClientService;
        }

        public async Task<ActionResult> Index()
        {
            var id = (await CurrentSelectedUser()).Id;
            var categories = await _webReportClientService.ServicesCategories(id);
            var services = await _webReportClientService.ServicesWithPayments(id);

            var filter = new Dictionary<ServiceCategoryDto, List<ServiceDto>>();
            services.ForEach(x => filter.Add(categories.First(c => c.Id == x.Key), services[x.Key]));
            ViewBag.ServicesCategories = filter;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PieChart(ReportFilterDto model)
        {
            model.UserId = (await CurrentSelectedUser()).Id;
            return Json(await _webReportClientService.PieChart(model), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> LineChart(ReportFilterDto model)
        {
            model.UserId = (await CurrentSelectedUser()).Id;
            return Json(await _webReportClientService.LineChart(model), JsonRequestBehavior.AllowGet);

        }
    }
}