using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceIntegration
    {
        TransactionResult MakePayment(WsBillPaymentOnlineDto data);
        TransactionResult CancelPayment(WsPaymentCancellationDto data);
        WebServiceTransactionHistoryDto TransactionsHistory(WsBillQueryDto dto);
        TransactionCommerceResult GetServices(WsCommerceQueryDto dto);
        TransactionResult RemoveCard(WsCardRemoveDto dto);
        IList<WebhookNewAssociationDto> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto);
    }
}
