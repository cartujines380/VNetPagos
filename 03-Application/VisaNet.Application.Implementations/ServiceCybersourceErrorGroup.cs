using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceCybersourceErrorGroup : BaseService<CybersourceErrorGroup, CybersourceErrorGroupDto>, IServiceCybersourceErrorGroup
    {
        public ServiceCybersourceErrorGroup(IRepositoryCybersourceErrorGroup repository)
            : base(repository)
        {

        }

        public override IQueryable<CybersourceErrorGroup> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override CybersourceErrorGroupDto Converter(CybersourceErrorGroup entity)
        {
            return new CybersourceErrorGroupDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Order = entity.Order,
                CybersourceErrors = entity.CybersourceErrors.Select(c => new CybersourceErrorDto
                {
                    Id = c.Id,
                    ReasonCode = c.ReasonCode
                }).ToList()
            };
        }

        public override CybersourceErrorGroup Converter(CybersourceErrorGroupDto entity)
        {
            return new CybersourceErrorGroup
            {
                Id = entity.Id,
                Name = entity.Name,
                Order = entity.Order,
                CybersourceErrors = entity.CybersourceErrors.Select(c => new CybersourceError
                {
                    Id = c.Id,
                    ReasonCode = c.ReasonCode
                }).ToList()
            };
        }
    }
}
