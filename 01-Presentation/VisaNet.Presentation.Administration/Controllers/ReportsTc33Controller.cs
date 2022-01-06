using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ComplexTypes;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Hubs;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;


namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsTc33Controller : BaseController
    {
        private readonly ITc33ClientService _tc33ClientService;
        private readonly IPaymentClientService _paymentClientService;
        private readonly IConciliationVisaNetTc33ClientService _conciliationVisaNetTc33ClientService;
        private readonly IEmailService _emailService;

        private static Regex digitsOnly = new Regex(@"[^\d]");

        public ReportsTc33Controller(ITc33ClientService tc33ClientService, IPaymentClientService paymentClientService, IConciliationVisaNetTc33ClientService conciliationVisaNetTc33ClientService, IEmailService emailService)
        {
            _tc33ClientService = tc33ClientService;
            _paymentClientService = paymentClientService;
            _conciliationVisaNetTc33ClientService = conciliationVisaNetTc33ClientService;
            _emailService = emailService;
        }

        [CustomAuthentication(Actions.ReportsTc33)]
        public ActionResult Index()
        {
            return View(new ReportsTc33FilterDto
            {
                CreationDateFrom = new DateTime(2014, 01, 01),
                CreationDateTo = DateTime.Today
            });
        }

        [CustomAuthentication(Actions.ReportsTc33)]
        public async Task<ActionResult> ProccessFile()
        {
            var path = "";
            var fileName = "";
            var size = 0;
            try
            {
                var httpPostedFileBase = Request.Files[0];
                if (httpPostedFileBase != null && httpPostedFileBase.ContentLength == 0)
                {
                    ShowNotification("Debe seleccionar un archivo a procesar", NotificationType.Error);
                }
                else
                {
                    foreach (string fileToProccess in Request.Files)
                    {
                        var input = Request.Files[fileToProccess];
                        if (input != null)
                        {
                            // Get length of file in bytes
                            size = input.ContentLength;
                            // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
                            size = size / 1024;
                            // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
                            size = size / 1024;

                            fileName = ShortenPath(input.FileName);
                            if (!String.IsNullOrEmpty(input.FileName))
                            {
                                path = Path.Combine(ConfigurationManager.AppSettings["Tc33Path"], fileName);
                                if (System.IO.File.Exists(path))
                                {
                                    //archivo ya existe en el directorio
                                    NLogLogger.LogTc33Event(NLogType.Info,
                                        "TC33 Error - Nombre de archivo repetido. Debe cambiar el nombre del archivo");
                                    ShowNotification("Nombre de archivo repetido. Debe cambiar el nombre del archivo",
                                        NotificationType.Error);
                                    return RedirectToAction("Index");
                                }
                                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Intento gaurdar archivo en path: " + path);
                                input.SaveAs(path);
                                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Archivo guardado.");
                            }
                        }
                    }

                    var result = await _tc33ClientService.CreateProcess(new Tc33Dto()
                    {
                        State = Tc33StateDto.Process,
                        InputFileName = fileName,
                    });

                    var user = (SystemUserDto)Session[SessionConstants.CURRENT_USER];
                    Task.Run(() => Proccessfile(result, user));
                    ShowNotification(
                        "Se procesará el archivo de forma async. Cuando finalice se actualizará el listado de archivos.",
                        NotificationType.Success);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(exception);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            return RedirectToAction("Index");
        }

        [CustomAuthentication(Actions.ReportsTc33)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerTc33(Request, param);

            var data = await _tc33ClientService.GetDataForTable(filter);
            var totalRecords = await _tc33ClientService.GetDataForTableCount(filter);

            ViewBag.Ok = !data.Any() ? true : data.FirstOrDefault().State != Tc33StateDto.OkConError;

            //var dataToShow = data.Skip(filter.DisplayStart);

            //if (filter.DisplayLength.HasValue)
            //    dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            var dataModel = data.Select(d => d.ToModel());
            //var dataModel = dataToShow.Select(d => d.ToModel());

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsTc33)]
        public async Task<ActionResult> DownloadInputFile(Guid id)
        {
            var tc33 = await _tc33ClientService.GetTC33(id);
            var path = Path.Combine(ConfigurationManager.AppSettings["Tc33Path"], tc33.InputFileName);
            var arrBytes = System.IO.File.ReadAllBytes(path);
            return File(arrBytes, "application/TXT", tc33.InputFileName);
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsTc33)]
        public async Task<ActionResult> DownloadOutputFile(Guid id)
        {
            var tc33 = await _tc33ClientService.GetTC33(id);
            var path = Path.Combine(ConfigurationManager.AppSettings["Tc33PathOut"], tc33.OutputFileName);
            var arrBytes = System.IO.File.ReadAllBytes(path);
            return File(arrBytes, "application/TXT", tc33.InputFileName);
        }

        [HttpGet]
        public async Task<ActionResult> TransactionModal(Guid id)
        {
            var trns = await _tc33ClientService.GetTC33Transactions(id);
            ViewBag.Title = "Transacciones";
            ViewBag.SubTitle = "Request id";
            return PartialView("_ModalTransacciones", trns);
        }

        [HttpGet]
        public async Task<ActionResult> ErrorsModal(Guid id)
        {
            var tc33 = await _tc33ClientService.GetTC33(id);
            ViewBag.Title = "Errores";
            ViewBag.SubTitle = "Mensajes";
            return PartialView("_ModalTransacciones", new List<String>() { tc33.Errors });
        }

        [HttpGet]
        public async Task<ActionResult> DetailModal(Guid id)
        {
            var tc33 = await _tc33ClientService.GetTC33(id);
            var result = new List<Tc33DetailTrns>();

            result.Add(new Tc33DetailTrns() { Label = "Cantidad transacciones en Pesos", Value = tc33.TransactionPesosCount.ToString() });
            result.Add(new Tc33DetailTrns() { Label = "Monto total en Pesos", Value = tc33.TransactionPesosAmount.ToString() });
            result.Add(new Tc33DetailTrns() { Label = "Cantidad transacciones en Dolares", Value = tc33.TransactionDollarCount.ToString() });
            result.Add(new Tc33DetailTrns() { Label = "Monto total en Dolares", Value = tc33.TransactionDollarAmount.ToString() });


            return PartialView("_DetailTransacciones", result);
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsTc33)]
        public async Task<ActionResult> DownloadDetails(Guid id)
        {
            var arrBytes = await _tc33ClientService.GenerateDetailsFile(id);
            var file = await _tc33ClientService.GetTC33(id);
            return File(arrBytes, "application/PDF", file.InputFileName.ToLower().Replace(".txt", ".pdf")); //chequar que nombre usar
        }


        //PASAMOS LOGICA A BO, APICORE PASA A AZURE
        private async Task<Tc33OutputDto> Proccessfile(Tc33Dto entityDto, SystemUserDto userSession)
        {
            var errors = new List<string>();
            var transactionInfo = new List<string>();
            var path = "";
            var currentLine = "";
            var fileName = "";
            Tc33Dto tc33Dto = null;
            var isPaymentTransaction = true;
            try
            {
                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Inicia proceso para lectura de archivo ");
                if (entityDto == null)
                {
                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Objeto inicial nullo ");
                    return null;
                }
                if (entityDto.Id == Guid.Empty)
                {
                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Objeto inicial sin ID");
                    return null;
                }

                tc33Dto = entityDto;

                if (string.IsNullOrEmpty(entityDto.InputFileName))
                {
                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Nombre de archivo nullo");
                    tc33Dto.State = Tc33StateDto.Error;
                    tc33Dto.Errors = tc33Dto.Errors + "No llego el nombre del archivo. <br />";
                    await _tc33ClientService.EditProcess(tc33Dto);
                    return null;
                }

                fileName = entityDto.InputFileName;
                path = Path.Combine(ConfigurationManager.AppSettings["Tc33Path"], fileName);
                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - PATH: " + path);
                var lines = System.IO.File.ReadAllLines(path);
                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Lines: " + lines != null ? lines.Count().ToString() : "");

                var tc33OutLines = new List<string>();
                var paymentsForConsolidation = new List<Tc33ProccesDto>();
                var tc33OutLinesToBeAdded = new List<string>();

                VisanetConciliationModel validationModel = null;

                //Valor definido en el trailer del documento
                decimal totalTransactionCount = 0;
                //Para ir contando las transacciones
                decimal totalTransactionCountCounter = 0;
                //Valor definido en el trailer del documento
                decimal totalTransactionAmmount = 0;
                //Para ir sumando el valor de las transacciones
                decimal totalTransactionAmmountSum = 0;
                //Para ir sumando el valor de las transacciones que debo restar si la transacción no se encuentra en el portal
                decimal totalTransactionAmmountToDelete = 0;
                //Para ir contando las transacciones que debo restar si la transacción no se encuentra en el portal
                decimal totalTransactionCountToDelete = 0;


                int transactionPesosCount = 0;
                int transactionDollarCount = 0;
                double transactionPesosAmount = 0;
                double transactionDollarAmount = 0;

                if (!lines[0].Contains("HEDR"))
                {
                    NLogLogger.LogTc33Event(NLogType.Error, "TC33 Error - Cabezal no encontrado");
                    errors.Add("Cabezal no encontrado");
                }
                else
                    tc33OutLines.Add(lines[0]);

                //Nombre único del archivo
                var captureFileNumber = lines[0].Substring(24, 8);

                if (lines[0].Substring(4, 6) != "491954")
                {
                    var msg = string.Format("El Destinatio nBIN ({0}) debería ser {1}. Línea {2}.", lines[0].Substring(4, 5), "491954", 1);
                    NLogLogger.LogTc33Event(NLogType.Error, "TC33 Error - " + msg);
                    errors.Add(msg);
                }

                var lineCounter = 1;
                currentLine = lines[lineCounter];
                var req06 = false;
                Tc33ProccesDto tc33Transaction = null;
                PaymentDto payment = null;
                while (!currentLine.Substring(16, 4).Contains("TRLR"))
                {
                    string currentLineCode = currentLine.Substring(0, 4);
                    if (currentLineCode == "3300")
                    {
                        #region 3000
                        //ya hice una pasada
                        if (currentLine.Contains("3300") && req06)
                        {
                            //FALTO LA LINEA REQ06 en el archivo
                            if (validationModel != null && validationModel.Valid)
                            {
                                //Tengo que agregar la linea. La transaccion la tengo en el portal
                                #region AGREGAR 3306

                                Add3306(ref tc33OutLinesToBeAdded, ref validationModel, ref req06, ref errors);
                                Add330BLine(ref tc33OutLinesToBeAdded, ref validationModel, ref errors);

                                #endregion
                            }
                        }
                        Add330BLine(ref tc33OutLinesToBeAdded, ref validationModel, ref errors);
                        CheckTrns(tc33OutLinesToBeAdded, ref tc33OutLines, validationModel, ref totalTransactionCountToDelete,
                            ref totalTransactionAmmountToDelete, ref transactionPesosCount, ref transactionDollarCount,
                            ref transactionPesosAmount, ref transactionDollarAmount, ref transactionInfo);
                        tc33OutLinesToBeAdded = new List<string>();
                        #region 3300
                        //Si estoy parado en el inicio de una transacción
                        //CP01 TCR 0 — TRANSACTION DATA
                        var transactionType = currentLine.Substring(149, 2);
                        if (currentLine.Substring(134, 3) != "858" && currentLine.Substring(134, 3) != "840")
                        {
                            var msg = string.Format("Se encontró una transacción con una con moneda que el sistema no opera, linea {0}", lineCounter + 1);
                            NLogLogger.LogTc33Event(NLogType.Error, "TC33 Error - " + msg);
                            errors.Add(msg);
                        }

                        tc33Transaction = new Tc33ProccesDto();
                        tc33Transaction.TransactionType = transactionType;
                        tc33Transaction.SourceAmount = currentLine.Substring(122, 12);
                        tc33Transaction.SourceCurrencyCode = currentLine.Substring(134, 3);

                        totalTransactionCountCounter++;

                        //Si es una compra
                        if (transactionType == "01")
                        {
                            //Si es compra suma en al ammount, las devoluciones tambien suman
                            totalTransactionAmmountSum += decimal.Parse(tc33Transaction.SourceAmount.Substring(0, tc33Transaction.SourceAmount.Length - 2)) + decimal.Parse(tc33Transaction.SourceAmount.Substring(tc33Transaction.SourceAmount.Length - 2, 2)) / 100;
                            isPaymentTransaction = true;

                            tc33Transaction.AuthorizationDate = currentLine.Substring(89, 4);
                        }
                        //Si es una devolución
                        else if (transactionType == "02" || transactionType == "05")
                        {
                            //Si es compra suma en al ammount, las devoluciones tambien suman
                            totalTransactionAmmountSum += decimal.Parse(tc33Transaction.SourceAmount.Substring(0, tc33Transaction.SourceAmount.Length - 2)) + decimal.Parse(tc33Transaction.SourceAmount.Substring(tc33Transaction.SourceAmount.Length - 2, 2)) / 100;
                            isPaymentTransaction = false;
                            //req06 = true;
                        }
                        #endregion
                        var nextLine = lines[lineCounter + 1];

                        #region 3301
                        if (nextLine.Substring(0, 4) != "3301")
                        {
                            errors.Add(string.Format("Documento mal formado, falta linea 3301 en la posición: {0}", lineCounter + 2));
                        }
                        else
                        {
                            tc33Transaction.RequestId = nextLine.Substring(102, 26);
                            string requestId = tc33Transaction.RequestId.Trim();
                            payment = await _paymentClientService.GetByTransactionNumber(requestId);

                            var logPaymentCyberSource = await _tc33ClientService.GetLogFromDb(requestId); // GetLogFromDb(requestId, 0);

                            validationModel = new VisanetConciliationModel(payment, logPaymentCyberSource, _paymentClientService);
                            validationModel.Tc33CurrencyCode = tc33Transaction.SourceCurrencyCode;
                            validationModel.Tc33TrnsType = tc33Transaction.TransactionType;

                            var currency = tc33Transaction.SourceCurrencyCode == "858" ? "UYU" : "USD";
                            var alreadyIn = await WasAlreadyProccessed(validationModel.TransactionNumber);

                            if (isPaymentTransaction)
                            {
                                //CP01 TCR 1 — ADDITIONAL DATA
                                tc33Transaction.AuthorizationCode = nextLine.Substring(8, 6);
                                tc33Transaction.CardAcceptorId = nextLine.Substring(16, 15);
                                tc33Transaction.TerminalId = nextLine.Substring(31, 8);
                                tc33Transaction.CommercePaymentIndicator = nextLine.Substring(39, 0);

                                validationModel.Tc33Amount =
                                    decimal.Parse(tc33Transaction.SourceAmount.Substring(0,
                                        tc33Transaction.SourceAmount.Length - 2)) +
                                    decimal.Parse(
                                        tc33Transaction.SourceAmount.Substring(tc33Transaction.SourceAmount.Length - 2,
                                            2)) / 100;

                                if (validationModel.IsPaymentDone && (validationModel.Bills == null || !validationModel.Bills.Any()))
                                {
                                    var msg =
                                        string.Format("La transación {0} no tiene una factura asociada. Línea {1}.",
                                            payment.TransactionNumber, nextLine);
                                    NLogLogger.LogTc33Event(NLogType.Error, "TC33 Error - " + msg);
                                    errors.Add(msg);
                                    validationModel.Valid = false;
                                }
                                else
                                {
                                    if (!validationModel.IsPaymentDone)
                                    {
                                        transactionInfo.Add(
                                            string.Format("No se encontró la transacción número: {0}, línea {1}",
                                                requestId, lineCounter + 2));
                                    }
                                    else if (!validationModel.Valid)
                                    {
                                        transactionInfo.Add(
                                            string.Format("No se encontró la transacción número: {0}, línea {1}",
                                                requestId, lineCounter + 2));
                                    }
                                    else if (alreadyIn)
                                    {
                                        transactionInfo.Add(
                                            string.Format("La transacción número: {0} ya fue procesada, línea {1}",
                                                validationModel.TransactionNumber, lineCounter + 2));
                                        //Pasa a ser no valido. Para quitar la transaccion del archivo
                                        validationModel.Valid = false;
                                    }
                                    else
                                    {
                                        var purchaseId = nextLine.Substring(44, 25);
                                        if (validationModel.TransactionNumber.Trim() != purchaseId.Trim())
                                        {
                                            errors.Add(
                                                string.Format(
                                                    "No coincide el Purchase Id: {0}, con el request id de la autorización: {1}. Línea {2}",
                                                    purchaseId, validationModel.TransactionNumber, lineCounter + 2));
                                        }

                                        if (ParseCyberSourceAuthTime(validationModel.CyberSourceData.AuthTime) !=
                                            EstimateDate(tc33Transaction.AuthorizationDate))
                                        {
                                            //por salto de hora, entendemos que puede ser igual la hora de pago en portal con la autorizacion de CS
                                            if (validationModel.TransactionDate == null)
                                            {
                                                errors.Add(
                                                    string.Format(
                                                        "No coincide la fecha de autorización de CyberSource: {0}, con la encontrada en la tra transacción ({1}). Línea {2}",
                                                        ParseCyberSourceAuthTime(
                                                            validationModel.CyberSourceData.AuthTime),
                                                        EstimateDate(tc33Transaction.AuthorizationDate), lineCounter + 1));
                                            }
                                            var paymentDate = new DateTime(validationModel.TransactionDate.Year,
                                                validationModel.TransactionDate.Month, validationModel.TransactionDate.Day);
                                            if (paymentDate != EstimateDate(tc33Transaction.AuthorizationDate))
                                            {
                                                errors.Add(
                                                    string.Format(
                                                        "No coincide la fecha de autorización de CyberSource: {0}, con la encontrada en la tra transacción ({1}). Línea {2}",
                                                        ParseCyberSourceAuthTime(
                                                            validationModel.CyberSourceData.AuthTime),
                                                        EstimateDate(tc33Transaction.AuthorizationDate), lineCounter + 1));
                                            }
                                        }


                                        if (currency != validationModel.Currency)
                                        {
                                            errors.Add(
                                                string.Format(
                                                    "La moneda de la transacción ({0}) no coincide con la que se encuentra en el sistema ({1}). Línea {2}.",
                                                    currency, validationModel.Currency, lineCounter + 1));
                                        }

                                        var ammount = double.Parse(tc33Transaction.SourceAmount.Substring(0, 10)) +
                                                      double.Parse(tc33Transaction.SourceAmount.Substring(10, 2)) / 100;

                                        if (Math.Abs(ammount - validationModel.AmountTocybersource) > 0.001)
                                        {
                                            errors.Add(
                                                string.Format(
                                                    "El monto de la transacción ({0}) no coincide con la que se encuentra en el sistema ({1}). Línea {2}.",
                                                    ammount.ToString("G"), validationModel.AmountTocybersource,
                                                    lineCounter + 1));
                                        }

                                        if (tc33Transaction.AuthorizationCode !=
                                            validationModel.CyberSourceData.AuthCode)
                                        {
                                            errors.Add(
                                                string.Format(
                                                    "El código de autorización ({0}) no coincide con la que se encuentra en el sistema ({1}). Línea {2}.",
                                                    tc33Transaction.AuthorizationCode,
                                                    validationModel.CyberSourceData.AuthCode, lineCounter + 2));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //ESTO ES PARA UN REFUND
                                //OBTENGO LAS DOS TRANSACCIONES Y COMPARO MONTOS
                                if (alreadyIn)
                                {
                                    transactionInfo.Add(
                                        string.Format("La transacción número: {0} ya fue procesada, línea {1}",
                                            validationModel.TransactionNumber, lineCounter + 2));
                                    //Pasa a ser no valido. Para quitar la transaccion del archivo
                                    validationModel.Valid = false;
                                }
                                else
                                {
                                    req06 = true;
                                    var refundTrnsNumber = validationModel.TransactionNumber;
                                    var originalTc33TrnsNumber = nextLine.Substring(44, 22);

                                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - REFUND ENCONTRADO. TRNS: " + refundTrnsNumber);

                                    var originalPayment = await _paymentClientService.GetByTransactionNumber(originalTc33TrnsNumber);
                                    //_servicePayment.AllNoTracking(x => x.TransactionNumber == originalTc33TrnsNumber, x => x.Bills).FirstOrDefault();

                                    if (originalPayment == null)
                                    {
                                        transactionInfo.Add(
                                            string.Format(
                                            "No se encontro registro en el sistema de la transacción {0} sobre la cual se realizo el refund {1} en Cybersource. Se quitara este bloque del archivo resultante. Línea {2}.",
                                            originalTc33TrnsNumber, refundTrnsNumber, lineCounter + 1));
                                        //errors.Add(string.Format(
                                        //    "No se encontro registro en el sistema de la transacción {0} sobre la cual se realizo el refund {1} en Cybersource. Se quitara este bloque del archivo resultante. Línea {2}.",
                                        //    originalTc33TrnsNumber, refundTrnsNumber, lineCounter + 1));
                                        validationModel.Valid = false;
                                    }
                                    else
                                    {
                                        if (originalPayment.PaymentStatus != PaymentStatusDto.Refunded)
                                        {
                                            errors.Add(string.Format(
                                            "La transacción {0} sobre la cual se realizo el refund {1} esta con estado {2}. No se puede realizar un refund sobre esta transaccion. Línea {3}.",
                                                        originalTc33TrnsNumber, refundTrnsNumber, originalPayment.PaymentStatus.ToString(), lineCounter + 1));
                                        }
                                        if (Math.Abs(originalPayment.AmountTocybersource.SignificantDigits(2) - validationModel.TotalAmount.SignificantDigits(2)) > 0.001)
                                        {
                                            errors.Add(string.Format(
                                            "El monto {0} de la transacción {1} y el monto {2} del refund {3} no coinciden. Línea {4}.",
                                                        originalPayment.AmountTocybersource, originalTc33TrnsNumber, validationModel.TotalAmount, refundTrnsNumber, lineCounter + 1));
                                        }
                                        if (!originalPayment.Currency.Equals(currency))
                                        {
                                            errors.Add(string.Format(
                                            "La moneda {0} de la transacción {1} y la moneda {2} del refund {3} no coinciden. Línea {4}.",
                                                        originalPayment.Currency, originalTc33TrnsNumber, currency, refundTrnsNumber, lineCounter + 1));
                                        }
                                        if (originalPayment.Id != validationModel.OriginalPayment.Id)
                                        {
                                            errors.Add(string.Format(
                                            "El nro de transaccion original {0} del archivo tc33 no concuerda con los registros de log (nro transaccion {1}). Línea {2}.",
                                                        originalTc33TrnsNumber, originalPayment.TransactionNumber, lineCounter + 1));
                                        }
                                    }
                                    tc33Transaction.TransactionDate = validationModel.TransactionDate;
                                    paymentsForConsolidation.Add(tc33Transaction);
                                }
                            }

                            //MODIFICAR LA POSICION 24 CON EL NRO DE SUCURSAL
                            Modify3301(ref nextLine, payment);
                        }
                        #endregion

                        nextLine = lines[lineCounter + 4];
                        #region 3304

                        if (nextLine.Substring(0, 4) != "3304")
                        {
                            errors.Add(string.Format("Documento mal formado, falta linea 3304 en la posición: {0}", lineCounter + 2));
                        }
                        #endregion

                        tc33OutLinesToBeAdded.Add(currentLine);
                        #endregion
                    }
                    else if (currentLineCode == "3301")
                    {
                        Modify3301(ref currentLine, payment);
                        tc33OutLinesToBeAdded.Add(currentLine);
                    }
                    else if (currentLineCode == "3302" || currentLineCode == "3303" || currentLineCode == "3304" || currentLineCode == "3305")
                    {
                        tc33OutLinesToBeAdded.Add(currentLine);
                    }
                    else if (currentLineCode == "3306")
                    {
                        #region 3306
                        //CP01 TCR 6 — INSTALLMENT PAYMENT
                        //ESTO SOLO HAY QUE HACERLO DEPENDIENDO DEL TIPO DE TRANSACCIÓN
                        if (validationModel.Valid)
                        {
                            /* Si numérico completo con ceros a la izquierda, sí alfanumérico completo con espacios
                                * 0-Descuento N1 (Numérico de largo 1)
                                * 1-Serie del comprobante A1 (Alfanumérico de largo 1)
                                * 2-Número factura N7
                                * 3-Importe total de la factura N12,2 (Numérico de largo 12 con dos decimales)
                                * 4-Importe gravado N12,2
                                * 5-Importe devolución N12,2
                                * 6-Porcentaje del beneficio N4,2 
                                */
                            //la factura solo puede ser de 7 caracteres.
                            var bill = validationModel.Bills.First().BillExternalId.PadLeft(7, '0');
                            if (bill.Count() > 7)
                            {
                                bill = bill.Substring(bill.Count() - 7, 7);
                            }

                            //var line0 = validationModel.DiscountApplyed ? "6" : "0";
                            var line0 = validationModel.DiscountApplyed ? ((int)validationModel.DiscountType).ToString() : "0"; //nuevo
                            var line1 = "A";
                            var line2 = CleanString(bill);
                            var line3 = validationModel.TotalAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                            var line4 = validationModel.TotalTaxedAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                            var line5 = validationModel.DiscountApplyed ? validationModel.Discount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                            var line6 = validationModel.DiscountApplyed ? (validationModel.DiscountPer).ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0');
                            var newTc06 = (currentLine.Substring(0, 83) + string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                line0,
                                line1,
                                line2,
                                line3,
                                line4,
                                line5,
                                line6
                                )).PadRight(167, ' ') + "0";
                            //validationModel.Discount * 100 / validationModel.TotalTaxedAmount
                            currentLine = newTc06;
                            paymentsForConsolidation.Add(tc33Transaction);
                        }
                        tc33OutLinesToBeAdded.Add(currentLine);
                        req06 = false;
                        #endregion
                    }
                    else if (currentLineCode == "3307" || currentLineCode == "3308" || currentLineCode == "3309" || currentLineCode == "330A" || currentLineCode.Contains("9100"))
                    {
                        #region 3307-3308-3309-330A-9100
                        //if (currentLineCode.Contains("9100"))
                        //{
                        //    //SI ENTRO ACA ES UNA LINEA INTERMEDIA NO EL FINAL DEL ARCHIVO
                        //    Add330BLine(ref tc33OutLinesToBeAdded, ref validationModel, ref errors);    
                        //}
                        //tc33OutLinesToBeAdded.Add(currentLine);
                        #endregion
                    }
                    //else
                    //{
                    //    tc33OutLinesToBeAdded.Add(currentLine);
                    //}

                    lineCounter++;
                    currentLine = lines[lineCounter];
                }

                //llegue al final de las transacciones. Tengo que ver si a la ultima le falta 3306
                if (req06)
                {
                    //FALTO LA LINEA REQ06 en el archivo
                    if (validationModel != null && validationModel.Valid)
                    {
                        //Tengo que agregar la linea. La transaccion la tengo en el portal
                        #region AGREGAR 3306
                        Add3306(ref tc33OutLinesToBeAdded, ref validationModel, ref req06, ref errors);
                        #endregion
                    }
                }

                Add330BLine(ref tc33OutLinesToBeAdded, ref validationModel, ref errors);
                //dato de la ultima pasada
                CheckTrns(tc33OutLinesToBeAdded, ref tc33OutLines, validationModel, ref totalTransactionCountToDelete,
                            ref totalTransactionAmmountToDelete, ref transactionPesosCount, ref transactionDollarCount,
                            ref transactionPesosAmount, ref transactionDollarAmount, ref transactionInfo);

                if (currentLine.Contains("TRLR"))
                {
                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - TRLR ENCONTRADO. LINEA : " + lineCounter);
                    totalTransactionCount = int.Parse(currentLine.Substring(32, 9));
                    var totalAmountString = currentLine.Substring(41, 20);
                    //Es un numero de 18 dígitos y dos decimales
                    totalTransactionAmmount = int.Parse(totalAmountString.Substring(0, 18)) + decimal.Parse(totalAmountString.Substring(totalAmountString.Length - 2, 2)) / 100;

                    string trlrNBin = currentLine.Substring(4, 6);

                    if (trlrNBin != "491954")
                    {
                        errors.Add(string.Format("El Destinatio nBIN ({0}) debería ser {1}. Línea {2}.", trlrNBin, "491954", lineCounter + 1));
                    }
                    if (totalTransactionAmmount != totalTransactionAmmountSum)
                    {
                        errors.Add(string.Format("No coinciden los montos de las transacciones con el total definido en el documento, calculado: {0} y en documento: {1}", totalTransactionAmmountSum, totalTransactionAmmount));
                    }
                    if (totalTransactionCount != totalTransactionCountCounter)
                    {
                        errors.Add(string.Format("No coinciden las cantidades transacciones en el documento con el total definido en el documento, calculado: " +
                                                 "{0} y en documento: {1}", totalTransactionCountCounter, totalTransactionCount));
                    }

                    var lineToEdit = currentLine;
                    if (totalTransactionCountToDelete > 0)
                    {
                        lineToEdit = lineToEdit.Remove(32, 9);
                        var paymentsCountString = totalTransactionCount - totalTransactionCountToDelete + "";
                        paymentsCountString = paymentsCountString.PadLeft(9, '0');
                        lineToEdit = lineToEdit.Insert(32, paymentsCountString);
                    }
                    if (totalTransactionAmmountToDelete > 0)
                    {
                        lineToEdit = lineToEdit.Remove(41, 20);
                        var newAmount = totalTransactionAmmount - totalTransactionAmmountToDelete;
                        var newAmountString = (newAmount * 100).ToString("0");
                        lineToEdit = lineToEdit.Insert(41, newAmountString.PadLeft(20, '0'));
                    }
                    tc33OutLines.Add(lineToEdit);
                    lineCounter++;
                }
                while (lineCounter < lines.Count())
                {
                    currentLine = lines[lineCounter];
                    if (currentLine.StartsWith("9200"))
                    {

                    }
                    tc33OutLines.Add(currentLine);
                    lineCounter++;
                }

                var outFileName = fileName.Split('.').GetValue(0) + "_" + DateTime.Now.ToString("HHmmss") + ".txt";

                tc33Dto = new Tc33Dto
                {
                    Transactions = new List<Tc33TransactionDto>(),
                    InputFileName = fileName,
                    TransactionDollarAmount = transactionDollarAmount,
                    TransactionDollarCount = transactionDollarCount,
                    TransactionPesosAmount = transactionPesosAmount,
                    TransactionPesosCount = transactionPesosCount
                };

                if (errors.Any())
                {
                    tc33Dto.State = Tc33StateDto.Error;
                    tc33Dto.Errors = String.Join("<br />", errors);
                }
                else
                {
                    tc33Dto.State = transactionInfo.Any() ? Tc33StateDto.OkConError : Tc33StateDto.Ok;
                    tc33Dto.Errors = String.Join("<br />", transactionInfo);
                    tc33Dto.OutputFileName = outFileName;
                }

                System.IO.File.WriteAllLines(ConfigurationManager.AppSettings["Tc33PathOut"] + "\\" + outFileName, tc33OutLines);

                //CREA REGISTROS PARA CONCILIACION
                foreach (var tc33ProccesDto in paymentsForConsolidation)
                {
                    try
                    {
                        var visanetConciliation = new ConciliationVisanetDto
                        {
                            Amount = double.Parse(tc33ProccesDto.SourceAmount.Substring(0, 10)) + double.Parse(tc33ProccesDto.SourceAmount.Substring(10, 2)) / 100,
                            Currency = tc33ProccesDto.SourceCurrencyCode == "858" ? "UYU" : "USD", //858UYU /840 USD
                            Date = tc33ProccesDto.TransactionDate.HasValue ? tc33ProccesDto.TransactionDate.Value : EstimateDate(tc33ProccesDto.AuthorizationDate),
                            RequestId = tc33ProccesDto.RequestId.Trim(),
                            ConciliationType = tc33ProccesDto.TransactionType == "01" ? ConciliationTypeDto.Complete : ConciliationTypeDto.CybersourceOnly

                        };
                        //chequeo que no exista ya

                        var wasAlreadyProccessed = await WasAlreadyProccessed(visanetConciliation.RequestId);
                        if (!wasAlreadyProccessed)
                        {
                            await _conciliationVisaNetTc33ClientService.Create(visanetConciliation);
                            NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Se crea valor en tabla de conciliación visanet. Request id : " + visanetConciliation.RequestId);
                        }
                        else
                        {
                            await _conciliationVisaNetTc33ClientService.Edit(visanetConciliation);
                            NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Registro en tabla de conciliación visanet repetido. Request id : " + visanetConciliation.RequestId);
                        }

                        tc33Dto.Transactions.Add(new Tc33TransactionDto
                        {
                            RequestId = tc33ProccesDto.RequestId
                        });
                    }
                    catch (Exception exception)
                    {
                        var msg = "    Error en procesamiento de registros para concilicacion. Trns : " +
                                  tc33ProccesDto.RequestId;
                        NLogLogger.LogTc33Event(NLogType.Info, msg);
                        NLogLogger.LogTc33Event(exception);
                        errors.Add(msg);
                    }
                }

                //Primer elimino si tengo uno en procesando con el mismo inputname. Luego inserto
                NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Termina proceso de lecutra de archivo");

                if (errors != null && errors.Any())
                {
                    tc33Dto.Transactions.Clear();
                    NLogLogger.LogTc33Event(NLogType.Info, "TC33 - Errores: " + errors.Count);
                    foreach (var error in errors)
                    {
                        NLogLogger.LogTc33Event(NLogType.Info, "    Detalle Error: " + error);
                    }
                }

                _tc33ClientService.CreateProcess(tc33Dto);

                GenerateNotification(userSession, string.Format("Se ha procesado el archivo {0}. Chequear el estado del mismo en VisaNetPagosAdmin.", fileName), NotificationType.Success);

                return new Tc33OutputDto { Errors = errors, OutputFileName = tc33Dto.OutputFileName };
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(NLogType.Error, "TC33 Exception en linea " + currentLine);
                NLogLogger.LogTc33Event(exception);
                errors.Add(exception.Message);

                tc33Dto.State = Tc33StateDto.Error;
                tc33Dto.Errors = tc33Dto.Errors + "Ocurrio una excepcion en la linea " + currentLine + ". <br />";
                if (tc33Dto.Id != Guid.Empty)
                {
                    _tc33ClientService.CreateProcess(tc33Dto);
                }
                else
                {
                    _tc33ClientService.CreateProcess(tc33Dto);
                }
                GenerateNotification(userSession, "Error al generar archivo", NotificationType.Error, exception);
                return null;
            }
        }

        private async Task<bool> WasAlreadyProccessed(string requestId)
        {
            try
            {
                if (!string.IsNullOrEmpty(requestId))
                {
                    return await _tc33ClientService.WasAlreadyProccessed(requestId);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(NLogType.Info, "wasAlreadyProccessed error");
                NLogLogger.LogTc33Event(exception);
                throw;
            }
            return false;
        }

        private void Add3306(ref List<string> tc33OutLinesToBeAdded, ref VisanetConciliationModel validationModel, ref bool req06, ref List<String> errors)
        {
            NLogLogger.LogTc33Event(NLogType.Info, "TC33 - AGREGO 3306 FALTANTE. CS: " + validationModel.TransactionNumber);
            var i = tc33OutLinesToBeAdded.Count - 1;
            try
            {
                //BUSCO LA 3300
                while (!tc33OutLinesToBeAdded[i].StartsWith("3300")) i--;

                if (!validationModel.IsRefund)
                {
                    //ACTUALIZO 3300
                    tc33OutLinesToBeAdded[i] = tc33OutLinesToBeAdded[i].Substring(0, 90) +
                                    ParseCyberSourceAuthTime(validationModel.CyberSourceData.AuthTime).ToString("MMdd") +
                                    tc33OutLinesToBeAdded[i].Substring(93, 3) +
                                    validationModel.CyberSourceData.AuthAmount.Replace(".", "").Replace(",", "").PadLeft(12, '0') +
                                    (validationModel.CyberSourceData.ReqCurrency == "UYU" ? "858" : "840") +
                                    validationModel.TotalTaxedAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0') +
                                    tc33OutLinesToBeAdded[i].Substring(123, 45);
                }
                //BUSCO LA 3301
                i = tc33OutLinesToBeAdded.Count - 1;
                while (!tc33OutLinesToBeAdded[i].StartsWith("3301")) i--;

                if (!validationModel.IsRefund)
                {
                    //ACTUALIZO 3301
                    tc33OutLinesToBeAdded[i] = tc33OutLinesToBeAdded[i].Substring(0, 9) + validationModel.CyberSourceData.AuthCode + tc33OutLinesToBeAdded[i].Substring(15, 153);

                }

                if (validationModel.IsPaymentDone)
                {
                    //Genero la nueva 3306
                    //la factura solo puede ser de 7 caracteres.
                    var bill = validationModel.Bills.First().BillExternalId.PadLeft(7, '0');
                    if (bill.Count() > 7)
                    {
                        bill = bill.Substring(bill.Count() - 7, 7);
                    }

                    //Chequeo que el monto total dividido (1+iva mínimo) sea mayor que el monto gravado
                    //Si no es así, se remplaza el monto gravado por monto total divido (1+iva mínimo)
                    if (validationModel.TotalAmount / (1 + 0.1) < validationModel.TotalTaxedAmount)
                        validationModel.TotalTaxedAmount = validationModel.TotalAmount / (1 + (0.1));

                    var newTc06 = ("3306".PadRight(83, ' ') + string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                        //validationModel.DiscountApplyed ? "6" : "0",
                                        ((int)validationModel.DiscountType).ToString(), //nuevo
                                        "A",
                                        CleanString(bill),
                                        validationModel.TotalAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                                        validationModel.TotalTaxedAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                                        validationModel.DiscountApplyed ? validationModel.Discount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                                        validationModel.DiscountApplyed ? (validationModel.DiscountPer).ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0')
                                        )).PadRight(167, ' ') + "0";
                    tc33OutLinesToBeAdded.Add(newTc06);
                }
                if (validationModel.IsRefund)
                {
                    var discountPer = validationModel.OriginalPayment.Bills.FirstOrDefault().Discount;

                    //Genero la nueva 3306
                    //la factura solo puede ser de 7 caracteres.
                    var bill = validationModel.OriginalPayment.Bills.First().BillExternalId.PadLeft(7, '0');
                    if (bill.Count() > 7)
                    {
                        bill = bill.Substring(bill.Count() - 7, 7);
                    }

                    //Chequeo que el monto total dividido (1+iva mínimo) sea mayor que el monto gravado
                    //Si no es así, se remplaza el monto gravado por monto total divido (1+iva mínimo)
                    if (validationModel.TotalAmount / (1 + 0.1) < validationModel.OriginalPayment.TotalTaxedAmount)
                        validationModel.OriginalPayment.TotalTaxedAmount = validationModel.TotalAmount / (1 + (0.1));

                    var newTc06 = ("3306".PadRight(83, ' ') + string.Format("{0}{1}{2}{3}{4}{5}{6}",
                        //validationModel.DiscountApplyed ? "6" : "0",
                        ((int)validationModel.DiscountType).ToString(), //nuevo
                        "A",
                        CleanString(bill),
                        //EL MONTO DE LA TRANSACCION TIENE QUE SER IGUAL AL MONTO DEL REFUND
                        validationModel.OriginalPayment.TotalAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                        validationModel.OriginalPayment.TotalTaxedAmount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                        validationModel.OriginalPayment.DiscountApplyed ? validationModel.OriginalPayment.Discount.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0'),
                        validationModel.OriginalPayment.DiscountApplyed ? (discountPer).ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0') : 0.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(4, '0')
                        )).PadRight(167, ' ') + "0";
                    tc33OutLinesToBeAdded.Add(newTc06);
                }
            }
            catch (Exception e)
            {
                errors.Add("No se pudo agregar la linea 3006 en transaccion " + validationModel.TransactionNumber);
                errors.Add(e.Message);
                NLogLogger.LogTc33Event(e);
            }

            req06 = false;
        }
        private void Add330BLine(ref List<string> tc33OutLinesToBeAdded, ref VisanetConciliationModel validationModel, ref List<String> errors)
        {
            try
            {
                var newLine = string.Empty;
                if (validationModel != null && tc33OutLinesToBeAdded != null && validationModel.Valid)
                {
                    var alreadyIn = tc33OutLinesToBeAdded.Any(x => x.StartsWith("330B"));
                    if (!alreadyIn)
                    {
                        //la factura solo puede ser de 7 caracteres.
                        var billDto = validationModel.IsRefund ? validationModel.OriginalPayment.Bills.First() : validationModel.Bills.First();

                        var bill = billDto.BillExternalId.PadLeft(7, '0');
                        if (bill.Count() > 7)
                        {
                            bill = bill.Substring(bill.Count() - 7, 7);
                        }

                        var billAmount = validationModel.IsRefund
                            ? validationModel.OriginalPayment.Discount * 100
                            : validationModel.Discount * 100;

                        var quota = "01";
                        if (validationModel.Quota > 0 && validationModel.Quota < 10)
                        {
                            quota = validationModel.Quota.ToString("####").PadLeft(2, '0');
                        }
                        if (validationModel.Quota > 9)
                        {
                            quota = validationModel.Quota.ToString();
                        }

                        newLine = ("330B" + string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
                                    "854", // FILLER (Fijo 854)
                                    "00", // PLAN
                                    "0", //MESES DIFERIDOS
                                    quota, //CANTIDAD CUOTAS
                                    "0", //INDICADOR ENVIO PISTA
                                    "         ", //CEDULA IDENTIDAD
                                    ((int)validationModel.DiscountType).ToString(), //INDI
                                    billAmount.ToString("####").PadLeft(12, '0'), // IMPORTE DEVOLUCION IVA 000000001613 => 12
                                    CleanString(bill) // NUMERO FACTURA
                                    )).PadRight(168, ' ');

                        tc33OutLinesToBeAdded.Add(newLine);
                    }
                }
            }
            catch (Exception e)
            {
                var str = string.Format("No se pudo agregar la linea 330B en transaccion {0}", (validationModel != null ? validationModel.TransactionNumber : "NULL"));
                if (errors != null)
                {
                    errors.Add(str);
                    errors.Add(e.Message);
                }
                NLogLogger.LogTc33Event(NLogType.Info, str);
                NLogLogger.LogTc33Event(e);
            }
        }
        private void Modify3301(ref string line, PaymentDto payment)
        {
            //TODO AHORA SOLO UTILIZO EL DEFAULT. NOS VAN A PEDIR SACAR ESTO DEL SERVICIO
            string currentLineCode = line.Substring(0, 4);
            if (currentLineCode == "3301")
            {
                var branch = string.Empty;

                if (payment != null && payment.GatewayDto != null && payment.GatewayDto.Enum == (int)GatewayEnumDto.Apps)
                {
                    var serGateway = payment.ServiceDto.ServiceGatewaysDto.FirstOrDefault(x => x.GatewayId == payment.GatewayId);
                    if (serGateway != null)
                    {
                        branch = serGateway.ServiceType;
                    }
                }
                if (!string.IsNullOrEmpty(branch))
                {
                    line = line.Remove(24, 3).Insert(24, branch);
                }
                else
                {
                    var defaultSucursalValue = ConfigurationManager.AppSettings["DefaultSucursal"];
                    if (!string.IsNullOrEmpty(defaultSucursalValue))
                    {
                        line = line.Remove(24, 3).Insert(24, defaultSucursalValue);
                    }
                }
            }
        }

        private void CheckTrns(List<string> tc33OutLinesToBeAdded, ref List<string> tc33OutLines, VisanetConciliationModel validationModel,
            ref decimal totalTransactionCountToDelete, ref decimal totalTransactionAmmountToDelete, ref int transactionPesosCount,
            ref int transactionDollarCount, ref double transactionPesosAmount, ref double transactionDollarAmount, ref List<string> transactionInfo)
        {

            //Chequeo transacciones repetidas. Si ya fue procesada en archivos anteriores hay que sacarla
            if (tc33OutLinesToBeAdded.Count > 0)
            {
                if (validationModel != null && validationModel.Valid && (validationModel.IsPaymentDone || validationModel.IsRefund))
                {
                    tc33OutLines.AddRange(tc33OutLinesToBeAdded);
                    if (validationModel.Tc33CurrencyCode.Equals("858"))
                    {
                        transactionPesosCount++;
                        transactionPesosAmount = transactionPesosAmount + (validationModel.Tc33TrnsType.Equals("01") ? (double)validationModel.Tc33Amount : -(double)validationModel.Tc33Amount);
                    }
                    if (validationModel.Tc33CurrencyCode.Equals("840"))
                    {
                        transactionDollarCount++;
                        transactionDollarAmount = transactionDollarAmount + (validationModel.Tc33TrnsType.Equals("01") ? (double)validationModel.Tc33Amount : -(double)validationModel.Tc33Amount);
                    }
                }
                else
                {
                    //no existe la transaccion en el portal. No la agrego al output y guardo datos a modificar en los totales
                    totalTransactionCountToDelete++;
                    var amount = validationModel.IsRefund ? (decimal)validationModel.AmountTocybersource : validationModel.Tc33Amount;
                    totalTransactionAmmountToDelete = totalTransactionAmmountToDelete + amount;
                }
            }
        }

        private DateTime ParseCyberSourceAuthTime(string authTime)
        {
            return DateTime.ParseExact(authTime.Substring(0, 10), "yyyy-MM-dd", null);
        }
        private static string CleanString(string str)
        {
            return digitsOnly.Replace(str, "0");
        }
        private DateTime EstimateDate(string dateString)
        {
            var month = int.Parse(dateString.Substring(0, 2));
            var day = int.Parse(dateString.Substring(2, 2));

            if (month > DateTime.Now.Month)
            {
                return new DateTime(DateTime.Now.Year - 1, month, day);
            }

            if (month == DateTime.Now.Month)
            {
                return day > DateTime.Now.Day ? new DateTime(DateTime.Now.Year - 1, month, day) : new DateTime(DateTime.Now.Year, month, day);
            }

            return new DateTime(DateTime.Now.Year, month, day);
        }

        private void GenerateNotification(SystemUserDto userSession, string message, NotificationType type, Exception e = null)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(userSession.Id.ToString()).notify(message, type.ToString());

            var dto = new Tc33SyncNotificationDto
            {
                UserName = userSession.LDAPUserName,
                Type = type,
                ExpectionError = e,
                Message = message
            };

            _emailService.SendNotificationTc33Synchronization(dto);
        }

        public class VisanetConciliationModel
        {
            public string TransactionNumber { get; private set; }
            public CyberSourceData CyberSourceData { get; private set; }
            public string Currency { get; private set; }
            public double AmountTocybersource { get; private set; }
            public bool DiscountApplyed { get; private set; }
            public IList<BillDto> Bills { get; private set; }
            public double TotalAmount { get; private set; }
            public double TotalTaxedAmount { get; set; }
            public double Discount { get; private set; }
            public int DiscountPer { get; set; }
            public bool Valid { get; set; }
            public bool IsPaymentDone { get; set; }
            public DiscountLabelTypeDto DiscountType { get; set; } //nuevo
            public DateTime TransactionDate { get; set; }

            public decimal Tc33Amount { get; set; }
            public string Tc33CurrencyCode { get; set; }
            public string Tc33TrnsType { get; set; }

            public bool IsRefund { get; set; }
            public PaymentDto OriginalPayment { get; set; }

            public int Quota { get; set; }

            public VisanetConciliationModel(PaymentDto payment, LogDto logPaymentCyberSource, IPaymentClientService _paymentClientService)
            {
                Valid = payment != null || logPaymentCyberSource != null;
                IsPaymentDone = payment != null;

                if (Valid)
                {
                    if (payment != null)
                    {
                        var reqAmount = payment.AmountTocybersource > 0 ? payment.AmountTocybersource.ToString() : payment.CyberSourceData.ReqAmount.Replace('.', ',');
                        TransactionNumber = payment.TransactionNumber;
                        //CyberSourceData = payment.CyberSourceData;
                        CyberSourceData = new CyberSourceData
                        {
                            AuthTime = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthTime,
                            AuthCode = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthCode,
                            AuthAmount = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthAmount,
                            ReqCurrency = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqCurrency
                        };
                        Currency = payment.Currency;
                        AmountTocybersource = payment.AmountTocybersource > 0 ? payment.AmountTocybersource.SignificantDigits(2) : double.Parse(reqAmount).SignificantDigits(2);
                        DiscountApplyed = payment.DiscountApplyed;
                        Bills = payment.Bills.ToList();
                        TotalAmount = payment.TotalAmount;
                        TotalTaxedAmount = payment.TotalTaxedAmount;
                        Discount = payment.Discount;
                        DiscountPer = payment.Bills != null && payment.Bills.Any() ? payment.Bills.First().Discount : 0;
                        TransactionDate = payment.Date;
                        Quota = payment.Quotas;

                        //nuevo
                        if (payment.Date.CompareTo(new DateTime(2015, 12, 23, 09, 45, 48)) < 0)
                        {
                            //esto se hace para los pagos viejos donde no existia el campo DiscountObj
                            DiscountType = payment.DiscountApplyed ? DiscountLabelTypeDto.FinancialInclusion : DiscountLabelTypeDto.NoDiscount;
                        }
                        else
                        {
                            DiscountType = payment.DiscountApplyed ? payment.DiscountObj != null ? (DiscountLabelTypeDto)(int)payment.DiscountObj.DiscountLabel : DiscountLabelTypeDto.NoDiscount : DiscountLabelTypeDto.NoDiscount;

                            //TODO QUITAR ESTO CUANDO VISA NOS PIDA. SE HARCODEA POR PEDIDO DE ELLOS 20160401
                            if (DiscountType == DiscountLabelTypeDto.TaxReintegration)
                            {
                                DiscountType = DiscountLabelTypeDto.TourismActivity;
                            }
                        }
                    }
                    else
                    {
                        IsRefund = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.TransactionType == TransactionType.Refund;
                        TransactionNumber = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.TransactionId;
                        CyberSourceData = new CyberSourceData
                        {
                            AuthTime = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthTime,
                            AuthCode = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthCode,
                            AuthAmount = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.AuthAmount,
                            ReqCurrency = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqCurrency,
                            BillTransRefNo = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.BillTransRefNo,
                        };
                        Currency = logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqCurrency;
                        AmountTocybersource = string.IsNullOrEmpty(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount) ? double.Parse("0") :
                            double.Parse(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount, CultureInfo.GetCultureInfo("en-US"));
                        AmountTocybersource = AmountTocybersource.SignificantDigits(2);
                        DiscountApplyed = false;
                        Bills = new List<BillDto>() { new BillDto() { BillExternalId = "0" } };
                        TotalAmount = string.IsNullOrEmpty(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount) ? double.Parse("0") :
                            double.Parse(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount, CultureInfo.GetCultureInfo("en-US"));
                        TotalTaxedAmount = string.IsNullOrEmpty(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount) ? double.Parse("0") :
                            double.Parse(logPaymentCyberSource.LogPaymentCyberSource.CyberSourceLogData.ReqAmount, CultureInfo.GetCultureInfo("en-US"));
                        Discount = 0;
                        DiscountPer = 0;
                        DiscountType = DiscountLabelTypeDto.NoDiscount; //nuevo
                        TransactionDate = logPaymentCyberSource.LogPaymentCyberSource.TransactionDateTime;

                        if (IsRefund)
                        {
                            OriginalPayment = _paymentClientService.GetByTransactionNumber(CyberSourceData.BillTransRefNo).GetAwaiter().GetResult();
                            DiscountType = OriginalPayment != null && OriginalPayment.DiscountApplyed ?
                                    OriginalPayment.DiscountObj != null ? (DiscountLabelTypeDto)(int)OriginalPayment.DiscountObj.DiscountLabel : DiscountLabelTypeDto.NoDiscount
                                : DiscountLabelTypeDto.NoDiscount;

                            //TODO QUITAR ESTO CUANDO VISA NOS PIDA. SE HARCODEA POR PEDIDO DE ELLOS 20160401
                            if (DiscountType == DiscountLabelTypeDto.TaxReintegration)
                            {
                                DiscountType = DiscountLabelTypeDto.TourismActivity;
                            }
                        }
                    }
                }
            }
        }

    }
}