using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IBinGroupClientService
    {
        Task<ICollection<BinGroupDto>> FindAll();
        Task<ICollection<BinGroupDto>> FindAll(BaseFilter filtersDto);
        Task<BinGroupDto> Find(Guid id);
        Task Create(BinGroupDto bin);
        Task Edit(BinGroupDto bin);
        Task Delete(Guid id);
    }
}
