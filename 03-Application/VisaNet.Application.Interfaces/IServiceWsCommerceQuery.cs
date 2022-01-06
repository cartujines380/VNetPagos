using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsCommerceQuery : IService<WsCommerceQuery, WsCommerceQueryDto>
    {
        ICollection<WsCommerceQueryDto> GetCommerceQueriesForTable(ReportsIntegrationFilterDto filterDto);
        int GetCommerceQueriesForTableCount(ReportsIntegrationFilterDto filterDto);
    }
}
