using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IContactClientService
    {
        Task<ICollection<ContactDto>> FindAll();
        Task<ICollection<ContactDto>> FindAll(BaseFilter filtersDto);
        Task<ContactDto> Find(Guid id);
        Task Create(ContactDto serviceCategory);
        Task Edit(ContactDto serviceCategory);
        Task Delete(Guid id);
        Task<IEnumerable<ContactDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto);

        //nuevo
        Task<int[]> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto);
    }
}
