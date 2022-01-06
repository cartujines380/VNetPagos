using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsLifApiBillController : BaseController
    {
        private readonly ILifApiBillClientService _lifApiBillClientService;

        public ReportsLifApiBillController(ILifApiBillClientService lifApiBillClientService)
        {
            _lifApiBillClientService = lifApiBillClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsLifApiBillList)]
        public ActionResult Index()
        {

            return View(new LifApiBillFilterDto
            {
                DateFrom = DateTime.Now.AddHours(-1),
                DateTo = DateTime.Now
            });
        }

        [CustomAuthentication(Actions.ReportsLifApiBillList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerLifApiBill(Request, param);

            var data = await _lifApiBillClientService.FindAll(filter);
            var totalRecords = await _lifApiBillClientService.GetDataForLifApiBillCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Date = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                IdApp = d.AppId,
                IdOperation = d.OperationId,
                Amount = d.Amount,
                TaxedAmount = d.TaxedAmount,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsLifApiBillDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _lifApiBillClientService.Find(id);
                return View(dto.ToModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);

            }
            return RedirectToAction("Index");
        }

    }
}