using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ICustomerSiteSystemUserClientService
    {
        Task<ICollection<CustomerSiteSystemUserDto>> GetSystemUserLigth();
        Task<ICollection<CustomerSiteSystemUserDto>> GetDataForSystemUserTable(BaseFilter filtersDto);
        Task<int> GetDataForSystemUserTableCount(BaseFilter filtersDto);
        Task<CustomerSiteSystemUserDto> Find(Guid id);
        Task<CustomerSiteSystemUserDto> Create(CustomerSiteSystemUserDto bin);
        Task Edit(CustomerSiteSystemUserDto bin);
        Task Delete(Guid id);
        Task ChangeState(Guid entityId);
    }
}
