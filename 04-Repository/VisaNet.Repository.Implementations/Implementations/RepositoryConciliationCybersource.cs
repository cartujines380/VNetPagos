using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationCybersource : BaseRepository<ConciliationCybersource>, IRepositoryConciliationCybersource
    {
        protected readonly DbContext _context;

        public RepositoryConciliationCybersource(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }

        public List<ConciliationCybersource> GetConciliationsNotInSummary()
        {
            return _context.Database.SqlQuery<ConciliationCybersource>("SP_VisaNet_ConciliationSummaryCybersource").ToList();
        }
    }
}
