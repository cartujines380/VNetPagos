namespace VisaNet.Domain.EntitiesDtos.ChangeTracker
{
    public class AuditLogDetailDto
    {
        
        public int Id { get; set; }
        
        public string ColumnName { get; set; }

        public string OrginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual int AuditLogId { get; set; }
        
        public virtual AuditLogDto Log { get; set; }
    }
}