namespace VisaNet.Domain.EntitiesDtos
{
    public class MailgunFailureDto : MailgunResponseDto
    {
        public string Code { get; set; }
        //public string AttachmentX { get; set; }
        public string Description { get; set; }
    }
}