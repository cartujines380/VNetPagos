using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceDiscountCalculator
    {
        List<CyberSourceExtraDataDto> Calculate(DiscountQueryDto discountQuery);

        bool ValidateBin(int binNumber, Guid serviceId);
    }
}
