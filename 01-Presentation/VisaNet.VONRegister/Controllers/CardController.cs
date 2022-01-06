using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject.Infrastructure.Language;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;
using VisaNet.VONRegister.Constants;
using VisaNet.VONRegister.Models;

namespace VisaNet.VONRegister.Controllers
{
    public class CardController : BaseController
    {
        private readonly IWebCardClientService _cardService;
        private readonly IWebBinClientService _binService;
        private readonly IWebServiceAssosiateClientService _assosiateService;
        private readonly IWebCyberSourceAccessClientService _cyberSourceAccessService;
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebParameterClientService _webParameterClientService;

        public CardController(IWebCardClientService cardService, IWebBinClientService binService, IWebServiceAssosiateClientService assosiateService,
            IWebCyberSourceAccessClientService cyberSourceAccessService, IWebServiceClientService webServiceClientService, IWebParameterClientService webParameterClientService)
        {
            _cardService = cardService;
            _binService = binService;
            _assosiateService = assosiateService;
            _cyberSourceAccessService = cyberSourceAccessService;
            _webServiceClientService = webServiceClientService;
            _webParameterClientService = webParameterClientService;
        }

        [HttpGet]
        public async Task<ActionResult> Add()
        {
            ICollection<CardDto> cards = new List<CardDto>();
            var model = Session[SessionConstants.RegisterModel] as Register;
            var service = Session[SessionConstants.CurrentService] as ServiceDto;
            var user = Session[SessionConstants.CurrentSelectedUser] as ApplicationUserDto;
            var cardsList = new List<RegisterdCard>();
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("010 - (IdOperacion:({0})", Session[SessionConstants.OperationId]));

            //In case the user makes an F5 on the page
            if (model == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            if (!model.NewUser)
            {

                //If there is a user then get the users's cards
                if (user != null)
                {
                    cards = await _cardService.FindAll(new CardFilterDto
                    {
                        DueDateMonth = string.Empty,
                        DueDateYear = string.Empty,
                        MaskedNumber = null,
                        Description = null,
                        UserId = user.Id,
                        Active = true
                    });

                    var refs = new string[6];
                    var serviceAsociated = await _assosiateService.ServiceAssosiatedToUser(user.Id, service.Id, refs);

                    cardsList.AddRange((from dto in cards
                                        where dto.Active
                                        select CardConverter(dto, serviceAsociated)
                                       ).OrderBy(x => x.Expired ? 3 : x.AlreadyIn ? 2 : 1));
                }
            }

            var obj = await _webParameterClientService.Get();
            var merchantId = obj.MerchantId;

            model.TermsAndConditionsService = service.TermsAndConditions;
            model.ServiceName = service.Name;
            model.TermsAndConditionsPostConfirm = service.PostAssociationDesc;
            model.Email = model == null ? user.Email : model.Email;
            model.CardList = cardsList.OrderBy(x => x.AlreadyIn).ToList();
            model.NewCard = !cards.Any();
            model.MerchantId = merchantId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Register model)
        {
            try
            {
                var user = Session[SessionConstants.CurrentSelectedUser] as ApplicationUserDto;
                var registerModel = Session[SessionConstants.RegisterModel] as Register;
                var service = Session[SessionConstants.CurrentService] as ServiceDto;

                ICollection<CardDto> cards = new List<CardDto>();
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("010 - (IdOperacion:({0})", Session[SessionConstants.OperationId]));
                var cardsList = new List<RegisterdCard>();
                //If there is a user then get the users's cards
                if (user != null)
                {
                    cards = await _cardService.FindAll(new CardFilterDto
                    {
                        DueDateMonth = string.Empty,
                        DueDateYear = string.Empty,
                        MaskedNumber = null,
                        Description = null,
                        UserId = user.Id,
                        Active = true
                    });

                    var refs = new string[6];
                    //NO PASO POR AGREGAR REFERENCIAS
                    if (model != null)
                    {
                        refs[0] = model.Payment.Reference1Value;
                        refs[1] = model.Payment.Reference2Value;
                        refs[2] = model.Payment.Reference3Value;
                        refs[3] = model.Payment.Reference4Value;
                        refs[4] = model.Payment.Reference5Value;
                        refs[5] = model.Payment.Reference6Value;
                    }

                    var serviceAsociated = await _assosiateService.ServiceAssosiatedToUser(user.Id, service.Id, refs);

                    cardsList.AddRange(from dto in cards
                                       where dto.Active
                                       select CardConverter(dto, serviceAsociated));

                }

                model.TermsAndConditionsService = service.TermsAndConditions;
                model.ServiceName = service.Name;
                model.TermsAndConditionsPostConfirm = service.PostAssociationDesc;
                model.Email = registerModel == null ? user.Email : registerModel.Email;
                model.CardList = cardsList.OrderBy(x => x.AlreadyIn).ToList();
                model.NewCard = !cards.Any();

                var obj = await _webParameterClientService.Get();
                var merchantId = obj.MerchantId;
                model.MerchantId = merchantId;

                return View(model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("020 - IdOperacion:({0})", Session[SessionConstants.OperationId]));
                NLogLogger.LogAppsEvent(exception);
            }
            return RedirectToAction("End", "Home", new End
            {
                OperationId = Session[SessionConstants.OperationId] as string,
                UrlCallback = Session[SessionConstants.CallbackUrl] as string,
                ResultCode = "1",
                ResultDescription = "Ha ocurrido un error.",
            });
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCreditCard(Register model)
        {
            try
            {
                BinDto bin;
                var creatingANewCard = Boolean.Parse(Request["NewCard"]);
                var selectedCardId = !creatingANewCard ? Guid.Parse(Request["SelectedCard"]) : Guid.Empty;
                var cardBin = creatingANewCard ? int.Parse(Request["CardBin"]) : 0;
                var service = Session[SessionConstants.CurrentService] as ServiceDto;

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("011 - (IdOperacion:({0}) NewCard:{1} selectedCardId:{2} cardBin:{3}", Session[SessionConstants.OperationId], creatingANewCard, selectedCardId, cardBin));

                if (!creatingANewCard)
                {
                    bin = await _binService.FindByGuid(selectedCardId);
                }
                else
                {
                    bin = await _binService.Find(cardBin);
                }

                if (bin != null && !bin.Active)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, null, PresentationWebStrings.Bin_Not_Valid, PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                //si el servicio no acepta el tipo de tarjeta ingresado envío una excepción
                var isBinAssociatedToService = bin == null || (await _webServiceClientService.IsBinAssociatedToService(bin.Value, service.Id));
                if (!isBinAssociatedToService)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", PresentationWebStrings.Bin_Not_Valid_For_Service, PresentationCoreMessages.NotificationFail, NotificationType.Error));
                }

