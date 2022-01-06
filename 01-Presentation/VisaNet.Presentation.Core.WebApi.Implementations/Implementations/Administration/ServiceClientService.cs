using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ServiceClientService : WebApiClientService, IServiceClientService
    {
        public ServiceClientService(ITransactionContext transactionContext)
            : base("Service", transactionContext)
        {
        }

        public Task<ICollection<ServiceDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<ServiceDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<ServiceDto>> GetDataForList(Guid serviceId, bool container)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForList", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() }, { "container", container.ToString() } }));
        }

        public Task<ServiceDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ServiceDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(ServiceDto service)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, service));
        }

        public Task Edit(ServiceDto service)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, service.Id, service));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task ChangeStatus(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri + "/ChangeStatus", TransactionContext, id));
        }

        public Task<ICollection<ServiceDto>> GetServicesLigthWithoutChildens(Guid containerId, GatewayEnumDto? gatewayEnumDto = null)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesLigthWithoutChildens",
                TransactionContext, new Dictionary<string, string>() { { "containerId", containerId.ToString() }, { "gatewayEnumDto", gatewayEnumDto.ToString() } }));
        }

        public Task<ICollection<ServiceDto>> GetServicesFromContainer(Guid containerId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesFromContainer",
                 TransactionContext, new Dictionary<string, string>() { { "containerId", containerId.ToString() } }));
        }

        public Task<IEnumerable<ReportsUsersVonDto>> GetDataForReportsUsersVon(ReportsUserVonFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<ReportsUsersVonDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForReportsUsersVon",
                 TransactionContext, filter.GetFilterDictionary()));
        }

        public Task<int> GetDataForReportsUsersVonCount(ReportsUserVonFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForReportsUsersVonCount",
                TransactionContext, filter.GetFilterDictionary()));
        }

        public Task<IEnumerable<CardVonDto>> GetVonUsersCards(Guid userId, Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<CardVonDto>>(new WebApiHttpRequestGet(BaseUri + "/GetVonUsersCards",
               TransactionContext, new Dictionary<string, string>() { { "userId", userId.ToString() }, { "serviceId", serviceId.ToString() } }));
        }
    }
}