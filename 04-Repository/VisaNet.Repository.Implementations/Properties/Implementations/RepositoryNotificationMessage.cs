using System;
using System.Data.Entity;
using System.Data.SqlClient;
using VisaNet.Common.MSMQ;
using VisaNet.Common.MSMQ.MSMQ;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryNotificationMessage : BaseRepository<NotificationMessage>, IRepositoryNotificationMessage
    {
        public RepositoryNotificationMessage(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            
        }

        public void NotifyNotificationProcess()
        {
            var msmq = new VisaNetMSMQ();
            msmq.Send(new MsmqNotification());
        }


        public void RegisterFail(Guid id)
        {
            //PONEMOS EL SQL PARA EVITAR BLOQUEOS CON LAS TRANSACCIONES
            ExecuteSQL(@"UPDATE NotificationMessages SET SendDateTime = @currentDate, EmailStatus = @emailStatus, SendIntents = SendIntents + 1 WHERE id=@id",
                new[]{ new SqlParameter("currentDate",DateTime.Now), 
                        new SqlParameter("emailStatus", (int)NotificationMessageStatus.Pending), 
                        new SqlParameter("id", id) });
        }

        public void RegisterSuccess(Guid id)
        {
            //PONEMOS EL SQL PARA EVITAR BLOQUEOS CON LAS TRANSACCIONES
            ExecuteSQL(@"UPDATE NotificationMessages SET SendDateTime = @currentDate, EmailStatus = @emailStatus WHERE id=@id",
                new[]{ new SqlParameter("currentDate",DateTime.Now), 
                        new SqlParameter("emailStatus", (int)NotificationMessageStatus.Delivered), 
                        new SqlParameter("id", id) });
        }
    }
}
