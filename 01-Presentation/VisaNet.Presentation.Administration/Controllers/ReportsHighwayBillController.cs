using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsHighwayBillController : BaseController
    {
        private readonly IReportsHighwayService _reportsHighwayService;

        public ReportsHighwayBillController(IReportsHighwayService reportsHighwayService)
        {
            _reportsHighwayService = reportsHighwayService;
        }

        [CustomAuthentication(Actions.ReportsHighwayBills)]
        public ActionResult Index()
        {
            return View(new ReportsHighwayBillFilterDto()
                        {
                            SortDirection = SortDirection.Desc,
                        });
        }

        [CustomAuthentication(Actions.ReportsHighwayBills)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerHighwayBill(Request, param);

            var data = await _reportsHighwayService.GetHighwayBillReports(filter);
            var count = await _reportsHighwayService.GetHighwayBillReportsCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy"),
                d.CodComercio,
                d.CodSucursal,
                d.RefCliente,
                ServiceName = d.ServiceDto != null ? d.ServiceDto.Name : "",
                d.NroFactura,
                FchFactura = d.FchFactura.ToString("dd/MM/yyyy"),
                FchVencimiento = d.FchVencimiento.ToString("dd/MM/yyyy"),
                d.DiasPagoVenc,
                d.Moneda,
                d.MontoTotal,
                d.MontoMinimo,
                d.MontoGravado,
                ConsFinal = d.ConsFinal ? "Sí" : "No",
                //d.Cuotas
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

    }
}