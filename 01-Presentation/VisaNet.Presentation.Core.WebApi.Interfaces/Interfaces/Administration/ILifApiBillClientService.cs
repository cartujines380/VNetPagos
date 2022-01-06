using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ILifApiBillClientService
    {
        Task<ICollection<LifApiBillDto>> FindAll(BaseFilter filtersDto);
        Task<LifApiBillDto> Find(Guid id);
        Task<int> GetDataForLifApiBillCount(BaseFilter filterDto);
    }
}
