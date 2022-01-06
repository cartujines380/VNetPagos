using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IConciliationDailySummaryClientService
    {
        Task<ICollection<ConciliationDailySummaryDto>> GetConciliationDailySummary(DailyConciliationFilterDto filter);
    }
}