using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Repository;
using VisaNet.Common.Security;

namespace VisaNet.Common.Logging.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly IWebApiTransactionContext _webApiTransactionContext;

        public LoggerService(ILoggerRepository loggerRepository,
                             IWebApiTransactionContext webApiTransactionContext)
        {
            _loggerRepository = loggerRepository;
            _webApiTransactionContext = webApiTransactionContext;
        }

        public IEnumerable<LogDto> All(Expression<Func<Log, LogDto>> select = null, Expression<Func<Log, bool>> where = null, params Expression<Func<Log, object>>[] properties)
        {
            if (@select != null)
                return _loggerRepository.All(where, properties).Select(select).ToList();

            return _loggerRepository.All(where, properties).Select(l => new LogDto
            {
                Id = l.Id,
                IP = l.IP,
                UserName = l.UserName,
                DateTime = l.DateTime,
                SessionId = l.SessionId,
                ApplicationUserId = l.ApplicationUserId,
                SystemUserId = l.SystemUserId,
                TransactionIdentifier = l.TransactionIdentifier,
                TransactionDateTime = l.TransactionDateTime,

                RequestUri = l.RequestUri,
                LogType = l.LogType,
                LogUserType = l.LogUserType,
                LogCommunicationType = l.LogCommunicationType,
                LogOperationType = l.LogOperationType,
                Message = l.Message,
                ExceptionMessage = l.ExceptionMessage,
                InnerException = l.InnerException,
                AnonymousUserId = l.AnonymousUserId,

                IncludeCallCenterMessage = l.IncludeCallCenterMessage,
                CallCenterMessage = l.CallCenterMessage,

            }).ToList();
        }

        public IEnumerable<LogDto> AllNoTracking(Expression<Func<Log, LogDto>> select = null, Expression<Func<Log, bool>> where = null, params Expression<Func<Log, object>>[] properties)
        {
            if (@select != null)
                return _loggerRepository.AllNoTracking(where, properties).Select(select).ToList();

            return _loggerRepository.AllNoTracking(where, properties).Select(l => new LogDto
            {
                Id = l.Id,
                IP = l.IP,
                UserName = l.UserName,
                DateTime = l.DateTime,
                SessionId = l.SessionId,
                ApplicationUserId = l.ApplicationUserId,
                SystemUserId = l.SystemUserId,
                TransactionIdentifier = l.TransactionIdentifier,
                TransactionDateTime = l.TransactionDateTime,

                RequestUri = l.RequestUri,
                LogType = l.LogType,
                LogUserType = l.LogUserType,
                LogCommunicationType = l.LogCommunicationType,
                LogOperationType = l.LogOperationType,
                Message = l.Message,
                ExceptionMessage = l.ExceptionMessage,
                InnerException = l.InnerException,
                AnonymousUserId = l.AnonymousUserId,

                IncludeCallCenterMessage = l.IncludeCallCenterMessage,
                CallCenterMessage = l.CallCenterMessage,

            }).ToList();
        }


        public IEnumerable<AuditTransactionLogDto> Transactions(DateTime from, DateTime to, Guid? systemUserId, Guid? userId, string ip, LogUserType? logUserType, LogOperationType? logOperationType, string user, int startRow, int rowsCount, string message)
        {

            //var sb = new StringBuilder("WHERE (DATEADD(dd, 0, DATEDIFF(dd, 0, l.TransactionDateTime)) BETWEEN @From AND @To)");
            var sb = new StringBuilder("WHERE (l.TransactionDateTime BETWEEN @From AND @To)");
            if (systemUserId.HasValue) { sb.Append(" AND (l.SystemUserId = @systemUserId) "); }
            if (userId.HasValue) { sb.Append(" AND (l.ApplicationUserId = @userId OR a.AnonymousUserId = @userId) "); }
            if (string.IsNullOrEmpty(ip) == false) { sb.Append(" AND (l.IP = @IP) "); }
            if (logOperationType.HasValue) { sb.Append(" AND (l.LogOperationType = @logOperationType) "); }
            if (logUserType.HasValue) { sb.Append(" AND (l.LogUserType = @logUserType) "); }

            if (!string.IsNullOrEmpty(message)) { sb.Append(" AND (l.Message like '%'+@message+'%') "); }

            var queryUser = string.Empty;

            if (string.IsNullOrEmpty(user) == false)
            {
                queryUser = @" AND ((sysUsrs.LDAPUserName = NULL OR sysUsrs.LDAPUserName like '%'+@user+'%') OR
								  (appUsrs.Email = NULL OR appUsrs.Email like '%'+@user+'%') OR
								  (anonUsrs.Email = NULL OR anonUsrs.Email like '%'+@user+'%')) ";
            }

            var queryWhere = sb.ToString() + queryUser;

            var query = @"
