using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class CyberSourceAcknowledgementClientService : WebApiClientService, ICyberSourceAcknowledgementClientService
    {
        public CyberSourceAcknowledgementClientService(ITransactionContext transactionContext)
            : base("CyberSourceAcknowledgement", transactionContext)
        {
        }

        public Task Process(CyberSourceAcknowledgementDto post)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/Process", TransactionContext, post));
        }
    }
}