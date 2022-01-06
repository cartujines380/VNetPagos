using System;


namespace VisaNet.Common.Logging.Entities
{
    public class LogAnonymousModel
    {
        public Guid AnonymousUserId { get; set; }
        public string Message { get; set; }
        public string CallCenterMessage { get; set; }
        public LogType LogType { get; set; }
        public LogOperationType LogOperationType { get; set; }
        public LogCommunicationType LogCommunicationType { get; set; }
        public CyberSourceLogDataDto CyberSourceLogData { get; set; }
        public CyberSourceVerifyByVisaDataDto CyberSourceVerifyByVisaData { get; set; }
        public Guid? TemporaryId { get; set; }
    }
}
