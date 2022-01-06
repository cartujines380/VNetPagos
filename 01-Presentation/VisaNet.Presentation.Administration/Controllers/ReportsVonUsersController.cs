using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsVonUsersController : BaseController
    {
        private readonly IServiceClientService _serviceClientService;

        public ReportsVonUsersController(IServiceClientService serviceClientService)
        {
            _serviceClientService = serviceClientService;
        }

        [CustomAuthentication(Actions.ReportsUsersVonList)]
        public async Task<ActionResult> Index()
        {
            var services = await _serviceClientService.FindAll();
            ViewBag.Services = new SelectList(services.Where(w => w.Active), "Id", "Name");
            var model = new ReportsUserVonFilterDto
            {
                DateFrom = DateTime.Today.AddYears(-1),
                DateTo = DateTime.Today
            };
            return View(model);
        }

        [CustomAuthentication(Actions.ReportsUsersVonList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsUserVON(Request, param);

            var dataModel = await _serviceClientService.GetDataForReportsUsersVon(filter);
            var totalRecords = await _serviceClientService.GetDataForReportsUsersVonCount(filter);

            var data = dataModel.Select(s => new ReportsUsersVonViewModel
            {
                AnonymousUserId = s.AnonymousUserId,
                AppId = s.AppId,
                CardsCount = s.CardsCount,
                CreationDate = s.CreationDate.ToString("dd/MM/yyyy 00:00:00"),
                Email = s.Email,
                Name = s.Name,
                PaymentsCount = s.PaymentsCount,
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Surname = s.Surname,
                UserExternalId = s.UserExternalId,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsUsersVonList)]
        public async Task<ActionResult> ViewUsersCards(Guid userId, Guid serviceId)
        {
            var cards = await _serviceClientService.GetVonUsersCards(userId, serviceId);
            cards = cards.OrderByDescending(x => x.CreationDate).ToList();

            return PartialView("_VonUsersCards", cards.Select(s => new CardUserVonViewModel
            {
                CardName = s.CardName,
                CardMaskedNumber = s.CardMaskedNumber,
                CardDueDate = s.CardDueDate.ToString("MM/yy"),
                CreationDate = s.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                Id = s.Id
            }));
        }

    }
}