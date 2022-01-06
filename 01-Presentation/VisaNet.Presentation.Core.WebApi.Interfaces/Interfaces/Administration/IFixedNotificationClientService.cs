using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IFixedNotificationClientService
    {
        Task<ICollection<FixedNotificationDto>> FindAll();
        Task<IEnumerable<FixedNotificationDto>> FindAll(FixedNotificationFilterDto filters);
        Task Edit(FixedNotificationDto model);
        Task<FixedNotificationDto> GetById(Guid id);
        Task ResolveAll(FixedNotificationFilterDto filter, string comment);
    }
}
