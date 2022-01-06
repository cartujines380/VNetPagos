using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;
using System.Globalization;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Enums;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsIntegrationController : BaseController
    {
        private readonly IWebhookLogClientService _webhookLogService;
        private readonly IIntegrationClientService _integrationClientService;

        public ReportsIntegrationController(IWebhookLogClientService webhookLogService, IIntegrationClientService integrationClientService)
        {
            _webhookLogService = webhookLogService;
            _integrationClientService = integrationClientService;
        }

        [CustomAuthentication(Actions.ReportsIntegration)]
        public ActionResult Index()
        {
            var idOperation = Request["IdOperation"];
            var type = Request["Type"];
            var model = new ReportsIntegrationFilterDto()
            {
                SortDirection = SortDirection.Desc,
                IdOperation = idOperation,
                ExternalRequestType = string.IsNullOrEmpty(type) ? 0 : int.Parse(type)
            };
            return View(model);
        }

        [CustomAuthentication(Actions.ReportsIntegration)]
        public ActionResult AjaxHandlerRenderTable(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsIntegration(Request, param);
            var content = string.Empty;

            switch (filter.ExternalRequestType)
            {
                case 0:
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Debe seleccionar el tipo a consultar.", "Atención!", NotificationType.Error), JsonRequestBehavior.AllowGet);

                case (int)ExternalRequestTypeDto.WebhookDown:
                    content = RenderPartialViewToString("_WebhookDown", filter);
                    break;

                case (int)ExternalRequestTypeDto.WebhookNewAssociation:
                    content = RenderPartialViewToString("_WebhookNewAssociation", filter);
                    break;

                case (int)ExternalRequestTypeDto.WebhookRegistration:
                    content = RenderPartialViewToString("_WebhookRegistration", filter);
                    break;

                case (int)ExternalRequestTypeDto.WsBillPaymentOnline:
                    content = RenderPartialViewToString("_WsBillPaymentOnline", filter);
                    break;

                case (int)ExternalRequestTypeDto.WsBillQuery:
                    content = RenderPartialViewToString("_WsBillQuery", filter);
                    break;

                case (int)ExternalRequestTypeDto.WsCommerceQuery:
                    content = RenderPartialViewToString("_WsCommerceQuery", filter);
                    break;

                case (int)ExternalRequestTypeDto.WsPaymentCancellation:
                    content = RenderPartialViewToString("_WsPaymentCancellation", filter);
                    break;

                case (int)ExternalRequestTypeDto.WsCardRemove:
                    content = RenderPartialViewToString("_WsCardRemove", filter);
                    break;
            }

            return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsIntegration)]
        public async Task<ActionResult> AjaxHandlerLoadTable(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerReportsIntegration(Request, param);
            var content = string.Empty;

            switch (filter.ExternalRequestType)
            {
                case 0:
                    ShowNotification("Debe seleccionar el tipo a consultar.", NotificationType.Info);
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Debe seleccionar el tipo a consultar.", "Atención!", NotificationType.Error), JsonRequestBehavior.AllowGet);

                case (int)ExternalRequestTypeDto.WebhookDown:
                    return await LoadWebhookDownTable(param, filter);

                case (int)ExternalRequestTypeDto.WebhookNewAssociation:
                    return await LoadWebhookNewAssociationTable(param, filter);

                case (int)ExternalRequestTypeDto.WebhookRegistration:
                    return await LoadWebhookRegistrationTable(param, filter);

                case (int)ExternalRequestTypeDto.WsBillPaymentOnline:
                    return await LoadWsBillPaymentOnlineTable(param, filter);

                case (int)ExternalRequestTypeDto.WsBillQuery:
                    return await LoadWsBillQueryTable(param, filter);

                case (int)ExternalRequestTypeDto.WsCommerceQuery:
                    return await LoadWsCommerceQueryTable(param, filter);

                case (int)ExternalRequestTypeDto.WsPaymentCancellation:
                    return await LoadWsPaymentCancellationTable(param, filter);

                case (int)ExternalRequestTypeDto.WsCardRemove:
                    return await LoadWsCardRemoveTable(param, filter);
            }

            ShowNotification("Error, intente nuevamente.", NotificationType.Error);
            return Json(new JsonResponse(AjaxResponse.Error, "", "Error, intente nuevamente.", "Error", NotificationType.Error), JsonRequestBehavior.AllowGet);
        }

        //Load tables
        private async Task<JsonResult> LoadWebhookDownTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data1 = await _webhookLogService.GetWebhookDownsForTable(filter);
            var totalRecords = await _webhookLogService.GetWebhookDownsForTableCount(filter);

            var dataModel1 = data1.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                d.HttpResponseCode,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.IdCard,
                d.IdUser
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel1.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWebhookNewAssociationTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data2 = await _webhookLogService.GetWebhookNewAssociationsForTable(filter);
            var totalRecords = await _webhookLogService.GetWebhookNewAssociationsForTableCount(filter);

            var dataModel2 = data2.Select(d => new
            {
                d.Id,
                d.IdApp,
                d.IdOperationApp,
                d.HttpResponseCode,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.CardAffiliation,
                d.CardAffiliationCode,
                d.CardBank,
                d.CardBankCode,
                d.CardDueDate,
                d.CardMask,
                d.CardType,
                d.DiscountAmount,
                d.IdCard,
                d.IdUser,
                d.IsAssociation,
                d.IsPayment,
                d.RefCliente1,
                d.RefCliente2,
                d.RefCliente3,
                d.RefCliente4,
                d.RefCliente5,
                d.RefCliente6,
                d.TransactionNumber,

                Email = d.UserData.Email,
                Type =
                    d.IsAssociation && d.IsPayment ? "Pago + Asociación" :
                    d.IsAssociation && !d.IsPayment ? "Asociación" :
                    !d.IsAssociation && d.IsPayment ? "Pago" : "-"
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel2.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWebhookRegistrationTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data3 = await _webhookLogService.GetWebhookRegistrationsForTable(filter);
            var totalRecords = await _webhookLogService.GetWebhookRegistrationsForTableCount(filter);

            var dataModel = data3.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                Action =
                    d.Action == WebhookRegistrationActionEnumDto.Association ? "Asociación" :
                    d.Action == WebhookRegistrationActionEnumDto.Payment ? "Pago" :
                    d.Action == WebhookRegistrationActionEnumDto.Tokenization ? "Tokenización" : "",
                d.EnableEmailChange,
                d.EnableRememberUser,
                d.IdUsuario,
                d.MerchantId,
                d.PaymentId,
                d.ReferenceNumber,
                d.ReferenceNumber2,
                d.ReferenceNumber3,
                d.ReferenceNumber4,
                d.ReferenceNumber5,
                d.ReferenceNumber6,
                d.SendEmail,

                Name = d.UserData.Name,
                Surname = d.UserData.Surname,
                Email = d.UserData.Email,
                IdentityNumber = d.UserData.IdentityNumber,

                BillCurrency = d.Bill.Currency,
                BillAmount = d.Bill.Amount,
                BillTaxedAmount = d.Bill.TaxedAmount,
                BillQuota = d.Bill.Quota,
                BillGenerationDate = d.Bill.GenerationDate,
                BillFinalConsumer = d.Bill.FinalConsumer,
                BillDescription = d.Bill.Description,
                BillExpirationDate = d.Bill.ExpirationDate,
                BillExternalId = d.Bill.ExternalId,

                TransactionNumber = d.PaymentDto != null ? d.PaymentDto.TransactionNumber : string.Empty,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWsBillPaymentOnlineTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data4 = await _integrationClientService.GetBillPaymentsOnlineForTable(filter);
            var totalRecords = await _integrationClientService.GetBillPaymentsOnlineForTableCount(filter);

            var dataModel4 = data4.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.AmountTaxed,
                d.AmountTotal,
                d.BillNumber,
                CodBranch = d.CodBranch != 0 ? d.CodBranch.ToString() : "",
                CodCommerce = d.CodCommerce != 0 ? d.CodCommerce.ToString() : "",
                d.Codresult,
                d.ConsFinal,
                d.Currency,
                d.CustomerIp,
                d.CustomerPhone,
                DateBill = d.DateBill.ToString("dd/MM/yyyy"),
                d.Description,
                d.IdCard,
                d.IdMerchant,
                d.IdUser,
                d.Indi,
                d.PaymentId,
                Quota = d.Quota > 1 ? d.Quota : 1,
                d.WcfVersion,

                TransactionNumber = d.PaymentDto != null ? d.PaymentDto.TransactionNumber : string.Empty,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel4.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWsBillQueryTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data5 = await _integrationClientService.GetBillQueriesForTable(filter);
            var totalRecords = await _integrationClientService.GetBillQueriesForTableCount(filter);

            var dataModel5 = data5.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.BillNumber,
                CodBranch = d.CodBranch != 0 ? d.CodBranch.ToString() : "",
                CodCommerce = d.CodCommerce != 0 ? d.CodCommerce.ToString() : "",
                d.Codresult,
                Date = d.Date.ToString("dd/MM/yyyy"),
                d.IdMerchant,
                d.RefClient,
                d.RefClient2,
                d.RefClient3,
                d.RefClient4,
                d.RefClient5,
                d.RefClient6,
                d.WcfVersion
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel5.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWsCommerceQueryTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data6 = await _integrationClientService.GetCommerceQueriesForTable(filter);
            var totalRecords = await _integrationClientService.GetCommerceQueriesForTableCount(filter);

            var dataModel6 = data6.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.Codresult,
                d.WcfVersion
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel6.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWsPaymentCancellationTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data7 = await _integrationClientService.GetPaymentCancellationsForTable(filter);
            var totalRecords = await _integrationClientService.GetPaymentCancellationsForTableCount(filter);

            var dataModel7 = data7.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.Codresult,
                d.IdOperacionCobro,
                d.WcfVersion
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel7.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<JsonResult> LoadWsCardRemoveTable(JQueryDataTableParamModel param, ReportsIntegrationFilterDto filter)
        {
            var data8 = await _integrationClientService.GetCardRemovesForTable(filter);
            var totalRecords = await _integrationClientService.GetCardRemovesForTableCount(filter);

            var dataModel8 = data8.Select(d => new
            {
                d.Id,
                d.IdApp,
                IdOperationApp = d.IdOperation,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),

                d.Codresult,
                d.IdCard,
                d.IdUser,
                d.WcfVersion,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel8.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        //Details modals
        [CustomAuthentication(Actions.ReportsIntegration)]
        public async Task<ActionResult> WebhookNewAssociationDetails(Guid id)
        {
            var dto = await _webhookLogService.GetWebhookNewAssociation(id);
            var model = new WebhookNewAssociationModel
            {
                Id = dto.Id,

                CreationDate = dto.CreationDate,
                IdApp = dto.IdApp,
                IdOperationApp = dto.IdOperationApp,
                IdOperation = dto.IdOperation,
                HttpResponseCode = dto.HttpResponseCode,

                IdUser = dto.IdUser,
                UserEmail = dto.UserData.Email,
                UserName = dto.UserData.Name,
                UserSurname = dto.UserData.Surname,
                UserIdentityNumber = dto.UserData.IdentityNumber,
                UserMobileNumber = dto.UserData.MobileNumber,
                UserPhoneNumber = dto.UserData.PhoneNumber,
                UserAddress = dto.UserData.Address,

                IdCard = dto.IdCard,
                CardDueDate = dto.CardDueDate,
                CardMask = dto.CardMask,
                CardType = string.Format("{0} ({1})", EnumHelpers.GetName(typeof(CardTypeDto), (int)dto.CardType, EnumsStrings.ResourceManager), ((int)dto.CardType).ToString()),
                CardBank = dto.CardBank,
                CardBankCode = dto.CardBankCode,
                CardAffiliation = dto.CardAffiliation,
                CardAffiliationCode = dto.CardAffiliationCode,

                RefCliente1 = dto.RefCliente1,
                RefCliente2 = dto.RefCliente2,
                RefCliente3 = dto.RefCliente3,
                RefCliente4 = dto.RefCliente4,
                RefCliente5 = dto.RefCliente5,
                RefCliente6 = dto.RefCliente6,

                IsAssociation = dto.IsAssociation ? "Sí" : "No",
                IsPayment = dto.IsPayment ? "Sí" : "No",

                TransactionNumber = dto.TransactionNumber,
                DiscountAmount = dto.DiscountAmount.ToString("###.##", new CultureInfo("es-UY")),
            };
            return PartialView("_LbWebhookNewAssociationDetails", model);
        }

        [CustomAuthentication(Actions.ReportsIntegration)]
        public async Task<ActionResult> WebhookRegistrationDetails(Guid id)
        {
            var dto = await _webhookLogService.GetWebhookRegistration(id);
            var model = new WebhookRegistrationModel
            {
                Id = dto.Id,

                CreationDate = dto.CreationDate,
                IdApp = dto.IdApp,
                IdOperation = dto.IdOperation,
                UrlCallback = dto.UrlCallback,
                MerchantId = dto.MerchantId,
                Action =
                    dto.Action == WebhookRegistrationActionEnumDto.Association ? "Asociación" :
                    dto.Action == WebhookRegistrationActionEnumDto.Payment ? "Pago" :
                    dto.Action == WebhookRegistrationActionEnumDto.Tokenization ? "Tokenización" : "",
                EnableEmailChange = dto.EnableEmailChange,
                EnableRememberUser = dto.EnableRememberUser,
                SendEmail = dto.SendEmail.HasValue && dto.SendEmail.Value ? "Sí" : "No",

                IdUsuario = dto.IdUsuario,
                UserEmail = dto.UserData.Email,
                UserName = dto.UserData.Name,
                UserSurname = dto.UserData.Surname,
                UserIdentityNumber = dto.UserData.IdentityNumber,
                UserMobileNumber = dto.UserData.MobileNumber,
                UserPhoneNumber = dto.UserData.PhoneNumber,
                UserAddress = dto.UserData.Address,

                BillExternalId = dto.Bill.ExternalId,
                BillAmount = dto.Bill.Amount,
                BillTaxedAmount = dto.Bill.TaxedAmount,
                BillCurrency = dto.Bill.Currency,
                BillFinalConsumer = dto.Bill.FinalConsumer,
                BillGenerationDate = dto.Bill.GenerationDate,
                BillQuota = dto.Bill.Quota,
                BillDescription = dto.Bill.Description,
                BillExpirationDate = dto.Bill.ExpirationDate,

                ReferenceNumber = dto.ReferenceNumber,
                ReferenceNumber2 = dto.ReferenceNumber2,
                ReferenceNumber3 = dto.ReferenceNumber3,
                ReferenceNumber4 = dto.ReferenceNumber4,
                ReferenceNumber5 = dto.ReferenceNumber5,
                ReferenceNumber6 = dto.ReferenceNumber6,

                PaymentId = dto.PaymentId,
                PaymentDate = dto.PaymentDto != null ? dto.PaymentDto.Date.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                PaymentTransactionNumber = dto.PaymentDto != null ? dto.PaymentDto.TransactionNumber : string.Empty,
            };
            return PartialView("_LbWebhookRegistrationDetails", model);
        }

        [CustomAuthentication(Actions.ReportsIntegration)]
        public async Task<ActionResult> WsBillPaymentOnlineDetails(Guid id)
        {
            var dto = await _integrationClientService.GetWsBillPaymentOnline(id);
            var model = new WsBillPaymentOnlineModel
            {
                Id = dto.Id,

                CreationDate = dto.CreationDate,
                IdApp = dto.IdApp,
                IdOperation = dto.IdOperation,
                CodCommerce = dto.CodCommerce > 0 ? dto.CodCommerce.ToString() : string.Empty,
                CodBranch = dto.CodBranch > 0 ? dto.CodBranch.ToString() : string.Empty,
                IdMerchant = dto.IdMerchant,

                IdUser = dto.IdUser,
                IdCard = dto.IdCard,

                BillNumber = dto.BillNumber,
                Description = dto.Description,
                DateBill = dto.DateBill,
                Currency = dto.Currency,
                AmountTotal = dto.AmountTotal.ToString("###.##"),
                AmountTaxed = dto.AmountTaxed.ToString("###.##"),
                Indi = dto.Indi.ToString(),
                ConsFinal = dto.ConsFinal ? "Sí" : "No",
                Quota = dto.Quota.ToString(),

                Codresult = dto.Codresult,

                PaymentId = dto.PaymentId,
                PaymentDate = dto.PaymentDto != null ? dto.PaymentDto.Date.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                PaymentTransactionNumber = dto.PaymentDto != null ? dto.PaymentDto.TransactionNumber : string.Empty,

                DeviceFingerprint = dto.DeviceFingerprint,
                CustomerIp = dto.CustomerIp,
                CustomerPhone = dto.CustomerPhone,
                WcfVersion = dto.WcfVersion
            };

            if (dto.CustomerShippingAddresDto != null)
            {
                model.CustomerShippingAddress = new CustomerShippingAddressModel
                {
                    City = dto.CustomerShippingAddresDto.City,
                    Complement = dto.CustomerShippingAddresDto.Complement,
                    Corner = dto.CustomerShippingAddresDto.Corner,
                    Country = dto.CustomerShippingAddresDto.Country,
                    DoorNumber = dto.CustomerShippingAddresDto.DoorNumber,
                    Latitude = dto.CustomerShippingAddresDto.Latitude,
                    Longitude = dto.CustomerShippingAddresDto.Longitude,
                    Neighborhood = dto.CustomerShippingAddresDto.Neighborhood,
                    Phone = dto.CustomerShippingAddresDto.Phone,
                    PostalCode = dto.CustomerShippingAddresDto.PostalCode,
                    Street = dto.CustomerShippingAddresDto.Street
                };
            }

            return PartialView("_LbWsBillPaymentOnlineDetails", model);
        }

    }
}