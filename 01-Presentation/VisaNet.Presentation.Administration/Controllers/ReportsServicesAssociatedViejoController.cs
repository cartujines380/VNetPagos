using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsServicesAssociatedViejoController : BaseController
    {
        private readonly IServiceAssociatedClientService _serviceAssociatedClientService;
        private readonly IServiceClientService _serviceClientService;
        private readonly IServiceCategoryClientService _serviceCategoryClientService;
        private readonly IBinsClientService _binsClientService;

        public ReportsServicesAssociatedViejoController(IServiceAssociatedClientService serviceAssociatedClientService,
            IServiceClientService serviceClientService,
            IServiceCategoryClientService serviceCategoryClientService,
            IBinsClientService binsClientService)
        {
            _serviceAssociatedClientService = serviceAssociatedClientService;
            _serviceClientService = serviceClientService;
            _serviceCategoryClientService = serviceCategoryClientService;
            _binsClientService = binsClientService;
        }

        //
        // GET: /ReportsServicesAssociated/
        //[CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        public async Task<ActionResult> Index()
        {
            var services = await _serviceClientService.FindAll();
            ViewBag.Services = new SelectList(services, "Id", "Name");

            var serviceCategories = await _serviceCategoryClientService.FindAll();
            ViewBag.ServiceCategories = new SelectList(serviceCategories, "Id", "Name");
            

            return View();
        }

        //[CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        //public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        //{
        //    var filter = AjaxHandlers.AjaxHandlerServiceAssociated(Request, param);
            
        //    var data = await _serviceAssociatedClientService.Get(filter);

        //    var bins = await _binsClientService.FindAll();

        //    var dataModel = data.Select(d => d.ToModel(bins));

        //    if (filter.SortDirection == SortDirection.Asc)
        //        dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? p.ClientEmail :
        //                                filter.OrderBy == "1" ? p.ClientName :
        //                                filter.OrderBy == "2" ? p.ClientSurname :
        //                                filter.OrderBy == "3" ? p.ServiceName :
        //                                filter.OrderBy == "4" ? p.ServiceCategoryName :
        //                                filter.OrderBy == "5" ? p.CardMaskedNumber :
        //                                filter.OrderBy == "6" ? p.CardBin :
        //                                filter.OrderBy == "7" ? p.CardType :
        //                                filter.OrderBy == "8" ? p.AutomaticPayment :
        //                                filter.OrderBy == "9" ? p.Enabled : "");
        //    else
        //        dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? p.ClientEmail :
        //                                filter.OrderBy == "1" ? p.ClientName :
        //                                filter.OrderBy == "2" ? p.ClientSurname :
        //                                filter.OrderBy == "3" ? p.ServiceName :
        //                                filter.OrderBy == "4" ? p.ServiceCategoryName :
        //                                filter.OrderBy == "5" ? p.CardMaskedNumber :
        //                                filter.OrderBy == "6" ? p.CardBin :
        //                                filter.OrderBy == "7" ? p.CardType :
        //                                filter.OrderBy == "8" ? p.AutomaticPayment :
        //                                filter.OrderBy == "9" ? p.Enabled : "");


        //    var dataToShow = dataModel.Skip(filter.DisplayStart);

        //    if (filter.DisplayLength.HasValue)
        //        dataToShow = dataToShow.Take(filter.DisplayLength.Value);

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = data.Count(),
        //        iTotalDisplayRecords = data.Count(),
        //        aaData = dataToShow.ToList()
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[CustomAuthentication(Actions.ReportsServicesAssociatedDetails)]
        //public async Task<ActionResult> ExcelExport()
        //{
        //    var servicesAssociated = await _serviceAssociatedClientService.Get(new ReportsServicesAssociatedFilterDto());

        //    var bins = await _binsClientService.FindAll();

        //    var items = servicesAssociated.Select(d => d.ToModel(bins));

        //    var data = from s in items
        //               select new
        //               {
        //                   s.ClientEmail,
        //                   s.ClientName,
        //                   s.ClientSurname,
        //                   s.ClientIdentityNumber,
        //                   s.ClientMobileNumber,
        //                   s.ClientPhoneNumber,
        //                   s.ClientAddress,
        //                   s.ServiceName,
        //                   s.ServiceCategoryName,
        //                   s.ReferenceNumber,
        //                   s.Description,
        //                   s.CardMaskedNumber,
        //                   s.CardDueDate,
        //                   s.CardBin,
        //                   s.CardType,
        //                   s.AutomaticPayment,
        //                   s.Enabled,
        //               };

        //    var headers = new[]
        //        {
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientEmail,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientName,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientSurname,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientIdentityNumber,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientMobileNumber,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientPhoneNumber,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ClientAddress,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ServiceName,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ServiceCategoryName,
        //            EntitiesDtosStrings.ServiceAssociatedDto_ReferenceNumber,
        //            EntitiesDtosStrings.ServiceAssociatedDto_Description,
        //            EntitiesDtosStrings.ServiceAssociatedDto_CardMaskedNumber,
        //            EntitiesDtosStrings.ServiceAssociatedDto_CardDueDate,
        //            EntitiesDtosStrings.ServiceAssociatedDto_CardBin,
        //            EntitiesDtosStrings.ServiceAssociatedDto_CardType,
        //            EntitiesDtosStrings.ServiceAssociatedDto_AutomaticPayment,
        //            EntitiesDtosStrings.ServiceAssociatedDto_Enabled,
        //        };

        //    var memoryStream = ExcelExporter.ExcelExport("Servicios Asociados", data, headers);
        //    return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"],
        //                string.Format("{0}.{1}", "Reporte_ServiciosAsociados", "xlsx"));
        //}

	}
}