using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class HomePageMapper
    {
        public static HomePageDto ToDto(this HomePageModel entity)
        {
            return new HomePageDto
            {
                Id = entity.Id,
                HomePageItem1Id = entity.HomePageItem1Id,
                HomePageItem1 = entity.HomePageItem1 == null ? null : entity.HomePageItem1.ToDto(),
                HomePageItem2Id = entity.HomePageItem2Id,
                HomePageItem2 = entity.HomePageItem2 == null ? null : entity.HomePageItem2.ToDto(),
                HomePageItem3Id = entity.HomePageItem3Id,
                HomePageItem3 = entity.HomePageItem3 == null ? null : entity.HomePageItem3.ToDto()
            };
        }

        public static HomePageModel ToModel(this HomePageDto entity)
        {
            return new HomePageModel
            {
                Id = entity.Id,
                HomePageItem1Id = entity.HomePageItem1Id,
                HomePageItem1 = entity.HomePageItem1 == null ? null : entity.HomePageItem1.ToModel(),
                HomePageItem2Id = entity.HomePageItem2Id,
                HomePageItem2 = entity.HomePageItem2 == null ? null : entity.HomePageItem2.ToModel(),
                HomePageItem3Id = entity.HomePageItem3Id,
                HomePageItem3 = entity.HomePageItem3 == null ? null : entity.HomePageItem3.ToModel()
            };
        }
    }
}