using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IDebitCommerceClientService
    {
        Task<int> GetDebitSuscriptionListCount(BaseFilter filterDto);
        Task<ICollection<DebitRequestDto>> GetDebitSuscriptionList(BaseFilter filterDto);
        Task UpdateCommerceDebitCatche();
        Task<DebitRequestDto> Find(Guid id);
        Task<CustomerSiteCommerceDto> FindCommerce(int debitProductId);
        Task<ICollection<DebitRequestExcelDto>> ExcelExportManualSynchronization();
    }
}
