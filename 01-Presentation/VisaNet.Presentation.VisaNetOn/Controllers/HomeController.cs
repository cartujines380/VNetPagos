using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.VisaNetOn.Constants;
using VisaNet.Presentation.VisaNetOn.Models;
using VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebWebhookRegistrationClientService _webhookRegistrationClientService;
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebServiceAssosiateClientService _serviceAssosiateClientService;
        private readonly IWebApplicationUserClientService _applicationUserClientService;
        private readonly IWebVisaNetOnIntegrationClientService _visaNetOnIntegrationClientService;
        private readonly IWebCardClientService _webCardClientService;
        private readonly IWebParameterClientService _webParameterClientService;

        public HomeController(IWebWebhookRegistrationClientService webhookRegistrationClientService, IWebServiceClientService serviceClientService,
            IWebServiceAssosiateClientService serviceAssosiateClientService, IWebApplicationUserClientService applicationUserClientService,
            IWebVisaNetOnIntegrationClientService visaNetOnIntegrationClientService, IWebCardClientService webCardClientService, IWebParameterClientService webParameterClientService)
        {
            _webhookRegistrationClientService = webhookRegistrationClientService;
            _serviceClientService = serviceClientService;
            _serviceAssosiateClientService = serviceAssosiateClientService;
            _applicationUserClientService = applicationUserClientService;
            _visaNetOnIntegrationClientService = visaNetOnIntegrationClientService;
            _webCardClientService = webCardClientService;
            _webParameterClientService = webParameterClientService;
        }

        [HttpPost]
        public async Task<ActionResult> Index(AccessModel accessModel)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("HomeController - Index - Token de acceso: {0}", accessModel.TokenAcceso));

            if (string.IsNullOrEmpty(accessModel.TokenAcceso))
            {
                NLogLogger.LogEvent(NLogType.Error, "HomeController - Index - AccessToken vacio");
                return RedirectToAction("Error");
            }

            WebhookRegistrationDto webhookRegistrationDto = null;
            try
            {
                //Obtener webhookRegistrationDto
                var dto = new WebhookAccessTokenDto { AccessToken = accessModel.TokenAcceso };
                webhookRegistrationDto = await _webhookRegistrationClientService.GetByAccessToken(dto);
                if (webhookRegistrationDto == null)
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("HomeController - Index - AccessToken ({0}) no valido. No se encontro en BD", accessModel.TokenAcceso));
                    return RedirectToAction("Error");
                }

                //Validar token
                await _webhookRegistrationClientService.ValidateAccessToken(dto);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("HomeController - Index - AccessToken ({0}) inválido.", accessModel.TokenAcceso));
                return HandleBusinessException(webhookRegistrationDto, e);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return RedirectToAction("Error");
            }

            try
            {
                //Obtener y validar servicio
                var serviceAndValidation = await FindAndValidateService(webhookRegistrationDto);
                var service = serviceAndValidation.Item1;
                var validationRedirect = serviceAndValidation.Item2;
                if (validationRedirect != null)
                {
                    //Si falla alguna validacion del servicio entonces retorna con el codigo correspondiente
                    return validationRedirect;
                }

                //Si viene IdUsuario se busca y se valida
                UserData userData = null;
                if (!string.IsNullOrEmpty(webhookRegistrationDto.IdUsuario))
                {
                    //Primero se busca en tabla de VonData (AnonymousUser)
                    userData = await FindRecurrentUser(webhookRegistrationDto, service);
                    if (userData == null)
                    {
                        //Si no lo encontro se busca en tabla de Servicios Asociados (ApplicationUser)
                        var userAndValidation = await FindAndValidateRegisteredUser(webhookRegistrationDto, service);
                        userData = userAndValidation.Item1;
                        validationRedirect = userAndValidation.Item2;
                        if (validationRedirect != null)
                        {
                            //Si falla alguna validacion del usuario entonces retorna con el codigo correspondiente
                            return validationRedirect;
                        }
                    }
                }

                var pageModel = await LoadPageModel(webhookRegistrationDto, service, userData);

                //Si no hay factura es una asociacion
                if (webhookRegistrationDto.Bill == null || string.IsNullOrEmpty(webhookRegistrationDto.Bill.ExternalId))
                {
                    Session[SessionConstants.PAGE_ASSOCIATION_MODEL] = pageModel;
                    return RedirectToAction("Index", "Association", new { t = accessModel.TokenAcceso });
                }
                //Si hay factura es un pago
                Session[SessionConstants.PAGE_PAYMENT_MODEL] = pageModel;
                return RedirectToAction("Index", "Payment", new { t = accessModel.TokenAcceso });
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("HomeController - Index - Excepcion. AccessToken: {0}. " +
                    "IdApp: {1}. IdOperacion: {2}", accessModel.TokenAcceso, webhookRegistrationDto.IdApp, webhookRegistrationDto.IdOperation));
                NLogLogger.LogEvent(e);

                return RedirectToAction("End", new End
                {
                    UrlCallback = webhookRegistrationDto.UrlCallback,
                    OperationId = webhookRegistrationDto.IdOperation,
                    ResultCode = "1",
                    ResultDescription = "Error general."
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> LinkPayment(AccessModel accessModel)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("HomeController - LinkPayment - Token de acceso: {0}", accessModel.TokenAcceso));

            if (string.IsNullOrEmpty(accessModel.TokenAcceso))
            {
                NLogLogger.LogEvent(NLogType.Error, "HomeController - LinkPayment - AccessToken vacio");
                return RedirectToAction("Error");
            }

            WebhookRegistrationDto webhookRegistrationDto = null;
            try
            {
                //Obtener webhookRegistrationDto
                var dto = new WebhookAccessTokenDto { AccessToken = accessModel.TokenAcceso };

                webhookRegistrationDto = await _webhookRegistrationClientService.GetByAccessToken(dto);
                if (webhookRegistrationDto == null)
                {
                    TempData["token"] = accessModel.TokenAcceso;
                    return RedirectToAction("Error");
                }

                //Validar token
                await _webhookRegistrationClientService.ValidateAccessToken(dto);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("HomeController - LinkPayment - AccessToken ({0}) inválido.", accessModel.TokenAcceso));
                return HandleBusinessException(webhookRegistrationDto, e);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return RedirectToAction("Error");
            }

            //Obtener y validar servicio
            var serviceAndValidation = await FindAndValidateService(webhookRegistrationDto);
            var service = serviceAndValidation.Item1;
            var validationRedirect = serviceAndValidation.Item2;
            if (validationRedirect != null)
            {
                //Si falla alguna validacion del servicio entonces retorna con el codigo correspondiente
                return validationRedirect;
            }

            var pageModel = (PagePaymentModel)await LoadPageModel(webhookRegistrationDto, service, null);
            pageModel.RememberUser = false; //Para ocultar checkbox de recordar

            //Si hay factura es un pago
            Session[SessionConstants.PAGE_PAYMENT_MODEL] = pageModel;
            return RedirectToAction("Index", "Payment");
        }

        public async Task<ActionResult> SuccessfulLogIn(Guid webhookRegistrationId)
        {
            WebhookRegistrationDto webhookRegistrationDto = null;
            try
            {
                //Obtener webhookRegistrationDto
                webhookRegistrationDto = await _webhookRegistrationClientService.FindById(webhookRegistrationId);
                var applicationUser = (ApplicationUserDto)TempData["ApplicationUser"];

                var userData = new UserData
                {
                    ApplicationUserId = applicationUser.Id,
                    Email = applicationUser.Email,
                    Name = applicationUser.Name,
                    Surname = applicationUser.Surname,
                    Address = applicationUser.Address,
                    CybersourceIdentifier = applicationUser.CyberSourceIdentifier,
                    CardList = applicationUser.CardDtos != null ? applicationUser.CardDtos.Where(x => x.Active && !x.Deleted).Select(x => new CardData
                    {
                        Id = x.Id,
                        Active = x.Active,
                        MaskedNumber = x.MaskedNumber,
                        DueDate = x.DueDate,
                        Expired = new DateTime(x.DueDate.AddMonths(1).Year, x.DueDate.AddMonths(1).Month, 1) <= DateTime.Now
                    }).ToList() : new List<CardData>()
                };

                //Obtener y validar servicio
                var serviceAndValidation = await FindAndValidateService(webhookRegistrationDto);
                var service = serviceAndValidation.Item1;
                var validationRedirect = serviceAndValidation.Item2;
                if (validationRedirect != null)
                {
                    //Si falla alguna validacion del servicio entonces retorna con el codigo correspondiente
                    return validationRedirect;
                }

                //Cargar cuotas de tarjetas
                foreach (var cardData in userData.CardList)
                {
                    cardData.Quotas = await GetCardCuotas(Convert.ToInt32(cardData.MaskedNumber.Substring(0, 6)), service);
                }

                var pageModel = await LoadPageModel(webhookRegistrationDto, service, userData);

                //Si no hay factura es una asociacion
                if (webhookRegistrationDto.Bill == null || string.IsNullOrEmpty(webhookRegistrationDto.Bill.ExternalId))
                {
                    Session[SessionConstants.PAGE_ASSOCIATION_MODEL] = pageModel;
                    return RedirectToAction("Index", "Association");
                }
                //Si hay factura es un pago
                Session[SessionConstants.PAGE_PAYMENT_MODEL] = pageModel;
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception e)
            {

                NLogLogger.LogEvent(NLogType.Error, string.Format("HomeController - SuccessfulLogin - Excepcion. WebhookRegistrationId: {0}. " +
                    "IdApp: {1}. IdOperacion: {2}", webhookRegistrationId, webhookRegistrationDto != null ? webhookRegistrationDto.IdApp : string.Empty,
                    webhookRegistrationDto != null ? webhookRegistrationDto.IdOperation : string.Empty));
                NLogLogger.LogEvent(e);

                return RedirectToAction("End", new End
                {
                    WebhookRegistrationId = webhookRegistrationId,
                    ResultCode = "1",
                    ResultDescription = "Error general.",
                    AppId = webhookRegistrationDto != null ? webhookRegistrationDto.IdApp : string.Empty,
                    OperationId = webhookRegistrationDto != null ? webhookRegistrationDto.IdOperation : string.Empty,
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> End(End model)
        {
            WebhookRegistrationDto dto = null;
            if (string.IsNullOrEmpty(model.UrlCallback))
            {
                //Si no viene UrlCallback chequeo si la puedo obtener por BD por WebhookRegistrationId
                if (model.WebhookRegistrationId.HasValue && model.WebhookRegistrationId.Value != Guid.Empty)
                {
                    dto = await _webhookRegistrationClientService.FindById(model.WebhookRegistrationId.Value);
                }
                else
                {
                    RedirectToAction("Error");
                }
                if (dto == null)
                {
                    RedirectToAction("Error");
                }
                else
                {
                    model.UrlCallback = dto.UrlCallback;
                    model.OperationId = dto.IdOperation;
                }
            }

            NLogLogger.LogEvent(NLogType.Info, string.Format("HomeController - End - IdApp: {0}, IdOperation: {1}, ResultCode: {2}, ResultDesc: {3}, TrnsNumber: {4}",
                model.AppId, model.OperationId, model.ResultCode, model.ResultDescription, model.TrnsNumber));

            return View("End", model);
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View("Error");
        }

        private async Task<PageModel> LoadPageModel(WebhookRegistrationDto webhookRegistrationDto, ServiceDto service, UserData loggedUser = null)
        {
            PageModel pageModel;
            UserData user = loggedUser;
            var merchantIdParams = string.Empty;

            if (webhookRegistrationDto.Bill != null && !string.IsNullOrEmpty(webhookRegistrationDto.Bill.ExternalId))
            {
                var enableRememberUser = !string.Equals(webhookRegistrationDto.EnableRememberUser, "N", StringComparison.InvariantCultureIgnoreCase);

                //Es un Pago
                pageModel = new PagePaymentModel
                {
                    RememberUser = user == null && enableRememberUser,
                    EnableRememberUser = enableRememberUser,
                    Quotas = 1,
                    BillData = new BillData
                    {
                        Amount = (double.Parse(webhookRegistrationDto.Bill.Amount) / 100),
                        Currency = webhookRegistrationDto.Bill.Currency.Equals("N", StringComparison.InvariantCultureIgnoreCase) ? "UYU" : "USD",
                        CurrencySymbol = webhookRegistrationDto.Bill.Currency.Equals("N", StringComparison.InvariantCultureIgnoreCase) ? "$" : "US$",
                        Description = webhookRegistrationDto.Bill.Description,
                        ExternalId = webhookRegistrationDto.Bill.ExternalId,
                        FinalConsumer = webhookRegistrationDto.Bill.FinalConsumer == "1",
                        GenerationDate = !string.IsNullOrEmpty(webhookRegistrationDto.Bill.GenerationDate) ?
                            DateTime.ParseExact(webhookRegistrationDto.Bill.GenerationDate, "yyyyMMdd", CultureInfo.InvariantCulture)
                            : DateTime.Today,
                        AcceptQuotas = !string.IsNullOrEmpty(webhookRegistrationDto.Bill.Quota) && webhookRegistrationDto.Bill.Quota.Equals("S", StringComparison.InvariantCultureIgnoreCase),
                        TaxedAmount = (double.Parse(webhookRegistrationDto.Bill.TaxedAmount) / 100)
                    }
                };

                if (webhookRegistrationDto.BillLines != null && webhookRegistrationDto.BillLines.Any() &&
                    webhookRegistrationDto.BillLines.All(x => !string.IsNullOrEmpty(x.Concept) && (double.Parse(x.Amount) / 100) > 0))
                {
                    //Si las lineas de factura estan bien formadas se agregan al modelo
                    ((PagePaymentModel)pageModel).BillData.BillsDetails = new List<LineData>();
                    foreach (var line in webhookRegistrationDto.BillLines)
                    {
                        var lineData = new LineData
                        {
                            Order = string.IsNullOrEmpty(line.Order) ? 0 : int.Parse(line.Order),
                            Concept = line.Concept.Length > 39 ? line.Concept.Substring(0, 36) + "..." : line.Concept,
                            Amount = (double.Parse(line.Amount) / 100)
                        };
                        ((PagePaymentModel)pageModel).BillData.BillsDetails.Add(lineData);
                    }
                }
            }
            else
            {
                //Es una Asociacion
                pageModel = new PageAssociationModel();
                var merchantIdParamsObj = await _webParameterClientService.Get();
                merchantIdParams = merchantIdParamsObj.MerchantId;
            }

            //Cargar el modelo
            pageModel.WebhookRegistrationId = webhookRegistrationDto.Id;
            pageModel.ServiceInfo = new ServiceInfo
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                ServiceContainerName = service.ServiceContainerDto != null ? service.ServiceContainerDto.Name : string.Empty,
                ImageName = !string.IsNullOrEmpty(service.ImageUrl) ? service.ImageUrl :
                    service.ServiceContainerDto != null && !string.IsNullOrEmpty(service.ServiceContainerDto.ImageUrl) ? service.ServiceContainerDto.ImageUrl :
                    null,
                IdApp = webhookRegistrationDto.IdApp,
                MerchantId = string.IsNullOrEmpty(merchantIdParams) ? webhookRegistrationDto.MerchantId : merchantIdParams,
                UrlCallback = webhookRegistrationDto.UrlCallback,
                IdUsuario = webhookRegistrationDto.IdUsuario,
                PostAssociationDesc = service.ServiceContainerDto != null ? service.ServiceContainerDto.PostAssociationDesc : service.PostAssociationDesc,
                TermsAndConditionsService = service.ServiceContainerDto != null ? service.ServiceContainerDto.TermsAndConditions : service.TermsAndConditions,
                AllowsWebservicePayments = service.ServiceContainerDto != null ? service.ServiceContainerDto.AllowWcfPayment : service.AllowWcfPayment,
                MaxQuotasAllowed = service.MaxQuotaAllow,
                ReferenceNumber1 = webhookRegistrationDto.ReferenceNumber,
                ReferenceNumber2 = webhookRegistrationDto.ReferenceNumber2,
                ReferenceNumber3 = webhookRegistrationDto.ReferenceNumber3,
                ReferenceNumber4 = webhookRegistrationDto.ReferenceNumber4,
                ReferenceNumber5 = webhookRegistrationDto.ReferenceNumber5,
                ReferenceNumber6 = webhookRegistrationDto.ReferenceNumber6,
            };
            pageModel.IdOperation = webhookRegistrationDto.IdOperation;
            pageModel.EnableEmailChange = webhookRegistrationDto.EnableEmailChange != null && !webhookRegistrationDto.EnableEmailChange.Equals("N", StringComparison.InvariantCultureIgnoreCase);
            pageModel.NewUser = (user == null);
            pageModel.UserData = new UserData
            {
                Address = user != null ? user.Address : webhookRegistrationDto.UserData.Address,
                Email = user != null ? user.Email : webhookRegistrationDto.UserData.Email,
                Name = user != null ? user.Name : webhookRegistrationDto.UserData.Name,
                Surname = user != null ? user.Surname : webhookRegistrationDto.UserData.Surname,
                CybersourceIdentifier = user != null ? user.CybersourceIdentifier : null,
                AnonymousUserId = user != null ? user.AnonymousUserId : null,
                ApplicationUserId = user != null ? user.ApplicationUserId : null,
            };

            pageModel.UserData.CardList = user != null ? user.CardList : new List<CardData>();
            pageModel.NewCard = user == null || user.CardList == null || !user.CardList.Any();

            return pageModel;
        }

        private async Task<UserData> FindRecurrentUser(WebhookRegistrationDto webhookRegistrationDto, ServiceDto service)
        {
            //Busca en tabla de VonData
            UserData userData = null;

            var vonData = await _visaNetOnIntegrationClientService.FindVonData(webhookRegistrationDto.IdApp, webhookRegistrationDto.IdUsuario);
            if (vonData != null && vonData.Any())
            {
                vonData = vonData.OrderByDescending(x => x.CreationDate).ToList();

                userData = new UserData
                {
                    AnonymousUserId = vonData.FirstOrDefault().AnonymousUserId,
                    Email = vonData.FirstOrDefault().AnonymousUserDto.Email,
                    Name = vonData.FirstOrDefault().AnonymousUserDto.Name,
                    Surname = vonData.FirstOrDefault().AnonymousUserDto.Surname,
                    Address = vonData.FirstOrDefault().AnonymousUserDto.Address,
                    CybersourceIdentifier = vonData.FirstOrDefault().AnonymousUserDto.CyberSourceIdentifier,
                    CardList = vonData.Select(x => new CardData
                    {
                        Id = new Guid(x.CardExternalId), //este Id no es de la tabla Cards, sino el ExternalId de la tabla VonData
                        Active = true,
                        MaskedNumber = x.CardMaskedNumber,
                        DueDate = x.CardDueDate,
                        Expired = new DateTime(x.CardDueDate.AddMonths(1).Year, x.CardDueDate.AddMonths(1).Month, 1) <= DateTime.Now,
                    }).ToList()
                };

                //Cargar cuotas de tarjetas
                foreach (var cardData in userData.CardList)
                {
                    cardData.Quotas = "1";
                    if (webhookRegistrationDto.Action == WebhookRegistrationActionEnumDto.Payment)
                    {
                        cardData.Quotas = await GetCardCuotas(Convert.ToInt32(cardData.MaskedNumber.Substring(0, 6)), service);
                    }
                }
            }
            return userData;
        }

        private async Task<Tuple<UserData, ActionResult>> FindAndValidateRegisteredUser(WebhookRegistrationDto webhookRegistrationDto, ServiceDto service)
        {
            //Busca en tabla de Servicios Asociados
            UserData userData = null;
            ActionResult actionResult = null;
            ServiceAssociatedDto serviceAssociated = null;

            try
            {
                serviceAssociated = await _serviceAssosiateClientService.GetServiceAssociatedDtoFromIdUserExternal(
                    webhookRegistrationDto.IdUsuario, webhookRegistrationDto.IdApp);
            }
            catch (Exception)
            {
                //Se envuelve en try-catch porque si pasan un IdUsuario que no es Guid tira excepcion, y asi devolvemos "Usuario no encontrado".
            }
            if (serviceAssociated != null)
            {
                var applicationUser = await _applicationUserClientService.GetUserWithCards(serviceAssociated.UserId);
                if (applicationUser != null)
                {
                    if (applicationUser.MembershipIdentifierObj.Blocked)
                    {
                        //Se verifica que no este bloqueado por VisaNet
                        actionResult = RedirectToAction("End", new End
                        {
                            UrlCallback = webhookRegistrationDto.UrlCallback,
                            OperationId = webhookRegistrationDto.IdOperation,
                            ResultCode = "67",
                            ResultDescription = "Usuario bloqueado por VisaNet."
                        });
                    }
                    else
                    {
                        userData = new UserData
                        {
                            ApplicationUserId = applicationUser.Id,
                            Email = applicationUser.Email,
                            Name = applicationUser.Name,
                            Surname = applicationUser.Surname,
                            Address = applicationUser.Address,
                            CybersourceIdentifier = applicationUser.CyberSourceIdentifier,
                            CardList = applicationUser.CardDtos.Where(x => x.Active && !x.Deleted).Select(x => new CardData
                            {
                                Id = x.Id,
                                Active = x.Active,
                                MaskedNumber = x.MaskedNumber,
                                DueDate = x.DueDate,
                                Expired = new DateTime(x.DueDate.AddMonths(1).Year, x.DueDate.AddMonths(1).Month, 1) <= DateTime.Now
                            }).ToList()
                        };

                        //Cargar cuotas de tarjetas
                        foreach (var cardData in userData.CardList)
                        {
                            cardData.Quotas = "1";
                            if (webhookRegistrationDto.Action == WebhookRegistrationActionEnumDto.Payment)
                            {
                                cardData.Quotas = await GetCardCuotas(Convert.ToInt32(cardData.MaskedNumber.Substring(0, 6)), service);
                            }
                        }
                    }
                }
                else
                {
                    actionResult = RedirectToAction("End", new End
                    {
                        UrlCallback = webhookRegistrationDto.UrlCallback,
                        OperationId = webhookRegistrationDto.IdOperation,
                        ResultCode = "63",
                        ResultDescription = "Usuario no encontrado."
                    });
                }
            }
            else
            {
                actionResult = RedirectToAction("End", new End
                {
                    UrlCallback = webhookRegistrationDto.UrlCallback,
                    OperationId = webhookRegistrationDto.IdOperation,
                    ResultCode = "63",
                    ResultDescription = "Usuario no encontrado."
                });
            }
            return new Tuple<UserData, ActionResult>(userData, actionResult);
        }

        private async Task<Tuple<ServiceDto, ActionResult>> FindAndValidateService(WebhookRegistrationDto webhookRegistrationDto)
        {
            ServiceDto service = null;
            ActionResult actionResult = null;

            if (webhookRegistrationDto.Bill == null || string.IsNullOrEmpty(webhookRegistrationDto.Bill.ExternalId))
            {
                //Es una ASOCIACION
                service = await _serviceClientService.GetServiceByUrlName(webhookRegistrationDto.IdApp);
                if (service == null)
                {
                    //Se verifica que exista un servicio con ese IdApp
                    actionResult = RedirectToAction("End", new End
                    {
                        UrlCallback = webhookRegistrationDto.UrlCallback,
                        OperationId = webhookRegistrationDto.IdOperation,
                        ResultCode = "56",
                        ResultDescription = "Comercio no encontrado."
                    });
                }
                else if (!service.Active || !service.AllowVon)
                {
                    //Se verifica que este habilitado
                    actionResult = RedirectToAction("End", new End
                    {
                        UrlCallback = webhookRegistrationDto.UrlCallback,
                        OperationId = webhookRegistrationDto.IdOperation,
                        ResultCode = "57",
                        ResultDescription = "Comercio no habilitado."
                    });
                }
                else if (service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
                {
                    //Se verifica version de integracion del servicio
                    actionResult = RedirectToAction("End", new End
                    {
                        UrlCallback = webhookRegistrationDto.UrlCallback,
                        OperationId = webhookRegistrationDto.IdOperation,
                        ResultCode = "75",
                        ResultDescription = "Comercio no tiene configurada la versión adecuada."
                    });
                }
            }
            else
            {
                //Es un PAGO
                var services = await _serviceClientService.GetServicesFromMerchand(webhookRegistrationDto.IdApp, webhookRegistrationDto.MerchantId, GatewayEnumDto.Apps);
                if (!services.Any())
                {
                    //Se verifica que exista un servicio con ese IdApp
                    actionResult = RedirectToAction("End", new End
                    {
                        UrlCallback = webhookRegistrationDto.UrlCallback,
                        OperationId = webhookRegistrationDto.IdOperation,
                        ResultCode = "56",
                        ResultDescription = "Comercio no encontrado."
                    });
                }
                else
                {
                    if (services.Count() > 1)
                    {
                        //Se verifica que sea unico el servicio con ese IdApp
                        actionResult = RedirectToAction("End", new End
                        {
                            UrlCallback = webhookRegistrationDto.UrlCallback,
                            OperationId = webhookRegistrationDto.IdOperation,
                            ResultCode = "58",
                            ResultDescription = "Comercio duplicado."
                        });
                    }
                    else
                    {
                        service = services.First();
                        if (service.ServiceContainerDto != null)
                        {
                            //Tiene servicio padre
                            if (!service.Active || !service.ServiceContainerDto.Active || !service.ServiceContainerDto.AllowVon)
                            {
                                //Se verifica que este habilitado
                                actionResult = RedirectToAction("End", new End
                                {
                                    UrlCallback = webhookRegistrationDto.UrlCallback,
                                    OperationId = webhookRegistrationDto.IdOperation,
                                    ResultCode = "57",
                                    ResultDescription = "Comercio no habilitado."
                                });
                            }
                            else if ((int)service.ServiceContainerDto.UrlIntegrationVersion < (int)UrlIntegrationVersionEnumDto.FourthVersion)
                            {
                                //Se verifica version de integracion del padre
                                actionResult = RedirectToAction("End", new End
                                {
                                    UrlCallback = webhookRegistrationDto.UrlCallback,
                                    OperationId = webhookRegistrationDto.IdOperation,
                                    ResultCode = "75",
                                    ResultDescription = "Comercio no tiene configurada la versión adecuada."
                                });
                            }
                        }
                        else
                        {
                            //No tiene servicio padre
                            if (!service.Active || !service.AllowVon)
                            {
                                //Se verifica que este habilitado
                                actionResult = RedirectToAction("End", new End
                                {
                                    UrlCallback = webhookRegistrationDto.UrlCallback,
                                    OperationId = webhookRegistrationDto.IdOperation,
                                    ResultCode = "57",
                                    ResultDescription = "Comercio no habilitado."
                                });
                            }
                            else if ((int)service.UrlIntegrationVersion < (int)UrlIntegrationVersionEnumDto.FourthVersion)
                            {
                                //Se verifica su version de integracion
                                actionResult = RedirectToAction("End", new End
                                {
                                    UrlCallback = webhookRegistrationDto.UrlCallback,
                                    OperationId = webhookRegistrationDto.IdOperation,
                                    ResultCode = "75",
                                    ResultDescription = "Comercio no tiene configurada la versión adecuada."
                                });
                            }
                        }
                    }
                }
            }

            return new Tuple<ServiceDto, ActionResult>(service, actionResult);
        }

        private async Task<string> GetCardCuotas(int cardBin, ServiceDto service)
        {
            var quotas = "1";
            if (service.MaxQuotaAllow > 1)
            {
                quotas = await _webCardClientService.GetQuotasForBinAndService(cardBin, service.Id);
            }
            return quotas;
        }

        private ActionResult HandleBusinessException(WebhookRegistrationDto webhookRegistrationDto, WebApiClientBusinessException e)
        {
            var resultCode = "1";
            var resultDescription = "Error general.";

            var end = new End
            {
                UrlCallback = webhookRegistrationDto.UrlCallback,
                OperationId = webhookRegistrationDto.IdOperation,
                AppId = webhookRegistrationDto.IdApp,
                WebhookRegistrationId = webhookRegistrationDto.Id,
            };

            if (e.Message == ExceptionMessages.ACCESS_TOKEN_INVALID_STATE)
            {
                resultCode = "18";
                resultDescription = ExceptionMessages.ACCESS_TOKEN_INVALID_STATE;
            }
            else if (e.Message == ExceptionMessages.WEBBHOOKREGISTRATION_ACCESSTOKEN_EXPIRED)
            {
                resultCode = "19";
                resultDescription = ExceptionMessages.WEBBHOOKREGISTRATION_ACCESSTOKEN_EXPIRED;
            }
            else if (e.Message == ExceptionMessages.ACCESS_TOKEN_EXPIRED)
            {
                resultCode = "19";
                resultDescription = ExceptionMessages.ACCESS_TOKEN_EXPIRED;
            }
            else if (e.Message == ExceptionMessages.BILL_EXPIRED)
            {
                resultCode = "20";
                resultDescription = ExceptionMessages.BILL_EXPIRED;
            }
            else if (e.Message == ExceptionMessages.BILL_ALREADY_PAID)
            {
                resultCode = "8";
                resultDescription = ExceptionMessages.BILL_ALREADY_PAID;
            }
            else if (e.Message == ExceptionMessages.ID_NOT_FOUND)
            {
                resultCode = "18";
                resultDescription = "No se encontró el token de acceso.";
            }

            end.ResultCode = resultCode;
            end.ResultDescription = resultDescription;

            NLogLogger.LogEvent(NLogType.Info, "HomeController - HandleBusinessException - " +
                "IdApp: " + webhookRegistrationDto.IdApp + " , IdOperation: " + webhookRegistrationDto.IdOperation +
                " , Code: " + resultCode + " , Description: " + resultDescription);

            return RedirectToAction("End", end);
        }

    }
}