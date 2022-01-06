using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationBanred : BaseService<ConciliationBanred, ConciliationBanredDto>, IServiceConciliationBanred
    {
        private readonly IServiceConciliationRun _serviceConciliationRun;

        private string _directoryUnprocessed = ConfigurationManager.AppSettings["BanredUnprocessedFolder"];
        private string _directoryProcessed = ConfigurationManager.AppSettings["BanredProcessedFolder"];
        private string _folderUnprocessedBlob = ConfigurationManager.AppSettings["AzureConciliationBanredUnprocessedFolder"];
        private string _folderProcessedBlob = ConfigurationManager.AppSettings["AzureConciliationBanredProcessedFolder"];

        public ServiceConciliationBanred(IRepositoryConciliationBanred repository, IServiceConciliationRun serviceConciliationRun)
            : base(repository)
        {
            _serviceConciliationRun = serviceConciliationRun;
        }

        public override IQueryable<ConciliationBanred> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationBanredDto Converter(ConciliationBanred entity)
        {
            var dto = new ConciliationBanredDto
            {
                Id = entity.Id,
                Date = entity.Date,
                VisaTransactionId = entity.VisaTransactionId,
                ReferenceNumber = entity.ReferenceNumber,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount
            };

            return dto;
        }

        public override ConciliationBanred Converter(ConciliationBanredDto entity)
        {
            var cBanred = new ConciliationBanred
            {
                Id = entity.Id,
                Date = entity.Date,
                VisaTransactionId = entity.VisaTransactionId,
                ReferenceNumber = entity.ReferenceNumber,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount
            };
            return cBanred;
        }

        public bool DirectoryConciliation()
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - DirectoryConciliation - Comienza procesar directorio.");

            const bool isManualRun = false;
            var conciliationRunUpdated = false;
            var conciliationRun = CreateConciliationRun(isManualRun, "Directorio");

            try
            {
                //Se obtienen todos los archivos del directorio indicado
                var filePaths = Directory.GetFiles(_directoryUnprocessed);

                var oldConciliations = AllNoTracking().Select(c => c.VisaTransactionId).ToList();

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);
                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - DirectoryConciliation - Comienza procesar archivo " + fileName);

                    string line;
                    var file = new StreamReader(filePath);
                    while ((line = file.ReadLine()) != null)
                    {
                        ConciliateLine(line, oldConciliations);
                    }
                    file.Close();

                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - DirectoryConciliation - Termina procesar archivo " + fileName);

                    //Se mueve el archivo al directorio de procesados
                    var pathAux = _directoryProcessed + "\\" + fileName;
                    if (File.Exists(pathAux))
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationBanred - DirectoryConciliation - Archivo {0} previamente procesado. Elimino del path {1}", fileName, filePath));
                        File.Delete(filePath);
                    }
                    else
                    {
                        File.Move(filePath, _directoryProcessed + "\\" + fileName);
                    }
                }

                //Se actualiza el estado de la corrida
                UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.CompletedOk, null);
                conciliationRunUpdated = true;

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - DirectoryConciliation - Termina procesar directorio.");
            }
            catch (Exception exception)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.TerminatedWithException, null, exception);
                }
                NLogLogger.LogEvent(NLogType.Error, "ServiceConciliationBanred - DirectoryConciliation - Se produjo un error al procesar los archivos de banred");
                NLogLogger.LogEvent(exception);
                throw;
            }

            return true;
        }

        public bool SingleFileConciliation(string fileName)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - SingleFileConciliation - Comienza procesar archivo " + fileName);

            const bool isManualRun = true;
            var conciliationRunUpdated = false;
            var conciliationRun = CreateConciliationRun(isManualRun, fileName);

            try
            {
                //Se obtienen las líneas archivo indicado del Blob
                var lines = FileStorage.Instance.GetTextFileLines(_folderUnprocessedBlob, BlobAccessType.Blob, fileName);

                var oldConciliations = AllNoTracking().Select(c => c.VisaTransactionId).ToList();

                foreach (var line in lines)
                {
                    ConciliateLine(line, oldConciliations);
                }

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - SingleFileConciliation - Termina procesar archivo " + fileName);

                //Se actualiza el estado de la corrida
                UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.CompletedOk, null);
                conciliationRunUpdated = true;

                //Se mueve el archivo al directorio de procesados
                if (FileStorage.Instance.CheckIfFileExists(_folderProcessedBlob, BlobAccessType.Blob, fileName))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationBanred - SingleFileConciliation - Archivo {0} previamente procesado. Elimino del Blob", fileName));
                    FileStorage.Instance.DeleteFile(_folderUnprocessedBlob, fileName);
                }
                else
                {
                    FileStorage.Instance.CopyFileAndDeleteFromSource(_folderUnprocessedBlob, _folderProcessedBlob, BlobAccessType.Blob, fileName);
                }
            }
            catch (Exception exception)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.TerminatedWithException, null, exception);
                }
                NLogLogger.LogEvent(NLogType.Error, "ServiceConciliationBanred - SingleFileConciliation - Se produjo un error al procesar el archivo: " + fileName);
                NLogLogger.LogEvent(exception);
                throw;
            }

            return true;
        }

        private void ConciliateLine(string line, IList<long> oldConciliations)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - ConciliateLine - Lineas a procesar: " + line);
            var parameters = line.Split(',');

            if (oldConciliations.All(c => c != Int64.Parse(parameters[7])))
            {
                var conciliationBanred = new ConciliationBanredDto
                {
                    Id = Guid.NewGuid(),
                    Date =
                        new DateTime(Int32.Parse(parameters[0].Substring(0, 4)),
                            Int32.Parse(parameters[0].Substring(4, 2)),
                            Int32.Parse(parameters[0].Substring(6, 2)),
                            Int32.Parse(parameters[0].Substring(8, 2)),
                            Int32.Parse(parameters[0].Substring(10, 2)),
                            Int32.Parse(parameters[0].Substring(12, 2))),
                    VisaTransactionId = Int64.Parse(parameters[7]),
                    ReferenceNumber = parameters[2],
                    BillExternalId = parameters[3],
                    Currency = parameters[5] == "D" ? Currency.DOLAR_AMERICANO : Currency.PESO_URUGUAYO,
                    Amount = Double.Parse(parameters[4]) / 100
                };
                Create(conciliationBanred);
            }
            else
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("ServiceConciliationBanred - ConciliateLine - Dato repetido. La transferencia {0} ya se encuentra en la BD Conciliacion Banred", Int64.Parse(parameters[7])));
            }
        }

        private ConciliationRunDto CreateConciliationRun(bool isManualRun, string filename)
        {
            try
            {
                var dto = new ConciliationRunDto
                {
                    App = ConciliationAppDto.Banred,
                    IsManualRun = isManualRun,
                    InputFileName = filename,
                    State = ConciliationRunStateDto.Started
                };

                var newDto = _serviceConciliationRun.Create(dto, true);
                return newDto;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - CreateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private void UpdateConciliationRun(ConciliationRunDto dto, ConciliationRunStateDto state, string resultDescription, Exception resultException = null)
        {
            if (dto == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - UpdateConciliationRun - Dto null");
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
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationBanred - UpdateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
            }
        }

    }
}