using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceFixedNotification : IService<FixedNotification, FixedNotificationDto>
    {
        IEnumerable<FixedNotificationDto> GetDataForMenu();
        IEnumerable<FixedNotificationDto> GetDataForTable(FixedNotificationFilterDto filters);
        void ResolveAll(ResolveAllFixedDto resolveAllFixedDto);
        string ExceptionMsg(Exception exception);
    }
}
