using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.FrameworkExtensions;

namespace VisaNet.Common.Logging.NLog
{
    public enum NLogType
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Trace
    }

    public static class NLogLogger
    {
        #region Common

        public static void LogEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Default, e, operationType, commonData);
        }

        public static void LogEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Default, type, message, operationType, commonData, additionalData);
        }

        public static void LogEventTrace(string message, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            var objectData = commonData == null ? new GraylogCommonData() : commonData.ToObject<GraylogCommonData>();
            GraylogLogger.LogEvent(new GraylogLog
            {
                Host = LogPlatform.Default,
                Level = NLogType.Trace,
                ShortMessage = message,
                OperationType = operationType ?? OperationType.Unknown,
                CommonData = objectData,
                GraylogTraceId = objectData.TraceId,
            });
        }

        #endregion Common

        #region Highway
        public static void LogHighwayEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Highway, e, operationType, commonData);
        }

        public static void LogHighwayEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Highway, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Extract
        public static void LogExtractEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Extract, e, operationType, commonData);
        }

        public static void LogExtractEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Extract, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Geocom
        public static void LogGeocomEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Geocom, e, operationType, commonData);
        }

        public static void LogGeocomEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Geocom, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Sucive
        public static void LogSuciveEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Sucive, e, operationType, commonData);
        }

        public static void LogSuciveEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Sucive, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Sistarbanc
        public static void LogSistarbancEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Sistarbanc, e, operationType, commonData);
        }

        public static void LogSistarbancEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Sistarbanc, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Tc33
        public static void LogTc33Event(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Tc33, e, operationType, commonData);
        }

        public static void LogTc33Event(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Tc33, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region HighwayFileProcess
        public static void LogHighwayFileProccessEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.HighwayFileProccess, e, operationType, commonData);
        }

        public static void LogHighwayFileProccessEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.HighwayFileProccess, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Apps
        public static void LogAppsEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.Apps, e, operationType, commonData);
        }

        public static void LogAppsEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.Apps, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region ErrorCs
        public static void LogErrorCsEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.ErrorCs, e, operationType, commonData);
        }

        public static void LogErrorCsEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.ErrorCs, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region EmailNotification
        public static void LogEmailNotificationEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.EmailNotification, e, operationType, commonData);
        }

        public static void LogEmailNotificationEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.EmailNotification, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Debit
        public static void LogDebitEvent(Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            LogException(LogPlatform.VisaNet, e, operationType, commonData);
        }

        public static void LogDebitEvent(NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            LogMessage(LogPlatform.VisaNet, type, message, operationType, commonData, additionalData);
        }
        #endregion

        #region Private methods
        private static void LogException(LogPlatform host, Exception e, OperationType? operationType = null, Dictionary<string, object> commonData = null)
        {
            var objectData = commonData == null ? new GraylogCommonData() : commonData.ToObject<GraylogCommonData>();

            GraylogLogger.LogEvent(new GraylogLog
            {
                Host = host,
                Level = NLogType.Error,
                ShortMessage = e.Message,
                OperationType = operationType ?? OperationType.Unknown,
                Data = new
                {
                    ExceptionType = e.GetType().Name,
                    ExceptionTrace = e.StackTrace,
                    ExceptionTargetSite = e.TargetSite
                },
                GraylogTraceId = objectData.TraceId,
                CommonData = objectData,
            });
        }

        private static void LogMessage(LogPlatform host, NLogType type, string message, OperationType? operationType = null, Dictionary<string, object> commonData = null, object additionalData = null)
        {
            var objectData = commonData == null ? new GraylogCommonData() : commonData.ToObject<GraylogCommonData>();

            //TODO: esto no se usa mas por ahora, porque los datos de objectData ya no se loguean en Data, sino que tienen sus propios tags
            //dynamic data;
            //if (additionalData == null)
            //{
            //    data = objectData;
            //}
            //else
            //{
            //    data = ObjectExtensions.Merge(objectData, additionalData);
            //}

            GraylogLogger.LogEvent(new GraylogLog
            {
                Host = host,
                Level = type,
                ShortMessage = message,
                OperationType = operationType ?? OperationType.Unknown,
                Data = additionalData,
                GraylogTraceId = objectData.TraceId,
                CommonData = objectData,
            });
        }
        #endregion
    }
}