namespace VisaNet.WebApi.Models
{
    public class MailgunFailure : MailgunResponse
    {
        public string Code { get; set; }
        //public string AttachmentX { get; set; }
        public string Description { get; set; }
    }
}