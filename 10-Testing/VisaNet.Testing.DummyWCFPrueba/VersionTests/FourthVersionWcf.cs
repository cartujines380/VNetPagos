using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;
using VisaNet.Testing.DummyWCFPrueba.Model;

//Referencia al WebService
using ServiceReference = VisaNet.Testing.DummyWCFPrueba.Localhost;
//using ServiceReference = VisaNet.Testing.DummyWCFPrueba.Demouy_Production_v03;
//using ServiceReference = VisaNet.Testing.DummyWCFPrueba.Demouy_Development_v03;
//using ServiceReference = VisaNet.Testing.DummyWCFPrueba.Demouy_Testing_v03;
//using ServiceReference = VisaNet.Testing.DummyWCFPrueba.Azure_VisaNet_v03;

namespace VisaNet.Testing.DummyWCFPrueba.VersionTests
{
    public static class FourthVersionWcf
    {
        private static CultureInfo _culture;
        private const string Folder = "FourthVersionData";

        public static void Tests()
        {
            _culture = new CultureInfo(ConfigurationManager.AppSettings["AppCulture"]);
            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;

            while (true)
            {
                ConsoleOptions();
                Console.Write("OPCION: ");
                var a = Console.ReadLine();

                switch (a)
                {
                    case "co":
                        Payment();
                        break;
                    case "af":
                        CancelPayment();
                        break;
                    case "rt":
                        GetTransactions();
                        break;
                    case "rs":
                        GetServices();
                        break;
                    case "bs":
                        CardOrAssociationDown();
                        break;
                    case "s":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void ConsoleOptions()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("OPCIONES");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("co   => Realiza el cobro de una factura dada");
            Console.WriteLine("af   => Realiza cancelacion de una transacción dada");
            Console.WriteLine("rt   => Listado de las transacciones para un dia y idapp dado");
            Console.WriteLine("rs   => Listado de comercios de un idapp dado");
            Console.WriteLine("bs   => Realiza la baja de una tarjeta y/o servicio asociado");
            Console.WriteLine("s    => Salir ");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Instrucciones en archivo README");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private static void Payment()
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ;

            var fileName = ConfigurationManager.AppSettings["CobroOnlineJson"];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", Folder, fileName);
            NLogLogger.LogEvent(NLogType.Info, path);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var facturas = JsonConvert.DeserializeObject<IEnumerable<CobroOnlineModel>>(json, new JsonSerializerSettings()
                {
                    Culture = _culture
                });

                foreach (var cobroOnlineModel in facturas)
                {
                    var data = new ServiceReference.CobrarFacturaData
                    {
                        IdApp = cobroOnlineModel.IdApp,
                        Factura = new ServiceReference.FacturaOnline()
                        {
                            CodComercio = cobroOnlineModel.FacturaOnlineModel.CodComercio,
                            CodSucursal = cobroOnlineModel.FacturaOnlineModel.CodSucursal,
                            IdUsuario = cobroOnlineModel.FacturaOnlineModel.IdUsuario,
                            IdTarjeta = cobroOnlineModel.FacturaOnlineModel.IdTarjeta,
                            Descripcion = cobroOnlineModel.FacturaOnlineModel.Descripcion,
                            FchFactura = cobroOnlineModel.FacturaOnlineModel.FchFactura,
                            Moneda = cobroOnlineModel.FacturaOnlineModel.Moneda,
                            MontoTotal = cobroOnlineModel.FacturaOnlineModel.MontoTotal,
                            Indi = cobroOnlineModel.FacturaOnlineModel.Indi,
                            MontoGravado = cobroOnlineModel.FacturaOnlineModel.MontoGravado,
                            ConsFinal = cobroOnlineModel.FacturaOnlineModel.ConsFinal,
                            Cuotas = cobroOnlineModel.FacturaOnlineModel.Cuotas,
                            IdMerchant = cobroOnlineModel.FacturaOnlineModel.IdMerchant,
                            DeviceFingerprint = cobroOnlineModel.FacturaOnlineModel.DeviceFingerprint,
                            IpCliente = cobroOnlineModel.FacturaOnlineModel.IpCliente,
                            TelefonoCliente = cobroOnlineModel.FacturaOnlineModel.TelefonoCliente
                        }
                    };
                    if (cobroOnlineModel.FacturaOnlineModel.DireccionEnvioClienteModel != null)
                    {
                        var tmp = cobroOnlineModel.FacturaOnlineModel.DireccionEnvioClienteModel;
                        data.Factura.DireccionEnvioCliente = new ServiceReference.CustomerShippingAddres()
                        {
                            Calle = tmp.Calle,
                            Barrio = tmp.Barrio,
                            Telefono = tmp.Telefono,
                            CodigoPostal = tmp.CodigoPostal,
                            Complemento = tmp.Complemento,
                            Esquina = tmp.Esquina,
                            Latitud = tmp.Latitud,
                            Longitud = tmp.Longitud,
                            NumeroPuerta = tmp.NumeroPuerta,
                            Ciudad = tmp.Ciudad,
                            Pais = tmp.Pais,
                        };
                    }
                    if (cobroOnlineModel.FacturaOnlineModel.AuxiliarData != null && cobroOnlineModel.FacturaOnlineModel.AuxiliarData.Any())
                    {
                        data.Factura.AuxiliarData = new ServiceReference.AuxiliarData[cobroOnlineModel.FacturaOnlineModel.AuxiliarData.Length];

                        for (int i = 0; i < cobroOnlineModel.FacturaOnlineModel.AuxiliarData.Length; i++)
                        {
                            data.Factura.AuxiliarData[i] = new ServiceReference.AuxiliarData()
                            {
                                Id_auxiliar = cobroOnlineModel.FacturaOnlineModel.AuxiliarData[i].Id_auxiliar,
                                Dato_auxiliar = cobroOnlineModel.FacturaOnlineModel.AuxiliarData[i].Dato_auxiliar,
                            };
                        }
                    }

                    int idOpInt = 0;
                    Int32.TryParse(cobroOnlineModel.IdOperacion, out idOpInt);
                    data.IdOperacion = idOpInt < 0 ? Guid.NewGuid().ToString() : cobroOnlineModel.IdOperacion;

                    int nroFactura = 0;
                    Int32.TryParse(cobroOnlineModel.FacturaOnlineModel.NroFactura, out nroFactura);
                    data.Factura.NroFactura = nroFactura < 0 ? DateTime.Now.ToString("yyyyMMddHHmmss") : cobroOnlineModel.FacturaOnlineModel.NroFactura;

                    var cliente = new ServiceReference.VNPAccessClient();
                    var paramsArray = new[]
                    {
                        data.IdApp,
                        data.Factura.CodComercio,
                        data.Factura.CodSucursal,
                        data.Factura.IdUsuario,
                        data.Factura.IdTarjeta,
                        data.Factura.NroFactura,
                        data.Factura.Descripcion,
                        data.Factura.FchFactura.ToString("yyyyMMdd"),
                        data.Factura.Moneda,
                        data.Factura.MontoTotal.ToString("#0.00", CultureInfo.CurrentCulture),
                        data.Factura.Indi.ToString(),
                        data.Factura.MontoGravado.ToString("#0.00", CultureInfo.CurrentCulture),
                        data.Factura.ConsFinal.ToString(),
                        data.Factura.Cuotas.ToString(),
                        data.IdOperacion,
                        data.Factura.IdMerchant,
                        data.Factura.DeviceFingerprint,
                        data.Factura.IpCliente,
                        data.Factura.TelefonoCliente,
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
                        data.Factura.DireccionEnvioCliente.Pais,
                    };

                    string firmaDigital = PruebaSignature.GenerateSignature(paramsArray);

                    data.FirmaDigital = firmaDigital;

                    var result = cliente.CobroFacturaOnlineApp(data);
                    Console.WriteLine(result.CodResultado);
                }
            }
        }

        private static void CancelPayment()
        {
            var fileName = ConfigurationManager.AppSettings["CancelacionJson"];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", Folder, fileName);
            NLogLogger.LogEvent(NLogType.Info, path);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var facturas = JsonConvert.DeserializeObject<IEnumerable<AnularFacturaModel>>(json, new JsonSerializerSettings()
                {
                    Culture = _culture
                });

                foreach (var anularFacturaModel in facturas)
                {
                    var data = new ServiceReference.AnularFacturaData()
                    {
                        IdApp = anularFacturaModel.IdApp,
                        IdOperacionCobro = anularFacturaModel.IdOperacionCobro,
                    };

                    int idOpInt = 0;
                    Int32.TryParse(anularFacturaModel.IdOperacion, out idOpInt);

                    data.IdOperacion = idOpInt < 0 ? Guid.NewGuid().ToString() : anularFacturaModel.IdOperacion;

                    var cliente = new ServiceReference.VNPAccessClient();

                    var paramsArray = new[]
                    {
                        data.IdApp,
                        data.IdOperacion,
                        data.IdOperacionCobro
                    };

                    string firmaDigital = PruebaSignature.GenerateSignature(paramsArray);
                    data.FirmaDigital = firmaDigital;
                    var result = cliente.AnulacionCobroApp(data);
                    Console.WriteLine(result.CodResultado);
                }
            }
        }

