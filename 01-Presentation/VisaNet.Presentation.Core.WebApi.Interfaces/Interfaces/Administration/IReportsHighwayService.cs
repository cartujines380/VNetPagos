using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IReportsHighwayService
    {
        Task<ICollection<HighwayEmailDto>> GetHighwayEmailsReports(ReportsHighwayEmailFilterDto filter);
        Task<int> GetHighwayEmailsReportsCount(ReportsHighwayEmailFilterDto filter);
        Task<ICollection<HighwayBillDto>> GetHighwayBillReports(ReportsHighwayBillFilterDto filter);
        Task<int> GetHighwayBillReportsCount(ReportsHighwayBillFilterDto filter);

        Task<ICollection<HighwayEmailErrorDto>> ProccessEmailFile(HighwayEmailDto email);
        Task<ICollection<HighwayEmailErrorDto>> ProccessEmailFileExternalSoruce(HighwayEmailDto email);
        Task<HighwayEmailDto> GetHighwayEmail(Guid id);

    }
}