DECLARE @totalRows INTEGER;
SELECT @totalRows = COUNT (0) 
    FROM (
			SELECT l.TransactionIdentifier
					,count(1) as Aa
					,l.TransactionDateTime											  
			FROM Logs l
			LEFT OUTER JOIN LDAPBaseUsers sysUsrs on sysUsrs.Id = l.SystemUserId
			LEFT OUTER JOIN ApplicationUsers appUsrs on appUsrs.Id = l.ApplicationUserId
			LEFT OUTER JOIN AnonymousUsers anonUsrs on anonUsrs.Id = l.AnonymousUserId
			" + queryWhere + @"
			GROUP BY l.TransactionIdentifier
					,l.TransactionDateTime


) a    

									
SELECT   a.RowNum
		,a.TransactionIdentifier
	    ,a.SystemUserId
	    ,a.ApplicationUserId
	    ,a.AnonymousUserId
	    ,a.IP
	    ,a.TransactionDateTime
	    ,a.LDAPUserName
	    ,a.ApplicationUserEmail		
		,a.LogOperationType
        ,a.LogUserType
	    ,a.AnonymousUserEmail
        ,a.LogType
        ,a.LogCommunicationType
        ,a.Message
        ,@totalRows AS TotalRows
FROM (
	SELECT  ROW_NUMBER() OVER ( ORDER BY l.TransactionDateTime DESC) AS RowNum
            ,l.TransactionIdentifier
		    ,max(l.SystemUserId) as SystemUserId
		    ,max(l.ApplicationUserId) as ApplicationUserId
		    ,max(l.AnonymousUserId) as AnonymousUserId
		    ,max(l.IP) as IP
		    ,l.TransactionDateTime as TransactionDateTime	
            ,max(l.LogUserType) as LogUserType
            ,(select top 1 LogOperationType from logs where TransactionIdentifier = l.TransactionIdentifier order by DateTime) AS LogOperationType
		    ,max(sysUsrs.LDAPUserName) as LDAPUserName
		    ,max(appUsrs.Email) AS ApplicationUserEmail
		    ,max(anonUsrs.Email) AS AnonymousUserEmail
            ,max(l.LogType) AS LogType
            ,max(l.Message) AS Message
		    ,max(l.LogCommunicationType) AS LogCommunicationType
	FROM Logs l
	LEFT OUTER JOIN LDAPBaseUsers sysUsrs on sysUsrs.Id = l.SystemUserId
	LEFT OUTER JOIN ApplicationUsers appUsrs on appUsrs.Id = l.ApplicationUserId
	LEFT OUTER JOIN AnonymousUsers anonUsrs on anonUsrs.Id = l.AnonymousUserId
	" + queryWhere + @"GROUP BY l.TransactionIdentifier
		    ,l.TransactionDateTime
) a
WHERE (RowNum > @StartRow AND RowNum <= @StartRow + @RowsCount)
ORDER BY RowNum
";


            var parameters = new List<SqlParameter>
            {
                new SqlParameter("From",from),
                new SqlParameter("To",to),
                new SqlParameter("StartRow",startRow),
                new SqlParameter("RowsCount",rowsCount),
            };

            if (systemUserId.HasValue) { parameters.Add(new SqlParameter("systemUserId", systemUserId.Value)); }
            if (userId.HasValue) { parameters.Add(new SqlParameter("userId", userId.Value)); }
            if (string.IsNullOrEmpty(ip) == false) { parameters.Add(new SqlParameter("IP", ip)); }
            if (logOperationType.HasValue) { parameters.Add(new SqlParameter("logOperationType", logOperationType.Value)); }
            if (logUserType.HasValue) { parameters.Add(new SqlParameter("logUserType", logUserType.Value)); }
            if (string.IsNullOrEmpty(user) == false) { parameters.Add(new SqlParameter("user", "%" + user + "%")); }
            if (string.IsNullOrEmpty(message) == false) { parameters.Add(new SqlParameter("message", "%" + message + "%")); }

            var result = _loggerRepository.ExecuteSQL<AuditTransactionLogDto>(query.ToString(), parameters.ToArray()).ToList();

            return result;
        }

        public void Create(LogDto entity)
        {
            var newLog = new Log
            {
                Id = entity.Id,
                IP = entity.IP,
                UserName = entity.UserName,
                DateTime = entity.DateTime,
                SessionId = entity.SessionId,
                ApplicationUserId = entity.ApplicationUserId,
                AnonymousUserId = entity.AnonymousUserId,
                SystemUserId = entity.SystemUserId,
                TransactionIdentifier = entity.TransactionIdentifier,
                TransactionDateTime = entity.TransactionDateTime,

                RequestUri = entity.RequestUri,
                LogType = entity.LogType,
                LogOperationType = entity.LogOperationType,
                LogUserType = entity.LogUserType,
                LogCommunicationType = entity.LogCommunicationType,
                Message = entity.Message,

                IncludeCallCenterMessage = entity.IncludeCallCenterMessage,
                CallCenterMessage = entity.CallCenterMessage,

                ExceptionMessage = (entity.Exception != null) ? entity.Exception.Message : string.Empty,
                InnerException = (entity.Exception != null && entity.Exception.InnerException != null) ? entity.Exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (entity.LogPaymentCyberSource != null) ? new LogPaymentCyberSource
                {
                    Id = entity.LogPaymentCyberSource.Id,
                    TransactionIdentifier = entity.TransactionIdentifier,
                    TransactionDateTime = entity.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(entity.LogPaymentCyberSource.CyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(entity.LogPaymentCyberSource.CyberSourceVerifyByVisaData),
                    TransactionType = entity.TransactionType
                } : null,
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();
        }

        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, LogUserType userType, Guid userId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null)
        {
            if (userType == LogUserType.Registered)
            {
                CreateLog(logType, logOperationType, logCommunicationType, userId, message, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
            if (userType == LogUserType.NoRegistered)
            {
                CreateLogForAnonymousUser(logType, logOperationType, logCommunicationType, userId, message, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
            if (userType == LogUserType.Other)
            {
                CreateLog(logType, logOperationType, logCommunicationType, message, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
        }

        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, LogUserType userType, Guid userId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null)
        {
            if (userType == LogUserType.Registered)
            {
                CreateLog(logType, logOperationType, logCommunicationType, userId, message, callCenterMessage, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
            if (userType == LogUserType.NoRegistered)
            {
                CreateLogForAnonymousUser(logType, logOperationType, logCommunicationType, userId, message, callCenterMessage, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
            if (userType == LogUserType.Other)
            {
                CreateLog(logType, logOperationType, logCommunicationType, message, callCenterMessage, exception, cyberSourceLogData, cyberSourceVerifyByVisaData);
            }
        }

        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var logUserType = LogUserType.Other;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) || (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)))
                logUserType = LogUserType.Registered;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)))
                logUserType = LogUserType.NoRegistered;

            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = logUserType,
                LogCommunicationType = logCommunicationType,
                Message = message,

                ApplicationUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) ? Guid.Parse(_webApiTransactionContext.ApplicationUserId) : (Guid?)null,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,
                AnonymousUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)) ? Guid.Parse(_webApiTransactionContext.AnonymousUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,
                InnerException = (exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }
        
        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var logUserType = LogUserType.Other;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) || (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)))
                logUserType = LogUserType.Registered;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)))
                logUserType = LogUserType.NoRegistered;

            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = logUserType,
                LogCommunicationType = logCommunicationType,
                Message = message,

                IncludeCallCenterMessage = (string.IsNullOrEmpty(callCenterMessage) == false),
                CallCenterMessage = callCenterMessage,

                ApplicationUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) ? Guid.Parse(_webApiTransactionContext.ApplicationUserId) : (Guid?)null,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,
                AnonymousUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)) ? Guid.Parse(_webApiTransactionContext.AnonymousUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,
                InnerException = (exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,

            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }

        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid applicationUserId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var logUserType = LogUserType.Registered;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) || (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)))
                logUserType = LogUserType.Registered;

            if (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId))
                logUserType = LogUserType.NoRegistered;

            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = logUserType,
                LogCommunicationType = logCommunicationType,
                Message = message,

                ApplicationUserId = applicationUserId,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,
                AnonymousUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)) ? Guid.Parse(_webApiTransactionContext.AnonymousUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,
                InnerException = (exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }

        public void CreateLog(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid applicationUserId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var logUserType = LogUserType.Registered;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) || (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)))
                logUserType = LogUserType.Registered;

            if (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId))
                logUserType = LogUserType.NoRegistered;

            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = logUserType,
                LogCommunicationType = logCommunicationType,
                Message = message,

                IncludeCallCenterMessage = (string.IsNullOrEmpty(callCenterMessage) == false),
                CallCenterMessage = callCenterMessage,

                ApplicationUserId = applicationUserId,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,
                AnonymousUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)) ? Guid.Parse(_webApiTransactionContext.AnonymousUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,
                InnerException = (exception != null) ? exception.StackTrace : string.Empty,//(exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }

        public void CreateLogForAnonymousUser(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid anonymousUserId, string message, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = LogUserType.NoRegistered,
                LogCommunicationType = logCommunicationType,
                Message = message,

                AnonymousUserId = anonymousUserId,
                ApplicationUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) ? Guid.Parse(_webApiTransactionContext.ApplicationUserId) : (Guid?)null,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,
                InnerException = (exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,

                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }

        public void CreateLogForAnonymousUser(LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, Guid anonymousUserId, string message, string callCenterMessage, Exception exception = null, CyberSourceLogDataDto cyberSourceLogData = null, CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData = null, Guid? linkingTransactionIdentifier = null)
        {
            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = LogUserType.NoRegistered,
                LogCommunicationType = logCommunicationType,
                Message = message,

                IncludeCallCenterMessage = (string.IsNullOrEmpty(callCenterMessage) == false),
                CallCenterMessage = callCenterMessage,

                AnonymousUserId = anonymousUserId,
                ApplicationUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) ? Guid.Parse(_webApiTransactionContext.ApplicationUserId) : (Guid?)null,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,

                ExceptionMessage = (exception != null) ? exception.Message : string.Empty,

                InnerException = (exception != null && exception.InnerException != null) ? exception.InnerException.Message : string.Empty,
                LogPaymentCyberSource = (cyberSourceLogData != null || cyberSourceVerifyByVisaData != null) ? new LogPaymentCyberSource
                {
                    TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                    TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                    CyberSourceLogData = LoadCyberSourceLogData(cyberSourceLogData),
                    CyberSourceVerifyByVisaData = LoadCyberSourceVerifyByVisaData(cyberSourceVerifyByVisaData),
                    TransactionType = cyberSourceLogData.TransactionType,
                    PaymentPlatform = cyberSourceLogData.PaymentPlatform
                } : null,
            };


            _loggerRepository.Create(newLog);
            _loggerRepository.Save();

            UpdateLinkedLogs(linkingTransactionIdentifier, newLog);
        }

        private CyberSourceLogData LoadCyberSourceLogData(CyberSourceLogDataDto cyberSourceLogData)
        {
            return (cyberSourceLogData != null)
                ? new CyberSourceLogData
                {
                    Decision = cyberSourceLogData.Decision,
                    ReasonCode = cyberSourceLogData.ReasonCode,
                    Message = cyberSourceLogData.Message,
                    TransactionId = cyberSourceLogData.TransactionId,
                    BillTransRefNo = cyberSourceLogData.BillTransRefNo,
                    PaymentToken = cyberSourceLogData.PaymentToken,

                    AuthResponse = cyberSourceLogData.AuthResponse,
                    AuthCode = cyberSourceLogData.AuthCode,
                    AuthAmount = cyberSourceLogData.AuthAmount,
                    AuthAvsCode = cyberSourceLogData.AuthAvsCode,
                    AuthTime = cyberSourceLogData.AuthTime,
                    AuthTransRefNo = cyberSourceLogData.AuthTransRefNo,

                    ReqAmount = cyberSourceLogData.ReqAmount,
                    ReqCardExpiryDate = cyberSourceLogData.ReqCardExpiryDate,
                    ReqCardNumber = cyberSourceLogData.ReqCardNumber,
                    ReqCardType = cyberSourceLogData.ReqCardType,
                    ReqCurrency = cyberSourceLogData.ReqCurrency,
                    ReqPaymentMethod = cyberSourceLogData.ReqPaymentMethod,
                    ReqProfileId = cyberSourceLogData.ReqProfileId,
                    ReqReferenceNumber = cyberSourceLogData.ReqReferenceNumber,
                    ReqTransactionType = cyberSourceLogData.ReqTransactionType,
                    ReqTransactionUuid = cyberSourceLogData.ReqTransactionUuid,

                }
                : new CyberSourceLogData();
        }

        private CyberSourceVerifyByVisaData LoadCyberSourceVerifyByVisaData(CyberSourceVerifyByVisaDataDto cyberSourceVerifyByVisaData)
        {
            return (cyberSourceVerifyByVisaData != null)
                ? new CyberSourceVerifyByVisaData
                {
                    PayerAuthenticationXid = cyberSourceVerifyByVisaData.PayerAuthenticationXid,
                    PayerAuthenticationEci = cyberSourceVerifyByVisaData.PayerAuthenticationEci,
                    PayerAuthenticationCavv = cyberSourceVerifyByVisaData.PayerAuthenticationCavv,
                    PayerAuthenticationProofXml = cyberSourceVerifyByVisaData.PayerAuthenticationProofXml,
                }
                : new CyberSourceVerifyByVisaData();
        }

        public IEnumerable<CyberSourceLogDataDto> CybersourceTransactions(DateTime from, DateTime to, string transactionType, string bin)
        {
            var query = _loggerRepository.All(l =>
                (l.LogPaymentCyberSource != null) &&
                (from == default(DateTime) || l.LogPaymentCyberSource.TransactionDateTime > from) &&
                (to == default(DateTime) || l.LogPaymentCyberSource.TransactionDateTime < to) &&
                (
                    (!string.IsNullOrEmpty(transactionType) && l.LogPaymentCyberSource.CyberSourceLogData.ReqTransactionType == transactionType) ||
                    (string.IsNullOrEmpty(transactionType) && string.IsNullOrEmpty(l.LogPaymentCyberSource.CyberSourceLogData.ReqTransactionType) && l.LogPaymentCyberSource.CyberSourceLogData.ReasonCode == "100")
                ) &&
                (
                    (string.IsNullOrEmpty(bin)) ||
                    ((!string.IsNullOrEmpty(bin)) && l.LogPaymentCyberSource.CyberSourceLogData.ReqCardNumber.Substring(0, 6) == bin)
                )).Select(l => new CyberSourceLogDataDto
                {
                    ReasonCode = l.LogPaymentCyberSource.CyberSourceLogData.ReasonCode,
                    TransactionType = l.LogPaymentCyberSource.TransactionType
                });

            return query.ToList();
        }

        public IEnumerable<LogPaymentCyberSourceDto> CybersourceTransactionsDetails(DateTime from, DateTime to, string transactionType, string bin)
        {
            return _loggerRepository.All(l => l.LogPaymentCyberSource != null &&
                                              (from == default(DateTime) || l.LogPaymentCyberSource.TransactionDateTime > from) &&

                                              (to == default(DateTime) || l.LogPaymentCyberSource.TransactionDateTime < to) &&

                                              ((!String.IsNullOrEmpty(transactionType) && l.LogPaymentCyberSource.CyberSourceLogData.ReqTransactionType == transactionType) ||
                                               (String.IsNullOrEmpty(transactionType) && String.IsNullOrEmpty(l.LogPaymentCyberSource.CyberSourceLogData.ReqTransactionType) && l.LogPaymentCyberSource.CyberSourceLogData.ReasonCode == "100")) &&

                                               ((String.IsNullOrEmpty(bin)) || ((!String.IsNullOrEmpty(bin)) && l.LogPaymentCyberSource.CyberSourceLogData.ReqCardNumber.Substring(0, 6) == bin))
                                               ).Select(l => new LogPaymentCyberSourceDto
                                              {
                                                  TransactionDateTime = l.LogPaymentCyberSource.TransactionDateTime,
                                                  CyberSourceLogData = new CyberSourceLogDataDto
                                                  {
                                                      Decision = l.LogPaymentCyberSource.CyberSourceLogData.Decision,
                                                      ReasonCode = l.LogPaymentCyberSource.CyberSourceLogData.ReasonCode,
                                                      TransactionId = l.LogPaymentCyberSource.CyberSourceLogData.TransactionId,
                                                      Message = l.LogPaymentCyberSource.CyberSourceLogData.Message,
                                                      ReqCardNumber = l.LogPaymentCyberSource.CyberSourceLogData.ReqCardNumber,
                                                      ReqCardExpiryDate = l.LogPaymentCyberSource.CyberSourceLogData.ReqCardExpiryDate,
                                                      ReqTransactionType = l.LogPaymentCyberSource.CyberSourceLogData.ReqTransactionType,
                                                      ReqCurrency = l.LogPaymentCyberSource.CyberSourceLogData.ReqCurrency,
                                                      ReqAmount = l.LogPaymentCyberSource.CyberSourceLogData.ReqAmount,
                                                      TransactionType = l.LogPaymentCyberSource.TransactionType
                                                  }
                                              }).ToList();
        }

        public void CreateLogWithTemporaryId(Guid temporaryId, LogType logType, LogOperationType logOperationType, LogCommunicationType logCommunicationType, string message, string callCenterMessage)
        {
            var logUserType = LogUserType.Other;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) || (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)))
                logUserType = LogUserType.Registered;

            if ((!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)))
                logUserType = LogUserType.NoRegistered;

            var newLog = new Log
            {
                IP = _webApiTransactionContext.IP,
                UserName = _webApiTransactionContext.UserName,
                TransactionIdentifier = _webApiTransactionContext.TransactionIdentifier,
                TransactionDateTime = _webApiTransactionContext.TransactionDateTime,
                RequestUri = _webApiTransactionContext.RequestUri,

                DateTime = DateTime.Now,
                LogType = logType,
                LogOperationType = logOperationType,
                LogUserType = logUserType,
                LogCommunicationType = logCommunicationType,
                Message = message,

                IncludeCallCenterMessage = (string.IsNullOrEmpty(callCenterMessage) == false),
                CallCenterMessage = callCenterMessage,

                ApplicationUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.ApplicationUserId)) ? Guid.Parse(_webApiTransactionContext.ApplicationUserId) : (Guid?)null,
                SystemUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.SystemUserId)) ? Guid.Parse(_webApiTransactionContext.SystemUserId) : (Guid?)null,
                AnonymousUserId = (!string.IsNullOrEmpty(_webApiTransactionContext.AnonymousUserId)) ? Guid.Parse(_webApiTransactionContext.AnonymousUserId) : (Guid?)null,
                TemporaryId = temporaryId
            };

            _loggerRepository.Create(newLog);
            _loggerRepository.Save();
        }

        private void UpdateLinkedLogs(Guid? linkingTransactionIdentifier, Log newLog)
        {
            if (!linkingTransactionIdentifier.HasValue) return;

            var logToUpdate = _loggerRepository.All(x => x.TemporaryId == linkingTransactionIdentifier.Value).FirstOrDefault();

            if (logToUpdate != null)
            {
                _loggerRepository.SetTrackingStatus(true);
                logToUpdate.TransactionIdentifier = newLog.TransactionIdentifier;
                _loggerRepository.Save();
                _loggerRepository.SetTrackingStatus(false);
            }
        }

    }
}