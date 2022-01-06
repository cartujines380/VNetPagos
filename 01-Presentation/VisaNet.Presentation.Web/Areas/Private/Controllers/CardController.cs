using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class CardController : BaseController
    {
        private readonly IWebCardClientService _webCardClientService;
        private readonly IWebBinClientService _webBinClientService;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebLogClientService _logClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;
        private readonly IWebParameterClientService _webParameterClientService;
        private readonly IWebPaymentClientService _webPaymentClientService;

        public CardController(IWebCardClientService webCardClientService, IWebBinClientService webBinClientService, IWebApplicationUserClientService webApplicationUserClientService,
            IWebLogClientService logClientService, IWebServiceClientService webServiceClientService, IWebCyberSourceAccessClientService webCyberSourceAccessClientService,
            IWebParameterClientService webParameterClientService, IWebPaymentClientService webPaymentClientService)
        {
            _webCardClientService = webCardClientService;
            _webBinClientService = webBinClientService;
            _webApplicationUserClientService = webApplicationUserClientService;
            _logClientService = logClientService;
            _webCyberSourceAccessClientService = webCyberSourceAccessClientService;
            _webParameterClientService = webParameterClientService;
            _webPaymentClientService = webPaymentClientService;
        }

        public async Task<ActionResult> Index()
        {
            var models = await LoadCards(null, "", "");

            ViewBag.IsSearch = false;

            return View("Index", models);
        }

        public async Task<ActionResult> GetCards(string mask, string dueDateMonth, string dueDateYear)
        {
            try
            {
                var models = await LoadCards(mask, dueDateMonth, dueDateYear);

                ViewBag.IsSearch = !String.IsNullOrEmpty(mask) || !String.IsNullOrEmpty(dueDateMonth) ||
                                   !String.IsNullOrEmpty(dueDateYear);

                return PartialView("_CardList", models);
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

        private async Task<List<CardListModel>> LoadCards(string mask, string dueDateMonth, string dueDateYear)
        {
            var currentUser = await CurrentSelectedUser();

            var list = await _webCardClientService.FindAll(new CardFilterDto()
            {
                DueDateMonth = dueDateMonth,
                DueDateYear = dueDateYear,
                MaskedNumber = mask,
                UserId = currentUser.Id,
                DisplayLength = 10000
            });

            var listMask = list.Select(c => Int32.Parse(c.MaskedNumber.Substring(0, 6))).ToList();
            var bins = await _webBinClientService.GetBinsFromMask(listMask);

            return (from dto in list
                    let bin = bins.FirstOrDefault(b => b.Value == Int32.Parse(dto.MaskedNumber.Substring(0, 6)))
                    select new CardListModel()
                    {
                        Mask = dto.MaskedNumber + (new DateTime(dto.DueDate.Year, dto.DueDate.Month, 1).CompareTo(DateTime.Now) < 0 ? " (Vencida)" : ""),
                        DueDate = string.Format("{0}-{1}", dto.DueDate.Year, dto.DueDate.Month),
                        CardImage = bin == null || string.IsNullOrEmpty(bin.ImageUrl) ? null : bin.ImageUrl,
                        Active = dto.Active,
                        Id = dto.Id,
                        Expired = new DateTime(dto.DueDate.Year, dto.DueDate.Month, 1).CompareTo(DateTime.Now) < 0,
                        Description = dto.Description
                    }).ToList();
        }

        public async Task<ActionResult> DeactivateCard(Guid cardId, string mask, string dueDateMonth, string dueDateYear)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();
                await _webCardClientService.DesactivateCard(new CardOperationDto()
                {
                    CardId = cardId,
                    UserId = user.Id
                });
                message = PresentationWebStrings.Service_Card_Deactivate;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;

                var userUpdated = await _webApplicationUserClientService.GetUserWithCards(user.Id);
                SetCurrentSelectedUser(userUpdated);

            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadCards(mask, dueDateMonth, dueDateYear);

            ViewBag.IsSearch = !String.IsNullOrEmpty(mask) || !String.IsNullOrEmpty(dueDateMonth) ||
                               !String.IsNullOrEmpty(dueDateYear);

            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);


            //var list = await GetCardList(serviceId);
            //return PartialView("_CardList", list);
        }

        public async Task<ActionResult> ActivateCard(Guid cardId, string mask, string dueDateMonth, string dueDateYear)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();
                await _webCardClientService.ActivateCard(new CardOperationDto()
                {
                    CardId = cardId,
                    UserId = user.Id
                });
                message = PresentationWebStrings.Service_Card_Activate;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;

                var userUpdated = await _webApplicationUserClientService.GetUserWithCards(user.Id);
                SetCurrentSelectedUser(userUpdated);
            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadCards(mask, dueDateMonth, dueDateYear);

            ViewBag.IsSearch = !String.IsNullOrEmpty(mask) || !String.IsNullOrEmpty(dueDateMonth) ||
                               !String.IsNullOrEmpty(dueDateYear);

            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> EliminateCard(Guid cardId, string mask, string dueDateMonth, string dueDateYear)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();

                await _webCardClientService.EliminateCard(new CardOperationDto()
                {
                    CardId = cardId,
                    UserId = user.Id
                });


                message = PresentationWebStrings.Service_Card_Eliminate;
                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;

                var userUpdated = await _webApplicationUserClientService.GetUserWithCards(user.Id);
                SetCurrentSelectedUser(userUpdated);



            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadCards(mask, dueDateMonth, dueDateYear);

            ViewBag.IsSearch = !String.IsNullOrEmpty(mask) || !String.IsNullOrEmpty(dueDateMonth) ||
                               !String.IsNullOrEmpty(dueDateYear);

            var content = RenderPartialViewToString("_CardList", list);

            return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> MigrateServices(Guid oldCardId, Guid newCardId, string mask, string dueDateMonth, string dueDateYear)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                var user = await CurrentSelectedUser();
                user = await _webApplicationUserClientService.GetUserWithCards(user.Id);

                if (user.CardDtos.Count(x => x.Active) > 1)
                {
                    //verifico que los servicios de la tarjeta vieja acepten el tipo de la tarjeta nueva
                    var oldCard = await _webCardClientService.FindWithServices(oldCardId);
                    var oldCardServices = oldCard.ServicesAssociatedDto;

                    if (oldCardServices == null)
                    {
                        //si la tarjeta vieja no tiene servicios asociados
                        message = PresentationWebStrings.Service_Card_Migration_No_Services_To_Migrate;
                        title = PresentationWebStrings.Action_Warning;
                        response = AjaxResponse.Success;
                        notification = NotificationType.Alert;
                    }
                    else
                    {
                        var newCard = await _webCardClientService.Find(newCardId);
                        var newCardBin = await _webBinClientService.Find(Convert.ToInt32(newCard.MaskedNumber.Substring(0, 6)));

                        if (newCardBin == null)
                        {
                            message = PresentationWebStrings.Bin_Not_Defined;
                            title = PresentationWebStrings.Action_Error;
                        }
                        else
                        {
                            //var allAccept = true;
                            //foreach (var item in oldCardServices)
                            //{
                            //    var service = await _webServiceClientService.Find(item.ServiceId);
                            //    var isBinAssociatedToService = await _webServiceClientService.IsBinAssociatedToService(int.Parse(newCard.MaskedNumber.Substring(0, 6)), service.Id);
                            //    if (!isBinAssociatedToService)
                            //    {
                            //        //si alguno no acepta el tipo de tarjeta seteo la variable para mostrar mensaje diferente
                            //        allAccept = false;
                            //        break;
                            //    }
                            //}

                            //if (allAccept)
                            //{
                            //    await _webCardClientService.MigrateServices(user.Id, oldCardId, newCardId);

                            //    response = AjaxResponse.Success;
                            //    message = PresentationWebStrings.Service_Card_Migration;
                            //    title = PresentationWebStrings.Action_Succesfull;
                            //    notification = NotificationType.Success;

                            //    var userUpdated = await _webApplicationUserClientService.GetUserWithCards(user.Id);
                            //    SetCurrentSelectedUser(userUpdated);
                            //}
                            //else
                            //{
                            //    message = PresentationWebStrings.Service_Card_Migration_Error_Card_Not_Accepted;
                            //    title = PresentationWebStrings.Action_Warning;
                            //    notification = NotificationType.Error;
                            //}

                            await _webCardClientService.MigrateServices(user.Id, oldCardId, newCardId);

                            response = AjaxResponse.Success;
                            message = PresentationWebStrings.Service_Card_Migration;

                            title = PresentationWebStrings.Action_Succesfull;
                            notification = NotificationType.Success;

                            var userUpdated = await _webApplicationUserClientService.GetUserWithCards(user.Id);
                            SetCurrentSelectedUser(userUpdated);
                        }
                    }
                }
                else
                {
                    message = PresentationWebStrings.Service_Card_Migration_Error_No_Active_Cards;
                    title = PresentationWebStrings.Action_Error;
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (Exception ex)
            {
                message = "Ha ocurrido un error.";
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadCards(mask, dueDateMonth, dueDateYear);

            ViewBag.IsSearch = !String.IsNullOrEmpty(mask) || !String.IsNullOrEmpty(dueDateMonth) ||
                               !String.IsNullOrEmpty(dueDateYear);

            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);


            //var list = await GetCardList(serviceId);
            //return PartialView("_CardList", list);
        }

        public async Task<ViewResult> AddCard()
        {
            var objParams = await _webParameterClientService.Get();
            var merchantId = objParams.MerchantId;

            return View(new ServiceAssociateCardModel()
            {
                MerchantId = merchantId
            });
        }

        //CALLBACK DE CYBERSOURCE
        public async Task<ActionResult> AssociateCardToUser()
        {
            NLogLogger.LogEvent(NLogType.Info, "AssociateCardToUser - Llega comunicaccion de CS");
            NLogLogger.LogEvent(NLogType.Info, "AssociateCardToUser - RequestId: " + Request.Form["transaction_id"]);

            var dataDictionary = GenerateDictionary(Request.Form);

            var card_desc = Session["card_descrition"] != null ? Session["card_descrition"].ToString() : "";

            dataDictionary.Add("card_description", card_desc);

            var newCardResult = await _webApplicationUserClientService.AddCard(dataDictionary);

            if (newCardResult.TokenizationData.PaymentResponseCode != (int)ErrorCodeDto.CYBERSOURCE_OK)
            {
                return EvaluateErrors(newCardResult.TokenizationData);
            }

            ShowNotification(PresentationWebStrings.Card_Add_Success, NotificationType.Success);
            return RedirectToAction("Index");
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(RedirectEnums redirectTo, string nameTh, string cardBin, string fingerPrint)
        {
            var currentUser = await CurrentSelectedUser();
            var url = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            var keys = await _webCyberSourceAccessClientService.GenerateKeys(new KeysInfoForTokenRegisteredUser
            {
                UserId = currentUser.Id,
                TransactionReferenceNumber = Guid.NewGuid().ToString(),
                RedirectTo = redirectTo.ToString("D"),
                NameTh = nameTh,
                CardBin = cardBin,
                CallcenterUser = String.Empty,
                Platform = PaymentPlatformDto.VisaNet.ToString(),
                PaymentTypeDto = PaymentTypeDto.Manual,
                UrlReturn = url,
                FingerPrint = fingerPrint,
            });

            Session[SessionConstants.TEMPORARY_ID] = keys["merchant_defined_data29"];

            return keys;
        }

        private ActionResult EvaluateErrors(CsResponseData csResponseData)
        {
            var reasonCode = (CybersourceMsg)csResponseData.PaymentResponseCode;

            if (reasonCode != CybersourceMsg.Accepted)
            {
                #region ErrorCodes

                /* 100 : ok
                 * 102 : [msj datos inválidos] error de campos             
                 * 202 : [msj datos inválidos] tarjeta vencida
                 * 204 : [msj datos inválidos] fondos insuficientes 
                 * 205 : [msj datos inválidos] tarjeta declarada como robada o perdida             
                 * 208 : [msj datos inválidos] tarjeta inactiva / no autorizada
                 * 210 : [msj datos inválidos] limite de crédito excedido
                 * 211 : [msj datos inválidos] CVN inválido
                 * 222 : [msj datos inválidos] cuenta congelada
                 * 231 : [msj datos inválidos] número de cuenta inválido
                 * 232 : [msj datos inválidos] el tipo de tarjeta no es aceptada por el procesador             
                 * 240 : [msj datos inválidos] El tipo de tarjeta es inválido o no correpsonde con el nro de cuenta
                 * 230 : [msj datos inválidos] Autorización aceptada, declinada por cybersource (CVN Check)
                 * 104 : [página error] error de keys
                 * 200 : [página error] Autorización aceptada, rechazada por cybersource por no pasar el AVS
                 * 201 : [página error] Autorización automatica rechazada
                 * 207 : [página error] error de banco             
                 * 233 : [página error] Negación general
                 * 234 : [página error] Error en datos del usuario en cybersource
                 * 236 : [página error] Error general del procesador
                 * 475 : [página error] Payer authentication error
                 * 476 : [página error] Payer authentication error
                 * 520 : [página error] Autorización aceptada, declinada por cybersource
                 */

                #endregion

                switch (reasonCode)
                {
                    case CybersourceMsg.InvalidFields:
                    case CybersourceMsg.CardIssuer:
                    case CybersourceMsg.AuthorizationRejected:
                    case CybersourceMsg.ExpiredCard:
                    case CybersourceMsg.GeneralDecline:
                    case CybersourceMsg.InsufficientFunds:
                    case CybersourceMsg.StolenLostCard:
                    case CybersourceMsg.BankUnavailable:
                    case CybersourceMsg.InactiveUnAuthorizedCard:
                    case CybersourceMsg.CreditLimitReached:
                    case CybersourceMsg.InvalidCVN:
                    case CybersourceMsg.AccountFrozen:
                    case CybersourceMsg.CVNCheckInvalid:
                    case CybersourceMsg.InvalidAccountNumber:
                    case CybersourceMsg.CardTypeNotAccepted:
                    case CybersourceMsg.GeneralDeclineByProcessor:
                    case CybersourceMsg.ProcessorFailure:
                    case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    case CybersourceMsg.PayerAuthenticationError:
                        return InvalidCardData(csResponseData);

                    case CybersourceMsg.AVSCheckInvalid:
                    case CybersourceMsg.UserCybersourceError:
                    case CybersourceMsg.PayerAuthenticationNotAuthenticated:
                    case CybersourceMsg.AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings:
                    case CybersourceMsg.ConfigurationKeysInvalids:
                        return RedirectToAction("NotificationError");
                }
            }

            return null;
        }

        private ActionResult InvalidCardData(CsResponseData csResponseData)
        {
            ShowNotification(csResponseData.PaymentResponseMsg, NotificationType.Info);
            return RedirectToAction("AddCard");
        }

        public ActionResult NotificationError()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> NewCardValidation(string nameTh, string cardBin, string fpProfiler, string card_description = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(card_description) && card_description.Length > 50)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "El largo máximo de la descripción es de 50 caracteres", PresentationWebStrings.Action_Warning, NotificationType.Error));
                }

                var cybersourceData = await LoadKeysForCybersource(RedirectEnums.PrivateAddCardToUser, nameTh, cardBin, fpProfiler);
                var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData);

                Session["card_descrition"] = card_description;

                return Json(new JsonResponse(AjaxResponse.Success, new { keys = content }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationWebStrings.Action_Warning, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpGet]
        public async Task<ActionResult> LogCSInvocation()
        {
            try
            {
                //Creo nuevo log
                var user = await CurrentSelectedUser();
                await _logClientService.Put(Guid.Parse((string)Session[SessionConstants.TEMPORARY_ID]), new LogDto
                {
                    LogCommunicationType = LogCommunicationType.CyberSource,
                    LogType = LogType.Info,
                    LogOperationType = LogOperationType.NewCardAdded,
                    LogUserType = LogUserType.Registered,
                    CallCenterMessage = string.Format("Inicia comunicación a CS para asociar tarjeta"),
                    Message = string.Format("Inicia comunicación a CS para asociar tarjeta"),
                    ApplicationUserId = user.Id
                });
                return Json(new JsonResponse(AjaxResponse.Success, "", "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> TestMigration(Guid oldCardId, Guid newCardId)
        {
            var user = await CurrentSelectedUser();
            var testResult = await _webCardClientService.TestMigration(oldCardId, newCardId, user.Id);

            if (!testResult.FailedServices.Any())
            {
                return Json(new JsonResponse(AjaxResponse.Success, null, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JsonResponse(AjaxResponse.BusinessError, testResult, PresentationWebStrings.MigrationTestFail, PresentationWebStrings.Action_Warning, NotificationType.Info), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> AssociatedServices(Guid cardId)
        {
            var cards = await _webCardClientService.GetAssociatedServices(cardId);
            var debits = await _webCardClientService.GetAssociatedDebits(cardId);

            var model = new SevicesAssosiatedViewModel
            {
                DebitsAssociated = debits.ToList(),
                ServicesAssociated = cards.ToList()
            };

            return PartialView("_AssociatedServices", model);
        }

        public async Task<ActionResult> GetServicesToMigrate(Guid cardId)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";
            var model = new SevicesAssosiatedViewModel();

            try
            {
                var servicesAssociated = await _webCardClientService.GetAssociatedServices(cardId);
                var debitsAssociated = await _webCardClientService.GetAssociatedDebits(cardId);

                title = PresentationWebStrings.Action_Succesfull;
                response = AjaxResponse.Success;
                notification = NotificationType.Success;

                if (debitsAssociated.Count > 0)
                {
                    message = PresentationWebStrings.Debit_Card_Migration_No_Debits_To_Migrate;
                    title = PresentationWebStrings.Action_Warning;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Alert;
                }

                ViewBag.showCards = servicesAssociated.Count >= 0;

                model = new SevicesAssosiatedViewModel
                {
                    DebitsAssociated = debitsAssociated.ToList(),
                    ServicesAssociated = servicesAssociated.ToList()
                };

            }
            catch (Exception ex)
            {

                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            return Json(new JsonResponse(response, model, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditCardDescription(Guid cardId, string description)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "";
            var title = "";

            try
            {
                if (!string.IsNullOrEmpty(description) && description.Length > 50)
                {
                    message = "El largo máximo de la descripción es de 50 caracteres";
                    title = PresentationWebStrings.Action_Warning;
                }
                else
                {
                    await _webCardClientService.EditCardDescription(cardId, description);

                    title = PresentationWebStrings.Action_Succesfull;
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }
            catch (WebApiClientFatalException ex)
            {
                message = ex.Message;
                title = PresentationWebStrings.Action_Error;
            }

            var list = await LoadCards(null, "", "");

            var content = RenderPartialViewToString("_CardList", list);

            return
                Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditCardDescriptionView(Guid cardId, string description)
        {
            ViewBag.cardId = cardId;
            ViewBag.description = description;
            return PartialView("_EditCardDescription");
        }

    }
}