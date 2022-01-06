using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VisaNet.Common.Security;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using VisaNet.Utilities.Cybersource;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class CyberSourceAccessClientService : WebApiClientService, ICyberSourceAccessClientService
    {
        public CyberSourceAccessClientService(ITransactionContext transactionContext)
            : base("CyberSourceAccess", transactionContext)
        {

        }

        public Task<IDictionary<string, string>> GenerateKeys(IGenerateToken item)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return WebApiClient.CallApiServiceAsync<IDictionary<string, string>>(new WebApiHttpRequestPost(
                             BaseUri + "/GenerateKeys", TransactionContext, item, settings));
        }

        public Task<string> GetCardNumberByToken(CybersourceGetCardNameDto dto)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestGet(BaseUri + "/GetCardNumberByToken", TransactionContext, dto.ToDictionary()));
        }
    }
}
