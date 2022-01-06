using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IAffiliationCardClientService
    {
        Task<ICollection<AffiliationCardDto>> FindAll();
        Task<ICollection<AffiliationCardDto>> GetDataForTable(BaseFilter filtersDto);
        Task<int> GetDataForTableCount(BaseFilter filtersDto);
        Task<AffiliationCardDto> Find(Guid id);
        Task Create(AffiliationCardDto affiliationCard);
        Task Edit(AffiliationCardDto affiliationCard);
        Task Delete(Guid id);
        Task ChangeStatus(Guid id);
    }
}
