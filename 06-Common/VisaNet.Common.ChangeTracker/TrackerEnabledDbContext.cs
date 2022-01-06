using System.Data.Entity;
using System.Linq;
using VisaNet.Common.ChangeTracker.Models;
using VisaNet.Common.Security;

namespace TrackerEnabledDbContext
{
    public class TrackerContext : DbContext
    {
        private readonly IWebApiTransactionContext _transactionContext;

        public TrackerContext()
            : base()
        {
            _transactionContext = new WebApiTransactionContext(false);
        }

        public TrackerContext(IWebApiTransactionContext transactionContext)
            : base()
        {
            _transactionContext = transactionContext ?? new WebApiTransactionContext(false);
        }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<AuditLogDetail> LogDetails { get; set; }


        public override int SaveChanges()
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (var ent in ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                using (var auditer = new LogAuditor(ent, _transactionContext))
                {
                    var record = auditer.GetLogRecord(_transactionContext.UserName,
                    ent.State == EntityState.Modified ? EventType.Modified : EventType.Deleted, this);
                    if (record != null)
                    {
                        AuditLog.Add(record);
                    }
                }
            }

            var addedEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            var result = base.SaveChanges();
            //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

            // Get all Added entities
            foreach (var ent in addedEntries)
            {
                using (var auditer = new LogAuditor(ent, _transactionContext))
                {
                    var record = auditer.GetLogRecord(_transactionContext.UserName, EventType.Added, this);
                    if (record != null)
                    {
                        AuditLog.Add(record);
                    }
                }
            }

            //save changed to audit of added entries
            base.SaveChanges();
            return result;
        }
    }
}
