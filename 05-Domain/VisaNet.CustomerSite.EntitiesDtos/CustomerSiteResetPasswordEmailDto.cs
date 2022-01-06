namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class ResetPasswordEmailDto : BasicInfoForUserEmails
    {
        public string CommerceName { get; set; }
        public string ResetPasswordToken { get; set; }
    }

    public class NewUserEmailDto : BasicInfoForUserEmails
    {
        public string CommerceName { get; set; }
        public string ConfirmationPasswordToken { get; set; }
    }

    public abstract class BasicInfoForUserEmails
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public string BaseUrl { get; set; } //se agarra del web.config
    }
}