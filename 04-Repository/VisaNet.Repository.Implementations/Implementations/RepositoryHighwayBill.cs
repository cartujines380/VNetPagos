using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryHighwayBill : BaseRepository<HighwayBill>, IRepositoryHighwayBill
    {
        public RepositoryHighwayBill(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {

        }
        int countlocalP = 0;
        int countlocalD = 0;
        long totallocalP = 0;
        long totallocalD = 0;

        private int DeleteAllBillsForServiceId(Guid serviceId, Guid emailId, bool deleteBillsFromEmail)
        {
            //var sql = "DELETE FROM [dbo].[HighwayBills] WHERE ServiceId = @id AND HighwayEmailId != @emailId";
            //var sqlBillData = new SqlParameter[] { new SqlParameter("@id", serviceId), new SqlParameter("@emailId", emailId) };
            //_db.ExecuteSqlCommand(sql, sqlBillData.ToArray<object>());
            var result = 0;
            var count = 0;
            string sql = "SP_VisaNet_DeleteHighwayBills";

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["AppContext"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    #region Set up parameters
                    command.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    #endregion

                    #region Set parameter values
                    command.Parameters.AddWithValue("@servicio", serviceId);
                    command.Parameters.AddWithValue("@emailid", emailId);
                    command.Parameters.AddWithValue("@filas", 1000);
                    command.Parameters.AddWithValue("@emailequal", deleteBillsFromEmail);

                    #endregion

                    // Open connection and execute stored procedure
                    connection.Open();
                    var rows = command.ExecuteNonQuery();

                    #region Read output values
                    //Payments
                    result = Convert.ToInt16(command.Parameters["@result"].Value);
                    count = Convert.ToInt32(command.Parameters["@count"].Value);

                    #endregion

                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("DeleteAllBillsForServiceId - Excepcion capturada"));
                NLogLogger.LogHighwayFileProccessEvent(exception);
                return 0;
            }

            return result;
        }

        public string UserAction()
        {
            return TransactionContext.UserName;
        }

        //private int InserBill(SqlParameter[] parameters)
        //{
        //    if (_db.Connection.State == ConnectionState.Open)
        //    {
        //        var sql = "insert into [dbo].[highwaybills] ([id] ,[creationuser],[lastmodificationuser],[creationdate],[lastmodificationdate],[codcomercio],[codsucursal],[refcliente]," +
        //                  "[nrofactura],[descripcion],[fchfactura],[fchvencimiento],[diaspagovenc],[moneda],[montototal],[montogravado],[consfinal],[cuotas],[highwayemailid],[serviceid]," +
        //                  "[montominimo],[pagodebito],[type],[refcliente2],[refcliente3],[refcliente4],[refcliente5],[refcliente6],[errorcode])" +
        //                  "values (@id,@creationuser,@lastmodificationuser,@creationdate,@lastmodificationdate,@codcomercio,@codsucursal,@refcliente,@nrofactura," +
        //                  "@descripcion,@fchfactura,@fchvencimiento,@diaspagovenc,@moneda,@montototal,@montogravado,@consfinal,@cuotas," +
        //                  "@highwayemailid,@serviceid,@montominimo,@pagodebito,@type,@refcliente2,@refcliente3,@refcliente4,@refcliente5,@refcliente6, @errorcode)";


        //        return _db.ExecuteSqlCommand(sql, parameters.ToArray<object>());    
        //    }
        //    return -1;
        //}

        public List<String> MasiveInsert(string[] lines, int codCommerce, int codBranch, Guid emailId, Guid serviceId, out int countN, out int countD, out double valueN, out double valueD)
        {
            var generalErrors = new List<String>();
            var commitOk = false;
            var countcommits = 1;
            var excepCode = 0;
            while (countcommits < 6 && !commitOk)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Intento Nro " + countcommits));
                if (countcommits > 1)
                {
                    SqlConnection.ClearAllPools();
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Limpio pools conecction"));
                    if (excepCode == 1205)
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - SqlExcepcion 1205"));
                        Thread.Sleep(3000);
                    }
                }

                generalErrors.Clear();

                using (var transaction = _db.BeginTransaction())
                {
                    try
                    {
                        Parallel.ForEach(lines, new ParallelOptions { MaxDegreeOfParallelism = 20 }, x =>
                        {
                            var errors = AnalizeLine(x, codCommerce, codBranch, emailId, serviceId, lines);
                            generalErrors.AddRange(errors);
                        });

                        countcommits++;

                        if (generalErrors.Any())
                        {
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Hay errores - Intento Rollback"));
                            transaction.Rollback();
                        }
                        else
                        {
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - No hay errores - Intento commit"));
                            transaction.Commit();
                            transaction.Dispose();
                            commitOk = true;
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Cantidad en Pesos {0}, monto en pesos {1}, cantidad en dolares {2}, monto en dolares {3}",
                                            countlocalP, totallocalP / 100, countlocalD, totallocalD / 100));
                        }
                    }
                    catch (Exception exception)
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Exception"));
                        NLogLogger.LogHighwayFileProccessEvent(exception);
                        generalErrors.Add(string.Format("Error en archivo adjunto. Error en el procesamiento de facturas."));

                        var sqlException = exception as SqlException;
                        if (sqlException != null)
                            excepCode = sqlException.Number;
                    }
                }
            }

            if (commitOk)
            {
                var count = 0;
                var ready = false;
                var result = 0;
                while (count < 3 && !ready)
                {
                    result = DeleteAllBillsForServiceId(serviceId, emailId, false);
                    if (result == 1)
                        ready = true;
                    else
                        count++;
                }

                //si no pude borrar las viejas, elimino las nuevas
                if (!ready)
                {
                    generalErrors.Add(string.Format("Error en archivo adjunto. Error en el procesamiento de facturas."));
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Error al intentar eliminar facturas viejas. Procesdo a eliminar facturas nuevas."));
                    count = 0;
                    result = 0;
                    while (count < 3 && !ready)
                    {
                        result = DeleteAllBillsForServiceId(serviceId, emailId, true);
                        if (result == 1)
                            ready = true;
                        else
                            count++;
                    }
                    if (ready)
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Se eliminaron facturas nuevas."));
                    else
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFile - MasiveInsert - Error al intentar eliminar facturas nuevas. Posible duplicado de facturas."));

                }
            }

            countN = countlocalP;
            countD = countlocalD;
            valueN = (double)totallocalP / 100;
            valueD = (double)totallocalD / 100;
            return generalErrors;
        }

        private List<String> AnalizeLine(string line, int codCommerce, int codBranch, Guid emailId, Guid serviceId, string[] lines)
        {
            #region metodo
            var generalErrors = new List<String>();
            var count = 0;
            var index = 0;

            if (!String.IsNullOrEmpty(line))
            {
                try
                {
                    index = Array.FindIndex(lines, w => w.Equals(line));
                }
                catch (Exception exception)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Highway bill insert - Se produjo una excepcion la obtencion de index ");
                    NLogLogger.LogHighwayFileProccessEvent(exception);
                }


                try
                {

                    #region Datos por posición

                    /*
                    * int CodComercio	
                    * int CodSucursal	
                    * string RefCliente	
                    * string NroFactura	
                    * string Descripcion	
                    * string FchFactura	
                    * string FchVencimiento	
                    * int DiasPagoVenc	
                    * string Moneda	
                    * int MontoTotal	
                    * int MontoMinimo	
                    * int MontoGravado	
                    * string ConsFinal	
                    * int Cuotas	
                    * El resto son datos auxiliares. La primera pos indica id y la 2da elvalor
                */

                    #endregion

                    if (string.IsNullOrEmpty(line))
                        return generalErrors;

                    var item = line.Split('|');

                    var auxDataInt = item.Count() - 14;
                    auxDataInt = auxDataInt / 2;
                    var sqlBillData = new SqlParameter[29 + auxDataInt];

                    var hBill = new HighwayBill()
                    {
                        Id = Guid.NewGuid(),
                        ServiceId = serviceId,
                        HighwayEmailId = emailId,
                    };

                    sqlBillData[0] = new SqlParameter("@id", hBill.Id);
                    sqlBillData[1] = new SqlParameter("@highwayemailid", hBill.HighwayEmailId);
                    sqlBillData[2] = new SqlParameter("@serviceid", hBill.ServiceId);

                    #region VALIDATION PER LINES

                    //COD COMERCIO
                    FileValidateEmpty(item[0], "CodComercio", ref generalErrors, index);
                    var vpncommerce = FileValidatePositiveNumeric(item[0], "CodComercio", ref generalErrors,
                        index);
                    if (vpncommerce)
                    {
                        hBill.CodComercio = int.Parse(item[0]);
                        if (hBill.CodComercio != codCommerce)
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto. El campo {0} de la línea {1} no concuerda con el código del archivo ({2}).",
                                    "CodComercio", index, codCommerce));
                    }

                    //COD SUCURSAL
                    FileValidateEmpty(item[1], "CodSucursal", ref generalErrors, index);
                    var vpnbranch = FileValidatePositiveNumeric(item[1], "CodSucursal", ref generalErrors,
                        index);
                    if (vpnbranch)
                    {
                        hBill.CodSucursal = int.Parse(item[1]);
                        if (hBill.CodSucursal != codBranch)
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto. El campo {0} de la línea {1} no concuerda con el código del archivo ({2}).",
                                    "CodSucursal", index, codBranch));
                    }

                    //REF CLIENTE (Obligatorio)
                    FileValidateEmpty(item[2], "RefCliente", ref generalErrors, index);
                    const int maxRefCliente = 30;
                    FileValidateMax(item[2], "RefCliente", ref generalErrors, index, maxRefCliente);
                    hBill.RefCliente = item[2];

                    //var infile = bills.Any(i => i.NroFactura.Equals(item[3]));
                    //if (infile)
                    //{
                    //    generalErrors.Add(
                    //        string.Format(
                    //            "Error en archivo adjunto. El campo {0} de la línea {1} ya existe en el archivo (NroFactura: {2}).",
                    //            "NroFactura", index, item[3]));
                    //}
                    //var inrepo = _repositoryPayment.AllNoTracking(null, b => b.Bills).Any(z => z.ServiceId == serviceId && z.Bills.Any(y => y.BillExternalId.Equals(hBill.NroFactura)));
                    //if (inrepo)
                    //{
                    //    generalErrors.Add(string.Format("Error en archivo adjunto. El campo {0} de la línea {1} ya fue marcado como pago realizado (NroFactura: {2}).", "NroFactura", index, item[3]));
                    //}

                    //NRO FACTURA (Obligatorio)
                    FileValidateEmpty(item[3], "NroFactura", ref generalErrors, index);
                    const int maxNroFactura = 50;
                    FileValidateMax(item[3], "NroFactura", ref generalErrors, index, maxNroFactura);
                    hBill.NroFactura = item[3];

                    //DESCRIPCION (No Obligatorio)
                    //FileValidateEmpty(item[4], "Descripción", ref generalErrors, index);
                    const int maxDescripcion = 200;
                    FileValidateMax(item[4], "Descripción", ref generalErrors, index, maxDescripcion);
                    hBill.Descripcion = item[4];

                    //FCH FACTURA (Obligatorio)
                    var resultFchFactura = FileValidateEmpty(item[5], "FchFactura", ref generalErrors, index);
                    if (resultFchFactura)
                    {
                        try
                        {
                            var year = Int16.Parse(item[5].Substring(0, 4));
                            var month = Int16.Parse(item[5].Substring(4, 2));
                            var day = Int16.Parse(item[5].Substring(6, 2));
                            var date = new DateTime(year, month, day);
                            hBill.FchFactura = date;
                        }
                        catch (Exception)
                        {
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto: El campo {0} ({1}) de la línea {2} no respeta el formato esperado ({3}).",
                                    "FchFactura", item[5], index, "AAAAMMDD"));
                        }
                    }

                    //FCH VENCIMIENTO (Obligatorio)
                    var resultFchVencimiento = FileValidateEmpty(item[6], "FchVencimiento", ref generalErrors,
                        index);
                    if (resultFchVencimiento)
                    {
                        try
                        {
                            var year = Int16.Parse(item[6].Substring(0, 4));
                            var month = Int16.Parse(item[6].Substring(4, 2));
                            var day = Int16.Parse(item[6].Substring(6, 2));
                            var date = new DateTime(year, month, day);
                            hBill.FchVencimiento = date;
                        }
                        catch (Exception)
                        {
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto: El campo {0} ({1}) de la línea {2} no respeta el formato esperado ({3}).",
                                    "FchVencimiento", item[6], index, "AAAAMMDD"));
                        }
                    }

                    //DIAS PAGO VENC (No Obligatorio)
                    if (string.IsNullOrEmpty(item[7]))
                        hBill.DiasPagoVenc = 0;
                    else
                    {
                        var resultNumeric = FileValidatePositiveNumeric(item[7], "DiasPagoVenc", ref generalErrors,
                            index);
                        if (resultNumeric)
                        {
                            const int maxDiasPagoVenc = 3;
                            FileValidateMax(item[7], "DiasPagoVenc", ref generalErrors, index, maxDiasPagoVenc);
                            hBill.DiasPagoVenc = int.Parse(item[7]);
                        }
                    }

                    //MONEDA (Obligatorio)
                    var resultCurrency = FileValidateEmpty(item[8], "Moneda", ref generalErrors, index);
                    if (resultCurrency)
                    {
                        var currency = item[8];
                        if (currency.Equals("D") || currency.Equals("N"))
                            hBill.Moneda = currency.Equals("D") ? "USD" : "UYU";
                        else
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto: El campo {0} ({1}) de la línea {2} no tiene una opción válida({3}).",
                                    "Moneda", item[8], index, "N o D"));
                    }

                    //MONTO TOTAL (Obligatorio)
                    var resultMontoTotal = FileValidateEmpty(item[9], "MontoTotal", ref generalErrors, index);
                    long total = 0;
                    if (resultMontoTotal)
                    {
                        var resultNumeric = FileValidatePositiveNumeric(item[9], "MontoTotal", ref generalErrors,
                            index);
                        if (resultNumeric)
                        {
                            const int maxMontoTotal = 20;
                            FileValidateMax(item[9], "MontoTotal", ref generalErrors, index, maxMontoTotal);
                            hBill.MontoTotal = double.Parse(item[9]) / 100;
                            total = long.Parse(item[9]);
                        }
                    }

                    //MONTO MÍNIMO (No Obligatorio)
                    //var resultMontoMínimo = FileValidateEmpty(item[10], "MontoMinimo", ref generalErrors, index);
                    if (string.IsNullOrEmpty(item[10]))
                    {
                        // si no viene un monto mínimo establecido se pone en el total. 
                        hBill.MontoMinimo = hBill.MontoTotal;
                    }
                    else
                    {
                        var resultNumeric = FileValidatePositiveNumeric(item[10], "MontoMinimo", ref generalErrors,
                            index);
                        if (resultNumeric)
                        {
                            const int maxMontoMinimo = 20;
                            FileValidateMax(item[10], "MontoMinimo", ref generalErrors, index, maxMontoMinimo);
                            hBill.MontoMinimo = double.Parse(item[10]) / 100;
                        }
                    }

                    //MONTO GRAVADO (No Obligatorio)
                    //var resultMontoGravado = FileValidateEmpty(item[11], "MontoGravado", ref generalErrors, index);
                    if (string.IsNullOrEmpty(item[11]))
                    {
                        // si no viene un monto gravado establecido se pone en 0. 
                        hBill.MontoGravado = 0;
                    }
                    else
                    {
                        var resultNumeric = FileValidatePositiveNumeric(item[11], "MontoGravado", ref generalErrors,
                            index);
                        if (resultNumeric)
                        {
                            const int maxMontoGravado = 20;
                            FileValidateMax(item[11], "MontoGravado", ref generalErrors, index,
                                maxMontoGravado);
                            hBill.MontoGravado = double.Parse(item[11]) / 100;
                        }
                    }

                    //CONS FINAL (Obligatorio)
                    var resultConsFinal = FileValidateEmpty(item[12], "ConsFinal", ref generalErrors, index);
                    if (resultConsFinal)
                    {
                        var cons = item[12];
                        if (cons.Equals("S") || cons.Equals("N"))
                            hBill.ConsFinal = cons.Equals("S");
                        else
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto: El campo {0} ({1}) de la línea {2} no tiene una opción válida({3}).",
                                    "ConsFinal", item[12], index, "S o N"));
                    }

                    //CUOTAS (Obligatorio)
                    var resultCuotas = FileValidateEmpty(item[13], "Cuotas", ref generalErrors, index);
                    if (resultCuotas)
                    {
                        var resultNumeric = FileValidatePositiveNumeric(item[13], "Cuotas", ref generalErrors,
                            index);
                        if (resultNumeric)
                        {
                            var cons = int.Parse(item[13]);
                            if (cons == 1)
                            {
                                hBill.Cuotas = cons;
                            }
                            else
                            {
                                generalErrors.Add(
                                    string.Format(
                                        "Error en archivo adjunto: El campo {0} ({1}) de la línea {2} no tiene una opción válida({3}).",
                                        "Cuotas", item[13], index, "1"));
                            }
                        }
                    }

                    // TODO - FALTA LUEGO LEER LOS DATOS ADICIONALES - NO PARA FASE 1

                    //CAMPOS AUXILIARES (No Obligatorios)
                    int x = 14;
                    const int maxIdAux = 20;
                    const int maxDatoAux = 200;
                    bool pares = true;
                    hBill.AuxiliarData = new Collection<HighwayAuxiliarData>();
                    while (x <= 52 && pares && x < item.Length && !string.IsNullOrEmpty(item[x]))
                    {
                        if (x + 1 < item.Length && !string.IsNullOrEmpty(item[x + 1]))
                        {
                            var maxIdOk = FileValidateMax(item[x], "IdAuxiliar", ref generalErrors, index,
                                maxIdAux);
                            var maxDatoOk = FileValidateMax(item[x + 1], "DatoAuxiliar", ref generalErrors,
                                index, maxDatoAux);

                            if (maxIdOk && maxDatoOk)
                            {
                                var dataAux = new HighwayAuxiliarData()
                                {
                                    IdValue = item[x],
                                    Value = item[x + 1]
                                };

                                hBill.AuxiliarData.Add(dataAux);
                            }
                            x = x + 2;
                        }
                        else
                        {
                            //error: deben venir de a pares
                            pares = false;
                            generalErrors.Add(
                                string.Format(
                                    "Error en archivo adjunto: Un campo {0} de la línea {1} no tiene su par correspondiente (Id_auxiliar|Dato_auxiliar).",
                                    "Auxiliar", index));
                        }
                    }
                    if (x >= 54 && x < item.Length && !string.IsNullOrEmpty(item[x]))
                    {
                        //error: no pueden ser mas de 20 campos auxiliares
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: Hay más de 20 campos auxiliares en la línea {0}",
                                index));
                    }

                    #endregion

                    if (!generalErrors.Any())
                    {
                        sqlBillData[3] = new SqlParameter("@codcomercio", hBill.CodComercio);
                        sqlBillData[4] = new SqlParameter("@codsucursal", hBill.CodSucursal);
                        sqlBillData[5] = new SqlParameter("@consfinal", hBill.ConsFinal);
                        sqlBillData[6] = new SqlParameter("@cuotas", hBill.Cuotas);
                        if (string.IsNullOrEmpty(hBill.Descripcion))
                            sqlBillData[7] = new SqlParameter("@descripcion", DBNull.Value);
                        else
                            sqlBillData[7] = new SqlParameter("@descripcion", hBill.Descripcion);

                        sqlBillData[8] = new SqlParameter("@diaspagovenc", hBill.DiasPagoVenc);
                        sqlBillData[9] = new SqlParameter("@fchfactura", hBill.FchFactura);
                        sqlBillData[10] = new SqlParameter("@fchvencimiento", hBill.FchVencimiento);
                        sqlBillData[11] = new SqlParameter("@moneda", hBill.Moneda);
                        sqlBillData[12] = new SqlParameter("@montogravado", hBill.MontoGravado);
                        sqlBillData[13] = new SqlParameter("@montominimo", hBill.MontoMinimo);
                        sqlBillData[14] = new SqlParameter("@montototal", hBill.MontoTotal);
                        sqlBillData[15] = new SqlParameter("@nrofactura", hBill.NroFactura);
                        sqlBillData[16] = new SqlParameter("@pagodebito", hBill.PagoDebito);
                        sqlBillData[17] = new SqlParameter("@refcliente", hBill.RefCliente);

                        if (string.IsNullOrEmpty(hBill.RefCliente2))
                            sqlBillData[18] = new SqlParameter("@refcliente2", DBNull.Value);
                        else
                            sqlBillData[18] = new SqlParameter("@refcliente2", hBill.RefCliente2);
                        if (string.IsNullOrEmpty(hBill.RefCliente3))
                            sqlBillData[19] = new SqlParameter("@refcliente3", DBNull.Value);
                        else
                            sqlBillData[19] = new SqlParameter("@refcliente3", hBill.RefCliente3);
                        if (string.IsNullOrEmpty(hBill.RefCliente4))
                            sqlBillData[20] = new SqlParameter("@refcliente4", DBNull.Value);
                        else
                            sqlBillData[20] = new SqlParameter("@refcliente4", hBill.RefCliente4);
                        if (string.IsNullOrEmpty(hBill.RefCliente5))
                            sqlBillData[21] = new SqlParameter("@refcliente5", DBNull.Value);
                        else
                            sqlBillData[21] = new SqlParameter("@refcliente5", hBill.RefCliente5);
                        if (string.IsNullOrEmpty(hBill.RefCliente6))
                            sqlBillData[22] = new SqlParameter("@refcliente6", DBNull.Value);
                        else
                            sqlBillData[22] = new SqlParameter("@refcliente6", hBill.RefCliente6);

                        sqlBillData[23] = new SqlParameter("@type", 1);

                        //_repositoryHighwayBill.Create(hBill);
                        var datedata = DateTime.Now;
                        hBill.CreationDate = datedata;
                        hBill.LastModificationDate = datedata;
                        hBill.CreationUser = TransactionContext.UserName;
                        hBill.LastModificationUser = TransactionContext.UserName;

                        sqlBillData[24] = new SqlParameter("@creationdate", datedata);
                        sqlBillData[25] = new SqlParameter("@lastmodificationdate", datedata);
                        sqlBillData[26] = new SqlParameter("@creationuser", TransactionContext.UserName);
                        sqlBillData[27] = new SqlParameter("@lastmodificationuser", TransactionContext.UserName);
                        sqlBillData[28] = new SqlParameter("@errorcode", int.Parse("0"));

                        if (hBill.Moneda.Equals("UYU"))
                        {
                            Interlocked.Increment(ref countlocalP);
                            Interlocked.Add(ref totallocalP, total);
                            //valueN = valueN + hBill.MontoTotal;
                        }
                        if (hBill.Moneda.Equals("USD"))
                        {
                            Interlocked.Increment(ref countlocalD);
                            Interlocked.Add(ref totallocalD, total);
                            //valueD = valueD + hBill.MontoTotal;
                        }

                        if (_db.Connection.State == ConnectionState.Open)
                        {
                            var sql = "insert into [dbo].[highwaybills] ([id] ,[creationuser],[lastmodificationuser],[creationdate],[lastmodificationdate],[codcomercio],[codsucursal],[refcliente]," +
                                      "[nrofactura],[descripcion],[fchfactura],[fchvencimiento],[diaspagovenc],[moneda],[montototal],[montogravado],[consfinal],[cuotas],[highwayemailid],[serviceid]," +
                                      "[montominimo],[pagodebito],[type],[refcliente2],[refcliente3],[refcliente4],[refcliente5],[refcliente6],[errorcode])" +
                                      "values (@id,@creationuser,@lastmodificationuser,@creationdate,@lastmodificationdate,@codcomercio,@codsucursal,@refcliente,@nrofactura," +
                                      "@descripcion,@fchfactura,@fchvencimiento,@diaspagovenc,@moneda,@montototal,@montogravado,@consfinal,@cuotas," +
                                      "@highwayemailid,@serviceid,@montominimo,@pagodebito,@type,@refcliente2,@refcliente3,@refcliente4,@refcliente5,@refcliente6, @errorcode)";


                            var result = _db.ExecuteSqlCommand(sql, sqlBillData.ToArray<object>());

                            if (result != 1)
                            {
                                generalErrors.Add(string.Format("Error en archivo adjunto. La línea {0} no pudo ser guardada. Sql devolvio {1}", index, result));
                            }
                        }
                        else
                        {
                            generalErrors.Add(string.Format("Error en archivo adjunto. La línea {0} no pudo ser guardada. La conección estaba cerrada. Sql devolvio {1}",index));
                        }
                    }
                }
                catch (Exception exception)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Highway bill insert - Se produjo una excepcion en la linea: " + index);
                    NLogLogger.LogHighwayFileProccessEvent(exception);
                    generalErrors.Add(string.Format("Error en archivo adjunto. La línea {0} no pudo ser guardada.", index));
                }
            }

            #endregion
            return generalErrors;
        }

        #region validation

        private bool FileValidateEmpty(object item, string name, ref List<String> errors, int index)
        {
            if (String.IsNullOrEmpty((string)item))
            {
                if (errors != null)
                    errors.Add(
                        string.Format(
                            "Error en archivo adjunto: La línea {0} está mal formada - El campo {1} es obligatorio.",
                            index, name));
                return false;
            }
            return true;
        }

        private bool FileValidateMax(object item, string name, ref List<String> errors, int index, int largoMaximo)
        {
            if (item.ToString().Length > largoMaximo)
            {
                errors.Add(
                    string.Format(
                        "Error en archivo adjunto. El campo {0} de la línea {1} excede el largo máximo permitido ({2}).",
                        name, index, largoMaximo));
                return false;
            }
            return true;
        }

        private bool FileValidatePositiveNumeric(string item, string name, ref List<String> errors, int index)
        {
            var regex = new Regex(@"^\d+$");
            if (regex.IsMatch(item))
            {
                return true;
            }
            if (errors != null)
                errors.Add(
                    string.Format(
                        "Error en archivo adjunto: La línea {0} está mal formada - El campo {1} debe ser numérico.",
                        index, name));
            return false;
        }

        private string ValidateEmpty(object item, string name)
        {
            return String.IsNullOrEmpty((string)item) ? string.Format("El campo {0} es obligatorio.", name) : null;
        }

        private string ValidateMax(object item, string name, int largoMaximo)
        {
            return item.ToString().Length > largoMaximo
                ? string.Format("El campo {0} excede el largo máximo permitido ({1}).", name, largoMaximo)
                : null;
        }

        private string ValidatePositiveNumeric(double item, string name)
        {
            return item < 0 ? string.Format("El campo {0} debe ser numérico.", name) : null;
        }

        private string ValidatePositiveNumeric(int item, string name)
        {
            return item < 0 ? string.Format("El campo {0} debe ser numérico.", name) : null;
        }

        #endregion
    }
}