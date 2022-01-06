using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ServiceCategoryClientService : WebApiClientService, IServiceCategoryClientService
    {
        public ServiceCategoryClientService(ITransactionContext transactionContext)
            : base("ServiceCategory", transactionContext)
        {

        }

        public Task<ICollection<ServiceCategoryDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceCategoryDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<ServiceCategoryDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceCategoryDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ServiceCategoryDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ServiceCategoryDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(ServiceCategoryDto serviceCategory)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, serviceCategory));
        }

        public Task Edit(ServiceCategoryDto serviceCategory)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, serviceCategory.Id, serviceCategory));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }
    }
}
