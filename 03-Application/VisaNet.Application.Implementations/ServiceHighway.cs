using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Helpers;
using VisaNet.Utilities.SFTPClient;

namespace VisaNet.Application.Implementations
{
    public class ServiceHighway : BaseService<HighwayBill, HighwayBillDto>, IServiceHighway
    {
        private readonly IRepositoryHighwayEmail _repositoryHighwayEmail;
        private readonly IRepositoryHighwayBill _repositoryHighwayBill;
        private readonly IRepositoryService _repositoryService;
        private readonly ILoggerService _loggerService;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IRepositoryParameters _parameterRepository;

        private string _folderUnprocessedBlob = ConfigurationManager.AppSettings["AzureHighwayUnprocessedFolder"];

        private int count = 0;
        private int countN = 0;
        private int countD = 0;
        private double valueN = 0;
        private double valueD = 0;

        public ServiceHighway(IRepositoryHighwayBill repository, IRepositoryHighwayEmail repositoryHighwayEmail,
            IRepositoryHighwayBill repositoryHighwayBill, IRepositoryService repositoryService, ILoggerService loggerService,
            IRepositoryParameters parameterRepository, IServiceEmailMessage serviceNotificationMessage)
            : base(repository)
        {
            _repositoryHighwayEmail = repositoryHighwayEmail;
            _repositoryHighwayBill = repositoryHighwayBill;
            _repositoryService = repositoryService;
            _loggerService = loggerService;
            _parameterRepository = parameterRepository;
            _serviceNotificationMessage = serviceNotificationMessage;
        }

        public override IQueryable<HighwayBill> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override HighwayBillDto Converter(HighwayBill entity)
        {
            var highwayBillDto = new HighwayBillDto()
            {
                Id = entity.Id,
                CodSucursal = entity.CodSucursal,
                CodComercio = entity.CodComercio,
                ConsFinal = entity.ConsFinal,
                CreationDate = entity.CreationDate,
                Cuotas = entity.Cuotas,
                DiasPagoVenc = entity.DiasPagoVenc,
                Moneda = entity.Moneda,
                Descripcion = entity.Descripcion,
                RefCliente = entity.RefCliente,
                MontoGravado = entity.MontoGravado,
                MontoMinimo = entity.MontoMinimo,
                MontoTotal = entity.MontoTotal,
                NroFactura = entity.NroFactura,
                FchFactura = entity.FchFactura,
                FchVencimiento = entity.FchVencimiento,
                HighwayEmailId = entity.HighwayEmailId,
                ServiceId = entity.ServiceId,
                PagoDebito = entity.PagoDebito,
                Type = (HighwayBillTypeDto)entity.Type,
                ErrorCode = entity.ErrorCode,
                ErrorDesc = entity.ErrorDesc,
                ServiceDto = new ServiceDto()
                {
                    Name = entity.Service != null ? entity.Service.Name : ""
                }
            };
            if (entity.AuxiliarData != null)
            {
                highwayBillDto.AuxiliarData = entity.AuxiliarData.Select(x => new HighwayAuxiliarDataDto()
                {
                    Id_auxiliar = x.IdValue,
                    Dato_auxiliar = x.Value
                }).ToList();
            }
            return highwayBillDto;
        }

        public override HighwayBill Converter(HighwayBillDto entity)
        {
            var highwayBill = new HighwayBill()
            {
                Id = entity.Id,
                CodSucursal = entity.CodSucursal,
                CodComercio = entity.CodComercio,
                ConsFinal = entity.ConsFinal,
                CreationDate = entity.CreationDate,
                Cuotas = entity.Cuotas,
                DiasPagoVenc = entity.DiasPagoVenc,
                Moneda = entity.Moneda,
                Descripcion = entity.Descripcion,
                RefCliente = entity.RefCliente,
                MontoGravado = entity.MontoGravado,
                MontoMinimo = entity.MontoMinimo,
                MontoTotal = entity.MontoTotal,
                NroFactura = entity.NroFactura,
                FchFactura = entity.FchFactura,
                FchVencimiento = entity.FchVencimiento,
                HighwayEmailId = entity.HighwayEmailId,
                ServiceId = entity.ServiceId,
                PagoDebito = entity.PagoDebito,
                Type = (HighwayBillType)entity.Type,
                ErrorCode = entity.ErrorCode,
                ErrorDesc = entity.ErrorDesc,
                AuxiliarData = entity.AuxiliarData != null ? entity.AuxiliarData.Select(x => new HighwayAuxiliarData()
                {
                    IdValue = x.Id_auxiliar,
                    Value = x.Dato_auxiliar,
                    Id = Guid.NewGuid()
                }).ToList() :
                                                               new List<HighwayAuxiliarData>()
            };

            return highwayBill;
        }

        public HighwayEmailDto Converter(HighwayEmail entity)
        {
            var highwayBill = new HighwayEmailDto()
            {
                Id = entity.Id,
                CreationDate = entity.CreationDate,
                ServiceId = entity.ServiceId,

                Errors = entity.Errors != null ? entity.Errors.Select(x => new HighwayEmailErrorDto()
                {
                    Data = x.Data,
                    Id = x.Id,
                }).ToList() : new List<HighwayEmailErrorDto>(),
            };
            return highwayBill;
        }

        #region Process Email
        public void ProccessEmail(HighwayEmailDto dto)
        {
            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail");

            HighwayEmail email = null;
            Int32 codCommerce = -1;
            Int32 codBranch = -1;
            Service service = null;
            var serviceName = string.Empty;
            try
            {
                var sendResponse = true;
                var generalErrors = new List<String>();
                _repositoryHighwayEmail.ContextTrackChanges = true;
                var id = Guid.NewGuid();
                var nroTrans = _repositoryHighwayEmail.GetNextTransactionNumber();

                var outcomeName = "";
                if (dto == null) NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - NULL DTO");

                var fileName = dto.AttachmentInputName.Split('_');
                var resultcodCommmerce = Int32.TryParse(fileName[2], out codCommerce);
                var resultcodBranch = Int32.TryParse(fileName[3], out codBranch);
                service = _repositoryService.GetService(codCommerce, codBranch);
                serviceName = service != null ? service.Name : String.Empty;

                email = new HighwayEmail
                {
                    Id = id,
                    Sender = dto.Sender,
                    RecipientEmail = dto.RecipientEmail,
                    Subject = dto.Subject,
                    TimeStampSeconds = dto.TimeStampSeconds,
                    AttachmentInputName = dto.AttachmentInputName,
                    Status = (HighwayEmailStatus)(int)dto.Status,
                    Transaccion = nroTrans,
                    AttachmentOutputName = outcomeName,
                    ServiceId = Guid.Empty,
                    AttachmentCreationDate = DateTime.Today,
                    Type = HighwayEmailType.Email
                };
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - Email armado");
                if (dto.Status == HighwayEmailStatusDto.Processing)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail AttachmentInputName " + dto.AttachmentInputName);
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail nroTrans " + nroTrans);

                    #region ATTACHMENT NAME VALIDATION

                    //var fileName = dto.AttachmentInputName.Split('_');
                    if (fileName.Count() != 5)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));
                    if (!fileName[0].Equals("ENVIO", StringComparison.CurrentCultureIgnoreCase))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (!fileName[1].Equals("VNP", StringComparison.CurrentCultureIgnoreCase))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (String.IsNullOrEmpty(fileName[2]))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    //var resultcodCommmerce = Int32.TryParse(fileName[2], out codCommerce);
                    if (!resultcodCommmerce)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codcomercio", fileName[2]));
                    else
                        email.CodCommerce = codCommerce;

