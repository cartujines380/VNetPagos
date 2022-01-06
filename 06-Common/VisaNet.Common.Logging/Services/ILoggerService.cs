using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Common.Logging.Services
{
    public interface ILoggerService
    {
        IEnumerable<LogDto> All(Expression<Func<Log, LogDto>> select = null, Expression<Func<Log, bool>> where = null,
            params Expression<Func<Log, object>>[] properties);

        IEnumerable<LogDto> AllNoTracking(Expression<Func<Log, LogDto>> select = null,
            Expression<Func<Log, bool>> where = null, params Expression<Func<Log, object>>[] properties);

        IEnumerable<AuditTransactionLogDto> Transactions(DateTime from, DateTime to,
            Guid? systemUserId, Guid? userId, string ip,
            LogUserType? logUserType, LogOperationType? logOperationType, string user, int startRow, int rowsCount, string message);

        void Create(LogDto entity);

        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, LogUserType userType, Guid userId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null);
        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, LogUserType userType, Guid userId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null);

        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);
        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);

        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid applicationUserId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);
        void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid applicationUserId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);

        void CreateLogForAnonymousUser(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid anonymousUserId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);
        void CreateLogForAnonymousUser(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid anonymousUserId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null);

        IEnumerable<CyberSourceLogDataDto> CybersourceTransactions(DateTime from, DateTime to, string transactionType, string bin);
        IEnumerable<LogPaymentCyberSourceDto> CybersourceTransactionsDetails(DateTime from, DateTime to, string transactionType, string bin);

        void CreateLogWithTemporaryId(Guid temporaryId, LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, string callCenterMessage);
    }
}
