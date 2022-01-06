using System;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceDiscount : IService<Discount, DiscountDto>
    {
        DiscountDto GetDiscount(DateTime date, CardTypeDto cardType, DiscountTypeDto discountType);
    }
}
