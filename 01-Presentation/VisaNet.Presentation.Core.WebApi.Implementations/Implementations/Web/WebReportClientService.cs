using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{


    public class WebReportClientService : WebApiClientService, IWebReportClientService
    {
        public WebReportClientService(ITransactionContext transactionContext)
            : base("WebReport", transactionContext)
        {

        }

        public Task<List<List<object>>> PieChart(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<List<List<object>>>(new WebApiHttpRequestPost(BaseUri + "/PieChart", TransactionContext, filtersDto));
        }

        public Task<List<List<object>>> LineChart(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<List<List<object>>>(new WebApiHttpRequestPost(BaseUri + "/LineChart", TransactionContext, filtersDto));
        }

        public Task<List<ServiceCategoryDto>> ServicesCategories(Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<List<ServiceCategoryDto>>(new WebApiHttpRequestGet(BaseUri + "/ServicesCategories", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() } }));
        }

        public Task<Dictionary<Guid, List<ServiceDto>>> ServicesWithPayments(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<Dictionary<Guid, List<ServiceDto>>>(new WebApiHttpRequestGet(BaseUri + "/ServicesWithPayments", TransactionContext, new Dictionary<string, string> { { "userId", id.ToString() } }));
        }
    }
}
