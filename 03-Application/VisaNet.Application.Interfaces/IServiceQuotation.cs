using System;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceQuotation : IService<Quotation, QuotationDto>
    {
        QuotationDto GetQuotationForDate(DateTime date, CurrencyDto currency);
    }
}
