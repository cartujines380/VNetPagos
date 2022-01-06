using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService
{
    public interface IWsExternalAppClientService
    {
        Task<ICollection<HighwayBillDto>> AddNewBills(WebServiceBillsInputDto dto);
        Task<int> DeleteBills(WebServiceBillsDeleteDto dto);
        Task<List<WebServiceApplicationClientDto>> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto);
        Task<WebServiceBillStatusDto> BillsStatus(WsBillQueryDto dto);
        Task<string> GetCertificateThumbprint(WebServiceClientInputDto dto);
        Task<string> GetCertificateThumbprintIdApp(WebServiceClientInputDto dto);
        Task<TransactionResult> MakePayment(WsBillPaymentOnlineDto dto);
        Task<TransactionResult> CancelPayment(WsPaymentCancellationDto dto);
        Task<WebServiceTransactionHistoryDto> TransactionsHistory(WsBillQueryDto dto);
        Task<TransactionCommerceResult> GetServices(WsCommerceQueryDto dto);
        Task<TransactionResult> RemoveCard(WsCardRemoveDto dto);
        Task<IList<WebhookNewAssociationDto>> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto);
    }
}
