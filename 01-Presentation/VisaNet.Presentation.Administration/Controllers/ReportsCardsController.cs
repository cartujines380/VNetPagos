using System;
using System.Collections.Generic;
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
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsCardsController : BaseController
    {
        private readonly IApplicationUserClientService _applicationUserClientService;        
        private readonly IWebCardClientService _webCardClientService;

        public ReportsCardsController(IApplicationUserClientService applicationUserClientService, IWebCardClientService webCardClientService)
        {
            _applicationUserClientService = applicationUserClientService;            
            _webCardClientService = webCardClientService;
        }

        [CustomAuthentication(Actions.ReportsCardsDetails)]
        public ActionResult Index()
        {
            var filters = new ReportsCardsFilterDto();
            var applyFilters = false;

            var email = Request["Email"];
            var cardMask = Request["CardMask"];

            if (!string.IsNullOrEmpty(email))
            {
                applyFilters = true;
                filters.ClientEmail = email;
            }
            if (!string.IsNullOrEmpty(cardMask))
            {
                applyFilters = true;
                filters.CardMaskedNumber = cardMask;
            }

            ViewBag.CardStatus = GenerateCardStatusList();

            if (applyFilters)
            {
                return View(filters);
            }
            return View();
        }

        [CustomAuthentication(Actions.ReportsCardsDetails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCard(Request, param);

            var data = await _applicationUserClientService.ReportsCardsData(filter);
            var totalRecords = await _applicationUserClientService.ReportsCardsDataCount(filter);

            var dataModel = data.Select(d => new
            {
                d.ApplicationUserId,
                CardId = d.CardId.ToString(),
                ClientEmail = d.Email,
                ClientName = d.Name,
                ClientSurname = d.Surname,
                CardMaskedNumber = d.CardMaskedNumber,
                CardDueDate = d.CardDueDate.ToString("MM/yyyy"),
                CardBin = d.BinValue,
                CardType = d.CardType != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)d.CardType, EnumsStrings.ResourceManager) : "",
                CardActive = d.CardActive ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                CardDeleted = d.CardDeleted ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                d.DeletedFromCs,
                d.NumServicesAsociated,
                LastPaymentDate = d.LastPaymentDate != null && d.LastPaymentDate!=DateTime.MinValue ? d.LastPaymentDate.Value.ToString("dd/MM/yyyy") : ""
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsCardsDetails)]
        public async Task<ActionResult> ExcelExport(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCard(Request, param);

            filter.SortDirection = SortDirection.Asc;
            filter.DisplayStart = 0;
            filter.DisplayLength = null;

            var data = await _applicationUserClientService.ReportsCardsData(filter);

            var dataModel = from s in data
                            select new
                            {
                                ClientEmail = s.Email,
                                ClientName = s.Name,
                                ClientSurname = s.Surname,
                                ClientIdentityNumber = s.IdentityNumber,
                                ClientMobileNumber = s.MobileNumber,
                                ClientPhoneNumber = s.PhoneNumber,
                                ClientAddress = s.Address,
                                s.CardMaskedNumber,
                                s.CardDueDate,
                                CardBin = s.BinValue,
                                s.CardType,
                                s.CardActive,
                                s.CardDeleted,
                            };

            var headers = new[]
                {
                    EntitiesDtosStrings.CardDto_ClientEmail,
                    EntitiesDtosStrings.CardDto_ClientName,
                    EntitiesDtosStrings.CardDto_ClientSurname,
                    EntitiesDtosStrings.CardDto_ClientIdentityNumber,
                    EntitiesDtosStrings.CardDto_ClientMobileNumber,
                    EntitiesDtosStrings.CardDto_ClientPhoneNumber,
                    EntitiesDtosStrings.CardDto_ClientAddress,
                    EntitiesDtosStrings.CardDto_MaskedNumber,
                    EntitiesDtosStrings.CardDto_DueDate,
                    EntitiesDtosStrings.CardDto_Bin,
                    EntitiesDtosStrings.CardDto_Type,
                    EntitiesDtosStrings.CardDto_Active,
                    EntitiesDtosStrings.CardDto_Deleted
                };

            var memoryStream = ExcelExporter.ExcelExport("Tarjetas", dataModel, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"], string.Format("{0}.{1}", "Reporte_Tarjetas", "xlsx"));
        }

        public async Task<ActionResult> DeleteCard(Guid cardId, Guid userId)
        {
            if (Session[SessionConstants.CURRENT_USER] != null)
            {                
                try
                {
                    await _webCardClientService.EliminateCard(new CardOperationDto()
                    {
                        CardId = cardId,
                        UserId = userId
                    });


                    ShowNotification(PresentationAdminStrings.Reports_Cards_DeleteCs_Succes, VisaNet.Utilities.Notifications.NotificationType.Success);
                }
                catch (Exception)
                {
                    ShowNotification(PresentationAdminStrings.Reports_Cards_DeleteCs_Fail, VisaNet.Utilities.Notifications.NotificationType.Error);
                }                
            }

            return RedirectToAction("Index");
        }

        private List<SelectListItem> GenerateCardStatusList()
        {
            var list = Enum.GetValues(typeof(StatusEnumDto)).Cast<StatusEnumDto>();
            return list.Select(cardStatus => new SelectListItem()
            {
                Text = EnumHelpers.GetName(typeof(StatusEnumDto), (int)cardStatus, EnumsStrings.ResourceManager),
                Value = (int)cardStatus + "",
                Selected = (int)cardStatus == 1
            }).ToList();
        }



    }
}