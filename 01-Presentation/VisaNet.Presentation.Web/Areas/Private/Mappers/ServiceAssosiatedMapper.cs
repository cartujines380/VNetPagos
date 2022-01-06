using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Areas.Private.Models;

namespace VisaNet.Presentation.Web.Areas.Private.Mappers
{
    public static class ServiceAssosiatedMapper
    {
        public static ServiceAssociatedDto ToDto(this ServiceAssociateModel entity)
        {
            var serviceDto = new ServiceAssociatedDto()
                   {
                       ReferenceNumber = entity.ReferenceValue,
                       ReferenceNumber2 = entity.ReferenceValue2,
                       ReferenceNumber3 = entity.ReferenceValue3,
                       ReferenceNumber4 = entity.ReferenceValue4,
                       ReferenceNumber5 = entity.ReferenceValue5,
                       ReferenceNumber6 = entity.ReferenceValue6,
                       Description = entity.Description,
                       ServiceId = entity.ServiceToPayId,
                       Id = entity.Id != null ? Guid.Parse(entity.Id) : Guid.Empty,
                       Enabled = true
                   };

            var noticonf = new NotificationConfigDto()
                           {
                               DaysBeforeDueDate = entity.NotificationConfig.DaysBeforeDueDate,
                               BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                                                        {
                                                            Email = entity.NotificationConfig.DaysBeforeDueDateConfigEmail,
                                                            Sms = entity.NotificationConfig.DaysBeforeDueDateConfigSms,
                                                            Web = entity.NotificationConfig.DaysBeforeDueDateConfigWeb
                                                        },
                               ExpiredBillDto = new ExpiredBillDto()
                                                {
                                                    Email = entity.NotificationConfig.ExpiredBillEmail,
                                                    Sms = entity.NotificationConfig.ExpiredBillSms,
                                                    Web = entity.NotificationConfig.ExpiredBillWeb
                                                },
                               NewBillDto = new NewBillDto()
                                            {
                                                Email = entity.NotificationConfig.NewBillEmail,
                                                Sms = entity.NotificationConfig.NewBillSms,
                                                Web = entity.NotificationConfig.NewBillWeb
                                            },
                               FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                                                           {
                                                               Email = entity.NotificationConfig.FailedAutomaticPaymentEmail,
                                                               Sms = entity.NotificationConfig.FailedAutomaticPaymentSms,
                                                               Web = entity.NotificationConfig.FailedAutomaticPaymentWeb
                                                           },
                               SuccessPaymentDto = new SuccessPaymentDto()
                                                   {
                                                       Email = entity.NotificationConfig.SuccessPaymentEmail,
                                                       Sms = entity.NotificationConfig.SuccessPaymentSms,
                                                       Web = entity.NotificationConfig.SuccessPaymentWeb
                                                   }
                           };

            serviceDto.NotificationConfigDto = noticonf;

            return serviceDto;
        }
    }
}