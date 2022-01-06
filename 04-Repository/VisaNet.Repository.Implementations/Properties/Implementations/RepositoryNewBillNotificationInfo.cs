using System;
using System.Data.Entity;
using System.Data.SqlClient;
using VisaNet.Common.MSMQ;
using VisaNet.Common.MSMQ.MSMQ;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
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
