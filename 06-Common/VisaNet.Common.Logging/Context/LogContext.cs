using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Common.Logging.Context
{
    public class LogContext : DbContext
    {
        public LogContext():base("AppContext")
        {
            Database.SetInitializer<LogContext>(null);
        }
        
        //Logs
        public IDbSet<Log> Logs { get; set; }
        public IDbSet<LogPaymentCyberSource> LogPaymentCyberSource { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}
