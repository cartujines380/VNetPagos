using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ICustomerSiteCommerceClientService
    {
        Task<ICollection<CustomerSiteCommerceDto>> GetCommercesLigth();
        Task<ICollection<CustomerSiteCommerceDto>> GetDataForCommerceTable(BaseFilter filtersDto);
        Task<int> GetDataForCommerceTableCount(BaseFilter filtersDto);
        Task<CustomerSiteCommerceDto> Find(Guid id);
        Task Create(CustomerSiteCommerceDto bin);
        Task Edit(CustomerSiteCommerceDto bin);
        Task Delete(Guid id);
        Task ChangeState(Guid entityId);

        //DEBITO
        Task<ICollection<CustomerSiteCommerceDto>> GetCommercesDebit(BaseFilter filterDto);
        Task<int> GetCommercesDebitCount(BaseFilter filterDto);
        Task EditDebitCommerceServiceId(CustomerSiteCommerceDto dto);
    }
}