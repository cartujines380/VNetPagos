using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Controllers
{
    public class HighwayAdmissionController : BaseController
    {
        private readonly IWebApplicationUserClientService _userService;
        private readonly IWebRegisterUserClientService _registerUserClientService;
        private readonly IWebBinClientService _binClientService;
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebLogClientService _logClientService;
        private readonly IWebPageClientService _pageClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;

        public HighwayAdmissionController(IWebApplicationUserClientService userService, IWebBinClientService binClientService, IWebServiceClientService webServiceClientService, IWebServiceAssosiateClientService webServiceAssosiateClientService, IWebApplicationUserClientService webApplicationUserClientService, IWebLogClientService logClientService, IWebRegisterUserClientService registerUserClientService, IWebPageClientService pageClientService, IWebCyberSourceAccessClientService cyberSourceAccessClientService)
        {
            _userService = userService;
            _binClientService = binClientService;
            _webServiceClientService = webServiceClientService;
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _webApplicationUserClientService = webApplicationUserClientService;
            _logClientService = logClientService;
            _registerUserClientService = registerUserClientService;
            _pageClientService = pageClientService;
            _webCyberSourceAccessClientService = cyberSourceAccessClientService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                var serviceDto = await _webServiceClientService.GetServiceByUrlName(id);
                if (serviceDto != null)
                {
                    var model = new HighwayAdmissionModel()
                    {
                        ServiceId = serviceDto.Id,
                        ServiceName = serviceDto.Name,
                        ServiceUrlName = id,
                        SuccessFulAssociation = false,
                        ServiceImage = serviceDto.ImageUrl,
                        LoadNewData = false,
                        LoadRegistredData = false,
                        ReferenceName = serviceDto.ReferenceParamName,
                        ReferenceName2 = serviceDto.ReferenceParamName2,
                        ReferenceName3 = serviceDto.ReferenceParamName3,
                        ReferenceName4 = serviceDto.ReferenceParamName4,
                        ReferenceName5 = serviceDto.ReferenceParamName5,
                        ReferenceName6 = serviceDto.ReferenceParamName6,
                        PostAssociationDesc = serviceDto.PostAssociationDesc,
                        TermsAndConditionsService = serviceDto.TermsAndConditions,
                        AcceptTermsAndConditionsService = true,
                        AcceptTermsAndConditionsVisa = true,
                    };
                    Session[SessionConstants.HIGHWAY_ADMISSION] = model;

                    return View(model);
                }

                ShowNotification("Servicio no encontrado", NotificationType.Info);
                return View();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - Index - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return View();
        }

        private void CleanSession()
        {
            Session[SessionConstants.HIGHWAY_ADMISSION] = null;
            Session[SessionConstants.CURRENT_SELECTED_USER] = null;
            //Session[SessionConstants.CURRENT_USER_ID] = null;
            //Session[SessionConstants.CURRENT_USER_TYPE] = null;
        }

        public async Task<ActionResult> LoadUserAjax(string userName, string password)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "Error en ingreso.";
            var title = "Error";
            string content;

            try
            {
                if (await _userService.ValidateUser(new ValidateUserDto { UserName = userName, Password = password }))
                {
                    var user = await LoadUserWithCards(userName);

                    var cardList = new List<HighwayCardModel>();

                    message = "Ingrese los datos del servicio y agregue una nueva tarjeta para asociar.";

                    if (user.CardDtos != null && user.CardDtos.Count(x => x.Active) > 0)
                    {
                        cardList.AddRange(from dto in user.CardDtos where dto.Active select CardConverter(dto));
                        message = "Ingrese los datos del servicio y seleccione una tarjeta existente, o agregue una nueva.";
                    }
                    title = "Ingreso correcto";

                    var model = (HighwayAdmissionModel)Session[SessionConstants.HIGHWAY_ADMISSION];
                    if (model == null)
                    {
                        return Json(new JsonResponse(AjaxResponse.Error, "", "Error en pagina. ", PresentationCoreMessages.NotificationFail, NotificationType.Error));
                    }
                    model.UserId = user.Id;
                    model.Name = user.Name;
                    model.Surname = user.Surname;
                    model.Email = user.Email;
                    model.AssociatedCards = cardList;

                    model.LoadRegistredData = true;

                    Session[SessionConstants.HIGHWAY_ADMISSION] = model;

                    content = RenderPartialViewToString("_CardList", model);
                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;

                    return Json(new JsonResponse(response,
                        new
                        {
                            cardListContent = content,
                            userId = user.Id,
                            userName = user.Name,
                            userSurName = user.Surname,
                            userEmail = user.Email
                        },
                        message, title, notification), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - LoadUserAjax - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            content = RenderPartialViewToString("_CardList", null);

            return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);

        }

        private static HighwayCardModel CardConverter(CardDto cardDto)
        {
            if (cardDto != null)
            {
                var card = new HighwayCardModel
                {
                    Id = cardDto.Id,
                    Active = cardDto.Active,
                    DueDate = cardDto.DueDate,
                    Mask = cardDto.MaskedNumber

                };
                return card;
            }
            return null;
        }

        private async Task<ApplicationUserDto> LoadUserWithCards(string userName)
        {
            try
            {
                var user = await _userService.Find(userName);
                Session[SessionConstants.CURRENT_SELECTED_USER] = user;
                //Session[SessionConstants.CURRENT_SELECTED_USER_ID] = user.Id;
                //Session[SessionConstants.CURRENT_USER_TYPE] = CurrentUserType.Public;

                return user;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - LoadUserWithCards - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> Associate()
        {
            var model = (HighwayAdmissionModel)Session[SessionConstants.HIGHWAY_ADMISSION];

            if (model == null)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", "Error en pagina. ", PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            if (model.UserId == null || model.UserId == Guid.Empty)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", "Error en usuario.", PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }

            if (model.CardId == null || model.CardId == Guid.Empty)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", "Error en tarjeta.", PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }

            var appUser = await _webApplicationUserClientService.Find(model.UserId.Value);

            #region Registro Log

            var strLog = string.Format(LogStrings.Service_init,
                appUser.Email, appUser.Name, appUser.Surname,
                model.ServiceName);
            await _logClientService.Put(new LogModel
            {
                LogType = LogType.Info,
                LogUserType = LogUserType.Registered,
                LogCommunicationType = LogCommunicationType.VisaNet,
                LogOperationType = LogOperationType.ServiceAssociated,
                Message = strLog,
                CallCenterMessage = strLog,
            });
            #endregion

            var serviceAssociatedDto = new ServiceAssociatedDto
            {
                ServiceId = model.ServiceId,
                ReferenceNumber = model.ReferenceNumber,
                ReferenceNumber2 = model.ReferenceNumber2,
                ReferenceNumber3 = model.ReferenceNumber3,
                ReferenceNumber4 = model.ReferenceNumber4,
                ReferenceNumber5 = model.ReferenceNumber5,
                ReferenceNumber6 = model.ReferenceNumber6,
                UserId = appUser.Id,
                DefaultCardId = model.CardId.Value,
                Enabled = true,

            };

            var noticonf = new NotificationConfigDto()
            {
                DaysBeforeDueDate = 3,
                BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                {
                    Email = true,
                    Sms = false,
                    Web = false
                },
                ExpiredBillDto = new ExpiredBillDto()
                {
                    Email = true,
                    Sms = false,
                    Web = false
                },
                NewBillDto = new NewBillDto()
                {
                    Email = true,
                    Sms = false,
                    Web = false
                },
                FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                {
                    Email = true,
                    Sms = false,
                    Web = false
                },
                SuccessPaymentDto = new SuccessPaymentDto()
                {
                    Email = true,
                    Sms = false,
                    Web = false
                }
            };

            serviceAssociatedDto.NotificationConfigDto = noticonf;

            serviceAssociatedDto = await _webServiceAssosiateClientService.Create(serviceAssociatedDto);

            ViewBag.Email = appUser.Email;
            ViewBag.ServiceName = model.ServiceName;
            ViewBag.SuccessfulAssociation = true;

            CleanSession();

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCardTypeAndReferences(HighwayAdmissionModel mUser)
        {
            try
            {
                var model = (HighwayAdmissionModel)Session[SessionConstants.HIGHWAY_ADMISSION];
                if (model == null)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Error en pagina. ", PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                if (!mUser.AcceptTermsAndConditionsVisa)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Debe aceptar los Términos y Condiciones.", "Atención!", NotificationType.Alert));
                }
                if (!String.IsNullOrEmpty(model.TermsAndConditionsService) && !mUser.AcceptTermsAndConditionsService)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Debe aceptar los Términos y Condiciones.", "Atención!", NotificationType.Alert));
                }

                //SI NO HAY USUARIO VALIDO DATOS DE USUARIO NUEVO
                if (mUser.UserId == null || mUser.UserId == Guid.Empty)
                {
                    if (!ModelState.IsValid)
                    {
                        var content = RenderPartialViewToString("_NewUserData", mUser);
                        return
                            Json(new JsonResponse(AjaxResponse.Error, content, "Faltan campos", PresentationCoreMessages.NotificationFail, NotificationType.Error));
                    }
                    try
                    {
                        var user = await _webApplicationUserClientService.Find(mUser.Email);
                        if (user != null)
                        {
                            return
                                Json(new JsonResponse(AjaxResponse.Error, null, ExceptionMessages.USER_EMAIL_DUPLICATED,
                                    PresentationCoreMessages.NotificationFail, NotificationType.Error));
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.LogEvent(ex);
                    }

                    model.Name = mUser.Name;
                    model.Surname = mUser.Surname;
                    model.Email = mUser.Email;

                }

                var service = await _webServiceClientService.Find(model.ServiceId);

                #region Validate References

                var message = "Debe completar los siguientes campos: ";
                var refError = false;

                if (string.IsNullOrEmpty(mUser.ReferenceNumber))
                {
                    message = message + model.ReferenceName + " ";
                    refError = true;
                }
                if (!string.IsNullOrEmpty(model.ReferenceName2) && string.IsNullOrEmpty(mUser.ReferenceNumber2))
                {
                    message = message + model.ReferenceName2 + " ";
                    refError = true;
                }
                if (!string.IsNullOrEmpty(model.ReferenceName3) && string.IsNullOrEmpty(mUser.ReferenceNumber3))
                {
                    message = message + model.ReferenceName3 + " ";
                    refError = true;
                }
                if (!string.IsNullOrEmpty(model.ReferenceName4) && string.IsNullOrEmpty(mUser.ReferenceNumber4))
                {
                    message = message + model.ReferenceName4 + " ";
                    refError = true;
                }
                if (!string.IsNullOrEmpty(model.ReferenceName5) && string.IsNullOrEmpty(mUser.ReferenceNumber5))
                {
                    message = message + model.ReferenceName5 + " ";
                    refError = true;
                }
                if (!string.IsNullOrEmpty(model.ReferenceName6) && string.IsNullOrEmpty(mUser.ReferenceNumber6))
                {
                    message = message + model.ReferenceName6 + " ";
                    refError = true;
                }

                if (refError)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", message, "¡Atención!", NotificationType.Error));
                }

                #endregion

                if (model.UserId != null && mUser.UserId != null && mUser.UserId.Value != Guid.Empty)
                {
                    var refs = new string[]
                               {
                                   mUser.ReferenceNumber, mUser.ReferenceNumber2, mUser.ReferenceNumber3, mUser.ReferenceNumber4, mUser.ReferenceNumber5, mUser.ReferenceNumber6
                               };

                    var serviceAssociatedId = await _webServiceAssosiateClientService.IsServiceAssosiatedToUser(mUser.UserId.Value, model.ServiceId, refs);

                    //si el servicio ya esta asociado, no tengo que crear uno nuevo
                    if (serviceAssociatedId != Guid.Empty)
                    {
                        return
                        Json(new JsonResponse(AjaxResponse.Error, "", "El usuario ya tiene asociado el servicio.", "¡Atención!", NotificationType.Error));
                    }
                }

                if (model.LoadRegistredData)
                {
                    model.Email_AppUser = mUser.Email_AppUser;
                }

                model.UserId = mUser.UserId;

                model.ReferenceNumber = mUser.ReferenceNumber;
                model.ReferenceNumber2 = mUser.ReferenceNumber2;
                model.ReferenceNumber3 = mUser.ReferenceNumber3;
                model.ReferenceNumber4 = mUser.ReferenceNumber4;
                model.ReferenceNumber5 = mUser.ReferenceNumber5;
                model.ReferenceNumber6 = mUser.ReferenceNumber6;

                model.CardId = mUser.CardId;

                model.IdentityNumber = mUser.IdentityNumber;
                model.CallCenterPin = mUser.CallCenterPin;
                model.PhoneNumber = mUser.PhoneNumber;
                model.MobileNumber = mUser.MobileNumber;
                model.Address = mUser.Address;
                model.Password = mUser.Password;

                Session[SessionConstants.HIGHWAY_ADMISSION] = model;

                bool newCard = model.CardId == null || model.CardId == Guid.Empty;

                BinDto bin;

                if (!newCard)
                {
                    bin = await _binClientService.FindByGuid(model.CardId.Value);
                }
                else
                {
                    bin = await _binClientService.Find(Convert.ToInt32(mUser.CardBin));
                }

                if (bin != null && !bin.Active)
                {
                    return
                       Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid,
                           PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                //si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = await _webServiceClientService.IsBinAssociatedToService(bin.Value, service.Id);
                if (!isBinAssociatedToService)
                {
                    return
                        Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid_For_Service,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                if (newCard)
                {
                    var currentUser = mUser.UserId != null ? await _webApplicationUserClientService.Find(mUser.UserId.Value) : new ApplicationUserDto()
                    {
                        Name = mUser.Name,
                        Surname = mUser.Surname,
                        Email = mUser.Email,
                        Id = Guid.Empty,
                        Address = mUser.Address
                    };

                    var cybersourceData = await LoadKeysForCybersource(service, RedirectEnums.HighwayAdmission, model.CardHolderName, currentUser);
                    var content = RenderPartialViewToString("_CybersourceKeys", cybersourceData);
                    return Json(new JsonResponse(AjaxResponse.Success, new { keys = content, }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Success, "", "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - ValidateCardTypeAndReferences - Excepcion");
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", "ERROR GENERAL", PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - ValidateCardTypeAndReferences - Excepcion");
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", "ERROR GENERAL", PresentationCoreMessages.NotificationFail,
                        NotificationType.Error));
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - ValidateCardTypeAndReferences - Excepcion");
                NLogLogger.LogEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, "", "ERROR GENERAL", PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(ServiceDto sDto, RedirectEnums redirectTo, string nameTh, ApplicationUserDto currentUser)
        {
            var keys = await _webCyberSourceAccessClientService.GenerateKeys(new KeysInfoForTokenRegisteredUser
            {
                UserId = currentUser.Id,
                TransactionReferenceNumber = Guid.NewGuid().ToString(),
                RedirectTo = redirectTo.ToString("D"),
                NameTh = nameTh,
                Platform = PaymentPlatformDto.Apps.ToString(),
            });

            return keys;
        }

        //CALLBACK DE CYBERSOURCE
        public async Task<ActionResult> TokengenerationCallBack()
        {
            var model = (HighwayAdmissionModel)Session[SessionConstants.HIGHWAY_ADMISSION];
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - INICIO METODO " + DateTime.Now.ToString("G"));


                if (model == null)
                {
                    NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - TokengenerationCallBack - MODELO NULLO");
                    ShowNotification("Se perdieron los datos del usuario", NotificationType.Error);
                    return null;
                }

                var email = model.UserId != Guid.Empty ? model.Email_AppUser : model.Email;

                var cyberSourceData = (CyberSourceDataDto)TempData["CyberSourceData"];
                var verifyByVisaData = (VerifyByVisaDataDto)TempData["VerifyByVisaData"];

                #region Registro Log

                var strLog = string.Format(LogStrings.Service_Highway_init, email, model.ServiceName);

                await _logClientService.Put(new LogModel
                {
                    LogType = LogType.Info,
                    LogUserType = LogUserType.NoRegistered,
                    LogCommunicationType = LogCommunicationType.VisaNet,
                    Message = strLog,
                    CallCenterMessage = strLog,
                    LogOperationType = LogOperationType.DebitPaymentBatch,
                    CyberSourceLogData = new CyberSourceLogDataDto
                    {
                        AuthAmount = cyberSourceData.AuthAmount,
                        AuthAvsCode = cyberSourceData.AuthAvsCode,
                        AuthCode = cyberSourceData.AuthCode,
                        AuthResponse = cyberSourceData.AuthResponse,
                        AuthTime = cyberSourceData.AuthTime,
                        AuthTransRefNo = cyberSourceData.AuthTransRefNo,
                        BillTransRefNo = cyberSourceData.BillTransRefNo,
                        Decision = cyberSourceData.Decision,
                        Message = cyberSourceData.Message,
                        PaymentToken = cyberSourceData.PaymentToken,
                        ReasonCode = cyberSourceData.ReasonCode,
                        ReqAmount = cyberSourceData.ReqAmount,
                        ReqCardExpiryDate = cyberSourceData.ReqCardExpiryDate,
                        ReqCardNumber = cyberSourceData.ReqCardNumber,
                        ReqCardType = cyberSourceData.ReqCardType,
                        ReqCurrency = cyberSourceData.ReqCurrency,
                        ReqPaymentMethod = cyberSourceData.ReqPaymentMethod,
                        ReqProfileId = cyberSourceData.ReqProfileId,
                        ReqReferenceNumber = cyberSourceData.ReqReferenceNumber,
                        ReqTransactionType = cyberSourceData.ReqTransactionType,
                        ReqTransactionUuid = cyberSourceData.ReqTransactionUuid,
                        TransactionId = cyberSourceData.TransactionId,
                        TransactionType = TransactionType.CardToken,
                        PaymentPlatform = PaymentPlatform.VisaNet
                    },
                    CyberSourceVerifyByVisaData = new CyberSourceVerifyByVisaDataDto
                    {
                        PayerAuthenticationXid = verifyByVisaData.PayerAuthenticationXid,
                        PayerAuthenticationProofXml = verifyByVisaData.PayerAuthenticationProofXml,
                        PayerAuthenticationCavv = verifyByVisaData.PayerAuthenticationCavv,
                        PayerAuthenticationEci = verifyByVisaData.PayerAuthenticationEci,
                    }
                });

                #endregion

                //Se evalua el resultado de la transacción
                var result = EvaluateErrors((CybersourceMsg)Convert.ToInt16(cyberSourceData.ReasonCode), model);
                if (result != null)
                    return result;

                ApplicationUserDto appUser = null;
                if (model.UserId != null && model.UserId != Guid.Empty)
                {
                    NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - Usuario registrado " + model.Email_AppUser);
                    appUser = await _webApplicationUserClientService.Find(model.UserId.Value);
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - Usuario nuevo " + model.Email);
                    appUser = new ApplicationUserDto()
                    {
                        Address = model.Address,
                        Email = model.Email,
                        IdentityNumber = model.IdentityNumber,
                        MobileNumber = model.MobileNumber,
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Surname = model.Surname,
                        CallCenterKey = model.CallCenterPin,
                    };
                }

                //CREO EL USUARIO
                if (model.UserId == null || model.UserId == Guid.Empty)
                {
                    await _registerUserClientService.Create(new ApplicationUserCreateEditDto()
                    {
                        Address = appUser.Address,
                        Email = appUser.Email,
                        IdentityNumber = appUser.IdentityNumber,
                        MobileNumber = appUser.MobileNumber,
                        Name = appUser.Name,
                        Surname = appUser.Surname,
                        PhoneNumber = appUser.PhoneNumber,
                        Password = model.Password,
                        CallCenterKey = appUser.CallCenterKey
                    });
                    var newUser = await _webApplicationUserClientService.Find(model.Email);
                    appUser.Id = newUser.Id;
                    NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - Usuario nuevo CREADO" + model.Email);
                }

                //TENGO Q CREAR LA NEVA TARJETA PARA ESTE DTO, LOS DATOS VIENE DE CYBERSOURCE
                var csExpiry = (String)TempData["CsExpiry"];
                var splitedExpiry = csExpiry.Split('-');
                var dueDate = new DateTime(Convert.ToInt16(splitedExpiry[1]), Convert.ToInt16(splitedExpiry[0]), 1);
                var cardDto = new CardDto()
                {
                    DueDate = dueDate,
                    MaskedNumber = (String)TempData["CsMask"],
                    PaymentToken = (String)TempData["CsToken"],
                    CybersourceTransactionId = TempData["CsTransaction"] == null ? "" : (String)TempData["CsTransaction"],
                    Active = true,
                    Name = TempData["CsMerchantDefinedData18"] != null ? (String)TempData["CsMerchantDefinedData18"] : "",
                };

                cardDto = await _webApplicationUserClientService.AddCard(appUser.Id, cardDto);

                NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - Tarjeta creada para usuario " + model.Email);

                var service = await _webServiceClientService.Find(model.ServiceId);
                var fullNotifications = service.ServiceGatewaysDto.Count > 1 || service.EnableAutomaticPayment || service.ServiceGatewaysDto.FirstOrDefault().Gateway.Enum != (int)GatewayEnum.Carretera;

                var dto = new ServiceAssociatedDto
                {
                    UserId = appUser.Id,
                    DefaultCardId = cardDto.Id,
                    ReferenceNumber = model.ReferenceNumber,
                    ReferenceNumber2 = model.ReferenceNumber2,
                    ReferenceNumber3 = model.ReferenceNumber3,
                    ReferenceNumber4 = model.ReferenceNumber4,
                    ReferenceNumber5 = model.ReferenceNumber5,
                    ReferenceNumber6 = model.ReferenceNumber6,
                    ServiceId = model.ServiceId,
                    Enabled = true,
                };

                var noticonf = new NotificationConfigDto()
                {
                    DaysBeforeDueDate = 5,
                    BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                    {
                        Email = fullNotifications,
                        Sms = false,
                        Web = false
                    },
                    ExpiredBillDto = new ExpiredBillDto()
                    {
                        Email = fullNotifications,
                        Sms = false,
                        Web = false
                    },
                    NewBillDto = new NewBillDto()
                    {
                        Email = fullNotifications,
                        Sms = false,
                        Web = false
                    },
                    FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                    {
                        Email = fullNotifications,
                        Sms = false,
                        Web = false
                    },
                    SuccessPaymentDto = new SuccessPaymentDto()
                    {
                        Email = true,
                        Sms = false,
                        Web = false
                    }
                };

                dto.NotificationConfigDto = noticonf;
                dto = await _webServiceAssosiateClientService.Create(dto);

                NLogLogger.LogEvent(NLogType.Info, "HighwayAdmissionController - TokengenerationCallBack - Servicio asosiado a usuario " + model.Email);

                ViewBag.Email = appUser.Email;
                ViewBag.ServiceName = model.ServiceName;
                ViewBag.SuccessfulAssociation = true;

                CleanSession();

                return View("Index", model);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - TokengenerationCallBack - Excepcion");
                NLogLogger.LogEvent(ex);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - TokengenerationCallBack - Excepcion");
                NLogLogger.LogEvent(ex);
            }
            catch (Exception ex)
            {
                ShowNotification("Error general", NotificationType.Error);
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - TokengenerationCallBack - Excepcion");
                NLogLogger.LogEvent(ex);
            }
            return View("Index", model);
        }

        private ActionResult EvaluateErrors(CybersourceMsg reasonCode, HighwayAdmissionModel model)
        {
            if (reasonCode != CybersourceMsg.Accepted)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - TokengenerationCallBack - Error en CS: " + reasonCode);

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
                        return InvalidCardData(reasonCode, model);

                    case CybersourceMsg.AVSCheckInvalid:
                    case CybersourceMsg.UserCybersourceError:
                    case CybersourceMsg.PayerAuthenticationNotAuthenticated:
                    case CybersourceMsg.AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings:
                    case CybersourceMsg.ConfigurationKeysInvalids:
                        return RedirectToAction("NotificationError");
                }
                return GetError(reasonCode, model);
            }
            return null;
        }
        private ActionResult InvalidCardData(CybersourceMsg reasonCode, HighwayAdmissionModel model)
        {
            switch (reasonCode)
            {
                case CybersourceMsg.InvalidFields:
                    ShowNotification(PresentationWebStrings.Card_InvalidFields, NotificationType.Info);
                    break;
                case CybersourceMsg.ExpiredCard:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.InsufficientFunds:
                    ShowNotification(PresentationWebStrings.Card_InsufficientFunds, NotificationType.Info);
                    break;
                case CybersourceMsg.StolenLostCard:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.CreditLimitReached:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidCVN:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.AccountFrozen:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidAccountNumber:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.CardTypeNotAccepted:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                case CybersourceMsg.CVNCheckInvalid:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
                default:
                    ShowNotification(PresentationWebStrings.Card_ExpiriedCard, NotificationType.Info);
                    break;
            }
            SaveErrorAudit(reasonCode, model);
            model.LoadRegistredData = model.UserId != null && model.UserId != Guid.Empty;
            model.LoadNewData = model.UserId == Guid.Empty || model.UserId == null;
            return View("Index", model);
        }
        private ActionResult GetError(CybersourceMsg reasonCode, HighwayAdmissionModel model)
        {
            SaveErrorAudit(reasonCode, model);
            ShowNotification("ERROR GENERAL. INTENTAR NUEVAMENTE CON OTRA TARJETA", NotificationType.Error);
            model.LoadRegistredData = model.UserId != null && model.UserId != Guid.Empty;
            model.LoadNewData = model.UserId == Guid.Empty || model.UserId == null;
            return View("Index", model);
        }
        private void SaveErrorAudit(CybersourceMsg reasonCode, HighwayAdmissionModel model)
        {
            NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - GetError - Reason code : " + (int)reasonCode);
            var strLog = string.Format(LogStrings.Payment_cybersourceCallback_error,
                reasonCode + " " + (int)reasonCode,
                model.UserId == Guid.Empty ? model.Email : model.Email_AppUser,
                model.UserId == Guid.Empty ? model.Name : "",
                model.UserId == Guid.Empty ? model.Surname : "",
                model.ServiceName, "");

            _logClientService.Put(new LogModel
            {
                LogType = LogType.Error,
                LogUserType = LogUserType.NoRegistered,
                LogCommunicationType = LogCommunicationType.VisaNet,
                Message = strLog,
                LogOperationType = LogOperationType.DebitPaymentBatch
            });
        }


        public async Task<ActionResult> ConditionsVisa()
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var title = "Error";
            var message = "Error al cargar Términos y Condiciones de VisaNetPagos.";

            try
            {
                var page = await _pageClientService.FindType(PageTypeDto.LegalPages);

                var viewModel = new HighwayAdmissionModel { TermsAndConditionsService = page.Content };

                var content = RenderPartialViewToString("_TermsAndConditionsVisa", viewModel);

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                title = "Correcto";
                message = "Correcto";

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - ConditionsVisa - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ConditionsService()
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var title = "Error";
            var message = "Error al cargar Términos y Condiciones del servicio.";

            try
            {
                var model = (HighwayAdmissionModel)Session[SessionConstants.HIGHWAY_ADMISSION];

                var termsLines = model.TermsAndConditionsService.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                var viewModel = new HighwayAdmissionModel
                {
                    ServiceName = model.ServiceName,
                    TermsAndConditionsServiceLines = termsLines
                };

                var content = RenderPartialViewToString("_TermsAndConditionsService", viewModel);

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                title = "Correcto";
                message = "Correcto";

                return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "HighwayAdmissionController - ConditionsService - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            return Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);
        }

    }
}