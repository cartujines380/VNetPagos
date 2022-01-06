using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationSummary : IService<ConciliationSummary, ConciliationSummaryDto>
    {
        IList<ReportConciliationDetailDto> GetDataForTable(ReportsConciliationFilterDto filtersDto);
        void GenerateSummary();
    }
}
