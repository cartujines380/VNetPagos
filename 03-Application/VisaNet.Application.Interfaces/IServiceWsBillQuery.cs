using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsBillQuery : IService<WsBillQuery, WsBillQueryDto>
    {
        ICollection<WsBillQueryDto> GetBillQueriesForTable(ReportsIntegrationFilterDto filterDto);
        int GetBillQueriesForTableCount(ReportsIntegrationFilterDto filterDto);
    }
}
