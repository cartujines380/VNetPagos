namespace VisaNet.Presentation.Administration.Models
{
    public class SystemVersionsModel
    {
        //Webs
        public string AdminVersion { get; set; }
        public string WebVersion { get; set; }
        public string VonRegisterVersion { get; set; }
        public string VisaNetOnVersion { get; set; }
        public string CustomerSiteVersion { get; set; }

        //Public-APIs
        public string NotificationPublicApiVersion { get; set; }
        public string LifPublicApiVersion { get; set; }

        //Private-APIs
        public string CoreApiVersion { get; set; }
        public string LifApiVersion { get; set; }
        public string CustomerSiteApiVersion { get; set; }

        //Processes
        public string PaymentProcessVersion { get; set; }
        public string NotificationProcessVersion { get; set; }
        public string ConciliationProcessVersion { get; set; }
        public string CsAckProcessVersion { get; set; }
        public string DebitSynchronizationProcessVersion { get; set; }

    }
}