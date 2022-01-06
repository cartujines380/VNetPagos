using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsCardRemove : IService<WsCardRemove, WsCardRemoveDto>
    {
        ICollection<WsCardRemoveDto> GetCardRemovesForTable(ReportsIntegrationFilterDto filterDto);
        int GetCardRemovesForTableCount(ReportsIntegrationFilterDto filterDto);
        WsCardRemoveDto GetByIdOperation(string idOperation);
        bool IsOperationIdRepited(string idOperation, string idApp);
    }
}
