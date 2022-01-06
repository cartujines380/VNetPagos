using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class ParametersMapper
    {
        public static ParametersDto ToDto(this ParametersModel entity)
        {
            return new ParametersDto
            {
                Id = entity.Id,
                Contact = new EmailDto { EmailAddress = entity.ContactEmail },
                ErrorNotification = new EmailDto { EmailAddress = entity.ErrorNotificationEmail },
                SendingEmail = new EmailDto { EmailAddress = entity.SendingEmail },
                LoginNumberOfTries = entity.LoginNumberOfTries,
                CybersourceAccessKey = entity.CybersourceAccessKey,
                CybersourceProfileId = entity.CybersourceProfileId,
                CybersourceSecretKey = entity.CybersourceSecretKey,
                CybersourceTransactionKey = entity.CybersourceTransactionKey,
                MerchantId = entity.MerchantId,
                Banred = new BankCodeDto { Code = entity.BanredCode },
                Sistarbanc = new BankCodeDto { Code = entity.SistarbancCode },
                Cybersource = new BankCodeDto { Code = entity.CybersourceCode },
                Sucive = new BankCodeDto { Code = entity.SuciveCode },
                Geocom = new BankCodeDto { Code = entity.GeocomCode },
                SistarbancBrou = new BankCodeDto { Code = entity.SistarbancCodeBrou },
            };
        }

        public static ParametersModel ToModel(this ParametersDto entity)
        {
            return new ParametersModel
            {
                Id = entity.Id,
                ContactEmail = entity.Contact.EmailAddress,
                ErrorNotificationEmail = entity.ErrorNotification.EmailAddress,
                SendingEmail = entity.SendingEmail.EmailAddress,
                LoginNumberOfTries = entity.LoginNumberOfTries,
                MerchantId = entity.MerchantId,
                BanredCode = entity.Banred.Code,
                SistarbancCode = entity.Sistarbanc.Code,
                CybersourceCode = entity.Cybersource.Code,
                SuciveCode = entity.Sucive.Code,
                GeocomCode = entity.Geocom.Code,
                SistarbancCodeBrou = entity.SistarbancBrou.Code
            };
        }
    }
}