using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Areas.Private.Models;

namespace VisaNet.Presentation.Web.Areas.Private.Mappers
{
    public static class NotificationMapper
    {
        public static NotificationDto ToDto(this NotificationModel entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                ServiceId = entity.ServiceId,
                Date = entity.Date,
                Message = entity.Message,
                NotificationPrupose = entity.NotificationPrupose,
                RegisteredUserId = entity.RegisteredUserId
            };
        }

        public static NotificationModel ToModel(this NotificationDto entity)
        {
            return new NotificationModel
            {
                Id = entity.Id,
                ServiceId = entity.ServiceId,
                ServiceName = entity.Service == null ? string.Empty : entity.Service.Name,
                ServiceImageName = entity.Service == null ? string.Empty : !string.IsNullOrEmpty(entity.Service.ImageUrl) ? entity.Service.ImageUrl : string.Empty,
                ServiceDesc = entity.Service == null ? string.Empty : entity.Service.Description,
                Date = entity.Date,
                Message = entity.Message,
                NotificationPrupose = entity.NotificationPrupose,
                RegisteredUserId = entity.RegisteredUserId,
                RegisteredUserEmail = entity.RegisteredUser.Email,
                RegisteredUserMobileNumber = entity.RegisteredUser.MobileNumber
            };
        }

    }
}