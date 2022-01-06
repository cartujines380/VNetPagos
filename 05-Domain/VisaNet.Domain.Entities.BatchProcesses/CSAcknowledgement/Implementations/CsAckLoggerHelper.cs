using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Implementations
{
    public class CsAckLoggerHelper : ICsAckLoggerHelper
    {
        //Process CS Post
        public void LogCSPostStarted(CyberSourceAcknowledgementDto post, LogPlatform platform)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - Process - Inicia procesamiento del POST de CyberSource");
        }

        public void LogCSPostRecieved(CyberSourceAcknowledgementDto post, LogPlatform platform)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - Process - Se inserta el registro recibido del POST de CyberSource");
        }

        public void LogCSPostException(Exception e, LogPlatform platform)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - Process - Error en el procesamiento del POST de CyberSource");
        }

        //Void Payments
        public void LogVoidProcessStarted()
        {
            NLogLogger.LogEvent(NLogType.Info, "ConsoleApplication.CSAcknowledgement - Inicio de corrida del proceso de Voids");
        }

        public void LogTransactionIds(ICollection<CyberSourceAcknowledgementDto> cyberSourceAcknowledgements)
        {
            var strIds = string.Format("Listado de transacciones obtenidas: {0}", string.Join(",", cyberSourceAcknowledgements.Select(x => x.TransactionId)));
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - " + strIds);
        }

        public void LogVoidProcessFinished()
        {
            NLogLogger.LogEvent(NLogType.Info, "ConsoleApplication.CSAcknowledgement - Fin de corrida del proceso de Voids");
        }

        public void LogVoidProcessException(Exception e)
        {
            NLogLogger.LogEvent(NLogType.Info, "ConsoleApplication.CSAcknowledgement - Error en corrida del proceso de Voids");
        }

        public void LogExecuteVoidStarted(CancelPayment cancel)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - Se hace el Void para el pago " + cancel.RequestId);
        }

        public void LogExecuteVoidResult(CyberSourceOperationData result, string requestId)
        {
            var cod = result != null && result.VoidData != null ? result.VoidData.PaymentResponseCode.ToString() : string.Empty;
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - Resultado del Void del pago " + requestId + " cod: " + cod);
        }

        public void LogExecuteReversalResult(CyberSourceOperationData result, string requestId)
        {
            var cod = result != null && result.VoidData != null ? result.VoidData.PaymentResponseCode.ToString() : string.Empty;
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - Resultado del reverso del pago " + requestId + " cod: " + cod);
        }

        public void LogExecuteReversalException(Exception e = null)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - Error haciendo el reverso de un CyberSourceAcknowledgement");
        }
        public void LogExecuteVoidException(Exception e = null)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceCyberSourceAcknowledgement - VoidPayments - Error haciendo el Void de un CyberSourceAcknowledgement");
        }
    }
}