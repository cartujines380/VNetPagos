using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CyberSource;
using Newtonsoft.Json;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;


namespace VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor
{
    public class PaymentProcessorHandler
    {
        private static readonly IServiceServiceAssosiate _serviceServiceAssosiate = NinjectRegister.Get<IServiceServiceAssosiate>();
        private static readonly IServiceService _serviceService = NinjectRegister.Get<IServiceService>();
        private static readonly IServiceBill _serviceBill = NinjectRegister.Get<IServiceBill>();
        private static readonly IServicePayment _servicePayment = NinjectRegister.Get<IServicePayment>();
        private static readonly IServiceDiscountCalculator _serviceDiscountCalculator = NinjectRegister.Get<IServiceDiscountCalculator>();
        private static readonly IServiceNotification _serviceNotification = NinjectRegister.Get<IServiceNotification>();
        private static readonly ILoggerService _loggerService = NinjectRegister.Get<ILoggerService>();
        private static readonly IServiceBin _binSerice = NinjectRegister.Get<IServiceBin>();
        private static readonly IServiceProcessHistory _processHistoryService = NinjectRegister.Get<IServiceProcessHistory>();
        private static readonly IRepositoryParameters _repositoryParameters = NinjectRegister.Get<IRepositoryParameters>();
        private static readonly IServiceEmailMessage _serviceNotificationMessage = NinjectRegister.Get<IServiceEmailMessage>();

        int _totalServices = 0;
        int _totalServicesTryGetBill = 0;
        int _totalBillsForService = 0;
        int _billscount = 0;
        int _paymentsDone = 0;
        int _paymentCancelled = 0;
        int _paymentsNotDoneAmount = 0;
        int _paymentsNotDoneOverQuotas = 0;
        int _errorsBeforeGettingBill = 0;
        int _exceptionscount = 0;
        int _exceptionsGeneral = 0;


