using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePromotion : IService<Promotion, PromotionDto>
    {
        IEnumerable<PromotionDto> GetDataForTable(PromotionFilterDto filters);
        IEnumerable<PromotionDto> GetLastActive();
    }
}
