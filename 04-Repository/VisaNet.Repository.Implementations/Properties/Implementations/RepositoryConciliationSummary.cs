using System.Collections.Generic;
using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationSummary : BaseRepository<ConciliationSummary>, IRepositoryConciliationSummary
    {
        private readonly Database _db;

        public RepositoryConciliationSummary(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _db = context.Database;
        }

        public IEnumerable<T> ExecuteSQL<T>(string sql, object[] parameters)
        {
            return _db.SqlQuery<T>(sql, parameters);
        }
    }
}
