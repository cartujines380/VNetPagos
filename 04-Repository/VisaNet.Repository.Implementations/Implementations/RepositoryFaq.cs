using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryFaq : BaseRepository<Faq>, IRepositoryFaq
    {
        public RepositoryFaq(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
