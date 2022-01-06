using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ServiceValidatorClientService : WebApiClientService, IServiceValidatorClientService
    {
        public ServiceValidatorClientService(ITransactionContext transactionContext)
            : base("ServiceValidator", transactionContext)
        {
        }

        public Task<bool> ValidateLinkService(Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/ValidateLinkService", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task<bool> ValidateLinkService(ServiceDto service)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(
                BaseUri + "/ValidateLinkService", TransactionContext, service));
        }

        public Task<bool> ValidateVisaNetOnService(Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/ValidateLinkService", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task<bool> ValidateVisaNetOnService(ServiceDto service)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(
                BaseUri + "/ValidateVisaNetOnService", TransactionContext, service));
        }

        public Task<bool> ValidateDebitService(Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/ValidateLinkService", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task<bool> ValidateDebitService(ServiceDto service)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(
                BaseUri + "/ValidateDebitService", TransactionContext, service));
        }

        public Task<bool> ValidateServiceGatewayActive(Guid serviceId, GatewayEnumDto gatewayEnum)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                BaseUri + "/ValidateLinkService", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() }, { "gatewayEnum", gatewayEnum.ToString() } }));
        }

    }
}