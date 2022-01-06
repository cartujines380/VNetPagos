using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryNewBillNotificationInfo : BaseRepository<NewBillNotificationInfo>, IRepositoryNewBillNotificationInfo
    {
        public RepositoryNewBillNotificationInfo(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}