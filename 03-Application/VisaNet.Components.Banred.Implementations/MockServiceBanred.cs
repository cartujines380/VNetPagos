using System;
using System.Collections.Generic;
using System.ServiceModel;
using VisaNet.Common.Exceptions;
using VisaNet.Components.Banred.Implementations.BanredWsPagosBancos;
using VisaNet.Components.Banred.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Banred.Implementations
{
    public class MockServiceBanred : IServiceBanred
    {
        private readonly PagosBancosClient _pagosBancos;

        public MockServiceBanred()
        {
            _pagosBancos = new PagosBancosClient();
        }

        public ICollection<BillBanredDto> ConsultaFacturas(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte)
        {
            try
            {
                string listaFacturas = string.Empty;
                listaFacturas = LoadData(codigoCuentaEnte[0]);

                var result = 0;
                var billList = new List<BillBanredDto>();
                if (string.IsNullOrEmpty(listaFacturas))
                    return billList;

                if (result != 0) return billList;
                var bills = listaFacturas.Split('|');
                foreach (var bill in bills)
                {
                    var properties = bill.Split(',');
                    //properties[0] nroFactura		
                    //properties[1] fechaVencimiento
                    //properties[2] descripcion		
                    //properties[3] montoFactura	
                    //properties[4] moneda			
                    //properties[5] pagable	1 = La deuda se puede pagar
                    //        2 = La deuda no se puede pagar por estar vencida
                    //        3 = La deuda no se puede pagar porque existen deudas impagas anteriores ( las facturas ya vencidas deben pagarse primero) 
                    //properties[6] pagoMinimo		
                    //properties[7] formaPago
                    //        1 = Solo Total
                    //        2 = Total y Mínimo
                    //        3 =  Parcial (cualquier monto entre el total y el mínimo)

                    billList.Add(new BillBanredDto
                                 {
                                     Number = properties[0],
                                     ExpirationDate =
                                         new DateTime(Int32.Parse(properties[1].Substring(0, 4)),
                                         Int32.Parse(properties[1].Substring(4, 2)),
                                         Int32.Parse(properties[1].Substring(6, 2))),
                                     Description = properties[2],
                                     Amount = Double.Parse(properties[3]) / 100,
                                     Currency = properties[4].Equals("N") ? "UYU" : "USD",
                                     Payable = (properties[5].Equals("1") || properties[5].Equals("3")),
                                     FinalConsumer = true,
                                     TaxedAmount = Double.Parse(properties[3]) / 100,
                                 });
                }
                return billList;
            }
            catch (Exception e)
            {
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
            }
        }
        
        public int CheckAccount(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte)
        {
            return 1;
        }

        //properties[0] nroFactura		
        //properties[1] fechaVencimiento
        //properties[2] descripcion		
        //properties[3] montoFactura	
        //properties[4] moneda			
        //properties[5] pagable	1 = La deuda se puede pagar
        //        2 = La deuda no se puede pagar por estar vencida
        //        3 = La deuda no se puede pagar porque existen deudas impagas anteriores ( las facturas ya vencidas deben pagarse primero) 
        //properties[6] pagoMinimo		
        //properties[7] formaPago
        //        1 = Solo Total
        //        2 = Total y Mínimo
        //        3 =  Parcial (cualquier monto entre el total y el mínimo)

        private string LoadData(string nroReferencia)
        {
            switch (nroReferencia)
            {
                case "95062533000186": return CaseFacturaImpaga();
                case "95404427000177": return CaseVariasFacturasAPagarConVencidas();
                case "09028109000157": return CaseVariasFacturasAPagar();
                case "12003882000418": return FacturasPorArribaDelMaximo();
                case "12018882000268": throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
                case "pruebaNotificaciones": return NotificationsBills();
            }
            return "";
        }

        private string NotificationsBills()
        {
            var result = "";
            result = result + "VENCIDA 1,20140328,vencida1,42000,N,1,0,1";
            result = result + "|" + "VENCIDA 2,20140428,vencida2,4000,N,1,0,1";
            result = result + "|" + "POR VENCER A FALTA DE UN DIA,20140605,porVencertieneQueGuardarla,48000,N,1,0,1";
            result = result + "|" + "EMITIDA,20140628,emitida,48000,N,1,0,1";
            return result;
        }

        /// <summary>
        /// Devuelve 1 factura impaga,
        ///     Fecha vencimiento: fecha actual + 1 día
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// </summary>
        /// <returns></returns>
        private string CaseFacturaImpaga()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var nroFactura = rnd.Next(10000, 99999);

            var fecha = DateTime.Now.AddDays(1);

            var result = "";
            result = result + "nro " + nroFactura + "," + fecha.ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1";
            
            return result;
        }
                                                                                                                                                                                                                                                                                                                                                   
        /// <summary>
        /// Devuelve 8 facturas impagas,
        ///     Fecha vencimiento: fecha actual + 1*(posicion en array) días
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// </summary>
        /// <returns></returns>
        private string CaseVariasFacturasAPagar()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var fecha = DateTime.Now.Date;

            var lst = new List<string>();
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(2).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(3).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(4).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(5).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(6).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(7).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(8).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro 159357," + fecha.AddMonths(8).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            
            return string.Join("|", lst);
        }


        /// <summary>
        /// Devuelve 4 facturas impagas, 
        /// F1)
        ///     Fecha vencimiento: fecha actual - 2 meses
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// F2)
        ///     Fecha vencimiento: fecha actual - 1 mes
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// F3)
        ///     Fecha vencimiento: fecha actual + 1 día
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// F4)
        ///     Fecha vencimiento: fecha actual + 1 mes + 1 día
        ///     Monto: Random entre 1000 y 9999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// </summary>
        /// <returns></returns>
        private string CaseVariasFacturasAPagarConVencidas()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var fecha = DateTime.Now.Date;

            var lst = new List<string>();
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(-2).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(-1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(1).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(1000, 9999) * 100 + ",N,1,0,1");

            return string.Join("|", lst);
        }

        /// <summary>
        /// Devuelve 8 facturas impagas,
        ///     Fecha vencimiento: fecha actual + 1*(posicion en array) días
        ///     Monto: Random entre 10001 y 99999
        ///     Moneda: Pesos
        ///     Pagable: Si
        ///     PagoMinimo: N/A
        ///     FormaPago: Solo total
        /// </summary>
        /// <returns></returns> 
        private string FacturasPorArribaDelMaximo()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var fecha = DateTime.Now.Date;

            var lst = new List<string>();
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(10001, 99999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(2).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(10001, 99999) * 100 + ",N,1,0,1");
            lst.Add("nro " + rnd.Next(10000, 99999) + "," + fecha.AddMonths(3).AddDays(1).ToString("yyyyMMdd") + ",desc," + rnd.Next(10001, 99999) * 100 + ",N,1,0,1");
            return string.Join("|", lst);
        }

        public string PagarFactura(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte, string nroFactura,
            double montoPago, double montoDescuentoIva, string monedaPago, string fechaVencimiento, string transactionNumber)
        {
            try
            {
                var result = 0;//_pagosBancos.pagarFacturaEnte(idAgenteExterno, codigoEnte, codigoCuentaEnte, nroFactura, montoPago, monedaPago, fechaVencimiento, idOperacionAgente, firmaDigital, out textoResultado, out nroTransaccion, out fechaHoraPago);


                //var rnd = new Random();

                //if (rnd.Next(1, 4) == 3)
                //    throw new ProviderFatalException("error");

                if (result != 0)
                {
                    throw new ProviderFatalException("error");
                }
                var rnd = new Random();
                
                return rnd.Next(10000, 99999) + "";
            }
            catch (TimeoutException e)
            {
                //Console.WriteLine("TimeoutException: " + e.Message + "\n" + e.StackTrace);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_COMMUNICATION);
            }
            catch (FaultException e)
            {
                //Console.WriteLine("FaultException: " + e.Message + "\n" + e.StackTrace);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_FAULT);
            }
            catch (CommunicationException e)
            {
                //Console.WriteLine("CommunicationException: " + e.Message + "\n" + e.StackTrace);
                throw new ProviderWithoutConectionException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
            }
        }
    }
}
