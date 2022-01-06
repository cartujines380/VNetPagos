using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Testing.VisaNetOn.Models;
using VisaNet.Utilities.DigitalSignature;
using VisaNet.Utilities.JsonResponse;

namespace VisaNet.Testing.VisaNetOn.Controllers
{
    [RoutePrefix("TEST")]
    public class LoadPageController : Controller
    {
        //TESTS DE POST
        [HttpGet]
        [Route("PaginaTestToken")]
        public ActionResult A()
        {
            return View("tokenConUsuario");
        }

        [HttpGet]
        [Route("PaginaTestPago")]
        public ActionResult B()
        {
            return View("pagoConUsuario");
        }

        [HttpGet]
        [Route("PaginaTestTokenSinUsuario")]
        public ActionResult C()
        {
            return View("tokenSinUsuario");
        }

        [HttpGet]
        [Route("PaginaTestPagoSinUsuario")]
        public ActionResult D()
        {
            return View("pagoSinUsuario");
        }

        [HttpPost]
        public ActionResult PaginaTokenSinUsuarioFirma(TokenizationModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                UrlCallback = model.UrlCallback,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                UserData = new UserDataInputDto()
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
            };
            var paramsArray = GenerateSignatureArray(ActionEnum.TokenizationFirstTime, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);
            return ClientResponse(lista, "Acceso/AsociacionUsuarioNuevo");
        }

