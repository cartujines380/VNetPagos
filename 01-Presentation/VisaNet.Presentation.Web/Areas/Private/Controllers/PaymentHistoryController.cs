using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class PaymentHistoryController : BaseController
    {
        private readonly IWebPaymentClientService _paymentClientService;

        public PaymentHistoryController(IWebPaymentClientService paymentClientService)
        {
            _paymentClientService = paymentClientService;
        }

        public ActionResult Index()
        {
            return View(new PaymentFilterDto());
        }

        public async Task<ActionResult> GetPayments(PaymentFilterDto filter)
        {
            try
            {
                filter.UserId = (await CurrentSelectedUser()).Id;
                ViewBag.DisplayStart = filter.DisplayStart;
                ViewBag.DisplayLength = filter.DisplayLength;

                var payments = await _paymentClientService.GetDataForFromList(filter);

                ViewBag.IsSearch = filter.From != null || filter.To != null || !String.IsNullOrEmpty(filter.ServiceAssociatedDto) ||
                    filter.PaymentTypeFilterDto != null;

                return PartialView("_PaymentList", payments.Select(n => n.ToModel()).ToList());
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return null;
        }

        public async Task<ActionResult> SendCopyPaymentByEmail(Guid id, string transactionNumber)
        {
            try
            {
                var user = await CurrentSelectedUser();
                await _paymentClientService.SendTicketByEmail(id, transactionNumber, user.Id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationWebStrings.Payment_Email_Send_Success, PresentationCoreMessages.NotificationSuccess, NotificationType.Info), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Info), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

    }
}