        public void ObtainBillsAndPay()
        {
            _paymentsDone = 0;
            _paymentCancelled = 0;
            _paymentsNotDoneAmount = 0;
            _paymentsNotDoneOverQuotas = 0;
            _billscount = 0;
            _errorsBeforeGettingBill = 0;
            _exceptionscount = 0;
            _totalServices = 0;

            var firstRun = true;
            var lastRun = false;
            var exceededRuns = false;
            ProcessHistoryDto processHistory = null;
            var pendingPaymentsList = new List<PendingAutomaticPaymentDto>();
            var processStatus = ProcessStatusDto.Success;
            var totalErrorsRetry = 0;
            var totalErrorsControlled = 0;

            try
            {
                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO PROGRAMADO");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));


                //verifico el numero de intento para determinar si se debe correr y si debe enviar notificaciones
                var processMaxTries = Convert.ToInt32(ConfigurationManager.AppSettings["ProcessPaymentMaxTries"]);
                var processCount = _processHistoryService.GetProcessCountForDate(DateTime.Now); //cantidad de veces ya corrido

                if (processCount > 0)
                {
                    var executedSuccessfully = _processHistoryService.ProcessExecutedSuccessfully(DateTime.Now);
                    if (executedSuccessfully)
                    {
                        NLogLogger.LogEvent(NLogType.Info, "EL PROCESO YA FUE EJECUTADO CORRECTAMENTE");
                        return;
                    }
                    firstRun = false;
                }

                //TODO cambiar esta condicion si se quiere poder correrlo manualmente mas veces del limite
                if (processCount >= processMaxTries)
                {
                    NLogLogger.LogEvent(NLogType.Info, "SE ALCANZÓ EL NUMERO MÁXIMO DE INTENTOS (" + processMaxTries + ") ");
                    exceededRuns = true;
                    return;
                }

                //si es el ultimo intento envio notificaciones
                if (processCount >= processMaxTries - 1)
                {
                    lastRun = true;
                }

                NLogLogger.LogEvent(NLogType.Info, "INTENTO NUMERO: " + (processCount + 1));

                NLogLogger.LogEvent(NLogType.Info, "Proceso Pago Programado -- Registro en la tabla ProcessHistory con intento '" + (processCount + 1) + "' y estado 'Inicia'");

                processHistory = _processHistoryService.Create(new ProcessHistoryDto
                {
                    Count = processCount + 1,
                    Process = ProcessTypeDto.AutomaticPayment,
                    Status = ProcessStatusDto.Start,
                    PendingAutomaticPayments = new List<PendingAutomaticPaymentDto>()
                }, true);


                NLogLogger.LogEvent(NLogType.Info, "Proceso Pago Programado -- Obtengo lista de servicios");
                Console.WriteLine("Consultando servicios con pagos programados");
                sw.Start();

                List<ServiceAssociatedDto> services;

                if (firstRun)
                {
                    //si es la primer corrida los obtengo de la lista de servicios asociados
                    services = _serviceServiceAssosiate.GetServicesActiveAutomaticPayment().ToList();
                }
                else
                {
                    //sino los obtengo de los pendientes de las corridas anteriores
                    services = _processHistoryService.GetPendingAutomaticPayments(DateTime.Now).ToList();
                }
                _totalServices = services.Count();

                sw.Stop();
                Console.WriteLine("Tiempo de consulta {0}", sw.Elapsed);
                Console.WriteLine("Cantidad de servicios obtenidos {0}", _totalServices);
                Console.WriteLine();

                NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- Cantidad de servicios: " + _totalServices);

                foreach (var associatedDto in services)
                {
                    try
                    {
                        //filtro servicios inactivos momentaneamente
                        if(IsServiceMomentarilyDisabled(associatedDto.ServiceId))
                        {
                            continue;
                        }

                        var message = string.Empty;

                        if (!associatedDto.ServiceDto.EnableAutomaticPayment)
                        {
                            var cannotpay = string.Format("   Proceso Pago Programado -- El servicio: {0} (ref: {1}), Usuario {2}, servicio asociado id {3} tiene desahbilitado el pago programado",
                            associatedDto.ServiceDto != null ? associatedDto.ServiceDto.Name : "NULL",
                            associatedDto.ReferenceNumber,
                            associatedDto.RegisteredUserDto != null ? associatedDto.RegisteredUserDto.Email : "NULL",
                            associatedDto.Id);
                            NLogLogger.LogEvent(NLogType.Info, cannotpay);

                            message = "El servicio: " + associatedDto.ServiceDto.Name + " tiene desahbilitado el pago programado. Esto no se podra ejecutar por el momento";
                            GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, null);

                            //si no es la primer corrida y ocurre, debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                            }
                            totalErrorsControlled++;

                            continue;
                        }

                        #region Pago Servicio Asociado

                        //VALIDO QUE SEAN PAGOS ILIMITADOS
                        var msg = string.Format("   Proceso Pago Programado -- Inicio proceso de pago para servicio: {0} (ref: {1}), Usuario {2}, servicio asociado id {3}",
                            associatedDto.ServiceDto != null ? associatedDto.ServiceDto.Name : "NULL",
                            associatedDto.ReferenceNumber,
                            associatedDto.RegisteredUserDto != null ? associatedDto.RegisteredUserDto.Email : "NULL",
                            associatedDto.Id);
                        NLogLogger.LogEvent(NLogType.Info, msg);

                        if (!associatedDto.AutomaticPaymentDto.UnlimitedQuotas)
                        {
                            if (associatedDto.AutomaticPaymentDto.QuotasDone > associatedDto.AutomaticPaymentDto.Quotas)
                            {
                                msg =
                                    string.Format(
                                        "       Proceso Pago Programado -- El servicio: {0} (ref: {1}) excedio la cantidad de pagos permitidos. Pagos realizados {2}, pagos permitidos {3}",
                                        associatedDto.ServiceDto != null ? associatedDto.ServiceDto.Name : "NULL",
                                        associatedDto.ReferenceNumber,
                                        associatedDto.AutomaticPaymentDto.QuotasDone,
                                        associatedDto.AutomaticPaymentDto.Quotas);
                                NLogLogger.LogEvent(NLogType.Info, msg);

                                //si no es la primer corrida y ocurre, debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                                if (!firstRun)
                                {
                                    _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                                }
                                totalErrorsControlled++;

                                continue;
                            }
                        }
                        else
                        {
                            msg = string.Format("       Proceso Pago Programado -- Servicio: {0} (ref: {1}) con cantidad de pagos ilimitado",
                                    associatedDto.ServiceDto != null ? associatedDto.ServiceDto.Name : "NULL",
                                    associatedDto.ReferenceNumber);
                            NLogLogger.LogEvent(NLogType.Info, msg);
                        }
                        //&& (sc.AutomaticPayment.UnlimitedQuotas || sc.AutomaticPayment.QuotasDone < sc.AutomaticPayment.Maximum)

                        #region Validaciones tarjeta default

                        //no existe token
                        message = string.Empty;
                        if (associatedDto.DefaultCard == null || (associatedDto.DefaultCard != null && string.IsNullOrEmpty(associatedDto.DefaultCard.PaymentToken)))
                        {
                            #region Error token vacio
                            message = string.Format(
                       "El pago programado para el servicio {0} {1} no pudo ser ejecutado ya que no existe una correcta configuración de la tarjeta asociada. Le rogamos que verifique la configuración.",
                               associatedDto.ServiceDto.Name,
                               associatedDto.Description);
                            NLogLogger.LogEvent(NLogType.Info, message);

                            var logerMsg =
                                string.Format(
                                    "El token de la tarjeta se encuentra vacío en el sistema. Servicio: {0} {1}. Tarjeta: {2}",
                                    associatedDto.ServiceDto.Name,
                                    associatedDto.Description,
                                    associatedDto.DefaultCard.MaskedNumber);

                            GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, logerMsg);
                            _errorsBeforeGettingBill++;
                            //no puedo pagar las facturas del servicio por lo que continuo con otro

                            //si no es la primer corrida y ocurre, debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                            }
                            totalErrorsControlled++;

                            continue;
                            #endregion
                        }

                        //no se puede obtener el bin
                        var bin = 0;
                        if (associatedDto.DefaultCard != null &&
                            Int32.TryParse(associatedDto.DefaultCard.MaskedNumber.Substring(0, 6), out bin) == false)
                        {
                            var logerMsg =
                                string.Format(
                                    "El Bin de la tarjeta no es correcto. Servicio: {0} {1}. Tarjeta: {2}",
                                    associatedDto.ServiceDto.Name,
                                    associatedDto.Description,
                                    associatedDto.DefaultCard.MaskedNumber);

                            GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, logerMsg);
                            _errorsBeforeGettingBill++;
                            //no puedo pagar las facturas del servicio por lo que continuo con otro

                            //si no es la primer corrida y ocurre, debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                            }
                            totalErrorsControlled++;

                            continue;
                        }

                        //tarjeta vencida
                        if (associatedDto.DefaultCard != null && associatedDto.DefaultCard.DueDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                        {
                            message = string.Format("       La tarjeta de crédito {0} se encuentra vencida desde el {1}.",
                                    associatedDto.DefaultCard.MaskedNumber,
                                    associatedDto.DefaultCard.DueDate.ToString("MM/yyyy"));
                            NLogLogger.LogEvent(NLogType.Info, message);
                            GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, message);
                            //no puedo pagar las facturas del servicio por lo que continuo con otro
                            _errorsBeforeGettingBill++;

                            //si no es la primer corrida y ocurre, debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                            }
                            totalErrorsControlled++;

                            continue;
                        }

                        #endregion

                        msg = string.Format("       Proceso Pago Programado -- Obtengo lista de facturas para servicio: {0} (ref: {1})",
                            associatedDto.ServiceDto != null ? associatedDto.ServiceDto.Name : "NULL",
                            associatedDto.ReferenceNumber);
                        NLogLogger.LogEvent(NLogType.Info, msg);

                        ICollection<BillDto> bills = null;
                        var errorInBills = false;
                        try
                        {
                            _totalServicesTryGetBill++;
                            bills = ObtainBillsForService(associatedDto);
                            _totalBillsForService = bills != null
                                ? _totalBillsForService + bills.Count
                                : _totalBillsForService + 0;
                        }
                        catch (BillException e)
                        {
                            errorInBills = true;
                            #region Exception

                            _errorsBeforeGettingBill++;
                            NLogLogger.LogEvent(NLogType.Info, "       Proceso Pago Programado -- BillException CAPTURADA");
                            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- BillException CAPTURADA");
                            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- MENSAJE: " + e.Message);
                            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
                            if (e.InnerException != null)
                                NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
                            
                            message = string.Format(
                                "No se pudieron obtener las facturas correspondientes al servicio que tiene programado para {0} {1} en la fecha {2}." +
                                " Detalle del problema: " + e.Message + Environment.NewLine + 
                                " El día de mañana se intentará nuevamente. En caso de haber configurado su pago programado para 1 o 2 días previos al vencimiento, le sugerimos que consulte manualmente a través del Portal https://www.visanetpagos.com.uy." +
                                " En caso de realizar su pago de forma manual a través del Portal, no se realizará el pago programado." +
                                " Si tiene alguna duda, por favor comuniquese con el call center.",
                                associatedDto.ServiceDto.Name,
                                associatedDto.Description,
                                DateTime.Now.ToString("dd/MM/yyyy"));

                            processStatus = ProcessStatusDto.Error;
                            totalErrorsRetry++;
                            if (firstRun)
                            {
                                //si es la primer corrida insertar en la tabla Pending con Status ErrorRetry y mensaje "Error al obtener facturas"
                                var toRetry = new PendingAutomaticPaymentDto()
                                {
                                    Date = DateTime.Now,
                                    PendingServiceAssociateId = associatedDto.Id,
                                    Status = PendingRegisterStatusDto.ErrorRetry,
                                    ProcessHistoryId = processHistory.Id,
                                    LastProcessHistoryId = processHistory.Id,
                                    ErrorMessage = "Error al obtener facturas."
                                };
                                pendingPaymentsList.Add(toRetry);
                            }
                            if (lastRun)
                            {
                                //si es la ultima corrida cambiar el Status a ErrorDefinitive y mando notificaciones
                                GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, e,
                                    null);

                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id,
                                    DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorDefinitive);
                            }
                            #endregion
                        }
                        catch (Exception e)
                        {
                            errorInBills = true;
                            #region Exception

                            _errorsBeforeGettingBill++;
                            NLogLogger.LogEvent(NLogType.Info, "       Proceso Pago Programado -- EXCEPCION CAPTURADA");
                            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- EXCEPCION CAPTURADA");
                            NLogLogger.LogEvent(NLogType.Error,
                                "       Proceso Pago Programado -- MENSAJE: " + e.Message);
                            NLogLogger.LogEvent(NLogType.Error,
                                "       Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
                            if (e.InnerException != null)
                            {
                                NLogLogger.LogEvent(NLogType.Error,
                                    "       Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
                            }

                            message = string.Format(
                                "No se pudieron obtener las facturas correspondientes al servicio que tiene programado para {0} {1} en la fecha {2}." +
                                "El día de mañana se intentará nuevamente. En caso de haber configurado su pago programado para 1 o 2 días previos al vencimiento, le sugerimos que consulte manualmente a través del Portal https://www.visanetpagos.com.uy." +
                                "En caso de realizar su pago de forma manual a través del Portal, no se realizará el pago programado. " +
                                "Si tiene alguna duda, por favor comuniquese con el call center.",
                                associatedDto.ServiceDto.Name,
                                associatedDto.Description,
                                DateTime.Now.ToString("dd/MM/yyyy"));

                            processStatus = ProcessStatusDto.Error;
                            totalErrorsRetry++;
                            if (firstRun)
                            {
                                //si es la primer corrida insertar en la tabla Pending con Status ErrorRetry y mensaje "Error al obtener facturas"
                                var toRetry = new PendingAutomaticPaymentDto()
                                              {
                                                  Date = DateTime.Now,
                                                  PendingServiceAssociateId = associatedDto.Id,
                                                  Status = PendingRegisterStatusDto.ErrorRetry,
                                                  ProcessHistoryId = processHistory.Id,
                                                  LastProcessHistoryId = processHistory.Id,
                                                  ErrorMessage = "Error al obtener facturas."
                                              };
                                pendingPaymentsList.Add(toRetry);
                            }
                            if (lastRun)
                            {
                                //si es la ultima corrida cambiar el Status a ErrorDefinitive y mando notificaciones
                                GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, e,
                                    null);

                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id,
                                    DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorDefinitive);
                            }


                            #endregion
                        }
                        if (!errorInBills)
                        {
                            var errorRetry = false;
                            var errorControlled = false;
                            var messageQueue = new List<KeyValuePair<string, NotificationPruposeDto>>();

                            if (bills != null && bills.Any())
                            {
                                NLogLogger.LogEvent(NLogType.Info, "       Proceso Pago Programado -- Filtro la lista de facturas habilitadas a pagar (" + bills.Count + ") para el servicio asociado: " + associatedDto.Id);

                                //si la pasarela es sucive hay q confirmar
                                if (bills != null && bills.Any() && (bills.FirstOrDefault().Gateway == GatewayEnumDto.Sucive || bills.FirstOrDefault().Gateway == GatewayEnumDto.Geocom))
                                {
                                    bills = BillsEnableToPaySucive(associatedDto, bills);
                                }
                                else
                                {
                                    bills = BillsEnableToPay(bills, associatedDto);
                                }

                                NLogLogger.LogEvent(NLogType.Info, "       Proceso Pago Programado -- Cantidad de facturas a pagar: " + bills.Count + " , servicio asociado: " + associatedDto.Id);
                                Console.WriteLine("Proceso Pago Programado -- Cantidad de facturas a pagar: {0}, servicio asociado: {1}", bills.Count, associatedDto.Id);

                                #region bills

                                foreach (var bill in bills)
                                {
                                    RefreshTransactionData(associatedDto.RegisteredUserDtoId);
                                    var payBills = new List<BillDto> { bill };

                                    //Calculo y registro los descuentos aplicados, en el caso de tarjetas de debito
                                    NLogLogger.LogEvent(NLogType.Info, string.Format("            Proceso Pago Programado -- Calculando descuento {0}", associatedDto.Id));

                                    var binNumber = Int32.Parse(associatedDto.DefaultCard.MaskedNumber.Substring(0, 6));

                                    //obtengo la moneda de las facturas que se van a pagar
                                    //las facturas deben ser todas de la misma moneda.

                                    #region Calculate Discount

                                    var currency = bill.Currency;
                                    var discountQuery = new DiscountQueryDto
                                    {
                                        Bills = payBills,
                                        BinNumber = binNumber,
                                        ServiceId = associatedDto.ServiceDto.Id
                                    };

                                    //obtengo los valores con los descuentos correspondientes
                                    var cyberSourceExtraDataList = _serviceDiscountCalculator.Calculate(discountQuery);
                                    var cyberSourceExtraData = cyberSourceExtraDataList.FirstOrDefault();

                                    var payment = new PaymentDto
                                    {
                                        Bills = new List<BillDto>() { cyberSourceExtraData.BillDto },
                                        CardId = associatedDto.DefaultCardId,
                                        Card = associatedDto.DefaultCard,
                                        Date = DateTime.Now,
                                        ReferenceNumber = associatedDto.ReferenceNumber,
                                        ReferenceNumber2 = associatedDto.ReferenceNumber2,
                                        ReferenceNumber3 = associatedDto.ReferenceNumber3,
                                        ReferenceNumber4 = associatedDto.ReferenceNumber4,
                                        ReferenceNumber5 = associatedDto.ReferenceNumber5,
                                        ReferenceNumber6 = associatedDto.ReferenceNumber6,
                                        ServiceAssociatedId = associatedDto.Id,
                                        ServiceAssociatedDto = associatedDto,
                                        ServiceDto = associatedDto.ServiceDto,
                                        ServiceId = associatedDto.ServiceId,
                                        RegisteredUserId = associatedDto.RegisteredUserDtoId,
                                        RegisteredUser = associatedDto.RegisteredUserDto,
                                        PaymentType = PaymentTypeDto.Automatic,
                                        Currency = currency,
                                        Discount = cyberSourceExtraData.BillDto.DiscountAmount,//se debe ingresar el discountamount
                                        DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0,
                                        TotalAmount = cyberSourceExtraData.BillDto.Amount,
                                        TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount,
                                        AmountTocybersource = cyberSourceExtraData.CybersourceAmount,
                                        PaymentPlatform = PaymentPlatformDto.VisaNet,
                                        DiscountObjId = cyberSourceExtraData.DiscountDto != null ? cyberSourceExtraData.DiscountDto.Id : Guid.Empty,
                                        DiscountObj = cyberSourceExtraData.DiscountDto,
                                    };

                                    cyberSourceExtraData.BinNumber = int.Parse(payment.Card.MaskedNumber.Substring(0, 6));
                                    cyberSourceExtraData.CallcenterUser = String.Empty;

                                    NLogLogger.LogEvent(NLogType.Info, "      Proceso Pago Automático -- Llamo a metodo para pagar por medio del WCF Cybersource, servicio asociado: " +
                                        associatedDto.Id);

                                    #endregion

                                    if (payment.AmountTocybersource > 0)
                                    {
                                        #region Cybersource Payment

                                        Console.WriteLine("Realizando pago a través de CyberSource");
                                        sw.Restart();
                                        var paymentData = PayWithCybersource(payment, associatedDto, cyberSourceExtraData);
                                        sw.Stop();
                                        Console.WriteLine("Tiempo de ejecución: {0}", sw.Elapsed);
                                        payment.TransactionNumber = paymentData != null ? paymentData.TransactionId : null;
                                        payment.CyberSourceData = paymentData;

                                        #endregion

                                        if (paymentData != null && paymentData.ReasonCode.Equals("100"))
                                        {
                                            Console.WriteLine("Pago realizado");
                                            NLogLogger.LogEvent(NLogType.Info, "            Proceso Pago Programado -- Se registro el pago en cybersource, REQUIEST ID : " + paymentData.TransactionId + ", servicio asociado: " + associatedDto.Id);

                                            #region Notify Gateway

                                            //notifico a los entes
                                            NLogLogger.LogEvent(NLogType.Info, "            Proceso Pago Programado -- Notifico a los entes por medio de las pasarelas asociadas al servicio, servicio asociado: " +
                                                        associatedDto.Id);

                                            Console.WriteLine("Notificando pago de facturas");
                                            sw.Restart();
                                            var paymentDto = _servicePayment.NotifyGateways(payment);
                                            sw.Stop();
                                            Console.WriteLine("Tiempo de ejecución: {0}", sw.Elapsed);

                                            #endregion

                                            //Si el payment es null entonces se produjo un error en la notificación por las pasarelas
                                            //y se realizó el rollback de la transacción en cybersource
                                            if (paymentDto.NewPaymentDto != null)
                                            {
                                                _paymentsDone++;

                                                NLogLogger.LogEvent(NLogType.Info, "            Proceso Pago Programado -- Se registro el pago en nuevo sistema, REQUIEST ID : " + paymentData.TransactionId + ", servicio asociado: " + associatedDto.Id);
                                            }
                                            else
                                            {
                                                Console.WriteLine(
                                                    "No se pudo realizar el pago contra las pasarelas de pago");

                                                #region Error al intentar notificar a las pasarelas

                                                _paymentCancelled++;

                                                NLogLogger.LogEvent(NLogType.Info, "            Proceso Pago Programado -- paymentDto null");
                                                message = string.Format(
                                                            "El pago programado para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Por favor comuniquese con el call center si tiene alguna duda.",
                                                                associatedDto.ServiceDto.Name,
                                                                associatedDto.Description,
                                                                DateTime.Now.ToString("dd/MM/yyyy"));

                                                //error no controlado
                                                processStatus = ProcessStatusDto.Error;
                                                errorRetry = true;
                                                if (lastRun)
                                                {
                                                    GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, null);
                                                }

                                                #endregion

                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("No se pudo realizar el pago contra Cybresource");
                                            NLogLogger.LogEvent(NLogType.Info, "      Proceso Pago Programado -- No se pudo realizar el pago contra Cybresource. Reason Code: " + (paymentData != null ? paymentData.ReasonCode : " "));

                                            #region Error la intentar pagar en CS
                                            _paymentCancelled++;
                                            message = string.Format(
                                                        "El pago programado para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Por favor comuniquese con el call center si tiene alguna duda.",
                                                            associatedDto.ServiceDto.Name,
                                                            associatedDto.Description,
                                                            DateTime.Now.ToString("dd/MM/yyyy"));

                                            //si no se pudo realizar el pago en Cybersource lo considero como error no controlado
                                            processStatus = ProcessStatusDto.Error;
                                            errorRetry = true;
                                            if (lastRun)
                                            {
                                                GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, null);
                                            }

                                            #endregion

                                        }

                                        Console.WriteLine();
                                        Console.WriteLine();
                                    }
                                    else
                                    {
                                        Console.WriteLine("No se realizo pago contra Cybresource por no tener monto mayor a 0");
                                        NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- No se realizo pago contra Cybresource por no tener monto mayor a 0");
                                        #region Error al intentar pagar. Monto de factura no mayor a 0

                                        _paymentCancelled++;
                                        message = string.Format(
                                                    "El pago programado para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Por favor comuniquese con el call center si tiene alguna duda.",
                                                        associatedDto.ServiceDto.Name,
                                                        associatedDto.Description,
                                                        DateTime.Now.ToString("dd/MM/yyyy"));

                                        errorControlled = true;
                                        //manda mail al final si no hubo errorRetry o si es la ultima corrida
                                        messageQueue.Add(new KeyValuePair<string, NotificationPruposeDto>(message, NotificationPruposeDto.AlertNotification));

                                        //GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, null, null);

                                        #endregion
                                    }

                                }
                                #endregion

                                NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- SE COMPLETO EL PROCESO DE PAGO PARA EL SERVICIO ASOCIADO : " + associatedDto.Id);
                                Console.WriteLine();
                                Console.WriteLine();
                            }
                            else
                            {
                                NLogLogger.LogEvent(NLogType.Info, "       Proceso Pago Programado -- Cantidad de facturas a pagar: 0 , servicio asociado: " + associatedDto.Id);
                            }

