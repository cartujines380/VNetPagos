using System;
using VisaNet.Lif.Domain.EntitesDtos;


namespace VisaNet.LIF.Interfaces
{
    public interface IDiscountService
    {
        DiscountCalculationDto CalculateDiscount(BillDto bill, string bin);
        DiscountCalculationDto CalculateDiscount(BillDto bill, string binValue, Guid serviceId);
    }
}
