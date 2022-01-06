namespace VisaNet.Presentation.Administration.Models
{
    public class AuditLogDetailModel
    {
        public string Date { get; set; }
        public string IP { get; set; }
        public string SystemUser { get; set; }
        public string ApplicationUser { get; set; }
        public string AnonymousUser { get; set; }
        public string LogUserType { get; set; }
        public string LogType { get; set; }
        public string LogCommunicationType { get; set; }
        public string LogOperationType { get; set; }
        public string Message { get; set; }
        public bool Registered { get; set; }
    }
}
