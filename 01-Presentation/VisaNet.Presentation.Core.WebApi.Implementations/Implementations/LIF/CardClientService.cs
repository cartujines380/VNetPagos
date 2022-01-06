using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Lif.Domain.EntitesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.LIF
{
    public class CardClientService : WebApiClientService, ICardClientService
    {
        public CardClientService(ITransactionContext transactionContext)
            : base("Card", transactionContext)
        {
        }

        public Task<BinDto> GetCardInfo(string bin, bool includeIssuingCompany)
        {
            return WebApiClient.CallApiServiceAsync<BinDto>(new WebApiHttpRequestPost(BaseUri + "/GetCardInfo", TransactionContext, new { bin, includeIssuingCompany }));
        }

        public Task<ICollection<BinDto>> GetNationalBins(bool includeIssuingCompany)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinDto>>(new WebApiHttpRequestPost(BaseUri + "/GetNationalBins", TransactionContext, new { includeIssuingCompany }));
        }

    }
}