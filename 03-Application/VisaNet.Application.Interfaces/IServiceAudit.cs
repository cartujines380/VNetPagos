using System;
using System.Collections.Generic;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceAudit
    {
        IEnumerable<LogDto> GetDetails(Guid id);
        IEnumerable<AuditTransactionLogDto> GetDataForTable(AuditFilterDto filterDto);
        IEnumerable<AuditExcelDto> ExcelExport(AuditFilterDto filterDto);
    }
}