        private static void GetTransactions()
        {
            var fileName = ConfigurationManager.AppSettings["ConsultaFacturasJson"];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", Folder, fileName);
            NLogLogger.LogEvent(NLogType.Info, path);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var facturas = JsonConvert.DeserializeObject<IEnumerable<ConsultaFacturasModel>>(json, new JsonSerializerSettings()
                {
                    Culture = _culture
                });

                if (facturas == null)
                    Console.WriteLine("Sin datos para leer");
                else
                {
                    foreach (var consultar in facturas)
                    {
                        var data = new ServiceReference.ConsultaFacturasData()
                        {
                            IdApp = consultar.IdApp,
                            CodComercio = consultar.CodComercio,
                            CodSucursal = consultar.CodComercio,
                            IdMerchant = consultar.IdMerchant,
                            Fecha = consultar.Fecha,
                            NroFactura = string.IsNullOrEmpty(consultar.NroFactura) ? null : consultar.NroFactura,
                            RefCliente = string.IsNullOrEmpty(consultar.RefCliente) ? null : consultar.RefCliente,
                            RefCliente2 = string.IsNullOrEmpty(consultar.RefCliente2) ? null : consultar.RefCliente2,
                            RefCliente3 = string.IsNullOrEmpty(consultar.RefCliente3) ? null : consultar.RefCliente3,
                            RefCliente4 = string.IsNullOrEmpty(consultar.RefCliente4) ? null : consultar.RefCliente4,
                            RefCliente5 = string.IsNullOrEmpty(consultar.RefCliente5) ? null : consultar.RefCliente5,
                            RefCliente6 = string.IsNullOrEmpty(consultar.RefCliente6) ? null : consultar.RefCliente6,
                        };

                        int idOpInt = 0;
                        Int32.TryParse(consultar.IdOperacion, out idOpInt);

                        data.IdOperacion = idOpInt < 0 ? Guid.NewGuid().ToString() : consultar.IdOperacion;

                        var cliente = new ServiceReference.VNPAccessClient();

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

                        string firmaDigital = PruebaSignature.GenerateSignature(paramsArray);

                        data.FirmaDigital = firmaDigital;

                        var result = cliente.ConsultaTransacciones(data);
                        if (result != null)
                        {
                            Console.WriteLine(result.CodResultado);
                            if (result.CodResultado == 0)
                            {
                                Console.WriteLine("Resumen de pagos:");
                                Console.WriteLine("Trns encontradas: " + result.ResumenPagos.CantFacturas);
                                Console.WriteLine("Trns UYU: " + result.ResumenPagos.CantPesosPagados);
                                Console.WriteLine("Trns UYU Monto: " + result.ResumenPagos.SumaPesosPagados);
                                Console.WriteLine("Trns USD: " + result.ResumenPagos.CantDolaresPagados);
                                Console.WriteLine("Trns USD Monto: " + result.ResumenPagos.SumaDolaresPagados);

                                Console.WriteLine("EstadoFacturas: ");
                                foreach (var estadoFacturas in result.EstadoFacturas.OrderByDescending(x => x.FchPago))
                                {
                                    Console.WriteLine("Nro Factura: " + estadoFacturas.NroFactura + ", Estado: " +
                                                      estadoFacturas.Estado);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(-1);
                        }
                    }
                }
            }
        }

        private static void GetServices()
        {
            var cliente = new ServiceReference.VNPAccessClient();

            var fileName = ConfigurationManager.AppSettings["ConsultaServiciosJson"];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", Folder, fileName);
            NLogLogger.LogEvent(NLogType.Info, path);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var info = JsonConvert.DeserializeObject<IEnumerable<ConsultaServiciosModel>>(json, new JsonSerializerSettings()
                {
                    Culture = _culture
                });

                foreach (var consulta in info)
                {
                    var data = new ServiceReference.ConsultaComerciosData()
                    {
                        IdApp = consulta.IdApp,
                    };

                    int idOpInt = 0;
                    Int32.TryParse(consulta.IdOperacion, out idOpInt);

                    data.IdOperacion = idOpInt < 0 ? Guid.NewGuid().ToString() : consulta.IdOperacion;

                    var paramsArray = new[]
                        {
                            data.IdApp,
                            data.IdOperacion,
                        };

                    string firmaDigital = PruebaSignature.GenerateSignature(paramsArray);

                    data.FirmaDigital = firmaDigital;

                    var result = cliente.ConsultaComercios(data);
                    Console.WriteLine(result.CodResultado);
                    if (result.Comercios != null && result.Comercios.Any())
                    {
                        foreach (var comercio in result.Comercios)
                        {
                            Console.WriteLine("Comercio: " + comercio.Nombre + ", MerchantId: " + comercio.IdMerchant);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se obtuvieron comercios");
                    }
                }
            }
        }

        private static void CardOrAssociationDown()
        {
            var cliente = new ServiceReference.VNPAccessClient();

            var fileName = ConfigurationManager.AppSettings["BajaServicioJson"];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", Folder, fileName);
            NLogLogger.LogEvent(NLogType.Info, path);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var info = JsonConvert.DeserializeObject<IEnumerable<BajaServicioModel>>(json, new JsonSerializerSettings()
                {
                    Culture = _culture
                });

                foreach (var consulta in info)
                {
                    var data = new ServiceReference.BajaTarjetaData()
                    {
                        IdApp = consulta.IdApp,
                        IdUsuario = consulta.IdUsuario,
                        IdTarjeta = consulta.IdTarjeta,
                    };

                    int idOpInt = 0;
                    Int32.TryParse(consulta.IdOperacion, out idOpInt);

                    data.IdOperacion = idOpInt < 0 ? Guid.NewGuid().ToString() : consulta.IdOperacion;

                    var paramsArray = new[]
                        {
                            data.IdApp,
                            data.IdOperacion,
                            data.IdUsuario,
                            data.IdTarjeta
                        };

                    string firmaDigital = PruebaSignature.GenerateSignature(paramsArray);

                    data.FirmaDigital = firmaDigital;

                    var result = cliente.BajaTarjeta(data);
                    Console.WriteLine(result.CodResultado);
                }
            }
        }

    }
}