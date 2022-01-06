using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationVisanetCallback : BaseService<ConciliationVisanetCallback, ConciliationVisanetCallbackDto>, IServiceConciliationVisanetCallback
    {
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IRepositoryConciliationVisanetCallback _repositoryConciliationVisanetCallback;
        private readonly IServiceConciliationRun _serviceConciliationRun;

        private string _directoryUnprocessed = ConfigurationManager.AppSettings["VisanetCallbackUnprocessedFolder"];
        private string _directoryProcessed = ConfigurationManager.AppSettings["VisanetCallbackProcessedFolder"];
        private string _folderUnprocessedBlob = ConfigurationManager.AppSettings["AzureConciliationVisanetCallbackUnprocessedFolder"];
        private string _folderProcessedBlob = ConfigurationManager.AppSettings["AzureConciliationVisanetCallbackProcessedFolder"];

        public ServiceConciliationVisanetCallback(IRepositoryConciliationVisanetCallback repository, IServiceEmailMessage serviceEmailMessage,
            IServiceFixedNotification serviceFixedNotification, IRepositoryConciliationVisanetCallback repositoryConciliationVisanetCallback,
            IServiceConciliationRun serviceConciliationRun)
            : base(repository)
        {
            _serviceEmailMessage = serviceEmailMessage;
            _serviceFixedNotification = serviceFixedNotification;
            _repositoryConciliationVisanetCallback = repositoryConciliationVisanetCallback;
            _serviceConciliationRun = serviceConciliationRun;
        }

        public override IQueryable<ConciliationVisanetCallback> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationVisanetCallbackDto Converter(ConciliationVisanetCallback entity)
        {
            var dto = new ConciliationVisanetCallbackDto
            {
                Id = entity.Id,
                RegisterType = entity.RegisterType,
                FileId = entity.FileId,
                RegisterId = entity.RegisterId,
                IgnoreCode = entity.IgnoreCode,
                Merchant = entity.Merchant,
                Branch = entity.Branch,
                Currency = entity.Currency,
                SalePlan = entity.SalePlan,
                Refund = entity.Refund,
                CardMaskedNumber = entity.CardMaskedNumber,
                Amount = entity.Amount,
                AuthorizationCode = entity.AuthorizationCode,
                CyberSourceId = entity.CyberSourceId,
                State = (VisanetCallbackStateDto)((int)entity.State),
                ErrorCode = entity.ErrorCode,
                ErrorDescription = entity.ErrorDescription,
                RegisterDate = entity.RegisterDate,
                TimeStamp = entity.TimeStamp
            };

            return dto;
        }

        public override ConciliationVisanetCallback Converter(ConciliationVisanetCallbackDto entity)
        {
            var cVisanetCallback = new ConciliationVisanetCallback
            {
                Id = entity.Id,
                RegisterType = entity.RegisterType,
                FileId = entity.FileId,
                RegisterId = entity.RegisterId,
                IgnoreCode = entity.IgnoreCode,
                Merchant = entity.Merchant,
                Branch = entity.Branch,
                Currency = entity.Currency,
                SalePlan = entity.SalePlan,
                Refund = entity.Refund,
                CardMaskedNumber = entity.CardMaskedNumber,
                Amount = entity.Amount,
                AuthorizationCode = entity.AuthorizationCode,
                CyberSourceId = entity.CyberSourceId,
                State = (VisanetCallbackState)((int)entity.State),
                ErrorCode = entity.ErrorCode,
                ErrorDescription = entity.ErrorDescription,
                RegisterDate = entity.RegisterDate,
                TimeStamp = entity.TimeStamp
            };
            return cVisanetCallback;
        }

        public override void Edit(ConciliationVisanetCallbackDto entity)
        {
            Repository.ContextTrackChanges = true;

            var entityDb = Repository.All().FirstOrDefault(e => e.Id.Equals(entity.Id));
            if (entityDb != null)
            {
                entityDb.RegisterType = entity.RegisterType;
                entityDb.FileId = entity.FileId;
                entityDb.RegisterId = entity.RegisterId;
                entityDb.IgnoreCode = entity.IgnoreCode;
                entityDb.Merchant = entity.Merchant;
                entityDb.Branch = entity.Branch;
                entityDb.Currency = entity.Currency;
                entityDb.SalePlan = entity.SalePlan;
                entityDb.Refund = entity.Refund;
                entityDb.CardMaskedNumber = entity.CardMaskedNumber;
                entityDb.Amount = entity.Amount;
                entityDb.AuthorizationCode = entity.AuthorizationCode;
                entityDb.CyberSourceId = entity.CyberSourceId;
                entityDb.State = (VisanetCallbackState)((int)entity.State);
                entityDb.ErrorCode = entity.ErrorCode;
                entityDb.ErrorDescription = entity.ErrorDescription;
                entityDb.RegisterDate = entity.RegisterDate;
                entityDb.TimeStamp = entity.TimeStamp;

                Repository.Edit(entityDb);
                Repository.Save();
            }

            Repository.ContextTrackChanges = false;
        }

        public void DirectoryConciliation()
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - DirectoryConciliation - Comienza procesar directorio.");

            const bool isManualRun = false;
            var conciliationRunState = ConciliationRunStateDto.CompletedOk;
            var conciliationRunUpdated = false;
            string globalResultMessage = null;

            var conciliationRun = CreateConciliationRun(isManualRun, "Directorio");

            try
            {
                var resultsDictionary = new Dictionary<string, string>(); // <nombre_archivo, lista_de_lineas_con_error>

                //Cargo todos los archivos txt de los archivos no preocesados
                var txtFiles = Directory.EnumerateFiles(_directoryUnprocessed, "*.txt").OrderBy(x => x).ToList();

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - DirectoryConciliation - Cantidad de archivos a procesar: " + txtFiles.Count);

                foreach (var file in txtFiles)
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - DirectoryConciliation - Comienza procesar archivo " + file);

                    var lineCounter = 1;
                    var linesProcessed = 0;
                    var totalLines = File.ReadLines(file).Count();
                    var lineErrors = new List<int>();
                    string fileId = null;
                    var fileDateTime = DateTime.MinValue;

                    foreach (var line in File.ReadAllLines(file))
                    {
                        ConciliateLine(file, line, lineCounter, totalLines, ref linesProcessed, ref lineErrors, ref fileId, ref fileDateTime);
                        lineCounter++;
                    }

                    string resultMessage;
                    if (!lineErrors.Any())
                    {
                        resultMessage = linesProcessed + " líneas procesadas correctamente.";

                        globalResultMessage += resultMessage;

                        //Muevo el archivo al directorio de procesados y le modifico el nombre con la fecha actual
                        File.Move(file, Path.Combine(_directoryProcessed, string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), file.Split('\\').Last())));
                        File.Delete(file);
                    }
                    else
                    {
                        conciliationRunState = ConciliationRunStateDto.CompletedWithErrors;

                        resultMessage = linesProcessed + " líneas procesadas correctamente. Las siguientes líneas contienen contienen errores: ";
                        foreach (var line in lineErrors)
                        {
                            resultMessage += line + ", ";
                        }
                        resultMessage = resultMessage.Substring(0, resultMessage.Length - 2);

                        globalResultMessage += resultMessage;

                        _serviceFixedNotification.Create(new FixedNotificationDto
                        {
                            Category = FixedNotificationCategoryDto.BatchProcess,
                            Description = "Ocurrió un error al procesar un archivo " + file + " de conciliación",
                            Level = FixedNotificationLevelDto.Error,
                            DateTime = DateTime.Now
                        });
                    }

                    //Agrego el resultado del archivo al diccionario
                    resultsDictionary.Add(file.Split('\\').Last(), resultMessage);

                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - DirectoryConciliation - Termina procesar archivo " + file);
                }

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - DirectoryConciliation - Termina procesar directorio.");

                //Se actualiza el estado de la corrida
                UpdateConciliationRun(conciliationRun, conciliationRunState, globalResultMessage);
                conciliationRunUpdated = true;

                //Se envía un mail con el resultado del procesamiento de los archivos
                _serviceEmailMessage.SendConciliationProcessResult(resultsDictionary);
            }
            catch (Exception exception)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    conciliationRunState = ConciliationRunStateDto.TerminatedWithException;
                    UpdateConciliationRun(conciliationRun, conciliationRunState, globalResultMessage, exception);
                }
                NLogLogger.LogEvent(NLogType.Error, "ServiceConciliationVisanetCallback - DirectoryConciliation - Exception");
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        public void SingleFileConciliation(string fileName)
        {
            const bool isManualRun = true;
            var conciliationRunState = ConciliationRunStateDto.CompletedOk;
            var conciliationRunUpdated = false;
            string resultMessage = null;

            var conciliationRun = CreateConciliationRun(isManualRun, fileName);

            try
            {
                var resultsDictionary = new Dictionary<string, string>(); // <nombre_archivo, lista_de_lineas_con_error>

                //Se obtienen las líneas archivo indicado del Blob
                var lines = FileStorage.Instance.GetTextFileLines(_folderUnprocessedBlob, BlobAccessType.Blob, fileName);

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - SingleFileConciliation - Comienza procesar archivo " + fileName);

                var lineCounter = 1;
                var linesProcessed = 0;
                var totalLines = lines.Count();
                var lineErrors = new List<int>();
                string fileId = null;
                var fileDateTime = DateTime.MinValue;

                foreach (var line in lines)
                {
                    ConciliateLine(fileName, line, lineCounter, totalLines, ref linesProcessed, ref lineErrors, ref fileId, ref fileDateTime);
                    lineCounter++;
                }

                if (!lineErrors.Any())
                {
                    resultMessage = linesProcessed + " líneas procesadas correctamente.";
                    //Muevo el archivo al directorio de procesados y le modifico el nombre con la fecha actual
                    var newFileName = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                    FileStorage.Instance.CopyFileAndDeleteFromSource(_folderUnprocessedBlob, _folderProcessedBlob, BlobAccessType.Blob, fileName, newFileName);
                }
                else
                {
                    conciliationRunState = ConciliationRunStateDto.CompletedWithErrors;
                    resultMessage = linesProcessed + " líneas procesadas correctamente. Las siguientes líneas contienen contienen errores: ";
                    foreach (var line in lineErrors)
                    {
                        resultMessage += line + ", ";
                    }
                    resultMessage = resultMessage.Substring(0, resultMessage.Length - 2);

                    _serviceFixedNotification.Create(new FixedNotificationDto
                    {
                        Category = FixedNotificationCategoryDto.BatchProcess,
                        Description = "Ocurrió un error al procesar un archivo " + fileName + " de conciliación",
                        Level = FixedNotificationLevelDto.Error,
                        DateTime = DateTime.Now
                    });
                }

                //Agrego el resultado del archivo al diccionario
                resultsDictionary.Add(fileName, resultMessage);

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - SingleFileConciliation - Termina procesar archivo " + fileName);

                //Se actualiza el estado de la corrida
                UpdateConciliationRun(conciliationRun, conciliationRunState, resultMessage);
                conciliationRunUpdated = true;

                //Se envía un mail con el resultado del procesamiento de los archivos
                _serviceEmailMessage.SendConciliationProcessResult(resultsDictionary);
            }
            catch (Exception exception)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    conciliationRunState = ConciliationRunStateDto.TerminatedWithException;
                    UpdateConciliationRun(conciliationRun, conciliationRunState, resultMessage, exception);
                }
                NLogLogger.LogEvent(NLogType.Error, "ServiceConciliationVisanetCallback - SingleFileConciliation - Exception");
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        private void ConciliateLine(string file, string line, int lineCounter, int totalLines, ref int linesProcessed,
            ref List<int> lineErrors, ref string fileId, ref DateTime fileDateTime)
        {
            try
            {
                if (lineCounter == 1)
                {
                    //Primera línea
                    var fistLineIsValid = FirstLineIsValid(line, ref fileId, ref fileDateTime);
                    if (!fistLineIsValid)
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Linea con formato inválido # {0}", lineCounter));
                        lineErrors.Add(lineCounter);
                    }
                }
                else if (lineCounter == totalLines)
                {
                    //Última línea
                    var lastLineIsValid = LastLineIsValid(line, fileId);
                    if (!lastLineIsValid)
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Linea con formato inválido # {0}", lineCounter));
                        lineErrors.Add(lineCounter);
                    }
                }
                else
                {
                    //Líneas intermedias (registros a conciliar)
                    var register = SplitLineIntoRegister(line);
                    if (register != null)
                    {
                        if (register.FileId == fileId)
                        {
                            //Controlo que no exista el registro
                            var log = _repositoryConciliationVisanetCallback.AllNoTracking().FirstOrDefault(x =>
                                x.CyberSourceId == register.CyberSourceId);
                            if (log == null)
                            {
                                _repositoryConciliationVisanetCallback.Create(register);
                                _repositoryConciliationVisanetCallback.Save();
                            }
                            else
                            {
                                //TODO: por ahora solo se van a insertar registros
                                //En el futuro se podría agregar la logica para que chequee si se está haciendo una actualización del registro y en ese caso
                                //se debería actualizar el registro de la Callback a ROJO y borrar la referencia en la SUMMARY al registro para que cuando corra 
                                //el proceso de nuevo la considere y haga de nuevo la conciliación.
                            }
                            linesProcessed++;
                        }
                        else
                        {
                            //Si la línea tiene diferente fileId
                            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Linea con no coincide con Id del archivo # {0}", lineCounter));
                            lineErrors.Add(lineCounter);
                        }
                    }
                    else
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Linea con formato inválido # {0}", lineCounter));
                        lineErrors.Add(lineCounter);
                    }
                }
            }
            catch (FormatException)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Linea con formato inválido # {0}", lineCounter));
                lineErrors.Add(lineCounter);
            }
            catch (Exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceConciliationVisanetCallback - ConciliateLine - Archivo " + file + " - Error no recuperable al guardar linea {0}", lineCounter));
                lineErrors.Add(lineCounter);
            }
        }

        private bool FirstLineIsValid(string line, ref string fileId, ref DateTime fileDateTime)
        {
            //VER: averiguar que es cada parte de la linea y que hay que validar
            var isValid = false;
            try
            {
                if (line.Length >= 51)
                {
                    fileId = line.Substring(1, 8);
                    fileDateTime = DateTime.ParseExact(line.Substring(37, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    isValid = (line.Substring(0, 1).ToUpper() == "H");
                }
                return isValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LastLineIsValid(string line, string fileId)
        {
            //VER: averiguar que es cada parte de la linea y que hay que validar
            var isValid = false;
            try
            {
                if (line.Length >= 40)
                {
                    var lineFileId = line.Substring(1, 8);
                    isValid = (line.Substring(0, 1).ToUpper() == "T" && lineFileId == fileId);
                }
                return isValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private ConciliationVisanetCallback SplitLineIntoRegister(string line)
        {
            ConciliationVisanetCallback register = null;
            try
            {
                if (line.Length >= 172)
                {
                    register = new ConciliationVisanetCallback
                    {
                        RegisterType = line.Substring(0, 1).Trim(),
                        FileId = line.Substring(1, 8).Trim(),
                        RegisterId = line.Substring(9, 8).Trim(),
                        IgnoreCode = line.Substring(17, 8).Trim(),
                        Merchant = line.Substring(33, 9).Trim(),
                        Branch = line.Substring(42, 9).Trim(),
                        Currency = line.Substring(51, 4) == "0858" ? "UYU" : "USD",
                        SalePlan = line.Substring(55, 9).Trim(),
                        Refund = line.Substring(64, 1).Trim(),
                        CardMaskedNumber = line.Substring(65, 19).Trim(),
                        Amount = float.Parse(line.Substring(84, 14)) / 100,
                        AuthorizationCode = line.Substring(98, 6).Trim(),
                        CyberSourceId = line.Substring(104, 22).Trim(), //En el Excel dice 25 pero es 22
                        State = (VisanetCallbackState)int.Parse(line.Substring(129, 1)),
                        ErrorCode = line.Substring(130, 4).Trim(),
                        ErrorDescription = line.Substring(134, 30).Trim(),
                        RegisterDate = DateTime.ParseExact(line.Substring(164, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                        TimeStamp = DateTime.Now
                    };
                }
                return register;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ConciliationRunDto CreateConciliationRun(bool isManualRun, string filename)
        {
            try
            {
                var dto = new ConciliationRunDto
                {
                    App = ConciliationAppDto.Batch,
                    IsManualRun = isManualRun,
                    InputFileName = filename,
                    State = ConciliationRunStateDto.Started
                };

                var newDto = _serviceConciliationRun.Create(dto, true);
                return newDto;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - CreateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private void UpdateConciliationRun(ConciliationRunDto dto, ConciliationRunStateDto state, string resultDescription, Exception resultException = null)
        {
            if (dto == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - UpdateConciliationRun - Dto null");
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
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationVisanetCallback - UpdateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
            }
        }

    }
}