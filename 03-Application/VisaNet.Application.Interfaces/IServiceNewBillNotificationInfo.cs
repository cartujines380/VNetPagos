using System;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using VisaNet.Domain.EntitiesDtos.NotificationsData;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceNewBillNotificationInfo : IService<NewBillNotificationInfo, NewBillNotificationInfoDto>
    {
        bool AlreadyExistsExpiredBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId);
        bool AlreadyExistsBeforeDueDateBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId);
        bool AlreadyExistsNewBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId);
    }
}