                var terms = !string.IsNullOrWhiteSpace(service.TermsAndConditions) ? service.TermsAndConditions.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) : new string[0];

                return Json(new JsonResponse(AjaxResponse.Success, terms, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(e);
                return Json(new JsonResponse(AjaxResponse.Error, null, PresentationWebStrings.InvalidData, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> AssociateCard(Register model)
        {
            var resultCode = "1";
            var resultDescription = "";

            try
            {
                var user = Session[SessionConstants.CurrentSelectedUser] as ApplicationUserDto;
                var service = Session[SessionConstants.CurrentService] as ServiceDto;
                var operationId = Session[SessionConstants.OperationId] as string;

                if (model != null && user != null && model.SelectedCard != Guid.Empty)
                {
                    NLogLogger.LogAppsEvent(NLogType.Info, string.Format("012 - (IdOperacion:({0}) Id Usuario:{1} Id Servicio:{2} CardId:{3}", Session[SessionConstants.OperationId], user.Id, service.Id, model.SelectedCard));

                    var dto = new ServiceAssociatedDto()
                    {
                        UserId = user.Id,
                        ServiceId = service.Id,
                        DefaultCardId = model.SelectedCard,
                        ReferenceNumber = model.Payment.Reference1Value,
                        ReferenceNumber2 = model.Payment.Reference2Value,
                        ReferenceNumber3 = model.Payment.Reference3Value,
                        ReferenceNumber4 = model.Payment.Reference4Value,
                        ReferenceNumber5 = model.Payment.Reference5Value,
                        ReferenceNumber6 = model.Payment.Reference6Value,
                        OperationId = operationId,
                        Enabled = true,
                        Active = true,
                    };
                    var result = await _assosiateService.AssociateServiceToUserFromCardCreated(dto);
                    if (result != null)
                    {
                        resultCode = "0";
                        resultDescription = "";
                    }
                }
                else
                {
                    resultCode = "1";
                    resultDescription = "Ha ocurrido un error.";
                }

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("017 - (IdOperacion:({0}) Id Usuario:{1} Id Servicio:{2} CardId:{3} Reference1Value:{4} ReferenceNumber2:{5} ReferenceNumber3:{6} ReferenceNumber4:{7} ReferenceNumber5:{8} ReferenceNumber6:{9}", Session[SessionConstants.OperationId], user.Id, service.Id, model.SelectedCard, model.Payment.Reference1Value, model.Payment.Reference2Value, model.Payment.Reference3Value, model.Payment.Reference4Value, model.Payment.Reference5Value, model.Payment.Reference6Value));

                var resultModel = new End
                {
                    OperationId = Session[SessionConstants.OperationId] as string,
                    UrlCallback = Session[SessionConstants.CallbackUrl] as string,
                    ResultCode = resultCode,
                    ResultDescription = resultDescription
                };

                ClearSessionVariables();
                return RedirectToAction("End", "Home", resultModel);
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(e);
            }

            return RedirectToAction("End", "Home", new End
            {
                OperationId = Session[SessionConstants.OperationId] as string,
                UrlCallback = Session[SessionConstants.CallbackUrl] as string,
                ResultCode = "1",
                ResultDescription = "Ha ocurrido un error.",
            });
        }

        [HttpGet]
        public ActionResult References()
        {
            //var model = (Register)TempData["model"];
            var model = Session[SessionConstants.RegisterModel] as Register;
            var service = Session[SessionConstants.CurrentService] as ServiceDto;

            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("013 - (IdOperacion:({0}) Id Servicio:{1}", Session[SessionConstants.OperationId], service.Id));

            model.Payment = new Payment
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

            return View(model);
        }

        [HttpPost]
        public async Task<string> LoadCyberSourceKeys(Register model)
        {
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("014 - (IdOperacion:({0})", Session[SessionConstants.OperationId]));

            var cybersourceData = await LoadKeysForCybersource(RedirectEnums.AppAdmission, Request["CardHolderName"], model);

            return RenderPartialViewToString("_CybersourceKeys", cybersourceData);
        }

        private async Task<IDictionary<string, string>> LoadKeysForCybersource(RedirectEnums redirectTo, string nameTh, Register model)
        {
            var user = Session[SessionConstants.CurrentSelectedUser] as ApplicationUserDto;
            var service = Session[SessionConstants.CurrentService] as ServiceDto;
            var operationID = Session[SessionConstants.OperationId] as string;
            var registerModel = Session[SessionConstants.RegisterModel] as Register;

            KeysInfoForToken appToken;
            if (model.NewUser)
            {
                appToken = new KeysInfoForTokenNewUser
                {
                    UserId = Guid.Empty,
                    Name = model.Name,
                    Surname = model.Surname,
                    Address = model.Address,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    MobileNumber = model.Mobile,
                    IdentityNumber = model.Identity,
                    Password = HashPassword(registerModel.Password),
                };
            }
            else
            {
                appToken = new KeysInfoForTokenRegisteredUser
                {
                    UserId = user.Id
                };
            }

            appToken.TransactionReferenceNumber = Guid.NewGuid().ToString();
            appToken.RedirectTo = redirectTo.ToString("D");
            appToken.NameTh = nameTh;
            appToken.OperationId = operationID;
            appToken.Platform = PaymentPlatformDto.Apps.ToString();
            appToken.PaymentTypeDto = PaymentTypeDto.App;
            appToken.UrlReturn = ConfigurationManager.AppSettings["URLCallBackCyberSource"];
            appToken.ServiceId = service.Id;
            appToken.ReferenceNumber1 = model.Payment.Reference1Value;
            appToken.ReferenceNumber2 = model.Payment.Reference2Value;
            appToken.ReferenceNumber3 = model.Payment.Reference3Value;
            appToken.ReferenceNumber4 = model.Payment.Reference4Value;
            appToken.ReferenceNumber5 = model.Payment.Reference5Value;
            appToken.ReferenceNumber6 = model.Payment.Reference6Value;
            appToken.FingerPrint = model.FpProfiler;

            var keys = await _cyberSourceAccessService.GenerateKeys(appToken);
            return keys;
        }

        private string HashPassword(string password)
        {
            return PasswordHash.CreatePasswordForApps(password);
        }

        private static RegisterdCard CardConverter(CardDto cardDto, ServiceAssociatedDto service)
        {
            if (cardDto != null)
            {
                var cardUsed = service != null && service.Active && service.CardDtos.Any(x => x.Id == cardDto.Id && x.Active);
                var realDueDate = cardDto.DueDate.AddMonths(1);
                var card = new RegisterdCard
                {
                    Id = cardDto.Id,
                    Active = cardDto.Active,
                    DueDate = cardDto.DueDate,
                    MaskedNumber = cardDto.MaskedNumber, 
                    Description = cardDto.Description,
                    AlreadyIn = cardUsed,
                    Expired = new DateTime(realDueDate.Year, realDueDate.Month, 1) <= DateTime.Now
                };
                return card;
            }
            return null;
        }

    }
}