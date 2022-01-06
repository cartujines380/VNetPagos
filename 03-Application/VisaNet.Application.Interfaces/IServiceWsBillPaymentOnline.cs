using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsBillPaymentOnline : IService<WsBillPaymentOnline, WsBillPaymentOnlineDto>
    {
        ICollection<WsBillPaymentOnlineDto> GetBillPaymentsOnlineForTable(ReportsIntegrationFilterDto filterDto);
        int GetBillPaymentsOnlineForTableCount(ReportsIntegrationFilterDto filterDto);
        WsBillPaymentOnlineDto GetByIdOperation(string idOperation, string idApp);
        bool IsOperationIdRepited(string idOperation, string idApp);
    }
}
