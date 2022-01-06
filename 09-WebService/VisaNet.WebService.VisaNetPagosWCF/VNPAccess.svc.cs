using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.Utilities.DigitalSignature;
using VisaNet.Utilities.Helpers;
using VisaNet.WebService.VisaNetPagosWCF.EntitiesModel;

namespace VisaNet.WebService.VisaNetPagosWCF
{
    /// <summary>
    /// Proporciona los métodos necesarios para realizar el pago de facturas, anulación de transacciónes, consulta de transacciónes y servicios asociados.
    /// </summary>
    public class VNPAccess : IVNPAccess
    {
        private IWsExternalAppClientService _wsExternalAppClientService;
        private IWsExternalClientService _wsExternalLogClientService;

        private StandardKernel _kernel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuesta CobroFacturaOnlineApp(CobrarFacturaData data)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("WCF  CobroFacturaOnlineApp - App: {0}, IdOperacion :{1}", data.IdApp, data.IdOperacion));
            WsBillPaymentOnlineDto log = null;
            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                _wsExternalLogClientService = NinjectRegister.Get<IWsExternalClientService>();
                VNPRespuesta response = null;

                string errors;

                log = new WsBillPaymentOnlineDto
                {
                    IdApp = data.IdApp,
                    IdOperation = data.IdOperacion,
                    CodCommerce = data.Factura != null && !string.IsNullOrEmpty(data.Factura.CodSucursal) ? int.Parse(data.Factura.CodComercio) : 0,
                    CodBranch = data.Factura != null && !string.IsNullOrEmpty(data.Factura.CodSucursal) ? int.Parse(data.Factura.CodSucursal) : 0,
                    IdMerchant = data.Factura != null ? data.Factura.IdMerchant : string.Empty,
                    IdUser = data.Factura != null ? data.Factura.IdUsuario : string.Empty,
                    IdCard = data.Factura != null ? data.Factura.IdTarjeta : string.Empty,
                    BillNumber = data.Factura != null ? data.Factura.NroFactura : string.Empty,
                    Description = data.Factura != null ? data.Factura.Descripcion : string.Empty,
                    DateBill = data.Factura != null ? data.Factura.FchFactura : DateTime.Now,
                    Currency = data.Factura != null ? data.Factura.Moneda : string.Empty,
                    AmountTotal = data.Factura != null ? data.Factura.MontoTotal : 0,
                    Indi = data.Factura != null ? data.Factura.Indi : -1,
                    AmountTaxed = data.Factura != null ? data.Factura.MontoGravado : 0,
                    ConsFinal = data.Factura != null && data.Factura.ConsFinal,
                    Quota = data.Factura != null ? data.Factura.Cuotas : 1,
                    Id = Guid.NewGuid(),
                    Codresult = -1,
                    DeviceFingerprint = data.Factura != null ? data.Factura.DeviceFingerprint : string.Empty,
                    CustomerIp = data.Factura != null ? data.Factura.IpCliente : string.Empty,
                    CustomerPhone = data.Factura != null ? data.Factura.TelefonoCliente : string.Empty,
                    WcfVersion = GetVersion()
                };
                if (data.Factura != null && data.Factura.DireccionEnvioCliente != null)
                {
                    log.CustomerShippingAddresDto = new CustomerShippingAddresDto
                    {
                        Street = data.Factura.DireccionEnvioCliente.Calle,
                        DoorNumber = data.Factura.DireccionEnvioCliente.NumeroPuerta,
                        Complement = data.Factura.DireccionEnvioCliente.Complemento,
                        Corner = data.Factura.DireccionEnvioCliente.Esquina,
                        PostalCode = data.Factura.DireccionEnvioCliente.CodigoPostal,
                        Latitude = data.Factura.DireccionEnvioCliente.Latitud,
                        Longitude = data.Factura.DireccionEnvioCliente.Longitud,
                        Neighborhood = data.Factura.DireccionEnvioCliente.Barrio,
                        Phone = data.Factura.DireccionEnvioCliente.Telefono,
                        City = data.Factura.DireccionEnvioCliente.Ciudad,
                        Country = data.Factura.DireccionEnvioCliente.Pais,
                    };
                }

