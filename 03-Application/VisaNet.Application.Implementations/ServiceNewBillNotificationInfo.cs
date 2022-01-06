using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.NotificationsData;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceNewBillNotificationInfo : BaseService<NewBillNotificationInfo, NewBillNotificationInfoDto>, IServiceNewBillNotificationInfo
    {
        public ServiceNewBillNotificationInfo(IRepositoryNewBillNotificationInfo repository)
            : base(repository)
        {
        }

        public override IQueryable<NewBillNotificationInfo> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override NewBillNotificationInfoDto Converter(NewBillNotificationInfo entity)
        {
            if (entity == null)
            {
                return null;
            }

            var obj = new NewBillNotificationInfoDto
            {
                Id = entity.Id,
                BillNumber = entity.BillNumber,
                ExpirationDate = entity.ExpirationDate,
                CreationDate = entity.CreationDate,
                BillType = (BillTypeDto)entity.BillType,
                ServiceId = entity.ServiceId,
                ApplicationUserId = entity.ApplicationUserId
            };

            return obj;
        }

        public override NewBillNotificationInfo Converter(NewBillNotificationInfoDto entity)
        {
            if (entity == null)
            {
                return null;
            }

            var obj = new NewBillNotificationInfo
            {
                Id = entity.Id,
                BillNumber = entity.BillNumber,
                ExpirationDate = entity.ExpirationDate,
                CreationDate = entity.CreationDate,
                BillType = (BillType)entity.BillType,
                ServiceId = entity.ServiceId,
                ApplicationUserId = entity.ApplicationUserId
            };

            return obj;
        }

        public bool AlreadyExistsExpiredBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId)
        {
            var exists = Repository.AllNoTracking(n =>
                n.BillNumber == billExternalId &&
                n.ApplicationUserId != null && n.ApplicationUserId == applicationUserId &&
                n.ServiceId != null && n.ServiceId == serviceId &&
                n.BillType == BillType.ExpiredBill
                ).Any();

            return exists;
        }

        public bool AlreadyExistsBeforeDueDateBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId)
        {
            var exists = Repository.AllNoTracking(n =>
                n.BillNumber == billExternalId &&
                n.ApplicationUserId != null && n.ApplicationUserId == applicationUserId &&
                n.ServiceId != null && n.ServiceId == serviceId &&
                (n.BillType == BillType.AboutToExpireBill || n.BillType == BillType.ExpiredBill)
                ).Any();

            return exists;
        }

        public bool AlreadyExistsNewBillNotification(string billExternalId, Guid applicationUserId, Guid serviceId)
        {
            var exists = Repository.AllNoTracking(n =>
                n.BillNumber == billExternalId &&
                n.ApplicationUserId != null && n.ApplicationUserId == applicationUserId &&
                n.ServiceId != null && n.ServiceId == serviceId
                ).Any();

            return exists;
        }

    }
}