using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebReportClientService
    {
        Task<List<List<object>>> PieChart(BaseFilter filtersDto);
        Task<List<List<object>>> LineChart(BaseFilter filtersDto);
        Task<List<ServiceCategoryDto>> ServicesCategories(Guid userId);

        Task<Dictionary<Guid, List<ServiceDto>>> ServicesWithPayments(Guid id);
    }
}
