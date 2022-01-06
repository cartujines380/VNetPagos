namespace VisaNet.Domain.EntitiesDtos
{
    public class ChangeTrackerExcelDto
    {
        public string Date { get; set; }
        public string Ip { get; set; }
        public string LogType { get; set; }
        public string UserName { get; set; }
        public string TableName { get; set; }
        public string AditionalInfo { get; set; }
        public string ColumnName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
    }
}