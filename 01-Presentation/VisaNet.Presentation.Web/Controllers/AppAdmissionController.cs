using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
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
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Enums;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Controllers
{
    public class AppAdmissionController : BaseController
    {
        private readonly IWebApplicationUserClientService _webUserService;
        private readonly IWebRegisterUserClientService _registerUserClientService;
        private readonly IWebBinClientService _binClientService;
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebLogClientService _logClientService;
        private readonly IWebWebhookLogClientService _webWebhookLogClientService;
        private readonly IWebPageClientService _pageClientService;
        private readonly IWebCyberSourceAccessClientService _webCyberSourceAccessClientService;

        public AppAdmissionController(IWebApplicationUserClientService userService,
            IWebRegisterUserClientService registerUserClientService, IWebBinClientService binClientService,
            IWebServiceClientService webServiceClientService,
            IWebServiceAssosiateClientService webServiceAssosiateClientService,
            IWebApplicationUserClientService webApplicationUserClientService, IWebLogClientService logClientService,
            IWebWebhookLogClientService webWebhookLogClientService, IWebPageClientService pageClientService,
            IWebCyberSourceAccessClientService webCyberSourceAccessClientService)
        {
            _webUserService = userService;
            _registerUserClientService = registerUserClientService;
            _binClientService = binClientService;
            _webServiceClientService = webServiceClientService;
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _webApplicationUserClientService = webApplicationUserClientService;
            _logClientService = logClientService;
            _webWebhookLogClientService = webWebhookLogClientService;
            _pageClientService = pageClientService;
            _webCyberSourceAccessClientService = webCyberSourceAccessClientService;
        }

        [HttpPost]
        public async Task<ActionResult> Index(string id)
        {
            var idOperation = "";
            var email = "";
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index POST id: " + id));
                email = Request.Form["Email"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index email: " + email));
                var name = Request.Form["Nombre"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index name: " + name));
                var surname = Request.Form["Apellido"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index surname: " + surname));
                var address = Request.Form["Direccion"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index address: " + address));
                var phone = Request.Form["Telefono"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index phone: " + phone));
                var mobile = Request.Form["Movil"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index mobile: " + mobile));
                var identity = Request.Form["CI"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index identity: " + identity));
                var allowsNewEmail = Request.Form["PermiteCambioEmail"] ?? "S"; //por defecto es S
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index allowsNewEmail: " + allowsNewEmail));
                idOperation = Request.Form["IdOperacion"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index idOperation: " + idOperation));
                var urlCallback = Request.Form["UrlCallback"];
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index urlCallback: " + urlCallback));
                var registeredUser = string.IsNullOrEmpty(Request.Form["RegisteredUser"])
                    ? false
                    : (!Request.Form["RegisteredUser"].Equals("true") ? false : true);



                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Index registeredUser: " + registeredUser));

                SaveBrowserData(idOperation, id);

                NLogLogger.LogAppsEvent(NLogType.Info,
                    string.Format(
                        "AppAdmissionController - POST - Id: {0}, Id Operation: {1}, Email :{2} , Nombre Completo: {3}, CI: {4}",
                        id, idOperation, email, name + " " + surname, identity));

                var webHook = new WebhookRegistrationDto
                              {
                                  IdOperation = idOperation,
                                  UrlCallback = urlCallback,
                                  EnableEmailChange =
                                      allowsNewEmail.Equals("N", StringComparison.InvariantCultureIgnoreCase)
                                          ? "N"
                                          : "S",
                                  IdApp = id,
                                  UserData = new UserDataInputDto
                                             {
                                                 Name = name,
                                                 Surname = surname,
                                                 Email = email,
                                                 Address = address,
                                                 IdentityNumber = identity,
                                                 PhoneNumber = phone,
                                                 MobileNumber = mobile
                                             }
                              };

                var idOperationrepited = await _webWebhookLogClientService.WebhookRegistrationIsIdOperationRepited(
                    idOperation, id);

                if (idOperationrepited)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error, string.Format("Id Operación {0} repetido. Id app {1}.", idOperation, id));
                    ViewBag.ResultCode = "11";
                    ViewBag.ResultDescription = "Id Operación repetido.";
                    return View("End", new AppAdmissionModel
                    {
                        UrlCallback = webHook.UrlCallback
                    });
                }

                try
                {
                    _webWebhookLogClientService.CreateWebhookRegistration(webHook);
                }
                catch (WebApiClientBusinessException exception)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error, "Exception CreateWebhookRegistration");
                    NLogLogger.LogAppsEvent(exception);
                    ViewBag.ResultCode = "11";
                    ViewBag.ResultDescription = "Id Operación repetido.";
                    return View("End", new AppAdmissionModel
                    {
                        UrlCallback = webHook.UrlCallback
                    });

                }

                if (!IsRequired(webHook))
                {
                    if (!string.IsNullOrEmpty(webHook.UrlCallback))
                    {
                        ViewBag.ResultCode = "3"; //3: Campo obligatorio no enviado
                        ViewBag.ResultDescription = "Falta campo IdOperacion.";

                        return View("End", new AppAdmissionModel
                                           {
                                               UrlCallback = webHook.UrlCallback
                                           });
                    }

                    return View("Error");
                }

                var model =
                    await
                        LoadData(id, email, name, surname, address, phone, mobile, identity, allowsNewEmail, idOperation,
                            urlCallback);

                if (model == null)
                {
                    ViewBag.ResultCode = "2"; //2: Errores en los campos enviados (¿usar otro codigo?)
                    ViewBag.ResultDescription = "No se pudo cargar el formulario.";
                    NLogLogger.LogAppsEvent(NLogType.Info,
                        string.Format(
                            "AppAdmissionController - FIN POST ERROR Id: {0}, Id Operation: {1}, Email :{2} ", id,
                            idOperation, email));
                    return View("End", new AppAdmissionModel
                                       {
                                           UrlCallback = webHook.UrlCallback
                                       });
                }

                if (registeredUser)
                    model.RegisteredEmail = registeredUser;

                NLogLogger.LogAppsEvent(NLogType.Info,
                    string.Format("AppAdmissionController - FIN POST Id: {0}, Id Operation: {1}, Email :{2} ", id,
                        idOperation, email));
                Session[SessionConstants.APP_ADMISSION] = model;
                return View(model);

            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmission - Index - Ha ocurrido una excepcion.");
                NLogLogger.LogAppsEvent(NLogType.Info,
                    string.Format(
                        "AppAdmissionController - FIN POST EXCEPTION Id: {0}, Id Operation: {1}, Email :{2} ", id,
                        idOperation, email));
                NLogLogger.LogAppsEvent(e);
                return View("Error");
            }
        }

        public bool IsRequired(WebhookRegistrationDto dto)
        {
            if (string.IsNullOrEmpty(dto.UrlCallback)) return false;
            if (string.IsNullOrEmpty(dto.IdOperation)) return false;
            return true;
        }

        public async Task<AppAdmissionModel> LoadData(string id, string email, string name, string surname,
            string address, string phone, string mobile, string identity, string allowsNewEmail, string operationId,
            string urlCallaback)
        {
            var serviceDto = await _webServiceClientService.GetServiceByUrlName(id);
            if (serviceDto != null)
            {
                var model = new AppAdmissionModel()
                            {
                                ServiceId = serviceDto.Id,
                                ServiceName = serviceDto.Name,
                                ServiceUrlName = id,
                                SuccessFulAssociation = false,
                                ServiceImage = serviceDto.ImageUrl,
                                PostAssociationDesc = serviceDto.PostAssociationDesc,
                                TermsAndConditionsService = serviceDto.TermsAndConditions,
                                AskReferences = serviceDto.AskUserForReferences,

                                LoadNewData = false,
                                LoadRegistredData = false,

                                Email = email,
                                Name = name,
                                Surname = surname,
                                Address = address,
                                PhoneNumber = phone,
                                MobileNumber = mobile,
                                IdentityNumber = identity,
                                AllowsNewEmail = !allowsNewEmail.Equals("N", StringComparison.InvariantCultureIgnoreCase),
                                IdOperation = operationId,
                                UrlCallback = urlCallaback,
                            };

                if (serviceDto.AskUserForReferences)
                {
                    model.ReferenceName = serviceDto.ReferenceParamName;
                    model.ReferenceRegex = serviceDto.ReferenceParamRegex;
                    model.ReferenceName2 = serviceDto.ReferenceParamName2;
                    model.ReferenceRegex2 = serviceDto.ReferenceParamRegex2;
                    model.ReferenceName3 = serviceDto.ReferenceParamName3;
                    model.ReferenceRegex3 = serviceDto.ReferenceParamRegex3;
                    model.ReferenceName4 = serviceDto.ReferenceParamName4;
                    model.ReferenceRegex4 = serviceDto.ReferenceParamRegex4;
                    model.ReferenceName5 = serviceDto.ReferenceParamName5;
                    model.ReferenceRegex5 = serviceDto.ReferenceParamRegex5;
                    model.ReferenceName6 = serviceDto.ReferenceParamName6;
                    model.ReferenceRegex6 = serviceDto.ReferenceParamRegex6;
                }

                //NLogLogger.LogAppsEvent(NLogType.Info, model.ServiceName);

                if (!string.IsNullOrEmpty(model.PostAssociationDesc))
                {
                    var postAsocLines = model.PostAssociationDesc.Split(new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None);
                    model.PostAssociationDescLines = postAsocLines;
                }

                if (!string.IsNullOrEmpty(model.TermsAndConditionsService))
                {
                    var serviceTermsLines = model.TermsAndConditionsService.Split(new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None);
                    model.TermsAndConditionsServiceLines = serviceTermsLines;
                }

                try
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        var exists = await _webApplicationUserClientService.Find(model.Email);
                        if (exists != null)
                        {
                            //model.Email_AppUser = model.Email;
                            model.RegisteredEmail = true;
                        }
                    }
                    else
                    {
                        model.LoadNewData = true;
                    }
                }
                catch (WebApiClientBusinessException e)
                {
                    model.RegisteredEmail = false;
                    model.LoadNewData = true;
                }
                catch (Exception e)
                {
                    NLogLogger.LogAppsEvent(e);
                    model.RegisteredEmail = false;
                    model.LoadNewData = true;
                }
                return model;
            }
            return null;
        }

        private void CleanSession()
        {
            Session[SessionConstants.APP_ADMISSION] = null;
            Session[SessionConstants.CURRENT_SELECTED_USER] = null;
            //Session[SessionConstants.CURRENT_USER_ID] = null;
            //Session[SessionConstants.CURRENT_USER_TYPE] = null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetMyPassword(AppAdmissionModel model)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = PresentationCoreMessages.NotificationFail;
            var title = "Error";

            if (String.IsNullOrEmpty(model.Email))
            {
                message = "Debés ingresar tu correo electrónico.";
                return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(model.ServiceUrlName))
            {
                return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
            }

            try
            {
                var result = await _webUserService.ResetPassword(model.Email);
                if (result == 1)
                {
                    response = AjaxResponse.Success;
                    return Json(new JsonResponse(response, "", message, title, notification),
                        JsonRequestBehavior.AllowGet);
                }
                if (result == 2) //actualmente siempre devuelve 1
                {
                    response = AjaxResponse.Success;
                    return Json(new JsonResponse(response, "", message, title, notification),
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (WebApiClientBusinessException)
            {
                response = AjaxResponse.Success;
                return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientFatalException)
            {
                response = AjaxResponse.Success;
                return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(exception);
                NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmission - ForgetMyPassword - Ha ocurrido una excepcion.");
                return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ForgetMyPasswordAjax()
        {
            try
            {
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - ForgetMyPasswordAjax. IdApp: {0}, Email: {1}, Operacion {2}", model.ServiceUrlName, model.Email, model.IdOperation));

                var content = RenderPartialViewToString("_ForgetMyPassword", model);
                return
                    Json(new JsonResponse(AjaxResponse.Success, content, String.Empty, String.Empty,
                        NotificationType.Success));
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(e);
                NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmission - ForgetMyPasswordAjax - Ha ocurrido una excepcion");
                return
                    Json(new JsonResponse(AjaxResponse.Error, null, String.Empty, "Ha ocurrido un error",
                        NotificationType.Error));
            }
        }

        public async Task<ActionResult> LoadUserAjax(string userName, string password)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = PresentationCoreMessages.Security_UserNameOrPasswordNotValid;
            var title = "Error";
            string content;

            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmissionController - LoadUserAjax - Valido usuario " + userName);

                if (await _webUserService.ValidateUser(new ValidateUserDto { UserName = userName, Password = password }))
                {
                    var user = await _webUserService.Find(userName);
                    var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];
                    if (model == null)
                    {
                        NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmissionController - LoadUserAjax - MODELO NULLO !?");
                        return
                            Json(new JsonResponse(AjaxResponse.Error, "", "Error en página. ",
                                PresentationCoreMessages.NotificationFail, NotificationType.Error));
                    }

                    NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - LoadUserAjax. IdApp: {0}, Email: {1}, Operacion {2}",
                        model.ServiceUrlName, string.IsNullOrEmpty(model.Email) ? userName : model.Email, model.IdOperation));

                    var serviceDto = await _webServiceClientService.Find(model.ServiceId);

                    if (user != null)
                    {
                        model.UserId = user.Id;
                        model.User = user;
                    }

                    title = "Ingreso correcto";

                    model.UserId = user.Id;
                    model.Name = user.Name;
                    model.Surname = user.Surname;
                    model.Email = user.Email;

                    model.Address = user.Address;
                    model.MobileNumber = user.MobileNumber;
                    model.PhoneNumber = user.PhoneNumber;
                    model.IdentityNumber = user.IdentityNumber;

                    model.LoadRegistredData = true;
                    model.LoadNewData = false;

                    if (serviceDto.AskUserForReferences)
                    {
                        model.ReferenceName = serviceDto.ReferenceParamName;
                        model.ReferenceRegex = serviceDto.ReferenceParamRegex;
                        model.ReferenceName2 = serviceDto.ReferenceParamName2;
                        model.ReferenceRegex2 = serviceDto.ReferenceParamRegex2;
                        model.ReferenceName3 = serviceDto.ReferenceParamName3;
                        model.ReferenceRegex3 = serviceDto.ReferenceParamRegex3;
                        model.ReferenceName4 = serviceDto.ReferenceParamName4;
                        model.ReferenceRegex4 = serviceDto.ReferenceParamRegex4;
                        model.ReferenceName5 = serviceDto.ReferenceParamName5;
                        model.ReferenceRegex5 = serviceDto.ReferenceParamRegex5;
                        model.ReferenceName6 = serviceDto.ReferenceParamName6;
                        model.ReferenceRegex6 = serviceDto.ReferenceParamRegex6;
                    }
                    else
                    {
                        Session[SessionConstants.APP_ADMISSION] = model;
                        return await LoadUserCards(model);

                    }

                    Session[SessionConstants.APP_ADMISSION] = model;

                    var referencesView = model.AskReferences
                        ? RenderPartialViewToString("_References", model)
                        : string.Empty;

                    response = AjaxResponse.Success;
                    notification = NotificationType.Success;

                    return Json(new JsonResponse(response,
                        new
                        {
                            userId = user.Id,
                            userName = user.Name,
                            userSurName = user.Surname,
                            userEmail = user.Email,
                            allowReturn = model.AllowsNewEmail ? "true" : "false",
                            referencesView = referencesView
                        },
                        message, title, notification), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                message = "No se pudo cargar el usuario. Intente nuevamente o comuniquese con nuestro Call Center";
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - LoadUserAjax - Excepcion");
                NLogLogger.LogAppsEvent(exception);
            }
            content = RenderPartialViewToString("_CardList", null);

            return Json(new JsonResponse(response, content, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> LoadUserCards(AppAdmissionModel modelView)
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = PresentationCoreMessages.AppsError_Description;
            var title = "Error"; //: "Ingreso correcto";
            string content;

            try
            {
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - LoadUserCards. IdApp: {0}, Email: {1}, Operacion {2}",
                    model.ServiceUrlName, string.IsNullOrEmpty(model.Email) ? modelView.Email : model.Email, model.IdOperation));

                CheckReferences(modelView);
                if (model.UserId != Guid.Empty)
                {
                    CheckUserDataOnRegistered();
                }
                if (!ModelState.IsValid)
                {
                    var userdata = (model.UserId == null || model.UserId == Guid.Empty)
                        ? RenderPartialViewToString("_NewUserData", modelView)
                        : string.Empty;
                    var referencesView = RenderPartialViewToString("_References", modelView);
                    message = PresentationCoreMessages.AppsError_Validation;
                    return Json(new JsonResponse(response,
                        new
                        {
                            userdata = userdata,
                            referencesView = referencesView,
                        },
                        message, title, notification), JsonRequestBehavior.AllowGet);
                }

                if (model.UserId != null && model.UserId != Guid.Empty)
                {
                    NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - LoadUserCards: Tengo un userId. IdApp: {0}, Email: {1}, Operacion {2}",
                            model.ServiceUrlName, model.Email, model.IdOperation));

                    if (model.User != null)
                    {
                        var cardList = new List<HighwayCardModel>();
                        message = "Agregue una nueva tarjeta para asociar.";

                        if (model == null)
                        {
                            NLogLogger.LogAppsEvent(NLogType.Info,
                                "AppAdmissionController - LoadUserAjax - MODELO NULLO !?");
                            return
                                Json(new JsonResponse(AjaxResponse.Error, "", "Error en página. ",
                                    PresentationCoreMessages.NotificationFail, NotificationType.Error));
                        }

                        var refs = new string[6];
                        if (model.AskReferences)
                        {
                            refs[0] = modelView.ReferenceValue;
                            refs[1] = modelView.ReferenceValue2;
                            refs[2] = modelView.ReferenceValue3;
                            refs[3] = modelView.ReferenceValue4;
                            refs[4] = modelView.ReferenceValue5;
                            refs[5] = modelView.ReferenceValue6;
                        }

                        var serviceAsociated =
                            await
                                _webServiceAssosiateClientService.ServiceAssosiatedToUser(model.User.Id, model.ServiceId,
                                    refs);

                        if (model.User.CardDtos != null && model.User.CardDtos.Count(x => x.Active) > 0)
                        {
                            cardList.AddRange(from dto in model.User.CardDtos
                                              where dto.Active
                                              select CardConverter(dto, serviceAsociated));
                            message = "Seleccione una tarjeta existente, o agregue una nueva.";
                        }

                        model.LoadRegistredData = true;
                        model.LoadNewData = false;
                        model.AssociatedCards = cardList;

                        model.ReferenceValue = modelView.ReferenceValue;
                        model.ReferenceValue2 = modelView.ReferenceValue2;
                        model.ReferenceValue3 = modelView.ReferenceValue3;
                        model.ReferenceValue4 = modelView.ReferenceValue4;
                        model.ReferenceValue5 = modelView.ReferenceValue5;
                        model.ReferenceValue6 = modelView.ReferenceValue6;

                        Session[SessionConstants.APP_ADMISSION] = model;

                        content = RenderPartialViewToString("_CardList", model);

                        response = AjaxResponse.Success;
                        notification = NotificationType.Success;

                        if (model.UserId != null && model.UserId != Guid.Empty)
                        {
                            title = "Ingreso correcto";
                        }

                        return Json(new JsonResponse(response,
                            new
                            {
                                cardListContent = content,
                                allowReturn = model.AllowsNewEmail ? "true" : "false",
                            },
                            message, title, notification), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - LoadUserCards: No tengo userId, cargo referencias solamente. IdApp: {0}, Email: {1}, Operacion {2}",
                            model.ServiceUrlName, model.Email, model.IdOperation));

                    var referencesView = RenderPartialViewToString("_References", modelView);
                    try
                    {
                        var user = await _webApplicationUserClientService.Find(modelView.Email);
                        if (user != null)
                        {
                            return
                                Json(new JsonResponse(AjaxResponse.Error, referencesView,
                                    ExceptionMessages.USER_EMAIL_DUPLICATED,
                                    PresentationCoreMessages.NotificationFail, NotificationType.Error));
                        }
                    }
                    catch (WebApiClientBusinessException ex)
                    {
                        if (!ex.Message.Equals("El usuario indicado no existe."))
                        {
                            NLogLogger.LogAppsEvent(ex);
                        }
                    }
                    catch (Exception exception)
                    {
                        NLogLogger.LogAppsEvent(exception);
                    }


                    model.Name = modelView.Name;
                    model.Surname = modelView.Surname;
                    model.Email = modelView.Email;

                    model.Address = modelView.Address;
                    model.MobileNumber = modelView.MobileNumber;
                    model.PhoneNumber = modelView.PhoneNumber;
                    model.IdentityNumber = modelView.IdentityNumber;

                    model.Password = modelView.Password;

                    Session[SessionConstants.APP_ADMISSION] = model;
                }
            }
            catch (Exception exception)
            {
                message = "No se pudo cargar las tarjetas. Intente nuevamente o comuniquese con nuestro Call Center";
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - LoadUserAjax - Excepcion");
                NLogLogger.LogAppsEvent(exception);
            }

            response = AjaxResponse.Success;
            notification = NotificationType.Success;
            return Json(new JsonResponse(response,
                new
                {
                    cardListContent = RenderPartialViewToString("_CardList", modelView),
                    allowReturn = modelView.AllowsNewEmail ? "true" : "false",
                },
                message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> NewUserAjax()
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "Se ha producido un error.";
            var title = "Error";

            try
            {
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];
                if (model == null)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error,
                        "AppAdmissionController - NewUserAjax - Modelo de sesión nulo");
                    return Json(new JsonResponse(response, "", message, title, notification),
                        JsonRequestBehavior.AllowGet);
                }

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - NewUserAjax. IdApp: {0}, Email: {1}, Operacion {2}",
                            model.ServiceUrlName, model.Email, model.IdOperation));

                model.LoadNewData = true;
                model.LoadRegistredData = false;

                model.UserId = Guid.Empty;
                model.AssociatedCards = null;

                if (model.AskReferences)
                {
                    var serviceDto = await _webServiceClientService.Find(model.ServiceId);
                    model.ReferenceName = serviceDto.ReferenceParamName;
                    model.ReferenceRegex = serviceDto.ReferenceParamRegex;
                    model.ReferenceName2 = serviceDto.ReferenceParamName2;
                    model.ReferenceRegex2 = serviceDto.ReferenceParamRegex2;
                    model.ReferenceName3 = serviceDto.ReferenceParamName3;
                    model.ReferenceRegex3 = serviceDto.ReferenceParamRegex3;
                    model.ReferenceName4 = serviceDto.ReferenceParamName4;
                    model.ReferenceRegex4 = serviceDto.ReferenceParamRegex4;
                    model.ReferenceName5 = serviceDto.ReferenceParamName5;
                    model.ReferenceRegex5 = serviceDto.ReferenceParamRegex5;
                    model.ReferenceName6 = serviceDto.ReferenceParamName6;
                    model.ReferenceRegex6 = serviceDto.ReferenceParamRegex6;
                }

                Session[SessionConstants.APP_ADMISSION] = model;
                var referencesView = model.AskReferences
                    ? RenderPartialViewToString("_References", model)
                    : string.Empty;

                Session[SessionConstants.APP_ADMISSION] = model;

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                message = "Correcto";
                title = "Correcto";

                return Json(new JsonResponse(response,
                    new
                    {
                        referencesView = referencesView
                    },
                    message, title, notification), JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - NewUserAjax - Excepcion");
                NLogLogger.LogAppsEvent(exception);
            }
            return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReturnToLoginAjax()
        {
            var response = AjaxResponse.Error;
            var notification = NotificationType.Error;
            var message = "Se ha producido un error.";
            var title = "Error";

            try
            {
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];
                if (model == null)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error,
                        "AppAdmissionController - ReturnToLoginAjax - Modelo de sesión nulo");
                    return Json(new JsonResponse(response, "", message, title, notification),
                        JsonRequestBehavior.AllowGet);
                }

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - ReturnToLoginAjax. IdApp: {0}, Email: {1}, Operacion {2}",
                            model.ServiceUrlName, model.Email, model.IdOperation));

                model.LoadNewData = false;
                model.LoadRegistredData = false;

                if (model.UserId != Guid.Empty)
                {
                    model.UserId = Guid.Empty;
                    model.AssociatedCards = null;

                    Session[SessionConstants.CURRENT_SELECTED_USER] = null;
                    //Session[SessionConstants.CURRENT_USER_ID] = null;
                    //Session[SessionConstants.CURRENT_USER_TYPE] = null;
                }

                var initialModel = await _webWebhookLogClientService.GetwebHookRegistrationsByIdOperation(model.IdOperation, model.ServiceId);

                model.Email = initialModel.UserData.Email;
                model.Name = initialModel.UserData.Name;
                model.Surname = initialModel.UserData.Surname;
                model.Address = initialModel.UserData.Address;
                model.IdentityNumber = initialModel.UserData.IdentityNumber;
                model.PhoneNumber = initialModel.UserData.PhoneNumber;
                model.MobileNumber = initialModel.UserData.MobileNumber;

                Session[SessionConstants.APP_ADMISSION] = model;

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                message = "Correcto";
                title = "Correcto";

                //debo recargar la vista de las tarjetas
                var content = RenderPartialViewToString("_CardList", null);

                return Json(new JsonResponse(response, content, message, title, notification),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - NewUserAjax - Excepcion");
                NLogLogger.LogAppsEvent(exception);
            }
            return Json(new JsonResponse(response, "", message, title, notification), JsonRequestBehavior.AllowGet);
        }

        private static HighwayCardModel CardConverter(CardDto cardDto, ServiceAssociatedDto service)
        {
            if (cardDto != null)
            {
                var cardUsed = service != null && service.Active &&
                               service.CardDtos.Any(x => x.Id == cardDto.Id && x.Active);
                var card = new HighwayCardModel
                           {
                               Id = cardDto.Id,
                               Active = cardDto.Active,
                               DueDate = cardDto.DueDate,
                               Mask = cardDto.MaskedNumber,
                               AlreadyIn = cardUsed,
                           };
                return card;
            }
            return null;
        }

        //private async Task<ApplicationUserDto> LoadUserWithCards(string userName)
        //{
        //    try
        //    {
        //        var user = await _userService.Find(userName);
        //        Session[SessionConstants.CURRENT_SELECTED_USER] = user;
        //        //Session[SessionConstants.CURRENT_USER_ID] = user.Id;
        //        //Session[SessionConstants.CURRENT_USER_TYPE] = CurrentUserType.Public;

        //        return user;
        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - LoadUserWithCards - Excepcion");
        //        NLogLogger.LogAppsEvent(exception);
        //    }
        //    return null;
        //}

        [HttpPost]
        public async Task<ActionResult> ValidateCardType(AppAdmissionModel mUser)
        {
            var content = RenderPartialViewToString("_NewUserData", mUser);
            try
            {
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - ValidateCardType. IdApp: {0}, Email: {1}, Operacion {2}",
                           model.ServiceUrlName, model.Email, model.IdOperation));

                if (model == null)
                {
                    NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmissionController - ValidateCardType - MODELO NULLO");
                    return
                        Json(new JsonResponse(AjaxResponse.Error, content,
                            "No hemos podido procesar su información. Por favor cierre la ventana e inicie el proceso nuevamente.",
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                var service = await _webServiceClientService.Find(model.ServiceId);

                model.Email = mUser.Email;
                model.UserId = mUser.UserId;
                model.CardId = mUser.CardId;
                model.CardHolderName = mUser.CardHolderName;
                model.ReferenceValue = mUser.ReferenceValue;
                model.ReferenceValue2 = mUser.ReferenceValue2;
                model.ReferenceValue3 = mUser.ReferenceValue3;
                model.ReferenceValue4 = mUser.ReferenceValue4;
                model.ReferenceValue5 = mUser.ReferenceValue5;
                model.ReferenceValue6 = mUser.ReferenceValue6;

                Session[SessionConstants.APP_ADMISSION] = model;

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
                       Json(new JsonResponse(AjaxResponse.Error, content, PresentationWebStrings.Bin_Not_Valid,
                           PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                //si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = await _webServiceClientService.IsBinAssociatedToService(bin.Value, service.Id);
                if (!isBinAssociatedToService)
                {
                    return
                        Json(new JsonResponse(AjaxResponse.Error, content, PresentationWebStrings.Bin_Not_Valid_For_Service,
                            PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                if (newCard)
                {
                    ApplicationUserDto user = null;
                    //Devuelvo el usuario o creo uno nuevo si es necesario.
                    if (model.User != null || model.UserId != null && model.UserId != Guid.Empty)
                    {
                        NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - ValidateCardType: Usuario registrado. IdApp: {0}, Email: {1}, Operacion {2}",
                           model.ServiceUrlName, model.Email, model.IdOperation));
                        user =
                            await
                                _webApplicationUserClientService.Find(model.UserId.HasValue
                                    ? model.UserId.Value
                                    : model.User.Id);

                        model.User = user;
                        model.UserId = user.Id;
                        Session[SessionConstants.APP_ADMISSION] = model;
                    }
                    var cybersourceData = await LoadKeysForCybersource(RedirectEnums.AppAdmission, model.CardHolderName, model);
                    var contentKey = RenderPartialViewToString("_CybersourceKeys", cybersourceData);

                    return Json(new JsonResponse(AjaxResponse.Success, new { keys = contentKey }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(AjaxResponse.Success, "", "", "", NotificationType.Success),
                    JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - ValidateCardType - Excepcion");
                NLogLogger.LogAppsEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, content,
                        "No hemos podido validar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.",
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - ValidateCardType - Excepcion");
                NLogLogger.LogAppsEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, content,
                        "No hemos podido validar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.",
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - ValidateCardType - Excepcion");
                NLogLogger.LogAppsEvent(exception);
                return
                    Json(new JsonResponse(AjaxResponse.Error, content,
                        "No hemos podido validar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.",
                        PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Associate(AppAdmissionModel modelView)
        {
            //var idOp = Request.Form["IdOperation"];
            var resultCode = "1";
            var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Associate. IdApp: {0}, Email: {1}, Operacion {2}",
                           model.ServiceUrlName, model.Email, model.IdOperation));
            try
            {
                if (model != null && model.User != null && model.CardId != null && model.CardId != Guid.Empty)
                {
                    var dto = new ServiceAssociatedDto()
                              {
                                  UserId = model.User.Id,
                                  ServiceId = model.ServiceId,
                                  DefaultCardId = model.CardId.Value,
                                  ReferenceNumber = modelView.ReferenceValue,
                                  ReferenceNumber2 = modelView.ReferenceValue2,
                                  ReferenceNumber3 = modelView.ReferenceValue3,
                                  ReferenceNumber4 = modelView.ReferenceValue4,
                                  ReferenceNumber5 = modelView.ReferenceValue5,
                                  ReferenceNumber6 = modelView.ReferenceValue6,
                                  OperationId = model.IdOperation,
                                  Enabled = true,
                                  Active = true,
                              };
                    var result = await _webServiceAssosiateClientService.AssociateServiceToUserFromCardCreated(dto);
                    if (result != null)
                    {
                        resultCode = "0";
                        ViewBag.ResultDescription = "";
                    }
                }
                else
                {
                    resultCode = "1";
                    ViewBag.ResultDescription = "Ha ocurrido un error.";
                }
                CleanSession();

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Associate. IdApp: {0}, Email: {1}, Operacion {2}. ResultCode: {3}",
                          model.ServiceUrlName, model.Email, model.IdOperation, resultCode));

                ViewBag.ResultCode = resultCode;
                return View("End", model);
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(e);
            }

            resultCode = "1";
            ViewBag.ResultDescription = "Ha ocurrido un error.";
            ViewBag.ResultCode = resultCode;
            return View("End", model);
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(RedirectEnums redirectTo, string nameTh, AppAdmissionModel model)
        {
            var modelSession = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];

            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Load Keys: Inicio datos. IdApp: {0}, Email: {1}, Operacion {2}",
                           modelSession.ServiceUrlName, modelSession.Email, modelSession.IdOperation));

            KeysInfoForToken appToken;
            if (!modelSession.UserId.HasValue)
            {
                appToken = new KeysInfoForTokenNewUser
                {
                    UserId = Guid.Empty,
                    Name = modelSession.Name,
                    Surname = modelSession.Surname,
                    Address = modelSession.Address,
                    Email = modelSession.Email,
                    PhoneNumber = modelSession.PhoneNumber,
                    MobileNumber = modelSession.MobileNumber,
                    IdentityNumber = modelSession.IdentityNumber,
                    Password = HashPassword(modelSession.Password)
                };
            }
            else
            {
                appToken = new KeysInfoForTokenRegisteredUser
                {
                    UserId = modelSession.UserId.Value
                };
            }

            appToken.TransactionReferenceNumber = Guid.NewGuid().ToString();
            appToken.RedirectTo = redirectTo.ToString("D");
            appToken.NameTh = nameTh;
            appToken.OperationId = model.IdOperation;
            appToken.Platform = PaymentPlatformDto.Apps.ToString();
            appToken.PaymentTypeDto = PaymentTypeDto.App;
            appToken.UrlReturn = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            appToken.ServiceId = modelSession.ServiceId;
            appToken.ReferenceNumber1 = modelSession.ReferenceValue;
            appToken.ReferenceNumber2 = modelSession.ReferenceValue2;
            appToken.ReferenceNumber3 = modelSession.ReferenceValue3;
            appToken.ReferenceNumber4 = modelSession.ReferenceValue4;
            appToken.ReferenceNumber5 = modelSession.ReferenceValue5;
            appToken.ReferenceNumber6 = modelSession.ReferenceValue6;

            var keys = await _webCyberSourceAccessClientService.GenerateKeys(appToken);
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Load Keys: Devuelvo datos. Id Operation: {0}", modelSession != null ? modelSession.IdOperation : ""));
            return keys;
        }

        //CALLBACK DE CYBERSOURCE
        public async Task<ActionResult> TokengenerationCallBack()
        {
            var processValue = DateTime.Now.ToString("yyyyMMddhhmmss");
            var resultCode = "1";
            CybersourceCreateAppAssociationDto result = null;
            try
            {
                NLogLogger.LogAppsEvent(NLogType.Info,
                    "AppAdmissionController - TokengenerationCallBack - INICIO METODO. PROCESO " + processValue);

                var formData = GenerateDictionary(Request.Form);
                result = await _webServiceAssosiateClientService.ProccesDataFromCybersourceForApps(formData);

                if (result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto != null)
                    {
                        resultCode = "0";
                        ViewBag.ResultDescription = "Se realizó la asociación correctamente";
                    }
                    else
                    {
                        resultCode = result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode.ToString();
                        ViewBag.ResultDescription =
                            result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc;
                    }
                }
                else
                {
                    resultCode = result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseCode.ToString();
                    ViewBag.ResultDescription = result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData.PaymentResponseMsg;
                }

                var model = new AppAdmissionModel()
                            {
                                UrlCallback = result.WebhookRegistrationDto.UrlCallback,
                                IdOperation = result.WebhookRegistrationDto.IdOperation,
                            };
                CleanSession();
                NLogLogger.LogAppsEvent(NLogType.Info,
                    string.Format(
                        "AppAdmissionController - TokengenerationCallBack - Realizo POST a {0}, id de operación {1}, PROCESO {2}, idapp {3}",
                        result.WebhookRegistrationDto.UrlCallback, result.WebhookRegistrationDto.IdOperation, processValue, result.WebhookRegistrationDto.IdApp));

                ViewBag.ResultCode = resultCode;
                return View("End", model);

            }
            catch (WebApiClientBusinessException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error,
                    "AppAdmissionController - TokengenerationCallBack WebApiClientBusinessException- Id Operation: PROCESO " + processValue);
                NLogLogger.LogAppsEvent(ex);
            }
            catch (WebApiClientFatalException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error,
                    "AppAdmissionController - TokengenerationCallBack WebApiClientFatalException- Id Operation: PROCESO " + processValue);
                NLogLogger.LogAppsEvent(ex);
            }
            catch (Exception ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Error,
                    "AppAdmissionController - TokengenerationCallBack Excepcion- Id Operation: {0}, Email PROCESO " + processValue);
                NLogLogger.LogAppsEvent(ex);
            }

            if (result != null)
            {
                if (ViewBag.ResultCode == null)
                {
                    resultCode = "1";
                }
                if (ViewBag.ResultDescription == null)
                {
                    ViewBag.ResultDescription = "Error General";
                }

                var model = new AppAdmissionModel()
                {
                    UrlCallback = result.WebhookRegistrationDto.UrlCallback,
                    IdOperation = result.WebhookRegistrationDto.IdOperation,
                };
                ViewBag.ResultCode = resultCode;

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - Associate. IdApp: {0}, Email: {1}, Operacion {2}",
                          model.ServiceUrlName, model.Email, model.IdOperation));

                return View("End", model);
            }

            return RedirectToAction("Index", "Error");
        }

        public ActionResult CancelAssociation()
        {
            try
            {
                var result = new AppAdmissionModel();
                var model = (AppAdmissionModel)Session[SessionConstants.APP_ADMISSION];

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("AppAdmissionController - CancelAssociation. Email {0}, Operacion {1}", model.Email, model.IdOperation));

                if (model != null)
                {
                    result.UrlCallback = model.UrlCallback;
                    result.IdOperation = model.IdOperation;
                }
                ViewBag.ResultCode = "16"; //VER que codigo se utiliza para CANCELADO
                ViewBag.ResultDescription = "El usuario ha cancelado la operación.";

                return View("End", result);
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - CancelAssociation - Excepcion");
                NLogLogger.LogAppsEvent(e);
                return View("Error");
            }
        }

        private ActionResult EvaluateErrors(CybersourceMsg reasonCode, AppAdmissionModel model)
        {
            if (reasonCode != CybersourceMsg.Accepted)
            {
                NLogLogger.LogAppsEvent(NLogType.Error,
                    "AppAdmissionController - TokengenerationCallBack - Error en CS: " + reasonCode);

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
                 * 236 : [página error] Ha ocurrido un error. del procesador
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

        private ActionResult InvalidCardData(CybersourceMsg reasonCode, AppAdmissionModel model)
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
                    ShowNotification(PresentationWebStrings.Card_StolenLostCard, NotificationType.Info);
                    break;
                case CybersourceMsg.CreditLimitReached:
                    ShowNotification(PresentationWebStrings.Card_CreditLimitReached, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidCVN:
                    ShowNotification(PresentationWebStrings.Card_InvalidCVN, NotificationType.Info);
                    break;
                case CybersourceMsg.AccountFrozen:
                    ShowNotification(PresentationWebStrings.Card_AccountFrozen, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidAccountNumber:
                    ShowNotification(PresentationWebStrings.Card_InvalidAccountNumber, NotificationType.Info);
                    break;
                case CybersourceMsg.CardTypeNotAccepted:
                    ShowNotification(PresentationWebStrings.Card_CardTypeNotAccepted, NotificationType.Info);
                    break;
                case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    ShowNotification(PresentationWebStrings.Card_InvalidCardTypeOrNotCorrelateWithCardNumber,
                        NotificationType.Info);
                    break;
                case CybersourceMsg.CVNCheckInvalid:
                    ShowNotification(PresentationWebStrings.Card_CVNCheckInvalid, NotificationType.Info);
                    break;
                default:
                    ShowNotification(
                        "No hemos podido procesar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.",
                        NotificationType.Info);
                    break;
            }
            SaveErrorAudit(reasonCode, model);
            model.ShowCardsAfterCsFail = true;
            return View("Index", model);
        }

        private ActionResult GetError(CybersourceMsg reasonCode, AppAdmissionModel model)
        {
            SaveErrorAudit(reasonCode, model);
            ShowNotification(
                "No hemos podido procesar su tarjeta. Por favor intente nuevamente o ingrese una nueva tarjeta.",
                NotificationType.Error);
            model.LoadRegistredData = model.UserId != null && model.UserId != Guid.Empty;
            model.LoadNewData = model.UserId == Guid.Empty || model.UserId == null;
            return View("Index", model);
        }

        private void SaveErrorAudit(CybersourceMsg reasonCode, AppAdmissionModel model)
        {
            NLogLogger.LogAppsEvent(NLogType.Error,
                "AppAdmissionController - GetError - Reason code : " + (int)reasonCode);
            var strLog = string.Format(LogStrings.Payment_cybersourceCallback_error,
                reasonCode + " " + (int)reasonCode,
                //model.UserId == Guid.Empty ? model.Email : model.Email_AppUser,
                model.UserId == Guid.Empty ? model.Email : "",
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

                var viewModel = new AppAdmissionModel { TermsAndConditionsService = page.Content };

                var content = RenderPartialViewToString("_TermsAndConditionsVisa", viewModel);

                response = AjaxResponse.Success;
                notification = NotificationType.Success;
                title = "Correcto";
                message = "Correcto";

                return Json(new JsonResponse(response, content, message, title, notification),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Error, "AppAdmissionController - ConditionsVisa - Excepcion");
                NLogLogger.LogAppsEvent(exception);
            }
            return Json(new JsonResponse(response, null, message, title, notification), JsonRequestBehavior.AllowGet);
        }

        private void CheckReferences(AppAdmissionModel model)
        {
            if (string.IsNullOrEmpty(model.ReferenceName) && string.IsNullOrEmpty(model.ReferenceValue))
            {
                ModelState.Remove("ReferenceValue");
            }
            if (string.IsNullOrEmpty(model.ReferenceName2) && string.IsNullOrEmpty(model.ReferenceValue2))
            {
                ModelState.Remove("ReferenceValue2");
            }
            if (string.IsNullOrEmpty(model.ReferenceName3) && string.IsNullOrEmpty(model.ReferenceValue3))
            {
                ModelState.Remove("ReferenceValue3");
            }
            if (string.IsNullOrEmpty(model.ReferenceName4) && string.IsNullOrEmpty(model.ReferenceValue4))
            {
                ModelState.Remove("ReferenceValue4");
            }
            if (string.IsNullOrEmpty(model.ReferenceName5) && string.IsNullOrEmpty(model.ReferenceValue5))
            {
                ModelState.Remove("ReferenceValue5");
            }
            if (string.IsNullOrEmpty(model.ReferenceName6) && string.IsNullOrEmpty(model.ReferenceValue6))
            {
                ModelState.Remove("ReferenceValue6");
            }

        }

        private void CheckUserDataOnRegistered()
        {
            ModelState.Remove("Name");
            ModelState.Remove("Surname");
            ModelState.Remove("IdentityNumber");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("MobileNumber");
            ModelState.Remove("Address");
            ModelState.Remove("Password");
            ModelState.Remove("PasswordConfirmation");
        }

        private async Task<ApplicationUserDto> CreateNewUser(AppAdmissionModel model)
        {
            var appUser = new ApplicationUserDto()
                      {
                          Address = model.Address,
                          Email = model.Email,
                          IdentityNumber = model.IdentityNumber,
                          MobileNumber = model.MobileNumber,
                          Name = model.Name,
                          Surname = model.Surname,
                          PhoneNumber = model.PhoneNumber,
                      };

            //CREO EL USUARIO
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
                                                        CallCenterKey = ""
                                                    });
            appUser = await _webApplicationUserClientService.Find(model.Email);
            NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmissionController - TokengenerationCallBack - Usuario nuevo CREADO " + model.Email + ", Operacion " + model.IdOperation);
            return appUser;
        }

        private string HashPassword(string password)
        {
            return PasswordHash.CreatePasswordForApps(password);
        }

        public void SaveBrowserData(string idOperation, string idApp)
        {
            var browser = Request.Browser;
            string str = "Id app: " + idApp + ", Id operation: " + idOperation + ", " +
                       "Browser:"
                       + "Type = " + browser.Type + ","
                       + "Name = " + browser.Browser + ","
                       + "Version = " + browser.Version + ","
                       + "Major Version = " + browser.MajorVersion + ","
                       + "Minor Version = " + browser.MinorVersion + ","
                       + "Platform = " + browser.Platform + ","
                       + "Is Beta = " + browser.Beta + ","
                       + "Is Crawler = " + browser.Crawler + ","
                       + "Is AOL = " + browser.AOL + ","
                       + "Is Win16 = " + browser.Win16 + ","
                       + "Is Win32 = " + browser.Win32 + ","
                       + "Supports Frames = " + browser.Frames + ","
                       + "Supports Tables = " + browser.Tables + ","
                       + "Supports Cookies = " + browser.Cookies + ","
                       + "Supports VBScript = " + browser.VBScript + ","
                       + "Supports JavaScript = " +
                       browser.EcmaScriptVersion.ToString() + ","
                       + "Supports Java Applets = " + browser.JavaApplets + ","
                       + "Supports ActiveX Controls = " + browser.ActiveXControls
                       + "IsMobileDevice = " + browser.IsMobileDevice;

            //var MobileCheck = new Regex(@"android|(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            //var MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            if (Request != null && browser.IsMobileDevice && Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                var u = Request.ServerVariables["HTTP_USER_AGENT"].ToString();
                str = str + ", HTTP_USER_AGENT = " + u;

                //if (u.Length < 4)
                //{

                //}
                //if (MobileCheck.IsMatch(u) || MobileVersionCheck.IsMatch(u.Substring(0, 4)))
                //{

                //}
            }
            NLogLogger.LogAppsEvent(NLogType.Info, str);
        }

        //Esto esp para probar AppAdmission con un iframe
        //public ActionResult Test()
        //{
        //    return View("Iframe");
        //}

        //public ActionResult Iframe()
        //{
        //    return View("Test");
        //}
    }
}