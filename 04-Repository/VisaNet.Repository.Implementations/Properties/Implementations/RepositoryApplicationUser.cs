using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryApplicationUser : BaseRepository<ApplicationUser>, IRepositoryApplicationUser
    {
        public RepositoryApplicationUser(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public long GetNextCyberSourceIdentifier()
        {
            return _db.SqlQuery<long>("SELECT NEXT VALUE FOR ApplicationUsers_CyberSourceIdentifier").First();
        }
    }
}
