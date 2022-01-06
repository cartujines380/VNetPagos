using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IConciliationVNPClientService
    {
        Task RunConciliation(RunConciliationDto dto);

        Task<IEnumerable<ConciliationRunDto>> GetConciliationRunReport(ReportsConciliationRunFilterDto filter);
        Task<int> GetConciliationRunReportCount(ReportsConciliationRunFilterDto filter);

        Task<ConciliationRunDto> GetConciliationRun(Guid id);
    }
}