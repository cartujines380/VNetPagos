using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebDebitClientService
    {
        Task<ICollection<CustomerSiteCommerceDto>> GetCommercesDebit(BaseFilter customerSiteCommerceFilterDto = null);
        Task<CustomerSiteCommerceDto> FindCommerceDebit(Guid id);
        Task<IEnumerable<DebitRequestDto>> GetDebitRequestByUserId(Guid userId);
        Task<ICollection<DebitRequestTableDto>> GetDataForFromList(BaseFilter filtersDto);
        Task<bool> ValidateCardType(int binValue);
        Task<DebitRequestDto> Create(DebitRequestDto dto);
        Task<bool> CancelDebitRequest(Guid id);

        Task<CybersourceCreateDebitWithNewCardDto> ProccesDataFromCybersource(IDictionary<string, string> csDictionary);
        
    }
}