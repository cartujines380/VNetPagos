using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface INotificationClientService
    {
        Task<ICollection<NotificationDto>> FindAll();
        Task<IEnumerable<NotificationDto>> GetDashboardData(ReportsDashboardFilterDto filters);

        //nuevo
        Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filters);
    }
}
