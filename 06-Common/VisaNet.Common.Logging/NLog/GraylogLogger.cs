using System;
using System.Configuration;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using VisaNet.Common.Logging.Resources;
using System.Threading.Tasks;

namespace VisaNet.Common.Logging.NLog
{
    public class GraylogLogger
    {
        private static readonly string GraylogUri = ConfigurationManager.AppSettings["GraylogApiUri"];
        private static readonly bool EnabledSaveSecundary = bool.Parse(ConfigurationManager.AppSettings["GraylogEnabledSaveSecundary"]);

        public static void LogEvent(GraylogLog log)
        {
            Task.Run(() => LogUsingGraylog(log));

            if (EnabledSaveSecundary)
            {
                log.ShortMessage = (log.GraylogTraceId == Guid.Empty) ? log.ShortMessage : string.Format("{0} | LogTraceId: {1}", log.ShortMessage, log.GraylogTraceId);
                LogUsingNLog(log);
            }
        }

        private static void LogUsingGraylog(GraylogLog log)
        {
            if (string.IsNullOrEmpty(GraylogUri)) return; //Si no se quiere loguear en Graylog hay que dejar la Url vacía

            try
            {
                var client = new RestClient(GraylogUri);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json; charset=utf-8");

                var data = new
                {
                    version = ConfigurationManager.AppSettings["GraylogVersion"],
                    host = LogStrings.ResourceManager.GetString(log.Host.ToString()),
                    //timestamp se deja vacío para que lo tome la del server
                    level = Enum.GetName(typeof(NLogType), log.Level),
                    environment = ConfigurationManager.AppSettings["GraylogEnvironment"],
                    operation_type = Enum.GetName(typeof(OperationType), log.OperationType),

                    log_trace_id = log.GraylogTraceId != Guid.Empty ? log.GraylogTraceId.ToString() : string.Empty,
                    merchant_id = log.CommonData.MerchantId,
                    user_ip = log.CommonData.UserIP,
                    user_email = log.CommonData.UserEmail,
                    transaction_id = log.CommonData.TransactionId != Guid.Empty ? log.CommonData.TransactionId.ToString() : string.Empty,
                    cybersource_id = log.CommonData.CybersourceId,

                    short_message = log.ShortMessage,
                    full_data = JsonConvert.SerializeObject(log.Data),
                };

                request.AddJsonBody(data);

                client.ExecuteAsync(request, (x) =>
                {
                    LogGraylogErrorUsingNLog(log, x.ErrorMessage);
                });
            }
            catch (Exception e)
            {
                LogGraylogErrorUsingNLog(log, e.Message);
            }
        }

        private static void LogUsingNLog(GraylogLog log)
        {
            var logger = LogManager.GetLogger(log.Host.ToString());
            switch (log.Level)
            {
                case NLogType.Fatal:
                    logger.Fatal(log.ShortMessage);
                    break;
                case NLogType.Error:
                    logger.Error(log.ShortMessage);
                    break;
                case NLogType.Warn:
                    logger.Warn(log.ShortMessage);
                    break;
                case NLogType.Info:
                    logger.Info(log.ShortMessage);
                    break;
                case NLogType.Debug:
                    logger.Debug(log.ShortMessage);
                    break;
                case NLogType.Trace:
                    logger.Trace(log.ShortMessage);
                    break;
                default:
                    logger.Info(log.ShortMessage);
                    break;
            }
        }

        private static void LogGraylogErrorUsingNLog(GraylogLog originalLog, string errorMessage)
        {
            //Log the original Log to txt if it wasn't already logged
            if (!EnabledSaveSecundary)
            {
                LogUsingNLog(originalLog);
            }

            //Log the error message Log to txt
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var errorLog = new GraylogLog
                {
                    Level = NLogType.Error,
                    Host = originalLog.Host,
                    ShortMessage = string.Format("Graylog Error: {0}", errorMessage)
                };
                LogUsingNLog(errorLog);
            }
        }

    }
}