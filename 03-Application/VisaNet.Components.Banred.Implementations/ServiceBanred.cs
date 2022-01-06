using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.Banred.Implementations.BanredWsPagosBancos;
using VisaNet.Components.Banred.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Components.Banred.Implementations
{
    public class ServiceBanred : IServiceBanred
    {
        private readonly PagosBancosClient _pagosBancos;
        private readonly ILoggerService _loggerService;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IRepositoryParameters _parameterRepository;

        private const int _payment = 0;
        private const int _getBills = 1;

        public ServiceBanred(ILoggerService loggerService, IServiceFixedNotification serviceFixedNotification, IServiceEmailMessage serviceNotificationMessage, IRepositoryParameters parameterRepository)
        {
            _loggerService = loggerService;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceNotificationMessage = serviceNotificationMessage;
            _parameterRepository = parameterRepository;
            _pagosBancos = new PagosBancosClient();
        }

        public ICollection<BillBanredDto> ConsultaFacturas(string idAgenteExterno, string codigoEnte,
            string[] codigoCuentaEnte)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls;
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                string textoResultado;
                int cantFacturas;
                string listaFacturas;

                NLogLogger.LogEvent(NLogType.Info, string.Format("Banred - Intento de consulta por facturas. Ente: {0}, Cuenta: {1}", codigoEnte, codigoCuentaEnte.Length > 0 ? codigoCuentaEnte[0] : ""));
                var result = _pagosBancos.consultaFacturasEnte(idAgenteExterno, codigoEnte, codigoCuentaEnte[0], out textoResultado, out cantFacturas, out listaFacturas);
                NLogLogger.LogEvent(NLogType.Info, string.Format("Banred - Intento de consulta por facturas realizado. Ente: {0}, Cuenta: {1}, Texto resultado: {2}, Cantidad facturas: {3}", codigoEnte, codigoCuentaEnte[0], textoResultado, cantFacturas));

                var billList = new List<BillBanredDto>();

                if (result != 0)
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Banred - Consulta por facturas. Ente: {0}, Cuenta: {1}, Texto resultado: {2}, Cantidad facturas: {3}", codigoEnte, codigoCuentaEnte[0], textoResultado, cantFacturas));
                    NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), textoResultado, null);
                    var msg = CheckMsg(codigoEnte, textoResultado, result);
                    throw new BillException(msg);
                }

                if (cantFacturas == 0)
                {
                    return billList;
                }

                var bills = listaFacturas.Split('|');
                int i = 0;
                foreach (var bill in bills)
                {
                    var properties = bill.Split(',');
                    //properties[0] nroFactura		
                    //properties[1] fechaVencimiento
                    //properties[2] descripcion		
                    //properties[3] montoFactura	
                    //properties[4] moneda			
                    //properties[5] pagable	
                    //          1 = La deuda se puede pagar
                    //          2 = La deuda no se puede pagar por estar vencida
                    //          3 = La deuda no se puede pagar porque existen deudas impagas anteriores ( las facturas ya vencidas deben pagarse primero) 
                    //properties[6] pagoMinimo		
                    //properties[7] formaPago
                    //        1 = Solo Total
                    //        2 = Total y Mínimo
                    //        3 = Parcial (cualquier monto entre el total y el mínimo)
                    //properties[8] consumidorfinal "S"/"N"
                    //properties[9] MontoGravado Monto sobre el cual se aplican los descuentos, si es consumidor final

                    var toAdd = new BillBanredDto
                                {
                                    Number = properties[0],
                                    ExpirationDate =
                                        new DateTime(Int32.Parse(properties[1].Substring(0, 4)),
                                        Int32.Parse(properties[1].Substring(4, 2)),
                                        Int32.Parse(properties[1].Substring(6, 2))),
                                    Description = properties[2],
                                    Amount = Double.Parse(properties[3]) / 100,
                                    Currency = properties[4].Equals("N") ? "UYU" : "USD",
                                    Payable = properties[5].Equals("1"), //&& !properties[7].Equals("3"),
                                    FinalConsumer = !String.IsNullOrEmpty(properties[8]) && properties[8].Equals("S"),
                                    TaxedAmount = !String.IsNullOrEmpty(properties[9]) ? Double.Parse(properties[9]) / 100 : 0,
                                    MinAmount = string.IsNullOrEmpty(properties[6]) ? 0 : Double.Parse(properties[6]) / 100,
                                    PaymentType = Int16.Parse(properties[7]),
                                    PayableType = properties[5],
                                };

                    NLogLogger.LogEvent(NLogType.Info, string.Format("Banred factura. Servicio {0}, Nro ref {1}, Monto {2}, Moneda {3}, Pagable {4}, Tipo {5}, Desc {6}",
                        codigoEnte, codigoCuentaEnte[0], properties[3], properties[4], properties[5], properties[7], properties[2]));

                    if (properties[5].Equals("1") && toAdd.Payable)
                    {
                        toAdd.DashboardDescription = "Pagable";
                    }
                    else if (properties[5].Equals("1") && !toAdd.Payable)
                    {
                        toAdd.DashboardDescription = "No se puede pagar";
                    }
                    else if (properties[5].Equals("2"))
                    {
                        toAdd.DashboardDescription = "No se puede pagar por estar vencida";
                    }
                    else if (properties[5].Equals("3"))
                    {
                        toAdd.DashboardDescription = "No se puede pagar porque existen deudas impagas anteriores";
                    }

                    billList.Add(toAdd);
                    i++;
                }
                return billList;
            }
            catch (TimeoutException e)
            {
                NotifyErrorGetBill(codigoEnte,
                    string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - TimeoutException");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_COMMUNICATION);
            }
            catch (FaultException e)
            {
                //Console.WriteLine("FaultException: " + e.Message + "\n" + e.StackTrace);
                NotifyErrorGetBill(codigoEnte,
                    string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - FaultException");
                NLogLogger.LogEvent(e);
                NotificationFix(codigoEnte, codigoCuentaEnte, string.Empty, 0, 0, string.Empty, string.Empty, string.Empty, e, _getBills);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_FAULT);
            }
            catch (CommunicationException e)
            {
                NotifyErrorGetBill(codigoEnte,
                    string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - CommunicationException");
                NLogLogger.LogEvent(e);
                NotificationFix(codigoEnte, codigoCuentaEnte, string.Empty, 0, 0, string.Empty, string.Empty, string.Empty, e, _getBills);
                throw new ProviderWithoutConectionException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (BillException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                NotifyErrorGetBill(codigoEnte,
                    string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
            }
        }

        public string PagarFactura(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte,
                                 string nroFactura, double montoPago, double montoDescuentoIva, string monedaPago, string fechaVencimiento,
                                 string transactionNumber)
        {
            try
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Banred,
                    string.Format(LogStrings.Payment_NotifyBanred, transactionNumber, nroFactura));

                monedaPago = monedaPago.Equals("UYU") ? "N" : "D";

                var montoBanred = montoPago * 100 + "";
                var montoDescuento = montoDescuentoIva * 100 + "";
                var reference = codigoCuentaEnte.Length > 0 ? codigoCuentaEnte[0] : "";

                var paramsArray = new[]
                {
                    //idAgenteExterno,
                    codigoEnte,
                    reference,
                    nroFactura,
                    montoBanred,
                    //montoDescuento,
                    monedaPago,
                    fechaVencimiento,
                    transactionNumber
                };

                string firmaDigital = BanredDigitalSignature.GenerateSignature(paramsArray);

                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                string textoResultado;
                string nroTransaccion;
                string fechaHoraPago;

                NLogLogger.LogEvent(NLogType.Info, string.Format("Banred - Intento de pago de facturas. Ente: {0}, Cuenta: {1} , Nro Factura {2}, Visa Transaction Number {3}",
                    codigoEnte, reference, nroFactura, transactionNumber));

                /*El parámetro del WS "idOperacionAgente" es un valor único de la trasnacción. Se indica como el nro de transacción enviado por CyberSource (param: transactionNumber)*/
                var result = _pagosBancos.pagarFacturaEnte(idAgenteExterno, codigoEnte, codigoCuentaEnte[0], nroFactura, montoBanred, montoDescuento,
                    monedaPago, fechaVencimiento, transactionNumber, firmaDigital, out textoResultado, out nroTransaccion, out fechaHoraPago);

                NLogLogger.LogEvent(NLogType.Info, string.Format("Banred - Intento de pago de facturas realizado. Ente: {0}, Cuenta: {1}, Texto resultado: {2}, Nro Transaccion: {3}",
                    codigoEnte, reference, textoResultado, nroTransaccion));

                if (result != 0)
                {
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Banred,
                        string.Format(LogStrings.Payment_NotifyBanred_Error, textoResultado, transactionNumber, nroFactura, montoPago, montoDescuentoIva));

                    NLogLogger.LogEvent(NLogType.Error, string.Format("Banred - Intento pago de facturas. Ente: {0}, Cuenta: {1}, Texto resultado: {2}, Nro Transaccion: {3}", codigoEnte, codigoCuentaEnte[0], textoResultado, nroTransaccion));
                    NotifyErrorPayBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), textoResultado, nroFactura, transactionNumber, null);
                    throw new ProviderFatalException(textoResultado);
                }
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Banred,
                    string.Format(LogStrings.Payment_NotifyBanred_Done, transactionNumber, nroFactura, montoPago,
                        montoDescuentoIva));
                return nroTransaccion;
            }
            catch (TimeoutException e)
            {
                NotifyErrorPayBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", nroFactura, transactionNumber, e);
                _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Banred,
                    string.Format(LogStrings.Payment_NotifyBanred_Error, CodeExceptions.BANRED_TIMEOUT,
                        transactionNumber, nroFactura));
                NLogLogger.LogEvent(NLogType.Error, "Banred - TimeoutException");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_TIMEOUT);
            }
            catch (FaultException e)
            {
                NotificationFix(codigoEnte, codigoCuentaEnte, nroFactura, montoPago, montoDescuentoIva, monedaPago, fechaVencimiento, transactionNumber, e, _payment);
                NotifyErrorPayBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", nroFactura, transactionNumber, e);
                _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Banred,
                    string.Format(LogStrings.Payment_NotifyBanred_Error, CodeExceptions.BANRED_FAULT, transactionNumber,
                        nroFactura));
                NLogLogger.LogEvent(NLogType.Error, "Banred - FaultException");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_FAULT);
            }
            catch (CommunicationException e)
            {
                NotificationFix(codigoEnte, codigoCuentaEnte, nroFactura, montoPago, montoDescuentoIva, monedaPago, fechaVencimiento, transactionNumber, e, _payment);
                NotifyErrorPayBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", nroFactura, transactionNumber, e);
                _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Banred,
                    string.Format(LogStrings.Payment_NotifyBanred_Error, CodeExceptions.BANRED_COMMUNICATION,
                        transactionNumber, nroFactura));
                NLogLogger.LogEvent(NLogType.Error, "Banred - CommunicationException");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_COMMUNICATION);
            }
            catch (Exception e)
            {
                NotifyErrorPayBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", nroFactura, transactionNumber, e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public int CheckAccount(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls;
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                string textoResultado;
                int cantFacturas;
                string listaFacturas;

                var result = _pagosBancos.consultaFacturasEnte(idAgenteExterno, codigoEnte, codigoCuentaEnte[0], out textoResultado, out cantFacturas, out listaFacturas);

                if (result != 0)
                {
                    NLogLogger.LogEvent(NLogType.Error, string.Format("Banred - No se pudo validar la cuenta" +
                        "Ente: {0}" + ", Cuenta: {1}" + ", Texto resultado: {2}",
                        codigoEnte, codigoCuentaEnte[0], textoResultado));
                    NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), textoResultado, null);
                    var msg = CheckMsg(codigoEnte, textoResultado, result);
                    throw new BillException(msg);
                }

                return 1;
            }
            catch (TimeoutException e)
            {
                NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_COMMUNICATION);
            }
            catch (FaultException e)
            {
                NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_FAULT);
            }
            catch (CommunicationException e)
            {
                NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_COMMUNICATION);
            }
            catch (Exception e)
            {
                NotifyErrorGetBill(codigoEnte, string.Join(";", codigoCuentaEnte.Where(s => !String.IsNullOrEmpty(s)).ToArray()), "", e);
                NLogLogger.LogEvent(NLogType.Error, "Banred - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
            }
        }

        public void NotifyErrorGetBill(string codigoEnte, string codigoCuentaEnte, string textoresultado, Exception exception = null)
        {
            //TODO GREYLOG ERROR DE OBTENCION DE FACTURAS

            //var parameters = _parameterRepository.AllNoTracking().First();
            //var message = "Texto resultado: " + textoresultado + ", Cod ente: " + codigoEnte + ", ref: " + codigoCuentaEnte;
            //var exceptionMessage = exception != null ? exception.Message : string.Empty;
            //var stackTrace = exception != null ? exception.StackTrace : string.Empty;

            //_serviceNotificationMessage.SendInternalErrorNotification(parameters, "Error de obtencion de factura en Banred", null, message, exceptionMessage, stackTrace, exception != null ? exception.InnerException : null);
        }

        public void NotifyErrorPayBill(string codigoEnte, string codigoCuentaEnte, string textoresultado, string nroFactura, string transactionNumber, Exception exception = null)
        {
            //TODO GREYLOG ERROR DE PAGO DE FACTURAS
            var parameters = _parameterRepository.AllNoTracking().First();
            var message = "Texto resultado: " + textoresultado + ", Cod ente: " + codigoEnte + ", ref: " + codigoCuentaEnte + ", nroFactura: " + nroFactura + ", transacción: " + transactionNumber;
            var exceptionMessage = exception != null ? exception.Message : "";
            var stackTrace = exception != null ? exception.StackTrace : "";
            var innerException = exception != null ? exception.InnerException : null;

            _serviceNotificationMessage.SendInternalErrorNotification(parameters, "Error de pago de factura en Banred", null, message, exceptionMessage, stackTrace, innerException);
        }

        private string CheckMsg(string service, string result, int cod)
        {
            if (result != null)
            {
                switch (service)
                {
                    case "ANTEL":
                        if (result.Contains("El customerAccount no es valido"))
                            return ExceptionMessages.BILL_ANTEL_ACCOUNTNOTFOUND;
                        if (result.Contains("El número de cuenta"))
                            return ExceptionMessages.BILL_ANTEL_ONLYNUMBER;
                        break;
                    case "OSE":
                        if (result.Contains("Error reportado por OSE [08- Account number format invalid]"))
                            return ExceptionMessages.BILL_OSE_ACCOUNTNOTFOUND;
                        break;
                    case "UTE":
                        if (result.Contains("Error reportado por UTE[Ref. Pago no valida]"))
                            return ExceptionMessages.BILL_UTE_ACCOUNTNOTFOUND;
                        break;
                    case "EQT23"://TCC
                        if (result.Contains("El nro. no corresponde a un cliente"))
                            return ExceptionMessages.BILL_TCC_ACCOUNTNOTFOUND;
                        break;
                }
            }

            return ExceptionMessages.BANRED_GENERAL_ERROR;
        }

        private void NotificationFix(string codigoEnte, string[] codigoCuentaEnte,
                                 string nroFactura, double montoPago, double montoDescuentoIva, string monedaPago, string fechaVencimiento,
                                 string transactionNumber, Exception exception, int method)
        {
            //TODO GREYLOG AGREGAR NOTIFICATION FIX

            //var notiDesc = "";
            //var notiDetail = "";
            //NLogLogger.LogEvent(NLogType.Info, "Metodo " + method);

            //switch (method)
            //{
            //    case _payment:
            //        notiDesc = string.Format("Error en Pago Factura - Banred.");
            //        notiDetail = string.Format("Ha ocurrido un error al intentar confirmar una factura en banred. <br />" +
            //                               "El pago pudo haber quedado registrado en banred y al usuario no se le notifico ya que se genero una excepcion durante el proceso de pago (chequear cancelación de la transacción en Cybersource).<br />" +
            //                               "Chequear que la factura no este marcada como paga en banred.<br /><br />" +
            //                               "Los datos de la consulta son: <br/> " +
            //                               "    Ente: {0}<br />" +
            //                               "    Cuenta: {1}<br />" +
            //                               "    Nro transaccion: {2}<br />" +
            //                               "Datos de la factura:<br />" +
            //                               "    BillExternalId: {3}<br />" +
            //                               "    Currency: {4}<br />" +
            //                               "    Amount: {5}<br />" +
            //                               "    DiscountAmount: {6}<br />" +
            //                               "    ExpirationDate: {7}<br />" +
            //                               "{8}<br /><br />",
            //                               codigoEnte, codigoCuentaEnte[0], transactionNumber, nroFactura,
            //                               monedaPago, montoPago, montoDescuentoIva, fechaVencimiento,
            //                               _serviceFixedNotification.ExceptionMsg(exception));

            //        break;
            //    case _getBills:
            //        notiDesc = string.Format("Error en Obtencion Factura - Banred.");
            //        notiDetail = string.Format("Ha ocurrido un error al intentar obtener una factura en banred. <br />" +
            //                               "Los datos de la consulta son: <br/> " +
            //                               "    Ente: {0}<br />" +
            //                               "    Cuenta: {1}<br />" +
            //                               "{2}<br /><br />",
            //                               codigoEnte, codigoCuentaEnte[0],
            //                               _serviceFixedNotification.ExceptionMsg(exception));
            //        break;
            //}

            //if (string.IsNullOrEmpty(notiDesc)) return;

            //_serviceFixedNotification.Create(new FixedNotificationDto()
            //{
            //    Category = FixedNotificationCategoryDto.GatewayError,
            //    DateTime = DateTime.Now,
            //    Level = FixedNotificationLevelDto.Error,
            //    Description = notiDesc,
            //    Detail = notiDetail
            //});
        }
    }
}
