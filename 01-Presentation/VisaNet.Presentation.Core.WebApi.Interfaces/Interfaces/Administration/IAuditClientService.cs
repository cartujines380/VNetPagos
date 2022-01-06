using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IAuditClientService
    {
        Task<ICollection<AuditTransactionLogDto>> FindAll(AuditFilterDto filter);
        Task<ICollection<AuditLogDto>> GetLogs(Guid id);
        Task<ICollection<AuditExcelDto>> ExcelExport(AuditFilterDto filter);
        Task<ICollection<ChangeTrackerExcelDto>> ChangeLogExcelExport(ChangeTrackerFilterDto filter);
    }
}
