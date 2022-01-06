using System;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class End
    {
        public Guid? WebhookRegistrationId { get; set; }
        public string UrlCallback { get; set; }
        public string OperationId { get; set; }
        public string ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public string AppId { get; set; }
        public string TrnsNumber { get; set; }
    }
}