                            #region Acciones post recorrida de bills

                            //si es la primer corrida y hubo errores no controlados insertar en la tabla de pendientes
                            if (firstRun && errorRetry)
                            {
                                var toRetry = new PendingAutomaticPaymentDto()
                                {
                                    Date = DateTime.Now,
                                    PendingServiceAssociateId = associatedDto.Id,
                                    Status = PendingRegisterStatusDto.ErrorRetry,
                                    ProcessHistoryId = processHistory.Id,
                                    LastProcessHistoryId = processHistory.Id
                                };
                                pendingPaymentsList.Add(toRetry);
                            }
                            //si es la ultima corrida y hubo errores no controlados cambiar el Status a ErrorDefinitive
                            if (lastRun && errorRetry)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorDefinitive);
                            }
                            //si no es la primer corrida, Y NO HUBO ERRORES debo cambiar el Status a Success en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun && !errorRetry && !errorControlled)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.Success);
                            }
                            //si no es la primer corrida, Y HUBO SOLAMENTE ERRORES CONTROLADOS debo cambiar el Status a ErrorControlled en la tabla Pending y actualizar el LastProcessHistoryId
                            if (!firstRun && !errorRetry && errorControlled)
                            {
                                _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorControlled);
                            }
                            //si no hubo errores no controlados o si es la ultima corrida
                            if (!errorRetry || lastRun)
                            {
                                foreach (var toSnd in messageQueue)
                                {
                                    GeneralNotification(toSnd.Value, associatedDto, toSnd.Key, null, null);
                                }
                            }
                            #endregion

                            if (errorRetry)
                            {
                                totalErrorsRetry++;
                            }
                            else if (errorControlled)
                            {
                                totalErrorsControlled++;
                            }
                        }
                        #endregion
                    }
                    //catch (BusinessException ex)
                    //{
                    //    #region BusinessException

                    //    _exceptionscount++;

                    //    NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- BUSINESSEXCEPTION");
                    //    NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- MENSAJE:" + ex.Message);
                    //    NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- STACK TRACE: " + ex.StackTrace);

                    //    var message = string.Format(
                    //                "El pago programado para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Por favor comuniquese con el call center",
                    //                associatedDto.ServiceDto.Name,
                    //                associatedDto.Description,
                    //                DateTime.Now.ToString("dd/MM/yyyy"));

                    //    GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, ex, null);

                    //    #endregion
                    //}
                    catch (Exception e)
                    {
                        #region Exception

                        _exceptionscount++;
                        NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- EXCEPCION CAPTURADA");
                        NLogLogger.LogEvent(NLogType.Error, "  Proceso Pago Programado -- EXCEPCION CAPTURADA");
                        NLogLogger.LogEvent(NLogType.Error, "  Proceso Pago Programado -- MENSAJE: " + e.Message);
                        NLogLogger.LogEvent(NLogType.Error, "  Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
                        if (e.InnerException != null)
                        {
                            NLogLogger.LogEvent(NLogType.Error,
                                "Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
                        }

                        var message = string.Format(
                                "El pago programado para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Por favor comuniquese con el call center",
                                associatedDto.ServiceDto.Name,
                                associatedDto.Description,
                                DateTime.Now.ToString("dd/MM/yyyy"));

                        processStatus = ProcessStatusDto.Error;
                        totalErrorsRetry++;
                        if (firstRun)
                        {
                            //si es la primer corrida insertar en la tabla Pending con Status ErrorRetry
                            var toRetry = new PendingAutomaticPaymentDto()
                            {
                                Date = DateTime.Now,
                                PendingServiceAssociateId = associatedDto.Id,
                                Status = PendingRegisterStatusDto.ErrorRetry,
                                ProcessHistoryId = processHistory.Id,
                                LastProcessHistoryId = processHistory.Id
                            };
                            pendingPaymentsList.Add(toRetry);
                        }
                        if (lastRun)
                        {
                            //si es la ultima corrida cambiar el Status a ErrorDefinitive y mando notificaciones
                            GeneralNotification(NotificationPruposeDto.AlertNotification, associatedDto, message, e, null);

                            _processHistoryService.ChangePendingAutomaticPaymentStatus(associatedDto.Id, DateTime.Now, processHistory.Id, (int)PendingRegisterStatusDto.ErrorDefinitive);
                        }

                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                #region Exception
                _exceptionsGeneral++;
                NLogLogger.LogEvent(NLogType.Info, "Proceso Pago Programado -- EXCEPCION CAPTURADA");
                NLogLogger.LogEvent(NLogType.Error, "Proceso Pago Programado -- EXCEPCION CAPTURADA");
                NLogLogger.LogEvent(NLogType.Error, "Proceso Pago Programado -- MENSAJE: " + e.Message);
                NLogLogger.LogEvent(NLogType.Error, "Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
                if (e.InnerException != null)
                {
                    NLogLogger.LogEvent(NLogType.Error, "Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
                }

                var parameters = _repositoryParameters.AllNoTracking().First();
                var title = "ERROR EN PAGO PROGRAMADO";

                var exceptionMessage = e != null ? e.Message : "";
                var stackTrace = e != null ? e.StackTrace : "";
                var innerException = e != null ? e.InnerException : null;

                _serviceNotificationMessage.SendInternalErrorNotification(parameters, title, string.Empty, String.Empty, exceptionMessage, stackTrace, innerException);

                //VER!! (si entra a este catch la primera vez la lista de pendientes queda vacia y las otras no van a tener registros)
                processStatus = ProcessStatusDto.Error;

                #endregion
            }
            finally
            {
                #region Finally
                NLogLogger.LogEvent(NLogType.Info, "FIN PROCESO PAGO PROGRAMADO ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));

                //si excedio la cantidad de intentos no mando el mail
                if (!exceededRuns)
                {
                    var parameters = _repositoryParameters.AllNoTracking().First();
                    var message = "El proceso de pago Programado fue finalizado en la fecha " + DateTime.Now.ToString("dd/MM/yyyy");
                    var totalServices = _totalServices + " servicios con pago programados.";
                    var totalServicesTryGetBill = _totalServicesTryGetBill + " servicios para los cuales se intento obtener facturas.";
                    var totalBillsForService = _totalBillsForService + " facturas obtenidas para pagar.";
                    var totalBillToPay = _billscount + " facturas habilitadas para pago.";
                    var paymentsDone = _paymentsDone + " facturas pagadas con éxito.";
                    var paymentsNotDoneAmount = _paymentsNotDoneAmount + " facturas con error (monto máximo configurado menor al monto de la factura)";
                    var paymentsNotDoneOverQuotas = _paymentsNotDoneOverQuotas + " facturas con error (superan límite de cantidad máxima de cuotas habilitadas por el usuario).";
                    var paymentCancelled = _paymentCancelled + " facturas canceladas por Cybersource o porque no se pudo notificar al ente.";
                    var errorsBeforeGettingBill = _errorsBeforeGettingBill + " errores producidos durante/previo a la obtención de facturas.";
                    var exceptionsCount = _exceptionscount + " errores producidos durante el proceso de pago de facturas.";
                    var exceptionsGeneral = _exceptionsGeneral + " excepciones generadas durante el proceso.";

                    _serviceNotificationMessage.SendAutomaticPaymentNotification(parameters, message, totalBillToPay, paymentsDone, paymentsNotDoneAmount, paymentsNotDoneOverQuotas, paymentCancelled, errorsBeforeGettingBill, exceptionsCount, totalServices, totalServicesTryGetBill, totalBillsForService, exceptionsGeneral);
                }

                //setear el estado final del proceso, y si es la primer corrida guardar los registros a reintentar
                if (processHistory != null)
                {
                    if (firstRun)
                    {
                        processHistory.PendingAutomaticPayments = pendingPaymentsList;
                    }
                    processHistory.Status = processStatus;

                    processHistory.Additional = "Servicios obtenidos:" + _totalServices + "|Correctos:" + (_totalServices - totalErrorsRetry - totalErrorsControlled) + "|Errores controlados:" + totalErrorsControlled + "|Errores no controlados:" + totalErrorsRetry;

                    _processHistoryService.Edit(processHistory);
                }

                #endregion
            }
        }

        /// <summary>
        /// Obtengo las facturas a pagar para el servicio asociado correspondiente
        /// </summary>
        /// <param name="serviceAssociatedDto"></param>
        /// <returns></returns>
        private ICollection<BillDto> ObtainBillsForService(ServiceAssociatedDto serviceAssociatedDto)
        {
            var references = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName, serviceAssociatedDto.ReferenceNumber);
            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName2))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName2, serviceAssociatedDto.ReferenceNumber2);
            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName3))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName3, serviceAssociatedDto.ReferenceNumber3);
            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName4))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName4, serviceAssociatedDto.ReferenceNumber4);
            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName5))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName5, serviceAssociatedDto.ReferenceNumber5);
            if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName6))
                references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName6, serviceAssociatedDto.ReferenceNumber6);
            var userBills = _serviceBill.GetBillsForRegisteredUser(new RegisteredUserBillFilterDto()
            {
                                                    ServiceId = serviceAssociatedDto.ServiceId,
                                                    References = references,
                                                    UserId = serviceAssociatedDto.RegisteredUserDtoId,
                                                    CardId = serviceAssociatedDto.DefaultCardId
                                                });

            return userBills.Bills;
            }

        /// <summary>
        /// Llamo metodo para pagar en cybersource
        /// </summary>
        /// <param name="paymentDto"></param>
        /// <param name="serviceAssociated"></param>
        /// <param name="cyberSourceExtraData"></param>
        /// <returns></returns>
        private CyberSourceDataDto PayWithCybersource(PaymentDto paymentDto, ServiceAssociatedDto serviceAssociated, CyberSourceExtraDataDto cyberSourceExtraData)
        {
            var tryCounter = 5;
            var exit = false;
            CyberSourceDataDto data = null;
            var transactionId = Guid.NewGuid();
            while (tryCounter > 0 && exit == false)
            {
                try
                {
                    var payment = new GeneratePayment
                    {
                        ApplicationUserId = serviceAssociated.RegisteredUserDtoId,
                        Currency = paymentDto.Currency,
                        MerchandId = serviceAssociated.ServiceDto.MerchantId,
                        Token = serviceAssociated.DefaultCard.PaymentToken,
                        TransaccionId = transactionId.ToString(),
                        GrandTotalAmount = paymentDto.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Key = serviceAssociated.ServiceDto.CybersourceTransactionKey,
                        UserType = LogUserType.Other,
                        PaymentPlatform = PaymentPlatformDto.VisaNet
                    };

                    var merchantDefinedData = LoadMerchantDefinedData(serviceAssociated, cyberSourceExtraData);

                    if (!String.IsNullOrEmpty(payment.Token))
                    {
                        var access = new CyberSourceAccess(NinjectRegister.Get<ILoggerService>(), NinjectRegister.Get<IServiceService>(),
                            NinjectRegister.Get<IServiceFixedNotification>(), NinjectRegister.Get<IServiceApplicationUser>(),
                            NinjectRegister.Get<IServiceAnonymousUser>(), NinjectRegister.Get<IServiceParameters>(),
                            NinjectRegister.Get<IRepositoryPayment>(), NinjectRegister.Get<IServiceCard>(), NinjectRegister.Get<IRepositoryConciliationCybersource>());
                        data = access.GeneratePayment(payment, merchantDefinedData);
                        exit = true;
                    }
                    tryCounter--;
                }
                catch (CybersourceException e)
                {
                    tryCounter--;
                }
            }
            return data;
        }

        /// <summary>
        /// Filtro la lista de facturas comparando con los parametros configurados en el pago programado
        /// </summary>
        /// <param name="bills"></param>
        /// <param name="serviceAssociatedDto"></param>
        /// <returns></returns>
        private ICollection<BillDto> BillsEnableToPay(IEnumerable<BillDto> bills, ServiceAssociatedDto serviceAssociatedDto)
        {
            var payment = serviceAssociatedDto.AutomaticPaymentDto;

            var finalList = new List<BillDto>();
            var billsUpdate = bills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();

            int quotasDone = payment.QuotasDone;
            int max = payment.Quotas;
            int countBills = billsUpdate.Count();
            var subtotal = quotasDone + countBills;

            foreach (var bill in billsUpdate)
            {
                //   facturas con fecha de vencimineto mayor a la fecha de hoy
                if (bill.ExpirationDate.AddDays(-payment.DaysBeforeDueDate).CompareTo(DateTime.Today.Date) <= 0)
                {
                    _billscount++;
                    //la cantidad facturas a pagar no puede ser mayor al maximo ingresado por el usuario. Si hay mas, pago las mas viejas
                    if (max >= subtotal || payment.UnlimitedQuotas)
                    {
                        if (bill.Amount <= payment.Maximum || payment.UnlimitedAmount)
                        {
                            finalList.Add(bill);
                        }
                        else
                        {
                            _paymentsNotDoneAmount++;
                            NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado --  Monto de factura mayor al máximo ingresado");
                            #region Notifico

                            var message = string.Format(
                                "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el monto máximo ingresado {2} {3} es inferior al monto de la factura {4} {5}",
                                serviceAssociatedDto.ServiceDto.Name,
                                serviceAssociatedDto.Description,
                                bill.Currency.Equals("UYU") ? "$" : "U$S",
                                payment.Maximum,
                                bill.Currency.Equals("UYU") ? "$" : "U$S",
                                bill.Amount);
                            NLogLogger.LogEvent(NLogType.Info, message);

                            if (message.Length > 500)
                            {
                                NLogLogger.LogEvent(NLogType.Error, "MENSAJE MAYOR A 500 !! TAMAÑO: " + message.Length + "MENSAJE: " + message);
                                message = message.Substring(0, 490);
                            }

                            _serviceNotificationMessage.SendGeneralNotification(serviceAssociatedDto.RegisteredUserDto, "Pago programado no realizado", message);

                            //var notificationNewUser = new EmailMessage
                            //{
                            //    EmailType = EmailType.GeneralNotification,
                            //    ApplicationUserId = serviceAssociatedDto.RegisteredUserDtoId,
                            //    DataByType = JsonConvert.SerializeObject(new
                            //    {
                            //        Title = "Pago programado no realizado",
                            //        Message = message
                            //    }),
                            //};

                            #region AppNotification

                            _serviceNotification.Create(new NotificationDto
                            {
                                Date = DateTime.Now,
                                Message = message,
                                NotificationPrupose = NotificationPruposeDto.AlertNotification,
                                RegisteredUserId = serviceAssociatedDto.RegisteredUserDtoId,
                                ServiceId = serviceAssociatedDto.ServiceId,
                                 
                            });
                            #endregion

                            _loggerService.CreateLog(LogType.Info,
                                LogOperationType.AutomaticPaymentBatch,
                                LogCommunicationType.VisaNet,
                                serviceAssociatedDto.RegisteredUserDtoId,
                                string.Format(
                                    "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, en la facutra {3} el monto máximo ingresado {4}{5} es inferior al monto de la factura {6}{7}",
                                    serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                                    serviceAssociatedDto.Description,
                                    serviceAssociatedDto.Id,
                                    bill.BillExternalId,
                                    bill.Currency,
                                    payment.Maximum,
                                    bill.Currency,
                                    bill.Amount),
                                message);

                            #endregion
                        }
                    }
                    else
                    {
                        _paymentsNotDoneOverQuotas++;
                        NLogLogger.LogEvent(NLogType.Info, "  Proceso Pago Programado -- Pase la cantidad de pagos programados configurados.");
                        #region Notifico

                        var message = string.Format(
                            "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el máximo número {2} de facturas a pagar ya fue superado.",
                            serviceAssociatedDto.ServiceDto.Name,
                            serviceAssociatedDto.Description,
                            max);
                        NLogLogger.LogEvent(NLogType.Info, message);
                        if (message.Length > 500)
                        {
                            NLogLogger.LogEvent(NLogType.Error, "MENSAJE MAYOR A 500 !! TAMAÑO: " + message.Length + "MENSAJE: " + message);
                            message = message.Substring(0, 490);
                        }

                        _serviceNotificationMessage.SendGeneralNotification(serviceAssociatedDto.RegisteredUserDto, "Pago programado no realizado", message);

                        _loggerService.CreateLog(LogType.Info,
                            LogOperationType.AutomaticPaymentBatch,
                            LogCommunicationType.VisaNet,
                            serviceAssociatedDto.RegisteredUserDtoId,
                            string.Format(
                                "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, el máximo número {3} de facturas a pagar ya fue superado.",
                                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                                serviceAssociatedDto.Description,
                                serviceAssociatedDto.Id,
                                max),
                            message);
                        _serviceNotification.Create(new NotificationDto
                        {
                            Date = DateTime.Now,
                            NotificationPrupose = NotificationPruposeDto.AlertNotification,
                            Message = message,
                            RegisteredUserId = serviceAssociatedDto.RegisteredUserDtoId,
                            ServiceId = serviceAssociatedDto.ServiceId,
                        });
                        #endregion
                    }
                }
                subtotal--;
            }
            return finalList;
        }

        private ICollection<BillDto> BillsEnableToPaySucive(ServiceAssociatedDto serviceAssociatedDto, IEnumerable<BillDto> bills)
        {
            var billsUpdate = bills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();

            var days = serviceAssociatedDto.AutomaticPaymentDto.DaysBeforeDueDate;
            var patentToPayLines = serviceAssociatedDto.AutomaticPaymentDto.SuciveAnnualPatent ? CheckSucivePatent(billsUpdate, days) : "";

            var linesToPay = "";
            //Si no hay patente anual a pagar, pagos las vencidas y dentro del rango de dias del usuario. Si hay, pago todas hasta la primera cuota
            linesToPay = LinesWithAnnualPatent(billsUpdate, days, patentToPayLines);

            //chequeo si se puede pagar este listado
            var dtpo = GetDeptoFromService(serviceAssociatedDto);
            if (string.IsNullOrEmpty(linesToPay))
            {
                return new List<BillDto>();
            }
            var gateway =
                serviceAssociatedDto.ServiceDto.ServiceGatewaysDto.FirstOrDefault(
                    x => x.Gateway.Enum == (int)bills.FirstOrDefault().Gateway);
            var billDto = _serviceBill.ChekBills(linesToPay, int.Parse(serviceAssociatedDto.ReferenceNumber6), dtpo, bills.FirstOrDefault().Gateway, gateway.ReferenceId);

            if (billDto != null)
            {
                _billscount++;

                int quotasDone = serviceAssociatedDto.AutomaticPaymentDto.QuotasDone;
                int max = serviceAssociatedDto.AutomaticPaymentDto.Quotas;
                //la cantidad facturas a pagar no puede ser mayor al maximo ingresado por el usuario. Si hay mas, pago las mas viejas
                if (max < quotasDone && !serviceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas)
                {
                    _paymentsNotDoneOverQuotas++;
                    #region Notifico

                    var message = string.Format(
                        "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el máximo número {2} de facturas a pagar ya fue superado.",
                        serviceAssociatedDto.ServiceDto.Name,
                        serviceAssociatedDto.Description,
                        max);
                    NLogLogger.LogEvent(NLogType.Info, message);
                    if (message.Length > 500)
                    {
                        NLogLogger.LogEvent(NLogType.Error, "MENSAJE MAYOR A 500 !! TAMAÑO: " + message.Length + "MENSAJE: " + message);
                        message = message.Substring(0, 490);
                    }

                    _serviceNotificationMessage.SendGeneralNotification(serviceAssociatedDto.RegisteredUserDto, "Pago programado no realizado", message);

                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.AutomaticPaymentBatch,
                        LogCommunicationType.VisaNet,
                        serviceAssociatedDto.RegisteredUserDtoId,
                        string.Format(
                            "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, el máximo número {3} de facturas a pagar ya fue superado.",
                            serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                            serviceAssociatedDto.Description,
                            serviceAssociatedDto.Id,
                            max),
                        message);
                    _serviceNotification.Create(new NotificationDto
                    {
                        Date = DateTime.Now,
                        NotificationPrupose = NotificationPruposeDto.AlertNotification,
                        Message = message,
                        RegisteredUserId = serviceAssociatedDto.RegisteredUserDtoId,
                        ServiceId = serviceAssociatedDto.ServiceId,
                    });
                    #endregion
                    return new List<BillDto>();
                }

                //chequeo que el monto maximo no sobrepase el configurado
                if (billDto.Amount <= serviceAssociatedDto.AutomaticPaymentDto.Maximum || serviceAssociatedDto.AutomaticPaymentDto.UnlimitedAmount)
                {
                    _paymentsNotDoneAmount++;
                    #region Notifico

                    var message = string.Format(
                        "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el monto máximo ingresado {2} {3} es inferior al monto de la factura {4} {5}",
                        serviceAssociatedDto.ServiceDto.Name,
                        serviceAssociatedDto.Description,
                        billDto.Currency.Equals("UYU") ? "$" : "U$S",
                        serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                        billDto.Currency.Equals("UYU") ? "$" : "U$S",
                        billDto.Amount);
                    NLogLogger.LogEvent(NLogType.Info, message);

                    if (message.Length > 500)
                    {
                        NLogLogger.LogEvent(NLogType.Error, "MENSAJE MAYOR A 500 !! TAMAÑO: " + message.Length + "MENSAJE: " + message);
                        message = message.Substring(0, 490);
                    }

                    _serviceNotificationMessage.SendGeneralNotification(serviceAssociatedDto.RegisteredUserDto, "Pago programado no realizado", message);

                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.AutomaticPaymentBatch,
                        LogCommunicationType.VisaNet,
                        serviceAssociatedDto.RegisteredUserDtoId,
                        string.Format(
                            "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, en la facutra {3} el monto máximo ingresado {4}{5} es inferior al monto de la factura {6}{7}",
                            serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                            serviceAssociatedDto.Description,
                            serviceAssociatedDto.Id,
                            billDto.BillExternalId,
                            billDto.Currency,
                            serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                            billDto.Currency,
                            billDto.Amount),
                        message);

                    _serviceNotification.Create(new NotificationDto
                    {
                        Date = DateTime.Now,
                        NotificationPrupose = NotificationPruposeDto.AlertNotification,
                        Message = message,
                        RegisteredUserId = serviceAssociatedDto.RegisteredUserDtoId,
                        ServiceId = serviceAssociatedDto.ServiceId,
                    });

                    #endregion
                    return new List<BillDto>() { billDto };
                }
            }
            return new List<BillDto>();
        }

        private string LinesWithAnnualPatent(IEnumerable<BillDto> bills, int days, string linePatents)
        {
            var result = "";
            if (string.IsNullOrEmpty(linePatents))
            {
                //PAGO SOLO UNA
                foreach (var bill in bills)
                {
                    if (DateTime.Today.Date.CompareTo(bill.ExpirationDate) < 0)
                    {
                        //facturas con fecha de vencimineto mayor a la fecha de hoy
                        if (bill.ExpirationDate.AddDays(-days).CompareTo(DateTime.Today.Date) <= 0)
                        {
                            result = result + bill.Line;
                        }
                    }
                    else
                    {
                        //ESTA VENCIDA. SE AGREGAR AL PAGO
                        result = result + bill.Line;
                    }
                }
            }
            else
            {
                //PAGO ANUAL
                var next = true;
                var firstPatent = String.IsNullOrEmpty(linePatents) ? "" : linePatents.Split(';').First();
                foreach (var bill in bills)
                {
                    if (next)
                    {
                        //ESTA VENCIDA. SE AGREGAR AL PAGO
                        result = result + bill.Line;
                        if (bill.Line.Contains(firstPatent))
                            next = false;
                    }
                }
                return result + linePatents;
            }

            return result;
        }

        private string CheckSucivePatent(IEnumerable<BillDto> bills, int days)
        {
            var now = DateTime.Today;
            var patente = bills.Where(b => b.Description.Contains("PATENTE") && b.ExpirationDate.CompareTo(now) >= 0).OrderBy(b => b.ExpirationDate).GroupBy(b => b.Discount).ToList();
            //cuotas de patentes no vencidas
            if (patente == null) return "";
            foreach (var pat in patente)
            {
                //el año de la patente anual a pagar no puede ser menor a este año
                if (pat.Key >= now.Year)
                {
                    //para ser anual tienen que ser 6 cuotas
                    if (pat.Count() == 6)
                    {
                        var bill = pat.First();
                        //Tiene que estar dentro del rango de dias habilitados por el usuario para pagar
                        if (bill.ExpirationDate.AddDays(-days).CompareTo(DateTime.Today.Date) <= 0)
                        {
                            var lines = pat.Select(b => b.Line);
                            return String.Join("", lines);
                        }
                    }
                }
            }
            return "";
        }

        private CyberSourceMerchantDefinedDataDto LoadMerchantDefinedData(ServiceAssociatedDto serviceAssociated, CyberSourceExtraDataDto cyberSourceExtraData)
        {

            var paymentsCount = _servicePayment.CountPaymentsDone(serviceAssociated.RegisteredUserDtoId, Guid.Empty, serviceAssociated.ServiceId);

            var merchantDefinedData = new CyberSourceMerchantDefinedDataDto
            {
                ServiceType = serviceAssociated.ServiceDto.ServiceCategory.Name,
                OperationTypeDto = OperationTypeDto.UniquePayment.ToString(),
                UserRegistered = "Y",
                UserRegisteredDays = (DateTime.Now.Date - serviceAssociated.RegisteredUserDto.CreationDate.Date).Days.ToString(),
                ReferenceNumber1 = serviceAssociated.ReferenceNumber,
                ReferenceNumber2 = serviceAssociated.ReferenceNumber2,
                ReferenceNumber3 = serviceAssociated.ReferenceNumber3,
                ReferenceNumber4 = serviceAssociated.ReferenceNumber4,
                ReferenceNumber5 = serviceAssociated.ReferenceNumber5,
                ReferenceNumber6 = serviceAssociated.ReferenceNumber6,
                RedirctTo = 0.ToString(),
                DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0 ? ((int)cyberSourceExtraData.DiscountDto.DiscountLabel).ToString() : "0",
                TotalAmount = cyberSourceExtraData.BillDto.Amount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                Discount = cyberSourceExtraData.BillDto.DiscountAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                BillNumber = cyberSourceExtraData.BillDto.BillExternalId,
                AditionalNumberElectornicBill = "",
                NameTh = serviceAssociated.DefaultCard.Name,
                PaymentCount = paymentsCount,
                UserCi = serviceAssociated.RegisteredUserDto.IdentityNumber,
                UserMobile = serviceAssociated.RegisteredUserDto.MobileNumber,
                UserRegisteredAddress = serviceAssociated.RegisteredUserDto.Address,

                MerchandId = serviceAssociated.ServiceDto.MerchantId,
                CardBin = cyberSourceExtraData.BinNumber.ToString(),
                ServiceName = serviceAssociated.ServiceDto.Name,
                CallcenterUser = cyberSourceExtraData.CallcenterUser,
                ServiceId = serviceAssociated.ServiceDto.Id,
                Plataform = PaymentPlatformDto.VisaNet.ToString(),
                PaymentTypeDto = PaymentTypeDto.Automatic

            };
            return merchantDefinedData;
        }

        private void RefreshTransactionData(Guid applicationUserId)
        {
            var transactionContext = NinjectRegister.Get<IWebApiTransactionContext>();
            transactionContext.TransactionIdentifier = Guid.NewGuid();
            transactionContext.TransactionDateTime = DateTime.Now;
            transactionContext.ApplicationUserId = applicationUserId.ToString();
        }

        #region SUCIVE
        //public void SuciveBatchConsiliation(int day)
        //{
        //    try
        //    {
        //        var sucivePath = ConfigurationManager.AppSettings["SuciveBatchPath"];
        //        var date = day > 0 ? DateTime.Today.AddDays(-day) : DateTime.Today.AddDays(-1);

        //        NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVO BATCH PARA SUCIVE");
        //        NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));

        //        var parameters = _serviceParameters.AllNoTracking().FirstOrDefault();
        //        var filePath = parameters == null ? "" : sucivePath + parameters.Sucive.Code + "_" + date.ToString("yyyyMMdd") + ".txt";

        //        if (File.Exists(filePath))
        //            return;

        //        var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Sucive, Guid.Empty);
        //        NLogLogger.LogEvent(NLogType.Info, "HAY " + (payments == null ? "0" : payments.Count().ToString()) + " TRANSACCIONES PARA ENVIAR");

        //        var list = GenerateSuciveFileString(payments, parameters.Sucive.Code);

        //        CreateFile(list, filePath);
        //        SendFileSftp(filePath, parameters.Sucive.Code + "_" + date.ToString("yyyyMMdd") + ".txt");
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "SE PRODUJO UNA EXCEPCION");
        //        NLogLogger.LogEvent(e);

        //        throw e;
        //    }
        //    finally
        //    {
        //        NLogLogger.LogEvent(NLogType.Info, "FIN PROCESO GENERACION ARCHIVO BATCH");
        //    }
        //}
        //private string GenerateSuciveFileString(IEnumerable<PaymentDto> payments, string code)
        //{
        //    try
        //    {
        //        if (payments == null || !payments.Any())
        //            return "NO EXISTEN DATOS PARA ENVIAR";

        //        var raw = "CodigoAutorizacion;IdMedioPago;FechaPago;Monto;NroCobro;IdPadron;IdGobiernoDepartamental;LugarCobro;NroDebito" + Environment.NewLine;

        //        foreach (var dto in payments)
        //        {
        //            if (dto.Bills != null && dto.Bills.Any())
        //            {
        //                var dtpo = GetDeptoFromService(dto);

        //                raw = raw + dto.PaymentIdentifierDto.UniqueIdentifier + ";";
        //                raw = raw + code + ";";
        //                raw = raw + dto.Date.ToString("ddMMyyyyHHmm") + ";";
        //                raw = raw + dto.Bills.First().Amount.SignificantDigits(2).ToString("#0.00", CultureInfo.CurrentCulture) + ";";
        //                raw = raw + dto.Bills.First().BillExternalId + ";";
        //                raw = raw + dto.ReferenceNumber6 + ";";
        //                raw = raw + SuciveDepartamentValue(dtpo) + ";";
        //                raw = raw + ";";
        //                raw = raw + "0;";
        //                raw = raw + Environment.NewLine;    
        //            }
        //        }

        //        NLogLogger.LogEvent(NLogType.Info, raw);

        //        raw = raw + payments.Count();
        //        return raw;
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.LogEvent(NLogType.Error, "Error GenerateSuciveFileString");
        //        NLogLogger.LogEvent(e);
        //        throw e;
        //    }
        //}
        //private int SuciveDepartamentValue(int dptoVisanetValue)
        //{
        //    switch (dptoVisanetValue)
        //    {
        //        case (int) DepartamentDtoType.Artigas:
        //            return 1;
        //        case (int)DepartamentDtoType.Canelones:
        //            return 2;
        //        case (int)DepartamentDtoType.Cerro_Largo:
        //            return 3;
        //        case (int)DepartamentDtoType.Colonia:
        //            return 4;
        //        case (int)DepartamentDtoType.Durazno:
        //            return 5;
        //        case (int)DepartamentDtoType.Flores:
        //            return 6;
        //        case (int)DepartamentDtoType.Florida:
        //            return 16;
        //        case (int)DepartamentDtoType.Lavalleja:
        //            return 18;
        //        case (int)DepartamentDtoType.Maldonado:
        //            return 14;
        //        case (int)DepartamentDtoType.Montevideo:
        //            return 19;
        //        case (int)DepartamentDtoType.Paysandu:
        //            return 8;
        //        case (int)DepartamentDtoType.Rio_Negro:
        //            return 11;
        //        case (int)DepartamentDtoType.Rivera:
        //            return 17;
        //        case (int)DepartamentDtoType.Rocha:
        //            return 15;
        //        case (int)DepartamentDtoType.Salto:
        //            return 7;
        //        case (int)DepartamentDtoType.San_Jose:
        //            return 12;
        //        case (int)DepartamentDtoType.Soriano:
        //            return 10;
        //        case (int)DepartamentDtoType.Tacuarembo:
        //            return 9;
        //        case (int)DepartamentDtoType.Treinta_y_Tres:
        //            return 13;

        //        default:
        //            return -1;
        //    }
        //}
        //private void SendFileSftp(string filePath, string fileName)
        //{
        //    try
        //    {

        //        var keyName = ConfigurationManager.AppSettings["SshPrivateKeyName"];
        //        string keyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key", keyName);

        //        //Send Ftp Files - same idea as above - try...catch and try to repeat this code 
        //        //if you can't connect the first time, timeout after a certain number of tries. 
        //        //Send Ftp Files - same idea as above - try...catch and try to repeat this code 
        //        //if you can't connect the first time, timeout after a certain number of tries. 
        //        SessionOptions sessionOptions = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SFTPPassword"]) ? new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = ConfigurationManager.AppSettings["SFTPHostName"],
        //            PortNumber = int.Parse(ConfigurationManager.AppSettings["SFTPPortNumber"]),
        //            UserName = ConfigurationManager.AppSettings["SFTPUserName"],
        //            SshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"],
        //            SshPrivateKeyPath = keyPath,
        //            GiveUpSecurityAndAcceptAnySshHostKey = true,
        //        } :
        //        new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = ConfigurationManager.AppSettings["SFTPHostName"],
        //            UserName = ConfigurationManager.AppSettings["SFTPUserName"],
        //            Password = ConfigurationManager.AppSettings["SFTPPassword"],
        //            PortNumber = int.Parse(ConfigurationManager.AppSettings["SFTPPortNumber"]),
        //            GiveUpSecurityAndAcceptAnySshHostKey = true,
        //        };

        //        //nueva

        //        var session = new Session();
        //        session.SessionLogPath = ConfigurationManager.AppSettings["SuciveBatchPath"] + @"\log.txt";
        //        //session.ExecutablePath = "";
        //        session.Open(sessionOptions); //Attempts to connect to your sFtp site
        //        //Get Ftp File
        //        TransferOptions transferOptions = new TransferOptions(); 
        //        transferOptions.TransferMode = TransferMode.Binary; //The Transfer Mode - 
        //        //<em style="font-size: 9pt;">Automatic, Binary, or Ascii  
        //        transferOptions.FilePermissions = null; //Permissions applied to remote files; 
        //        //null for default permissions.  Can set user, 
        //        //Group, or other Read/Write/Execute permissions. 
        //        transferOptions.PreserveTimestamp = false; //Set last write time of 
        //        //destination file to that of source file - basically change the timestamp 
        //        //to match destination and source files.   
        //        transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
        //        //the parameter list is: local Path, Remote Path, Delete source file?, transfer Options  
        //        var transferResult = session.PutFiles(filePath, fileName, false, transferOptions);  //C:\inetpub\batchSistarbanc\

        //        //Throw on any error 
        //        transferResult.Check();
        //        //Log information and break out if necessary  
        //    }
        //    catch (Exception exception)
        //    {
        //        NLogLogger.LogEvent(exception);
        //        throw exception;
        //    }
        //}
        //private int GetDeptoFromService(PaymentDto dto)
        //{
        //    var service = _serviceService.GetById(dto.ServiceId);
        //    var dtpo = (int)service.Departament;

        //    return dtpo;
        //}
        #endregion

        private int GetDeptoFromService(ServiceAssociatedDto dto)
        {
            var service = _serviceService.GetById(dto.ServiceId);
            var dtpo = (int)service.Departament;

            return dtpo;
        }

        private void GeneralNotification(NotificationPruposeDto notidto, ServiceAssociatedDto serviceAssociatedDto, string message, Exception e, string logerMsg)
        {
            try
            {
                if (serviceAssociatedDto != null)
                {

                    _serviceNotificationMessage.SendGeneralNotification(serviceAssociatedDto.RegisteredUserDto, "Pago programado no realizado", message);

                    _loggerService.CreateLog(LogType.Info, LogOperationType.ServiceAssociatedPaymentException, LogCommunicationType.VisaNet,
                        serviceAssociatedDto.RegisteredUserDtoId, e != null ? e.Message : "", message);

                    _serviceNotification.Create(new NotificationDto
                    {
                        Date = DateTime.Now,
                        NotificationPrupose = notidto,
                        Message = message,
                        RegisteredUserId = serviceAssociatedDto.RegisteredUserDtoId,
                        ServiceId = serviceAssociatedDto.ServiceId,
                    });
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Error, "Pago programado. associatedDto == NULL");
                }

                if (e != null)
                {
                    var parameters = _repositoryParameters.AllNoTracking().First();
                    var title = "ERROR EN PAGO PROGRAMADO";
                    var user = new
                                                                                                                  {
                                                                                                                      serviceAssociatedDto.RegisteredUserDto.Name,
                                                                                                                      serviceAssociatedDto.RegisteredUserDto.Surname,
                                                                                                                      serviceAssociatedDto.RegisteredUserDto.Email,
                    };

                    _serviceNotificationMessage.SendInternalErrorNotification(parameters, title, user, message, e.Message, e.StackTrace, e.InnerException);
                }

                if (serviceAssociatedDto != null)
                {
                    var parameters = _repositoryParameters.AllNoTracking().First();
                    var title = "ERROR EN PAGO PROGRAMADO";
                    var user = new
                            {
                                serviceAssociatedDto.RegisteredUserDto.Name,
                                serviceAssociatedDto.RegisteredUserDto.Surname,
                                serviceAssociatedDto.RegisteredUserDto.Email,
                    };

                    _serviceNotificationMessage.SendInternalErrorNotification(parameters, title, user, message, string.Empty, string.Empty, null);

                }

                if (!String.IsNullOrEmpty(logerMsg) && serviceAssociatedDto != null)
                {
                    _loggerService.CreateLog(LogType.Info, LogOperationType.ServiceAssociatedBadBinNumber, LogCommunicationType.VisaNet, serviceAssociatedDto.RegisteredUserDtoId, logerMsg, message);
                }
                
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "GeneralNotification exception");
                NLogLogger.LogEvent(exception);
            }

        }

        public void SuciveReverse()
        {
            //ISucive10 _sucive10 = NinjectRegister.Get<ISucive10>();
            //_sucive10.ReversoPagoB();
        }

        private bool IsServiceMomentarilyDisabled(Guid serviceId)
        {
            try
            {
                var fileName = ConfigurationManager.AppSettings["ServiceIdMomentarilyDisabled"];

                using (var r = new StreamReader(fileName))
                {
                    string json = r.ReadToEnd();
                    var services = JsonConvert.DeserializeObject<IEnumerable<ServiceOfflineDto>>(json);
                    var today = DateTime.Now;

                    var servicesOffline =
                        services.Any(
                            x =>
                                today >= x.DateFrom && today <= x.DateTo &&
                                x.Ids.Any(
                                    y => y == serviceId));

                    if (servicesOffline)
                        return true;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en metodo ServiceBill - IsServiceMomentarilyDisabled");
                NLogLogger.LogEvent(e);
            }
            return false;
        }
    }
}
