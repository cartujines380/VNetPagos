using System;

namespace VisaNet.Common.Logging.NLog
{
    public class GraylogCommonData
    {
        public string UserEmail { get; set; }
        public Guid TransactionId { get; set; }
        public string UserIP { get; set; }
        public Guid TraceId { get; set; }
        public string CybersourceId { get; set; }
        public string MerchantId { get; set; }
    }
}