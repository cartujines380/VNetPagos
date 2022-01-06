using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceContact : IService<Contact, ContactDto>
    {
        IEnumerable<ContactDto> GetDataForTable(ContactFilterDto filters);
        IEnumerable<ContactDto> GetDashboardData(ReportsDashboardFilterDto filters);

        //nuevo
        int[] GetDashboardDataCount(ReportsDashboardFilterDto filters);
    }
}
