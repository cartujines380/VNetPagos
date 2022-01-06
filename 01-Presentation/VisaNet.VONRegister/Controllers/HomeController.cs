using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Utilities.DigitalSignature;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;
using VisaNet.VONRegister.Constants;
using VisaNet.VONRegister.Models;

namespace VisaNet.VONRegister.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebServiceAssosiateClientService _assosiateClientService;
        private readonly IWebWebhookLogClientService _webWebhookLogService;

        public HomeController(IWebServiceAssosiateClientService assosiateClientService, IWebWebhookLogClientService webWebhookLogService)
        {
            _assosiateClientService = assosiateClientService;
            _webWebhookLogService = webWebhookLogService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            NLogLogger.LogAppsEvent(NLogType.Info, "");
            return View(new Register());
        }

        [HttpGet]
        public ActionResult End(End model)
        {
            return View("End", model);
        }

        [HttpPost]
        public async Task<ActionResult> TokengenerationCallBack()
        {
            var processValue = DateTime.Now.ToString("yyyyMMddhhmmss");
            var resultCode = "1";
            var resultDescription = "Error General.";
            CybersourceCreateAppAssociationDto result = null;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "016 - TokengenerationCallBack - INICIO METODO. PROCESO " + processValue);

                var formData = GenerateDictionary(Request.Form);

                result = await _assosiateClientService.ProccesDataFromCybersourceForApps(formData);


                if (result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode == (int)ErrorCodeDto.OK)
                {
                    if (result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                    {
                        if (result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto != null)
                        {
                            resultCode = "0";
                            resultDescription = "Se realizó la asociación correctamente";
                        }
                        else
                        {
                            resultCode = "1";
                            resultDescription = "Error general";
                        }
                    }
                    else
                    {
                        resultCode = result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode.ToString();
                        resultDescription = result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseMsg;
                    }
                }
                else
                {
                    resultCode = result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode.ToString();
                    resultDescription = result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc;
                }

                var invalidCardMsg = await InvalidCardData((CybersourceMsg)result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode,
                    result.CybersourceCreateServiceAssociatedDto.CyberSourceMerchantDefinedData);

                if (!string.IsNullOrEmpty(invalidCardMsg))
                {
                    if (IsSessionVariblesAcive())
                    {
                        ShowToastr(invalidCardMsg, NotificationType.Error);
                        return RedirectToAction("Add", "Card");
                    }
                }

                var model = new End()
                            {
                                UrlCallback = result.WebhookRegistrationDto.UrlCallback,
                                OperationId = result.WebhookRegistrationDto.IdOperation,
                                ResultCode = resultCode,
                                ResultDescription = resultDescription,
                            };
                ClearSessionVariables();
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("016 - (IdOperacion:({1}) - TokengenerationCallBack - Realizo POST a {0}, id de operación {1}, PROCESO {2}, idapp {3}", result.WebhookRegistrationDto.UrlCallback, result.WebhookRegistrationDto.IdOperation, processValue, result.WebhookRegistrationDto.IdApp));
                ViewBag.ResultCode = resultCode;
                return View("End", model);
            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, string.Format("016 - (IdOperacion:({1}) - TokengenerationCallBack WebApiClientBusinessException- PROCESO {0}", processValue, Request.Form["req_merchant_defined_data27"]));
                NLogLogger.LogAppsEvent(ex);
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, string.Format("016 - (IdOperacion:({1}) - TokengenerationCallBack WebApiClientFatalException- PROCESO {0}", processValue, Request.Form["req_merchant_defined_data27"]));
                NLogLogger.LogAppsEvent(ex);
            }
            catch (Exception ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, string.Format("016 - (IdOperacion:({1}) - TokengenerationCallBack Excepcion- PROCESO {0}", processValue, Request.Form["req_merchant_defined_data27"]));
                NLogLogger.LogAppsEvent(ex);
            }

            var idOperation = Request.Form["req_merchant_defined_data27"];
            var idService = Guid.Parse(Request.Form["req_merchant_defined_data30"]);
            var logOperation = await _webWebhookLogService.GetwebHookRegistrationsByIdOperation(idOperation, idService);

            if (logOperation != null)
            {
                if (string.IsNullOrEmpty(resultCode))
                {
                    resultCode = "1";
                }
                if (string.IsNullOrEmpty(resultDescription))
                {
                    resultDescription = "Error General";
                }

                var model = new End
                {
                    UrlCallback = logOperation.UrlCallback,
                    OperationId = logOperation.IdOperation,
                    ResultCode = resultCode,
                    ResultDescription = resultDescription,
                };
                return View("End", model);
            }
            return View("Error");
        }

        [HttpGet]
        public ActionResult Cancel()
        {
            FormsAuthentication.SignOut();
            var model = new End()
            {
                UrlCallback = Session[SessionConstants.CallbackUrl] as string,
                OperationId = Session[SessionConstants.OperationId] as string,
                ResultCode = "16",
                ResultDescription = "El usuario ha cancelado la operación.",
            };
            ClearSessionVariables();
            return View("End", model);
        }

        private async Task<string> InvalidCardData(CybersourceMsg reasonCode, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            var msg = string.Empty;
            if (reasonCode != CybersourceMsg.Accepted)
            {
                switch (reasonCode)
                {
                    case CybersourceMsg.InvalidFields:
                        msg = PresentationWebStrings.Card_InvalidFields;
                        break;
                    case CybersourceMsg.ExpiredCard:
                        msg = PresentationWebStrings.Card_ExpiriedCard;
                        break;
                    case CybersourceMsg.InsufficientFunds:
                        msg = PresentationWebStrings.Card_InsufficientFunds;
                        break;
                    case CybersourceMsg.StolenLostCard:
                        msg = PresentationWebStrings.Card_StolenLostCard;
                        break;
                    case CybersourceMsg.CreditLimitReached:
                        msg = PresentationWebStrings.Card_CreditLimitReached;
                        break;
                    case CybersourceMsg.InvalidCVN:
                        msg = PresentationWebStrings.Card_InvalidCVN;
                        break;
                    case CybersourceMsg.AccountFrozen:
                        msg = PresentationWebStrings.Card_AccountFrozen;
                        break;
                    case CybersourceMsg.InvalidAccountNumber:
                        msg = PresentationWebStrings.Card_InvalidAccountNumber;
                        break;
                    case CybersourceMsg.CardTypeNotAccepted:
                        msg = PresentationWebStrings.Card_CardTypeNotAccepted;
                        break;
                    case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                        msg = PresentationWebStrings.Card_InvalidCardTypeOrNotCorrelateWithCardNumber;
                        break;
                    case CybersourceMsg.CVNCheckInvalid:
                        msg = PresentationWebStrings.Card_CVNCheckInvalid;
                        break;
                    default:
                        msg =
                            "No hemos podido procesar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.";
                        break;
                }
            }
            return msg;
        }

        private bool IsSessionVariblesAcive()
        {

            var model = Session[SessionConstants.RegisterModel] as Register;
            var service = Session[SessionConstants.CurrentService] as ServiceDto;

            if (model == null || service == null) return false;

            if (!model.NewUser)
            {
                var user = Session[SessionConstants.CurrentSelectedUser] as ApplicationUserDto;
                if (user == null) return false;
            }

            return true;
        }

        [HttpPost]
        public ActionResult Signature()
        {
            var email = Request.Form["Email"];
            var name = Request.Form["Nombre"];
            var surname = Request.Form["Apellido"];
            var address = Request.Form["Direccion"];
            var phone = Request.Form["Telefono"];
            var mobile = Request.Form["Movil"];
            var identity = Request.Form["CI"];
            var allowsNewEmail = Request.Form["PermiteCambioEmail"] ?? "S"; //por defecto es S
            var operationId = Request.Form["IdOperacion"];
            var callbackUrl = Request.Form["UrlCallback"];
            var appId = Request.Form["IdApp"];

            var paramsArray = new[]
                {
                    appId,
                    email,
                    name,
                    surname,
                    address,
                    phone,
                    mobile,
                    identity,
                    Request.Form["PermiteCambioEmail"],
                    operationId,
                    callbackUrl
                };

            var newstr = new string("‎3B4D89047F1FABBD8B6D93BAEEC14F68B7779AA9".Where(c => c < 128).ToArray());

            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            return Json(new JsonResponse(AjaxResponse.Success, siganture, PresentationWebStrings.Bin_Not_Valid_For_Service, PresentationCoreMessages.NotificationFail, NotificationType.Error));
        }

    }
}