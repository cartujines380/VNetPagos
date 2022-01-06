using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryHighwayEmail: BaseRepository<HighwayEmail>, IRepositoryHighwayEmail
    {
        public RepositoryHighwayEmail(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public long GetNextTransactionNumber()
        {
            return _db.SqlQuery<long>("SELECT NEXT VALUE FOR [dbo].[Seq_HighwayNroTransaction]").First();
        }

        public void Create(HighwayEmail entity)
        {
            entity.ClientIp = TransactionContext.IP;
            base.Create(entity);
        }
    }
}
