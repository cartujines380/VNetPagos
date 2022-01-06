using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class DiscountMapper
    {
        public static DiscountDto ToDto(this DiscountModel entity)
        {
            return new DiscountDto
            {
                Id = entity.Id,
                CardType = (CardTypeDto)entity.CardTypeId,
                From = entity.From,
                To = entity.To,
                Fixed = entity.Fixed,
                Additional = entity.Additional,
                MaximumAmount = entity.MaximumAmount,
                DiscountType = (DiscountTypeDto)entity.DiscountType,
                DiscountLabel = (DiscountLabelTypeDto)entity.DiscountLabel
            };
        }

        public static DiscountModel ToModel(this DiscountDto entity)
        {
            return new DiscountModel
            {
                Id = entity.Id,
                CardTypeId = (int)entity.CardType,
                From = entity.From,
                To = entity.To,
                Fixed = entity.Fixed,
                Additional = entity.Additional,
                MaximumAmount = entity.MaximumAmount,
                DiscountType = (int)entity.DiscountType,
                DiscountLabel = (int)entity.DiscountLabel
            };
        }
    }
}