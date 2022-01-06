using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsAutomaticPaymentsController : Controller
    {
        private readonly IServiceAssociatedClientService _serviceAssociatedClientService;

        public ReportsAutomaticPaymentsController(IServiceAssociatedClientService serviceAssociatedClientService)
        {
            _serviceAssociatedClientService = serviceAssociatedClientService;
        }

        [CustomAuthentication(Actions.ReportsAutomaticPaymentsDetails)]
        public async Task<ActionResult> Index()
        {
            var filters = new ReportsAutomaticPaymentsFilterDto();
            var applyFilters = false;

            var serviceAssociatedId = Request["ServiceAssociatedId"];

            if (!String.IsNullOrEmpty(serviceAssociatedId))
            {
                applyFilters = true;
                var serviceAssociated = await _serviceAssociatedClientService.Find(new Guid(serviceAssociatedId));

                var email = serviceAssociated.RegisteredUserDto.Email;
                var serviceNameAndDesc = !String.IsNullOrEmpty(serviceAssociated.Description) ? serviceAssociated.ServiceDto.Name + " - " + serviceAssociated.Description : serviceAssociated.ServiceDto.Name;

                filters.ServiceAssociatedId = new Guid(serviceAssociatedId);
                filters.ClientEmail = email;
                filters.ServiceNameAndDesc = serviceNameAndDesc;
                filters.CreationDateFrom = new DateTime(2014, 01, 01);
                filters.CreationDateTo = DateTime.Today;
            }

            if (applyFilters)
            {
                return View(filters);
            }
            return View();
        }

        [CustomAuthentication(Actions.ReportsAutomaticPaymentsDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerAutomaticPayments(Request, param);

            var data = await _serviceAssociatedClientService.ReportsAutomaticPaymentsDataFromDbView(filter);
            var totalRecords = await _serviceAssociatedClientService.ReportsAutomaticPaymentsDataCount(filter);

            var dataModel = data.Select(d => new
            {
                d.AutomaticPaymentId,
                d.ServiceAssociatedId,
                d.ServiceId,
                d.RegisteredUserId,
                d.ClientEmail,
                d.ServiceNameAndDesc,
                Maximum = d.UnlimitedAmount ? "Ilimitado" : d.Maximum.ToString(),
                d.DaysBeforeDueDate,
                Quotas = d.UnlimitedQuotas ? "Ilimitado" : d.Quotas.ToString(),
                SuciveAnnualPatent = d.SuciveAnnualPatent ? "Sí" : "No",
                d.PaymentsCount,
                d.PaymentsAmountPesos,
                d.PaymentsAmountDollars,
                CreationDate = d.CreationDate != DateTime.MinValue ? d.CreationDate.ToString("dd/MM/yyyy HH:mm") : "-",
                //LastModificationDate = d.LastModificationDate.ToString("dd/MM/yyyy HH:mm")
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsAutomaticPaymentsDetails)]
        public async Task<ActionResult> ExcelExport(FormCollection collection)
        {
            var dateFrom = !String.IsNullOrEmpty(collection["CreationDateFrom"])
                ? Convert.ToDateTime(collection["CreationDateFrom"])
                : DateTime.MinValue;
            var dateTo = !String.IsNullOrEmpty(collection["CreationDateTo"])
                ? Convert.ToDateTime(collection["CreationDateTo"])
                : DateTime.MinValue;

            var filter = new ReportsAutomaticPaymentsFilterDto
            {
                CreationDateFrom = dateFrom,
                CreationDateFromString = dateFrom != DateTime.MinValue ? dateFrom.ToString("dd/MM/yyyy") : "",
                CreationDateTo = dateTo,
                CreationDateToString = dateTo != DateTime.MinValue ? dateTo.ToString("dd/MM/yyyy") : "",
                ClientEmail = collection["ClientEmail"],
                ServiceNameAndDesc = collection["ServiceNameAndDesc"],
                ServiceAssociatedId = !String.IsNullOrEmpty(collection["ServiceAssociatedId"]) ? Guid.Parse(collection["ServiceAssociatedId"]) : Guid.Empty,
                DisplayLength = null,
            };

            var items = await _serviceAssociatedClientService.ReportsAutomaticPaymentsDataFromDbView(filter);

            var data = from d in items
                       orderby d.ClientEmail
                       select new
                       {
                           d.ClientEmail,
                           d.ServiceNameAndDesc,
                           Maximum = d.UnlimitedAmount ? "Ilimitado" : d.Maximum.ToString(),
                           d.DaysBeforeDueDate,
                           Quotas = d.UnlimitedQuotas ? "Ilimitado" : d.Quotas.ToString(),
                           SuciveAnnualPatent = d.SuciveAnnualPatent ? "Sí" : "No",
                           d.PaymentsCount,
                           d.PaymentsAmountPesos,
                           d.PaymentsAmountDollars,
                           CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm"),
                       };

            var headers = new[]
                {
                    EntitiesDtosStrings.PaymentDto_ClientEmail,
                    "Servicio asociado (Nombre - Descripcion)",
                    "Monto maximo",
                    "Dias previo venc.",
                    "Cuotas",
                    "Sucive anual",
                    "Cant. pagos realizados",
                    "Total pagos en pesos",
                    "Total pagos en dolares",
                    "Fecha de creación",
                };

            var memoryStream = ExcelExporter.ExcelExport("Pagos Programados", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"], string.Format("{0}.{1}", "Reporte_PagosProgramados", "xlsx"));
        }

    }
}