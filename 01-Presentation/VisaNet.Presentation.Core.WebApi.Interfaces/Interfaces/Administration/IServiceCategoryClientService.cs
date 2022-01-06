using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IServiceCategoryClientService
    {
        Task<ICollection<ServiceCategoryDto>> FindAll();
        Task<ICollection<ServiceCategoryDto>> FindAll(BaseFilter filtersDto);
        Task<ServiceCategoryDto> Find(Guid id);
        Task Create(ServiceCategoryDto serviceCategory);
        Task Edit(ServiceCategoryDto serviceCategory);
        Task Delete(Guid id);
    }
}
