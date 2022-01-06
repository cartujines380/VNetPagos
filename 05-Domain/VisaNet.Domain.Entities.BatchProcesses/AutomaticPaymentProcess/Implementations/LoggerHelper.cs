using System;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class LoggerHelper : ILoggerHelper
    {
        //Process
        public void LogProcessFinished(int runNumber)
        {
            NLogLogger.LogEvent(NLogType.Info, "FIN CORRIDA NÚMERO " + runNumber + " DEL PROCESO PAGO PROGRAMADO Y NOTIFICACION ");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
        }

        public void LogServicesCount(int count)
        {
            Console.WriteLine("Cantidad de servicios obtenidos {0}", count);
            NLogLogger.LogEvent(NLogType.Info, "    Proceso Pago Programado -- Cantidad de servicios: " + count);
        }

        public void LogActualServiceBeingProcessed(int actualServiceNumber)
        {
            Console.WriteLine("Procesando servicio nº {0}", actualServiceNumber);
            NLogLogger.LogEvent(NLogType.Info, "    Proceso Pago Programado -- Procesando servicio nº " + actualServiceNumber);
        }

        public void LogFinishedProcessingServices()
        {
            NLogLogger.LogEvent(NLogType.Info, "    Proceso Pago Programado -- Finaliza procesamiento de servicios");
        }

        //Process runs
        public void LogProcessExecutedSuccessfuly()
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO PROGRAMADO Y NOTIFICACIONES");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
            NLogLogger.LogEvent(NLogType.Info, "EL PROCESO YA FUE EJECUTADO CORRECTAMENTE");
            Console.WriteLine("El proceso ya fue ejecutado correctamente.");
        }

        public void LogProcessReachedMaxTries(int maxTries)
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO PROGRAMADO Y NOTIFICACIONES");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
            NLogLogger.LogEvent(NLogType.Info, "SE ALCANZÓ EL NUMERO MÁXIMO DE INTENTOS (" + maxTries + ") ");
            Console.WriteLine("Se alcanzó el número máximo de intentos (" + maxTries + ")");
        }

        public void LogProcessRunNumber(int timesExecuted)
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO PROGRAMADO Y NOTIFICACIONES");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
            NLogLogger.LogEvent(NLogType.Info, "INTENTO NUMERO: " + (timesExecuted + 1));
            Console.WriteLine("Ejecutando pago programado y notificaciones. Intento número: " + (timesExecuted + 1));
        }

        public void LogNotificationsSingleProcess()
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO NOTIFICACIONES");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
            Console.WriteLine("Ejecutando notificaciones");
        }

        public void LogAutomaticPaymentsSingleProcess(int timesExecuted)
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO PROGRAMADO");
            NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
            Console.WriteLine("Ejecutando pago programado. Intento número: " + (timesExecuted + 1));
        }

        //Service asociated
        public void LogServiceProcessStarted(ServiceAssociatedDto serviceAssociatedDto)
        {
            var msg =
                string.Format(
                    "   Proceso Pago Programado -- INICIA proceso para Servicio: {0} (Ref: {1}), Usuario {2}, " +
                    "Servicio Asociado Id: {3}",
                    serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
                    serviceAssociatedDto.ReferenceNumber,
                    serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                    serviceAssociatedDto.Id);
            NLogLogger.LogEvent(NLogType.Info, msg);
        }

        public void LogServiceProcessFinished(ServiceAssociatedDto serviceAssociatedDto)
        {
            var msg =
                string.Format(
                    "   Proceso Pago Programado -- FINALIZA proceso para Servicio: {0} (Ref: {1}), Usuario {2}, " +
                    "Servicio Asociado Id: {3}",
                    serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
                    serviceAssociatedDto.ReferenceNumber,
                    serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                    serviceAssociatedDto.Id);
            NLogLogger.LogEvent(NLogType.Info, msg);
        }

        public void LogInvalidCardToken(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "No se pudo validar el token de la tarjeta de crédito {2}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                serviceAssociatedDto.DefaultCard.MaskedNumber);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogInvalidCardBin(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "No se pudo validar el bin de la tarjeta de crédito {2}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                serviceAssociatedDto.DefaultCard.MaskedNumber);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogInvalidCardDueDate(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "La tarjeta de crédito {2} se encuentra vencida desde el {3}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                serviceAssociatedDto.DefaultCard.MaskedNumber,
                serviceAssociatedDto.DefaultCard.DueDate.ToString("MM/yyyy"));
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogServiceNotAllowsAutomaticPayment(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "El servicio: {2} no permite pagos programados.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogAutomaticPaymentDisabled(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "El usuario no tiene pago programado configurado para este servicio.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogInvalidPaymentsCount(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "Se excedio la cantidad de pagos permitidos. Pagos realizados {2}, pagos permitidos {3}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                serviceAssociatedDto.AutomaticPaymentDto.QuotasDone,
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogFilteredBillsForServiceAssociate(int filteredBillsCount, Guid serviceAssociatedDtoId)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
                "Cantidad de facturas a pagar: {1}.",
                serviceAssociatedDtoId,
                filteredBillsCount);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogIdPadronForServiceAssociate(Guid serviceAssociatedDtoId, int idPadron)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
                "Id Padron obtenido: {1}.",
                serviceAssociatedDtoId,
                idPadron);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        //Bill
        public void LogBillExceedsQuotas(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "La factura {2} del servicio {3} no se pudo pagar. Se superó la cantidad de pagos configurados ({4}).",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogBillExceedsAmount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "La factura {2} del servicio {3} no se pudo pagar. El monto de la factura ({4}) supera al monto configurado ({5}).",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
                billDto.Amount,
                serviceAssociatedDto.AutomaticPaymentDto.Maximum);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogCalculatingDiscount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                            "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                            "Calculando el descuento para la factura {2}.",
                            serviceAssociatedDto.Id,
                            serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                            billDto.BillExternalId);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogCallingCybersource(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            Console.WriteLine("Realizando pago a través de CyberSource");
            var message = string.Format(
                            "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                            "Llamo a Cybersource para intentar pagar la factura {2}.",
                            serviceAssociatedDto.Id,
                            serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                            billDto.BillExternalId);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogCallCybersourceSuccess(PaymentDto payment, CyberSourceDataDto paymentData)
        {
            Console.WriteLine("Pago realizado");
            Console.WriteLine("Notificando pago de facturas");
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "Se registró el pago en Cybersource, REQUEST ID: {2}. Se debe notificar a la pasarela.",
                payment.ServiceAssociatedId,
                payment.RegisteredUserId,
                paymentData.TransactionId);
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogPayBillSuccess(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "Se pagó correctamente la factura {2} del servicio {3}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogPayBillInvalidModel(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "No se pudo pagar la factura {2} del servicio {3}. Modelo inválido para cálculo del descuento.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogPayBillBankDontAllowQuota(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "No se pudo pagar la factura {2} del servicio {3}. El banco no permite el pago de esa cuota.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogPayBillBinNotValidForService(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}, Usuario: {1}. " +
                "No se pudo pagar la factura {2} del servicio {3}. El servicio no acepta el tipo de bin {4}.",
                serviceAssociatedDto.Id,
                serviceAssociatedDto.RegisteredUserDto != null ? serviceAssociatedDto.RegisteredUserDto.Email : "NULL",
                billDto.BillExternalId,
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
                serviceAssociatedDto.DefaultCard != null ? serviceAssociatedDto.DefaultCard.MaskedNumber.Substring(0, 6) : "NULL");
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        public void LogGatewayNotificationError(Guid serviceAssociatedDtoId)
        {
            var message = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
                "ERROR - No se pudo notificar a las pasarelas de pago.",
                serviceAssociatedDtoId);
            NLogLogger.LogEvent(NLogType.Info, message);

            Console.WriteLine("No se pudo realizar el pago contra las pasarelas de pago");
        }

        //Exceptions
        public void LogGetBillsHandledException(Exception e, Guid serviceAssociatedDtoId)
        {
            var msg = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
                "ERROR - BillException CAPTURADA - Error al obtener facturas.",
                serviceAssociatedDtoId);

            NLogLogger.LogEvent(NLogType.Info, msg);
            NLogLogger.LogEvent(NLogType.Error, msg);
            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- MENSAJE: " + e.Message);
            NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
            if (e.InnerException != null)
                NLogLogger.LogEvent(NLogType.Error, "       Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
        }

        public void LogGetBillsException(Exception e, Guid serviceAssociatedDtoId)
        {
            var msg = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
                "ERROR - Exception CAPTURADA - Error al obtener facturas.",
                serviceAssociatedDtoId);

            NLogLogger.LogEvent(NLogType.Info, msg);
            NLogLogger.LogEvent(NLogType.Error, msg);
            NLogLogger.LogEvent(NLogType.Error,
                "       Proceso Pago Programado -- MENSAJE: " + e.Message);
            NLogLogger.LogEvent(NLogType.Error,
                "       Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
            if (e.InnerException != null)
            {
                NLogLogger.LogEvent(NLogType.Error,
                    "       Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
            }
        }

        public void LogDiscountCalculationException(Exception e, ServiceAssociatedDto serviceAssociatedDto)
        {
            var msg = string.Format(
               "    Proceso Pago Programado -- Servicio Asociado Id: {0}. " +
               "ERROR - Exception CAPTURADA - Error al obtener descuento para el Servicio {1} y Bin tarjeta {2}.",
               serviceAssociatedDto.Id,
               serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "NULL",
               serviceAssociatedDto.DefaultCard != null ? serviceAssociatedDto.DefaultCard.BIN : 0);

            NLogLogger.LogEvent(NLogType.Info, msg);
            NLogLogger.LogEvent(NLogType.Error, msg);
            NLogLogger.LogEvent(NLogType.Error,
                "       Proceso Pago Programado -- MENSAJE: " + e.Message);
            NLogLogger.LogEvent(NLogType.Error,
                "       Proceso Pago Programado -- STACK TRACE: " + e.StackTrace);
            if (e.InnerException != null)
            {
                NLogLogger.LogEvent(NLogType.Error,
                    "       Proceso Pago Programado -- INNER EXCEPTION: " + e.InnerException);
            }
        }

        public void LogException(Exception e, Guid? serviceAssociatedDtoId = null)
        {
            var msg = "    Proceso Pago Programado -- Exception CAPTURADA - Ha ocurrido un error inesperado.";
            if (serviceAssociatedDtoId != null)
            {
                msg = string.Format(
                "    Proceso Pago Programado -- Servicio Asociado Id: {0}." +
                "ERROR - Exception CAPTURADA - Ha ocurrido un error inesperado.",
                serviceAssociatedDtoId);
            }
            NLogLogger.LogEvent(NLogType.Error, msg);
            NLogLogger.LogEvent(e);
        }

    }
}
