using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.VisaNetOn.Constants;
using VisaNet.Presentation.VisaNetOn.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebCyberSourceAccessClientService _cyberSourceAccessClientService;
        private readonly IWebVisaNetOnIntegrationClientService _visaNetOnIntegrationClientService;
        private readonly IWebAnonymousUserClientService _anonymousUserClientService;
        private readonly IWebBinClientService _binClientService;
        private readonly IWebDiscountClientService _discountClientService;
        private readonly IWebWebhookRegistrationClientService _webhookRegistrationClientService;
        private readonly IWebCardClientService _webCardClientService;

        public PaymentController(IWebServiceClientService serviceClientService, IWebCyberSourceAccessClientService cyberSourceAccessClientService,
            IWebVisaNetOnIntegrationClientService visaNetOnIntegrationClientService, IWebAnonymousUserClientService anonymousUserClientService,
            IWebBinClientService binClientService, IWebDiscountClientService discountClientService,
            IWebWebhookRegistrationClientService webhookRegistrationClientService, IWebCardClientService webCardClientService)
        {
            _serviceClientService = serviceClientService;
            _cyberSourceAccessClientService = cyberSourceAccessClientService;
            _visaNetOnIntegrationClientService = visaNetOnIntegrationClientService;
            _anonymousUserClientService = anonymousUserClientService;
            _binClientService = binClientService;
            _discountClientService = discountClientService;
            _webhookRegistrationClientService = webhookRegistrationClientService;
            _webCardClientService = webCardClientService;
        }

        public ActionResult Index()
        {
            var model = (PagePaymentModel)Session[SessionConstants.PAGE_PAYMENT_MODEL];
            Session[SessionConstants.PAGE_PAYMENT_MODEL] = null;

            if (model != null)
            {
                ViewBag.MerchantId = model.ServiceInfo.MerchantId.Trim();
                ViewBag.CsEnvironment = ConfigurationManager.AppSettings["CsEnvironment"];
                return View("Payment", model);
            }

            NLogLogger.LogEvent(NLogType.Error, "PaymentController - Index - PagePaymentModel null.");
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCardAndGetDiscount(PagePaymentModel model)
        {
            try
            {
                if (model.UserData.AnonymousUserId.HasValue)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - ValidateCardAndGetDiscount - IdApp: {0}, IdOperacion: {1}, NewCard:{2}, SelectedCardId:{3}, CardBin:{4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.NewCard, model.SelectedCardId, model.CardBin));
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - ValidateCardAndGetDiscount - IdApp: {0}, IdOperacion: {1}, NewCard:{2}, SelectedCardId:{3}, CardBin:{4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.NewCard, model.SelectedCardId, model.CardBin));
                }

                var bin = await _binClientService.Find(model.CardBin);
                if (bin != null && !bin.Active)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, null,
                        PresentationWebStrings.Bin_Not_Valid, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                //Si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = (bin == null) || (await _serviceClientService.IsBinAssociatedToService(bin.Value, model.ServiceInfo.ServiceId));
                if (!isBinAssociatedToService)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, null,
                        PresentationWebStrings.Bin_Not_Valid_For_Service, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                var bill = new BillDto
                {
                    Amount = model.BillData.Amount,
                    Currency = model.BillData.Currency,
                    TaxedAmount = model.BillData.TaxedAmount,
                    FinalConsumer = model.BillData.FinalConsumer,
                    Description = model.BillData.Description,
                };

                var discountQuery = new DiscountQueryDto
                {
                    Bills = new List<BillDto> { bill },
                    BinNumber = model.CardBin,
                    ServiceId = model.ServiceInfo.ServiceId
                };

                //Obtengo los valores con los descuentos correspondientes
                var discountList = await _discountClientService.GetDiscount(discountQuery);
                var discount = discountList.FirstOrDefault();

                return Json(new JsonResponse(AjaxResponse.Success,
                    new
                    {
                        TotalAmountStr = discount.BillDto.Amount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                        TotalAfterDiscountStr = (discount.BillDto.Amount - discount.BillDto.DiscountAmount).ToString("##,#0.00", CultureInfo.CurrentCulture),
                        DiscountTypeStr = discount.DiscountDto.DiscountLawDescription,
                        DiscountStr = discount.BillDto.DiscountAmount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                        DiscountApplied = discount.BillDto.DiscountAmount > 0,
                        Currency = discount.BillDto.Currency.Equals("UYU", StringComparison.InvariantCultureIgnoreCase) ? "$" : "US$",
                        CybersourceAmount = discount.CybersourceAmount.ToString("###0.00", CultureInfo.CurrentCulture),
                        DiscountObjId = discount.DiscountDto != null ? discount.DiscountDto.Id : Guid.Empty,
                        DiscountAmount = discount.BillDto.DiscountAmount.ToString("###0.00", CultureInfo.CurrentCulture),
                        BillDiscount = discount.BillDto.Discount,
                        Amount = discount.BillDto.Amount.ToString("###0.00", CultureInfo.CurrentCulture),
                        TaxedAmount = discount.BillDto.TaxedAmount.ToString("###0.00", CultureInfo.CurrentCulture),
                        DiscountType = discount.DiscountDto != null ? (int)discount.DiscountDto.DiscountLabel : 0
                    },
                    "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationWebStrings.InvalidData, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> LoadCardQuotas(int cardBin, Guid serviceId)
        {
            var quotas = "1";
            try
            {
                if (cardBin.ToString().Length == 6)
                {
                    quotas = await _webCardClientService.GetQuotasForBinAndService(cardBin, serviceId);
                }
                return Json(new JsonResponse(AjaxResponse.Success, new { Quotas = quotas, }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                quotas = "1";
                return Json(new JsonResponse(AjaxResponse.Success, new { Quotas = quotas, }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> LoadCyberSourceKeysForPayment(PagePaymentModel model, string nameTh, string fingerprint)
        {
            try
            {
                string name, surname;
                FillNameAndSurname(model.UserData.Name, model.UserData.Surname, nameTh, out name, out surname);
                model.UserData.Name = name;
                model.UserData.Surname = surname;

                var token = await InitializeToken(model);

                var service = await _serviceClientService.Find(model.ServiceInfo.ServiceId);
                var gateway = GetBestGateway(service.ServiceGatewaysDto);
                var billData = model.BillData;
                var discountData = model.DiscountData;

                token.ServiceId = model.ServiceInfo.ServiceId;
                token.FingerPrint = fingerprint;
                token.TransactionReferenceNumber = Guid.NewGuid().ToString();
                token.NameTh = nameTh;
                token.CardBin = model.CardBin.ToString();
                token.Platform = PaymentPlatformDto.VisaNetOn.ToString();
                token.PaymentTypeDto = PaymentTypeDto.App;
                token.GatewayId = gateway.GatewayId;
                token.UrlReturn = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
                token.OperationTypeDto = OperationTypeDto.UniquePayment;
                token.CybersourceAmount = discountData.CybersourceAmount;
                token.DiscountObjId = discountData.DiscountObjId;
                token.OperationId = model.IdOperation;
                token.Quotas = model.Quotas;
                token.ReferenceNumber1 = model.ServiceInfo.ReferenceNumber1;
                token.ReferenceNumber2 = model.ServiceInfo.ReferenceNumber2;
                token.ReferenceNumber3 = model.ServiceInfo.ReferenceNumber3;
                token.ReferenceNumber4 = model.ServiceInfo.ReferenceNumber4;
                token.ReferenceNumber5 = model.ServiceInfo.ReferenceNumber5;
                token.ReferenceNumber6 = model.ServiceInfo.ReferenceNumber6;
                token.Bill = new BillForToken
                {
                    Currency = billData.Currency,
                    DiscountAmount = discountData.DiscountAmount,
                    Amount = discountData.Amount,
                    TaxedAmount = discountData.TaxedAmount,
                    BillNumber = billData.ExternalId,
                    DiscountType = discountData.DiscountType,
                    BillExpirationDate = billData.GenerationDate.ToString("dd-MM-yyyy"), //Revisar
                    BillDescription = billData.Description,
                    BillGatewayTransactionId = string.Empty,
                    BillSucivePreBillNumber = string.Empty,
                    BillFinalConsumer = billData.FinalConsumer.ToString(),
                    BillDiscount = discountData.BillDiscount.ToString(),
                    BillDateInitTransaccion = string.Empty,
                    BillGatewayTransactionBrouId = string.Empty,
                    Quota = model.Quotas
                };

                var cybersourceData = await _cyberSourceAccessClientService.GenerateKeys(token);
                var cybersourceKeys = RenderPartialViewToString("_CybersourceKeys", cybersourceData);
                var data = new { Keys = cybersourceKeys };

                var isActive = await _webhookRegistrationClientService.IsTokenActive(new AccessTokenFilterDto()
                {
                    WebhookRegistrationId = model.WebhookRegistrationId
                });

                if (!isActive)
                {
                    var webhook = await _webhookRegistrationClientService.FindById(model.WebhookRegistrationId);
                    NLogLogger.LogEvent(NLogType.Error, string.Format("PaymentController - LoadCyberSourceKeysForPayment - AccessToken para idOperation {0} no activo.", webhook.IdOperation));
                    var modelEnd = new End
                    {
                        UrlCallback = webhook.UrlCallback,
                        OperationId = webhook.IdOperation,
                        ResultCode = "2",
                        ResultDescription = PresentationVisaNetOnStrings.AccessToken_Canceled,
                        AppId = model.ServiceInfo.IdApp,
                        WebhookRegistrationId = model.WebhookRegistrationId
                    };
                    return Json(new JsonResponse(AjaxResponse.Error, modelEnd, modelEnd.ResultDescription, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Success, data, "", ""), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    e.Message, PresentationCoreMessages.Notification_Title_Alert, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - LoadCyberSourceKeysForPayment - Ha ocurrido un error - IdApp: {0}, IdOperation: {1}",
                    model.ServiceInfo.IdApp, model.IdOperation));
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationWebStrings.Payment_General_Error, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        //Callback de Cybersource para pago
        public async Task<ActionResult> PaymentTokenGenerationCallback()
        {
            NLogLogger.LogEvent(NLogType.Info, "PaymentController - PaymentTokenGenerationCallback - Llega comunicacion de CS");
            NLogLogger.LogEvent(NLogType.Info, "PaymentController - PaymentTokenGenerationCallback - RequestId: " + Request.Form["transaction_id"]);

            var dictionaryData = GenerateDictionary(Request.Form);
            var actionRedirect = Enum.Parse(typeof(RedirectEnums), dictionaryData["ActionRedirect"]);
            dictionaryData.Remove("ActionRedirect");
            var processOperationDto = new ProcessOperationDto
            {
                FormData = dictionaryData,
                Action = (RedirectEnums)actionRedirect,
            };

            var operationId = dictionaryData["req_merchant_defined_data27"];
            var serviceId = new Guid(dictionaryData["req_merchant_defined_data30"]);
            var webHook = await _webhookRegistrationClientService.GetByIdOperation(operationId, serviceId);
            var trnsNumber = dictionaryData.ContainsKey("transaction_id") ? dictionaryData["transaction_id"] : string.Empty;

            try
            {
                var result = await _visaNetOnIntegrationClientService.ProcessOperation(processOperationDto);

                if (result.ResultCode.Equals("0"))
                {
                    var accessTokenFilter = new AccessTokenFilterDto { WebhookRegistrationId = webHook.Id };

                    //CHEQUEAR SI SIGUE ACTIVO EL TOKEN
                    var isActive = await _webhookRegistrationClientService.IsTokenActive(accessTokenFilter);
                    if (!isActive)
                    {
                        //CANCELAR 
                        var isCanceled = await _visaNetOnIntegrationClientService.CancelPayment(trnsNumber);
                        if (isCanceled)
                        {
                            NLogLogger.LogEvent(NLogType.Error, string.Format("PaymentController - PaymentTokenGenerationCallback - AccessToken de id operacion {0} no activo.", webHook.IdOperation));
                            return RedirectToAction("End", "Home", new End
                            {
                                UrlCallback = webHook.UrlCallback,
                                OperationId = webHook.IdOperation,
                                AppId = webHook.IdApp,
                                WebhookRegistrationId = webHook.Id,
                                TrnsNumber = trnsNumber,
                                ResultCode = "18",
                                ResultDescription = PresentationVisaNetOnStrings.AccessToken_Canceled
                            });
                        }
                    }

                    //ACTUALZIAR EL ESTADO DEL TOKEN A PAGADO
                    await _webhookRegistrationClientService.SetAccessTokenAsPaid(webHook.Id);

                    if (webHook.SendEmail.HasValue && webHook.SendEmail.Value)
                    {
                        _visaNetOnIntegrationClientService.SendPaymentTicketByEmail(trnsNumber, Guid.Parse(dictionaryData["req_merchant_defined_data31"]));
                    }
                }

                return RedirectToAction("End", "Home", new End
                {
                    UrlCallback = webHook.UrlCallback,
                    OperationId = operationId,
                    ResultCode = result.ResultCode,
                    ResultDescription = result.ResultDescription,
                    WebhookRegistrationId = webHook.Id,
                    AppId = webHook.IdApp,
                    TrnsNumber = trnsNumber,
                });
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - PaymentTokenGenerationCallback - Ha ocurrido un error - IdApp: {0}, IdOperation: {1}",
                    webHook.IdApp, operationId));
                NLogLogger.LogEvent(e);
                return RedirectToAction("End", "Home", new End
                {
                    UrlCallback = webHook.UrlCallback,
                    OperationId = operationId,
                    AppId = webHook.IdApp,
                    WebhookRegistrationId = webHook.Id,
                    ResultCode = "1",
                    ResultDescription = "Error general."
                });
            }
        }

        [HttpPost]
        public ActionResult CancelPayment(PagePaymentModel model)
        {
            return RedirectToAction("End", "Home", new End
            {
                UrlCallback = model.ServiceInfo.UrlCallback,
                OperationId = model.IdOperation,
                ResultCode = "16",
                ResultDescription = "Usuario canceló la operación.",
                AppId = model.ServiceInfo.IdApp,
                WebhookRegistrationId = model.WebhookRegistrationId
            });
        }

        private async Task<KeysInfoForPayment> InitializeToken(PagePaymentModel model)
        {
            KeysInfoForPayment token;
            var userId = Guid.Empty;
            var cardId = Guid.Empty;
            string redirectTo;

            if (model.UserData.ApplicationUserId.HasValue)
            {
                //Usuario registrado en VisaNetPagos
                token = new KeysInfoForPaymentRegisteredUser();
                userId = model.UserData.ApplicationUserId.Value;
                redirectTo = (RedirectEnums.VisaNetOnPaymentRegisteredNewToken).ToString("D");
                if (!model.NewCard)
                {
                    cardId = model.SelectedCardId.Value;
                    redirectTo = (RedirectEnums.VisaNetOnPaymentRegisteredWithToken).ToString("D");
                }
            }
            else if (model.UserData.AnonymousUserId.HasValue)
            {
                //Usuario recurrente de VisaNetOn
                token = new KeysInfoForPaymentRecurrentUser();
                userId = model.UserData.AnonymousUserId.Value;
                redirectTo = (RedirectEnums.VisaNetOnPaymentRecurrentNewToken).ToString("D");
                if (!model.NewCard)
                {
                    cardId = model.SelectedCardId.Value; //es el CardExternalId de la tabla VonData
                    redirectTo = (RedirectEnums.VisaNetOnPaymentRecurrentWithToken).ToString("D");
                }
            }
            else
            {
                //Se crea/obtiene usuario anonimo
                var anonymousUser = await LoadAnonymousUserData(model);
                userId = anonymousUser.Id;

                if (model.RememberUser)
                {
                    var isUserAssociatedToService = await IsRecurrentUserAssociatedToService(model.ServiceInfo.IdApp, userId);
                    if (isUserAssociatedToService)
                    {
                        //Usuario recurrente de VisaNetOn
                        token = new KeysInfoForPaymentRecurrentUser();
                        redirectTo = (RedirectEnums.VisaNetOnPaymentRecurrentNewToken).ToString("D");
                    }
                    else
                    {
                        //Se va a registrar al usuario recurrente al regreso de CS
                        token = new KeysInfoForPaymentNewUser
                        {
                            Email = model.UserData.Email,
                            Name = model.UserData.Name,
                            Surname = model.UserData.Surname,
                            Address = model.UserData.Address,
                        };
                        redirectTo = (RedirectEnums.VisaNetOnPaymentNewUser).ToString("D");
                    }
                }
                else
                {
                    //Pago anonimo
                    token = new KeysInfoForPaymentAnonymousUser();
                    redirectTo = (RedirectEnums.VisaNetOnPaymentAnonymous).ToString("D");
                }
            }

            token.UserId = userId;
            token.CardId = cardId;
            token.RedirectTo = redirectTo;

            return token;
        }

        private async Task<AnonymousUserDto> LoadAnonymousUserData(PagePaymentModel model)
        {
            var dto = new AnonymousUserDto
            {
                Email = model.UserData.Email,
                Address = model.UserData.Address,
                Name = model.UserData.Name,
                Surname = model.UserData.Surname,
                IsVonUser = true,
            };
            return await _anonymousUserClientService.CreateOrEditAnonymousUser(dto);
        }

        private async Task<bool> IsRecurrentUserAssociatedToService(string idApp, Guid anonymousUserId)
        {
            var vonData = await _visaNetOnIntegrationClientService.FindVonData(idApp, anonymousUserId);
            return vonData != null && vonData.Any();
        }

        #region Cybersource Fingerprint
        [HttpGet]
        public ActionResult FingerprintRedirectPng1(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/clear.png?org_id=" + orgId + "&session_id=" + sessionId + "&m=1";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectPng2(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/clear.png?org_id=" + orgId + "&session_id=" + sessionId + "&m=2";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectJs(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/check.js?org_id=" + orgId + "&session_id=" + sessionId;
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public ActionResult FingerprintRedirectSwf(string orgId, string sessionId)
        {
            var redirectUrl = "https://h.online-metrix.net/fp/fp.swf?org_id=" + orgId + "&session_id=" + sessionId;
            return Redirect(redirectUrl);
        }
        #endregion

        #region PAGOLINK
        [HttpPost]
        public async Task<ViewResult> Confirmation(CallbackModel model)
        {
            if (!model.CodResultado.Equals("0"))
            {
                return await LinkError(model);
            }
            var dto = await _visaNetOnIntegrationClientService.GetPaymentDto(model.TrnsNumber, model.AppId);
            var webhook = await _webhookRegistrationClientService.GetByIdOperation(model.IdOperacion, model.AppId);
            var discountMessage = "Descuento: ";
            if (dto.DiscountObj != null)
            {
                discountMessage = string.Concat(EnumHelpers.GetName(typeof(DiscountLabelTypeDto), (int)dto.DiscountObj.DiscountLabel, EnumsStrings.ResourceManager), ": ");
            }

            var confModel = new ConfirmationModel()
            {
                ServiceName = (dto.ServiceDto.ServiceContainerDto != null ? dto.ServiceDto.ServiceContainerDto.Name + " - " : string.Empty) + dto.ServiceDto.Name,
                UserId = dto.AnonymousUserId ?? (dto.RegisteredUserId ?? Guid.Empty),
                Email = dto.AnonymousUser != null ? dto.AnonymousUser.Email : (dto.RegisteredUser != null ? dto.RegisteredUser.Email : string.Empty),
                Transaction = dto.TransactionNumber,
                Date = dto.Date.ToString("dd/MM/yyyy"),
                Hrs = dto.Date.ToString("t"),
                Amount = dto.Bills != null ? dto.Bills.Any() ? dto.Bills.Sum(b => b.Amount).ToString("##,#0.00", CultureInfo.CurrentCulture) : "" : "",
                Currency = dto.Currency,
                Discount = dto.Discount,
                DiscountApplyed = dto.DiscountApplyed,
                TotalAmount = dto.TotalAmount,
                TotalTaxedAmount = dto.TotalTaxedAmount,
                Mask = dto.Card != null ? dto.Card.MaskedNumber : "",
                References = new Dictionary<string, string>(),
                AllowsAutomaticPayment = false,
                Quotas = dto.Quotas,
                SendEmail = webhook.SendEmail.Value,
            };

            confModel.IsAutogeneratedEmail = IsMailHarcoded(confModel.Email);

            confModel.DiscountTypeText = dto.DiscountApplyed ? discountMessage : "No aplica descuento Ley de Inclusión Financiera (19.210).";

            if (confModel.Currency != null && confModel.Currency.Equals(Currency.PESO_URUGUAYO))
                confModel.Currency = "$";
            if (confModel.Currency != null && confModel.Currency.Equals(Currency.DOLAR_AMERICANO))
                confModel.Currency = "U$S";

            if (dto.ServiceDto != null)
            {
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName, dto.ReferenceNumber);
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName2))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName2, dto.ReferenceNumber2);
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName3))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName3, dto.ReferenceNumber3);
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName4))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName4, dto.ReferenceNumber4);
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName5))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName5, dto.ReferenceNumber5);
                if (!string.IsNullOrEmpty(dto.ServiceDto.ReferenceParamName6))
                    confModel.References.Add(dto.ServiceDto.ReferenceParamName6, dto.ReferenceNumber6);
            }

            NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - Confirmation - IdApp: {0}, IdOperacion: {1}, CodResultado:{2}, DescResultado:{3}",
                webhook.IdApp, webhook.IdOperation, model.CodResultado, model.DescResultado));

            return View(confModel);
        }

        public async Task<ActionResult> DownloadTicket(Guid id, string transactionNumber)
        {
            var arrbytes = await _visaNetOnIntegrationClientService.DownloadTicket(transactionNumber, id);
            return File(arrbytes, "application/PDF", string.Format("Ticket_{0}.pdf", transactionNumber));
        }

        public async Task<ActionResult> SendEmail(Guid id, string transactionNumber)
        {
            try
            {
                await _visaNetOnIntegrationClientService.SendPaymentTicketByEmail(transactionNumber, id);
                return Json(new JsonResponse(AjaxResponse.Success,
                    "", "Se envió el email con el comprobante.", PresentationCoreMessages.NotificationSuccess, NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, null, "No se pudo enviar el email con el comprobante.",
                    PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ViewResult> LinkError(CallbackModel model)
        {
            var webhook = await _webhookRegistrationClientService.GetByIdOperation(model.IdOperacion, model.AppId);
            var services = await _serviceClientService.GetServicesFromMerchand(model.AppId, webhook.MerchantId, GatewayEnumDto.Apps);
            var service = services.FirstOrDefault();

            var errorTitle = "No se pudo procesar el pago";

            if (model.CodResultado == "18" || model.CodResultado == "19" || model.CodResultado == "20" || model.CodResultado == "8")
            {
                //Codigos de BusinessExceptions al obtener la factura
                errorTitle = "No se pudo obtener tu factura";
            }

            var confModel = new ConfirmationErrorModel
            {
                ServiceName = (service.ServiceContainerDto != null ? service.ServiceContainerDto.Name + " - " : string.Empty) + service.Name,
                ErrorDesc = model.DescResultado,
                ErrorTitle = errorTitle
            };

            NLogLogger.LogEvent(NLogType.Info, string.Format("PaymentController - LinkError - IdApp: {0}, IdOperacion: {1}, CodResultado:{2}, DescResultado:{3}",
                webhook.IdApp, webhook.IdOperation, model.CodResultado, model.DescResultado));

            return View("LinkError", confModel);
        }

        private bool IsMailHarcoded(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            var domain = ConfigurationManager.AppSettings["CustomerSiteEmailDomainForFinalCustomers"];
            return email.Contains(domain);
        }

        #endregion

    }
}