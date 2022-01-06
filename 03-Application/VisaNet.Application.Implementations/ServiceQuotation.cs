using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceQuotation : BaseService<Quotation, QuotationDto>, IServiceQuotation
    {
        public ServiceQuotation(IRepositoryQuotation repository)
            : base(repository)
        {

        }

        public override IQueryable<Quotation> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override QuotationDto Converter(Quotation entity)
        {
            return new QuotationDto
            {
                Id = entity.Id,
                DateFrom = entity.DateFrom,
                Currency = entity.Currency,
                ValueInPesos = entity.ValueInPesos
            };
        }

        public override Quotation Converter(QuotationDto entity)
        {
            return new Quotation
            {
                Id = entity.Id,
                DateFrom = entity.DateFrom,
                Currency = entity.Currency,
                ValueInPesos = entity.ValueInPesos
            };
        }

        public QuotationDto GetQuotationForDate(DateTime date, CurrencyDto currency)
        {
            var currencyStr = currency.ToString();

            var quotation = Repository.AllNoTracking().Where(x => x.Currency == currencyStr &&  x.DateFrom <= date)
                                      .OrderByDescending(q => q.DateFrom)
                                      .FirstOrDefault();

            if (quotation == null)
                return null;

            return Converter(quotation);
        }
    }
}
