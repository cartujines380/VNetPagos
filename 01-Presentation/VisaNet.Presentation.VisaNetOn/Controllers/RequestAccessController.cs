using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.VisaNetOn.Models;
using VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels;
using VisaNet.Utilities.DigitalSignature;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    [RoutePrefix("")]
    public class RequestAccessController : BaseController
    {
        private readonly IWebWebhookRegistrationClientService _webhookRegistrationClientService;
        private readonly IWebWebhookLogClientService _webWebhookLogClientService;
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebBillClientService _webBillClientService;

        private const int UnprocessableEntityHttpCode = 422;
        private const int InternalServerErrorHttpCode = 500;

        public RequestAccessController(IWebWebhookRegistrationClientService webhookRegistrationClientService,
            IWebWebhookLogClientService webWebhookLogClientService, IWebServiceClientService webServiceClientService, IWebBillClientService webBillClientService)
        {
            _webhookRegistrationClientService = webhookRegistrationClientService;
            _webWebhookLogClientService = webWebhookLogClientService;
            _webServiceClientService = webServiceClientService;
            _webBillClientService = webBillClientService;
        }


        [HttpPost]
        [Route("Acceso/PagoUsuarioNoRegistrado")]
        public async Task<JsonResult> PaymentFirstTime(PaymentModel model)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - Llega llamada. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var log = new WebhookRegistrationDto
                {
                    IdApp = model.IdApp,
                    IdOperation = model.IdOperacion,
                    UrlCallback = model.UrlCallback,
                    MerchantId = model.IdMerchant,
                    Action = WebhookRegistrationActionEnumDto.Payment,
                    Bill = new BillDataInputDto
                    {
                        ExternalId = model.FacturaIdentificador,
                        Amount = model.FacturaImporte,
                        Currency = model.FacturaMoneda,
                        TaxedAmount = model.FacturaImporteGravado,
                        Description = model.FacturaDescripcion,
                        FinalConsumer = model.FacturaConsFinal,
                        Quota = model.FacturaCuota,
                        GenerationDate = model.FacturaFecha,
                    },
                    UserData = new UserDataInputDto
                    {
                        Address = model.Direccion,
                        Email = model.Email,
                        IdentityNumber = model.CI,
                        MobileNumber = model.Movil,
                        Name = model.Nombre,
                        PhoneNumber = model.Telefono,
                        Surname = model.Apellido
                    },
                    EnableEmailChange = model.PermiteCambioEmail,
                    EnableRememberUser = model.PermiteRecordarUsuario,
                    ReferenceNumber = model.RefCliente1,
                    ReferenceNumber2 = model.RefCliente2,
                    ReferenceNumber3 = model.RefCliente3,
                    ReferenceNumber4 = model.RefCliente4,
                    ReferenceNumber5 = model.RefCliente5,
                    ReferenceNumber6 = model.RefCliente6,
                    FirmaDigital = model.FirmaDigital
                };
                if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
                {
                    log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto
                    {
                        Order = x.Orden,
                        Amount = x.Importe,
                        Concept = x.Concepto
                    }).ToList();
                }

                if (!string.IsNullOrEmpty(model.SendEmail))
                {
                    log.SendEmail = model.SendEmail.Equals("S", StringComparison.OrdinalIgnoreCase);
                }

                //INTENTO PERSISTIR EL WebhookRegistrationDto
                WebhookRegistrationDto logDto;
                try
                {
                    logDto = await _webWebhookLogClientService.CreateWebhookRegistration(log);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - Guardo el log. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                }
                catch (WebApiClientBusinessException e)
                {
                    return BusinessExceptionResult(e, "PaymentFirstTime", model.IdApp, model.IdOperacion);
                }
                if (logDto == null)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - logDto es null. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", model.IdOperacion });
                }

                //VALIDO DATOS ENVIADOS
                if (!ModelState.IsValid)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - No valida el modelo. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion)); string errors;
                    DataValidation.InputParametersAreValid(model, out errors);
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "2", DescResultado = errors, model.IdOperacion });
                }

                //VALIDO QUE SEA QUIEN DICE SER
                var info = await CheckInfo(ActionEnum.PaymentFirstTime, log);
                if (info != null)
                {
                    return info;
                }

                //VALIDO QUE LA FACTURA NO ESTE PAGA
                var isBillRepeated = await _webBillClientService.IsBillExlternalIdRepitedByMerchantId(logDto.Bill.ExternalId, logDto.MerchantId);
                if (isBillRepeated)
                {
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "8", DescResultado = "FACTURA YA PAGA EN EL SISTEMA", IdOperacion = logDto.IdOperation });
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - Voy a generar un token api core. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var result = await _webhookRegistrationClientService.GenerateAccessToken(logDto);
                var token = result != null && result.AccessToken != null ? result.AccessToken : string.Empty;
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentFirstTime - Obtengo el token generado por ApiCore y se devuelve al cliente. IdApp {0}, IdOperacion: {1}, AccessToken: {2}", model.IdApp, model.IdOperacion, token));

                return Json(new { CodResultado = "0", TokenAcceso = result.AccessToken, model.IdOperacion });
            }
            catch (WebApiClientBusinessException e)
            {
                return BusinessExceptionResult(e, "PaymentFirstTime", model.IdApp, model.IdOperacion);
            }
            catch (Exception e)
            {
                return UnhandledExceptionResult(e, "PaymentFirstTime", model.IdApp, model.IdOperacion);
            }
        }

        [HttpPost]
        [Route("Acceso/PagoUsuarioRecurrente")]
        public async Task<JsonResult> PaymentWithUser(PaymentWithUserModel model)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - Llega llamada. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var log = new WebhookRegistrationDto
                {
                    IdApp = model.IdApp,
                    IdOperation = model.IdOperacion,
                    IdUsuario = model.IdUsuario,
                    UrlCallback = model.UrlCallback,
                    MerchantId = model.IdMerchant,
                    Action = WebhookRegistrationActionEnumDto.Payment,
                    Bill = new BillDataInputDto
                    {
                        ExternalId = model.FacturaIdentificador,
                        Amount = model.FacturaImporte,
                        Currency = model.FacturaMoneda,
                        TaxedAmount = model.FacturaImporteGravado,
                        Description = model.FacturaDescripcion,
                        FinalConsumer = model.FacturaConsFinal,
                        Quota = model.FacturaCuota,
                        GenerationDate = model.FacturaFecha,
                    },
                    ReferenceNumber = model.RefCliente1,
                    ReferenceNumber2 = model.RefCliente2,
                    ReferenceNumber3 = model.RefCliente3,
                    ReferenceNumber4 = model.RefCliente4,
                    ReferenceNumber5 = model.RefCliente5,
                    ReferenceNumber6 = model.RefCliente6,
                    FirmaDigital = model.FirmaDigital
                };
                if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
                {
                    log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto
                    {
                        Order = x.Orden,
                        Amount = x.Importe,
                        Concept = x.Concepto
                    }).ToList();
                }
                if (!string.IsNullOrEmpty(model.SendEmail))
                {
                    log.SendEmail = model.SendEmail.Equals("S", StringComparison.OrdinalIgnoreCase);
                }

                //INTENTO PERSISTIR EL WebhookRegistrationDto
                WebhookRegistrationDto logDto;
                try
                {
                    logDto = await _webWebhookLogClientService.CreateWebhookRegistration(log);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - Guardo el log. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                }
                catch (WebApiClientBusinessException e)
                {
                    return BusinessExceptionResult(e, "PaymentWithUser", model.IdApp, model.IdOperacion);
                }
                if (logDto == null)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - logDto es null. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", model.IdOperacion });
                }

                //VALIDO DATOS ENVIADOS
                if (!ModelState.IsValid)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - No valida el modelo. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    string errors;
                    DataValidation.InputParametersAreValid(model, out errors);
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "2", DescResultado = errors, model.IdOperacion });
                }

                //VALIDO QUE SEA QUIEN DICE SER
                var info = await CheckInfo(ActionEnum.PaymentWithUser, log);
                if (info != null)
                {
                    return info;
                }

                //VALIDO QUE LA FACTURA NO ESTE PAGA
                var isBillRepeated = await _webBillClientService.IsBillExlternalIdRepitedByMerchantId(logDto.Bill.ExternalId, logDto.MerchantId);
                if (isBillRepeated)
                {
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "8", DescResultado = "FACTURA YA PAGA EN EL SISTEMA", IdOperacion = logDto.IdOperation });
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - Voy a generar un token api core. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var result = await _webhookRegistrationClientService.GenerateAccessToken(logDto);
                var token = result != null && result.AccessToken != null ? result.AccessToken : string.Empty;
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - PaymentWithUser - Obtengo el token generado por ApiCore y se devuelve al cliente. IdApp {0}, IdOperacion: {1}, AccessToken: {2}", model.IdApp, model.IdOperacion, token));

                return Json(new { CodResultado = "0", TokenAcceso = result.AccessToken, model.IdOperacion });
            }
            catch (WebApiClientBusinessException e)
            {
                return BusinessExceptionResult(e, "PaymentWithUser", model.IdApp, model.IdOperacion);
            }
            catch (Exception e)
            {
                return UnhandledExceptionResult(e, "PaymentWithUser", model.IdApp, model.IdOperacion);
            }
        }

        [HttpPost]
        [Route("Acceso/AsociacionUsuarioNuevo")]
        public async Task<JsonResult> TokenizationFirstTime(TokenizationModel model)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - Llega llamada. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var log = new WebhookRegistrationDto
                {
                    IdApp = model.IdApp,
                    IdOperation = model.IdOperacion,
                    UrlCallback = model.UrlCallback,
                    Action = WebhookRegistrationActionEnumDto.Tokenization,
                    UserData = new UserDataInputDto
                    {
                        Address = model.Direccion,
                        Email = model.Email,
                        IdentityNumber = model.CI,
                        MobileNumber = model.Movil,
                        Name = model.Nombre,
                        PhoneNumber = model.Telefono,
                        Surname = model.Apellido
                    },
                    EnableEmailChange = model.PermiteCambioEmail,
                    ReferenceNumber = model.RefCliente1,
                    ReferenceNumber2 = model.RefCliente2,
                    ReferenceNumber3 = model.RefCliente3,
                    ReferenceNumber4 = model.RefCliente4,
                    ReferenceNumber5 = model.RefCliente5,
                    ReferenceNumber6 = model.RefCliente6,
                    FirmaDigital = model.FirmaDigital
                };

                //INTENTO PERSISTIR EL WebhookRegistrationDto
                WebhookRegistrationDto logDto;
                try
                {
                    logDto = await _webWebhookLogClientService.CreateWebhookRegistration(log);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - Guardo el log. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                }
                catch (WebApiClientBusinessException e)
                {
                    return BusinessExceptionResult(e, "TokenizationFirstTime", model.IdApp, model.IdOperacion);
                }
                if (logDto == null)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - logDto es null. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", model.IdOperacion });
                }

                //VALIDO DATOS ENVIADOS
                if (!ModelState.IsValid)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - No valida el modelo. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    string errors;
                    DataValidation.InputParametersAreValid(model, out errors);
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "2", DescResultado = errors, model.IdOperacion });
                }

                //VALIDO QUE SEA QUIEN DICE SER
                var info = await CheckInfo(ActionEnum.TokenizationFirstTime, log);
                if (info != null)
                {
                    return info;
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - Voy a generar un token api core. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var result = await _webhookRegistrationClientService.GenerateAccessToken(logDto);
                var token = result != null && result.AccessToken != null ? result.AccessToken : string.Empty;
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationFirstTime - Obtengo el token generado por ApiCore y se devuelve al cliente. IdApp {0}, IdOperacion: {1}, AccessToken: {2}", model.IdApp, model.IdOperacion, token));

                return Json(new { CodResultado = "0", TokenAcceso = result.AccessToken, model.IdOperacion });
            }
            catch (WebApiClientBusinessException e)
            {
                return BusinessExceptionResult(e, "TokenizationFirstTime", model.IdApp, model.IdOperacion);
            }
            catch (Exception e)
            {
                return UnhandledExceptionResult(e, "TokenizationFirstTime", model.IdApp, model.IdOperacion);
            }
        }

        [HttpPost]
        [Route("Acceso/AsociacionUsuarioRecurrente")]
        public async Task<JsonResult> TokenizationWithUser(TokenizationWithUserModel model)
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - Llega llamada. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var log = new WebhookRegistrationDto
                {
                    IdApp = model.IdApp,
                    IdOperation = model.IdOperacion,
                    IdUsuario = model.IdUsuario,
                    UrlCallback = model.UrlCallback,
                    Action = WebhookRegistrationActionEnumDto.Tokenization,
                    ReferenceNumber = model.RefCliente1,
                    ReferenceNumber2 = model.RefCliente2,
                    ReferenceNumber3 = model.RefCliente3,
                    ReferenceNumber4 = model.RefCliente4,
                    ReferenceNumber5 = model.RefCliente5,
                    ReferenceNumber6 = model.RefCliente6,
                    FirmaDigital = model.FirmaDigital
                };

                //INTENTO PERSISTIR EL WebhookRegistrationDto
                WebhookRegistrationDto logDto;
                try
                {
                    logDto = await _webWebhookLogClientService.CreateWebhookRegistration(log);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - Guardo el log. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                }
                catch (WebApiClientBusinessException e)
                {
                    return BusinessExceptionResult(e, "TokenizationWithUser", model.IdApp, model.IdOperacion);
                }
                if (logDto == null)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - logDto es null. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", model.IdOperacion });
                }

                //VALIDO DATOS ENVIADOS
                if (!ModelState.IsValid)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - No valida el modelo. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                    string errors;
                    DataValidation.InputParametersAreValid(model, out errors);
                    Response.StatusCode = UnprocessableEntityHttpCode;
                    return Json(new { CodResultado = "2", DescResultado = errors, model.IdOperacion });
                }

                //VALIDO QUE SEA QUIEN DICE SER
                var info = await CheckInfo(ActionEnum.TokenizationWithUser, log);
                if (info != null)
                {
                    return info;
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - Voy a generar un token api core. IdApp {0}, IdOperacion: {1}", model.IdApp, model.IdOperacion));
                var result = await _webhookRegistrationClientService.GenerateAccessToken(logDto);
                var token = result != null && result.AccessToken != null ? result.AccessToken : string.Empty;
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - TokenizationWithUser - Obtengo el token generado por ApiCore y se devuelve al cliente. IdApp {0}, IdOperacion: {1}, AccessToken: {2}", model.IdApp, model.IdOperacion, token));

                return Json(new { CodResultado = "0", TokenAcceso = result.AccessToken, model.IdOperacion });
            }
            catch (WebApiClientBusinessException e)
            {
                return BusinessExceptionResult(e, "TokenizationWithUser", model.IdApp, model.IdOperacion);
            }
            catch (Exception e)
            {
                return UnhandledExceptionResult(e, "TokenizationWithUser", model.IdApp, model.IdOperacion);
            }
        }


        //Auxiliares
        private async Task<JsonResult> CheckInfo(ActionEnum action, WebhookRegistrationDto dto)
        {
            //VALIDO QUE SEA QUIEN DICE SER
            var certificateThumbprint = await _webServiceClientService.GetCertificateThumbprintIdApp(dto.IdApp);
            if (string.IsNullOrEmpty(certificateThumbprint))
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - CheckInfo - Certificado no encotrado. IdApp: {0}, IdOperacion: {1} ", dto.IdApp, dto.IdOperation));
                Response.StatusCode = UnprocessableEntityHttpCode;
                return Json(new { CodResultado = "13", DescResultado = "CERTIFICADO NO ENCONTRADO", IdOperacion = dto.IdOperation });
            }

            var paramsArray = GenerateSignatureArray(action, dto);
            var valid = DigitalSignature.CheckSignature(paramsArray, dto.FirmaDigital, certificateThumbprint);
            if (!valid)
            {
                var errorMsg = string.Format("RequestAccessController - CheckInfo - Firma invalida. IdApp: {0}, IdOperacion: {1} ", dto.IdApp, dto.IdOperation);
                NLogLogger.LogEvent(NLogType.Error, errorMsg);
                Response.StatusCode = UnprocessableEntityHttpCode;
                return Json(new { CodResultado = "12", DescResultado = "FIRMA INVALIDA", IdOperacion = dto.IdOperation });
            }

            return null;
        }

        private string[] GenerateSignatureArray(ActionEnum action, WebhookRegistrationDto dto)
        {
            switch (action)
            {
                case ActionEnum.TokenizationFirstTime:
                    return new[]
                    {
                        dto.IdApp,
                        dto.UserData != null ? dto.UserData.Email : string.Empty,
                        dto.UserData != null ? dto.UserData.Name : string.Empty,
                        dto.UserData != null ? dto.UserData.Surname : string.Empty,
                        dto.UserData != null ? dto.UserData.Address : string.Empty,
                        dto.UserData != null ? dto.UserData.PhoneNumber : string.Empty,
                        dto.UserData != null ? dto.UserData.MobileNumber : string.Empty,
                        dto.UserData != null ? dto.UserData.IdentityNumber : string.Empty,
                        dto.ReferenceNumber,
                        dto.ReferenceNumber2,
                        dto.ReferenceNumber3,
                        dto.ReferenceNumber4,
                        dto.ReferenceNumber5,
                        dto.ReferenceNumber6,
                        dto.EnableEmailChange,
                        dto.IdOperation,
                        dto.UrlCallback
                    };

                case ActionEnum.PaymentFirstTime:
                    var linescount = dto.BillLines != null && dto.BillLines.Any() ? dto.BillLines.Count * 3 : 0;
                    var array = new string[28 + linescount];
                    int i = 0;
                    array[i] = dto.IdApp; i++;
                    array[i] = dto.UserData != null ? dto.UserData.Email : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.Name : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.Surname : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.Address : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.PhoneNumber : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.MobileNumber : string.Empty; i++;
                    array[i] = dto.UserData != null ? dto.UserData.IdentityNumber : string.Empty; i++;
                    array[i] = dto.ReferenceNumber; i++;
                    array[i] = dto.ReferenceNumber2; i++;
                    array[i] = dto.ReferenceNumber3; i++;
                    array[i] = dto.ReferenceNumber4; i++;
                    array[i] = dto.ReferenceNumber5; i++;
                    array[i] = dto.ReferenceNumber6; i++;
                    array[i] = dto.EnableEmailChange; i++;
                    array[i] = dto.EnableRememberUser; i++;
                    array[i] = dto.IdOperation; i++;
                    array[i] = dto.UrlCallback; i++;
                    array[i] = dto.MerchantId; i++;
                    array[i] = dto.Bill.ExternalId; i++;
                    array[i] = dto.Bill.Amount; i++;
                    array[i] = dto.Bill.TaxedAmount; i++;
                    array[i] = dto.Bill.Currency; i++;
                    array[i] = dto.Bill.FinalConsumer; i++;
                    array[i] = dto.Bill.GenerationDate; i++;
                    array[i] = dto.Bill.Description; i++;
                    array[i] = dto.Bill.Quota; i++;
                    if (dto.BillLines != null && dto.BillLines.Any())
                    {
                        foreach (var lines in dto.BillLines.OrderBy(x => x.Order))
                        {
                            array[i] = lines.Order;
                            i++;
                            array[i] = lines.Concept;
                            i++;
                            array[i] = lines.Amount;
                            i++;
                        }
                    }
                    array[i] = dto.SendEmail != null ? dto.SendEmail.ToString() : string.Empty; i++;
                    return array;

                case ActionEnum.TokenizationWithUser:
                    return new[]
                    {
                        dto.IdApp,
                        dto.IdOperation,
                        dto.UrlCallback,
                        dto.IdUsuario,
                        dto.ReferenceNumber,
                        dto.ReferenceNumber2,
                        dto.ReferenceNumber3,
                        dto.ReferenceNumber4,
                        dto.ReferenceNumber5,
                        dto.ReferenceNumber6
                    };

                case ActionEnum.PaymentWithUser:
                    int ii = 0;
                    var linescountPaymentWithUser = dto.BillLines != null && dto.BillLines.Any() ? dto.BillLines.Count * 3 : 0;
                    var arrayPaymentWithUser = new string[21 + linescountPaymentWithUser];
                    arrayPaymentWithUser[ii] = dto.IdApp; ii++;
                    arrayPaymentWithUser[ii] = dto.IdOperation; ii++;
                    arrayPaymentWithUser[ii] = dto.UrlCallback; ii++;
                    arrayPaymentWithUser[ii] = dto.IdUsuario; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber2; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber3; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber4; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber5; ii++;
                    arrayPaymentWithUser[ii] = dto.ReferenceNumber6; ii++;
                    arrayPaymentWithUser[ii] = dto.MerchantId; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.ExternalId; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.Amount; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.TaxedAmount; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.Currency; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.FinalConsumer; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.GenerationDate; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.Description; ii++;
                    arrayPaymentWithUser[ii] = dto.Bill.Quota; ii++;
                    if (dto.BillLines != null && dto.BillLines.Any())
                    {
                        foreach (var lines in dto.BillLines.OrderBy(x => x.Order))
                        {
                            arrayPaymentWithUser[ii] = lines.Order;
                            ii++;
                            arrayPaymentWithUser[ii] = lines.Concept;
                            ii++;
                            arrayPaymentWithUser[ii] = lines.Amount;
                            ii++;
                        }
                    }
                    arrayPaymentWithUser[ii] = dto.SendEmail != null ? dto.SendEmail.ToString() : string.Empty; ii++;
                    return arrayPaymentWithUser;
            }
            return null;
        }

        private JsonResult BusinessExceptionResult(WebApiClientBusinessException e, string action, string idApp, string idOperation)
        {
            //TODO: manejar codigos de resultado y mensaje como Enums (ver de poner codigo dentro de WebApiClientBusinessException)

            NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - {0} - BusinessException. IdApp: {1}, IdOperacion: {2} ", action, idApp, idOperation));
            NLogLogger.LogEvent(e);

            Response.StatusCode = UnprocessableEntityHttpCode;
            if (e.Message == ExceptionMessages.OPERATION_ID_REPETED)
            {
                return Json(new { CodResultado = "11", DescResultado = "ID OPERACIÓN REPETIDO", IdOperacion = idOperation });
            }
            return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", IdOperacion = idOperation });
        }

        private JsonResult UnhandledExceptionResult(Exception e, string action, string idApp, string idOperation)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("RequestAccessController - {0} - Exception. IdApp: {1}, IdOperacion: {2} ", action, idApp, idOperation));
            NLogLogger.LogEvent(e);
            Response.StatusCode = InternalServerErrorHttpCode;
            return Json(new { CodResultado = "1", DescResultado = "ERROR GENERAL", IdOperacion = idOperation });
        }

    }
}