using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IBinsClientService
    {
        Task<ICollection<BinDto>> FindAll();
        Task<ICollection<BinDto>> GetDataForTable(BaseFilter filtersDto);
        Task<int> GetDataForTableCount(BaseFilter filtersDto);
        Task<BinDto> Find(Guid id);
        Task Create(BinDto bin);
        Task Edit(BinDto bin);
        Task Delete(Guid id);
        Task<ICollection<BankDto>> GetBanks();
        Task ChangeStatus(Guid id);
    }
}
