using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServicePaymentIdentifier : BaseService<PaymentIdentifier, PaymentIdentifierDto>, IServicePaymentIdentifier
    {
        public ServicePaymentIdentifier(IRepositoryPaymentIdentifier repository)
            : base(repository)
        {
            
        }


        public override IQueryable<PaymentIdentifier> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override PaymentIdentifierDto Converter(PaymentIdentifier entity)
        {
            return new PaymentIdentifierDto()
                   {
                       CyberSourceTransactionIdentifier = entity.CyberSourceTransactionIdentifier,
                       UniqueIdentifier = entity.UniqueIdentifier,
                       Id = entity.Id
                   };
        }

        public override PaymentIdentifier Converter(PaymentIdentifierDto entity)
        {
            return new PaymentIdentifier()
                   {
                       CyberSourceTransactionIdentifier = entity.CyberSourceTransactionIdentifier,
                       UniqueIdentifier = entity.UniqueIdentifier,
                       Id = entity.Id
                   };
        }
    }
}

