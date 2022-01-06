using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryMailMessage : BaseRepository<EmailMessage>, IRepositoryMailMessage
    {
        public RepositoryMailMessage(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
