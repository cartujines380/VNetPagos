using System.Collections.Generic;
using System.Data.Entity;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryTc33Transaction: BaseRepository<Tc33Transaction>, IRepositoryTc33Transaction
    {
        private readonly Database _db;
        internal readonly DbContext _context;

        public RepositoryTc33Transaction(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
            _db = context.Database;
        }


        public IEnumerable<T> ExecuteSql<T>(string sql, object[] parameters)
        {
            if (parameters != null)
            {
                return _db.SqlQuery<T>(sql, parameters);
            }

            return _db.SqlQuery<T>(sql);
        }
    }
}
