namespace VisaNet.WebApi.Models
{
    public class MailgunBounce: MailgunResponse
    {
        public string Error { get; set; }
        public string Notification { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Tag { get; set; }
        public string MailingList { get; set; }
    }
}