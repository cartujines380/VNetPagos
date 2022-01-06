using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryWsPaymentCancellation : BaseRepository<WsPaymentCancellation>, IRepositoryWsPaymentCancellation
    {
        public RepositoryWsPaymentCancellation(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
