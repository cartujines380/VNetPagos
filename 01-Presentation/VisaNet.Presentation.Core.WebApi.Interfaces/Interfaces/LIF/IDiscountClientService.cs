using System;
using System.Threading.Tasks;
using VisaNet.Lif.Domain.EntitesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF
{
    public interface IDiscountClientService
    {
        Task<DiscountCalculationDto> CalculateDiscount(BillDto toDomainObject, string bin);
        Task<DiscountCalculationDto> CalculateDiscount(BillDto toDomainObject, string bin, Guid serviceId);
    }
}