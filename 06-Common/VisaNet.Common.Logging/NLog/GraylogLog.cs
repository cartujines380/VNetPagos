using System;

namespace VisaNet.Common.Logging.NLog
{
    public class GraylogLog
    {
        public string Version { get; set; }
        public LogPlatform Host { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public NLogType Level { get; set; }
        public OperationType OperationType { get; set; }

        public Guid GraylogTraceId { get; set; }
        public GraylogCommonData CommonData { get; set; }

        public object Data { get; set; }

    }
}