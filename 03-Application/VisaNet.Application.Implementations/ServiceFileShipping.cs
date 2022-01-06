using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Components.Sistarbanc.Implementations.Implementations;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.ExtensionMethods;
using VisaNet.Utilities.SFTPClient;

namespace VisaNet.Application.Implementations
{
    public class ServiceFileShipping : BaseService<Bill, BillDto>, IServiceFileShipping
    {
        private readonly IServicePayment _servicePayment;
        private readonly IRepositoryService _repositoryService;
        private readonly IServiceParameters _serviceParameters;
        private readonly IServiceBin _binSerice;
        private readonly IRepositoryParameters _parameterRepository;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly ISuciveAccess _suciveAccess;

        public ServiceFileShipping(IRepositoryBill repository, IServicePayment servicePayment, IRepositoryService repositoryService, IServiceParameters serviceParameters, IRepositoryBank bankRepo, IServiceBin binSerice, IRepositoryParameters parameterRepository, IServiceEmailMessage serviceNotificationMessage, ISuciveAccess suciveAccess)
            : base(repository)
        {
            _servicePayment = servicePayment;
            _repositoryService = repositoryService;
            _serviceParameters = serviceParameters;
            _binSerice = binSerice;
            _parameterRepository = parameterRepository;
            _serviceNotificationMessage = serviceNotificationMessage;
            _suciveAccess = suciveAccess;
        }

        public override IQueryable<Bill> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override BillDto Converter(Bill entity)
        {
            if (entity == null) return null;

            return new BillDto
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                PaymentId = entity.PaymentId,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount,
                Discount = entity.Discount,
                DiscountAmount = entity.DiscountAmount,
                GatewayTransactionId = entity.GatewayTransactionId,
                SucivePreBillNumber = entity.SucivePreBillNumber
            };
        }

