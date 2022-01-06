using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceReports
    {
        IEnumerable<TransactionsAmountDto> GetTransactionsAmountData(ReportsTransactionsAmountFilterDto filtersDto);
        IEnumerable<CybersourceTransactionsDto> GetCybersourceTransactionsData(ReportsCybersourceTransactionsFilterDto filtersDto);
        IEnumerable<LogPaymentCyberSourceDto> GetCybersourceTransactionsDetails(ReportsCybersourceTransactionsFilterDto filtersDto);
        DashboardDto GetDashboardSP(ReportsDashboardFilterDto filtersDto);

        List<List<object>> GetDashboardPieChartData(ReportFilterDto filterDto);
        List<List<object>> GetDashboardLineChartData(ReportFilterDto filterDto);
        List<ServiceCategoryDto> ServicesCategories(Guid userId);
        Dictionary<Guid, List<ServiceDto>> ServicesWithPayments(Guid userId);
    }
}
