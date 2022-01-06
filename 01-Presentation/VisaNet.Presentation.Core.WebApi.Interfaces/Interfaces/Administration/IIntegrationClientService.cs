using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IIntegrationClientService
    {
        Task<ICollection<WsBillQueryDto>> GetBillQueriesForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetBillQueriesForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WsBillPaymentOnlineDto>> GetBillPaymentsOnlineForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetBillPaymentsOnlineForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WsCommerceQueryDto>> GetCommerceQueriesForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetCommerceQueriesForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WsPaymentCancellationDto>> GetPaymentCancellationsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetPaymentCancellationsForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WsCardRemoveDto>> GetCardRemovesForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetCardRemovesForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<WsBillPaymentOnlineDto> GetWsBillPaymentOnline(Guid id);
    }
}