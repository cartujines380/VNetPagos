using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class DebitManagmentController : BaseController
    {
        private readonly IWebDebitClientService _webDebitClientService;
        private readonly IWebCardClientService _webCardClientService;

        public DebitManagmentController(IWebDebitClientService webDebitClientService, IWebCardClientService webCardClientService)
        {
            _webDebitClientService = webDebitClientService;
            _webCardClientService = webCardClientService;
        }

        #region Public methods

        //
        // GET: /Private/Bill/
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Requests()
        {
            var userId = (await CurrentSelectedUser()).Id;

            var vm = new DebitRequestsViewModel();
            vm.Filters.UserId = userId;
            vm.Cards = await GetCardSelectListItemByUserId(userId);

            return View(vm);
        }

        public async Task<ActionResult> GetRequests(DebitRequestsViewModel viewModel)
        {
            try
            {
                return PartialView("_RequestList", await LoadRequestTable(viewModel));
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

        [HttpPost]
        public async Task<ActionResult> CancelDebitRequest(DebitRequestsViewModel viewModel)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            try
            {
                var result = await _webDebitClientService.CancelDebitRequest(viewModel.DebitRequestIdToCancel);

                if (result)
                {
                    //message = PresentationWebStrings.DebitRequest_Cancellation;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = "No pudimos cancelar su solicitud. Intentá nuevamente o comunicate con el CallCenter";
                    title = "";
                }

                viewModel.Filters.DisplayStart = 0;
                var list = await LoadRequestTable(viewModel);
                var content = RenderPartialViewToString("_RequestList", list);

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public async Task<ActionResult> DeleteDebitRequest(DebitRequestsViewModel viewModel)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            try
            {
                var result = await _webDebitClientService.CancelDebitRequest(viewModel.DebitRequestIdToCancel);

                if (result)
                {
                    //message = PresentationWebStrings.DebitRequest_Removed;
                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
                else
                {
                    message = "No pudimos eliminar el débito automático. Intentá nuevamente o comunicate con el CallCenter";
                    title = "";
                }

                viewModel.Filters.DisplayStart = 0;
                var list = await LoadRequestTable(viewModel);
                var content = RenderPartialViewToString("_RequestList", list);

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
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

        #endregion Public methods

        #region Private methods

        private async Task<List<SelectListItem>> GetCardSelectListItemByUserId(Guid userId)
        {
            ICollection<CardDto> cards = new List<CardDto>();
            cards = await _webCardClientService.FindAll(new CardFilterDto
            {
                DueDateMonth = string.Empty,
                DueDateYear = string.Empty,
                MaskedNumber = null,
                UserId = userId,
                Active = true
            });

            return cards.Select(card => new SelectListItem { Text = card.MaskedNumber, Value = card.Id.ToString() }).ToList();
        }

        private async Task<IList<DebitRequestTableModel>> LoadRequestTable(DebitRequestsViewModel viewModel)
        {
            var filters = viewModel.Filters;
            ViewBag.DisplayStart = filters.DisplayStart;
            ViewBag.DisplayLength = filters.DisplayLength;

            var requests = await _webDebitClientService.GetDataForFromList(filters);

            ViewBag.IsSearch = filters.DateFrom != null || filters.DateTo != null;

            var result = requests.Select(n => n.ToModel()).ToList();

            return result;
        }

        #endregion Private methods
    }
}