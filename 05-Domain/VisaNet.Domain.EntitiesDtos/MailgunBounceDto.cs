namespace VisaNet.Domain.EntitiesDtos
{
    public class MailgunBounceDto: MailgunResponseDto
    {
        public string Error { get; set; }
        public string Notification { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Tag { get; set; }
        public string MailingList { get; set; }
    }
}