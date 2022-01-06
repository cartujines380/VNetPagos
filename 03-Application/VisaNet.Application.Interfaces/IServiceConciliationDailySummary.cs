using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationDailySummary
    {
        ICollection<ConciliationDailySummaryDto> GetConciliationDailySummary(DailyConciliationFilterDto filter);
    }
}