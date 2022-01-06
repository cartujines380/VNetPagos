using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceSubscriber : IService<Subscriber, SubscriberDto>
    {
        IEnumerable<SubscriberDto> GetDataForTable(SubscriberFilterDto filters);
        IEnumerable<SubscriberDto> GetDashboardData(ReportsDashboardFilterDto filters);
        int GetDashboardDataCount(ReportsDashboardFilterDto filters);
        void DeleteByEmail(string email);
        bool ExistsEmail(string email);
    }
}
