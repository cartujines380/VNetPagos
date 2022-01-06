using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Utilities.DigitalSignature;
using VisaNet.Utilities.Notifications;
using VisaNet.VONRegister.Constants;
using VisaNet.VONRegister.Models;

namespace VisaNet.VONRegister.Controllers
{

    public class AccountController : BaseController
    {
        private readonly IWebApplicationUserClientService _userService;
        private readonly IWebServiceClientService _servicesService;
        private readonly IWebWebhookLogClientService _webWebhookLogService;
        private readonly IWebPageClientService _pageService;

        public AccountController(IWebApplicationUserClientService userService, IWebServiceClientService servicesService, IWebWebhookLogClientService webWebhookLogService, IWebPageClientService pageService)
        {
            _userService = userService;
            _servicesService = servicesService;
            _webWebhookLogService = webWebhookLogService;
            _pageService = pageService;
        }

        [HttpPost]
        public async Task<ActionResult> Index(Register model)
        {

            try
            {
                ClearSessionVariables();
                ModelState.Clear();
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
                var digitalSignature = Request.Form["FirmaDigital"];

                model.Name = name;
                model.Surname = surname;
                model.Email = email;
                model.Address = address;
                model.Phone = phone;
                model.Mobile = mobile;
                model.Identity = identity;

                model.EditableEmail = !allowsNewEmail.Equals("N", StringComparison.InvariantCultureIgnoreCase);

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("001 - IdOperacion:({11}) Email:{0} Nombre:{1} Apellido:{2} Direccion:{3} Telefono:{4} Movil:{5} CI:{6} PermiteCambioEmail:{7} IdOperacion:{8} UrlCallback:{9} IdApp:{10}", email, name, surname, address, phone, mobile, identity, allowsNewEmail, operationId, callbackUrl, appId, operationId));

                var webHook = new WebhookRegistrationDto
                {
                    IdOperation = operationId,
                    UrlCallback = callbackUrl,
                    EnableEmailChange =
                        allowsNewEmail.Equals("N", StringComparison.InvariantCultureIgnoreCase)
                            ? "N"
                            : "S",
                    IdApp = appId,
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

                var operationIdRepeated = await _webWebhookLogService.WebhookRegistrationIsIdOperationRepited(operationId, appId);

                if (operationIdRepeated)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error, string.Format("007 - IdOperacion:({0}) Id operación repetido. IdApp: {1}.", operationId, appId));
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        OperationId = operationId,
                        ResultCode = "11",
                        ResultDescription = "Id Operación repetido."
                    });
                }

                try
                {
                    await _webWebhookLogService.CreateWebhookRegistration(webHook);
                }
                catch (WebApiClientBusinessException exception)
                {
                    NLogLogger.LogAppsEvent(NLogType.Error, string.Format("008 - IdOperacion:({0}) Excepción al crear CreateWebhookRegistration", Session[SessionConstants.OperationId]));
                    NLogLogger.LogAppsEvent(exception);
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        OperationId = operationId,
                        ResultCode = "11",
                        ResultDescription = "Id Operación repetido."
                    });

                }

                //VALIDO QUE SEA QUIEN DICE SER
                var certificateThumbprint = await _servicesService.GetCertificateThumbprintIdApp(appId);
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("018 - Certificado no encotrado. IdApp: {0}, IdOperacion: {1} ", appId, operationId));
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        OperationId = operationId,
                        ResultCode = "13",
                        ResultDescription = "CERTIFICADO NO ENCONTRADO"
                    });
                }
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

                var valid = DigitalSignature.CheckSignature(paramsArray, digitalSignature, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("019 - Firma invalida. IdApp: {0}, IdOperacion: {1} ", appId, operationId);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        OperationId = operationId,
                        ResultCode = "12",
                        ResultDescription = "FIRMA INVALIDA"
                    });
                }

                //GUARDO HASH DOMINIO PARA IFRAME
                if (Request.UrlReferrer != null)
                {
                    var dns = Request.UrlReferrer.DnsSafeHost;
                    var domains = ConfigurationManager.AppSettings["EnableDomains"];
                    if (domains.Contains(dns))
                    {
                        Session[SessionConstants.HashedDomain] = dns;
                    }
                }

                if (!IsRequired(webHook))
                {
                    if (!string.IsNullOrEmpty(webHook.UrlCallback))
                    {
                        return RedirectToAction("End", "Home", new End
                        {
                            UrlCallback = webHook.UrlCallback,
                            ResultCode = "3",
                            ResultDescription = "Falta campo IdOperacion."
                        });
                    }

                    return View("Error");
                }

                var service = await LoadService();
                //En caso de que el AppId no sea válido
                if (service == null)
                {
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        OperationId = operationId,
                        ResultCode = "2",
                        ResultDescription = "ERRORES EN LOS CAMPOS ENVIADOS."
                    });
                }
                model.AskUserForReferences = service.AskUserForReferences;

                try
                {
                    var user = await _userService.Find(model.Email);
                    //If there is a user redirects to SignIn
                    if (user != null)
                    {
                        return RedirectToAction("SignIn", new { email = model.Email, model.EditableEmail });
                    }

                    //Else redirect to Register
                    return RedirectToAction("Create", model);
                }
                catch (WebApiClientBusinessException e)
                {
                    return RedirectToAction("Create", model);
                }
                catch (Exception exception)
                {
                    NLogLogger.LogAppsEvent(exception);
                    return RedirectToAction("End", "Home", new End
                    {
                        UrlCallback = webHook.UrlCallback,
                        ResultCode = "1",
                        OperationId = operationId,
                        ResultDescription = "Error General."
                    });
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(exception);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<ActionResult> NewUser()
        {
            var idOperation = (string)Session[SessionConstants.OperationId];
            var service = (ServiceDto)Session[SessionConstants.CurrentService];

            if (!string.IsNullOrEmpty(idOperation))
            {
                var logOperation = await _webWebhookLogService.GetwebHookRegistrationsByIdOperation(idOperation, service.Id);
                var model = new Register
                            {
                                Name = logOperation.UserData.Name,
                                Surname = logOperation.UserData.Surname,
                                Email = logOperation.UserData.Email,
                                Address = logOperation.UserData.Address,
                                Phone = logOperation.UserData.PhoneNumber,
                                Mobile = logOperation.UserData.MobileNumber,
                                Identity = logOperation.UserData.IdentityNumber,
                                Payment = CreatePaymentModel((ServiceDto)Session[SessionConstants.CurrentService]),
                                EditableEmail = !logOperation.EnableEmailChange.Equals("N", StringComparison.InvariantCultureIgnoreCase),
                            };
                return RedirectToAction("Create", model);
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<ActionResult> Create(Register model)
        {
            var serviceDto = (ServiceDto)Session[SessionConstants.CurrentService];
            model.Payment = CreatePaymentModel(serviceDto);
            var page = await _pageService.FindType(PageTypeDto.LegalPages);
            model.TermsAndConditionsVisa = page.Content;
            model.AgreeToTermsVisaNetPagos = true;
            model.TermsAndConditionsService = serviceDto.TermsAndConditions;
            model.AgreeToTermsService = true;
            model.AskUserForReferences = serviceDto.AskUserForReferences;
            model.ServiceName = serviceDto.Name;
            return View("Register", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Register model)
        {
            var page = await _pageService.FindType(PageTypeDto.LegalPages);
            model.TermsAndConditionsVisa = page.Content;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //User must agree to terms and conditions
            if (!model.AgreeToTermsVisaNetPagos)
            {
                ShowToastr(PresentationWebStrings.Registration_Conditions_Validation, NotificationType.Alert);
                return View(model);
            }

            //User must agree to terms and conditions
            if (model.Password != model.PasswordReEntered)
            {
                ShowToastr(PresentationWebStrings.Resgistration_Password_DoesnotMatch, NotificationType.Alert);
                return View(model);
            }

            try
            {
                model.NewUser = true;
                Session[SessionConstants.RegisterModel] = model;
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("009 - IdOperacion:({7}) Email:{0} Nombre:{1} Apellido:{2} Direccion:{3} Telefono:{4} Movil:{5} CI:{6}", model.Email, model.Name, model.Surname, model.Address, model.Phone, model.Mobile, model.Identity, Session[SessionConstants.OperationId]));

                return RedirectToAction("Add", "Card");
            }
            catch (BusinessException e)
            {
                ShowToastr(e.Message, NotificationType.Error);
                NLogLogger.LogAppsEvent(e);
                return View(model);
            }
            catch (Exception e)
            {
                ShowToastr(e.Message, NotificationType.Error);
                NLogLogger.LogAppsEvent(e);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> LogIn()
        {
            var idOperation = (string)Session[SessionConstants.OperationId];
            var service = (ServiceDto)Session[SessionConstants.CurrentService];

            if (!string.IsNullOrEmpty(idOperation))
            {
                var logOperation = await _webWebhookLogService.GetwebHookRegistrationsByIdOperation(idOperation, service.Id);
                return RedirectToAction("SignIn", new
                            {
                                returnUrl = logOperation.UrlCallback,
                                email = logOperation.UserData.Email,
                                editableEmail = !logOperation.EnableEmailChange.Equals("N", StringComparison.InvariantCultureIgnoreCase)
                            });
            }
            return View("Error");
        }

        [HttpGet]
        public async Task<ActionResult> SignIn(string returnUrl, string email, bool editableEmail)
        {
            if (User.Identity.IsAuthenticated)
            {
                return await LoadUser(User.Identity.Name, returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new SignIn { UserName = email, EditableEmail = editableEmail });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(SignIn model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (
                        await
                            _userService.ValidateUser(new ValidateUserDto
                            {
                                UserName = model.UserName,
                                Password = model.Password
                            }))
                    {
                        NLogLogger.LogAppsEvent(NLogType.Info,
                            string.Format("002 - IdOperacion:({1}) Email:{0}", model.UserName,
                                Session[SessionConstants.OperationId]));
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return await LoadUser(model.UserName, returnUrl);
                    }

                    NLogLogger.LogAppsEvent(NLogType.Info,
                        string.Format("003 - IdOperacion:({1}) Email:{0}", model.UserName,
                            Session[SessionConstants.OperationId]));
                    ModelState.AddModelError("Password", PresentationCoreMessages.Security_UserNameOrPasswordNotValid);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("Password", PresentationCoreMessages.Security_UserNameOrPasswordNotValid);
                    return View(model);
                }


            }
            catch (WebApiClientBusinessException)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("003 - IdOperacion:({1}) Email:{0}", model.UserName, Session[SessionConstants.OperationId]));
                ShowToastr(PresentationCoreMessages.Security_UserNameOrPasswordNotValid, NotificationType.Error);
                return View(model);
            }
            catch (WebApiClientFatalException exception)
            {
                NLogLogger.LogEvent(exception);
                ShowToastr(exception.Message, NotificationType.Error);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string email, bool editableEmail)
        {
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("004 - IdOperacion:({1}) Email:{0}", email, Session[SessionConstants.OperationId]));
            return View(new ResetPassowrd
            {
                UserName = email,
                EditableEmail = editableEmail
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassowrd model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.ResetPassword(model.UserName);
                ShowToastr(PresentationCoreMessages.Security_ForgetMyPassword_Result, NotificationType.Info);
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("005 - IdOperacion:({1}) Email:{0}", model.UserName, Session[SessionConstants.OperationId]));
                return RedirectToAction("SignIn", "Account", new { Email = model.UserName, model.EditableEmail });
            }
            catch (BusinessException e)
            {
                ShowToastr(e.Message, NotificationType.Error);
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("006 - IdOperacion:({1}) Email:{0}", model.UserName, Session[SessionConstants.OperationId]));
                return View(model);
            }
            catch (Exception e)
            {
                ShowToastr(e.Message, NotificationType.Error);
                NLogLogger.LogAppsEvent(e);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SignOut()
        {
            var opId = (string)Session[SessionConstants.OperationId];
            var service = (ServiceDto)Session[SessionConstants.CurrentService];
            var registration = await _webWebhookLogService.GetwebHookRegistrationsByIdOperation(opId, service.Id);

            if (registration == null)
                return RedirectToAction("Cancel", "Home");

            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn", "Account", new
            {
                returnUrl = registration.UrlCallback,
                email = registration.UserData.Email,
                editableEmail = !registration.EnableEmailChange.Equals("N")
            });
        }

        #region Private Methods
        private async Task<ActionResult> LoadUser(string userName, string returnUrl)
        {
            var user = await _userService.Find(userName);
            Session[SessionConstants.CurrentSelectedUser] = user;

            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            var register = new Register
            {
                Surname = user.Surname,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Identity = user.IdentityNumber,
                Mobile = user.MobileNumber,
                Phone = user.PhoneNumber
            };

            //TempData["model"] = register;
            Session[SessionConstants.RegisterModel] = register;

            var serviceDto = Session[SessionConstants.CurrentService] as ServiceDto;
            register.ServiceName = serviceDto.Name;

            if (serviceDto.AskUserForReferences)
            {
                return RedirectToAction("References", "Card");
            }

            Session[SessionConstants.RegisterModel] = register;
            return RedirectToAction("Add", "Card");
        }

        private bool ValidateRequieredFields()
        {
            var callbackUrl = !string.IsNullOrEmpty(Request.Form["UrlCallback"]);
            var operationId = !string.IsNullOrEmpty(Request.Form["IdOperacion"]);
            var appId = !string.IsNullOrEmpty(Request.Form["IdApp"]);

            return callbackUrl && operationId && appId;
        }

        private async Task<ServiceDto> LoadService()
        {
            var service = await _servicesService.GetServiceByUrlName(Request.Form["IdApp"]);
            Session[SessionConstants.CurrentService] = service;
            Session[SessionConstants.OperationId] = Request.Form["IdOperacion"];
            Session[SessionConstants.CallbackUrl] = Request.Form["UrlCallback"];

            return service;
        }

        private Payment CreatePaymentModel(ServiceDto service)
        {
            return new Payment
            {
                Description = service.Description,
                Reference1Name = service.ReferenceParamName,
                Reference1Regex = service.ReferenceParamRegex,
                Reference2Name = service.ReferenceParamName2,
                Reference2Regex = service.ReferenceParamRegex2,
                Reference3Name = service.ReferenceParamName3,
                Reference3Regex = service.ReferenceParamRegex3,
                Reference4Name = service.ReferenceParamName4,
                Reference4Regex = service.ReferenceParamRegex4,
                Reference5Name = service.ReferenceParamName5,
                Reference5Regex = service.ReferenceParamRegex5,
                Reference6Name = service.ReferenceParamName6,
                Reference6Regex = service.ReferenceParamRegex6,
                //ToolTipImage = service.ImageTooltip.InternalName
            };
        }


        private bool IsRequired(WebhookRegistrationDto webHook)
        {
            if (string.IsNullOrEmpty(webHook.UrlCallback)) return false;
            if (string.IsNullOrEmpty(webHook.IdOperation)) return false;
            return true;
        }
        #endregion
    }
}