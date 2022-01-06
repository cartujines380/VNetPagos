using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.SmsApi.Twilio;

namespace VisaNet.Application.Implementations
{
    public class ServiceSmsNotification : BaseService<SmsMessage, SmsMessageDto>, IServiceSmsNotification
    {
        public ServiceSmsNotification(IRepositorySmsMessage repository)
            : base(repository)
        {
        }

        public override IQueryable<SmsMessage> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override SmsMessageDto Converter(SmsMessage entity)
        {
            var dto = new SmsMessageDto()
            {
                Id = entity.Id,
                ApplicationUserId = entity.ApplicationUserId,
                Message = entity.Message,
                MobileNumber = entity.MobileNumber,
                SendDateTime = entity.SendDateTime,
                SendIntents = entity.SendIntents,
                SmsStatus = entity.SmsStatus,
                SmsType = entity.SmsType,
                LastSendIntentDateTime = entity.LastSendIntentDateTime,
                CreationDateTime = entity.CreationDateTime
            };

            return dto;
        }

        public override SmsMessage Converter(SmsMessageDto dto)
        {
            var entity = new SmsMessage()
            {
                Id = dto.Id,
                ApplicationUserId = dto.ApplicationUserId,
                Message = dto.Message,
                MobileNumber = dto.MobileNumber,
                SendDateTime = dto.SendDateTime,
                SendIntents = dto.SendIntents,
                SmsStatus = dto.SmsStatus,
                SmsType = dto.SmsType,
                LastSendIntentDateTime = dto.LastSendIntentDateTime,
                CreationDateTime = dto.CreationDateTime
            };

            return entity;
        }

        public void SendVonAccessSms(SmsMessageVonAccessDto dto)
        {
            var newMesg = PresentationCoreMessages.CustomerSiteSmsBillNotification + " " + dto.ShortUrl;

            Repository.ContextTrackChanges = true;
            try
            {
                var sms = new SmsMessage()
                {
                    MobileNumber = dto.PhoneNumber,
                    Message = newMesg,
                    SmsStatus = SmsStatus.New,
                    SmsType = SmsType.Tipo1,
                    CreationDateTime = DateTime.Now
                };

                sms.GenerateNewIdentity();

                Repository.Create(sms);
                Repository.Save();

                //Twilio SMS
                var twilioapi = new TwilioSmsEndpoint();
                var status = twilioapi.SendSms(sms.MobileNumber, newMesg);

                sms.SmsStatus = (SmsStatus)(int)status;
                Repository.Edit(sms);
                Repository.Save();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }

            Repository.ContextTrackChanges = false;
        }

    }
}