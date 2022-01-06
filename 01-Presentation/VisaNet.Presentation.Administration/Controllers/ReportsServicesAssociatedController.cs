using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsServicesAssociatedController : Controller
    {
        private readonly IServiceAssociatedClientService _serviceAssociatedClientService;
        private readonly IServiceCategoryClientService _serviceCategoryClientService;

        public ReportsServicesAssociatedController(IServiceAssociatedClientService serviceAssociatedClientService, IServiceCategoryClientService serviceCategoryClientService)
        {
            _serviceAssociatedClientService = serviceAssociatedClientService;
            _serviceCategoryClientService = serviceCategoryClientService;
        }

        [CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        public async Task<ActionResult> Index()
        {
            var filters = new ReportsServicesAssociatedFilterDto();
            var applyFilters = false;

            var serviceCategories = await _serviceCategoryClientService.FindAll();
            ViewBag.ServiceCategories = new SelectList(serviceCategories, "Id", "Name").OrderBy(item => item.Text);

            ViewBag.ServiceStatus = GenerateServiceStatusList();
            //ViewBag.Deleted = GenerateDeletedList();
            ViewBag.HasAutomaticPayment = GenerateHasAutomaticPaymentList();

            var email = Request["Email"];

            if (!String.IsNullOrEmpty(email))
            {
                applyFilters = true;
                filters.ClientEmail = email;
            }

            if (applyFilters)
            {
                return View(filters);
            }
            return View();
        }

        [CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerServiceAssociated(Request, param);

            var data = await _serviceAssociatedClientService.ReportsServicesAssociatedDataFromDbView(filter);

            var totalRecords = await _serviceAssociatedClientService.ReportsServicesAssociatedDataCount(filter);

            var dataModel = data.Select(d => new
            {
                d.ServiceAssociatedId,
                d.ServiceId,
                d.ClientEmail,
                d.ClientName,
                d.ClientSurname,
                d.ServiceNameAndDesc,
                d.ServiceCategory,
                d.ReferenceNumber,
                Enabled = d.Enabled ? "Activo" : "Inactivo",
                Active = d.Active ? "No" : "Sí", //es el eliminado
                AutomaticPayment = d.AutomaticPaymentId != null ? "Sí" : "No",
                d.DefaultCardMask,
                d.PaymentsCount,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm"),
                LastModificationDate = d.LastModificationDate.ToString("dd/MM/yyyy HH:mm")
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        [CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        public async Task<ActionResult> ExcelExport(FormCollection collection)
        {
            var dateFrom = !String.IsNullOrEmpty(collection["CreationDateFrom"])
                ? Convert.ToDateTime(collection["CreationDateFrom"])
                : DateTime.MinValue;
            var dateTo = !String.IsNullOrEmpty(collection["CreationDateTo"])
                ? Convert.ToDateTime(collection["CreationDateTo"])
                : DateTime.MinValue;

            var filter = new ReportsServicesAssociatedFilterDto
            {
                CreationDateFrom = dateFrom,
                CreationDateFromString = dateFrom != DateTime.MinValue ? dateFrom.ToString("dd/MM/yyyy") : "",
                CreationDateTo = dateTo,
                CreationDateToString = dateTo != DateTime.MinValue ? dateTo.ToString("dd/MM/yyyy") : "",
                ClientEmail = collection["ClientEmail"],
                ClientName = collection["ClientName"],
                ClientSurname = collection["ClientSurname"],
                ServiceNameAndDesc = collection["ServiceNameAndDesc"],
                ServiceCategoryId = !String.IsNullOrEmpty(collection["ServiceCategoryId"]) ? new Guid(collection["ServiceCategoryId"]) : default(Guid),
                Enabled = Convert.ToInt32(collection["Enabled"]),
                //Deleted = Convert.ToInt32(collection["Deleted"]),
                HasAutomaticPayment = Convert.ToInt32(collection["HasAutomaticPayment"]),
                DisplayLength = null
            };

            var items = await _serviceAssociatedClientService.ReportsServicesAssociatedDataFromDbView(filter);

            var data = from p in items
                       orderby p.ClientEmail
                       select new
                       {
                           p.ClientEmail,
                           p.ClientName,
                           p.ClientSurname,
                           p.ServiceNameAndDesc,
                           p.ServiceCategory,
                           p.ReferenceNumber,
                           Enabled = p.Enabled ? "Activo" : "Inactivo",
                           Active = p.Active ? "No" : "Sí",
                           AutomaticPayment = p.AutomaticPaymentId != null ? "Sí" : "No",
                           p.DefaultCardMask,
                           p.PaymentsCount,
                           p.CreationDate,
                           p.LastModificationDate,
                       };

            var headers = new[]
                {
                    EntitiesDtosStrings.PaymentDto_ClientEmail,
                    EntitiesDtosStrings.PaymentDto_ClientName,
                    EntitiesDtosStrings.PaymentDto_ClientSurname,
                    "Servicio asociado (Nombre - Descripcion)",
                    EntitiesDtosStrings.PaymentDto_ServiceCategoryName,
                    EntitiesDtosStrings.PaymentDto_ReferenceNumber,
                    "Estado",
                    "Eliminado",
                    "Pago automático",
                    EntitiesDtosStrings.PaymentDto_CardMaskedNumber,
                    "Cant. pagos realizados",
                    "Fecha de creación",
                    "Fecha de modificación",
                };

            var memoryStream = ExcelExporter.ExcelExport("Servicios Asociados", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"],
                        string.Format("{0}.{1}", "Reporte_ServiciosAsociados", "xlsx"));
        }

        private List<SelectListItem> GenerateServiceStatusList()
        {
            var list = Enum.GetValues(typeof(StatusEnumDto)).Cast<StatusEnumDto>();
            return list.Select(serviceStatus => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(StatusEnumDto), (int)serviceStatus, EnumsStrings.ResourceManager),
                Value = (int)serviceStatus + ""
            }).ToList();
        }

        //private List<SelectListItem> GenerateDeletedList()
        //{
        //    var list = Enum.GetValues(typeof(YesNoEnumDto)).Cast<YesNoEnumDto>();
        //    return list.Select(serviceDeleted => new SelectListItem()
        //    {
        //        Text = EnumHelpers.GetName(typeof(YesNoEnumDto), (int)serviceDeleted, EnumsStrings.ResourceManager),
        //        Value = (int)serviceDeleted + "",
        //    }).ToList();
        //}

        private List<SelectListItem> GenerateHasAutomaticPaymentList()
        {
            var list = Enum.GetValues(typeof(YesNoEnumDto)).Cast<YesNoEnumDto>();
            return list.Select(automaticPayment => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(YesNoEnumDto), (int)automaticPayment, EnumsStrings.ResourceManager),
                Value = (int)automaticPayment + "",
            }).ToList();
        }

    }
}