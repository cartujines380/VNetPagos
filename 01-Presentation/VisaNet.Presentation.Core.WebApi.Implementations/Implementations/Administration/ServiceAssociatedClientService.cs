using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ServiceAssociatedClientService : WebApiClientService, IServiceAssociatedClientService
    {
        public ServiceAssociatedClientService(ITransactionContext transactionContext)
            : base("ServiceAssociated", transactionContext)
        {

        }

        //public Task<ICollection<ServiceAssociatedDto>> Get(ReportsServicesAssociatedFilterDto filterDto)
        //{
        //    return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filterDto.GetFilterDictionary()));
        //}

        public Task<ICollection<ServiceAssociatedViewDto>> ReportsServicesAssociatedDataFromDbView(ReportsServicesAssociatedFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedViewDto>>(new WebApiHttpRequestGet(BaseUri + "/ReportsServicesAssociatedDataFromDbView", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> ReportsServicesAssociatedDataCount(ReportsServicesAssociatedFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/ReportsServicesAssociatedDataCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<AutomaticPaymentsViewDto>> ReportsAutomaticPaymentsDataFromDbView(ReportsAutomaticPaymentsFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AutomaticPaymentsViewDto>>(new WebApiHttpRequestGet(BaseUri + "/ReportsAutomaticPaymentsDataFromDbView", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> ReportsAutomaticPaymentsDataCount(ReportsAutomaticPaymentsFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/ReportsAutomaticPaymentsDataCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ServiceAssociatedDto> ServiceAssosiatedToUser(Guid userId, Guid serviceId, string[] refNumber)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestGet(
                 BaseUri + "/ServiceAssosiatedToUser", TransactionContext, new Dictionary<string, string> {
                 { "userId", userId.ToString() }, { "serviceId", serviceId.ToString() },
                 { "ref1", refNumber[0] },{ "ref2", refNumber[1] },{ "ref3", refNumber[2] },{ "ref4", refNumber[3] },{ "ref5", refNumber[4] },{ "ref6", refNumber[5] }}));
        }

        public Task<ServiceAssociatedDto> Find(Guid id)
        {
            return
               WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestGet(
                   BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<ICollection<ServiceAssociatedDto>> GetByServiceId(Guid id)
        {
            return
               WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(
                   BaseUri + "/GetByServiceId", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
    }
}
