using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ICustomerSiteBranchClientService
    {
        Task<ICollection<CustomerSiteBranchDto>> GetDataForBranchTable(BaseFilter filtersDto);
        Task<int> GetDataForBranchTableCount(BaseFilter filtersDto);
        Task<CustomerSiteBranchDto> Find(Guid id);
        Task Create(CustomerSiteBranchDto bin);
        Task Edit(CustomerSiteBranchDto bin);
        Task Delete(Guid id);
        Task ChangeState(Guid entityId);
        Task<ICollection<CustomerSiteBranchDto>> GetBranchesLigth(BaseFilter filtersDto);
    }
}
