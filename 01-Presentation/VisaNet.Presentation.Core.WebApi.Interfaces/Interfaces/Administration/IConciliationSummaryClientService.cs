using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IConciliationSummaryClientService
    {
        Task Edit(ConciliationSummaryDto conciliationSummary);
        Task<ConciliationSummaryDto> GetConciliationSummary(Guid id);
        Task<ConciliationBanredDto> GetConciliationBanred(Guid id);
        Task<ConciliationSistarbancDto> GetConciliationSistarbanc(Guid id);
        Task<ConciliationSuciveDto> GetConciliationSucive(Guid id);
        Task<ConciliationCybersourceDto> GetConciliationCybersource(Guid id);
        Task<ConciliationVisanetDto> GetConciliationVisanet(Guid conciliationGatewayId);
        Task<ConciliationVisanetCallbackDto> GetConciliationBatch(Guid id);
        Task<ICollection<ReportConciliationDetailDto>> GetConciliationSummary(ReportsConciliationFilterDto filtersDto);
    }
}
