using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceHomePage : BaseService<HomePage, HomePageDto>, IServiceHomePage
    {
        private readonly IServiceHomePageItem _serviceHomePageItem;

        public ServiceHomePage(IRepositoryHomePage repository, IServiceHomePageItem serviceHomePageItem)
            : base(repository)
        {
            _serviceHomePageItem = serviceHomePageItem;
        }

        public override IQueryable<HomePage> GetDataForTable()
        {
            return Repository.AllNoTracking(null, h => h.HomePageItem1, h => h.HomePageItem2, h => h.HomePageItem3);
        }

        public override HomePageDto Converter(HomePage entity)
        {
            if (entity == null) return null;

            return new HomePageDto
            {
                Id = entity.Id,
                HomePageItem1Id = entity.HomePageItem1Id,
                HomePageItem1 = entity.HomePageItem1 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem1),
                HomePageItem2Id = entity.HomePageItem2Id,
                HomePageItem2 = entity.HomePageItem2 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem2),
                HomePageItem3Id = entity.HomePageItem3Id,
                HomePageItem3 = entity.HomePageItem3 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem3),
            };
        }

        public override HomePage Converter(HomePageDto entity)
        {
            if (entity == null) return null;

            return new HomePage
            {
                Id = entity.Id,
                HomePageItem1Id = entity.HomePageItem1Id,
                HomePageItem1 = entity.HomePageItem1 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem1),
                HomePageItem2Id = entity.HomePageItem2Id,
                HomePageItem2 = entity.HomePageItem2 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem2),
                HomePageItem3Id = entity.HomePageItem3Id,
                HomePageItem3 = entity.HomePageItem3 == null ? null : _serviceHomePageItem.Converter(entity.HomePageItem3),
            };
        }

        
    }
}
