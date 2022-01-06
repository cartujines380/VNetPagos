using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class PromotionMapper
    {
        public static PromotionModel ToModel(this PromotionDto entity)
        {
            return new PromotionModel
            {
                ImageUrl = entity.ImageUrl
            };
        }
    }
}