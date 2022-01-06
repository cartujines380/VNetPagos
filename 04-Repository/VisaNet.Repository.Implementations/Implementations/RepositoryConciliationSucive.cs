using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationSucive : BaseRepository<ConciliationSucive>, IRepositoryConciliationSucive
    {
        protected readonly DbContext _context;

        public RepositoryConciliationSucive(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }

        public List<ConciliationSucive> GetConciliationsNotInSummary()
        {
            return _context.Database.SqlQuery<ConciliationSucive>("SP_VisaNet_ConciliationSummarySucive").ToList();
        }
    }
}