        public override Bill Converter(BillDto entity)
        {
            if (entity == null) return null;

            return new Bill
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                PaymentId = entity.PaymentId,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount,
                Discount = entity.Discount,
                DiscountAmount = entity.DiscountAmount,
                GatewayTransactionId = entity.GatewayTransactionId,
                SucivePreBillNumber = entity.SucivePreBillNumber

            };
        }

        private void NotifyError(string title, string msg, Exception exception = null)
        {
            var parameters = _parameterRepository.AllNoTracking().First();
            var exceptionMessage = exception != null ? exception.Message : "";
            var stackTrace = exception != null ? exception.StackTrace : "";
            var innerException = exception != null ? exception.InnerException : null;

            _serviceNotificationMessage.SendInternalErrorNotification(parameters, title, null, msg, exceptionMessage, stackTrace, innerException);
        }

        #region Sistarbanc Conciliation
        public void SistarbancBatchConsiliation()
        {
            //No se envia el archivo el dia siguiente a un dia no habil
            var day = Int16.Parse(ConfigurationManager.AppSettings["SistarbancBatchDay"]);
            //PRIMER DIA CON FACTURAS
            var date = DateTime.Now.AddDays(-day);

            //verifico si es un dia habil
            var holidaysSplit = ConfigurationManager.AppSettings["Holidays"].Split(';');


            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || holidaysSplit.Contains(date.ToString("dd/MM")))
            //if (date.AddDays(-1).DayOfWeek == DayOfWeek.Sunday || date.AddDays(-1).DayOfWeek == DayOfWeek.Monday || holidaysSplit.Contains(date.AddDays(-1).ToString("dd/MM")))
            {
                NLogLogger.LogSistarbancEvent(NLogType.Info, "  PROCESO BATCH SISTARBANC - Fecha (" + date.ToString("dd/MM/yyyy") + ") no hábil. No se genera archivo de conciliación.");
            }
            else
            {
                //sino, verifico cuantos dias hacia atras debo enviar
                int dayToCheck = day + 1;
                int daysBack = 0;
                bool continueChecking = true;
                while (continueChecking)
                {
                    var dayChk = DateTime.Now.AddDays(-dayToCheck);
                    if (dayChk.DayOfWeek == DayOfWeek.Saturday || dayChk.DayOfWeek == DayOfWeek.Sunday ||
                        holidaysSplit.Contains(dayChk.ToString("dd/MM")))
                    {
                        dayToCheck = dayToCheck + 1;
                        daysBack = daysBack + 1;
                    }
                    else
                        continueChecking = false;
                }
                SistarbancBatchProcess(day, daysBack);
            }

            NLogLogger.LogSistarbancEvent(NLogType.Info, "  FIN PROCESO GENERACION ARCHIVO BATCH");
            NLogLogger.LogSistarbancEvent(NLogType.Info, "  FECHA: " + DateTime.Now.ToString("G"));
        }

        private void SistarbancBatchProcess(int fromDayBack, int amountDaysBack)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["SistarbancBatchPath"];

                var date = DateTime.Today.AddDays(-fromDayBack);
                NLogLogger.LogSistarbancEvent(NLogType.Info, "  INICIA PROCESO GENERACION ARCHIVO BATCH PARA SISTARBANC PARA LA FECHA " + date.ToString("d"));

                var pathBaco = ConfigurationManager.AppSettings["SistarbancBatchPathBaco"];
                var parameters = _serviceParameters.AllNoTracking().FirstOrDefault();

                var fileNameNoBrou = parameters == null ? "" : path + parameters.Sistarbanc.Code + "_" + date.ToString("yyyyMMdd") + ".txt";
                var fileNameBrou = parameters == null ? "" : path + parameters.SistarbancBrou.Code + "_" + date.ToString("yyyyMMdd") + ".txt";
                var fileNameNoBrouBaco = parameters == null ? "" : pathBaco + parameters.Sistarbanc.Code + "_" + date.ToString("yyyyMMdd") + ".txt";
                var fileNameBrouBaco = parameters == null ? "" : pathBaco + parameters.SistarbancBrou.Code + "_" + date.ToString("yyyyMMdd") + ".txt";

                if (File.Exists(fileNameBrou))
                {
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "  Ya existe archivo en el path " + fileNameBrou);
                    return;
                }

                if (File.Exists(fileNameNoBrou))
                {
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "  Ya existe archivo en el path " + fileNameNoBrou);
                    return;
                }

                //if (File.Exists(fileNameNoBrouBaco))
                //{
                //    NLogLogger.LogSistarbancEvent(NLogType.Info, "  Ya existe archivo en el path " + fileNameNoBrouBaco);
                //    return;
                //}

                //if (File.Exists(fileNameBrouBaco))
                //{
                //    NLogLogger.LogSistarbancEvent(NLogType.Info, "  Ya existe archivo en el path " + fileNameBrouBaco);
                //    return;
                //}

                //obtengo las del primer dia indicado
                var brouPayments = FilterBrouPayments(_servicePayment.GetPaymentBatch(date, GatewayEnum.Sistarbanc, Guid.Empty, -1), true);
                var noBroupayments = FilterBrouPayments(_servicePayment.GetPaymentBatch(date, GatewayEnum.Sistarbanc, Guid.Empty, -1), false);

                //mientras tenga que seguir obteniendo hacia atras (por dia no habil)
                var auxDays = 1;
                while (auxDays <= amountDaysBack)
                {
                    var dateBack = DateTime.Today.AddDays(-(fromDayBack + auxDays));

                    NLogLogger.LogSistarbancEvent(NLogType.Info, "  AGREGO FACTURAS DE LA FECHA " + dateBack.ToString("d"));

                    var brouPaymentsBack = FilterBrouPayments(_servicePayment.GetPaymentBatch(dateBack, GatewayEnum.Sistarbanc, Guid.Empty, -1), true);
                    var noBroupaymentsBack = FilterBrouPayments(_servicePayment.GetPaymentBatch(dateBack, GatewayEnum.Sistarbanc, Guid.Empty, -1), false);

                    brouPayments = brouPayments.Concat(brouPaymentsBack).ToList();
                    noBroupayments = noBroupayments.Concat(noBroupaymentsBack).ToList();

                    auxDays = auxDays + 1;
                }

                var listadoBrou = GenerateFileString(brouPayments, true);
                var listadoNoBrou = GenerateFileString(noBroupayments, false);

                var created = CreateFile(listadoBrou, fileNameBrou);
                if (created)
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "      SE GENERO EL ARCHIVO " + fileNameBrou);

                created = CreateFile(listadoNoBrou, fileNameNoBrou);
                if (created)
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "      SE GENERO EL ARCHIVO " + fileNameNoBrou);

                //AGREGO ESTOS MISMOS DOS A OTRO PATH, PARA PROCESAMIENTO DE VISA Y ENVIO A SISTARBANC
                created = CreateFile(listadoBrou, fileNameBrouBaco);
                if (created)
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "      SE GENERO EL ARCHIVO " + fileNameBrouBaco);
                created = CreateFile(listadoNoBrou, fileNameNoBrouBaco);
                if (created)
                    NLogLogger.LogSistarbancEvent(NLogType.Info, "      SE GENERO EL ARCHIVO " + fileNameNoBrouBaco);
            }
            catch (Exception e)
            {
                NLogLogger.LogSistarbancEvent(NLogType.Error, "     ERROR AL PROCESAR SistarbancBatchConsiliation");
                NLogLogger.LogSistarbancEvent(e);
            }
        }

        private IEnumerable<PaymentDto> FilterBrouPayments(IEnumerable<PaymentDto> allPayments, bool filterBrou)
        {
            var brouPayments = new List<PaymentDto>();
            var noBrouPayments = new List<PaymentDto>();

            foreach (var payment in allPayments)
            {
                var bin = payment.Card.MaskedNumber.Substring(0, 6);
                var binDto = _binSerice.Find(int.Parse(bin));

                if (binDto != null && binDto.BankDtoId != null && binDto.BankDtoId != Guid.Empty)
                {
                    //CODIGO DE BROU
                    if (binDto.BankDto.Name.Equals("BROU"))
                    {
                        brouPayments.Add(payment);
                    }
                    else
                    {
                        noBrouPayments.Add(payment);
                    }
                }
                else
                {
                    noBrouPayments.Add(payment);
                }
            }

            return filterBrou ? brouPayments : noBrouPayments;
        }

        private static string GenerateFileString(IEnumerable<PaymentDto> payments, bool filterBrou)
        {
            double totalAmount = 0;
            var totalDetails = 0;
            var signatureList = new List<String>();

            foreach (var dto in payments)
            {
                foreach (var billDto in dto.Bills)
                {
                    totalDetails++;
                    var amountNeto = (billDto.Amount - billDto.DiscountAmount) * 100;
                    totalAmount = totalAmount + (billDto.Amount - billDto.DiscountAmount);
                    long userId = 0;
                    if (dto.RegisteredUserId != null && Guid.Empty != dto.RegisteredUserId)
                    {
                        userId = filterBrou ? dto.RegisteredUser.SistarbancBrouUser.UniqueIdentifier : dto.RegisteredUser.SistarbancUser.UniqueIdentifier;
                    }
                    if (dto.AnonymousUserId != null && Guid.Empty != dto.AnonymousUserId)
                    {
                        userId = filterBrou ? dto.AnonymousUser.SistarbancBrouUser.UniqueIdentifier : dto.AnonymousUser.SistarbancUser.UniqueIdentifier;
                    }

                    //List for signature
                    signatureList.Add('"' + "D" + '"');
                    signatureList.Add("," + '"' + dto.ServiceDto.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc).ReferenceId + '"');
                    signatureList.Add("," + '"' + dto.ServiceDto.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc).ServiceType + '"');
                    signatureList.Add("," + '"' + billDto.BillExternalId + '"');
                    signatureList.Add("," + '"' + dto.Date.ToString("yyyyMMdd") + '"');
                    signatureList.Add("," + amountNeto);
                    signatureList.Add("," + '"' + billDto.Currency + '"');
                    signatureList.Add("," + billDto.GatewayTransactionId);
                    signatureList.Add("," + '"' + userId + '"');
                    signatureList.Add("," + dto.PaymentIdentifierDto.UniqueIdentifier);
                    signatureList.Add("," + '"' + "B" + '"');
                    signatureList.Add(Environment.NewLine);

                }
            }

            signatureList.Add('"' + "T" + '"' + "," + totalDetails + "," + totalAmount * 100);
            signatureList.Add(Environment.NewLine);
            var signature = SistarbancDigitalSignature.GenerateSignature(signatureList.ToArray());

            var result = signatureList.Aggregate("", (current, str) => current + str);

            NLogLogger.LogSistarbancEvent(NLogType.Info, string.Format("  Se encontraron {0} transacciónes con tarjetas del {1} para informar ", totalDetails, filterBrou ? "BROU" : "NO-BROU"));

            return result + '"' + "F" + '"' + "," + signature;

        }
        #endregion

        #region Sucive Conciliation
        public void SuciveBatchConsiliation(DateTime date)
        {
            try
            {
                var sucivePath = ConfigurationManager.AppSettings["SuciveBatchPath"];
                
                var parameters = _serviceParameters.AllNoTracking().FirstOrDefault();
                var filePath = parameters == null ? "" : sucivePath + parameters.Sucive.Code + "_" + date.ToString("yyyyMMdd") + ".txt";

                if (File.Exists(filePath))
                {
                    NLogLogger.LogSuciveEvent(NLogType.Info, "El archivo ya existe. Ruta " + filePath);
                    return;
                }

                var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Sucive, Guid.Empty, -1);
                NLogLogger.LogSuciveEvent(NLogType.Info, "HAY " + (payments == null ? "0" : payments.Count().ToString()) + " TRANSACCIONES PARA ENVIAR");

                var list = GenerateSuciveFileString(payments, parameters.Sucive.Code);

                var created = CreateFile(list, filePath);
                if (created)
                    NLogLogger.LogSuciveEvent(NLogType.Info, "      SE GENERO EL ARCHIVO " + filePath);

                SendFileSftp(filePath, parameters.Sucive.Code + "_" + date.ToString("yyyyMMdd") + ".txt");
            }
            catch (Exception e)
            {
                NLogLogger.LogSuciveEvent(NLogType.Info, "SE PRODUJO UNA EXCEPCION");
                NLogLogger.LogSuciveEvent(e);

                throw e;
            }
            finally
            {
                NLogLogger.LogSuciveEvent(NLogType.Info, "FIN PROCESO GENERACION ARCHIVO BATCH");
            }
        }
        private string GenerateSuciveFileString(IEnumerable<PaymentDto> payments, string code)
        {
            try
            {
                if (payments == null || !payments.Any())
                    return "NO EXISTEN DATOS PARA ENVIAR";

                var raw = "CodigoAutorizacion;IdMedioPago;FechaPago;Monto;NroCobro;IdPadron;IdGobiernoDepartamental;LugarCobro;NroDebito" + Environment.NewLine;

                foreach (var dto in payments)
                {
                    var idPadron = dto.ReferenceNumber6;
                    if (string.IsNullOrEmpty(idPadron))
                    {
                        idPadron = GetIdPadron(dto);
                    }

                    if (dto.Bills != null && dto.Bills.Any())
                    {
                        var dtpo = GetDeptoFromService(dto);

                        raw = raw + dto.PaymentIdentifierDto.UniqueIdentifier + ";";
                        raw = raw + code + ";";
                        raw = raw + dto.Date.ToString("ddMMyyyyHHmm") + ";";
                        raw = raw + dto.Bills.First().Amount.SignificantDigits(2).ToString("#0.00", CultureInfo.CurrentCulture) + ";";
                        raw = raw + dto.Bills.First().BillExternalId + ";";
                        raw = raw + idPadron + ";";
                        raw = raw + SuciveDepartamentValue(dtpo) + ";";
                        raw = raw + ";";
                        raw = raw + "0;";
                        raw = raw + Environment.NewLine;
                    }
                }

                NLogLogger.LogSuciveEvent(NLogType.Info, raw);

                raw = raw + payments.Count();
                return raw;
            }
            catch (Exception e)
            {
                NLogLogger.LogSuciveEvent(NLogType.Error, "Error GenerateSuciveFileString");
                NLogLogger.LogSuciveEvent(e);
                throw e;
            }
        }

        private string GetIdPadron(PaymentDto dto)
        {
            if (dto.ServiceAssociatedDto != null && !string.IsNullOrEmpty(dto.ServiceAssociatedDto.ReferenceNumber6))
            {
                return dto.ServiceAssociatedDto.ReferenceNumber6;
            }

            var refs = new string[] { dto.ReferenceNumber, dto.ReferenceNumber2, dto.ReferenceNumber3, dto.ReferenceNumber4, dto.ReferenceNumber5, dto.ReferenceNumber6 };
            var serviceGateway = dto.ServiceDto.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int) GatewayEnumDto.Sucive);
            if (serviceGateway == null) 
                return string.Empty;

            var result = _suciveAccess.CheckAccount(refs, serviceGateway.ReferenceId, serviceGateway.ServiceType, (int) dto.ServiceDto.Departament);

            return result > 0 ? result.ToString() : string.Empty;

        }

        private int SuciveDepartamentValue(int dptoVisanetValue)
        {
            switch (dptoVisanetValue)
            {
                case (int)DepartamentDtoType.Artigas:
                    return 1;
                case (int)DepartamentDtoType.Canelones:
                    return 2;
                case (int)DepartamentDtoType.Cerro_Largo:
                    return 3;
                case (int)DepartamentDtoType.Colonia:
                    return 4;
                case (int)DepartamentDtoType.Durazno:
                    return 5;
                case (int)DepartamentDtoType.Flores:
                    return 6;
                case (int)DepartamentDtoType.Florida:
                    return 16;
                case (int)DepartamentDtoType.Lavalleja:
                    return 18;
                case (int)DepartamentDtoType.Maldonado:
                    return 14;
                case (int)DepartamentDtoType.Montevideo:
                    return 19;
                case (int)DepartamentDtoType.Paysandu:
                    return 8;
                case (int)DepartamentDtoType.Rio_Negro:
                    return 11;
                case (int)DepartamentDtoType.Rivera:
                    return 17;
                case (int)DepartamentDtoType.Rocha:
                    return 15;
                case (int)DepartamentDtoType.Salto:
                    return 7;
                case (int)DepartamentDtoType.San_Jose:
                    return 12;
                case (int)DepartamentDtoType.Soriano:
                    return 10;
                case (int)DepartamentDtoType.Tacuarembo:
                    return 9;
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    return 13;

                default:
                    return -1;
            }
        }
        private void SendFileSftp(string filePath, string fileName)
        {
            var options = new SftpConfigurationOptions()
            {
                SshPrivateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key"),
                SshPrivateKeyName = ConfigurationManager.AppSettings["SshPrivateKeyName"],
                    HostName = ConfigurationManager.AppSettings["SFTPHostName"],
                    PortNumber = int.Parse(ConfigurationManager.AppSettings["SFTPPortNumber"]),
                    SshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"],
                UsePassAndCertificate = true,
                Password = ConfigurationManager.AppSettings["SFTPPassword"],
                    UserName = ConfigurationManager.AppSettings["SFTPUserName"],
                SessionLogPath = ConfigurationManager.AppSettings["SuciveBatchPath"]+ @"\log.txt",
                };

            var sftpClient = new SftpClient(options);
            sftpClient.SendFile(filePath, fileName);
            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    Intento realizado de envio SFTP de archivo (" + fileName + ")");

            }
        private int GetDeptoFromService(PaymentDto dto)
        {
            var service = _repositoryService.GetById(dto.ServiceId);
            var dtpo = (int)service.Departament;

            return dtpo;
        }
        #endregion

        #region Geocom
        public void NotifyPaymentsToGeocom(DateTime date)
        {
            foreach (DepartamentDtoType depto in Enum.GetValues(typeof(DepartamentDtoType)))
            {
                var val = ConfigurationManager.AppSettings[depto.ToString()];
                if (!string.IsNullOrEmpty(val))
                {
                    NLogLogger.LogGeocomEvent(NLogType.Info, string.Format("        Departamento ({0}). ", depto));
                    NotifyGeocomFile((int)depto, date, val);
                }
            }
        }
        private void NotifyGeocomFile(int departament, DateTime date, string emailTo)
        {
            try
            {
                var parameters = _serviceParameters.AllNoTracking().FirstOrDefault();
                var depto = ((DepartamentDtoType)departament).ToString();
                var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Geocom, Guid.Empty, departament);

                if (payments == null || !payments.Any())
                {
                    NLogLogger.LogGeocomEvent(NLogType.Info,
                        string.Format("        Departamento ({0}) no tiene transacciones realizadas. ", depto));

                    var result = _serviceNotificationMessage.SendExtract(EmailType.ExtractGeocom, string.Format("VisaNetPagos - Reporte de pagos - {0} {1}", depto, date.ToString("dd/MM/yyyy")),
                           string.Format("No se realizaron pagos para la fecha {0}", date.ToString("dd/MM/yyyy")), emailTo);
                    LogResult(result, depto,emailTo);

                    return;
                }

                var fileName = parameters.Geocom.Code + "_" + date.ToString("yyyyMMdd") + ".txt";
                var path = Path.Combine(ConfigurationManager.AppSettings["FileShippingGeneratedFilesGeocom"], fileName);
                if (File.Exists(path))
                {
                    NLogLogger.LogGeocomEvent(NLogType.Info,
                        string.Format("        Departamento ({0}) ya tiene un archivo generado ({1}). ", depto,
                            fileName));
                    return;
                }

                var raw = "";
                var count = 0;
                double amountTotal = 0;

                foreach (var dto in payments)
                {
                    var bill = dto.Bills.FirstOrDefault();
                    amountTotal = amountTotal + bill.Amount;
                    var split = bill.Amount.ToString("0.00", CultureInfo.InvariantCulture);
                    var parts = split.Split('.');
                    raw = raw + bill.SucivePreBillNumber.PadLeft(9, '0');
                    raw = raw + parts[0].PadLeft(9, '0');
                    raw = raw + parts[1].PadLeft(2, '0');
                    raw = raw + dto.Date.ToString("yyyy");
                    raw = raw + dto.Date.ToString("MM");
                    raw = raw + dto.Date.ToString("dd");
                    raw = raw + dto.Date.ToString("HH");
                    raw = raw + dto.Date.ToString("mm");
                    raw = raw + "".PadLeft(5, '0');
                    raw = raw + bill.GatewayTransactionId.PadLeft(9, '0');
                    raw = raw + Environment.NewLine;
                    count++;
                }

                var total = amountTotal.ToString("0.00", CultureInfo.InvariantCulture);
                var totalSplit = total.Split('.');
                raw = raw + "EOF" + count.ToString().PadLeft(5, '0') + totalSplit[0].PadLeft(9, '0') + totalSplit[1].PadLeft(2, '0');

                var created = CreateFile(raw, path);

                if (created)
                {
                    NLogLogger.LogGeocomEvent(NLogType.Info,
                        string.Format("        Departamento ({0}) se creo archivo ({1}). ", depto, fileName));
                }
                else
                {
                    NLogLogger.LogGeocomEvent(NLogType.Info,
                        string.Format("        Departamento ({0}) no se pudo crear archivo ({1}). ", depto, fileName));
                }

                var result1 = _serviceNotificationMessage.SendExtract(EmailType.ExtractGeocom, string.Format("VisaNetPagos - Reporte de pagos - {0} {1}", depto, date.ToString("dd/MM/yyyy")),
                            string.Format("Estas son las transacciones realizadas para la fecha {0}", date.ToString("dd/MM/yyyy")), emailTo, path, fileName, "text/plain");

                LogResult(result1, depto, emailTo);

            }

            catch (Exception exception)
            {
                NLogLogger.LogGeocomEvent(NLogType.Error, string.Format("Excepcion en Geocom - Departamento ({0}), email to {1} ", departament, emailTo));
                NLogLogger.LogGeocomEvent(exception);
            }
        }
        #endregion

        public void NotifyPaymentsToService(DateTime date, int gateway)
        {
            var services = _repositoryService.All(s => s.Active && s.ServiceGateways.Any(g => g.Gateway.Enum == gateway && g.Active && g.SendExtract)
                , s => s.ServiceGateways.Select(g => g.Gateway)).ToList();

            NLogLogger.LogExtractEvent(NLogType.Info, "    Servicios a enviar : " + services.Count);
            foreach (var service in services)
            {
                switch (gateway)
                {
                    case (int)GatewayEnum.Banred:
                        NotifyBanredFile(service, date);
                        break;
                    case (int)GatewayEnum.Sistarbanc:
                        NLogLogger.LogExtractEvent(NLogType.Info, "No esta configurado el extract de Sistarbanc");
                        break;
                    case (int)GatewayEnum.Importe:
                        NotifyImporteFile(service, date);
                        break;
                }
            }
        }

        #region Banred
        private void NotifyBanredFile(Service service, DateTime date)
        {
            try
            {
                var serGateway =
                            service.ServiceGateways.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnum.Banred);
                if (serGateway == null)
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene pasarela cargada. ", service.Name));
                    return;
                }

                var codcom = serGateway.ReferenceId;
                var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Banred, service.Id, -1);

                if (payments == null || !payments.Any())
                {
                    NLogLogger.LogExtractEvent(NLogType.Info, string.Format("        Servicio ({0}) no tiene transacciones realizadas. ", service.Name));

                    var result = _serviceNotificationMessage.SendExtract(EmailType.ExtractBanred, string.Format("VisaNetPagos - Reporte de pagos Banred (Extract) - {0} {1}", service.Name, date.ToString("dd/MM/yyyy")),
                            string.Format("No se realizaron pagos para la fecha {0}", date.ToString("dd/MM/yyyy")), service.ExtractEmail);
                    LogResult(result, service.Name, service.ExtractEmail);
                    return;
                }

                var fileName = "EXTRACT " + codcom + "-" + date.ToString("yyyyMMdd") + ".txt";

                var path = Path.Combine(ConfigurationManager.AppSettings["FileShippingGeneratedFiles"], fileName);

                if (File.Exists(path))
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) ya tiene un archivo generado ({1}). ", service.Name,
                            fileName));
                    return;
                }

                var raw = "";
                var count = 0;
                var countPesos = 0;
                var countDollars = 0;
                double amountPesos = 0;
                double amountDollars = 0;
                double discountAmountPesos = 0;
                double discountAmountDollars = 0;

                foreach (var dto in payments)
                {
                    var bill = dto.Bills.FirstOrDefault();
                    var amount = bill != null ? (bill.Amount - bill.DiscountAmount) : 0;
                    var discountAmount = bill != null ? (bill.DiscountAmount) : 0;
                    raw = raw + codcom + ":";
                    raw = raw + dto.ReferenceNumber + ":";
                    raw = raw + (bill != null ? bill.BillExternalId : "") + ":";
                    raw = raw + amount.ToString().Replace(',', '.') + ":";
                    raw = raw + (bill != null ? bill.Currency.Equals("UYU") ? "N" : "D" : "") + ":";
                    raw = raw + dto.Date.ToString("yyyyMMdd") + ":";
                    raw = raw + "T" + ":"; //forma de pago (actualmente no aplica y va "T")
                    raw = raw + "0" + ":"; //cantidad de cuotas (actualmente no aplica y va "0")
                    raw = raw + discountAmount.ToString().Replace(',', '.');
                    raw = raw + Environment.NewLine;
                    count++;
                    if (bill.Currency.Equals("UYU"))
                    {
                        countPesos++;
                        amountPesos = amountPesos + amount;
                        discountAmountPesos = discountAmountPesos + discountAmount;
                    }
                    if (bill.Currency.Equals("USD"))
                    {
                        countDollars++;
                        amountDollars = amountDollars + amount;
                        discountAmountDollars = discountAmountDollars + discountAmount;
                    }
                }
                raw = raw + "TOT:" + date.ToString("yyyyMMdd") + ":" + count + ":" + countPesos + ":" + amountPesos + ":" + countDollars + ":" + amountDollars + ":" + discountAmountPesos + ":" + discountAmountDollars;

                var created = CreateFile(raw, path);

                if (created)
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) se creo archivo ({1}). ", service.Name, fileName));
                }
                else
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no se pudo crear archivo ({1}). ", service.Name, fileName));
                }

                var result1 = _serviceNotificationMessage.SendExtract(EmailType.ExtractBanred, string.Format("VisaNetPagos - Reporte de pagos Banred (Extract) - {0} {1}", service.Name, date.ToString("dd/MM/yyyy")),
                            string.Format("Estas son las transacciones realizadas para la fecha {0}", date.ToString("dd/MM/yyyy")), service.ExtractEmail, path, fileName, "text/plain");
                LogResult(result1, service.Name, service.ExtractEmail);
            }

            catch (Exception exception)
            {
                NLogLogger.LogExtractEvent(exception);
            }
        }
        #endregion

        #region Importe
        private void NotifyImporteFile(Service service, DateTime date)
        {
            try
            {
                var serGateway = service.ServiceGateways.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnum.Importe);
                if (serGateway == null)
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene pasarela cargada. ", service.Name));
                    return;
                }

                var codcom = service.UrlName;
                var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Importe, service.Id, -1);

                if (payments == null || !payments.Any())
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene transacciones realizadas. ", service.Name));

                    var result = _serviceNotificationMessage.SendExtract(EmailType.ExtractImporte, string.Format("VisaNetPagos - Reporte de pagos - {0} {1}",service.Name, date.ToString("dd/MM/yyyy")),
                            string.Format("No se realizaron pagos para la fecha {0}", date.ToString("dd/MM/yyyy")), service.ExtractEmail);

                    LogResult(result, service.Name, service.ExtractEmail);
                    return;
                }

                var fileName = "EXTRACT " + codcom + "-" + date.ToString("yyyyMMdd") + ".txt";

                var path = Path.Combine(ConfigurationManager.AppSettings["FileShippingGeneratedFiles"], fileName);

                if (File.Exists(path))
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) ya tiene un archivo generado ({1}). ", service.Name,
                            fileName));
                    return;
                }

                var raw = "";
                var count = 0;
                var countPesos = 0;
                var countDollars = 0;
                double amountPesos = 0;
                double amountDollars = 0;
                double discountAmountPesos = 0;
                double discountAmountDollars = 0;

                foreach (var dto in payments)
                {
                    var bill = dto.Bills.FirstOrDefault();
                    var amount = bill != null ? (bill.Amount - bill.DiscountAmount) : 0;
                    var discountAmount = bill != null ? (bill.DiscountAmount) : 0;
                    raw = raw + codcom + ":";
                    raw = raw + dto.ReferenceNumber + ":";
                    raw = raw + (bill != null ? bill.BillExternalId : "") + ":";
                    raw = raw + amount.ToString().Replace(',', '.') + ":";
                    raw = raw + (bill != null ? bill.Currency.Equals("UYU") ? "N" : "D" : "") + ":";
                    raw = raw + dto.Date.ToString("yyyyMMdd") + ":";
                    raw = raw + "T" + ":"; //forma de pago (actualmente no aplica y va "T")
                    raw = raw + "0" + ":"; //cantidad de cuotas (actualmente no aplica y va "0")
                    raw = raw + discountAmount.ToString().Replace(',', '.');
                    raw = raw + Environment.NewLine;
                    count++;
                    if (bill.Currency.Equals("UYU"))
                    {
                        countPesos++;
                        amountPesos = amountPesos + amount;
                        discountAmountPesos = discountAmountPesos + discountAmount;
                    }
                    if (bill.Currency.Equals("USD"))
                    {
                        countDollars++;
                        amountDollars = amountDollars + amount;
                        discountAmountDollars = discountAmountDollars + discountAmount;
                    }
                }
                raw = raw + "TOT:" + date.ToString("yyyyMMdd") + ":" + count + ":" + countPesos + ":" + amountPesos + ":" + countDollars + ":" + amountDollars + ":" + discountAmountPesos + ":" + discountAmountDollars;

                var created = CreateFile(raw, path);

                if (created)
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) se creo archivo ({1}). ", service.Name, fileName));
                }
                else
                {
                    NLogLogger.LogExtractEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no se pudo crear archivo ({1}). ", service.Name, fileName));
                }

                var result1 = _serviceNotificationMessage.SendExtract(EmailType.ExtractImporte, string.Format("VisaNetPagos - Reporte de pagos - {0} {1}", service.Name, date.ToString("dd/MM/yyyy")),
                            string.Format("Estas son las transacciones realizadas para la fecha {0}", date.ToString("dd/MM/yyyy")), service.ExtractEmail, path, fileName, "text/plain");

                LogResult(result1, service.Name, service.ExtractEmail);
            }

            catch (Exception exception)
            {
                NLogLogger.LogExtractEvent(exception);
            }
        }
        #endregion

        private bool CreateFile(string raw, string fileName)
        {
            try
            {
                TextWriter tw = new StreamWriter(fileName, true);
                tw.Write(raw);
                tw.Close();
                return true;
            }
            catch (Exception e)
            {
                NLogLogger.LogExtractEvent(e);
            }
            return false;
        }

        private void LogResult(int result, string service, string email)
        {
            if (result == 2)
            {
                NLogLogger.LogExtractEvent(NLogType.Info, string.Format("        Se envio email al cliente {0} notificando informe de pago para el servicio {1}.", email, service));
            }
            if (result == 1)
            {
                NLogLogger.LogExtractEvent(NLogType.Info, string.Format("        Se envio email al cliente {0} notificando informe de pago para el servicio {1} PERO NO SE PERSISTIO EN BD.", email, service));
            }
            if (result == 0)
            {
                NLogLogger.LogExtractEvent(NLogType.Info, string.Format("        Ha ocurrido un error en la generación de email de informe de pago para el servicio {0} ", service));
                NotifyError("No se pudo enviar el mail al cliente", "Servicio " + service);
            }
        }


    }
}