                    const int maxcodCommmerce = 7;
                    if (resultcodCommmerce && fileName[2].Count() > maxcodCommmerce)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codcomercio", fileName[2], maxcodCommmerce));

                    //var resultcodBranch = Int32.TryParse(fileName[3], out codBranch);
                    if (!resultcodBranch)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codsucursal", fileName[3]));
                    else
                        email.CodBranch = codBranch;

                    const int maxcodBranch = 3;
                    if (resultcodBranch && fileName[3].Count() > maxcodBranch)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codsucursal", fileName[3], maxcodBranch));

                    //service = _repositoryService.GetService(codCommerce, codBranch);
                    if (service == null)
                    {
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto. El {0} ({1}) y {2} ({3}) enviados no corresponden a un comercio válido.",
                                "CodComercio", codCommerce, "CodSucursal", codBranch));
                        email.Status = HighwayEmailStatus.RejectedServiceNotFound;
                    }
                    else
                        email.ServiceId = service.Id;

                    var fechaString = fileName[4].Split('.');
                    if (fechaString[0].Count() != 8)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));

                    try
                    {
                        var year = Int16.Parse(fechaString[0].Substring(0, 4));
                        var month = Int16.Parse(fechaString[0].Substring(4, 2));
                        var day = Int16.Parse(fechaString[0].Substring(6, 2));
                        var date = new DateTime(year, month, day);
                        email.AttachmentCreationDate = date;
                    }
                    catch (Exception)
                    {
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));
                        email.AttachmentCreationDate = DateTime.Now;
                    }

                    #endregion
                }

                if (generalErrors.Any())
                {
                    email.Status = HighwayEmailStatus.RejectedFileNameError;
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail HighwayEmailStatus.RejectedFileNameError");
                }
                if (email.Status == HighwayEmailStatus.Processing)
                {
                    var str = email.Sender.ToLower();

                    if (service == null || !service.HighwayEnableEmails.Select(x => x.Email.ToLower()).Contains(str))
                    {
                        sendResponse = false;
                        generalErrors.Add(
                            string.Format(
                                "Error de permisos: El email utilizado para el envío ({0}) no está habilitado para el comercio ({1}: {2}).",
                                email.Sender, "codcomercio", codCommerce));
                        email.Status = HighwayEmailStatus.RejectedSenderNotRegistered;
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail HighwayEmailStatus.RejectedSenderNotRegistered");
                    }
                }

                email.Errors = generalErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList();

                _repositoryHighwayEmail.Create(email);
                _repositoryHighwayEmail.Save();

                _repositoryHighwayEmail.ContextTrackChanges = true;

                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail Email guardado");
                if (email.Status == HighwayEmailStatus.Processing)
                {
                    //Obtengo archivo desde blob de azure
                    GetFileFromBlob(dto);

                    if (dto.Status != HighwayEmailStatusDto.Processing)
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - Error en lectura desde el SFTP");
                        if (email.Errors == null) email.Errors = new Collection<HighwayEmailError>();
                        email.Errors.Add(new HighwayEmailError()
                        {
                            Data =
                                string.Format(
                                    "Error en procesamiento. El archivo no se pudo trasnferir por SFTP. "),
                            Id = Guid.NewGuid(),
                            HighwayEmailId = email.Id
                        });
                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                    }
                    else
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - ProcesFile");
                        var newErrors = ProcesFile(dto.FileArray, codCommerce, codBranch, email.Id, service.Id);

                        if (newErrors.Any())
                        {
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - ProcesFile Con errores");
                            email.Status = HighwayEmailStatus.RejectedFileBadlyFormed;
                            var list = email.Errors.ToList();
                            list.AddRange(newErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList());
                            email.Errors = list;
                            _repositoryHighwayEmail.Edit(email);
                            _repositoryHighwayEmail.Save();
                        }
                        else
                        {
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - ProcessFile cambia a ser aceptado");
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmail - Cantidad en Pesos {0}, monto en pesos {1}, cantidad en dolares {2}, monto en dolares {3}",
                                countN, valueN, countD, valueD));
                            email.Status = HighwayEmailStatus.Accepted;

                            var name = dto.AttachmentInputName.Split('.');
                            outcomeName = name[0] + "_" + nroTrans + "." + name[1];
                            email.AttachmentOutputName = outcomeName;
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile outcomeName " + outcomeName);

                            _repositoryHighwayEmail.Edit(email);
                            _repositoryHighwayEmail.Save();
                        }
                    }
                }

                if (sendResponse)
                {
                    if (email.Status == HighwayEmailStatus.ProcessingError)
                    {
                        var msg = "Ha ocurrido un error interno en el procesamiento del archivo.";
                        SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Error interno.", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, msg);
                        return;
                    }
                    //ENVIO MAILS CON ERRORES
                    if (email.Errors.Any())
                    {
                        switch (email.Status)
                        {
                            case HighwayEmailStatus.RejectedFileBadlyFormed:
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Archivo mal formado", HighwayEmailTemplate.ProcessedWithErrors, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, "");
                                break;
                            case HighwayEmailStatus.RejectedFileNameError:
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Archivo mal nombrado", HighwayEmailTemplate.ProcessedWithErrors, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, "");
                                break;
                        }
                    }
                    else
                    {
                        string rejectedMessage;
                        switch (email.Status)
                        {
                            case HighwayEmailStatus.Accepted:
                                SendEmail(email.Sender, "VisaNetPagos - Archivo procesado correctamente. Nro de referencia " + nroTrans, HighwayEmailTemplate.ProcessedOk, null, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, nroTrans.ToString(), serviceName, "");
                                break;
                            case HighwayEmailStatus.RejectedFileNotFound:
                                rejectedMessage = "No se adjuntó el archivo de facturas.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - No se adjuntó archivo de facturas", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                            case HighwayEmailStatus.RejectedFileNotOne:
                                rejectedMessage = "Se envío más de un archivo adjunto.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Se adjuntó más de un archivo", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                            case HighwayEmailStatus.RejectedFileNameBadlyFormed:
                                rejectedMessage = "El nombre del archivo enviado está mal formado.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Nombre de archivo mal formado", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                            case HighwayEmailStatus.RejectedFileType:
                                rejectedMessage = "El tipo de archivo enviado es incorrecto.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Tipo de archivo incorrecto", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                            case HighwayEmailStatus.ProcessingError:
                                rejectedMessage = "Ha ocurrido un error interno en el procesamiento del archivo.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Error interno.", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                            case HighwayEmailStatus.RejectedCommitFailed:
                                rejectedMessage = "Ha ocurrido un error al intentar guardar las facturas.";
                                SendEmail(email.Sender, "VisaNetPagos - Error en procesamiento de archivo - Error interno.", HighwayEmailTemplate.Rejected, email.Errors, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, rejectedMessage);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Error, " ProccessEmail Exception");
                NLogLogger.LogHighwayFileProccessEvent(ex);
                email.Errors.Add(new HighwayEmailError()
                {
                    Data = "VisaNetPagos - Error en procesamiento de archivo: " + ex.Message,
                    Id = Guid.NewGuid(),
                });
                email.Status = HighwayEmailStatus.ProcessingError;
                _repositoryHighwayEmail.Edit(email);
                _repositoryHighwayEmail.Save();
                //ERROR EN PROCESAMIENTO. ENVIO MAIL 
                SendEmail(dto.Sender, "VisaNetPagos - Error en procesamiento de archivo - No se pudo procesar su archivo", HighwayEmailTemplate.Rejected, null, codCommerce.ToString(), codBranch.ToString(), email.AttachmentInputName, "", serviceName, "");
            }
        }

        public ICollection<HighwayEmailErrorDto> ProccessEmailFile(HighwayEmailDto dto)
        {
            var fileName = dto.AttachmentInputName.Split('_');

            Int32 codCommerce = -1;
            Int32 codBranch = -1;
            HighwayEmail email = null;
            var triedToSave = false;

            var resultcodCommmerce = Int32.TryParse(fileName[2], out codCommerce);
            var resultcodBranch = Int32.TryParse(fileName[3], out codBranch);

            Service service = null;
            service = _repositoryService.GetService(codCommerce, codBranch);
            var serviceName = service != null ? service.Name : String.Empty;

            try
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile Estado (" + dto.Status + ")");
                var generalErrors = new List<String>();
                _repositoryHighwayEmail.ContextTrackChanges = true;
                var id = Guid.NewGuid();
                var nroTrans = _repositoryHighwayEmail.GetNextTransactionNumber();
                var outcomeName = "";
                var path = "";

                //Service service = null;

                email = new HighwayEmail
                {
                    Id = id,
                    Sender = dto.Sender,
                    RecipientEmail = dto.RecipientEmail,
                    Subject = dto.Subject,
                    TimeStampSeconds = dto.TimeStampSeconds,
                    AttachmentInputName = dto.AttachmentInputName,
                    Status = (HighwayEmailStatus)(int)dto.Status,
                    Transaccion = nroTrans,
                    AttachmentOutputName = outcomeName,
                    ServiceId = Guid.Empty,
                    AttachmentCreationDate = DateTime.Today,
                    Type = HighwayEmailType.Manual,
                };
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile - Email armado");
                if (dto.Status == HighwayEmailStatusDto.Processing)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile HighwayEmailStatusDto.Processing");
                    if (dto.FileArray != null)
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile AttachmentInputName " + dto.AttachmentInputName);
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile nroTrans " + nroTrans);
                        var name = dto.AttachmentInputName.Split('.');
                        outcomeName = name[0] + "_" + nroTrans + "." + name[1];
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile outcomeName " + outcomeName);

                        path = Path.Combine(ConfigurationManager.AppSettings["HighwayFiles"], outcomeName);
                        File.WriteAllLines(path, dto.FileArray);

                        email.AttachmentOutputName = outcomeName;
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile path " + path);
                    }
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile Llego con archivo");

                    #region ATTACHMENT NAME VALIDATION

                    //var fileName = dto.AttachmentInputName.Split('_');
                    if (fileName.Count() != 5)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));
                    if (!fileName[0].Equals("ENVIO"))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (!fileName[1].Equals("VNP"))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (String.IsNullOrEmpty(fileName[2]))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    //var resultcodCommmerce = Int32.TryParse(fileName[2], out codCommerce);
                    if (!resultcodCommmerce)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codcomercio", fileName[2]));
                    else
                        email.CodCommerce = codCommerce;

                    const int maxcodCommmerce = 7;
                    if (resultcodCommmerce && fileName[2].Count() > maxcodCommmerce)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codcomercio", fileName[2], maxcodCommmerce));

                    //var resultcodBranch = Int32.TryParse(fileName[3], out codBranch);
                    if (!resultcodBranch)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codsucursal", fileName[3]));
                    else
                        email.CodBranch = codBranch;

                    const int maxcodBranch = 3;
                    if (resultcodBranch && fileName[3].Count() > maxcodBranch)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codsucursal", fileName[3], maxcodBranch));

                    service = _repositoryService.GetService(codCommerce, codBranch);
                    if (service == null)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto. El {0} ({1}) y {2} ({3}) enviados no corresponden a un comercio válido.",
                                "CodComercio", codCommerce, "CodSucursal", codBranch));
                    else
                        email.ServiceId = service.Id;

                    var fechaString = fileName[4].Split('.');
                    if (fechaString[0].Count() != 8)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));

                    try
                    {
                        var year = Int16.Parse(fechaString[0].Substring(0, 4));
                        var month = Int16.Parse(fechaString[0].Substring(4, 2));
                        var day = Int16.Parse(fechaString[0].Substring(6, 2));
                        var date = new DateTime(year, month, day);
                        email.AttachmentCreationDate = date;
                    }
                    catch (Exception)
                    {
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));
                        email.AttachmentCreationDate = DateTime.Now;
                    }

                    #endregion
                }

                if (generalErrors.Any())
                {
                    email.Status = HighwayEmailStatus.RejectedFileNameError;
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile HighwayEmailStatus.RejectedFileNameError");
                }

                email.Errors = generalErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList();
                var errorList = generalErrors.Select(x => new HighwayEmailErrorDto() { Data = x, Id = Guid.NewGuid() }).ToList();

                _repositoryHighwayEmail.Create(email);
                _repositoryHighwayEmail.Save();
                triedToSave = true;

                _repositoryHighwayEmail.ContextTrackChanges = true;

                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile Email guardado");
                if (email.Status == HighwayEmailStatus.Processing)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile - ProcessFile");
                    var newErrors = ProcesFile(dto.FileArray, codCommerce, codBranch, email.Id, service.Id);

                    if (newErrors.Any())
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFile - ProcessFile Con errores (" + newErrors.Count + ")");
                        email.Status = HighwayEmailStatus.RejectedFileBadlyFormed;
                        var list = email.Errors.ToList();
                        list.AddRange(newErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList());
                        email.Errors = list;

                        errorList.AddRange(newErrors.Select(x => new HighwayEmailErrorDto() { Data = x, Id = Guid.NewGuid() }).ToList());

                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                    }
                    else
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmail - ProcessFile cambia a ser aceptado");
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmail - Cantidad en Pesos {0}, monto en pesos {1}, cantidad en dolares {2}, monto en dolares {3}",
                            countN, valueN, countD, valueD));
                        email.Status = HighwayEmailStatus.Accepted;
                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                    }
                }
                return errorList;
            }
            catch (Exception ex)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Error, " ProccessEmailFile Exception");
                NLogLogger.LogHighwayFileProccessEvent(ex);

                if (triedToSave)
                {
                    email.Status = HighwayEmailStatus.ProcessingError;
                    _repositoryHighwayEmail.Edit(email);
                    _repositoryHighwayEmail.Save();
                }

                //ERROR EN PROCESAMIENTO. ENVIO MAIL 
                //SendEmail(dto.Sender, "VisaNetPagos - Error en procesamiento de archivo - No se pudo procesar su archivo", HighwayEmailTemplate.Rejected, null, codCommerce.ToString(), codBranch.ToString(), dto.AttachmentInputName, "", serviceName, "");
            }
            return new List<HighwayEmailErrorDto>();
        }

        public ICollection<HighwayEmailErrorDto> ProccessEmailFileExternalSoruce(HighwayEmailDto dto)
        {
            //El archivo ya fue guardado
            var fileName = dto.AttachmentInputName.Split('_');

            Int32 codCommerce = -1;
            Int32 codBranch = -1;
            HighwayEmail email = null;
            var triedToSave = false;

            var resultcodCommmerce = Int32.TryParse(fileName[2], out codCommerce);
            var resultcodBranch = Int32.TryParse(fileName[3], out codBranch);

            Service service = null;
            service = _repositoryService.GetService(codCommerce, codBranch);
            var serviceName = service != null ? service.Name : String.Empty;

            try
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce Estado (" + dto.Status + ")");
                var generalErrors = new List<String>();
                _repositoryHighwayEmail.ContextTrackChanges = true;
                var id = Guid.NewGuid();
                var nroTrans = _repositoryHighwayEmail.GetNextTransactionNumber();
                var outcomeName = "";

                email = new HighwayEmail
                {
                    Id = id,
                    Sender = dto.Sender,
                    RecipientEmail = dto.RecipientEmail,
                    Subject = dto.Subject,
                    TimeStampSeconds = dto.TimeStampSeconds,
                    AttachmentInputName = dto.AttachmentInputName,
                    Status = (HighwayEmailStatus)(int)dto.Status,
                    Transaccion = nroTrans,
                    AttachmentOutputName = outcomeName,
                    ServiceId = Guid.Empty,
                    AttachmentCreationDate = DateTime.Today,
                    Type = HighwayEmailType.Manual,
                };
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce - Email armado");
                if (dto.Status == HighwayEmailStatusDto.Processing)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce HighwayEmailStatusDto.Processing");
                    #region ATTACHMENT NAME VALIDATION

                    if (fileName.Count() != 5)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));
                    if (!fileName[0].Equals("ENVIO"))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (!fileName[1].Equals("VNP"))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (String.IsNullOrEmpty(fileName[2]))
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El nombre del archivo enviado {0} no respeta el formato esperado {1}.",
                                dto.AttachmentInputName, "ENVIO_VNP_<CODCOMERCIO>_<CODSUCURSAL>_<AAAAMMDD>.txt"));

                    if (!resultcodCommmerce)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codcomercio", fileName[2]));
                    else
                        email.CodCommerce = codCommerce;

                    const int maxcodCommmerce = 7;
                    if (resultcodCommmerce && fileName[2].Count() > maxcodCommmerce)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codcomercio", fileName[2], maxcodCommmerce));

                    if (!resultcodBranch)
                        generalErrors.Add(string.Format("Error en archivo adjunto: El {0} ({1}) debe ser numérico.",
                            "codsucursal", fileName[3]));
                    else
                        email.CodBranch = codBranch;

                    const int maxcodBranch = 3;
                    if (resultcodBranch && fileName[3].Count() > maxcodBranch)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: El {0} ({1}) excede la cantidad máxima de dígitos ({2}).",
                                "codsucursal", fileName[3], maxcodBranch));

                    service = _repositoryService.GetService(codCommerce, codBranch);
                    if (service == null)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto. El {0} ({1}) y {2} ({3}) enviados no corresponden a un comercio válido.",
                                "CodComercio", codCommerce, "CodSucursal", codBranch));
                    else
                        email.ServiceId = service.Id;

                    var fechaString = fileName[4].Split('.');
                    if (fechaString[0].Count() != 8)
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));

                    try
                    {
                        var year = Int16.Parse(fechaString[0].Substring(0, 4));
                        var month = Int16.Parse(fechaString[0].Substring(4, 2));
                        var day = Int16.Parse(fechaString[0].Substring(6, 2));
                        var date = new DateTime(year, month, day);
                        email.AttachmentCreationDate = date;
                    }
                    catch (Exception)
                    {
                        generalErrors.Add(
                            string.Format(
                                "Error en archivo adjunto: La fecha del archivo ({0}) no respeta el formato esperado {1}.",
                                fechaString, "AAAAMMDD"));
                        email.AttachmentCreationDate = DateTime.Now;
                    }

                    #endregion
                }

                if (generalErrors.Any())
                {
                    email.Status = HighwayEmailStatus.RejectedFileNameError;
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce HighwayEmailStatus.RejectedFileNameError");
                }

                email.Errors = generalErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList();
                var errorList = generalErrors.Select(x => new HighwayEmailErrorDto() { Data = x, Id = Guid.NewGuid() }).ToList();

                _repositoryHighwayEmail.Create(email);
                _repositoryHighwayEmail.Save();
                triedToSave = true;

                _repositoryHighwayEmail.ContextTrackChanges = true;

                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce Email guardado");
                if (email.Status == HighwayEmailStatus.Processing)
                {
                    //Obtengo archivo desde blob de azure
                    GetFileFromBlob(dto);

                    if (dto.Status != HighwayEmailStatusDto.Processing)
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce - Error en lectura desde el Blob");
                        email.Errors.Add(new HighwayEmailError()
                        {
                            Data = string.Format("Error en procesamiento. Archivo no encontrado en Blob. "),
                            Id = Guid.NewGuid(),
                            HighwayEmailId = email.Id
                        });
                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                        return errorList;
                    }

                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce - ProcessFile");
                    var newErrors = ProcesFile(dto.FileArray, codCommerce, codBranch, email.Id, service.Id);

                    if (newErrors.Any())
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce - ProcessFile Con errores (" + newErrors.Count + ")");
                        email.Status = HighwayEmailStatus.RejectedFileBadlyFormed;
                        var list = email.Errors.ToList();
                        list.AddRange(newErrors.Select(x => new HighwayEmailError() { Data = x, Id = Guid.NewGuid() }).ToList());
                        email.Errors = list;

                        errorList.AddRange(newErrors.Select(x => new HighwayEmailErrorDto() { Data = x, Id = Guid.NewGuid() }).ToList());

                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                    }
                    else
                    {
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce - ProcessFile cambia a ser aceptado");
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("ProccessEmailFileExternalSoruce - Cantidad en Pesos {0}, monto en pesos {1}, cantidad en dolares {2}, monto en dolares {3}",
                            countN, valueN, countD, valueD));
                        email.Status = HighwayEmailStatus.Accepted;

                        var name = dto.AttachmentInputName.Split('.');
                        outcomeName = name[0] + "_" + nroTrans + "." + name[1];
                        email.AttachmentOutputName = outcomeName;
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileExternalSoruce outcomeName " + outcomeName);

                        _repositoryHighwayEmail.Edit(email);
                        _repositoryHighwayEmail.Save();
                    }
                }
                return errorList;
            }
            catch (Exception ex)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Error, " ProccessEmailFileExternalSoruce Exception");
                NLogLogger.LogHighwayFileProccessEvent(ex);

                if (triedToSave)
                {
                    email.Status = HighwayEmailStatus.ProcessingError;
                    _repositoryHighwayEmail.Edit(email);
                    _repositoryHighwayEmail.Save();
                }

                //ERROR EN PROCESAMIENTO. ENVIO MAIL 
                //SendEmail(dto.Sender, "VisaNetPagos - Error en procesamiento de archivo - No se pudo procesar su archivo", HighwayEmailTemplate.Rejected, null, codCommerce.ToString(), codBranch.ToString(), dto.AttachmentInputName, "", serviceName, "");
            }
            return new List<HighwayEmailErrorDto>();
        }

        public List<string> ProcesFile(string[] lines, int codCommerce, int codBranch, Guid emailId, Guid serviceId)
        {
            var generalErrors = new List<String>();
            var bills = new List<HighwayBill>();
            try
            {
                if (!lines.Any())
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileSFTP - Error en archivo adjunto: No se encontraron facturas.");
                    generalErrors.Add(string.Format(
                        "Error en archivo adjunto: No se encontraron facturas "));
                    return generalErrors;
                }

                var lst = lines.Where(t => !string.IsNullOrEmpty(t)).ToList();
                var list1 = lst.Select(t => t.Split('|')).ToList();
                //minimo 14 campos
                var missingData = list1.Where(x => x.Length < 14).ToList();
                if (missingData.Any())
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileSFTP - Error en archivo adjunto: Linea con campos faltantes.");
                    foreach (var data in missingData)
                    {
                        generalErrors.Add(string.Format(
                            "Error en archivo adjunto: Linea con campos faltantes. Nro de campos: {0}, Linea: {1}", data.Length, string.Join("|", data)));
                    }
                    return generalErrors;
                }
                var list2 = list1.Select(l => l[3]).ToList();
                var duplicates = list2.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();

                if (duplicates.Any())
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "ProccessEmailFileSFTP - Error en archivo adjunto: Se encontraron Nro de facturas repetidos.");
                    generalErrors.Add(string.Format(
                        "Error en archivo adjunto: Se encontraron Nro de facturas repetidos. Estos son {0}", string.Join(", ", duplicates)));
                }
                else
                {
                    generalErrors = _repositoryHighwayBill.MasiveInsert(lines, codCommerce, codBranch, emailId,
                        serviceId, out countN, out countD, out valueN, out valueD);
                }

            }
            catch (Exception exception)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    ProcesFile - Exception");
                NLogLogger.LogHighwayFileProccessEvent(exception);
                generalErrors.Add(string.Format("Error en archivo adjunto: Se genero una error en el procesamiento del archivo"));
            }
            if (generalErrors.Any())
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    ProcesFile - Hay errores");
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    ProcesFile - " + string.Join(" ; ", generalErrors));
                return generalErrors;
            }

            return generalErrors;
        }

        private void GetFileFromBlob(HighwayEmailDto dto)
        {
            try
            {
                var lines = FileStorage.Instance.GetTextFileLines(_folderUnprocessedBlob, BlobAccessType.Blob, dto.AttachmentInputName);
                dto.FileArray = lines.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Validation
        private bool FileValidateEmpty(object item, string name, ref List<String> errors, int numeroLinea)
        {
            if (String.IsNullOrEmpty((string)item))
            {
                if (errors != null)
                    errors.Add(
                        string.Format(
                            "Error en archivo adjunto: La línea {0} está mal formada - El campo {1} es obligatorio.",
                            numeroLinea, name));
                return false;
            }
            return true;
        }
        private bool FileValidateMax(object item, string name, ref List<String> errors, int numeroLinea, int largoMaximo)
        {
            if (item.ToString().Length > largoMaximo)
            {
                errors.Add(
                    string.Format(
                        "Error en archivo adjunto. El campo {0} de la línea {1} excede el largo máximo permitido ({2}).",
                        name, numeroLinea, largoMaximo));
                return false;
            }
            return true;
        }
        private bool FileValidatePositiveNumeric(string item, string name, ref List<String> errors, int numeroLinea)
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
                        numeroLinea, name));
            return false;
        }

        private string ValidateEmpty(object item, string name)
        {
            return String.IsNullOrEmpty((string)item) ? string.Format("El campo {0} es obligatorio.", name) : null;
        }
        private string ValidateMax(object item, string name, int largoMaximo)
        {
            return item.ToString().Length > largoMaximo ? string.Format("El campo {0} excede el largo máximo permitido ({1}).", name, largoMaximo) : null;
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

        public List<ServiceEnableEmail> CreateRoutes(List<ServiceEnableEmail> list)
        {
            foreach (var email in list)
            {
                var routeId = _serviceNotificationMessage.CreateRoute(email.Email);
                email.RouteId = routeId;
            }

            return list;
        }

        public void DeleteRoutes(List<ServiceEnableEmail> list)
        {
            foreach (var email in list)
            {
                _serviceNotificationMessage.DeleteRoute(email.RouteId);
            }
        }

        public List<HighwayBillDto> GetBills(string[] references, int codCommerce, int codBrunch)
        {
            try
            {
                var refVal = references[0];
                var query = Repository.AllNoTracking(x => x.CodComercio == codCommerce && x.CodSucursal == codBrunch && x.RefCliente.Equals(refVal));
                return query.Select(x => new HighwayBillDto()
                {
                    CodComercio = x.CodComercio,
                    CodSucursal = x.CodSucursal,
                    Id = x.Id,
                    ServiceId = x.ServiceId,
                    Descripcion = x.Descripcion,
                    FchFactura = x.FchFactura,
                    FchVencimiento = x.FchVencimiento,
                    HighwayEmailId = x.HighwayEmailId,
                    Moneda = x.Moneda,
                    MontoGravado = x.MontoGravado,
                    MontoMinimo = x.MontoMinimo,
                    MontoTotal = x.MontoTotal,
                    NroFactura = x.NroFactura,
                    RefCliente = x.RefCliente,
                    ConsFinal = x.ConsFinal,
                    Cuotas = x.Cuotas,
                    DiasPagoVenc = x.DiasPagoVenc,
                    CreationDate = x.CreationDate
                }).ToList();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error,
                    string.Format(
                        "Carretera - No se pudo obtener la factura. codCommerce: {0}, codBrunch: {1}, references {2}",
                        codCommerce, codBrunch, string.Join(";", references)));
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        /// <summary>
        /// Check if bill is still ready for being paied
        /// </summary>
        /// <param name="gatewayReference"></param>
        /// <param name="serviceType"></param>
        /// <param name="referenceNumbers"></param>
        /// <param name="bills"></param>
        public bool ConfirmPayment(string gatewayReference, string serviceType, string[] referenceNumbers,
            ICollection<BillDto> bills, string transactionNumber)
        {
            try
            {
                var codCom = int.Parse(gatewayReference);
                var codBran = int.Parse(serviceType);
                var refVal = referenceNumbers[0];
                var bill = bills.First();
                var query =
                    _repositoryHighwayBill.AllNoTracking(
                        x =>
                            x.CodComercio == codCom && x.CodSucursal == codBran && x.RefCliente.Equals(refVal) &&
                            x.NroFactura.Equals(bill.BillExternalId));

                var result = query.FirstOrDefault();

                if (result == null)
                {
                    const string msg = "NO SE ENCONTRO FACUTRA DISPONIBLE";
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Carretera,
                        string.Format(LogStrings.Payment_NotifyHighway_Error, msg, transactionNumber, bill.BillExternalId));
                    NotifyErrorPayBill(msg, gatewayReference, serviceType, bill.BillExternalId, transactionNumber, referenceNumbers);
                    throw new BusinessException(msg);
                }

                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Carretera,
                    string.Format(LogStrings.Payment_NotifyHighway_Done, transactionNumber, bill.BillExternalId,
                        bills.FirstOrDefault().Amount, bills.FirstOrDefault().DiscountAmount));
                return true;
            }
            catch (Exception exception)
            {
                var fac = bills != null
                    ? bills.FirstOrDefault() != null ? bills.FirstOrDefault().BillExternalId : ""
                    : "";
                const string msg = "NO SE ENCONTRO FACUTRA DISPONIBLE";
                _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Carretera,
                    string.Format(LogStrings.Payment_NotifyHighway_Error, msg, transactionNumber, fac));
                NLogLogger.LogEvent(exception);
                NotifyErrorPayBill(msg, gatewayReference, serviceType, fac, transactionNumber, referenceNumbers,
                    exception);
                throw;
            }

        }

        public int CheckAccount(int codCommerce, int codBrunch, string[] codigoCuentaEnte)
        {
            try
            {
                //var result = GetBills(codigoCuentaEnte, codCommerce, codBrunch);
                //return result.Any() ? 1 : 0;

                //No se valida si hay facturas para chequear facturas en esta pasarela.
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Carretera - Exception");
                NLogLogger.LogEvent(e);
                throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
            }
        }

        private void NotifyErrorPayBill(string textoresultado, string codCommerce, string codBrunch, string nroFactura, string transactionNumber, string[] codigoCuentaEnte, Exception exception = null)
        {
            var parameters = _parameterRepository.AllNoTracking().First();
            var msg = "Texto resultado: " + textoresultado + ", codCommerce : " + codCommerce + ", codBrunch : " + codBrunch + ", ref: " + string.Join(";", codigoCuentaEnte) + ", nroFactura: " + nroFactura + ", transacción: " + transactionNumber;
            var exceptionMessage = exception != null ? exception.Message : "";
            var stackTrace = exception != null ? exception.StackTrace : "";
            var innerException = exception != null ? exception.InnerException : null;

            _serviceNotificationMessage.SendInternalErrorNotification(parameters, "Error de pago de factura en Carretera.", null, msg, exceptionMessage, stackTrace, innerException);
        }

        private void SendEmail(string to, string subject, HighwayEmailTemplate template, IEnumerable<HighwayEmailError> errors, string codCommerce, string codBranch, string fileName, string transactionNumber, string serviceName, string rejectedMessage)
        {
            var highwayEmailDataDto = new HighwayEmailDataDto()
            {
                CodBranch = codBranch,
                CodCommerce = codCommerce,
                Count = count,
                CountD = countD,
                CountN = countN,
                Email = to,
                Errors = errors != null ? errors.Select(x => new HighwayEmailErrorDto()
                {
                    Data = x.Data,
                    Id = x.Id
                }) : null,
                RejectedMessage = rejectedMessage,
                ServiceName = serviceName,
                Subject = subject,
                TransactionNumber = transactionNumber,
                ValueD = valueD,
                ValueN = valueN,
                FileName = fileName
            };

            switch (template)
            {
                case HighwayEmailTemplate.ProcessedOk:
                    _serviceNotificationMessage.SendHighwayEmailOk(highwayEmailDataDto);
                    break;
                case HighwayEmailTemplate.ProcessedWithErrors:
                    _serviceNotificationMessage.SendHighwayEmailErrors(highwayEmailDataDto);
                    break;
                case HighwayEmailTemplate.Rejected:
                    _serviceNotificationMessage.SendHighwayEmailRejected(highwayEmailDataDto);
                    break;
            }
        }

        public IEnumerable<HighwayEmailDto> GetHighwayEmailsReports_Old(ReportsHighwayEmailFilterDto filters)
        {
            var query = _repositoryHighwayEmail.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.Email.ToLower()));

            if (filters.Branch != null && filters.Branch > 0)
                query = query.Where(x => x.CodBranch == filters.Branch);

            if (filters.Commerce != null && filters.Commerce > 0)
                query = query.Where(x => x.CodCommerce == filters.Commerce);

            if (filters.From != default(DateTime))
            {
                query = query.Where(c => c.CreationDate > filters.From);
            }

            if (filters.To != default(DateTime))
            {
                var d = filters.To.AddDays(1);
                query = query.Where(c => c.CreationDate < d);
            }

            return query.Select(x => new HighwayEmailDto()
            {
                Id = x.Id,
                Sender = x.Sender,
                CodBranch = x.CodBranch,
                CodCommerce = x.CodCommerce,
                CreationDate = x.CreationDate,
                AttachmentInputName = x.AttachmentInputName,
                AttachmentOutputName = x.AttachmentOutputName,
                ServiceId = x.ServiceId,
                Status = (HighwayEmailStatusDto)x.Status,
                Transaccion = x.Transaccion,
                Errors = x.Errors.Select(zz => new HighwayEmailErrorDto()
                {
                    Data = zz.Data,
                    Id = zz.Id
                }).ToList(),
            }).ToList();
        }

        public IEnumerable<HighwayEmailDto> GetHighwayEmailsReports(ReportsHighwayEmailFilterDto filters)
        {
            var query = _repositoryHighwayEmail.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.Email.ToLower()));

            if (filters.Branch != null && filters.Branch > 0)
                query = query.Where(x => x.CodBranch == filters.Branch);

            if (filters.Commerce != null && filters.Commerce > 0)
                query = query.Where(x => x.CodCommerce == filters.Commerce);

            if (filters.From != default(DateTime))
            {
                query = query.Where(c => c.CreationDate > filters.From);
            }

            if (filters.To != default(DateTime))
            {
                var d = filters.To.AddDays(1);
                query = query.Where(c => c.CreationDate < d);
            }

            switch (filters.OrderBy)
            {
                case "0":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.Transaccion) : query.OrderByDescending(c => c.Transaccion);
                    break;
                case "1":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.CodCommerce) : query.OrderByDescending(c => c.CodCommerce);
                    break;
                case "2":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.CodBranch) : query.OrderByDescending(c => c.CodBranch);
                    break;
                case "3":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.Sender) : query.OrderByDescending(c => c.Sender);
                    break;
                case "4":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.CreationDate) : query.OrderByDescending(c => c.CreationDate);
                    break;
                case "5":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status);
                    break;
                case "6":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.AttachmentInputName) : query.OrderByDescending(c => c.AttachmentInputName);
                    break;
                case "7":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.AttachmentOutputName) : query.OrderByDescending(c => c.AttachmentOutputName);
                    break;
                default:
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.Transaccion) : query.OrderByDescending(c => c.Transaccion);
                    break;
            }

            query = query.Skip(filters.DisplayStart);
            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            return query.Select(x => new HighwayEmailDto()
            {
                Id = x.Id,
                Sender = x.Sender,
                CodBranch = x.CodBranch,
                CodCommerce = x.CodCommerce,
                CreationDate = x.CreationDate,
                AttachmentInputName = x.AttachmentInputName,
                AttachmentOutputName = x.AttachmentOutputName,
                ServiceId = x.ServiceId,
                Status = (HighwayEmailStatusDto)x.Status,
                Transaccion = x.Transaccion,
                Errors = x.Errors.Select(zz => new HighwayEmailErrorDto()
                {
                    Data = zz.Data,
                    Id = zz.Id
                }).ToList(),
            }).ToList();
        }

        public int GetHighwayEmailsReportsCount(ReportsHighwayEmailFilterDto filters)
        {
            var query = _repositoryHighwayEmail.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(x => x.Sender.ToLower().Contains(filters.Email.ToLower()));

            if (filters.Branch != null && filters.Branch > 0)
                query = query.Where(x => x.CodBranch == filters.Branch);

            if (filters.Commerce != null && filters.Commerce > 0)
                query = query.Where(x => x.CodCommerce == filters.Commerce);

            if (filters.From != default(DateTime))
            {
                query = query.Where(c => c.CreationDate > filters.From);
            }

            if (filters.To != default(DateTime))
            {
                var d = filters.To.AddDays(1);
                query = query.Where(c => c.CreationDate < d);
            }

            return query.Count();
        }

        public IEnumerable<HighwayBillDto> GetHighwayBillReports(ReportsHighwayBillFilterDto filter)
        {
            var query = Repository.AllNoTracking(null, p => p.Service);

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filter.DateFromString))
            {
                from = DateTime.Parse(filter.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filter.DateToString))
            {
                to = DateTime.Parse(filter.DateToString, new CultureInfo("es-UY"));
            }

            if (filter.CodComercio != null && filter.CodComercio > 0)
                query = query.Where(p => p.CodComercio == filter.CodComercio);

            if (filter.CodSucursal != null && filter.CodSucursal > 0)
                query = query.Where(p => p.CodSucursal == filter.CodSucursal);

            if (!string.IsNullOrEmpty(filter.NroFactura))
                query = query.Where(p => p.NroFactura.Contains(filter.NroFactura));

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate >= from);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate <= dateTo);
            }

            #region sort by
            switch (filter.OrderBy)
            {
                case "0":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.CreationDate) : query.OrderByDescending(p => p.CreationDate);
                    break;
                case "1":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.CodComercio) : query.OrderByDescending(p => p.CodComercio);
                    break;
                case "2":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.CodSucursal) : query.OrderByDescending(p => p.CodSucursal);
                    break;
                case "3":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.RefCliente) : query.OrderByDescending(p => p.RefCliente);
                    break;
                case "4":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Service.Name) : query.OrderByDescending(p => p.Service.Name);
                    break;
                case "5":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.NroFactura) : query.OrderByDescending(p => p.NroFactura);
                    break;
                case "6":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.FchFactura) : query.OrderByDescending(p => p.FchFactura);
                    break;
                case "7":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.FchVencimiento) : query.OrderByDescending(p => p.FchVencimiento);
                    break;
                case "8":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.DiasPagoVenc) : query.OrderByDescending(p => p.DiasPagoVenc);
                    break;
                case "9":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Moneda) : query.OrderByDescending(p => p.Moneda);
                    break;
                case "10":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.MontoTotal) : query.OrderByDescending(p => p.MontoTotal);
                    break;
                case "11":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.MontoMinimo) : query.OrderByDescending(p => p.MontoMinimo);
                    break;
                case "12":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.MontoGravado) : query.OrderByDescending(p => p.MontoGravado);
                    break;
                case "13":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.ConsFinal) : query.OrderByDescending(p => p.ConsFinal);
                    break;
                default:
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.CreationDate) : query.OrderByDescending(p => p.CreationDate);
                    break;
            }
            #endregion

            var queryPaged = query.Skip(filter.DisplayStart).Take((int)filter.DisplayLength);

            return queryPaged.Select(t => new HighwayBillDto
            {
                Id = t.Id,
                CodComercio = t.CodComercio,
                CodSucursal = t.CodSucursal,
                ConsFinal = t.ConsFinal,
                CreationDate = t.CreationDate,
                CreationUser = t.CreationUser,
                //Cuotas = t.Cuotas,
                Descripcion = t.Descripcion,
                DiasPagoVenc = t.DiasPagoVenc,
                FchFactura = t.FchFactura,
                FchVencimiento = t.FchVencimiento,
                LastModificationDate = t.LastModificationDate,
                LastModificationUser = t.LastModificationUser,
                Moneda = t.Moneda,
                MontoGravado = t.MontoGravado,
                MontoMinimo = t.MontoMinimo,
                MontoTotal = t.MontoTotal,
                NroFactura = t.NroFactura,
                RefCliente = t.RefCliente,
                ServiceDto = t.Service != null ? new ServiceDto
                {
                    Id = t.Service.Id,
                    Name = t.Service.Name
                } : new ServiceDto
                {
                    Id = default(Guid),
                    Name = string.Empty
                },
            }).ToList();
        }

        public int GetHighwayBillReportsCount(ReportsHighwayBillFilterDto filter)
        {
            var query = Repository.AllNoTracking(null, p => p.Service);

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filter.DateFromString))
            {
                from = DateTime.Parse(filter.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filter.DateToString))
            {
                to = DateTime.Parse(filter.DateToString, new CultureInfo("es-UY"));
            }

            if (filter.CodComercio != null && filter.CodComercio > 0)
                query = query.Where(p => p.CodComercio == filter.CodComercio);

            if (filter.CodSucursal != null && filter.CodSucursal > 0)
                query = query.Where(p => p.CodSucursal == filter.CodSucursal);

            if (!string.IsNullOrEmpty(filter.NroFactura))
                query = query.Where(p => p.NroFactura.Contains(filter.NroFactura));

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate >= from);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate <= dateTo);
            }

            return query.Select(t => new HighwayBillDto
            {
                Id = t.Id,
            }).Count();
        }

        public List<HighwayBillDto> ProccesBillsFromWcf(WebServiceBillsInputDto dto)
        {
            var errors = new List<HighwayBillDto>();
            Repository.ContextTrackChanges = true;
            try
            {
                var id = Guid.NewGuid();
                var nroTrans = _repositoryHighwayEmail.GetNextTransactionNumber();
                var service = _repositoryService.GetService(dto.CodCommerce, dto.CodBranch);
                var email = new HighwayEmail()
                {
                    Id = id,
                    Status = service != null ? HighwayEmailStatus.Accepted : HighwayEmailStatus.RejectedServiceNotFound,
                    Transaccion = nroTrans,
                    ServiceId = service != null ? service.Id : Guid.Empty,
                    CodCommerce = dto.CodCommerce,
                    CodBranch = dto.CodBranch,
                    Type = HighwayEmailType.WebService,
                    AttachmentCreationDate = DateTime.Now,

                };
                _repositoryHighwayEmail.Create(email);
                email.Errors = new List<HighwayEmailError>();
                //if (dto.ReplaceBills)
                //{
                //    _repositoryHighwayBill.DeleteAllBillsForServiceId(service.Id);
                //}
                if (dto.HighwayBillDtos.Any())
                {
                    var billsInVisa = HighwayBillsAlreadyIn(dto.HighwayBillDtos.Select(x => x.NroFactura).ToList());
                    foreach (var hbDto in dto.HighwayBillDtos)
                    {
                        try
                        {
                            //Si ya existe en visa, no se agrega
                            if (billsInVisa.Select(x => x.NroFactura).Contains(hbDto.NroFactura))
                            {
                                var billDb = billsInVisa.FirstOrDefault(x => x.NroFactura.ToLower().Equals(hbDto.NroFactura.ToLower()));
                                hbDto.Cuotas = billDb.Type == HighwayBillTypeDto.Pending ? 7 : 8;
                                hbDto.Descripcion = billDb.Type == HighwayBillTypeDto.Pending ? "FACTURA YA INGRESADA EN EL SISTEMA." : "FACTURA YA PAGA EN EL SISTEMA.";
                                errors.Add(hbDto);
                                email.Errors.Add(new HighwayEmailError() { Id = Guid.NewGuid(), Data = hbDto.Descripcion + " NROFACTURA: " + hbDto.NroFactura });
                                continue;
                            }

                            var generalErrors = new List<string>();

                            hbDto.CodComercio = email.CodCommerce;
                            hbDto.CodSucursal = email.CodBranch;
                            hbDto.Type = HighwayBillTypeDto.Pending;
                            //Nro de error va en cuotas !

                            var withOutErrors = DataValidationHelper.InputParametersAreValid(hbDto, out generalErrors);

                            if (withOutErrors)
                            {
                                var fFac = hbDto.FchFactura.CompareTo(new DateTime()) == 0;
                                var fVen = hbDto.FchVencimiento.CompareTo(new DateTime()) == 0;
                                if (fFac || fVen)
                                {
                                    hbDto.Cuotas = 3;
                                    hbDto.Descripcion = fFac ? "El campo FchFactura es obligatorio. - " : "";
                                    hbDto.Descripcion = hbDto.Descripcion + (fVen ? "El campo FchVencimiento es obligatorio." : "");
                                    errors.Add(hbDto);
                                    var a = new HighwayEmailError()
                                    {
                                        Id = Guid.NewGuid(),
                                        Data =
                                            hbDto.Descripcion + " NROFACTURA: " +
                                            hbDto.NroFactura
                                    };
                                    email.Errors.Add(a);
                                    _repositoryHighwayEmail.AddEntitiesNoRepository(a);
                                }
                                else
                                {
                                    hbDto.Moneda = hbDto.Moneda.ToLower().Equals("n") ? "UYU" : "USD";
                                    hbDto.HighwayEmailId = id;
                                    hbDto.ServiceId = service.Id;
                                    CreateWithoutSave(hbDto);
                                }
                            }
                            else
                            {
                                hbDto.Cuotas = 2;
                                hbDto.Descripcion = string.Join(" - ", generalErrors);
                                errors.Add(hbDto);
                                email.Errors.Add(new HighwayEmailError() { Id = Guid.NewGuid(), Data = hbDto.Descripcion + " NROFACTURA: " + hbDto.NroFactura });
                            }
                        }
                        catch (Exception exception)
                        {
                            hbDto.Cuotas = 1; //Error en procesamiento
                            errors.Add(hbDto);
                            var str = string.Format("WebService - Error en procesamiento de factura {0} para comercio {1} - sucursal {2}. {3}",
                                hbDto.NroFactura, email.CodCommerce, email.CodBranch, DateTime.Now);
                            NLogLogger.LogHighwayEvent(NLogType.Error, str);
                            NLogLogger.LogHighwayEvent(exception);
                            email.Errors.Add(new HighwayEmailError() { Id = Guid.NewGuid(), Data = str });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var str = string.Format("WebService - Error general en procesamiento de factura {0}", DateTime.Now);
                NLogLogger.LogHighwayEvent(NLogType.Error, str);
                NLogLogger.LogHighwayEvent(exception);
            }
            finally
            {
                _repositoryHighwayEmail.Save();
                _repositoryHighwayBill.Save();
            }
            Repository.ContextTrackChanges = false;
            return errors;
        }

        public HighwayEmailDto GetHighwayEmail(Guid emailId)
        {
            var entity = _repositoryHighwayEmail.GetById(emailId, x => x.Errors);
            return Converter(entity);
        }

        private List<HighwayBillDto> HighwayBillsAlreadyIn(List<string> nroFacturas)
        {
            return
                AllNoTracking()
                    .Where(
                        x =>
                            nroFacturas.Contains(x.NroFactura) &&
                            (x.Type == HighwayBillTypeDto.Pending || x.Type == HighwayBillTypeDto.Paid))
                    .Select(x => new HighwayBillDto() { NroFactura = x.NroFactura, Type = x.Type })
                    .ToList();
        }

        private void CreateWithoutSave(HighwayBillDto entity)
        {
            var efEntity = Converter(entity);
            _repositoryHighwayBill.Create(efEntity);
        }

        public int DeleteBills(WebServiceBillsDeleteDto billsNroFactura)
        {
            try
            {
                Repository.ContextTrackChanges = true;
                var save = true;
                var billsDto = _repositoryHighwayBill.All(x => billsNroFactura.NroFacturas.Contains(x.NroFactura));
                foreach (var hbill in billsDto)
                {
                    if (hbill.CodComercio.Equals(billsNroFactura.CodCommerce) && hbill.CodSucursal.Equals(billsNroFactura.CodBranch))
                        _repositoryHighwayBill.Delete(hbill.Id);
                    else
                        save = false;
                }

                if (save)
                    _repositoryHighwayBill.Save();
                else
                {
                    Repository.ContextTrackChanges = false;
                    return 2;
                }


            }
            catch (Exception exception)
            {
                var str = string.Format("WebService - Error en quitar facturas . Nro de factura: {0}", string.Join(";", billsNroFactura));
                NLogLogger.LogHighwayEvent(NLogType.Error, str);
                NLogLogger.LogHighwayEvent(exception);
                Repository.ContextTrackChanges = false;
                return 1;
            }
            Repository.ContextTrackChanges = false;
            return 0;
        }

        public IOrderedQueryable<HighwayBill> StatusBIlls(WebServiceBillsStatusInputDto dto)
        {
            var query = Repository.AllNoTracking(null, x => x.AuxiliarData).Where(x => x.CodComercio == dto.CodComercio && x.CodSucursal == dto.CodSucursal);

            if (!string.IsNullOrEmpty(dto.NroFactura))
                query.Where(x => x.NroFactura.ToLower().Equals(dto.NroFactura));

            if (!string.IsNullOrEmpty(dto.RefCliente1))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente1));
            }
            if (!string.IsNullOrEmpty(dto.RefCliente2))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente2));
            }
            if (!string.IsNullOrEmpty(dto.RefCliente3))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente3));
            }
            if (!string.IsNullOrEmpty(dto.RefCliente4))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente4));
            }
            if (!string.IsNullOrEmpty(dto.RefCliente5))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente5));
            }
            if (!string.IsNullOrEmpty(dto.RefCliente6))
            {
                query.Where(x => x.RefCliente.ToLower().Equals(dto.RefCliente6));
            }

            if (dto.FechaDesde != new DateTime() && dto.FechaHasta != new DateTime())
                query.Where(x => x.CreationDate >= dto.FechaDesde && x.CreationDate <= dto.FechaHasta);

            return query.OrderBy(x => x.NroFactura);
        }

        public void ChangeType(Guid billId, HighwayBillType type, int errorCode, string errorDesc)
        {
            _repositoryHighwayBill.ContextTrackChanges = true;
            var bill = _repositoryHighwayBill.GetById(billId);
            bill.Type = type;

            if (errorCode > 0)
                bill.ErrorCode = errorCode;

            if (!string.IsNullOrEmpty(errorDesc))
                bill.ErrorDesc = errorDesc;

            _repositoryHighwayBill.Edit(bill);
            _repositoryHighwayBill.Save();

            _repositoryHighwayBill.ContextTrackChanges = false;
        }

    }
}