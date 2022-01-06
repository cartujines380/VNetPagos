using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationSistarbanc : BaseRepository<ConciliationSistarbanc>, IRepositoryConciliationSistarbanc
    {
        protected readonly DbContext _context;

        public RepositoryConciliationSistarbanc(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }

        public List<ConciliationSistarbanc> GetConciliationsNotInSummary()
        {
            return _context.Database.SqlQuery<ConciliationSistarbanc>("SP_VisaNet_ConciliationSummarySistarbanc").ToList();
        }
    }
}
