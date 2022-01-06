using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceParameters : BaseService<Parameters, ParametersDto>, IServiceParameters
    {
        public ServiceParameters(IRepositoryParameters repository)
            : base(repository)
        {
        }

        public override IQueryable<Parameters> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ParametersDto Converter(Parameters entity)
        {
            if (entity == null) return null;

            return new ParametersDto
            {
                Id = entity.Id,
                Contact = new EmailDto { EmailAddress = entity.Contact.EmailAddress },
                ErrorNotification = new EmailDto { EmailAddress = entity.ErrorNotification.EmailAddress },
                SendingEmail = new EmailDto { EmailAddress = entity.SendingEmail.EmailAddress },
                LoginNumberOfTries = entity.LoginNumberOfTries,
                MerchantId = entity.MerchantId,
                Banred = new BankCodeDto { Code = entity.Banred.Code },
                Sistarbanc = new BankCodeDto { Code = entity.Sistarbanc.Code },
                SistarbancBrou = new BankCodeDto { Code = entity.SistarbancBrou.Code },
                Cybersource = new BankCodeDto { Code = entity.Cybersource.Code },
                Sucive = new BankCodeDto { Code = entity.Sucive.Code },
                Geocom = new BankCodeDto { Code = entity.Geocom.Code },
            };
        }

        public override Parameters Converter(ParametersDto entity)
        {
            if (entity == null) return null;

            return new Parameters
            {
                Id = entity.Id,
                Contact = new Email { EmailAddress = entity.Contact.EmailAddress },
                ErrorNotification = new Email { EmailAddress = entity.ErrorNotification.EmailAddress },
                SendingEmail = new Email { EmailAddress = entity.SendingEmail.EmailAddress },
                LoginNumberOfTries = entity.LoginNumberOfTries,
                Banred = new BankCode{ Code = entity.Banred.Code },
                Sistarbanc = new BankCode { Code = entity.Sistarbanc.Code },
                SistarbancBrou = new BankCode { Code = entity.SistarbancBrou.Code },
                Cybersource = new BankCode{ Code = entity.Cybersource.Code },
                Sucive = new BankCode{ Code = entity.Sucive.Code },
                Geocom = new BankCode{ Code = entity.Geocom.Code },
            };
        }

        public override void Edit(ParametersDto entity)
        {
            var entity_db = Repository.GetById(entity.Id);

            entity_db.Contact = new Email { EmailAddress = entity.Contact.EmailAddress };
            entity_db.ErrorNotification = new Email { EmailAddress = entity.ErrorNotification.EmailAddress };
            entity_db.SendingEmail = new Email { EmailAddress = entity.SendingEmail.EmailAddress };
            entity_db.Banred = new BankCode { Code = entity.Banred.Code };
            entity_db.Sistarbanc = new BankCode { Code = entity.Sistarbanc.Code };
            entity_db.SistarbancBrou = new BankCode { Code = entity.SistarbancBrou.Code };
            entity_db.Cybersource = new BankCode { Code = entity.Cybersource.Code };
            entity_db.Sucive = new BankCode { Code = entity.Sucive.Code };
            entity_db.Geocom = new BankCode { Code = entity.Geocom.Code };
            entity_db.LoginNumberOfTries = entity.LoginNumberOfTries;
            entity_db.CybersourceAccessKey = String.IsNullOrEmpty(entity.CybersourceAccessKey) ? entity_db.CybersourceAccessKey : entity.CybersourceAccessKey;
            entity_db.CybersourceSecretKey = String.IsNullOrEmpty(entity.CybersourceSecretKey) ? entity_db.CybersourceSecretKey : entity.CybersourceSecretKey;
            entity_db.CybersourceTransactionKey = String.IsNullOrEmpty(entity.CybersourceTransactionKey) ? entity_db.CybersourceTransactionKey : entity.CybersourceTransactionKey;
            entity_db.MerchantId = String.IsNullOrEmpty(entity.MerchantId) ? entity_db.MerchantId : entity.MerchantId;
            entity_db.CybersourceProfileId = String.IsNullOrEmpty(entity.CybersourceProfileId) ? entity_db.CybersourceProfileId : entity.CybersourceProfileId;
            
            Repository.Edit(entity_db);
            Repository.Save();
        }

        public ParametersDto GetParametersForCard()
        {
            var query = Repository.AllNoTracking().FirstOrDefault();
            var param = new ParametersDto()
                        {
                            CybersourceAccessKey = query.CybersourceAccessKey,
                            CybersourceProfileId = query.CybersourceProfileId,
                            CybersourceSecretKey = query.CybersourceSecretKey,
                            CybersourceTransactionKey = query.CybersourceTransactionKey,
                            MerchantId = query.MerchantId,
                        };

            return param;
        }

    }
}
