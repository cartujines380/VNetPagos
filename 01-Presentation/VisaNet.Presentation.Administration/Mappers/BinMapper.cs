using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Models;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class BinMapper
    {
        public static BinDto ToDto(this BinModel entity)
        {
            var dto = new BinDto
            {
                Id = entity.Id,
                Name = entity.Name,
                GatewayId = entity.GatewayId,
                Value = int.Parse(entity.Value),
                Description = entity.Description,
                ImageDeleted = entity.DeleteImage,
                CardType = (CardTypeDto)entity.CardTypeId,
                BankDtoId = entity.BankId,
                Active = entity.Active,
                Country = entity.Country,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl,
                AffiliationCardId = entity.AffiliationCardId,
                BinAuthorizationAmountTypeDtoList = entity.BinAuthorizationAmountModelList.Select(x => new BinAuthorizationAmountTypeDto()
                {
                    BinId = entity.Id,
                    AuthorizationAmountTypeDto = (AuthorizationAmountTypeDto)x.AuthorizationAmountTypeDto,
                    LawDto = x.LawDto,
                    Id = x.Id,
                }).ToList(),
            };

            return dto;
        }

        public static BinModel ToModel(this BinDto entity)
        {
            var model = new BinModel
            {
                Id = entity.Id,
                Name = entity.Name,
                GatewayId = entity.GatewayId,
                Value = entity.Value.ToString().PadLeft(6, '0'),
                Description = entity.Description,
                CardTypeId = (int)entity.CardType,
                BankId = entity.BankDtoId,
                Active = entity.Active,
                Country = entity.Country,
                BankName = entity.BankDto != null ? entity.BankDto.Name : "",
                CardTypeName = EnumHelpers.GetName(typeof(CardTypeDto), (int)entity.CardType, EnumsStrings.ResourceManager),
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl,
                AffiliationCardId = entity.AffiliationCardId,
            };

            if (entity.BinGroups != null && entity.BinGroups.Any())
            {
                model.BinGroups = new Collection<BinGroupModel>();
                foreach (var binGroup in entity.BinGroups)
                {
                    model.BinGroups.Add(binGroup.ToModel());
                }
            }

            if (entity.BinAuthorizationAmountTypeDtoList != null && entity.BinAuthorizationAmountTypeDtoList.Any())
            {
                model.BinAuthorizationAmountModelList =
                    entity.BinAuthorizationAmountTypeDtoList.Select(x => new BinAuthorizationAmountModel()
                    {
                        BinId = entity.Id,
                        AuthorizationAmountTypeDto = (int)x.AuthorizationAmountTypeDto,
                        LawDto = x.LawDto,
                        Id = x.Id,
                        Label = ModelsStrings.Bin_AuthorizationAmountType + " " + EnumHelpers.GetName(typeof(DiscountTypeDto), (int)x.LawDto, EnumsStrings.ResourceManager),
                    }).ToList();
            }

            if (entity.AffiliationCardDto != null)
            {
                model.AffiliationCardModel = new AffiliationCardModel()
                {
                    Name = entity.AffiliationCardDto.Name,
                    Active = entity.AffiliationCardDto.Active,
                    Code = entity.AffiliationCardDto.Code,
                    BankId = entity.AffiliationCardDto.BankId,
                };
            }
            return model;
        }

    }
}