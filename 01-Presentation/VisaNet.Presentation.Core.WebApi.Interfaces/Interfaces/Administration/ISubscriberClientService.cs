using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ISubscriberClientService
    {
        Task<ICollection<SubscriberDto>> FindAll();
        Task<ICollection<SubscriberDto>> FindAll(BaseFilter filtersDto);
        Task<SubscriberDto> Find(Guid id);
        Task Create(SubscriberDto subscriber);
        Task Edit(SubscriberDto subscriber);
        Task Delete(Guid id);
        Task<IEnumerable<SubscriberDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto);

        //nuevo
        Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto);
    }
}
