using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationBanred : BaseRepository<ConciliationBanred>, IRepositoryConciliationBanred
    {
        protected readonly DbContext _context;

        public RepositoryConciliationBanred(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }

        public List<ConciliationBanred> GetConciliationsNotInSummary()
        {
            return _context.Database.SqlQuery<ConciliationBanred>("SP_VisaNet_ConciliationSummaryBanred").ToList();
        }
    }
}
