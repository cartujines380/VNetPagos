using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebQuotationClientService : WebApiClientService, IWebQuotationClientService
    {
        public WebQuotationClientService(ITransactionContext transactionContext)
            : base("WebQuotation", transactionContext)
        {

        }

        public Task<ICollection<QuotationDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<QuotationDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

    }
}