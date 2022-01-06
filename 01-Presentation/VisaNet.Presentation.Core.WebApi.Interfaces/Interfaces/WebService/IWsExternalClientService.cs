using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService
{
    public interface IWsExternalClientService
    {
        Task<ICollection<WsBillQueryDto>> GetBillQueries();
        Task<WsBillQueryDto> CreateBillQuery(WsBillQueryDto dto);
        Task EditBillQuery(WsBillQueryDto dto);

        Task<ICollection<WsBillPaymentOnlineDto>> GetBillPaymentsOnline();
        Task<WsBillPaymentOnlineDto> CreateBillPaymentOnline(WsBillPaymentOnlineDto dto);
        Task EditBillPaymentOnline(WsBillPaymentOnlineDto dto);
        Task<bool> BillPaymentOnlineByOperationIdUsed(string idOperation);
        Task<bool> BillPaymentOnlineByOperationIdUsed(string idOperation, string idApp);

        Task<ICollection<WsCommerceQueryDto>> GetCommerceQueries();
        Task<WsCommerceQueryDto> CreateCommerceQuery(WsCommerceQueryDto dto);
        Task EditCommerceQuery(WsCommerceQueryDto dto);

        Task<ICollection<WsPaymentCancellationDto>> GetPaymentCancellations();
        Task<WsPaymentCancellationDto> CreatePaymentCancellation(WsPaymentCancellationDto dto);
        Task EditPaymentCancellation(WsPaymentCancellationDto dto);

        Task<bool> PaymentCancellationsByOperationIdUsed(string idOperation);

        Task<bool> PaymentCancellationsIsOperationIdUsed(string idOperation, string idApp);
        Task<bool> BillPaymentOnlineIsOperationIdUsed(string idOperation, string idApp);

        Task<bool> CardRemoveIsOperationIdUsed(string idOperation, string idApp);
        Task<WsCardRemoveDto> CreateCardRemove(WsCardRemoveDto dto);
        Task EditCardRemove(WsCardRemoveDto dto);
    }
}