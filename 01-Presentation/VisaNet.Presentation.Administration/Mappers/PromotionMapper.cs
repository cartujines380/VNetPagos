using System.Configuration;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class PromotionMapper
    {
        public static PromotionDto ToDto(this PromotionModel entity)
        {
            var dto = new PromotionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Active = entity.Active,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl
            };

            return dto;
        }

        public static PromotionModel ToModel(this PromotionDto entity)
        {
            var model = new PromotionModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Active = entity.Active,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl
            };

            return model;
        }

    }
}