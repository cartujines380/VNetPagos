using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IFaqClientService
    {
        Task<ICollection<FaqDto>> FindAll();
        Task<ICollection<FaqDto>> FindAll(BaseFilter filtersDto);
        Task<FaqDto> Find(Guid id);
        Task Create(FaqDto serviceCategory);
        Task Edit(FaqDto serviceCategory);
        Task Delete(Guid id);
    }
}
