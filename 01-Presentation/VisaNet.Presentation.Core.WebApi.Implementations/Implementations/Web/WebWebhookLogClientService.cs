using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebWebhookLogClientService : WebApiClientService, IWebWebhookLogClientService
    {
        public WebWebhookLogClientService(IWebServiceTransactionContext transactionContext)
            : base("WebWebhookLog", transactionContext)
        {

        }

        public Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrations()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookRegistrationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookRegistrations", TransactionContext));
        }

        public Task<WebhookRegistrationDto> CreateWebhookRegistration(WebhookRegistrationDto webhook)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestPut(BaseUri + "/PutWebhookRegistration", TransactionContext, webhook));
        }

        public Task<ICollection<WebhookDownDto>> GetWebhookDowns()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookDownDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookDowns", TransactionContext));
        }

        public Task CreateWebhookDown(WebhookDownDto webhook)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/PutWebhookDown", TransactionContext, webhook));
        }

        public Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociations()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookNewAssociationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookNewAssociations", TransactionContext));
        }

        public Task CreateWebhookNewAssociation(WebhookNewAssociationDto webhook)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/PutWebhookNewAssociation", TransactionContext, webhook));
        }

        public Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, string idapp)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(BaseUri + "/GetByIdOperation", TransactionContext, new Dictionary<string, string> { { "idOperation", idOperation }, { "idapp", idapp } }));
        }

        public Task<WebhookRegistrationDto> GetwebHookRegistrationsByIdOperation(string idOperation, Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<WebhookRegistrationDto>(new WebApiHttpRequestGet(BaseUri + "/GetwebHookRegistrationsByIdOperation", TransactionContext, new Dictionary<string, string> { { "idOperation", idOperation }, { "serviceId", serviceId.ToString()} }));
        }

        public Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookRegistrationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookRegistrationsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookRegistrationsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WebhookDownDto>> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookDownDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookDownsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookDownsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WebhookNewAssociationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookNewAssociationsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetWebhookNewAssociationsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<bool> WebhookRegistrationIsIdOperationRepited(string idOperation, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/WebhookRegistrationIsIdOperationRepited", TransactionContext, new Dictionary<string, string> { { "idOperation", idOperation }, { "idApp", idApp } }));
        }

        
    }
}