using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Debit.Mappers;
using VisaNet.Presentation.Web.Areas.Debit.Models;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Helpers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;
using CardModel = VisaNet.Presentation.Web.Areas.Debit.Models.CardModel;

namespace VisaNet.Presentation.Web.Areas.Debit.Controllers
{
    public class DebitController : BaseController
    {
        private readonly IWebDebitClientService _webDebitClientService;
        private readonly DebitCommerceManagment _debitCommerceManagment;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebCardClientService _webCardClientService;
        private readonly IWebBinClientService _webBinClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;

        public DebitController(IWebDebitClientService webDebitClientService, IWebApplicationUserClientService webApplicationUserClientService, IWebCardClientService webCardClientService, IWebBinClientService webBinClientService, IWebCyberSourceAccessClientService webCyberSourceAccessClientService)
        {
            _webDebitClientService = webDebitClientService;
            _webApplicationUserClientService = webApplicationUserClientService;
            _webCardClientService = webCardClientService;
            _webBinClientService = webBinClientService;
            _webCyberSourceAccessClientService = webCyberSourceAccessClientService;
            _debitCommerceManagment = new DebitCommerceManagment(webDebitClientService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ViewResult> Service(string commerceId)
        {
            var commercesList = await _webDebitClientService.GetCommercesDebit();
            var commerces = commercesList != null ? commercesList.Select(x => x.ToModel()).OrderBy(x => x.Name) : null;

            var userLogead = await CurrentSelectedUser();

            var debit = new DebitModel()
            {
                CommerceModel = new CommerceModel()
                {
                    Id = string.IsNullOrEmpty(commerceId) ? Guid.Empty : Guid.Parse(commerceId),
                },
                Commerces = commerces,
                ApplicationUserModel = userLogead != null ? userLogead.ToAppModel() : new ApplicationUserModel()
                {
                    AcceptTermsAndConditions = true,
                },
                Setps = GenerateSteps(userLogead == null)
            };

            Session[SessionConstants.DEBIT_PROCESS] = debit;
            //PASO A PEDIR DATOS DEL SERVICIO, PASO 1
            return View("Service", debit);
        }

        [HttpGet]
        public async Task<ViewResult> Service(Guid? commerceId = null)
        {
            Session[SessionConstants.DEBIT_PROCESS] = null; //Se setea en null porque sino puede guardar datos de un debito anterior
            var sessionModel = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            if (commerceId.HasValue || sessionModel == null)
            {
                var commercesList = await _webDebitClientService.GetCommercesDebit();
                var commerces = commercesList != null ? commercesList.Select(x => x.ToModel()).OrderBy(x => x.Name) : null;

                var userLogead = await CurrentSelectedUser();

                sessionModel = new DebitModel()
                {
                    CommerceModel = new CommerceModel() { Id = commerceId ?? Guid.Empty },
                    Commerces = commerces,
                    ApplicationUserModel = userLogead != null ? userLogead.ToAppModel() : new ApplicationUserModel()
                    {
                        AcceptTermsAndConditions = true,
                    },
                    Setps = GenerateSteps(userLogead == null)
                };

                Session[SessionConstants.DEBIT_PROCESS] = sessionModel;
            }
            return View("Service", sessionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ViewResult> ServiceNext(DebitModel model)
        {
            var sessionModel = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            try
            {
                var commerceId = model.CommerceModel.Id;
                if (commerceId == Guid.Empty)
                {
                    ShowNotification("Debes seleccionar un servicio.", NotificationType.Error);
                    return View("Service", sessionModel);
                }
                var commerce = await _debitCommerceManagment.GetCommerceFromCatche(commerceId);

                model.CommerceModel.ProductosListModel = commerce.ProductosListModel;
                model.CommerceModel.Name = commerce.Name;

                var product =
                    model.CommerceModel.ProductosListModel.FirstOrDefault(x => x.Id == model.CommerceModel.ProductSelected);

                foreach (var productModelProps in product.ProductPropertyModelList)
                {
                    var value = Request.Form["UserInputProp_" + productModelProps.InputSequence];
                    productModelProps.UserInput = value;
                }

                sessionModel.CommerceModel = model.CommerceModel;

                //TODO VALIDAR LOS INPUTS
                var errors = product.ProductPropertyModelList.Select(ValidateInput).ToList();

                if (errors.Any(x => x != ""))
                {
                    foreach (var error in errors.Where(x => x != ""))
                    {
                        ShowNotification(error, NotificationType.Error);
                    }
                    return View("Service", sessionModel);
                }

                sessionModel.CardModel = new CardModel();

                if (sessionModel.ApplicationUserModel.Id != Guid.Empty)
                {
                    var userWithCards = await _webApplicationUserClientService.GetUserWithCards(sessionModel.ApplicationUserModel.Id);
                    SetCurrentSelectedUser(userWithCards);
                    if (userWithCards.CardDtos != null)
                    {
                        var availableCards = userWithCards.CardDtos.Where(x => x.Active && !x.Deleted).ToList();
                        sessionModel.ApplicationUserModel.Cards = availableCards.Select(x => new CardModel()
                        {
                            Id = x.Id,
                            Number = x.MaskedNumber,
                            Description = x.Description,
                            DueDate = x.DueDate.ToString("MM/yyyy"),
                            Active = x.Active,
                        }).ToList();
                    }
                }
                Session[SessionConstants.DEBIT_PROCESS] = sessionModel;

                var userLogead = await CurrentSelectedUser();
                ViewBag.userLogead = userLogead;

                if (userLogead == null)
                {
                    return View("User", sessionModel);
                }
                else
                {
                    return View("Card", sessionModel);
                }
            }
            catch (Exception exception)
            {
                ShowNotification("Ha ocurrido un error.", NotificationType.Error);
                NLogLogger.LogDebitEvent(exception);
            }
            return View("Service", sessionModel ?? model);
        }

        [HttpGet]
        public new ActionResult User()
        {
            var sessionModel = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            return View("User", sessionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Card(DebitModel model)
        {
            var sessionModel = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            try
            {
                sessionModel.ApplicationUserModel = model.ApplicationUserModel;

                if (!model.ApplicationUserModel.AcceptTermsAndConditions)
                {
                    ShowNotification(PresentationWebStrings.Registration_Conditions_Validation, NotificationType.Error);
                    return View("User", sessionModel);
                }

                if (model.ApplicationUserModel == null)
                {
                    ShowNotification("Faltan los datos del usuario.", NotificationType.Error);
                    return View("User", sessionModel);
                }

                try
                {
                    var isEmailUsed = await _webApplicationUserClientService.Find(model.ApplicationUserModel.Email);
                    if (isEmailUsed != null)
                    {
                        ShowNotification(
                            string.Format("Lamentablemente el correo electrónico {0} ya fue utilizado.",
                                model.ApplicationUserModel.Email), NotificationType.Error);
                        return View("User", sessionModel);
                    }
                }
                catch (WebApiClientBusinessException exception)
                {
                    //no existe el usuario
                }

                sessionModel.ApplicationUserModel.Password = HashPassword(model.ApplicationUserModel.Password);

                Session[SessionConstants.DEBIT_PROCESS] = sessionModel;
                return View("Card", sessionModel);
            }
            catch (Exception exception)
            {
                NLogLogger.LogDebitEvent(exception);
                ShowNotification("Ha ocurrido un error.", NotificationType.Error);
            }
            return View("User", sessionModel ?? model);
        }

        public async Task<ActionResult> Failed(FailedModel model)
        {
            var commercesList = await _webDebitClientService.GetCommercesDebit();
            var commerces = commercesList != null ? commercesList.Select(x => x.ToModel()).OrderBy(x => x.Name) : null;

            var userLogead = await CurrentSelectedUser();

            var debit = new DebitModel()
            {
                CommerceModel = new CommerceModel() { Id = Guid.Empty, },
                Commerces = commerces,
                ApplicationUserModel = userLogead != null ? userLogead.ToAppModel() : new ApplicationUserModel()
                {
                    AcceptTermsAndConditions = true,
                },
                Setps = GenerateSteps(userLogead == null)
            };

            ShowNotification(model.FailedMsg, NotificationType.Error);

            Session[SessionConstants.DEBIT_PROCESS] = debit;
            //PASO A PEDIR DATOS DEL SERVICIO, PASO 1
            return View("Service", debit);
        }

        [HttpGet]
        public async Task<JsonResult> GetReferences(DebitModel model)
        {
            try
            {
                var id = model.CommerceModel.ProductSelected;
                var commerceId = model.CommerceModel.Id;

                var commerce = await _debitCommerceManagment.GetCommerceFromCatche(commerceId);
                var properties = commerce.ProductosListModel.First(x => x.Id == id);

                var content = RenderPartialViewToString("_referenceList", properties);
                return Json(new JsonResponse(AjaxResponse.Success, content, string.Empty, string.Empty, NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogDebitEvent(exception);
            }
            return Json(new JsonResponse(AjaxResponse.Error, string.Empty, "No se pudo cargar las referencias de este comercio.", PresentationWebStrings.Action_Error, NotificationType.Info), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductos(DebitModel model)
        {
            try
            {
                var commerceId = model.CommerceModel.Id;
                var commerce = await _debitCommerceManagment.GetCommerceFromCatche(commerceId);
                model.CommerceModel = commerce;
                var content = RenderPartialViewToString("_products", model);
                return Json(new JsonResponse(AjaxResponse.Success, content, string.Empty, string.Empty, NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogDebitEvent(exception);
            }
            return Json(new JsonResponse(AjaxResponse.Error, string.Empty, "No se pudo cargar las referencias de este comercio.", PresentationWebStrings.Action_Error, NotificationType.Info), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ViewResult> Card()
        {
            var sessionModel = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            return View("Card", sessionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ViewResult> Create(Guid card)
        {
            NLogLogger.LogDebitEvent(NLogType.Info, "INSTRUCCION DE CREACIÓN DE DEBITO CON TOKEN SELECCIONADO");
            DebitModel model = null;

            try
            {
                model = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;

                if (card == Guid.Empty)
                {
                    ShowNotification("Debe seleccionar una tarjeta", NotificationType.Info);
                    return View("Card", model);
                }

                var product =
                    model.CommerceModel.ProductosListModel.FirstOrDefault(x => x.Id == model.CommerceModel.ProductSelected);

                var finalDto = await _webDebitClientService.Create(
                    new DebitRequestDto()
                    {
                        Id = Guid.NewGuid(),
                        CardId = card,
                        UserId = model.ApplicationUserModel.Id,
                        DebitProductId = product.DebitProductid.Value,
                        State = DebitRequestStateDto.Pending,
                        Type = DebitRequestTypeDto.High,
                        References = product.ProductPropertyModelList.Select(x => new DebitRequestReferenceDto()
                        {
                            Id = Guid.NewGuid(),
                            ProductPropertyId = x.DebitProductPropertyId.Value,
                            Value = x.UserInput,
                            Index = x.InputSequence
                        }).ToList(),
                    });

                if (finalDto != null)
                {
                    Session[SessionConstants.DEBIT_PROCESS] = null;
                    var user = await CurrentSelectedUser();
                    var confirmationModel = new ConfirmationModel()
                    {
                        Setps = GenerateSteps(false),
                        ProductPropertyModelList = finalDto.References.Select(x => new ProductPropertyModel()
                        {
                            Name = product.ProductPropertyModelList.First(y => y.DebitProductPropertyId == x.ProductPropertyId).Name,
                            UserInput = x.Value,
                            InputSequence = x.Index
                        }).ToList(),
                        CommerceName = model.CommerceModel.Name,
                        ProductName = product.Description,
                        ApplicationUserModel = new ApplicationUserModel()
                        {
                            Email = user.Email
                        },
                        Number = finalDto.CardDto.MaskedNumber
                    };
                    return View("Confirmation", confirmationModel);
                }

            }
            catch (Exception exception)
            {

            }

            return View("Card", model);
        }


        [HttpGet]
        public async Task<ActionResult> ValidateSelectedCard(Guid cardId)
        {
            try
            {
                var card = await _webCardClientService.Find(cardId);
                var today = DateTime.Now;
                if (!((card.DueDate.Year.CompareTo(today.Year) == 0 && card.DueDate.Month.CompareTo(today.Month) >= 0) || card.DueDate.Year.CompareTo(today.Year) > 0))
                {
                    return Json(new JsonResponse(AjaxResponse.Error, string.Empty, ExceptionMessages.CARD_EXPIRED, string.Empty, NotificationType.Info), JsonRequestBehavior.AllowGet);
                }

                var isValid = await _webDebitClientService.ValidateCardType(card.BIN);
                if (isValid)
                {
                    return
                        Json(
                            new JsonResponse(AjaxResponse.Success, string.Empty, string.Empty, string.Empty,
                                NotificationType.Success), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Error, string.Empty, ExceptionMessages.BIN_NOTVALID_FOR_SERVICE, string.Empty, NotificationType.Info), JsonRequestBehavior.AllowGet);

            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogDebitEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogDebitEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Payment_General_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception e)
            {
                NLogLogger.LogDebitEvent(e);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", PresentationCoreMessages.Discount_Error,
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> ValidateNewCardType(int maskedNumber, string nameTh, string fpProfiler)
        {
            try
            {
                var bin = await _webBinClientService.Find(maskedNumber);

                if (bin != null && !bin.Active)
                {
                    return
                       Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid,
                           PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                //TODO CHEQUEAR SI SE PUEDE O NO DESDE EL WEBCONFIG
                var isBinEnable = await _webDebitClientService.ValidateCardType(maskedNumber);
                if (!isBinEnable)
                {
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid_For_Service,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                var cybersourceData = await LoadKeysForCybersource(RedirectEnums.Debit, nameTh, maskedNumber.ToString(), fpProfiler);
                var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData);

                return Json(new JsonResponse(AjaxResponse.Success, new { keys = content, }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);

            }
            catch (WebApiClientBusinessException ex)
            {
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
        }

        //CALLBACK DE CYBERSOURCE
        public async Task<ActionResult> TokengenerationCallBack()
        {
            var session = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;

            try
            {
                NLogLogger.LogDebitEvent(NLogType.Info, "CREAR DEBITO DESDE CS");
                var formData = GenerateDictionary(Request.Form);
                var result = await _webDebitClientService.ProccesDataFromCybersource(formData);

                if (result.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (result.DebitRequestDto != null)
                    {
                        Session[SessionConstants.DEBIT_PROCESS] = null;
                        var commerce = await _debitCommerceManagment.GetCommerceFromCatche(result.CommerceId);
                        var product = commerce.ProductosListModel.FirstOrDefault(x => x.Id == result.ProductId);

                        var model = new ConfirmationModel()
                        {
                            Setps = result.UserCreated ? GenerateSteps(true) : GenerateSteps(false),
                            ProductPropertyModelList = result.DebitRequestDto.References.Select(x => new ProductPropertyModel()
                            {
                                Name = product.ProductPropertyModelList.First(y => y.DebitProductPropertyId == x.ProductPropertyId).Name,
                                UserInput = x.Value,
                                InputSequence = x.Index
                            }).ToList(),
                            CommerceName = commerce.Name,
                            ProductName = product.Description,
                            ApplicationUserModel = new ApplicationUserModel()
                            {
                                Email = result.DebitRequestDto.ApplicationUserDto.Email
                            },
                            Number = result.DebitRequestDto.CardDto.MaskedNumber
                        };
                        return View("Confirmation", model);
                    }
                    else
                    {
                        if (result.InternalErrorCode == 15 || result.InternalErrorCode == 14)
                        {
                            ShowNotification(result.InternalErrorDesc, NotificationType.Error);
                            return RedirectToAction("Failed", new FailedModel() { FailedMsg = result.InternalErrorDesc });
                        }
                        if (result.InternalErrorCode == 15)
                        {
                            ShowNotification(result.InternalErrorDesc, NotificationType.Error);
                            if (session != null)
                            {
                                return View("Card", session);
                            }
                        }
                        if (session != null)
                        {
                            ShowNotification(result.InternalErrorDesc, NotificationType.Error);
                            return View("Card", session);
                        }
                    }
                }
                return await EvaluateErrors(result.TokenizationData);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Info);
                NLogLogger.LogDebitEvent(NLogType.Error, ex.Message);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(PresentationWebStrings.Service_Add_Error, NotificationType.Error);
                NLogLogger.LogDebitEvent(NLogType.Error, ex.Message);
            }
            return View("Card", session);
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(RedirectEnums redirectTo, string nameTh, string cardBin, string fpProfiler)
        {
            var url = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            var model = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;

            KeysInfoForTokenDebit keysModel = null;

            if (model.ApplicationUserModel.Id != Guid.Empty)
            {
                keysModel = new KeysInfoForTokenDebitRegisteredUser()
                {
                    UserId = model.ApplicationUserModel.Id,
                };
            }
            else
            {
                keysModel = new KeysInfoForTokenDebitNewUser()
                {
                    Email = model.ApplicationUserModel.Email,
                    Name = model.ApplicationUserModel.Name,
                    Surname = model.ApplicationUserModel.Surname,
                    MobileNumber = model.ApplicationUserModel.MobileNumber,
                    Password = model.ApplicationUserModel.Password,//HashPassword(model.ApplicationUserModel.Password),
                    IdentityNumber = model.ApplicationUserModel.IdentityNumber,
                    UserId = Guid.Empty,
                };
            }

            keysModel.TransactionReferenceNumber = Guid.NewGuid().ToString();
            keysModel.RedirectTo = redirectTo.ToString("D");
            keysModel.NameTh = nameTh;
            keysModel.CardBin = cardBin;
            keysModel.CallcenterUser = String.Empty;
            keysModel.Platform = PaymentPlatformDto.VisaNet.ToString();
            keysModel.PaymentTypeDto = PaymentTypeDto.Debit;
            keysModel.UrlReturn = url;
            keysModel.FingerPrint = fpProfiler;
            keysModel.OperationTypeDto = OperationTypeDto.Subscription;

            keysModel.ProductId = model.CommerceModel.ProductSelected;
            keysModel.CommerceId = model.CommerceModel.Id;

            var product =
                model.CommerceModel.ProductosListModel.FirstOrDefault(x => x.Id == model.CommerceModel.ProductSelected);

            keysModel.CommerceAndProductName = model.CommerceModel.Name + " - " + product.Description;

            var props = product.ProductPropertyModelList.OrderBy(x => x.InputSequence);

            for (int i = 0; i < props.Count(); i++)
            {
                var prop = props.ElementAt(i);
                switch (i)
                {
                    case 0:
                        keysModel.ReferenceNumber1 = prop.UserInput;
                        break;
                    case 1:
                        keysModel.ReferenceNumber2 = prop.UserInput;
                        break;
                    case 2:
                        keysModel.ReferenceNumber3 = prop.UserInput;
                        break;
                    case 3:
                        keysModel.ReferenceNumber4 = prop.UserInput;
                        break;
                    case 4:
                        keysModel.ReferenceNumber5 = prop.UserInput;
                        break;
                    case 5:
                        keysModel.ReferenceNumber6 = prop.UserInput;
                        break;
                }
            }

            var keys = await _webCyberSourceAccessClientService.GenerateKeys(keysModel);

            Session[SessionConstants.TEMPORARY_ID] = keys["merchant_defined_data29"];
            return keys;
        }

        private string ValidateInput(ProductPropertyModel model)
        {
            var result = string.Empty;

            if (model.Requiered && string.IsNullOrEmpty(model.UserInput))
                result = result + string.Format("El campo {0} es requerido. ", model.Name);

            if (model.MaxSize > 0 && model.UserInput.Length > model.MaxSize)
                result = string.Format("El campo {0} sobrepasa el tamaño permitodo {1}. ", model.Name, model.MaxSize);

            if (!string.IsNullOrEmpty(model.ContentType)) //A AN N
            {
                if (model.ContentType.ToUpper().Equals("N"))
                {
                    double value = 0;
                    if (!double.TryParse(model.UserInput, out value))
                        result = string.Format("El campo {0} no es un campo numérico. ", model.Name);
                }
                if (model.ContentType.ToUpper().Equals("A"))
                {
                    //return s.All(char.IsDigit);
                    var hasNumber = model.ContentType.Any(char.IsDigit);
                    if (hasNumber)
                        result = string.Format("El campo {0} no es un campo alfabético. ", model.Name);
                }
            }

            return result;
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

        private string HashPassword(string password)
        {
            return PasswordHash.CreatePasswordForApps(password);
        }

        private async Task<ActionResult> EvaluateErrors(CsResponseData csResponseData)
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
                        return RedirectToAction("Failed", new FailedModel()
                        {
                            FailedMsg = "Estimado usuario, en este momento no hemos podido procesar su solicitud a través del banco. " +
                                        "Por favor intentá nuevamente o comunícate con nuestro Call Center a través de nuestro formulario de contacto.",
                        });
                }
            }
            return null;
        }

        private ActionResult InvalidCardData(CsResponseData csResponseData)
        {
            var session = Session[SessionConstants.DEBIT_PROCESS] as DebitModel;
            if (session != null)
            {
                ShowNotification(csResponseData.PaymentResponseMsg, NotificationType.Error);
                return View("Card", session);
            }
            else
            {
                return RedirectToAction("Failed", new FailedModel()
                {
                    FailedMsg = csResponseData.PaymentResponseMsg
                });
            }
        }

        private Dictionary<DebitsStepsEnum, int> GenerateSteps(bool newUser)
        {
            if (newUser)
            {
                return new Dictionary<DebitsStepsEnum, int>()
                {
                    {DebitsStepsEnum.Service, 1},
                    {DebitsStepsEnum.User, 2},
                    {DebitsStepsEnum.Card, 3},
                    {DebitsStepsEnum.Confirmation, 4},
                };
            }

            return new Dictionary<DebitsStepsEnum, int>()
                    {
                        {DebitsStepsEnum.Service, 1},
                        {DebitsStepsEnum.Card, 2},
                        {DebitsStepsEnum.Confirmation, 3},
                    };
        }
    }
}