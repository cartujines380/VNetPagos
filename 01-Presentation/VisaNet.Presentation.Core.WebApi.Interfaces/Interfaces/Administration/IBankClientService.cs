using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IBankClientService
    {
        Task<ICollection<BankDto>> FindAll();
        Task<ICollection<BankDto>> FindAll(BaseFilter filtersDto);
        Task<BankDto> Find(Guid id);
        Task Create(BankDto bin);
        Task Edit(BankDto bin);
        Task Delete(Guid id);

        Task<int> GetDataForBankCount(BaseFilter filterDto);
    }
}
