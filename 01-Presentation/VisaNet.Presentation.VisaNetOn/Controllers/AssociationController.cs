using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.VisaNetOn.Constants;
using VisaNet.Presentation.VisaNetOn.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class AssociationController : BaseController
    {
        private readonly IWebBinClientService _binClientService;
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebCyberSourceAccessClientService _cyberSourceAccessClientService;
        private readonly IWebVisaNetOnIntegrationClientService _visaNetOnIntegrationClientService;
        private readonly IWebWebhookRegistrationClientService _webhookRegistrationClientService;
        private readonly IWebServiceAssosiateClientService _serviceAssosiateClientService;
        private readonly IWebAnonymousUserClientService _anonymousUserClientService;

        public AssociationController(IWebBinClientService binClientService,
            IWebServiceClientService serviceClientService, IWebCyberSourceAccessClientService cyberSourceAccessClientService,
            IWebVisaNetOnIntegrationClientService visaNetOnIntegrationClientService, IWebWebhookRegistrationClientService webhookRegistrationClientService,
            IWebServiceAssosiateClientService serviceAssosiateClientService, IWebAnonymousUserClientService anonymousUserClientService)
        {
            _binClientService = binClientService;
            _serviceClientService = serviceClientService;
            _cyberSourceAccessClientService = cyberSourceAccessClientService;
            _visaNetOnIntegrationClientService = visaNetOnIntegrationClientService;
            _webhookRegistrationClientService = webhookRegistrationClientService;
            _serviceAssosiateClientService = serviceAssosiateClientService;
            _anonymousUserClientService = anonymousUserClientService;
        }

        public ActionResult Index()
        {
            var model = (PageAssociationModel)Session[SessionConstants.PAGE_ASSOCIATION_MODEL];
            Session[SessionConstants.PAGE_ASSOCIATION_MODEL] = null;

            if (model != null)
            {
                return View("Association", model);
            }

            NLogLogger.LogEvent(NLogType.Error, "AssociationController - Index - PageAssociationModel null.");
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCard(PageAssociationModel model)
        {
            try
            {
                if (model.UserData.AnonymousUserId.HasValue)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - ValidateCard - IdApp: {0}, IdOperacion: {1}, NewCard:{2}, SelectedCardId:{3}, CardBin:{4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.NewCard, model.SelectedCardId, model.CardBin));
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - ValidateCard - IdApp: {0}, IdOperacion: {1}, NewCard:{2}, SelectedCardId:{3}, CardBin:{4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.NewCard, model.SelectedCardId, model.CardBin));
                }

                var bin = await _binClientService.Find(model.CardBin);
                if (bin != null && !bin.Active)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, null,
                        PresentationWebStrings.Bin_Not_Valid, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                //si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = bin == null || await _serviceClientService.IsBinAssociatedToService(bin.Value, model.ServiceInfo.ServiceId);
                if (!isBinAssociatedToService)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, null,
                        PresentationWebStrings.Bin_Not_Valid_For_Service, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Success, null,
                    string.Empty, string.Empty, NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationWebStrings.InvalidData, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> LoadCyberSourceKeysForAssociation(PageAssociationModel model, string nameTh, string fingerprint)
        {
            try
            {
                string name, surname;
                FillNameAndSurname(model.UserData.Name, model.UserData.Surname, nameTh, out name, out surname);
                model.UserData.Name = name;
                model.UserData.Surname = surname;

                if (model.NewCard)
                {
                    var token = await InitializeToken(model);

                    token.ServiceId = model.ServiceInfo.ServiceId;
                    token.TransactionReferenceNumber = Guid.NewGuid().ToString();
                    token.NameTh = nameTh;
                    token.CardBin = model.CardBin.ToString();
                    token.CallcenterUser = string.Empty;
                    token.Platform = PaymentPlatformDto.VisaNetOn.ToString();
                    token.PaymentTypeDto = PaymentTypeDto.App;
                    token.UrlReturn = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
                    token.OperationId = model.IdOperation;
                    token.ReferenceNumber1 = model.ServiceInfo.ReferenceNumber1;
                    token.ReferenceNumber2 = model.ServiceInfo.ReferenceNumber2;
                    token.ReferenceNumber3 = model.ServiceInfo.ReferenceNumber3;
                    token.ReferenceNumber4 = model.ServiceInfo.ReferenceNumber4;
                    token.ReferenceNumber5 = model.ServiceInfo.ReferenceNumber5;
                    token.ReferenceNumber6 = model.ServiceInfo.ReferenceNumber6;
                    token.FingerPrint = fingerprint;

                    var cybersourceData = await _cyberSourceAccessClientService.GenerateKeys(token);
                    var cybersourceKeys = RenderPartialViewToString("_CybersourceKeys", cybersourceData);
                    var data = new { Keys = cybersourceKeys };
                    return Json(new JsonResponse(AjaxResponse.Success, data, "", ""), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //No debería entrar acá (tarjeta existente entra a AssociateCard)
                    return Json(new JsonResponse(AjaxResponse.Error, null,
                        PresentationWebStrings.From_General_Error, PresentationCoreMessages.Notification_Title_Alert, NotificationType.Error), JsonRequestBehavior.AllowGet);
                }
            }
            catch (WebApiClientBusinessException e)
            {
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    e.Message, PresentationCoreMessages.Notification_Title_Alert, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - LoadCyberSourceKeysForAssociation - Ha ocurrido un error - IdApp: {0}, IdOperation: {1}",
                    model.ServiceInfo.IdApp, model.IdOperation));
                return Json(new JsonResponse(AjaxResponse.Error, null,
                    PresentationWebStrings.Apps_AssociationError, PresentationCoreMessages.NotificationFail, NotificationType.Error), JsonRequestBehavior.AllowGet);
            }
        }

        //Callback de Cybersource para asociacion
        public async Task<ActionResult> AssociationTokenGenerationCallback()
        {
            NLogLogger.LogEvent(NLogType.Info, "AssociationController - AssociationTokenGenerationCallback - Llega comunicacion de CS");
            NLogLogger.LogEvent(NLogType.Info, "AssociationController - AssociationTokenGenerationCallback - RequestId: " + Request.Form["transaction_id"]);

            var dictionaryData = GenerateDictionary(Request.Form);
            var actionRedirect = Enum.Parse(typeof(RedirectEnums), dictionaryData["ActionRedirect"]);
            dictionaryData.Remove("ActionRedirect");
            var processOperationDto = new ProcessOperationDto
            {
                FormData = dictionaryData,
                Action = (RedirectEnums)actionRedirect
            };

            var operationId = dictionaryData["req_merchant_defined_data27"];
            var serviceId = new Guid(dictionaryData["req_merchant_defined_data30"]);
            var webHook = await _webhookRegistrationClientService.GetByIdOperation(operationId, serviceId);

            try
            {
                var result = await _visaNetOnIntegrationClientService.ProcessOperation(processOperationDto);
                return RedirectToAction("End", "Home", new End
                {
                    UrlCallback = webHook.UrlCallback,
                    OperationId = operationId,
                    ResultCode = result.ResultCode,
                    ResultDescription = result.ResultDescription,
                    AppId = webHook.IdApp,
                    WebhookRegistrationId = webHook.Id
                });
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociationTokenGenerationCallback - Ha ocurrido un error - IdApp: {0}, IdOperation: {1}",
                    webHook.IdApp, operationId));
                NLogLogger.LogEvent(e);

                return RedirectToAction("End", "Home", new End
                {
                    UrlCallback = webHook.UrlCallback,
                    OperationId = operationId,
                    ResultCode = "1",
                    ResultDescription = "Error general.",
                    AppId = webHook.IdApp,
                    WebhookRegistrationId = webHook.Id
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AssociateCard(PageAssociationModel model)
        {
            var resultCode = "1";
            var resultDescription = "Ha ocurrido un error.";

            try
            {
                if (!model.NewUser && model.UserData.ApplicationUserId.HasValue && model.UserData.ApplicationUserId.Value != Guid.Empty &&
                    model.SelectedCardId.HasValue && model.SelectedCardId.Value != Guid.Empty)
                {
                    //Usuario registrado con tarjeta existente
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociateCard - Usuario registrado con tarjeta existente - IdApp: {0}, IdOperation: {1}, IdUsuario: {2}, IdServicio: {3}, CardId: {4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.UserData.ApplicationUserId, model.ServiceInfo.ServiceId, model.SelectedCardId.Value));

                    var dto = new ServiceAssociatedDto
                    {
                        UserId = model.UserData.ApplicationUserId.Value,
                        ServiceId = model.ServiceInfo.ServiceId,
                        DefaultCardId = model.SelectedCardId.Value,
                        OperationId = model.IdOperation,
                        Enabled = true,
                        Active = true,
                    };
                    var result = await _serviceAssosiateClientService.AssociateServiceToUserFromCardCreated(dto);
                    //Si no se puede notificar a la url alta, result devuelve false y se devuelve codigo 1: Error general.
                    if (result != null)
                    {
                        resultCode = "0";
                        resultDescription = "Asociación exitosa.";
                    }
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociateCard - Asociacion exitosa - IdApp: {0}, IdOperation: {1}, IdUsuario: {2}, IdServicio: {3}, CardId: {4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.UserData.ApplicationUserId, model.ServiceInfo.ServiceId, model.SelectedCardId.Value));
                }
                else if (!model.NewUser && model.UserData.AnonymousUserId.HasValue && model.UserData.AnonymousUserId.Value != Guid.Empty &&
                   model.SelectedCardId.HasValue && model.SelectedCardId.Value != Guid.Empty)
                {
                    //Usuario recurrente con tarjeta existente
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociateCard - Usuario recurrente con tarjeta existente - IdApp: {0}, IdOperation: {1}, IdUsuario: {2}, IdServicio: {3}, CardId: {4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.UserData.AnonymousUserId.Value, model.ServiceInfo.ServiceId, model.SelectedCardId.Value));

                    resultCode = "0";
                    resultDescription = "Asociación exitosa.";

                    NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociateCard - Asociacion exitosa - IdApp: {0}, IdOperation: {1}, IdUsuario: {2}, IdServicio: {3}, CardId: {4}",
                        model.ServiceInfo.IdApp, model.IdOperation, model.UserData.AnonymousUserId.Value, model.ServiceInfo.ServiceId, model.SelectedCardId.Value));
                }

                return RedirectToAction("End", "Home", new End
                {
                    OperationId = model.IdOperation,
                    UrlCallback = model.ServiceInfo.UrlCallback,
                    ResultCode = resultCode,
                    ResultDescription = resultDescription,
                    AppId = model.ServiceInfo.IdApp,
                    WebhookRegistrationId = model.WebhookRegistrationId
                });
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("AssociationController - AssociateCard - Ha ocurrido un error - IdApp: {0}, IdOperation: {1}",
                    model.ServiceInfo.IdApp, model.IdOperation));
                NLogLogger.LogEvent(e);

                return RedirectToAction("End", "Home", new End
                {
                    OperationId = model.IdOperation,
                    UrlCallback = model.ServiceInfo.UrlCallback,
                    ResultCode = "1",
                    ResultDescription = "Ha ocurrido un error.",
                    AppId = model.ServiceInfo.IdApp,
                    WebhookRegistrationId = model.WebhookRegistrationId
                });
            }
        }

        [HttpPost]
        public ActionResult CancelAssociation(PageAssociationModel model)
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

        private async Task<KeysInfoForToken> InitializeToken(PageAssociationModel model)
        {
            KeysInfoForToken token;
            var userId = Guid.Empty;
            string redirectTo;

            if (model.UserData.ApplicationUserId.HasValue)
            {
                //Usuario registrado en VisaNetPagos
                token = new KeysInfoForTokenRegisteredUser();
                userId = model.UserData.ApplicationUserId.Value;
                redirectTo = (RedirectEnums.VisaNetOnTokenizationRegistered).ToString("D");
            }
            else if (model.UserData.AnonymousUserId.HasValue)
            {
                //Usuario recurrente de VisaNetOn
                token = new KeysInfoForTokenRecurrentUser();
                userId = model.UserData.AnonymousUserId.Value;
                redirectTo = (RedirectEnums.VisaNetOnTokenizationRecurrent).ToString("D");
            }
            else
            {
                //Se crea/obtiene usuario anonimo
                var anonymousUser = await LoadAnonymousUserData(model);
                userId = anonymousUser.Id;

                var isUserAssociatedToService = await IsRecurrentUserAssociatedToService(model.ServiceInfo.IdApp, userId);
                if (isUserAssociatedToService)
                {
                    //Usuario recurrente de VisaNetOn
                    token = new KeysInfoForTokenRecurrentUser();
                    redirectTo = (RedirectEnums.VisaNetOnTokenizationRecurrent).ToString("D");
                }
                else
                {
                    //Se va a registrar al usuario recurrente al regreso de CS
                    token = new KeysInfoForTokenNewUser
                    {
                        Email = model.UserData.Email,
                        Name = model.UserData.Name,
                        Surname = model.UserData.Surname,
                        Address = model.UserData.Address,
                    };
                    redirectTo = (RedirectEnums.VisaNetOnTokenizationNewUser).ToString("D");
                }
            }

            token.UserId = userId;
            token.RedirectTo = redirectTo;

            return token;
        }

        private async Task<AnonymousUserDto> LoadAnonymousUserData(PageAssociationModel model)
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

    }
}