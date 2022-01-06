using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class WebhookLogClientService : WebApiClientService, IWebhookLogClientService
    {
        public WebhookLogClientService(IWebServiceTransactionContext transactionContext)
            : base("WebhookLog", transactionContext)
        {
        }

        public Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookRegistrationDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookRegistrationsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookRegistrationsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WebhookDownDto>> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookDownDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookDownsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookDownsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookNewAssociationDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookNewAssociationsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookNewAssociationsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<WebhookNewAssociationDto> GetWebhookNewAssociation(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<WebhookNewAssociationDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookNewAssociation", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<WebhookRegistrationDto> GetWebhookRegistration(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetWebhookRegistration", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

    }
}