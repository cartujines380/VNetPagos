using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Geocom.GCA_02;
using Geocom.GCA_03;
using Geocom.GCA_04;
using Geocom.GCA_05;
using Geocom.GCA_08;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Components.Geocom.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Geocom
{
    public class GeocomCa : BaseGeocom, IGeocomCa
    {
        
        private readonly ILoggerService _loggerService;

        public GeocomCa(ILoggerService loggerService, IServiceFixedNotification serviceFixedNotification)
            : base(serviceFixedNotification)
        {
            _loggerService = loggerService;
        }

        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaCir(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //   var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 2,
        //            ValorParametro = ref1, //NRO DE PADRON
        //        },
        //    };

        //   return list;

        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaCiu(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 10,
        //            ValorParametro = ref1,//NRO. PADRON
        //        },
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 2,
        //            TipoBusqueda = 10,
        //            ValorParametro = ref2,//UNIDAD
        //        },
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 3,
        //            TipoBusqueda = 10,
        //            ValorParametro = ref3,//BLOCK
        //        },
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 4,
        //            TipoBusqueda = 10,
        //            ValorParametro = ref4,//LOCALIDAD
        //        }
        //    };
        //    return list;
        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaCnv(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
            
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 2,
        //            ValorParametro = ref1,//NRO DE  CONVENIO
        //        }
        //    };
        //    return list;
        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaCom(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 2,
        //            ValorParametro = ref1,//FICHA
        //        },
        //    };
        //    return list;
        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaSem(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 2,
        //            ValorParametro = ref1,//DICOSE
        //        },
        //    };
        //    return list;
        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaNec(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 1,
        //            ValorParametro = ref1,//ID DE PADRON
        //        },
        //    };

        //    return list;

        //}
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaVar(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>()
        //    {
        //        new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 1,
        //            ValorParametro = ref1,//TRAMITE
        //        },
        //    };
        //    return list;
        //}
        ////ref1 es CI, ref2 es RUT. Se ingresa uno u otro
        //private List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem> BusquedaCon(string ref1, string ref2, string ref3, string ref4, string ref5, string ref6, string param)
        //{
        //    var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>();

        //    if (!string.IsNullOrEmpty(ref1))
        //    {
        //        list.Add(new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //        {
        //            NumeroParametro = 1,
        //            TipoBusqueda = 1,
        //            ValorParametro = ref1, //CI
        //        });
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(ref2))
        //        {
        //            list.Add(new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
        //            {
        //                NumeroParametro = 1,
        //                TipoBusqueda = 2,
        //                ValorParametro = ref2,//NRO DE R.U.T.
        //            });
        //        }
        //    }
        //    return list;
        //}
        
        private int BusquedaCc(string[] refs, string param, string type)
        {
          
            var suciveService = new Ws02BusquedaCCSoapPortClient();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);


            var codigoretorno = "";
            var mensajeretorno = "";

            //var list = SwithParam(refs, param);

            var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>();
            for (int i = 0; i < refs.Count(); i++)
            {
                if (!string.IsNullOrEmpty(refs[i]) || (param.Equals("CIU") && i < 4))
                {
                    list.Add(new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
                    {
                        NumeroParametro = i+1,
                        TipoBusqueda = type == null ? 0 : int.Parse(type),
                        ValorParametro = refs[i],
                    });
                }    
            }
            
            var result = suciveService.Execute(param, list.ToArray(), out codigoretorno, out mensajeretorno);

            short number = 0;
            Int16.TryParse(codigoretorno, out number);
            NotificationFix(number, mensajeretorno, -1, refs, string.Empty, param, "Ws02BusquedaCCSoapPortClient", DepartamentDtoType.Canelones, string.Empty);

            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Geocom BusquedaCc Canelones. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.GEOCOM_NORESPONSE);
            }
            if (result.Count() > 1)
            {
                NLogLogger.LogEvent(NLogType.Error, "Geocom BusquedaCc Canelones. Mas de un resultado devuelto");
                if (param.Equals("CIU"))
                {
                    throw new BillException("Los datos ingresados coinciden con más de una cuenta. Por favor, refine la búsqueda completando los campos faltantes");
                }
                throw new ProviderFatalException(CodeExceptions.GEOCOM_MULTIPLEIDPADRON);
            }

            if (number > 0)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Geocom BusquedaCc Canelones. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                return -1;
            }

            var tmp = result.FirstOrDefault();
            if (tmp != null)
                return tmp.CodigoIDPadron;

            return -1;

        }

        private ResultCaGetConsultaDeudaCc GetConsultaDeudaCc(int idPadron, string param, string cc)
        {
            var suciveService = new Ws03ConsultaDeudaCCSoapPortClient();

            int auxiliarCobro;
            string codigoretorno;
            string mensajeretorno;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);


            var result = suciveService.Execute(idPadron, cc, param, out auxiliarCobro, out codigoretorno, out mensajeretorno);

            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Geocom GetConsultaDeudaCc Canelones. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.GEOCOM_NORESPONSE);
            }

            short number = 0;
            Int16.TryParse(codigoretorno, out number);
            NotificationFix(number, mensajeretorno, idPadron, null, string.Empty, param, "Ws03ConsultaDeudaCCSoapPortClient", DepartamentDtoType.Canelones, string.Empty);

            if (number > 0)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Geocom GetConsultaDeudaCc Canelones. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                if (number == 001)
                {
                    throw new BillException(PresentationWebStrings.Payment_General_Error);
                }
                if (number == 004)
                {
                    return new ResultCaGetConsultaDeudaCc { AuxiliarCobro = -4};    
                }
                if (number == 015)
                {
                    GetConsultaDeudaCc(idPadron, param, "S");
                }
                if (number == 090)
                {
                    throw new BillException(ExceptionMessages.SUCIVE_ERROR090);    
                }
            }
            if (number == 000)
            {
                return new ResultCaGetConsultaDeudaCc { AuxiliarCobro = auxiliarCobro, List = result };
            }

            throw new ProviderFatalException(CodeExceptions.GEOCOM_NORESPONSE);
        }

        private BillGeocomDto GetConsultaDeuda(int auxiliarCobro, GCA_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] array, string param, int idPadron)
        {
            var suciveService = new Ws04BConsultaDeudaSoapPortClient();
            
            string codigoretorno;
            string mensajeretorno;
            SDT_ColeccionTotalesSDT_ColeccionTotalesItem[] totales = null;
            //SDT_ColeccionDetalleSDT_ColeccionDetalleItem[] detalle = null;
            SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem[] cabezales = null;
            SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem[] mensajes = null;
            int numeroPreFactura;
            double montoTotal;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);


            var result = suciveService.Execute(auxiliarCobro, array, out totales, out cabezales, out mensajes, out numeroPreFactura, out montoTotal, out codigoretorno, out mensajeretorno);

            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Geocom GetConsultaDeuda Canelones. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.GEOCOM_NORESPONSE);
            }

            short number = 0;
            Int16.TryParse(codigoretorno, out number);

            NotificationFix(number, mensajeretorno, idPadron, null, string.Empty, param, "Ws04BConsultaDeudaSoapPortClient", DepartamentDtoType.Canelones, string.Empty);

            var desc = result.OrderBy(b => b.Vencimiento).Select(b => b.Concepto);

            var date = DateTime.Now;
            if (result.FirstOrDefault() != null)
            {
                var str = result.FirstOrDefault().Vencimiento.Split('/');
                date = new DateTime(Int16.Parse(str[2].Count() == 2 ? "20" + str[2] : str[2]), Int16.Parse(str[1]), Int16.Parse(str[0]));
            }

            BillGeocomDto finalBill = null;
            //ConfirmarPago(numeroPreFactura, "0");
            if (codigoretorno.Equals("000"))
            {
                finalBill = new BillGeocomDto()
                {
                    Amount = montoTotal,
                    GeocomPreBillNumber = numeroPreFactura.ToString(),
                    Currency = "UYU",
                    Description = String.Join(" - ", desc),
                    ExpirationDate = date,
                    Payable = true,
                    Details = new List<BillGeocomDto>()
                };
                foreach (var temp in result)
                {
                    var amount = temp.Importe.Replace(".", ",");
                    var b = new BillGeocomDto()
                    {
                        Currency = "UYU",
                        ExpirationDate = date,
                        Amount = Double.Parse(amount),
                        Codigo = temp.CodigoConcepto.Trim(),
                        Payable = true,
                        Description = temp.Concepto + (temp.CodigoConcepto.Contains("200") ? " " + temp.Cuota + " " + temp.Anio : ""),
                        Year = temp.Anio,
                    };
                    finalBill.Details.Add(b);
                }
                return finalBill;
            }

            NLogLogger.LogEvent(NLogType.Error, "Geocom GetConsultaDeuda Canelones. Devulvo pre bill number -1");
            return new BillGeocomDto() { GeocomPreBillNumber = "-1", Description = mensajeretorno };
            
        }

        public long ConfirmarPago(int numeroPreFactura, string sucursal, string transactionId, string param)
        {
            var suciveservice = new Ws05ConfirmacionSoapPortClient();

            string codigoretorno;
            string mensajeretorno;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);


            var result = suciveservice.Execute(numeroPreFactura, sucursal, out codigoretorno, out mensajeretorno);

            short number = 0;
            Int16.TryParse(codigoretorno, out number);
            NotificationFix(number, mensajeretorno, -1, null, numeroPreFactura.ToString(), param, "Ws05ConfirmacionSoapPortClient", DepartamentDtoType.Canelones,transactionId);

            if (!codigoretorno.Equals("000"))
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Error al pagar sucive. Codigo: {0}, Mensaje {1}, transaccionid {2}, prefactura {3}", codigoretorno, mensajeretorno, transactionId, numeroPreFactura));
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Geocom,
                    string.Format(LogStrings.Payment_NotifyGeocom_Error, mensajeretorno + ", cod:" + codigoretorno, transactionId, numeroPreFactura));
                return -1;
            }
            else
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Geocom,
                    string.Format(LogStrings.Payment_NotifyGeocom_Done, transactionId, result));
            }

            //ReversoPago(numeroPreFactura, 102522);
            
            return result;
            
        }
        public List<BillGeocomDto> GetBills(string[] refs, string param, string type)
        {
            var id = -1;
            switch (param)
            {
                case "CON":
                    id = IdPadronCon(refs, param, type);
                    break;
                case "SEM":
                    id = IdPadronSem(refs, param, type);
                    break;
                default:
                    id = BusquedaCc(refs, param, type);
                    break;
            }
            var list = GetBillsWithCc(id, param);
            return list;
        }

        public List<BillGeocomDto> GetBillsWithCc(int idPadron, string param)
        {

            var result = GetConsultaDeudaCc(idPadron, param, "N");
            var listado = new List<BillGeocomDto>();
            BillGeocomDto bill = null;

            //No posee deuda
            if (result == null || result.AuxiliarCobro == -4)
                return listado;

            foreach (var temp in result.List)
            {
                var amount = temp.Importe.Replace(".", ",");
                var currentBill = new BillGeocomDto()
                {
                    Currency = "UYU",
                    Amount = Double.Parse(amount),
                    Codigo = temp.CodigoConcepto.Trim(),
                    Line = temp.Linea + ";",
                    Payable = true,
                    Allowed = temp.Permitido,
                    Description = temp.Concepto + (temp.CodigoConcepto.Contains("200") ? " " + temp.Cuota + " " + temp.Anio : ""),
                    IdPadron = idPadron,
                    Year = temp.Anio,
                    Details = new List<BillGeocomDto>()
                };

                if (temp.Permitido.Equals("S"))
                {
                    var str = temp.Vencimiento.Split('/');
                    currentBill.ExpirationDate = new DateTime(Int16.Parse(str[2].Count() == 2 ? "20" + str[2] : str[2]), Int16.Parse(str[1]), Int16.Parse(str[0]));
                    listado.Add(currentBill);
                    bill = currentBill;
                }
                if (temp.Permitido.Equals("N"))
                {
                    if (bill != null)
                    {
                        bill.Line = bill.Line + temp.Linea + ";";
                        bill.Details.Add(currentBill);
                    }
                }
            }
            return listado;
        }

        public BillGeocomDto CheckIfBillPayable(string lines, int idPadron, string param)
        {
            var arrayLines = lines.Split(';');
            var result = GetConsultaDeudaCc(idPadron, param, "N");

            var onebill = 0;

            var list = new List<GCA_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem>();
            foreach (var item in result.List)
            {
                var cobranza = new GCA_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem()
                {
                    Anio = item.Anio,
                    CodigoConcepto = item.CodigoConcepto,
                    Concepto = item.Concepto,
                    Cuota = item.Cuota,
                    Importe = item.Importe,
                    Linea = item.Linea,
                    Permitido = item.Permitido,
                    Vencimiento = item.Vencimiento,
                    Paga = "N"
                };
                var line = item.Linea.ToString();
                if (arrayLines.Contains(line))
                {
                    cobranza.Paga = "S";
                    onebill = cobranza.Permitido == "S" ? onebill + 1 : onebill;
                }
                list.Add(cobranza);
            }
            var preBill = GetConsultaDeuda(result.AuxiliarCobro, list.ToArray(), param, idPadron);

            //Si hay mas de una bill seleccionada, le pongo la fecha de vencimiento de la primera.
            if (onebill > 1)
            {
                var date = list.FirstOrDefault().Vencimiento.Split('/');
                preBill.ExpirationDate = new DateTime(Int16.Parse(date[2].Count() == 2 ? "20" + date[2] : date[2]), Int16.Parse(date[1]), Int16.Parse(date[0]));
            }

            if (!preBill.GeocomPreBillNumber.Equals("-1"))
                preBill.IdPadron = idPadron;
            return preBill;
        }

        public int CheckAccount(string[] refs, string param, string type)
        {

            if (type.Equals("1"))
            {
                var idPadron = int.Parse(refs[0]);
                var bills = GetBillsWithCc(idPadron, param);
                //existe el id padron
                if (bills != null)
                    return 1;
            }

            return param.Equals("CON") ?
            IdPadronCon(refs, param, type) :
            BusquedaCc(refs, param, type);
        }

        //public List<BillGeocomDto> prueba()
        //{
        //    var a = new BillGeocomDto()
        //            {
        //                ExpirationDate = DateTime.Today.AddDays(200),
        //                Codigo = "200",
        //                Line = "100;",
        //                Year = "2015",
        //                Payable = true
        //            };
        //    var b = new BillGeocomDto()
        //    {
        //        ExpirationDate = DateTime.Today.AddDays(1),
        //        Codigo = "200",
        //        Line = "101;",
        //        Year = "2015",
        //        Payable = true
        //    };
        //    return new List<BillGeocomDto>(){a,a,a,a,a,a};

        //}

        public List<ConciliationGeocomDto> Conciliation(DateTime date)
        {
            var suciveService = new Ws08CobradoDetalleSoapPortClient();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var list = new List<ConciliationGeocomDto>();
            //var codigoretorno = "";
            //var mensajeretorno = "";
            //double totalcobrado = 0;
            //var cantidadcobros = 0;
            //var result = suciveService.Execute(0, 0, date , out totalcobrado, out cantidadcobros, out codigoretorno, out mensajeretorno);
            
            //foreach (var item in result)
            //{
            //    var dto = new ConciliationGeocomDto()
            //              {
            //                  Amount = item.MontodelCobro,
            //                  BillExternalId = item.NumerodeCobro.ToString(),
            //                  Currency = "UYU",
            //                  Date = date,
            //                  Departament = DepartamentDtoType.Canelones
            //              };
            //    list.Add(dto);
            //}
            return list;
        }

        public int IdPadronCon(string[] refs, string param, string type)
        {
            //CEDULA
            var id = -1;
            var useCi = true;
            var data = refs[0];
            var originalValue = refs[0];
            if (data.Count() > 9)
            {
                useCi = false;
            }
            if (!param.Equals("CON")) return id;
            var newtype = useCi ? 1 : 2;
            if (useCi)
            {
                //Si es cedula, agrego la -
                refs[0] = FixCi(originalValue);
            }
            id = BusquedaCc(refs, param, newtype.ToString());
            
            if (id != -1) return id;

            refs[0] = originalValue;
            newtype = newtype == 1 ? 2 : 1;
            if (newtype == 1)
            {
                refs[0] = FixCi(originalValue);
            }
            id = BusquedaCc(refs, param, newtype.ToString());
            return id;
        }

        public int IdPadronSem(string[] refs, string param, string type)
        {
            //Discose
            var id = -1;
            if (!param.Equals("SEM")) return id;
            //Primero busco por Discose
            id = BusquedaCc(refs, param, "2");

            if (id != -1) return id;
            //sino por matrícula
            id = BusquedaCc(refs, param, "3");
            return id;
        }

        private string FixCi(string value)
        {
            var max = value.Count();
            var list = value.Substring(0, max - 1);
            return list + "-" + value.Substring(max - 1);
        }
    }
    
   
    public class ResultCaGetConsultaDeudaCc
    {
        public GCA_03.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] List { get; set; }
        public int AuxiliarCobro { get; set; }
    }
    
}
