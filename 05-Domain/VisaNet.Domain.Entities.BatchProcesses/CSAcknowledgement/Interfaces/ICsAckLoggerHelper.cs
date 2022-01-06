using System;
using System.Collections.Generic;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Interfaces
{
    public interface ICsAckLoggerHelper
    {
        //Process CS Post
        void LogCSPostStarted(CyberSourceAcknowledgementDto post, LogPlatform platfrom);
        void LogCSPostRecieved(CyberSourceAcknowledgementDto post, LogPlatform platfrom);
        void LogCSPostException(Exception exception, LogPlatform platfrom);

        //Void Payments
        void LogVoidProcessStarted();
        void LogTransactionIds(ICollection<CyberSourceAcknowledgementDto> cyberSourceAcknowledgements);
        void LogVoidProcessFinished();
        void LogVoidProcessException(Exception exception);
        void LogExecuteVoidStarted(CancelPayment cancel);
        void LogExecuteVoidResult(CyberSourceOperationData result, string requestId);
        void LogExecuteVoidException(Exception exception = null);
        void LogExecuteReversalResult(CyberSourceOperationData result, string requestId);
        void LogExecuteReversalException(Exception e = null);

    }
}
