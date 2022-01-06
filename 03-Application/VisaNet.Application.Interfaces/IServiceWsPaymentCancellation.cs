using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsPaymentCancellation : IService<WsPaymentCancellation, WsPaymentCancellationDto>
    {
        ICollection<WsPaymentCancellationDto> GetPaymentCancellationsForTable(ReportsIntegrationFilterDto filterDto);
        int GetPaymentCancellationsForTableCount(ReportsIntegrationFilterDto filterDto);
        WsPaymentCancellationDto GetByIdOperation(string idOperation);
        bool IsOperationIdRepited(string idOperation, string idApp);
    }
}
