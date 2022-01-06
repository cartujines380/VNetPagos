using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebBinClientService : WebApiClientService, IWebBinClientService
    {
        public WebBinClientService(ITransactionContext transactionContext)
            : base("WebBin", transactionContext)
        {

        }

        public Task<BinDto> Find(int value)
        {
            return
                WebApiClient.CallApiServiceAsync<BinDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "value", value.ToString() } }));
        }

        public Task<ICollection<BinDto>> GetBinsFromMask(IList<int> mask)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinDto>>(new WebApiHttpRequestPost(BaseUri + "/GetBinsFromMask", TransactionContext, mask));
        }

        public Task<BinDto> FindByGuid(Guid cardId)
        {
            return WebApiClient.CallApiServiceAsync<BinDto>(new WebApiHttpRequestGet(BaseUri + "/FindByGuid", TransactionContext, new Dictionary<string, string> { { "cardId", cardId.ToString() } }));
        }
    }
}
