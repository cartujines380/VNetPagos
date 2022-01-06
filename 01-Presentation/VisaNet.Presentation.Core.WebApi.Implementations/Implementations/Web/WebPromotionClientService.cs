using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebPromotionClientService : WebApiClientService,IWebPromotionClientService
    {

        public WebPromotionClientService(ITransactionContext transactionContext)
            : base("WebPromotion",transactionContext)
        {

        }

        public Task<ICollection<PromotionDto>> FindActive()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PromotionDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }
    }
}