        [HttpPost]
        public ActionResult PaginaTokenConUsuarioFirma(TokenizationWithUserModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                IdUsuario = model.IdUsuario,
                UrlCallback = model.UrlCallback,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
            };
            var paramsArray = GenerateSignatureArray(ActionEnum.TokenizationWithUser, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);
            return ClientResponse(lista, "Acceso/AsociacionUsuarioRecurrente");
        }

        [HttpPost]
        public ActionResult PaginaPagoSinUsuarioFirma(PaymentModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                UrlCallback = model.UrlCallback,
                MerchantId = model.IdMerchant,
                Bill = new BillDataInputDto()
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
                UserData = new UserDataInputDto()
                {
                    Address = model.Direccion,
                    Email = model.Email,
                    IdentityNumber = model.CI,
                    MobileNumber = model.Movil,
                    Name = model.Nombre,
                    PhoneNumber = model.Telefono,
                    Surname = model.Apellido
                },
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                EnableEmailChange = model.PermiteCambioEmail,
                EnableRememberUser = model.PermiteRecordarUsuario
            };
            if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
            {
                log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto()
                {
                    Order = x.Orden,
                    Amount = x.Importe,
                    Concept = x.Concepto
                }).ToList();
            }

            var paramsArray = GenerateSignatureArray(ActionEnum.PaymentFirstTime, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);
            return ClientResponse(lista, "Acceso/PagoUsuarioNoRegistrado");
        }

        [HttpPost]
        public ActionResult PaginaPagoConUsuarioFirma(PaymentWithUserModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                IdUsuario = model.IdUsuario,
                UrlCallback = model.UrlCallback,
                MerchantId = model.IdMerchant,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                Bill = new BillDataInputDto()
                {
                    ExternalId = model.FacturaIdentificador,
                    Amount = model.FacturaImporte,
                    Currency = model.FacturaMoneda,
                    TaxedAmount = model.FacturaImporteGravado,
                    Description = model.FacturaDescripcion,
                    FinalConsumer = model.FacturaConsFinal,
                    Quota = model.FacturaCuota,
                    GenerationDate = model.FacturaFecha,
                }
            };
            if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
            {
                log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto()
                {
                    Order = x.Orden,
                    Amount = x.Importe,
                    Concept = x.Concepto
                }).ToList();
            }
            var paramsArray = GenerateSignatureArray(ActionEnum.PaymentWithUser, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);
            return ClientResponse(lista, "Acceso/PagoUsuarioRecurrente");
        }


        //TESTS DE IFRAME
        [HttpGet]
        [Route("PaginaTestTokenIframe")]
        public ActionResult E()
        {
            return View("tokenConUsuarioIframe");
        }

        [HttpGet]
        [Route("PaginaTestPagoIframe")]
        public ActionResult F()
        {
            return View("pagoConUsuarioIframe");
        }

        [HttpGet]
        [Route("PaginaTestTokenSinUsuarioIframe")]
        public ActionResult G()
        {
            return View("tokenSinUsuarioIframe");
        }

        [HttpGet]
        [Route("PaginaTestPagoSinUsuarioIframe")]
        public ActionResult H()
        {
            return View("pagoSinUsuarioIframe");
        }

        [HttpPost]
        public ActionResult PaginaTokenConUsuarioFirmaIframe(TokenizationWithUserModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                IdUsuario = model.IdUsuario,
                UrlCallback = model.UrlCallback,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
            };
            var paramsArray = GenerateSignatureArray(ActionEnum.TokenizationWithUser, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);

            var accessToken = ClientResponseTokenForIframe(lista, "Acceso/AsociacionUsuarioRecurrente");
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessModel = new AccessUrlModel
                {
                    Token = accessToken
                };
                return IframeTest(accessModel);
            }
            return null;
        }

        [HttpPost]
        public ActionResult PaginaTokenSinUsuarioFirmaIframe(TokenizationModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                UrlCallback = model.UrlCallback,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                UserData = new UserDataInputDto()
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
            };
            var paramsArray = GenerateSignatureArray(ActionEnum.TokenizationFirstTime, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);

            var accessToken = ClientResponseTokenForIframe(lista, "Acceso/AsociacionUsuarioNuevo");
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessModel = new AccessUrlModel
                {
                    Token = accessToken
                };
                return IframeTest(accessModel);
            }
            return null;
        }

        [HttpPost]
        public ActionResult PaginaPagoSinUsuarioFirmaIframe(PaymentModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                UrlCallback = model.UrlCallback,
                MerchantId = model.IdMerchant,
                Bill = new BillDataInputDto()
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
                UserData = new UserDataInputDto()
                {
                    Address = model.Direccion,
                    Email = model.Email,
                    IdentityNumber = model.CI,
                    MobileNumber = model.Movil,
                    Name = model.Nombre,
                    PhoneNumber = model.Telefono,
                    Surname = model.Apellido
                },
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                EnableEmailChange = model.PermiteCambioEmail,
                EnableRememberUser = model.PermiteRecordarUsuario
            };
            if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
            {
                log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto()
                {
                    Order = x.Orden,
                    Amount = x.Importe,
                    Concept = x.Concepto
                }).ToList();
            }

            var paramsArray = GenerateSignatureArray(ActionEnum.PaymentFirstTime, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);

            var accessToken = ClientResponseTokenForIframe(lista, "Acceso/PagoUsuarioNoRegistrado");
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessModel = new AccessUrlModel
                {
                    Token = accessToken
                };
                return IframeTest(accessModel);
            }
            return null;
        }

        [HttpPost]
        public ActionResult PaginaPagoConUsuarioFirmaIframe(PaymentWithUserModel model)
        {
            var log = new WebhookRegistrationDto()
            {
                IdApp = model.IdApp,
                IdOperation = model.IdOperacion,
                IdUsuario = model.IdUsuario,
                UrlCallback = model.UrlCallback,
                MerchantId = model.IdMerchant,
                ReferenceNumber = model.RefCliente1,
                ReferenceNumber2 = model.RefCliente2,
                ReferenceNumber3 = model.RefCliente3,
                ReferenceNumber4 = model.RefCliente4,
                ReferenceNumber5 = model.RefCliente5,
                ReferenceNumber6 = model.RefCliente6,
                Bill = new BillDataInputDto()
                {
                    ExternalId = model.FacturaIdentificador,
                    Amount = model.FacturaImporte,
                    Currency = model.FacturaMoneda,
                    TaxedAmount = model.FacturaImporteGravado,
                    Description = model.FacturaDescripcion,
                    FinalConsumer = model.FacturaConsFinal,
                    Quota = model.FacturaCuota,
                    GenerationDate = model.FacturaFecha,
                }
            };
            if (model.FacturaLineas != null && model.FacturaLineas.Length > 0)
            {
                log.BillLines = model.FacturaLineas.Select(x => new WebhookRegistrationLineDto()
                {
                    Order = x.Orden,
                    Amount = x.Importe,
                    Concept = x.Concepto
                }).ToList();
            }
            var paramsArray = GenerateSignatureArray(ActionEnum.PaymentWithUser, log);
            var thumbprint = ConfigurationManager.AppSettings["TestThumbprint"];
            var newstr = new string(thumbprint.Where(c => c < 128).ToArray());
            var siganture = DigitalSignature.GenerateSignature(paramsArray, newstr);

            var lista = new Dictionary<string, string>();
            foreach (var key in Request.Form.AllKeys)
            {
                lista.Add(key, Request.Form[key]);
            }

            lista.Add("FirmaDigital", siganture);

            var accessToken = ClientResponseTokenForIframe(lista, "Acceso/PagoUsuarioRecurrente");
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessModel = new AccessUrlModel
                {
                    Token = accessToken
                };
                return IframeTest(accessModel);
            }
            return null;
        }

        public ActionResult IframeTest(AccessUrlModel model)
        {
            return View("IframeTest", model);
        }


        //Comunes
        public ActionResult PedirAcceso(string tokenAcceso)
        {
            var posturl = ConfigurationManager.AppSettings["BaseUrlVisaNetOn"] + "Home";
            return View("PaginaVNP", new AccessUrlModel
            {
                PostUrl = posturl,
                Token = tokenAcceso,
            });
        }

        public ActionResult ErrorAcceso(string codError, string codDesc, string operationId)
        {
            return View("ErrorAcceso", new End()
            {
                ResultDescription = codDesc,
                ResultCode = codError,
                OperationId = operationId
            });
        }

        private ActionResult ClientResponse(Dictionary<string, string> lista, string action)
        {
            try
            {
                var baseUrlVisaNetOn = ConfigurationManager.AppSettings["BaseUrlVisaNetOn"];
                using (var client = new HttpClient())
                {
                    var values = lista.ToDictionary(dataAux => dataAux.Key, dataAux => dataAux.Value);
                    var content = new FormUrlEncodedContent(values);
                    var url = baseUrlVisaNetOn + action;
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var responseString = response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return Json(new JsonResponse(AjaxResponse.Success, responseString.Result, null, null), JsonRequestBehavior.AllowGet);
                    }
                    NLogLogger.LogEvent(NLogType.Info,
                        string.Format("LoadPageController - ClientResponse - Respuesta distinta a ok. Codigo {0}, Mensaje {1}",
                            response.StatusCode, responseString.Result));
                    var obj = new
                    {
                        ResponseCode = response.StatusCode,
                        ResponseMessage = responseString.Result
                    };
                    return Json(new JsonResponse(AjaxResponse.BusinessError, obj, null, null), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("LoadPageController - ClientResponse - Excepcion: {0}", e.Message));
                return Json(new JsonResponse(AjaxResponse.Error, null, null, null), JsonRequestBehavior.AllowGet);
            }

        }

        private string ClientResponseTokenForIframe(Dictionary<string, string> lista, string action)
        {
            try
            {
                var baseUrlVisaNetOn = ConfigurationManager.AppSettings["BaseUrlVisaNetOn"];
                using (var client = new HttpClient())
                {
                    var values = lista.ToDictionary(dataAux => dataAux.Key, dataAux => dataAux.Value);
                    var content = new FormUrlEncodedContent(values);
                    var url = baseUrlVisaNetOn + action;
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var responseString = response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var accessResponse = responseString.Result;
                        if (!string.IsNullOrEmpty(accessResponse))
                        {
                            var jsonResponse = JsonConvert.DeserializeObject<ResponseModel>(accessResponse);
                            if (jsonResponse.CodResultado == "0")
                            {
                                return jsonResponse.TokenAcceso;
                            }
                        }
                    }
                    NLogLogger.LogEvent(NLogType.Info,
                        string.Format("LoadPageController - ClientResponseTokenForIframe - Respuesta distinta a ok. Codigo {0}, Mensaje {1}",
                            response.StatusCode, responseString.Result));
                    var obj = new
                    {
                        ResponseCode = response.StatusCode,
                        ResponseMessage = responseString.Result
                    };
                    return null;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("LoadPageController - ClientResponseTokenForIframe - Excepcion: {0}", e.Message));
                return null;
            }
        }

        private string[] GenerateSignatureArray(ActionEnum action, WebhookRegistrationDto dto)
        {
            switch (action)
            {
                case ActionEnum.TokenizationFirstTime:
                    return new string[]
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
                        dto.UrlCallback,
                    };

                case ActionEnum.PaymentFirstTime:
                    var linescount = dto.BillLines != null && dto.BillLines.Any() ? dto.BillLines.Count * 3 : 0;
                    var array = new string[27 + linescount];
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

                    return array;
                case ActionEnum.TokenizationWithUser:
                    return new string[]
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
                        dto.ReferenceNumber6,
                    };

                case ActionEnum.PaymentWithUser:
                    int ii = 0;
                    var linescountPaymentWithUser = dto.BillLines != null && dto.BillLines.Any() ? dto.BillLines.Count * 3 : 0;
                    var arrayPaymentWithUser = new string[20 + linescountPaymentWithUser];
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
                    return arrayPaymentWithUser;
            }
            return null;
        }

    }
}