using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceNotification : IService<Notification, NotificationDto>
    {
        IEnumerable<NotificationDto> GetDataForTable(NotificationFilterDto filters);
        IEnumerable<NotificationDto> GetDashboardData(ReportsDashboardFilterDto filters);

        //nuevo
        int GetDashboardDataCount(ReportsDashboardFilterDto filters);
    }
}
