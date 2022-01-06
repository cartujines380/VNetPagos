using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IReportsClientService
    {
        Task<IEnumerable<TransactionsAmountDto>> GetTransactionsAmountData(ReportsTransactionsAmountFilterDto filtersDto);
        Task<IEnumerable<CybersourceTransactionsDto>> GetCybersourceTransactionsData(ReportsCybersourceTransactionsFilterDto filtersDto);
        Task<IEnumerable<LogPaymentCyberSourceDto>> GetCybersourceTransactionsDetails(ReportsCybersourceTransactionsFilterDto filtersDto);
        Task<DashboardDto> GetDashboardSP(ReportsDashboardFilterDto filtersDto);
    }
}
