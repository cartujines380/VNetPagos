using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsDashboardController : BaseController
    {
        private readonly IReportsClientService _reportsClientService;

        public ReportsDashboardController(IReportsClientService reportsClientService)
        {
            _reportsClientService = reportsClientService;
        }

        //
        // GET: /ReportsDashboard/
        [CustomAuthentication(Actions.ReportsDashboardDetails)]
        public ActionResult Index()
        {
            return View(new ReportsDashboardFilterDto
            {
                From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                To = DateTime.Now,
                Currency = (int)CurrencyDto.UYU,
                
            });
        }

        [HttpPost]
        [CustomAuthentication(Actions.ReportsDashboardDetails)]
        public async Task<ActionResult> GetDashboard(ReportsDashboardFilterDto filtersDto)
        {
            try
            {
                var dashboard = await _reportsClientService.GetDashboardSP(filtersDto);

                //Application Users
                ViewBag.ApplicationUsers = dashboard.userQuantity;

                //Payments
                ViewBag.Payments = dashboard.totalQuantity;
                ViewBag.PaymentsAmount = dashboard.totalAmount;

                ViewBag.AnonymousPayments = dashboard.notRegisteredQuantity;
                ViewBag.AnonymousPaymentsAmount = dashboard.notRegisteredAmount;

                ViewBag.ManualPayments = dashboard.manualQuantity;
                ViewBag.ManualPaymentsAmount = dashboard.manualAmount;

                ViewBag.AutomaticPayments = dashboard.automaticQuantity;
                ViewBag.AutomaticPaymentsAmount = dashboard.automaticAmount;

                ViewBag.AppsPayments = dashboard.appsQuantity;
                ViewBag.AppsPaymentsAmount = dashboard.appsAmount;

                ViewBag.BankPayments = dashboard.bankQuantity;
                ViewBag.BankPaymentsAmount = dashboard.bankAmount;

                //Notifications
                ViewBag.Notifications = dashboard.notificationQuantity;

                //Contacts
                ViewBag.Contacts = dashboard.contactQuantity;
                ViewBag.ComplaintContacts = dashboard.complaintContactQuantity;
                ViewBag.QuestionContacts = dashboard.questionContactQuantity;
                ViewBag.SuggestionContacts = dashboard.suggestionContactQuantity;
                ViewBag.OtherContacts = dashboard.otherContactQuantity;

                //Subscribers
                ViewBag.Subscribers = dashboard.subscriberQuantity;


                ViewBag.Currency = filtersDto.Currency == (int)CurrencyDto.USD ? "U$D" : "$";

                var content = RenderPartialViewToString("_Dashboard");

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success, null, content));

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
    }
}