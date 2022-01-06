using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sucive.RN_02;
using Sucive.RN_03;
using Sucive.RN_04;
using Sucive.RN_05;
using Sucive.RN_06;
using Sucive.RN_08;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Sucive
{
    public class Sucive12 : BaseSucive, ISucive12
    {

        private readonly ILoggerService _loggerService;

        public Sucive12(ILoggerService loggerService, IServiceFixedNotification serviceFixedNotification)
            : base(serviceFixedNotification)
        {
            _loggerService = loggerService;
        }

        private int BusquedaCc(string[] refs, string param, string type)
        {
            var suciveService = new Ws02BusquedaCCSoapPortClient();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var codigoretorno = "";
            var mensajeretorno = "";

            var list = new List<SDT_BusquedaCargadaSDT_BusquedaCargadaItem>();
            for (int i = 0; i < refs.Count(); i++)
            {
                if (!string.IsNullOrEmpty(refs[i]))
                {
                    list.Add(new SDT_BusquedaCargadaSDT_BusquedaCargadaItem
                    {
                        NumeroParametro = i + 1,
                        TipoBusqueda = int.Parse(type),
                        ValorParametro = refs[i],
                    });
                }
            }

            var result = suciveService.Execute(param, list.ToArray(), out codigoretorno, out mensajeretorno);


            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Sucive BusquedaCc. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.SUCIVE_NORESPONSE);
            }
            if (result.Count() > 1)
            {
                NLogLogger.LogEvent(NLogType.Error, "Sucive BusquedaCc. Mas de un resultado devuelto");
                throw new ProviderFatalException(CodeExceptions.SUCIVE_MULTIPLEIDPADRON);
            }

            short number = 0;
            Int16.TryParse(codigoretorno, out number);
            if (number > 0)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Sucive BusquedaCc. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                return -1;
            }

            var tmp = result.FirstOrDefault();
            if (tmp != null)
                return tmp.CodigoIDPadron;

            return -1;

        }

        private Result12GetConsultaDeudaCc GetConsultaDeudaCc(int idPadron, string param)
        {
            var suciveService = new Ws03ConsultaDeudaCCSoapPortClient();

            long auxiliarCobro;
            string codigoretorno;
            string mensajeretorno;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var result = suciveService.Execute(idPadron, "S", param, out auxiliarCobro, out codigoretorno, out mensajeretorno);

            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Sucive GetConsultaDeudaCc. IdPadron: {0}, codigoretorno: {1}, mensajeretorno: {2}", idPadron, codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.SUCIVE_NORESPONSE);
            }

            short number = 0;
            Int16.TryParse(codigoretorno, out number);
            if (number > 0)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Sucive GetConsultaDeudaCc. IdPadron: {0}, codigoretorno: {1}, mensajeretorno: {2}", idPadron, codigoretorno, mensajeretorno));
                if (number == 004)
                {
                    return new Result12GetConsultaDeudaCc { AuxiliarCobro = -4 };
                }
            }

            if (codigoretorno.Equals("000"))
            {
                return new Result12GetConsultaDeudaCc { AuxiliarCobro = auxiliarCobro, List = result };
            }
            return null;
        }

        private BillSuciveDto GetConsultaDeuda(long auxiliarCobro, RN_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] array)
        {
            var suciveService = new Ws04BConsultaDeudaSoapPortClient();

            string codigoretorno;
            string mensajeretorno;
            SDT_ColeccionTotalesSDT_ColeccionTotalesItem[] totales = null;
            //SDT_ColeccionDetalleSDT_ColeccionDetalleItem[] detalle = null;
            SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem[] cabezales = null;
            SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem[] mensajes = null;
            long numeroPreFactura;
            double montoTotal;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var result = suciveService.Execute(auxiliarCobro, array, out totales, out cabezales, out mensajes, out numeroPreFactura, out montoTotal, out codigoretorno, out mensajeretorno);

            if (result == null)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Sucive GetConsultaDeuda. codigoretorno: {0}, mensajeretorno: {1}", codigoretorno, mensajeretorno));
                throw new ProviderFatalException(CodeExceptions.SUCIVE_NORESPONSE);
            }

            var desc = result.OrderBy(b => b.Vencimiento).Select(b => b.Concepto);

            var date = DateTime.Now;
            if (result.FirstOrDefault() != null)
            {
                var str = result.FirstOrDefault().Vencimiento.Split('/');
                date = new DateTime(Int16.Parse("20" + str[2]), Int16.Parse(str[1]), Int16.Parse(str[0]));
            }
            BillSuciveDto finalBill = null;
            //ConfirmarPago(numeroPreFactura, "0");
            if (codigoretorno.Equals("000"))
            {
                finalBill = new BillSuciveDto()
                {
                    Amount = montoTotal,
                    SucivePreBillNumber = numeroPreFactura.ToString(),
                    Currency = "UYU",
                    Description = String.Join(" - ", desc),
                    ExpirationDate = date,
                    Payable = true,
                    Details = new List<BillSuciveDto>()
                };
                foreach (var temp in result)
                {
                    var amount = temp.Importe.Replace(".", ",");
                    var b = new BillSuciveDto()
                    {
                        Currency = "UYU",
                        ExpirationDate = date,
                        Amount = Double.Parse(amount),
                        Codigo = temp.CodigoConcepto,
                        Payable = true,
                        Description = temp.Concepto + (temp.CodigoConcepto.Contains("200") ? " " + temp.Cuota + " " + temp.Anio : ""),
                        Year = temp.Anio,
                    };
                    finalBill.Details.Add(b);
                }
                return finalBill;
            }
            NLogLogger.LogEvent(NLogType.Error, "Geocom GetConsultaDeuda. Devulvo pre bill number -1");
            return new BillSuciveDto() { SucivePreBillNumber = "-1", Description = mensajeretorno };

        }

        public long ConfirmarPago(long numeroPreFactura, string sucursal, string transactionId)
        {
            var suciveservice = new Ws05ConfirmacionSoapPortClient();

            string codigoretorno;
            string mensajeretorno;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var result = suciveservice.Execute(numeroPreFactura, sucursal, out codigoretorno, out mensajeretorno);
            NotificationFix(numeroPreFactura.ToString(),transactionId,codigoretorno,mensajeretorno,DepartamentDtoType.Rio_Negro, null);

            if (!codigoretorno.Equals("000"))
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("Error al pagar sucive. Codigo: {0}, Mensaje {1}, transaccionid {2}", codigoretorno, mensajeretorno, transactionId));
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sucive,
                    string.Format(LogStrings.Payment_NotifySucive_Error, mensajeretorno + ", cod:" + codigoretorno, transactionId, numeroPreFactura));
                return -1;
            }
            else
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sucive,
                    string.Format(LogStrings.Payment_NotifySucive_Done, transactionId, result, numeroPreFactura));
            }

            //ReversoPago(numeroPreFactura, 102522);

            return result;

        }

        private void ReversoPago(long numeroPreFactura, int idPadron)
        {
            var suciveservice = new Ws06ReversoSoapPortClient();
            string mensajeretorno;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var result = suciveservice.Execute(numeroPreFactura, idPadron, out mensajeretorno);

            if (result == null)
                throw new ProviderFatalException(CodeExceptions.SUCIVE_NORESPONSE);
        }

        public List<BillSuciveDto> GetBills(string[] refs, string param, string type)
        {
            var id = BusquedaCc(refs, param, type);
            var list = GetBillsWithCc(id, param);
            //list.AddRange(prueba());
            return list;
        }

        public List<BillSuciveDto> GetBillsWithCc(int idPadron, string param)
        {

            var result = GetConsultaDeudaCc(idPadron, param);

            var listado = new List<BillSuciveDto>();
            BillSuciveDto bill = null;

            //No posee deuda
            if (result == null || result.AuxiliarCobro == -4)
                return listado;

            foreach (var temp in result.List)
            {
                var amount = temp.Importe.Replace(".", ",");
                var currentBill = new BillSuciveDto()
                {
                    Currency = "UYU",
                    Amount = Double.Parse(amount),
                    Codigo = temp.CodigoConcepto,
                    Line = temp.Linea + ";",
                    Payable = true,
                    Allowed = temp.Permitido,
                    Description = temp.Concepto + (temp.CodigoConcepto.Contains("200") ? " " + temp.Cuota + " " + temp.Anio : ""),
                    IdPadron = idPadron,
                    Year = temp.Anio,
                    Details = new List<BillSuciveDto>()
                };

                if (temp.Permitido.Equals("S"))
                {
                    var date = temp.Vencimiento.Split('/');
                    currentBill.ExpirationDate = new DateTime(Int16.Parse("20" + date[2]), Int16.Parse(date[1]),
                        Int16.Parse(date[0]));
                    listado.Add(currentBill);
                    bill = currentBill;
                }
                if (temp.Permitido.Equals("N"))
                {
                    if (bill != null)
                    {
                        //bill.Amount = bill.Amount + Double.Parse(amount);
                        bill.Line = bill.Line + temp.Linea + ";";
                        bill.Details.Add(currentBill);
                    }
                }
            }
            return listado;
        }

        public BillSuciveDto CheckIfBillPayable(string lines, int idPadron, string param)
        {
            var arrayLines = lines.Split(';');
            var result = GetConsultaDeudaCc(idPadron, param);

            var list = new List<RN_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem>();
            foreach (var item in result.List)
            {
                var cobranza = new RN_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem()
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
                }

                list.Add(cobranza);
            }
            var preBill = GetConsultaDeuda(result.AuxiliarCobro, list.ToArray());
            if (!preBill.SucivePreBillNumber.Equals("-1"))
                preBill.IdPadron = idPadron;
            return preBill;
        }

        public int CheckAccount(string[] references, string param, string type)
        {
            return BusquedaCc(references, param, type);
        }

        //public List<BillSuciveDto> prueba()
        //{
        //    var a = new BillSuciveDto()
        //            {
        //                ExpirationDate = DateTime.Today.AddDays(200),
        //                Codigo = "200",
        //                Line = "100;",
        //                Year = "2015",
        //                Payable = true
        //            };
        //    var b = new BillSuciveDto()
        //    {
        //        ExpirationDate = DateTime.Today.AddDays(1),
        //        Codigo = "200",
        //        Line = "101;",
        //        Year = "2015",
        //        Payable = true
        //    };
        //    return new List<BillSuciveDto>(){a,a,a,a,a,a};

        //}

        public List<ConciliationSuciveDto> Conciliation(DateTime date)
        {
            var suciveService = new Ws08bCobradoDetalleSoapPortClient();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 5000;

            var list = new List<ConciliationSuciveDto>();
            var codigoretorno = "";
            var mensajeretorno = "";
            double totalcobrado = 0;
            var cantidadcobros = 0;
            var result = suciveService.Execute(0, 0, date, out totalcobrado, out cantidadcobros, out codigoretorno, out mensajeretorno);

            foreach (var item in result)
            {
                var dto = new ConciliationSuciveDto()
                {
                    Amount = item.MontodelCobro,
                    BillExternalId = item.NumerodeCobro.ToString(),
                    Currency = "UYU",
                    Date = date,
                    Departament = DepartamentDtoType.Rio_Negro
                };
                list.Add(dto);
            }
            return list;
        }
    }


    public class Result12GetConsultaDeudaCc
    {
        public RN_03.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] List { get; set; }
        public long AuxiliarCobro { get; set; }
    }
    //public class ResultBusquedaCc
    //{
    //    public int IdPadron { get; set; }
    //    public ResultError Error { get; set; }
    //}
    //public class ResultGetConsultaDeuda
    //{
    //    public long NumeroPreFactura { get; set; }
    //    public ResultError Error { get; set; }
    //}

    //public class ResultError
    //{
    //    public string CodigoRetorno { get; set; }
    //    public string MensajeRetorno { get; set; }
    //}


}
