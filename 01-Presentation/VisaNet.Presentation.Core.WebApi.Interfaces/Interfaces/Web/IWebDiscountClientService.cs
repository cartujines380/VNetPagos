using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebDiscountClientService
    {
        Task<List<CyberSourceExtraDataDto>> GetDiscount(DiscountQueryDto discountQuery);

        Task<bool> ValidateBin(int binNumber, Guid serviceId);
    }
}