                try
                {
                    //Se registra el WsBillPaymentOnline
                    log = _wsExternalLogClientService.CreateBillPaymentOnline(log).GetAwaiter().GetResult();
                }
                catch (WebApiClientBusinessException exception)
                {
                    var errorMsg = string.Format("Error al validar el id operación. Este ya fue utilizado. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 11, DescResultado = "ID OPERACIÓN YA UTILIZADO", IdOperacion = data.IdOperacion };
                    return response;
                }
                
                var serviceIdentified = this.CheckCodCommerceMerchant(data.Factura.CodComercio, data.Factura.CodSucursal, log.IdMerchant);
                if (!string.IsNullOrEmpty(serviceIdentified))
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, serviceIdentified);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 2, DescResultado = serviceIdentified, IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditBillPaymentOnline(log);
                    return response;
                }

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);
                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditBillPaymentOnline(log);
                    return response;
                }

                withOutErrors = DataValidation.InputParametersAreValid(data.Factura, out errors);
                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos de la factura. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditBillPaymentOnline(log);
                    return response;
                }

                if (data.Factura != null && data.Factura.DireccionEnvioCliente != null)
                {
                    withOutErrors = DataValidation.InputParametersAreValid(data.Factura.DireccionEnvioCliente, out errors);
                    if (!withOutErrors)
                    {
                        var errorMsg = string.Format("Error al validar los campos requeridos de la direccion envio cliente de la factura. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                        NLogLogger.LogEvent(NLogType.Error, errorMsg);
                        response = new VNPRespuesta { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                        log.Codresult = response.CodResultado;
                        _wsExternalLogClientService.EditBillPaymentOnline(log);
                        return response;
                    }
                }

                var info = data.Factura != null
                    ? string.Format(
                        "Datos: IdApp '{0}', IdOperacion'{1}', CodComercio'{2}', CodSucursal'{3}', IdMerchant'{15}', IdUsuario'{4}', IdTarjeta'{5}'" +
                        ", NroFactura'{6}', Descripcion'{7}', FchFactura'{8}', Moneda'{9}', MontoTotal'{10}', Indi'{11}'" +
                        ", MontoGravado'{12}', ConsFinal'{13}', Cuotas'{14}'", data.IdApp, data.IdOperacion,
                        data.Factura.CodComercio, data.Factura.CodSucursal, data.Factura.IdUsuario, data.Factura.IdTarjeta,
                        data.Factura.NroFactura, data.Factura.Descripcion, data.Factura.FchFactura, data.Factura.Moneda,
                        data.Factura.MontoTotal, data.Factura.Indi, data.Factura.MontoGravado, data.Factura.ConsFinal, data.Factura.Cuotas, data.Factura.IdMerchant)
                    : string.Format("Datos: IdApp '{0}', IdOperacion'{1}'", data.IdApp, data.IdOperacion);

                NLogLogger.LogEvent(NLogType.Info, info);

                //Se busca el certificado del Servicio
                var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto { IdApp = data.IdApp }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    response = new VNPRespuesta { CodResultado = 13, DescResultado = "CERTIFICADO NO ENCONTRADO", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditBillPaymentOnline(log);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Certificado no encotrado. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion));
                    return response;
                }

                var paramsArray = new[]
                {
                    data.IdApp,
                    string.IsNullOrEmpty(data.Factura.CodComercio) ? string.Empty : data.Factura.CodComercio,
                    string.IsNullOrEmpty(data.Factura.CodSucursal) ? string.Empty : data.Factura.CodSucursal,
                    data.Factura.IdUsuario,
                    data.Factura.IdTarjeta,
                    data.Factura.NroFactura,
                    string.IsNullOrEmpty(data.Factura.Descripcion) ? string.Empty : data.Factura.Descripcion,
                    data.Factura.FchFactura.ToString("yyyyMMdd"),
                    data.Factura.Moneda,
                    data.Factura.MontoTotal.ToString("#0.00", CultureInfo.CurrentCulture),
                    data.Factura.Indi.ToString(),
                    data.Factura.MontoGravado.ToString("#0.00", CultureInfo.CurrentCulture),
                    data.Factura.ConsFinal.ToString(),
                    data.Factura.Cuotas.ToString(),
                    data.IdOperacion,
                    string.IsNullOrEmpty(data.Factura.IdMerchant) ? string.Empty : data.Factura.IdMerchant,
                    string.IsNullOrEmpty(data.Factura.DeviceFingerprint) ? string.Empty : data.Factura.DeviceFingerprint,
                    string.IsNullOrEmpty(data.Factura.IpCliente) ? string.Empty : data.Factura.IpCliente,
                    string.IsNullOrEmpty(data.Factura.TelefonoCliente) ? string.Empty : data.Factura.TelefonoCliente
                };

                if (data.Factura.DireccionEnvioCliente != null)
                {
                    var concatenated = paramsArray.Concat(new[]
                    {
                        data.Factura.DireccionEnvioCliente.Calle,
                        data.Factura.DireccionEnvioCliente.NumeroPuerta,
                        data.Factura.DireccionEnvioCliente.Complemento,
                        data.Factura.DireccionEnvioCliente.Esquina,
                        data.Factura.DireccionEnvioCliente.Barrio,
                        data.Factura.DireccionEnvioCliente.CodigoPostal,
                        data.Factura.DireccionEnvioCliente.Latitud,
                        data.Factura.DireccionEnvioCliente.Longitud,
                        data.Factura.DireccionEnvioCliente.Telefono,
                        data.Factura.DireccionEnvioCliente.Ciudad,
                        data.Factura.DireccionEnvioCliente.Pais
                    });
                    paramsArray = concatenated.ToArray();
                }

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 12, DescResultado = "FIRMA INVALIDA", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditBillPaymentOnline(log);
                    return response;
                }

                //Se invoca a la operacion de pago
                NLogLogger.LogEvent(NLogType.Info, "WCF  CobroFacturaOnlineApp - Inicio Metodo de pagar");
                var result = _wsExternalAppClientService.MakePayment(log).GetAwaiter().GetResult();
                NLogLogger.LogEvent(NLogType.Info, "WCF  CobroFacturaOnlineApp - Fin Metodo de pagar");


                NLogLogger.LogEvent(NLogType.Info, string.Format("WCF  CobroFacturaOnlineApp - Resultado del llamado a webapi {0}, nro transaccion {1}",
                    result.OperationResult, result.CsTransactionNumber));
                return new VNPRespuesta { CodResultado = result.OperationResult, NroTransaccion = result.CsTransactionNumber, IdOperacion = data.IdOperacion };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  CobroFacturaOnlineApp - Exception " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(exception);
                var error = new VNPRespuesta { CodResultado = 1, DescResultado = "Excepcion", IdOperacion = data.IdOperacion };
                try
                {
                    if (log != null)
                    {
                        log.Codresult = error.CodResultado;
                        _wsExternalLogClientService.EditBillPaymentOnline(log);
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Info, "WCF  CobroFacturaOnlineApp - Exception " + DateTime.Now.ToString("G"));
                    NLogLogger.LogEvent(e);
                }

                return error;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuesta AnulacionCobroApp(AnularFacturaData data)
        {
            NLogLogger.LogEvent(NLogType.Info, "WCF  AnulacionCobroApp - " + DateTime.Now.ToString("G"));
            WsPaymentCancellationDto log = null;
            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                _wsExternalLogClientService = NinjectRegister.Get<IWsExternalClientService>();
                VNPRespuesta response = null;
                
                log = new WsPaymentCancellationDto
                {
                    IdApp = data.IdApp,
                    IdOperation = data.IdOperacion,
                    IdOperacionCobro = data.IdOperacionCobro,
                    Id = Guid.NewGuid(),
                    Codresult = -1,
                    WcfVersion = GetVersion()
                };

                try
                {
                    //Se registra el WsPaymentCancellation
                    log = _wsExternalLogClientService.CreatePaymentCancellation(log).GetAwaiter().GetResult();
                }
                catch (WebApiClientBusinessException exception)
                {
                    var errorMsg = string.Format("Error al validar el id operación. Este ya fue utilizado. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 11, DescResultado = "ID OPERACIÓN YA UTILIZADO", IdOperacion = data.IdOperacion };
                    return response;
                }

                string errors;

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);

                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditPaymentCancellation(log);
                    return response;
                }

                var info = string.Format("Datos: IdApp '{0}', IdOperacion'{1}', IdOperacionCobro'{2}'", data.IdApp, data.IdOperacion, data.IdOperacionCobro);

                NLogLogger.LogEvent(NLogType.Info, info);

                //Se busca el certificado del Servicio
                var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto { IdApp = data.IdApp }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    response = new VNPRespuesta { CodResultado = 13, DescResultado = "CERTIFICADO NO ENCONTRADO", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditPaymentCancellation(log);
                    return response;
                }

                var paramsArray = new[]
                {
                    data.IdApp,
                    data.IdOperacion,
                    data.IdOperacionCobro
                };

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuesta { CodResultado = 12, DescResultado = "FIRMA INVALIDA", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditPaymentCancellation(log);
                    return response;
                }

                //Se invoca a la operacion de cancelacion
                var result = _wsExternalAppClientService.CancelPayment(log).GetAwaiter().GetResult();
                return new VNPRespuesta
                {
                    CodResultado = result.OperationResult,
                    NroTransaccion = result.CsTransactionNumber,
                    IdOperacion = data.IdOperacion
                };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  AnulacionCobroApp - Exception " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(exception);
                var error = new VNPRespuesta { CodResultado = 1, DescResultado = "Excepcion", IdOperacion = data.IdOperacion };
                try
                {
                    if (log != null)
                    {
                        log.Codresult = error.CodResultado;
                        _wsExternalLogClientService.EditPaymentCancellation(log);
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Info, "WCF  AnulacionCobroApp - Exception " + DateTime.Now.ToString("G"));
                    NLogLogger.LogEvent(e);
                }

                return error;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuestaConsultaFacturas ConsultaTransacciones(ConsultaFacturasData data)
        {
            NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaTransacciones - " + DateTime.Now.ToString("G"));

            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                _wsExternalLogClientService = NinjectRegister.Get<IWsExternalClientService>();

                var info = string.Format("Datos: CodComercio '{0}', CodSucursal '{1}', IdMerchant '{11}', IdApp '{2}', Fecha '{3}', NroFactura '{4}'" + ", RefCliente1 '{5}', RefCliente2 '{6}', RefCliente3 '{7}', RefCliente4 '{8}', " +
                    "RefCliente5 '{9}'," + " RefCliente6 '{10}'.", data.CodComercio, data.CodSucursal, data.IdApp, data.Fecha, data.NroFactura, data.RefCliente, data.RefCliente2, data.RefCliente3, data.RefCliente4,
                    data.RefCliente5, data.RefCliente6, data.IdMerchant);
                NLogLogger.LogEvent(NLogType.Info, info);

                string errors;

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);
                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    return new VNPRespuestaConsultaFacturas { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                }

                var log = new WsBillQueryDto
                {
                    IdOperation = data.IdOperacion,
                    BillNumber = data.NroFactura,
                    CodBranch = string.IsNullOrEmpty(data.CodSucursal) ? 0 : int.Parse(data.CodSucursal),
                    CodCommerce = string.IsNullOrEmpty(data.CodComercio) ? 0 : int.Parse(data.CodComercio),
                    IdMerchant = data.IdMerchant,
                    RefClient = data.RefCliente,
                    RefClient2 = data.RefCliente2,
                    RefClient3 = data.RefCliente3,
                    RefClient4 = data.RefCliente4,
                    RefClient5 = data.RefCliente5,
                    RefClient6 = data.RefCliente6,
                    IdApp = data.IdApp,
                    Date = data.Fecha,
                    WcfVersion = GetVersion()
                };

                try
                {
                    //Se registra el WsBillQuery
                    log = _wsExternalLogClientService.CreateBillQuery(log).GetAwaiter().GetResult();
                }
                catch (WebApiClientBusinessException exception)
                {
                    var errorMsg = string.Format("Error al validar el id operación. Este ya fue utilizado. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    return new VNPRespuestaConsultaFacturas { CodResultado = 11, DescResultado = "ID OPERACIÓN YA UTILIZADO", IdOperacion = data.IdOperacion };
                }

                //Se busca el certificado del Servicio
                var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto { IdApp = data.IdApp }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                    return new VNPRespuestaConsultaFacturas { CodResultado = 13, DescResultado = "Certificado faltante.", IdOperacion = data.IdOperacion };

                var paramsArray = new[]
                {
                    data.IdApp,
                    data.CodComercio,
                    data.CodSucursal,
                    data.Fecha.ToString("yyyyMMdd"),
                    data.NroFactura,
                    data.RefCliente,
                    data.RefCliente2,
                    data.RefCliente3,
                    data.RefCliente4,
                    data.RefCliente5,
                    data.RefCliente6,
                    data.IdOperacion,
                    data.IdMerchant
                };

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    return new VNPRespuestaConsultaFacturas { CodResultado = 12, DescResultado = "Firma invalida.", IdOperacion = data.IdOperacion };
                }

                //Se invoca a la operacion de consulta de facturas
                var result = _wsExternalAppClientService.TransactionsHistory(log).GetAwaiter().GetResult();

                var vnp = new VNPRespuestaConsultaFacturas
                {
                    CodResultado = result.CodResultado,
                    DescResultado = result.DescResultado,
                    ResumenPagos = new ResumenPagos
                    {
                        CantFacturas = result.ResumenPagos.CantFacturas,
                        CantDolaresPagados = result.ResumenPagos.CantDolaresPagados,
                        CantPesosPagados = result.ResumenPagos.CantPesosPagados,
                        SumaDolaresPagados = result.ResumenPagos.SumaDolaresPagados,
                        SumaPesosPagados = result.ResumenPagos.SumaPesosPagados
                    },
                    EstadoFacturas = result.EstadoFacturas.Select(y => new EstadoFacturas
                    {
                        RefCliente1 = y.RefCliente1,
                        RefCliente2 = y.RefCliente2,
                        RefCliente3 = y.RefCliente3,
                        RefCliente4 = y.RefCliente4,
                        RefCliente5 = y.RefCliente5,
                        RefCliente6 = y.RefCliente6,
                        NroFactura = y.NroFactura,
                        MontoTotal = y.MontoTotal,
                        MontoDescIVA = y.MontoDescIVA,
                        Moneda = y.Moneda,
                        FchPago = y.FchPago,
                        Estado = y.Estado,
                        DescEstado = y.DescEstado,
                        CodError = y.CodError,
                        CantCuotas = y.CantCuotas,
                        AuxiliarData = y.AuxiliarData.Select(z => new AuxiliarData
                        {
                            Dato_auxiliar = z.Dato_auxiliar,
                            Id_auxiliar = z.Id_auxiliar
                        }).ToList()
                    }).ToList()
                };
                return vnp;

            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaTransacciones - Exception " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(exception);
                return new VNPRespuestaConsultaFacturas { CodResultado = 1, DescResultado = "Error general.", IdOperacion = data.IdOperacion };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuestaConsultaComercios ConsultaComercios(ConsultaComerciosData data)
        {
            NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaComercios - " + DateTime.Now.ToString("G"));
            WsCommerceQueryDto log = null;
            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                _wsExternalLogClientService = NinjectRegister.Get<IWsExternalClientService>();
                VNPRespuestaConsultaComercios response = null;
                
                log = new WsCommerceQueryDto
                {
                    IdApp = data.IdApp,
                    IdOperation = data.IdOperacion,
                    Id = Guid.NewGuid(),
                    Codresult = response != null ? response.CodResultado : -1,
                    WcfVersion = GetVersion()
                };
                
                try
                {
                    //Se registra el WsCommerceQueryDto
                    log = _wsExternalLogClientService.CreateCommerceQuery(log).GetAwaiter().GetResult(); ;
                }
                catch (WebApiClientBusinessException exception)
                {
                    var errorMsg = string.Format("Error al validar el id operación. Este ya fue utilizado. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaConsultaComercios { CodResultado = 11, DescResultado = "ID OPERACIÓN YA UTILIZADO", IdOperacion = data.IdOperacion };
                    return response;
                }

                var errors = "";

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);
                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaConsultaComercios { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCommerceQuery(log);
                    return response;
                }

                var info = string.Format("Datos: IdApp '{0}', IdOperacion'{1}'", data.IdApp, data.IdOperacion);

                NLogLogger.LogEvent(NLogType.Info, info);

                //Se busca el certificado del Servicio
                var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto { IdApp = data.IdApp }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    response = new VNPRespuestaConsultaComercios { CodResultado = 13, DescResultado = "CERTIFICADO NO ENCONTRADO", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCommerceQuery(log);
                    return response;
                }

                var paramsArray = new[]
                {
                    data.IdApp,
                    data.IdOperacion
                };

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaConsultaComercios { CodResultado = 12, DescResultado = "FIRMA INVALIDA", IdOperacion = data.IdOperacion };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCommerceQuery(log);
                    return response;
                }

                //Se invoca a la operacion de consulta de comercios
                var result = _wsExternalAppClientService.GetServices(log).GetAwaiter().GetResult();
                log.Codresult = result.OperationResult;
                _wsExternalLogClientService.EditCommerceQuery(log);

                return new VNPRespuestaConsultaComercios
                {
                    CodResultado = result.OperationResult,
                    IdOperacion = data.IdOperacion,
                    Comercios = result.Commerces.Select(x => new Comercio
                    {
                        Active = x.Active,
                        Nombre = x.Name,
                        CodComercio = x.ServiceGatewaysDto.FirstOrDefault().ReferenceId,
                        CodSucursal = x.ServiceGatewaysDto.FirstOrDefault().ServiceType,
                        IdMerchant = x.MerchantId,
                    }).ToList()
                };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaComercios - Exception " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(exception);
                var error = new VNPRespuestaConsultaComercios { CodResultado = 1, DescResultado = "Excepcion", IdOperacion = data.IdOperacion };
                try
                {
                    if (log != null)
                    {
                        log.Codresult = error.CodResultado;
                        _wsExternalLogClientService.EditCommerceQuery(log);
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Info, "WCF  AnulacionCobroApp - Exception " + DateTime.Now.ToString("G"));
                    NLogLogger.LogEvent(e);
                }

                return error;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuestaBajaTarjeta BajaTarjeta(BajaTarjetaData data)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("WCF  BajaTarjeta - " + DateTime.Now.ToString("G") + " - App: {0}, IdOperacion :{1}", data.IdApp, data.IdOperacion));
            WsCardRemoveDto log = null;
            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                _wsExternalLogClientService = NinjectRegister.Get<IWsExternalClientService>();
                VNPRespuestaBajaTarjeta response = null;
                
                log = new WsCardRemoveDto
                {
                    IdApp = data.IdApp,
                    IdOperation = data.IdOperacion,
                    IdUser = data.IdUsuario,
                    IdCard = data.IdTarjeta,
                    Codresult = response != null ? response.CodResultado : -1,
                    WcfVersion = GetVersion()
                };

                try
                {
                    //Se registra el WsCardRemove
                    log = _wsExternalLogClientService.CreateCardRemove(log).GetAwaiter().GetResult(); ;
                }
                catch (WebApiClientBusinessException exception)
                {
                    var errorMsg = string.Format( "Error al validar el id operación. Este ya fue utilizado. IdApp: {0}, IdOperacion: {1}, IdUsuario: {2}, IdTarjeta: {3}",
                            data.IdApp, data.IdOperacion, data.IdUsuario, data.IdTarjeta);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaBajaTarjeta{CodResultado = 11,DescResultado = "ID OPERACIÓN YA UTILIZADO",IdOperacion = data.IdOperacion};
                    return response;
                }

                var errors = "";

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);
                if (!withOutErrors)
                {
                    var errorMsg =
                        string.Format(
                            "Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1}, IdUsuario: {2}, IdTarjeta: {3}, Mensaje:{4}.",
                            data.IdApp, data.IdOperacion, data.IdUsuario, data.IdTarjeta, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaBajaTarjeta
                    {
                        CodResultado = 2,
                        DescResultado = errors,
                        IdOperacion = data.IdOperacion
                    };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCardRemove(log);
                    return response;
                }


                var info = string.Format("Datos: IdApp '{0}', IdOperacion'{1}', IdUsuario: '{2}', IdTarjeta: '{3}'",
                    data.IdApp, data.IdOperacion, data.IdUsuario, data.IdTarjeta);

                NLogLogger.LogEvent(NLogType.Info, info);

                //Se busca el certificado del Servicio
                var certificateThumbprint =
                    _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto
                    {
                        IdApp = data.IdApp
                    }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    response = new VNPRespuestaBajaTarjeta
                    {
                        CodResultado = 13,
                        DescResultado = "CERTIFICADO NO ENCONTRADO",
                        IdOperacion = data.IdOperacion
                    };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCardRemove(log);
                    return response;
                }

                var paramsArray = new[]
                {
                    data.IdApp,
                    data.IdOperacion,
                    data.IdUsuario,
                    data.IdTarjeta
                };

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg =
                        string.Format(
                            "Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1}, IdUsuario: {2}, IdTarjeta: {3}",
                            data.IdApp, data.IdOperacion, data.IdUsuario, data.IdTarjeta);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaBajaTarjeta
                    {
                        CodResultado = 12,
                        DescResultado = "FIRMA INVALIDA",
                        IdOperacion = data.IdOperacion
                    };
                    log.Codresult = response.CodResultado;
                    _wsExternalLogClientService.EditCardRemove(log);
                    return response;
                }

                //Se invoca a la operacion de baja de tarjeta
                NLogLogger.LogEvent(NLogType.Info, "WCF  BajaTarjeta - Inicio Metodo de baja de tarjeta");
                var result = _wsExternalAppClientService.RemoveCard(log).GetAwaiter().GetResult();
                log.Codresult = result.OperationResult;
                NLogLogger.LogEvent(NLogType.Info, "WCF  BajaTarjeta - Fin Metodo de baja de tarjeta");

                NLogLogger.LogEvent(NLogType.Info,
                    string.Format("WCF  BajaTarjeta - Resultado del llamado a webapi {0}", result.OperationResult));
                _wsExternalLogClientService.EditCardRemove(log);
                return new VNPRespuestaBajaTarjeta
                {
                    CodResultado = result.OperationResult,
                    IdOperacion = data.IdOperacion,
                };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  BajaTarjeta - Exception " + DateTime.Now.ToString("G") + " - IdOperacion: " + data.IdOperacion);
                NLogLogger.LogEvent(exception);
                var error = new VNPRespuestaBajaTarjeta { CodResultado = 1, DescResultado = "Excepcion", IdOperacion = data.IdOperacion };
                try
                {
                    if (log != null)
                    {
                        log.Codresult = error.CodResultado;
                        _wsExternalLogClientService.EditCardRemove(log);
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Info, "WCF  BajaTarjeta - Exception " + DateTime.Now.ToString("G") + " - IdOperacion: " + data.IdOperacion);
                    NLogLogger.LogEvent(e);
                }

                return error;
            }
        }

        #region MetodosViejos
        //public VNPRespuestaEnvioFacturas EnvioFacturas(EnvioFacturasData data)
        //{
        //    try
        //    {
        //        if (_wsExternalAppClientService == null)
        //        {
        //            _kernel = _kernel ?? new StandardKernel();
        //            NinjectRegister.Register(_kernel, true);
        //            _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
        //        }
        //        var errors = "";
        //        var billsid = "";
        //        var countAux = 0;
        //        foreach (var factura in data.Facturas)
        //        {
        //            var str = factura.AuxiliarData.Aggregate("", (current, aD) => current + "'" + aD.Id_auxiliar + "''" + aD.Dato_auxiliar + "'");

        //            var aux = "RefCliente1:'" + factura.RefCliente1 + "',";
        //            aux = aux + "RefCliente2:'" + factura.RefCliente2 + "',";
        //            aux = aux + "RefCliente3:'" + factura.RefCliente3 + "',";
        //            aux = aux + "RefCliente4:'" + factura.RefCliente4 + "',";
        //            aux = aux + "RefCliente5:'" + factura.RefCliente5 + "',";
        //            aux = aux + "RefCliente6:'" + factura.RefCliente6 + "',";
        //            aux = aux + "NroFactura:'" + factura.NroFactura + "',";
        //            aux = aux + "Descripcion:'" + factura.Descripcion + "',";
        //            aux = aux + "FchFactura:'" + factura.FchFactura + "',";
        //            aux = aux + "FchVencimiento:'" + factura.FchVencimiento + "',";
        //            aux = aux + "DiasPagoVenc:'" + factura.DiasPagoVenc + "',";
        //            aux = aux + "Moneda:'" + factura.Moneda + "',";
        //            aux = aux + "MontoTotal:'" + factura.MontoTotal + "',";
        //            aux = aux + "MontoMinimo:'" + factura.MontoMinimo + "',";
        //            aux = aux + "MontoGravado:'" + factura.MontoGravado + "',";
        //            aux = aux + "ConsFinal:'" + factura.ConsFinal + "',";
        //            aux = aux + "Cuotas:'" + factura.Cuotas + "',";
        //            aux = aux + "PagoAuto:'" + factura.PagoAuto + "',";
        //            aux = aux + "AuxiliarData:" + str;
        //            billsid = billsid + aux;

        //            countAux = countAux + (factura.AuxiliarData.Count * 2);
        //        }
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  EnvioFacturas - " + DateTime.Now.ToString("G"));
        //        var info = string.Format("Datos: CodComercio '{0}', CodSucursal '{1}', ReemplazarFacturas '{2}', Facturas: {3}.",
        //            data.CodComercio, data.CodSucursal, data.ReemplazarFacturas, billsid);
        //        NLogLogger.LogEvent(NLogType.Info, info);

        //        var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprint(new WebServiceClientInputDto() { CodCommerce = data.CodComercio, CodBranch = data.CodSucursal }).Result;

        //        var paramsArray = new string[3 + countAux + (data.Facturas.Count * 13)];
        //        paramsArray[0] = data.CodComercio.ToString();
        //        paramsArray[1] = data.CodSucursal.ToString();
        //        paramsArray[2] = data.ReemplazarFacturas.ToString();

        //        int i = 3;
        //        foreach (var factura in data.Facturas)
        //        {
        //            paramsArray[i] = factura.RefCliente1; i++;
        //            paramsArray[i] = factura.RefCliente2; i++;
        //            paramsArray[i] = factura.RefCliente3; i++;
        //            paramsArray[i] = factura.RefCliente4; i++;
        //            paramsArray[i] = factura.RefCliente5; i++;
        //            paramsArray[i] = factura.RefCliente6; i++;
        //            paramsArray[i] = factura.NroFactura; i++;
        //            paramsArray[i] = factura.Descripcion; i++;
        //            paramsArray[i] = factura.FchFactura.ToString(); i++;
        //            paramsArray[i] = factura.FchVencimiento.ToString(); i++;
        //            paramsArray[i] = factura.DiasPagoVenc.ToString(); i++;
        //            paramsArray[i] = factura.Moneda; i++;
        //            paramsArray[i] = factura.MontoTotal.ToString(); i++;
        //            paramsArray[i] = factura.MontoMinimo.ToString(); i++;
        //            paramsArray[i] = factura.MontoGravado.ToString(); i++;
        //            paramsArray[i] = factura.ConsFinal.ToString(); i++;
        //            paramsArray[i] = factura.Cuotas.ToString(); i++;
        //            paramsArray[i] = factura.PagoAuto.ToString(); i++;
        //            foreach (var aD in factura.AuxiliarData)
        //            {
        //                paramsArray[i] = aD.Id_auxiliar; i++;
        //                paramsArray[i] = aD.Dato_auxiliar; i++;
        //            }
        //        }

        //        //var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateName);
        //        var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);

        //        if (!withOutErrors)
        //        {
        //            var errorMsg = string.Format("Error al validar los campos requeridos. CodComercio: {0}, CodSucursal: {1} Mensaje:{2}.", data.CodComercio, data.CodSucursal, errors);
        //            NLogLogger.LogEvent(NLogType.Error, errorMsg);
        //            return new VNPRespuestaEnvioFacturas() { CodResultado = 2, DescResultado = errors };
        //        }

        //        var dto = new WebServiceBillsInputDto()
        //                  {
        //                      CodBranch = data.CodSucursal,
        //                      CodCommerce = data.CodComercio,
        //                      ReplaceBills = data.ReemplazarFacturas,
        //                      DigitalFirm = data.FirmaDigital,
        //                      HighwayBillDtos = data.Facturas.Select(x => new HighwayBillDto()
        //                                                                  {
        //                                                                      CodSucursal = data.CodSucursal,
        //                                                                      CodComercio = data.CodComercio,
        //                                                                      ConsFinal = x.ConsFinal,
        //                                                                      Cuotas = x.Cuotas,
        //                                                                      DiasPagoVenc = x.DiasPagoVenc,
        //                                                                      Moneda = x.Moneda,
        //                                                                      Descripcion = x.Descripcion,
        //                                                                      RefCliente = x.RefCliente1,
        //                                                                      RefCliente2 = x.RefCliente2,
        //                                                                      RefCliente3 = x.RefCliente3,
        //                                                                      RefCliente4 = x.RefCliente4,
        //                                                                      RefCliente5 = x.RefCliente5,
        //                                                                      RefCliente6 = x.RefCliente6,
        //                                                                      MontoGravado = x.MontoGravado,
        //                                                                      MontoMinimo = x.MontoMinimo,
        //                                                                      MontoTotal = x.MontoTotal,
        //                                                                      NroFactura = x.NroFactura,
        //                                                                      FchFactura = x.FchFactura,
        //                                                                      FchVencimiento = x.FchVencimiento,
        //                                                                      PagoDebito = x.PagoAuto,
        //                                                                      AuxiliarData = x.AuxiliarData.Select(a => new HighwayAuxiliarDataDto()
        //                                                                                                                {
        //                                                                                                                    Id_auxiliar = a.Id_auxiliar,
        //                                                                                                                    Dato_auxiliar = a.Dato_auxiliar
        //                                                                                                                }).ToList()
        //                                                                  }).ToList()
        //                  };

        //        var result = _wsExternalAppClientService.AddNewBills(dto).Result;

        //        if (!result.Any())
        //        {
        //            return new VNPRespuestaEnvioFacturas() { CodResultado = 0 };
        //        }
        //        else
        //        {
        //            return new VNPRespuestaEnvioFacturas()
        //            {
        //                CodResultado = 2,
        //                DescResultado = "ERRORES EN LOS CAMPOS ENVIADOS. DETALLE EN CADA FACTURA",
        //                FacturasError = result.Select(x => new FacturaError()
        //                                                   {
        //                                                       CodError = x.Cuotas,
        //                                                       DescError = x.Descripcion,
        //                                                       NroFactura = x.NroFactura,
        //                                                       RefCliente = x.RefCliente
        //                                                   }).ToList()
        //            };
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  EnvioFacturas - Exception");
        //        NLogLogger.LogEvent(exception);
        //    }
        //    return new VNPRespuestaEnvioFacturas() { CodResultado = 1, DescResultado = "Errores varios", };
        //}
        //public VNPRespuestaConsultaFacturas ConsultaFacturas(ConsultaFacturasData data)
        //{
        //    NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaFacturas - " + DateTime.Now.ToString("G"));

        //    try
        //    {
        //        _kernel = new StandardKernel();
        //        NinjectRegister.Register(_kernel, true);
        //        _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();

        //        var errors = "";
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaFacturas - " + DateTime.Now.ToString("G"));
        //        var info = string.Format("Datos: CodComercio '{0}', CodSucursal '{1}', Fecha '{3}', NroFactura '{4}'" +
        //                                 ", RefCliente1 '{5}', RefCliente2 '{6}', RefCliente3 '{7}', RefCliente4 '{8}', RefCliente5 '{9}', RefCliente6 '{10}'.",
        //            data.CodComercio, data.CodSucursal, data.Fecha, data.NroFactura, data.RefCliente, data.RefCliente2, data.RefCliente3,
        //            data.RefCliente4, data.RefCliente5, data.RefCliente6);
        //        NLogLogger.LogEvent(NLogType.Info, info);

        //        var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprint(new WebServiceClientInputDto() { CodCommerce = int.Parse(data.CodComercio), CodBranch = int.Parse(data.CodSucursal) }).Result;

        //        var paramsArray = new[]
        //        {
        //            data.CodComercio.ToString(),
        //            data.CodSucursal.ToString(),
        //            //data.FechaDesde.ToString(),
        //            //data.FechaHasta.ToString(),
        //            //data.RefCliente1,
        //            data.RefCliente2,
        //            data.RefCliente3,
        //            data.RefCliente4,
        //            data.RefCliente5,
        //            data.RefCliente6,
        //            data.NroFactura
        //        };

        //        //var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateName);

        //        var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);

        //        if (!withOutErrors)
        //        {
        //            var errorMsg = string.Format("Error al validar los campos requeridos. CodComercio: {0}, CodSucursal: {1} Mensaje:{2}.", data.CodComercio, data.CodSucursal, errors);
        //            NLogLogger.LogEvent(NLogType.Error, errorMsg);
        //            return new VNPRespuestaConsultaFacturas() { CodResultado = 2, DescResultado = errors };
        //        }

        //        ////var result = _wsExternalAppClientService.BillsStatus(new WebServiceBillsStatusInputDto()
        //        ////                                              {
        //        ////                                                  CodComercio = data.CodComercio,
        //        ////                                                  CodSucursal = data.CodSucursal,
        //        ////                                                  RefCliente1 = data.RefCliente1,
        //        ////                                                  RefCliente2 = data.RefCliente2,
        //        ////                                                  RefCliente3 = data.RefCliente3,
        //        ////                                                  RefCliente4 = data.RefCliente4,
        //        ////                                                  RefCliente5 = data.RefCliente5,
        //        ////                                                  RefCliente6 = data.RefCliente6,
        //        ////                                                  NroFactura = data.NroFactura,
        //        ////                                                  FechaDesde = data.FechaDesde,
        //        ////                                                  FechaHasta = data.FechaDesde
        //        ////                                              }).Result;

        //        //var vnp = new VNPRespuestaConsultaFacturas()
        //        //          {
        //        //              CodResultado = result.CodResultado,
        //        //              DescResultado = result.DescResultado,
        //        //              ResumenPagos = new ResumenPagos()
        //        //                             {
        //        //                                 CantFacturas = result.ResumenPagos.CantFacturas,
        //        //                                 CantDolaresPagados = result.ResumenPagos.CantDolaresPagados,
        //        //                                 CantPesosPagados = result.ResumenPagos.CantPesosPagados,
        //        //                                 SumaDolaresPagados = result.ResumenPagos.SumaDolaresPagados,
        //        //                                 SumaPesosPagados = result.ResumenPagos.SumaPesosPagados
        //        //                             },
        //        //              EstadoFacturas = result.EstadoFacturas.Select(y => new EstadoFacturas()
        //        //                                                                 {
        //        //                                                                     RefCliente1 = y.RefCliente1,
        //        //                                                                     RefCliente2 = y.RefCliente2,
        //        //                                                                     RefCliente3 = y.RefCliente3,
        //        //                                                                     RefCliente4 = y.RefCliente4,
        //        //                                                                     RefCliente5 = y.RefCliente5,
        //        //                                                                     RefCliente6 = y.RefCliente6,
        //        //                                                                     NroFactura = y.NroFactura,
        //        //                                                                     MontoTotal = y.MontoTotal,
        //        //                                                                     MontoDescIVA = y.MontoDescIVA,
        //        //                                                                     Moneda = y.Moneda,
        //        //                                                                     FchPago = y.FchPago,
        //        //                                                                     Estado = y.Estado,
        //        //                                                                     DescEstado = y.DescEstado,
        //        //                                                                     CodError = y.CodError,
        //        //                                                                     CantCuotas = y.CantCuotas,
        //        //                                                                     AuxiliarData = y.AuxiliarData.Select(z => new AuxiliarData()
        //        //                                                                                                               {
        //        //                                                                                                                   Dato_auxiliar = z.Dato_auxiliar,
        //        //                                                                                                                   Id_auxiliar = z.Id_auxiliar
        //        //                                                                                                               }).ToList()
        //        //                                                                 }).ToList()
        //        //          };
        //        //return vnp;
        //        return null;
        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaFacturas - Exception");
        //        NLogLogger.LogEvent(exception);
        //    }
        //    return new VNPRespuestaConsultaFacturas() { CodResultado = 1, DescResultado = "Errores varios" };
        //}
        //public VNPRespuesta QuitarFacturas(QuitarFacturasData data)
        //{

        //    try
        //    {
        //        if (_wsExternalAppClientService == null)
        //        {
        //            _kernel = _kernel ?? new StandardKernel();
        //            NinjectRegister.Register(_kernel, true);
        //            _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
        //        }

        //        NLogLogger.LogEvent(NLogType.Info, "WCF  QuitarFacturas - " + DateTime.Now.ToString("G"));
        //        var billsid = data.FacturaEliminar.Select(x => x.NroFactura).Aggregate("", (current, billid) => current + " '" + billid + "'");
        //        var info = string.Format("      Datos: CodComercio '{0}', CodSucursal '{1}', FacturaEliminar {2}. ",
        //            data.CodComercio, data.CodSucursal, billsid);
        //        NLogLogger.LogEvent(NLogType.Info, info);

        //        var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprint(new WebServiceClientInputDto() { CodCommerce = data.CodComercio, CodBranch = data.CodSucursal }).Result;

        //        var paramsArray = new string[data.FacturaEliminar.Count() + 2];
        //        paramsArray[0] = data.CodComercio.ToString();
        //        paramsArray[1] = data.CodSucursal.ToString();
        //        var i = 2;
        //        foreach (var fq in data.FacturaEliminar)
        //        {
        //            paramsArray[i] = fq.NroFactura;
        //            i++;
        //        }

        //        //var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateName);

        //        var errors = "";
        //        var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);

        //        if (!withOutErrors)
        //        {
        //            var errorMsg = string.Format("Error al validar los campos requeridos. CodComercio: {0}, CodSucursal: {1} Mensaje:{2}.", data.CodComercio, data.CodSucursal, errors);
        //            NLogLogger.LogEvent(NLogType.Error, errorMsg);
        //            return new VNPRespuesta() { CodResultado = 2, DescResultado = errors };
        //        }

        //        var result = _wsExternalAppClientService.DeleteBills(new WebServiceBillsDeleteDto()
        //        {
        //            CodBranch = data.CodSucursal,
        //            CodCommerce = data.CodComercio,
        //            NroFacturas = data.FacturaEliminar.Select(x => x.NroFactura).ToList()
        //        }).Result;

        //        if (result == 0)
        //        {
        //            return new VNPRespuesta() { CodResultado = 0 };
        //        }
        //        if (result == 2)
        //        {
        //            return new VNPRespuesta() { CodResultado = 2, DescResultado = "El campo CodComercio y/o CodSucursal no coincide con los datos de las facturas registradas." };
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  QuitarFacturas - Exception");
        //        NLogLogger.LogEvent(exception);
        //    }
        //    return new VNPRespuesta() { CodResultado = 1, DescResultado = "Errores varios" };
        //}
        //public VNPRespuestaConsultaClientes ConsultaClientes(ConsultaClientesData data)
        //{
        //    try
        //    {
        //        if (_wsExternalAppClientService == null)
        //        {
        //            _kernel = _kernel ?? new StandardKernel();
        //            NinjectRegister.Register(_kernel, true);
        //            _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
        //        }
        //        var errors = "";
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaClientes - " + DateTime.Now.ToString("G"));
        //        var info = string.Format("Datos: CodComercio '{0}', CodSucursal '{1}', RefCliente1 '{2}',RefCliente2 '{3}',RefCliente3 '{4}'," +
        //                                 "RefCliente4 '{5}',RefCliente5 '{6}', RefCliente6 '{7}', FechaDesde '{8}'. firma {9}",
        //            data.CodComercio, data.CodSucursal, data.RefCliente1, data.RefCliente2, data.RefCliente3,
        //            data.RefCliente4, data.RefCliente5, data.RefCliente6, data.FechaDesde, data.FirmaDigital);
        //        NLogLogger.LogEvent(NLogType.Info, info);

        //        var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprint(new WebServiceClientInputDto() { CodCommerce = data.CodComercio, CodBranch = data.CodSucursal }).Result;

        //        var paramsArray = new[]
        //        {
        //            data.CodComercio.ToString(),
        //            data.CodSucursal.ToString(),
        //            data.FechaDesde.ToString(),
        //            data.RefCliente1,
        //            data.RefCliente2,
        //            data.RefCliente3,
        //            data.RefCliente4,
        //            data.RefCliente5,
        //            data.RefCliente6,
        //        };

        //        var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);

        //        if (!valid)
        //        {
        //            var errorMsg = string.Format("Error al validar los campos firmados. CodComercio: {0}, CodSucursal: {1} ", data.CodComercio, data.CodSucursal);
        //            NLogLogger.LogEvent(NLogType.Error, errorMsg);
        //            return new VNPRespuestaConsultaClientes() { CodResultado = 12, DescResultado = "FIRMA INVALIDA" };
        //        }

        //        var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);

        //        if (!withOutErrors)
        //        {
        //            var errorMsg = string.Format("Error al validar los campos requeridos. CodComercio: {0}, CodSucursal: {1} Mensaje:{2}.", data.CodComercio, data.CodSucursal, errors);
        //            NLogLogger.LogEvent(NLogType.Error, errorMsg);
        //            return new VNPRespuestaConsultaClientes() { CodResultado = 2, DescResultado = errors };
        //        }

        //        var result = _wsExternalAppClientService.AssosiatedServiceClientUpdate(new WebServiceClientInputDto()
        //                                                           {
        //                                                               CodCommerce = data.CodComercio,
        //                                                               CodBranch = data.CodSucursal,
        //                                                               FechaDesde = data.FechaDesde,
        //                                                               RefCliente1 = data.RefCliente1,
        //                                                               RefCliente2 = data.RefCliente2,
        //                                                               RefCliente3 = data.RefCliente3,
        //                                                               RefCliente4 = data.RefCliente4,
        //                                                               RefCliente5 = data.RefCliente5,
        //                                                               RefCliente6 = data.RefCliente6,
        //                                                           }).Result;

        //        var rcc = new VNPRespuestaConsultaClientes()
        //                  {
        //                      ListadoClientes = result.Select(x => new Cliente()
        //                                    {
        //                                        RefCliente1 = x.RefCliente1,
        //                                        RefCliente2 = x.RefCliente2,
        //                                        RefCliente3 = x.RefCliente3,
        //                                        RefCliente4 = x.RefCliente4,
        //                                        RefCliente5 = x.RefCliente5,
        //                                        RefCliente6 = x.RefCliente6,
        //                                        Apellido = x.Apellido,
        //                                        Nombre = x.Nombre,
        //                                        Documento = x.Documento,
        //                                        Email = x.Email,
        //                                        FchModificacion = x.FchModificacion,
        //                                        Telefono = x.Telefono,
        //                                        Estado = x.Estado
        //                                    }).ToList(),
        //                      CodResultado = 0
        //                  };

        //        return rcc;
        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaClientes - Exception");
        //        NLogLogger.LogEvent(exception);
        //    }
        //    return new VNPRespuestaConsultaClientes() { CodResultado = 1, DescResultado = "Errores varios" };
        //}
        //public VNPRespuesta TestConection()
        //{
        //    NLogLogger.LogEvent(NLogType.Info, "WCF  Test - " + DateTime.Now.ToString("G"));
        //    return new VNPRespuesta() { CodResultado = 0, DescResultado = "CONECCION OK" };
        //}
        #endregion

        private string GetVersion()
        {
            try
            {
                var version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(VNPAccess)).Location).FileVersion;
                return version;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("WCF  GetVersion "));
                NLogLogger.LogAppsEvent(exception);
            }
            return string.Empty;
        }

        /// <summary>
        /// Si el merchantId es nullo o vacio, el codCommerce y codBranch no lo pueden ser. Lo mismo aplica a la inversa.
        /// </summary>
        /// <param name="codCommerce"></param>
        /// <param name="codBranch"></param>
        /// <param name="merchantId"></param>
        private string CheckCodCommerceMerchant(string codCommerce, string codBranch, string merchantId)
        {
            if (string.IsNullOrEmpty(codCommerce) && string.IsNullOrEmpty(codBranch) && string.IsNullOrEmpty(merchantId))
            {
                return "Si no se envian datos en campo IdMerchant, se debe enviar en los campos CodComercio y CodSucursal";
            }

            if (string.IsNullOrEmpty(merchantId))
            {
                if (string.IsNullOrEmpty(codCommerce) && string.IsNullOrEmpty(codBranch))
                {
                    return string.IsNullOrEmpty(codCommerce) ? "Falta el campo CodComercio. " : string.Empty + " " +
                        (string.IsNullOrEmpty(codBranch) ? "Falta el campo CodSucursal" : string.Empty);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNPRespuestaConsultaUrlTransaccion ConsultaUrlTransaccion(ConsultaUrlTransaccionData data)
        {
            NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaUrlTransaccion - " + DateTime.Now.ToString("G"));
            WsUrlTransactionQueryDto log = null;
            try
            {
                _kernel = _kernel ?? new StandardKernel();
                NinjectRegister.Register(_kernel, true);
                _wsExternalAppClientService = NinjectRegister.Get<IWsExternalAppClientService>();
                VNPRespuestaConsultaUrlTransaccion response = null;

                log = new WsUrlTransactionQueryDto
                {
                    IdApp = data.IdApp,
                    IdOperation = data.IdOperacion,
                    WcfVersion = GetVersion(),
                    QueryDate = data.Fecha,
                };
                
                var errors = "";

                var withOutErrors = DataValidation.InputParametersAreValid(data, out errors);
                if (!withOutErrors)
                {
                    var errorMsg = string.Format("Error al validar los campos requeridos. IdApp: {0}, IdOperacion: {1} Mensaje:{2}.", data.IdApp, data.IdOperacion, errors);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaConsultaUrlTransaccion { CodResultado = 2, DescResultado = errors, IdOperacion = data.IdOperacion };
                    return response;
                }

                var info = string.Format("Datos: IdApp '{0}', IdOperacion'{1}'", data.IdApp, data.IdOperacion);

                NLogLogger.LogEvent(NLogType.Info, info);

                //Se busca el certificado del Servicio
                var certificateThumbprint = _wsExternalAppClientService.GetCertificateThumbprintIdApp(new WebServiceClientInputDto { IdApp = data.IdApp }).Result;
                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    response = new VNPRespuestaConsultaUrlTransaccion { CodResultado = 13, DescResultado = "CERTIFICADO NO ENCONTRADO", IdOperacion = data.IdOperacion };
                    return response;
                }

                var paramsArray = new[]
                {
                    data.IdApp,
                    data.IdOperacion,
                    data.Fecha.ToString("yyyyMMdd")
                };

                //Se valida la firma
                var valid = DigitalSignature.CheckSignature(paramsArray, data.FirmaDigital, certificateThumbprint);
                if (!valid)
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. IdApp: {0}, IdOperacion: {1} ", data.IdApp, data.IdOperacion);
                    NLogLogger.LogEvent(NLogType.Error, errorMsg);
                    response = new VNPRespuestaConsultaUrlTransaccion { CodResultado = 12, DescResultado = "FIRMA INVALIDA", IdOperacion = data.IdOperacion };
                    return response;
                }

                //Se invoca a la operacion de consulta de comercios
                var result = _wsExternalAppClientService.GetUrlTransacctionPosts(log).GetAwaiter().GetResult();

                return new VNPRespuestaConsultaUrlTransaccion
                {
                    IdOperacion = log.IdOperation,
                    CodResultado = 0,
                    DescResultado = string.Empty,
                    ListadoLlamadasUrlTransaccion = result.Select(ConverterWebhookUrlTransactionModel).ToList(),
                };
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "WCF  ConsultaComercios - Exception " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(exception);
                var error = new VNPRespuestaConsultaUrlTransaccion { CodResultado = 1, DescResultado = "Excepcion", IdOperacion = data.IdOperacion };
                return error;
            }
        }

        private WebhookUrlTransactionModel ConverterWebhookUrlTransactionModel(WebhookNewAssociationDto dto)
        {
            return new WebhookUrlTransactionModel()
            {
                Email=  dto.UserData.Email,
                IdUsuario=  dto.IdUser,
                IdTarjeta=  dto.IdCard,
                VencTarjeta=  dto.CardDueDate,
                SufijoTarjeta=  dto.CardMask,
                IdOperacionApp=  dto.IdOperationApp,
                IdOperacion=  dto.IdOperation,
                EnvioAsociacion=  dto.IsAssociation.ToString(),
                EnvioPago=  dto.IsPayment.ToString(),
                EmisorTarjeta=  dto.CardBank,
                TipoTarjeta=  dto.CardType != null ? ((int) dto.CardType).ToString() : null,
                CodEmisorTarjeta=  dto.CardBankCode,
                ProgramaTarjeta=  dto.CardAffiliation,
                CodProgramaTarjeta=  dto.CardAffiliationCode,
                FacturaImporteDescuento = dto.DiscountAmount.ToString("0.##").Replace(",", "").Replace(".", ""),
                HttpCodigoRespuesta = dto.HttpResponseCode,
                NroTransacción = dto.TransactionNumber,
                RefCliente1 = dto.RefCliente1,
                RefCliente2 = dto.RefCliente2,
                RefCliente3 = dto.RefCliente3,
                RefCliente4 = dto.RefCliente4,
                RefCliente5 = dto.RefCliente5,
                RefCliente6 = dto.RefCliente6,
            };
        }
    }
}