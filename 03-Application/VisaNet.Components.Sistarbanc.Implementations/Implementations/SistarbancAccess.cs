using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.Sistarbanc.Implementations.WsConsultasLIF;
using VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF;
using VisaNet.Components.Sistarbanc.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Components.Sistarbanc.Implementations.Implementations
{
    public class SistarbancAccess : ISistarbancAccess
    {

        private readonly IServiceServiceAssosiate _serviceAssosiate;
        private readonly ILoggerService _loggerService;
        private readonly IServiceFixedNotification _serviceFixedNotification;

        public SistarbancAccess(IServiceServiceAssosiate serviceAssosiate, ILoggerService loggerService, IServiceFixedNotification serviceFixedNotification)
        {
            _serviceAssosiate = serviceAssosiate;
            _loggerService = loggerService;
            _serviceFixedNotification = serviceFixedNotification;
        }

        /// <summary>
        /// Da de alta un usuario. Se debe ejecutar si es la primera ves que registra un pago
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idClienteBanco">Identidicador del cliente ante visanet</param>
        /// <param name="name">Nombre cliente</param>
        /// <param name="surname">Apellido cliente</param>
        /// <param name="transactionNumber">Numero de transaccion</param>
        public void AltaUsuario(string idBanco, string idClienteBanco, string name, string surname, string transactionNumber)
        {
            try
            {
                //ESTO SE HACE PORQUE NO ACEPTA CARACTERES ESPECIALES
                name = "a";
                surname = "a";
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_NewUser, name, surname, transactionNumber));
                var sistarbancService = new WsAltasBajasImpService();

                var paramsArray = new[]
            {
                idBanco,
                idClienteBanco,
                name,
                surname,
            };

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsArray);

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de alta de cliente. BancoVisa: {0}, idClienteBanco: {1}, name: {2}, surname: {3}", idBanco, idClienteBanco, name, surname));
                var result = sistarbancService.altaCliente(idBanco, idClienteBanco, name, surname, signature);
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento realizado de alta de cliente. BancoVisa: {0}, idClienteBanco: {1}, name: {2}, surname: {3}, " +
                    "codigo resultado: {4}, codigo desc: {5}", idBanco, idClienteBanco, name, surname,
                    result != null ? result.codigoError.ToString() : "", result != null ? result.descripcionError : ""));

                if (result == null)
                {
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_ErrorNewUser,
                        CodeExceptions.SISTARBANC_NORESPONSE, transactionNumber, name, surname));
                    NLogLogger.LogEvent(new Exception(CodeExceptions.SISTARBANC_NORESPONSE));
                    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
                }
                if (result.codigoError == 0)
                {
                    _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbancUser_Ok,
                        idClienteBanco, idBanco, name, surname, transactionNumber));
                    return;
                }
                if (result.codigoError == 15)
                {
                    //EL USUARIO YA ESTA REGISTADO.
                    NLogLogger.LogEvent(NLogType.Info, "Sistarbanc Info - Metodo AltaCliente");
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Usuario ya registrado, id banco: {0}, id cliente banco: {1}, nombre: {2}, apellido: {3}", idBanco, idClienteBanco, name, surname));
                    _loggerService.CreateLog(LogType.Alert, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_UserAlreadyCreated,
                        name, surname, transactionNumber));
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Error - Metodo AltaCliente");
                    NLogLogger.LogEvent(NLogType.Error, result.codigoError + " - " + result.descripcionError);
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, transactionNumber, name, surname));
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo AltaCliente");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        /// <summary>
        /// Da de baja un usuario.
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idClienteBanco">Identidicador del cliente ante visanet</param>
        /// <param name="transactionNumber">Identidicador transaccion</param>
        public void BajaUsuario(string idBanco, string idClienteBanco, string transactionNumber)
        {
            try
            {
                var sistarbancService = new WsAltasBajasImpService();

                var paramsArray = new[]
            {
                idBanco,
                idClienteBanco
            };

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsArray);

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de baja de cliente. BancoVisa: {0}, idClienteBanco: {1}", idBanco, idClienteBanco));
                var result = sistarbancService.bajaCliente(idBanco, idClienteBanco, signature);
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de baja de cliente. BancoVisa: {0}, idClienteBanco: {1}, codigo resultado: {2}, codigo desc: {3}",
                    idBanco, idClienteBanco, result != null ? result.codigoError.ToString() : "", result != null ? result.descripcionError : ""));


                if (result == null)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Sistarbanc Error - Metodo Bajacliente");
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_ErrorNewUser,
                        CodeExceptions.SISTARBANC_NORESPONSE, transactionNumber, "", ""));
                    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
                }

                if (result.codigoError != 0)
                {
                    NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Error - Metodo Bajacliente");
                    NLogLogger.LogEvent(NLogType.Error, result.codigoError + " - " + result.descripcionError);
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, transactionNumber, "", ""));
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo Bajacliente");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        /// <summary>
        /// Devuelve listado de facturas impagas para un servicio dado
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idOrganismo">Identificador del organismo emisor de las facturas </param>
        /// <param name="tipoServicio">Identificador del tipo del servicio</param>
        /// <param name="refServicio">Array con los parametros de referencia para el servicio</param>
        /// <returns></returns>
        public IEnumerable<BillSistarbancDto> GetBills(string idBancoVisa, string idBancoBrou, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            try
            {
                var service = new WsConsultasLIFClient();
                var refServicioToSend = refServicio.Where(i => i != null).ToArray();

                var paramsList = new List<String> { idBancoVisa, idOrganismo, tipoServicio };
                paramsList.AddRange(refServicioToSend);

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsList.ToArray());

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de consulta por facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Ref Servicio: {3}", idBancoVisa, idOrganismo, tipoServicio, refServicioToSend.Length > 0 ? refServicioToSend[0] : ""));
                var result = service.getServicioImpagoLIF(idBancoVisa, idOrganismo, tipoServicio, refServicioToSend, signature);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de consulta por facturas realizado. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Ref Servicio: {3}, Nro de Facturas {4}, Codigo Error: {5}," +
                                                                 "Error Desc {6}: ", idBancoVisa, idOrganismo, tipoServicio, refServicioToSend.Length > 0 ? refServicioToSend[0] : "",
                                                                 result != null ? (result.servicio != null ? (result.servicio.facturas != null ? result.servicio.facturas.Length.ToString() : "0") : "0") : "0",
                                                                 result != null ? result.codigoError : 0, result != null ? result.descripcionError : ""));

                if (result == null)
                {
                    NLogLogger.LogEvent(NLogType.Error, "Sistarbanc - Consulta por facturas. Error al obtener factura. Resultado = null");
                    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
                }

                if (result.codigoError == 11 || result.codigoError == 29) //11 no hay facturas impagas //29 factura esta paga
                {
                    return new List<BillSistarbancDto>();
                }

                if (result.codigoError != 0)
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Consulta por facturas.Error al obtener factura." +
                                                                      " Id banco: {0}, Ente: {1}, Cuenta: {2}, Cod error: {3}, Desc: {4}",
                       idBancoVisa, idOrganismo, String.Join("_", refServicioToSend.Length > 0 ? refServicioToSend[0] : ""), result.codigoError, result.descripcionError));

                    throw new BillException(GetCodeExceptions(result.codigoError, result.descripcionError, "", "", ""));
                }

                var listadoBrou = ServicioImpagoLif(idBancoBrou, idOrganismo, tipoServicio, refServicioToSend);

                var listado = (from factura in result.servicio.facturas
                               let year = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Year : int.Parse(factura.fechaVencimiento.Substring(0, 4))
                               let month = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Month : (int.Parse(factura.fechaVencimiento.Substring(4, 2)))
                               let day = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Day : (int.Parse(factura.fechaVencimiento.Substring(6, 2)))

                               let limitYear = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Year : int.Parse(factura.fechaLimitePago.Substring(0, 4))
                               let limitMonth = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Month : (int.Parse(factura.fechaLimitePago.Substring(4, 2)))
                               let limitDay = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Day : (int.Parse(factura.fechaLimitePago.Substring(6, 2)))
                               let limitDate = (new DateTime(limitYear, limitMonth, limitDay))

                               select new BillSistarbancDto()
                                      {
                                          Amount = factura.importeFactura,
                                          Currency = factura.moneda,
                                          ExpirationDate = new DateTime(year, month, day),
                                          Description = factura.descripcion,
                                          BillExternalId = factura.idFactura,
                                          DateInit = factura.fecha,
                                          IdTransaccionSTB = factura.idTransaccion, //factura.idTransaccion, //ESTO ES LA TRANSACCION SI TENGO QUE PAGAR CON TARJETA BRANCO NO BROU
                                          IdTransaccionStbBrou = listadoBrou.First(b => b.BillExternalId == factura.idFactura).IdTransaccionSTB, //ESTO ES LA TRANSACCION SI TENGO QUE PAGAR CON TARJETA BRANCO BROU
                                          Precedence = factura.precedencia,
                                          //Todas las q tengan precedencia 1 (1 es el valor minimo) y ademas no se supero la fecha limite se pueden pagar
                                          Payable = factura.precedencia == 1 && DateTime.Today <= limitDate,
                                          FinalConsumer = factura.consumidorFinal,
                                          TaxedAmount = double.Parse(factura.importeGravado),
                                          DashboardDescription = factura.precedencia == 1 ? DateTime.Today <= limitDate ? "Pagable" :
                                                    "Se superó la fecha límite de pago (" + limitDate.ToString("dd/MM/yyyy") + ")" :
                                                    "No se puede pagar porque existen deudas impagas anteriores"
                                      }).ToList();

                return listado;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo ServicioImpagoNroCuenta");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        /// <summary>
        /// Marca como pagado las facturas enviadas en billsIds con sistarbanc
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idOrganismo">Identificador del organismo emisor de las facturas </param>
        /// <param name="tipoServicio">Identificador del tipo del servicio</param>
        /// <param name="refServicio">Array con los parametros de referencia para el servicio</param>
        /// <param name="billsIds">Array con los ids de las facturas a pagar</param>
        /// <param name="idClienteBanco">Identidicador del clietnte</param>
        /// <param name="nroTrasnferenciaVisa">Nro de transferencia para visanet</param>
        /// <param name="automaticPaymentDto"></param>
        /// <param name="usertype"></param>
        public BillDto PagoReciboLif(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, BillDto bill, string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto)
        {
            try
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc, nroTrasnferenciaVisa));
                var sistarbancService = new WsPagosLIFClient();

                var amount = ((bill.Amount - bill.DiscountAmount) * 100).ToString();
                var paramsArray = new[]{
                                idClienteBanco, bill.BillExternalId, tipoServicio, idOrganismo, idBanco, nroTrasnferenciaVisa, amount, bill.Currency, "N",
                                bill.GatewayTransactionId, bill.DateInitTransaccion, "B" };

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsArray);

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento pago de facturas. BancoVisa: {0}, IdClienteBanco {1}, Organismo: {2}, Tipo Servicio: {3}, Nro Factura: {4}", idBanco, idClienteBanco, idOrganismo, tipoServicio, bill.BillExternalId));

                var result = sistarbancService.pagarRecibo(idClienteBanco, bill.BillExternalId, tipoServicio, idOrganismo, idBanco, nroTrasnferenciaVisa, amount + "", bill.Currency, "N", bill.GatewayTransactionId, bill.DateInitTransaccion, signature, "B");

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Se realizo intento pago de factura. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Nro Factura: {3}, " +
                                                                 "Codigo Error: {4}, Error Desc {5}: ",
                                                                 idClienteBanco, idOrganismo, tipoServicio, bill.BillExternalId, result != null ? result.codigoError : 0,
                                                                 result != null ? result.descripcionError : ""));
                if (result == null)
                {
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Error, CodeExceptions.SISTARBANC_NORESPONSE, nroTrasnferenciaVisa, bill.BillExternalId));
                    NLogLogger.LogEvent(new Exception(CodeExceptions.SISTARBANC_NORESPONSE));
                    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
                }
                if (result.codigoError == 29)//FACTURA YA ESTA PAGA
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Intento pago de facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Nro Factura: {3}, " +
                    "Codigo Error: {4}, Error Desc {5}: ", idClienteBanco, idOrganismo, tipoServicio, bill.BillExternalId, result.codigoError, result.descripcionError));
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, nroTrasnferenciaVisa, "", "", true));
                }
                if (result.codigoError == 57)//PAGO NO RESPETA PRECEDENCIA
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Intento pago de facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Nro Factura: {3}, " +
                    "Codigo Error: {4}, Error Desc {5}: ", idClienteBanco, idOrganismo, tipoServicio, bill.BillExternalId, result.codigoError, result.descripcionError));
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, nroTrasnferenciaVisa, "", "", true));
                }
                if (result.codigoError != 0)
                {
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Error, result.descripcionError, nroTrasnferenciaVisa, bill.BillExternalId));
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Intento pago de facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Nro Factura: {3}, " +
                    "Codigo Error: {4}, Error Desc {5}: ", idClienteBanco, idOrganismo, tipoServicio, bill.BillExternalId, result.codigoError, result.descripcionError));
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, nroTrasnferenciaVisa, "", "", true));
                }

                if (automaticPaymentDto != null)
                {
                    automaticPaymentDto.QuotasDone = automaticPaymentDto.QuotasDone + 1;
                    _serviceAssosiate.AutomaticPaymentAddQuotasDone(automaticPaymentDto);

                }
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Done, nroTrasnferenciaVisa, bill.BillExternalId, idBanco, idClienteBanco));

                return bill;
            }
            catch (TimeoutException exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc TimeoutException - Metodo PagoRecibo");
                NLogLogger.LogEvent(exception);
                NotificationFix(bill, exception);
                throw;
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("The request channel timed out while waiting for a reply "))
                {
                    //NotificationFix(bill, exception);
                }
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo PagoRecibo");
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        private string GetCodeExceptions(int codError, string desc, string transactionNumber, string name, string surename, bool payment = false)
        {
            _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc,
                string.Format(LogStrings.Sistarbanc_Error_Desc, codError, desc, transactionNumber));

            switch (codError)
            {
                case 2:
                    return ExceptionMessages.BILL_ACCOUNT_NOT_FOUND;
                case 9:
                case 999:
                    return ExceptionMessages.BILL_SERVICE_DISABLE;
                case 29:
                    return ExceptionMessages.SISTARBANC_NROERROR_29;
                case 50:
                case 101:
                    return ExceptionMessages.SISTARBANC_NROERROR_50;
                case 55:
                    return ExceptionMessages.BILL_NOT_PAYABLE;
                case 57:
                    return ExceptionMessages.SISTARBANC_NROERROR_57;
            }
            return payment
                ? ExceptionMessages.GENERAL_COMUNICATION_ERROR_PAYMENT
                : ExceptionMessages.GENERAL_COMUNICATION_ERROR_BILL;
        }

        public IEnumerable<ConciliationSistarbancDto> GetConciliation(DateTime from, DateTime to)
        {
            //    try
            //    {
            //        //METODO PARA CUANDO SE OBTENGA DESDE UN WEB SERVICE
            //        var list = new List<ConciliationSistarbancDto>
            //    {
            //        new ConciliationSistarbancDto
            //        {
            //            Id = Guid.NewGuid(),
            //            Date = DateTime.Now,
            //            IdTransaccionSTB = "234567",
            //            VisaTransactionId = 200,
            //            SistarbancUserId = 1000,
            //            BillExternalId = "4444",
            //            Currency = "UYU",
            //            Amount = 5000
            //        },
            //        new ConciliationSistarbancDto
            //        {
            //            Id = Guid.NewGuid(),
            //            Date = DateTime.Now,
            //            IdTransaccionSTB = "345678",
            //            VisaTransactionId = 201,
            //            SistarbancUserId = 1001,
            //            BillExternalId = "5555",
            //            Currency = "UYU",
            //            Amount = 6000
            //        }
            //    };

            //        if (from != default(DateTime))
            //            list = list.Where(l => l.Date > from).ToList();

            //        if (to != default(DateTime))
            //            list = list.Where(l => l.Date < to).ToList();

            //        return list;
            //    }
            //    catch (Exception e)
            //    {
            //        NLogLogger.LogEvent(e);
            //        throw;
            //    }
            return null;
        }

        public int CheckAccount(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            try
            {
                var sistarbancService = new WsConsultasLIFClient();

                var refServicioToSend = refServicio.Where(i => i != null).ToArray();

                var paramsList = new List<String> { idBanco, idOrganismo, tipoServicio };
                paramsList.AddRange(refServicioToSend);

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsList.ToArray());

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc CONSULTA CheckAccount getServicioImpagoLIF - idBanco: {0}, idOrganismo: {1}, tipoServicio: {2}, refServicioToSend: {3}",
                    idBanco, idOrganismo, tipoServicio, String.Join("_", refServicioToSend)));
                var result = sistarbancService.getServicioImpagoLIF(idBanco, idOrganismo, tipoServicio, refServicioToSend, signature);

                if (result != null && ((result.servicio != null && result.servicio.facturas != null) || (result.codigoError == 11 || result.codigoError == 29))) //11 no hay facturas impagas //29 factura esta paga
                    return 1;

                NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Metodo CheckAccount - No se pudo chequear cuenta " +
                       "Id banco: {0}" + ", Ente: {1}" + ", Cuenta: {2}" + ", Cod error: {3}" + ", Desc: {4}",
                      idBanco, idOrganismo, String.Join("_", refServicioToSend), result.codigoError, result.descripcionError));

                if (result.codigoError != 0)
                {
                    throw new BillException(GetCodeExceptions(result.codigoError, result.descripcionError, "", "", ""));
                }

                return -1;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo CheckAccount");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public IEnumerable<BillSistarbancDto> ServicioImpagoLif(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            try
            {
                //var sistarbancService = new WsConsultasImpClient();

                var service = new WsConsultasLIFClient();
                var refServicioToSend = refServicio.Where(i => i != null).ToArray();

                var paramsList = new List<String> { idBanco, idOrganismo, tipoServicio };
                paramsList.AddRange(refServicioToSend);

                string signature = SistarbancDigitalSignature.GenerateSignature(paramsList.ToArray());
                string firma = string.Empty;

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                //var result = sistarbancService.getServicioImpago(idBanco, idOrganismo, tipoServicio, refServicioToSend, signature);
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Intento de consulta por facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Ref Servicio: {3}", idBanco, idOrganismo, tipoServicio, refServicioToSend.Length > 0 ? refServicioToSend[0] : ""));
                var result = service.getServicioImpagoLIF(idBanco, idOrganismo, tipoServicio, refServicioToSend, signature);
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sistarbanc - Consulta por facturas. BancoVisa: {0}, Organismo: {1}, Tipo Servicio: {2}, Ref Servicio: {3}, Nro de Facturas {4}, Codigo Error: {5}," +
                                                 "Error Desc {6}: ", idBanco, idOrganismo, tipoServicio, refServicioToSend.Length > 0 ? refServicioToSend[0] : "",
                                                                 result != null ? (result.servicio != null ? (result.servicio.facturas != null ? result.servicio.facturas.Length.ToString() : "0") : "0") : "0",
                                                                 result != null ? result.codigoError : 0, result != null ? result.descripcionError : ""));

                if (result == null)
                    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
                if (result.codigoError == 11 || result.codigoError == 29) //11 no hay facturas impagas //29 factura esta paga
                {
                    return new List<BillSistarbancDto>();
                }
                if (result.codigoError != 0)
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Sistarbanc - Metodo ServicioImpagoLif - No Error al obtener factura. " +
                      "Id banco: {0}" + ", Ente: {1}" + ", Cuenta: {2}" + ", Cod error: {3}" + ", Desc: {4}",
                     idBanco, idOrganismo, String.Join("_", refServicioToSend.Length > 0 ? refServicioToSend[0] : ""), result.codigoError, result.descripcionError));
                    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, result.descripcionError, "", "", ""));
                }

                var listado = (from factura in result.servicio.facturas
                               let year = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Year : int.Parse(factura.fechaVencimiento.Substring(0, 4))
                               let month = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Month : (int.Parse(factura.fechaVencimiento.Substring(4, 2)))
                               let day = string.IsNullOrEmpty(factura.fechaVencimiento) ? DateTime.Now.Day : (int.Parse(factura.fechaVencimiento.Substring(6, 2)))

                               let limitYear = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Year : int.Parse(factura.fechaLimitePago.Substring(0, 4))
                               let limitMonth = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Month : (int.Parse(factura.fechaLimitePago.Substring(4, 2)))
                               let limitDay = string.IsNullOrEmpty(factura.fechaLimitePago) ? DateTime.Now.Day : (int.Parse(factura.fechaLimitePago.Substring(6, 2)))
                               let limitDate = (new DateTime(limitYear, limitMonth, limitDay))

                               select new BillSistarbancDto()
                               {
                                   Amount = factura.importeFactura,
                                   Currency = factura.moneda,
                                   ExpirationDate = new DateTime(year,month,day),
                                   Description = factura.descripcion,
                                   BillExternalId = factura.idFactura,
                                   DateInit = factura.fecha,
                                   IdTransaccionSTB = factura.idTransaccion,
                                   Precedence = factura.precedencia,
                                   //Todas las q tengan precedencia 1 (1 es el valor minimo) y ademas no se supero la fecha limite se pueden pagar
                                   Payable = factura.precedencia == 1 && DateTime.Today <= limitDate,
                                   FinalConsumer = factura.consumidorFinal,
                                   TaxedAmount = double.Parse(factura.importeGravado),
                                   AmountWithDiscount = "0"
                               }).ToList();

                return listado;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sistarbanc Exception - Metodo ServicioImpagoLif");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        private void NotificationFix(BillDto bill, Exception exception)
        {
            //TODO GREYLOG ERROR DE NOTIFICATION FIX PAGO FACTURAS

            //var notiDesc = string.Format("Error en Pago Factura - Sistarbanc.");
            //var notiDetail = string.Format("Ha ocurrido un error al intentar confirmar una factura en sistarbanc. <br />" +
            //                               "El pago pudo haber quedado registrado en sistarbanc y al usuario no se le notifico ya que se genero una excepcion durante el proceso de pago (chequear cancelación de la transacción en Cybersource).<br />" +
            //                               "Se necesita hablar con sistarbanc para cancelar la transación o generar la transacción a mano en el portal y en cybersource (sin notificar a sistarbanc).<br /><br />" +
            //                               "Los datos de la consulta son: <br/> " +
            //                               "Datos de la factura:<br />" +
            //                               "    BillExternalId: {0}<br />" +
            //                               "    Currency: {1}<br />" +
            //                               "    Amount: {2}<br />" +
            //                               "    Discount: {3}<br />" +
            //                               "    DiscountAmount: {4}<br />" +
            //                               "    TaxedAmount: {5}<br />" +
            //                               "    FinalConsumer: {6}<br />" +
            //                               "    ExpirationDate: {7}<br />" +
            //                               "    GatewayTransactionBrouId: {8}<br />" +
            //                               "    GatewayTransactionId: {9}<br />" +
            //                               "    Gateway: {10}<br />" +
            //                               "    Description: {11}<br /><br />" +
            //                               "{12}<br /><br />", bill.BillExternalId, bill.Currency, bill.Amount,
            //                               bill.Discount, bill.DiscountAmount, bill.TaxedAmount, bill.FinalConsumer, bill.ExpirationDate,
            //                               bill.GatewayTransactionBrouId, bill.GatewayTransactionId, bill.Gateway, bill.Description,
            //                               _serviceFixedNotification.ExceptionMsg(exception));

            //_serviceFixedNotification.Create(new FixedNotificationDto()
            //{
            //    Category = FixedNotificationCategoryDto.GatewayError,
            //    DateTime = DateTime.Now,
            //    Level = FixedNotificationLevelDto.Error,
            //    Description = notiDesc,
            //    Detail = notiDetail
            //});
        }
        #region Codigos de Error
        //  0 - OK
        //  1 - ORGANISMO INVALIDO
        //  2 - NUMERO CUENTA INVALIDO
        //  3 - TIPO SERVICIO INVALIDO
        //  4 - NUMERO CLIENTE INVALIDO
        //  5 - TIPO SERVICIO INVALIDO PARA ORGANISMO
        //  6 - ERROR AL PAGAR FACTURA
        //  7 - CODIGO BANCO INVALIDO
        //  8 - IMPORTE INVALIDO
        //  9 - SERVICIO NO DISPONIBLE
        // 10 - CLIENTE NO TIENE PERFIL ASOCIADO
        // 11 - NO EXISTEN FACTURAS IMPAGAS
        // 12 - IMPORTE NO COINCIDE
        // 13 - MONEDA NO COINCIDE
        // 14 - NUMERO TRANSACCION INCORRECTO
        // 15 - CLIENTE YA EXISTE
        // 16 - SERVICIO YA EXISTE EN PERFIL
        // 17 - SERVICIO NO EXISTE EN PERFIL
        // 18 - PARAMETRO INCORRECTO
        // 19 - ORIGEN INVALIDO PARA PERFIL
        // 20 - PAGO PENDIENTE DE CONFIRMACION
        // 21 - PAGO PENDIENTE DE CANCELACION
        // 22 - FIRMA INVALIDA
        // 23 - IMPORTE TOTAL INVALIDO
        // 24 - ESTADO TRANSACCION INVALIDO
        // 25 - NO SE PAGARON TODAS LAS FACTURAS
        // 26 - CODIGO INCORRECTO
        // 27 - ORIGEN INCORRECTO
        // 28 - VALORES INCORRECTOS EN FACTURA
        // 29 - FACTURA YA ESTA PAGA
        // 30 - FACTURA NO ES PAGABLE
        // 31 - TRANSACCION CANCELADA
        // 32 - CICLO INVALIDO
        // 33 - TRANSACCION NO ENCONTRADA
        // 34 - REVERSO NO ACEPTADO
        // 35 - FECHA INVALIDA
        // 36 - TRANSACCION REVERSADA
        // 37 - TIPO DOCUMENTO CLIENTE INVALIDO
        // 38 - DOCUMENTO CLIENTE INVALIDO
        // 39 - CODIGO SERVICIO INVALIDO
        // 40 - CODIGO COMERCIO INVALIDO
        // 41 - ID PROVEEDOR INVALIDO
        // 42 - TRANSACCION INICIADA
        // 43 - RESERVADO
        // 44 - RESERVADO
        // 45 - RESERVADO
        // 46 - RESERVADO
        // 47 - RESERVADO
        // 48 - MODO DE OPERACION INCORRECTO
        // 49 - SERVICIO REQUIERE CLAVE
        // 50 - NO SE PUDO INVOCAR AL ORGANISMO
        // 51 - CLAVE INCORRECTA
        // 52 - FACTURA EN PROCESO DE PAGO
        // 53 - FACTURAS EN PROCESAMIENTO
        // 54 - DATOS INCORRECTOS
        // 55 - FACTURA NO ES PAGABLE POR INTERNET
        // 56 - FACTURA VENCIDA
        // 57 - PAGO NO RESPETA PRECEDENCIA
        // 58 - SELECCION DE FACTURAS INVALIDA
        // 99 - ERROR INTERNO
        //101 - BANCO NO HABILITADO
        //999 - SERVICIO MOMENTANEAMENTE NO DISPONIBLE
        #endregion
    }
}
