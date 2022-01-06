using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationSistarbanc : BaseService<ConciliationSistarbanc, ConciliationSistarbancDto>, IServiceConciliationSistarbanc
    {
        private readonly IServicePayment _servicePayment;
        private readonly IServiceConciliationRun _serviceConciliationRun;

        private string _directoryUnprocessed = ConfigurationManager.AppSettings["SistarbancUnprocessedFolder"];
        private string _directoryProcessed = ConfigurationManager.AppSettings["SistarbancProcessedFolder"];
        private string _folderUnprocessedBlob = ConfigurationManager.AppSettings["AzureConciliationSistarbancUnprocessedFolder"];
        private string _folderProcessedBlob = ConfigurationManager.AppSettings["AzureConciliationSistarbancProcessedFolder"];

        public ServiceConciliationSistarbanc(IRepositoryConciliationSistarbanc repository, IServicePayment servicePayment,
            IServiceConciliationRun serviceConciliationRun)
            : base(repository)
        {
            _servicePayment = servicePayment;
            _serviceConciliationRun = serviceConciliationRun;
        }

        public override IQueryable<ConciliationSistarbanc> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationSistarbancDto Converter(ConciliationSistarbanc entity)
        {
            var dto = new ConciliationSistarbancDto
            {
                Id = entity.Id,
                Date = entity.Date,
                IdTransaccionSTB = entity.IdTransaccionSTB,
                VisaTransactionId = entity.VisaTransactionId,
                SistarbancUserId = entity.SistarbancUserId,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount
            };

            return dto;
        }

        public override ConciliationSistarbanc Converter(ConciliationSistarbancDto entity)
        {
            var cBanred = new ConciliationSistarbanc
            {
                Id = entity.Id,
                Date = entity.Date,
                IdTransaccionSTB = entity.IdTransaccionSTB,
                VisaTransactionId = entity.VisaTransactionId,
                SistarbancUserId = entity.SistarbancUserId,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount
            };
            return cBanred;
        }

        public bool DirectoryConciliation()
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - Comienza procesar directorio.");

            const bool isManualRun = false;
            var conciliationRunState = ConciliationRunStateDto.CompletedOk;
            string globalResultMessage = null;
            var conciliationRun = CreateConciliationRun(isManualRun, "Directorio");

            var filePaths = Directory.GetFiles(_directoryUnprocessed);
            var old = AllNoTracking().Select(c => c.IdTransaccionSTB).ToList();

            var totalFiles = filePaths != null && filePaths.Any() ? filePaths.Count() : 0;
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - Cantidad de archivos a procesar: " + totalFiles);

            var filename = "";
            var line = 0;
            var col = 0;

            foreach (var filePath in filePaths)
            {
                line = 0;
                try
                {
                    var package = new ExcelPackage(new FileInfo(filePath));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    filename = package.File.Name;

                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - Comienza procesar archivo " + filename);
                    ConciliateWorksheet(workSheet, old, ref line, ref col);

                    var aux = _directoryProcessed + '\\' + package.File.Name;
                    package.File.CopyTo(aux, false);
                    package.File.Delete();

                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - Termina procesar archivo " + filename);
                }
                catch (Exception exception)
                {
                    conciliationRunState = ConciliationRunStateDto.CompletedWithErrors;
                    var errorMsg = string.Format("Error en analisis de archivo: {0}, linea: {1}, columna: {2}. ", filename, line, col);
                    globalResultMessage += errorMsg;
                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - " + errorMsg);
                    NLogLogger.LogEvent(exception);
                }
            }

            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - DirectoryConciliation - Termina procesar directorio.");

            //Se actualiza el estado de la corrida
            UpdateConciliationRun(conciliationRun, conciliationRunState, globalResultMessage);

            return true;
        }

        public void SingleFileConciliation(string fileName)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - SingleFileConciliation - Comienza procesar archivo " + fileName);

            const bool isManualRun = false;
            var conciliationRunUpdated = false;
            string globalResultMessage = null;
            var conciliationRun = CreateConciliationRun(isManualRun, fileName);

            try
            {
                var dataset = FileStorage.Instance.GetExcelFileData(_folderUnprocessedBlob, BlobAccessType.Blob, fileName);
                var dataTable = ExtractDataSetToDataTable(dataset);
                var workSheet = ExtractDataTableToExcelWorksheet(dataTable);

                var old = AllNoTracking().Select(c => c.IdTransaccionSTB).ToList();

                var line = 0;
                var col = 0;

                try
                {
                    ConciliateWorksheet(workSheet, old, ref line, ref col);

                    //Se mueve el archivo al directorio de procesados
                    if (FileStorage.Instance.CheckIfFileExists(_folderProcessedBlob, BlobAccessType.Blob, fileName))
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationSistarbanc - SingleFileConciliation - Archivo {0} previamente procesado. Elimino del Blob", fileName));
                        FileStorage.Instance.DeleteFile(_folderUnprocessedBlob, fileName);
                    }
                    else
                    {
                        FileStorage.Instance.CopyFileAndDeleteFromSource(_folderUnprocessedBlob, _folderProcessedBlob, BlobAccessType.Blob, fileName);
                    }
                }
                catch (Exception exception)
                {
                    var errorMsg = string.Format("Error en analisis de archivo: {0}, linea: {1}, columna: {2}. ", fileName, line, col);
                    globalResultMessage += errorMsg;
                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - SingleFileConciliation - " + errorMsg);
                    NLogLogger.LogEvent(exception);

                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.TerminatedWithException, errorMsg, exception);
                    conciliationRunUpdated = true;
                }

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - SingleFileConciliation - Termina procesar archivo " + fileName);

                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.CompletedOk, null);
                }
            }
            catch (Exception e)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.TerminatedWithException, globalResultMessage, e);
                }
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - SingleFileConciliation - Error inesperado. Archivo: " + fileName);
                NLogLogger.LogEvent(e);
            }
        }

        private void ConciliateWorksheet(ExcelWorksheet workSheet, IList<string> old, ref int line, ref int col)
        {
            for (int j = workSheet.Dimension.Start.Row; j <= workSheet.Dimension.End.Row; j++)
            {
                col = 0;
                if (j > 1)
                {
                    line = j;
                    col = 6;
                    var val = workSheet.Cells[j, 6].Value;
                    if (val.Equals("03"))
                    {
                        col = 2;
                        var stbTransaccion = (string)workSheet.Cells[j, 2].Value; //double

                        //si no existe el payment, no existe en visanet
                        var payment = _servicePayment.AllNoTracking(null, c => c.Bills.Any(bill => bill.GatewayTransactionId.Equals(stbTransaccion)), c => c.PaymentIdentifier, c => c.Bills).FirstOrDefault();
                        var has = old.Any(c => c.Equals(stbTransaccion));
                        if (!has)
                        {
                            col = 1;
                            var fechaString = (string)workSheet.Cells[j, 1].Value; //fecha
                            var fAux = fechaString.Split('/');
                            var f2Aux = fAux[2].Split(' ');
                            var hrAux = f2Aux[1].Split(':');

                            var fecha = new DateTime(int.Parse(f2Aux[0]), int.Parse(fAux[1]), int.Parse(fAux[0]), int.Parse(hrAux[0]), int.Parse(hrAux[1]), int.Parse(hrAux[2]));
                            col = 8;
                            var userId = (string)workSheet.Cells[j, 8].Value;//double
                            col = 9;
                            var currency = ((string)workSheet.Cells[j, 9].Value).Equals("$") ? "UYU" : "USD";
                            col = 10;
                            var amount = double.Parse(workSheet.Cells[j, 10].Value.ToString(), new CultureInfo("es-UY"));
                            col = 11;
                            var amountTaxed = double.Parse(workSheet.Cells[j, 11].Value.ToString(), new CultureInfo("es-UY"));

                            Create(new ConciliationSistarbancDto()
                            {
                                Amount = amount,
                                AmountTaxed = amountTaxed,
                                Currency = currency,
                                Date = fecha,
                                IdTransaccionSTB = stbTransaccion,
                                SistarbancUserId = string.IsNullOrEmpty(userId) ? 0 : Int64.Parse(userId),
                                VisaTransactionId = payment != null ? payment.PaymentIdentifierDto.UniqueIdentifier : -1,
                                BillExternalId = payment != null ? payment.Bills.First().BillExternalId : ""
                            });
                        }
                    }
                }
            }
        }

        private static DataTable ExtractDataSetToDataTable(DataSet excelData)
        {
            var dt = new DataTable();
            dt.Columns.Add("Fecha", typeof(string));
            dt.Columns.Add("Nro Trx", typeof(string));
            dt.Columns.Add("Entidad", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Nro Servicio", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("Banco", typeof(string));
            dt.Columns.Add("Nro Cliente SB", typeof(string));
            dt.Columns.Add("Moneda", typeof(string));
            dt.Columns.Add("Importe", typeof(string));
            dt.Columns.Add("Importe Neto", typeof(string));

            excelData
               .Tables[0]
               .AsEnumerable()
               .Skip(1)
               .ToList()
               .ForEach(dr => dt.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8], dr[9], dr[10]));
            return dt;
        }

        private static ExcelWorksheet ExtractDataTableToExcelWorksheet(DataTable dataTable)
        {
            ExcelWorksheet ws = null;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ws = pck.Workbook.Worksheets.Add("Resultado Consulta");
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                pck.Save();
            }
            return ws;
        }

        private ConciliationRunDto CreateConciliationRun(bool isManualRun, string filename)
        {
            try
            {
                var dto = new ConciliationRunDto
                {
                    App = ConciliationAppDto.Sistarbanc,
                    IsManualRun = isManualRun,
                    InputFileName = filename,
                    State = ConciliationRunStateDto.Started
                };

                var newDto = _serviceConciliationRun.Create(dto, true);
                return newDto;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - CreateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private void UpdateConciliationRun(ConciliationRunDto dto, ConciliationRunStateDto state, string resultDescription, Exception resultException = null)
        {
            if (dto == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - UpdateConciliationRun - Dto null");
                return;
            }

            try
            {
                dto.State = state;
                dto.ResultDescription = resultDescription;
                dto.ExceptionMessage = resultException != null ? resultException.InnerException.Message : null;

                _serviceConciliationRun.UpdateConciliationRunResult(dto);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSistarbanc - UpdateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
            }
        }

    }
}