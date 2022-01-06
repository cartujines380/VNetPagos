using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IPromotionClientService
    {
        Task<ICollection<PromotionDto>> FindAll();
        Task<ICollection<PromotionDto>> FindAll(BaseFilter filtersDto);
        Task<PromotionDto> Find(Guid id);
        Task Create(PromotionDto service);
        Task Edit(PromotionDto service);
        Task Delete(Guid id);
        
    }
}
