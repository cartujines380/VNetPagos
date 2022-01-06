namespace VisaNet.Domain.EntitiesDtos
{
    public class AuditExcelDto
    {
        public string Date { get; set; }
        public string Ip { get; set; }
        public string UserType { get; set; }
        public string OperationType { get; set; }
        public string SystemUser { get; set; }
        public string ApplicationUser { get; set; }
        public string AnonymousUser { get; set; }
        public string Message { get; set; }
        public string LogType { get; set; }
        public string LogCommunicationType { get; set; }
    }
}