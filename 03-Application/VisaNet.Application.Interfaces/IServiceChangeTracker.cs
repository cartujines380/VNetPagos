using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using AuditLogDto = VisaNet.Domain.EntitiesDtos.ChangeTracker.AuditLogDto;

namespace VisaNet.Application.Interfaces
{
    using VisaNet.Common.ChangeTracker.Models;

    public interface IServiceChangeTracker : IService<AuditLog, AuditLogDto>
    {
        IEnumerable<AuditLogDto> GetDataForTable(ChangeTrackerFilterDto filters);
        int[] Count(ChangeTrackerFilterDto filters);
        AuditLogDto GetById(int id);
        IEnumerable<string> GetEntities();
        IEnumerable<ChangeTrackerExcelDto> ChangeLogExcelExport(ChangeTrackerFilterDto filterDto);
    }
}
