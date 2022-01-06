namespace VisaNet.Domain.EntitiesDtos
{
    public class ResetPasswordFromTokenDto
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
