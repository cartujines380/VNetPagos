using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationRun : IService<ConciliationRun, ConciliationRunDto>
    {
        IEnumerable<ConciliationRunDto> GetConciliationRunReport(ReportsConciliationRunFilterDto filter);
        int GetConciliationRunReportCount(ReportsConciliationRunFilterDto filter);

        void UpdateConciliationRunResult(ConciliationRunDto dto);
    }
}