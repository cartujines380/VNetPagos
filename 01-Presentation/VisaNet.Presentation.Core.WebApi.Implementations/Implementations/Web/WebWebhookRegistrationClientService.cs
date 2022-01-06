using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebWebhookRegistrationClientService : WebApiClientService, IWebWebhookRegistrationClientService
    {
        public WebWebhookRegistrationClientService(IWebServiceTransactionContext transactionContext)
            : base("WebWebhookRegistration", transactionContext)
        {
        }

        public Task<WebhookAccessTokenDto> GenerateAccessToken(WebhookRegistrationDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WebhookAccessTokenDto>(new WebApiHttpRequestPost(BaseUri + "/GenerateAccessToken", TransactionContext, dto));
        }

        public Task<WebhookRegistrationDto> GetByAccessToken(WebhookAccessTokenDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestPost(BaseUri + "/GetByAccessToken", TransactionContext, dto));
        }

        public Task<bool> ValidateAccessToken(WebhookAccessTokenDto dto)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ValidateAccessToken", TransactionContext, dto));
        }

        public Task<WebhookRegistrationDto> FindById(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(
                BaseUri + "/FindById", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, string idapp)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetByIdOperation", TransactionContext, new Dictionary<string, string> { { "idOperation", idOperation }, { "idapp", idapp } }));
        }

        public Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetByIdOperation", TransactionContext, new Dictionary<string, string> { { "idOperation", idOperation }, { "serviceId", serviceId.ToString() } }));
        }

        public Task<bool> IsTokenActive(AccessTokenFilterDto dto)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/IsTokenActive", TransactionContext, dto.GetFilterDictionary()));
        }

        public Task<bool> SetAccessTokenAsPaid(Guid webhookRegistrationId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/SetAccessTokenAsPaid", TransactionContext, new Dictionary<string, string> { { "webhookRegistrationId", webhookRegistrationId.ToString() } }));
        }

    }
}