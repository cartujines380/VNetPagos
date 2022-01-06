using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebAnonymousUserClientService : WebApiClientService, IWebAnonymousUserClientService
    {
        public WebAnonymousUserClientService(ITransactionContext transactionContext)
            : base("WebAnonymousUser", transactionContext)
        {

        }

        public Task<AnonymousUserDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<AnonymousUserDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<AnonymousUserDto> FindByEmail(string email)
        {
            return
                WebApiClient.CallApiServiceAsync<AnonymousUserDto>(new WebApiHttpRequestGet(
                    BaseUri + "/GetByEmail", TransactionContext, new Dictionary<string, string> { { "email", email } }));
        }

        public Task<AnonymousUserDto> Create(AnonymousUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<AnonymousUserDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }

        public Task Edit(AnonymousUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, entity));
        }

        public Task<AnonymousUserDto> CreateOrEditAnonymousUser(AnonymousUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<AnonymousUserDto>(new WebApiHttpRequestPost(
                BaseUri + "/CreateOrEditAnonymousUser", TransactionContext, entity));
        }
    }
}
