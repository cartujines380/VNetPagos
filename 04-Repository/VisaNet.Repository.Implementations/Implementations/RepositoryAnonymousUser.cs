using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryAnonymousUser : BaseRepository<AnonymousUser>, IRepositoryAnonymousUser
    {
        public RepositoryAnonymousUser(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public long GetNextCyberSourceIdentifier()
        {
            return _db.SqlQuery<long>("SELECT NEXT VALUE FOR AnonymousUsers_CyberSourceIdentifier").First();
        }
    }
}