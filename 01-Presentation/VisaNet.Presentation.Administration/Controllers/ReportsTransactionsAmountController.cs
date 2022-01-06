using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Models.Chart;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsTransactionsAmountController : BaseController
    {
        private readonly IReportsClientService _reportsClientService;

        public ReportsTransactionsAmountController(IReportsClientService reportsClientService)
        {
            _reportsClientService = reportsClientService;
        }

        //
        // GET: /ReportsTransactionsAmount/
        [CustomAuthentication(Actions.ReportsTransactionsAmountDetails)]
        public ActionResult Index()
        {
            return View(new ReportsTransactionsAmountFilterDto()
                        {
                            From = DateTime.Now.AddDays(-7),
                            To = DateTime.Now,
                            DateParameter = (int)DateParameterDto.Day,
                            SortDirection = SortDirection.Desc,
                        });
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsTransactionsAmountDetails)]
        public async Task<ActionResult> GetTable(ReportsTransactionsAmountFilterDto filtersDto)
        {
            try
            {
                var model = await _reportsClientService.GetTransactionsAmountData(filtersDto);

                ViewBag.Parameter = filtersDto.Parameter == (int)ParameterDto.Date ?
                        EnumHelpers.GetName(typeof(DateParameterDto), filtersDto.DateParameter, EnumsStrings.ResourceManager) :
                        EnumHelpers.GetName(typeof(ParameterDto), filtersDto.Parameter, EnumsStrings.ResourceManager);

                ViewBag.Dimension = EnumHelpers.GetName(typeof(DimensionDto), filtersDto.Dimension, EnumsStrings.ResourceManager);

                ViewBag.Currency = filtersDto.Dimension == (int)DimensionDto.Count
                    ? ""
                    : filtersDto.Currency == (int)CurrencyDto.UYU ? "$ " : "U$S ";

                var content = RenderPartialViewToString("_TransactionsAmountList", model);

                #region Chart
                var columns = new List<Column>
                {
                    new Column
                    {
                        label = "X",
                        type = "string"
                    },
                    new Column
                    {
                        label = EnumHelpers.GetName(typeof (DimensionDto), filtersDto.Dimension, EnumsStrings.ResourceManager),
                        type = "number"
                    }

                };

                var rows = new List<Row>();
                foreach (var group in model)
                {
                    var cells = new List<AbstractCell>
                    {
                        new TextCell
                        {
                            v = group.Name
                        },
                        new NumericCell
                        {
                            v = group.ValueTotal
                        }
                    };

                    rows.Add(new Row
                    {
                        c = cells
                    });
                }

                var table = new Chart
                {
                    cols = columns,
                    rows = rows
                };
                #endregion

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success, Json(table)));
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

        [CustomAuthentication(Actions.ReportsTransactionsAmountDetails)]
        public async Task<ActionResult> ExcelExport(ReportsTransactionsAmountFilterDto filtersDto)
        {
            var model = await _reportsClientService.GetTransactionsAmountData(filtersDto);

            var currency = "";
            var currencyString = "";
            var isCount = true;

            if (filtersDto.Dimension != (int)DimensionDto.Count)
            {
                isCount = false;
                currency = filtersDto.Currency == (int)CurrencyDto.UYU ? "$ " : "U$S ";
                currencyString = filtersDto.Currency == (int)CurrencyDto.UYU ? "Pesos" : "Dolares";
            }

            var data = from p in model
                       select new
                       {
                           p.Name,
                           Value = (!isCount ? "$ " : "") + (p.ValuePesos != 0 ? p.ValuePesos.ToString("#,#", CultureInfo.CurrentCulture) : "0"),
                           Value2 = (!isCount ? "U$S " : "") + (p.ValueDollars != 0 ? p.ValueDollars.ToString("#,#", CultureInfo.CurrentCulture) : "0"),
                           Value3 = currency + (p.ValueTotal != 0 ? p.ValueTotal.ToString("#,#", CultureInfo.CurrentCulture) : "0")
                       };

            var parameter = filtersDto.Parameter == (int)ParameterDto.Date
                ? EnumHelpers.GetName(typeof(DateParameterDto), filtersDto.DateParameter, EnumsStrings.ResourceManager)
                : EnumHelpers.GetName(typeof(ParameterDto), filtersDto.Parameter, EnumsStrings.ResourceManager);

            var dimension = EnumHelpers.GetName(typeof(DimensionDto), filtersDto.Dimension,
                EnumsStrings.ResourceManager);

            var headers = new[]
                {
                    parameter,
                    dimension + " en Pesos",
                    dimension + " en Dolares",
                    dimension + " Total" + (isCount ? "" : " en " + currencyString),
                };

            var memoryStream = ExcelExporter.ExcelExport("Cantidad de Transacciones", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"],
                        string.Format("{0}.{1}", "Reporte_Cantidad_De_Transacciones", "xlsx"));
        }
    }
}