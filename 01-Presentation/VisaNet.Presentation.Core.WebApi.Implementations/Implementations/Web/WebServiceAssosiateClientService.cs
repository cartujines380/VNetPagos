using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebServiceAssosiateClientService : WebApiClientService, IWebServiceAssosiateClientService
    {
        public WebServiceAssosiateClientService(ITransactionContext transactionContext)
            : base("WebServiceAssosiate", transactionContext)
        {

        }

        public Task<ICollection<ServiceAssociatedDto>> Get(ServiceFilterAssosiateDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri , TransactionContext, filterDto.GetFilterDictionary()));
        }
        public Task<IList<ServiceAssociatedDto>> GetDataForFrontList(ServiceFilterAssosiateDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<IList<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForFrontList", TransactionContext, filterDto.GetFilterDictionary()));
        }
        
        public Task<ServiceAssociatedDto> Find(Guid id)
        {
            return
               WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestGet(
                   BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<ServiceAssociatedDto> Create(ServiceAssociatedDto entity)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }

        public Task Edit(ServiceAssociatedDto service)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, service.Id, service));
        }

        public Task AddPayment(AutomaticPaymentDto automaticPayment)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/AddPayment", TransactionContext, automaticPayment));
        }

        public Task DeleteService(Guid serviceAssociatedId)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/DeleteService", TransactionContext, serviceAssociatedId));
        }

        public Task<ICollection<ServiceAssociatedDto>> GetServicesWithAutomaticPayment(ServiceFilterAssosiateDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesWithAutomaticPayment", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<ServiceAssociatedDto>> GetServicesActiveAutomaticPayment(ServiceFilterAssosiateDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesActiveAutomaticPayment", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task DeleteAutomaticPayment(Guid serviceAssosiatedId)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/DeleteAutomaticPayment", TransactionContext, serviceAssosiatedId));
        }

        public Task<Guid> IsServiceAssosiatedToUser(Guid userId, Guid serviceId, string[] refNumber)
        {
            return WebApiClient.CallApiServiceAsync<Guid>(new WebApiHttpRequestGet(
                 BaseUri + "/IsServiceAssosiatedToUser", TransactionContext, new Dictionary<string, string> {
                 { "userId", userId.ToString() }, { "serviceId", serviceId.ToString() },
                 { "ref1", refNumber[0] },{ "ref2", refNumber[1] },{ "ref3", refNumber[2] },{ "ref4", refNumber[3] },{ "ref5", refNumber[4] },{ "ref6", refNumber[5] }}));
        }

        public Task<ServiceAssociatedDto> ServiceAssosiatedToUser(Guid userId, Guid serviceId, string[] refNumber)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestGet(
                 BaseUri + "/ServiceAssosiatedToUser", TransactionContext, new Dictionary<string, string> {
                 { "userId", userId.ToString() }, { "serviceId", serviceId.ToString() },
                 { "ref1", refNumber[0] },{ "ref2", refNumber[1] },{ "ref3", refNumber[2] },{ "ref4", refNumber[3] },{ "ref5", refNumber[4] },{ "ref6", refNumber[5] }}));
        }

        public Task<ICollection<ServiceAssociatedDto>> GetServicesForBills(Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesForBills", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() } }));
        }

        public Task EditDescription(ServiceAssociatedDto service)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditDescription", TransactionContext, service.Id, service));
        }

        public Task<bool> HasAutomaticPaymentCreated(Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                 BaseUri + "/HasAutomaticPaymentCreated", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() } }));
        }

        public Task<bool> HasAsosiatedService(Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                 BaseUri + "/HasAsosiatedService", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() } }));
        }

        public Task<ServiceAssociatedDto> CreateOrUpdateDeleted(ServiceAssociatedDto entityDto)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestPost(
                 BaseUri + "/CreateOrUpdateDeleted", TransactionContext, entityDto));
        }
        
        public Task<CardDto> GenerateExternalId(Guid cardId)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestGet(
                BaseUri + "/GenerateExternalId", TransactionContext, new Dictionary<string, string> { { "cardId", cardId.ToString() } }));
        }

        public Task<CardDto> FindWithServices(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetWithServices", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<bool> DeleteCardFromService(CardServiceDataDto dto)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/DeleteCardFromService", TransactionContext, dto));
        }

        public Task<bool> AddCardToService(CardServiceDataDto dto)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/AddCardToService", TransactionContext, dto));
        }

        public Task<CybersourceCreateServiceAssociatedDto> ProccesDataFromCybersource(IDictionary<string, string> csDictionary)
        {
            return WebApiClient.CallApiServiceAsync<CybersourceCreateServiceAssociatedDto>(new WebApiHttpRequestPost(
                 BaseUri + "/ProccesDataFromCybersource", TransactionContext, csDictionary));
        }

        public Task<CybersourceCreateAppAssociationDto> ProccesDataFromCybersourceForApps(IDictionary<string, string> csDictionary)
        {
            return WebApiClient.CallApiServiceAsync<CybersourceCreateAppAssociationDto>(new WebApiHttpRequestPost(
                 BaseUri + "/ProccesDataFromCybersourceForApps", TransactionContext, csDictionary));
        }

        public Task<ServiceAssociatedDto> AssociateServiceToUserFromCardCreated(ServiceAssociatedDto dto)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestPost(
                 BaseUri + "/AssociateServiceToUserFromCardCreated", TransactionContext, dto));
        }

        public Task<ServiceAssociatedDto> GetServiceAssociatedDtoFromIdUserExternal(string idUserExternal, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<ServiceAssociatedDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetServiceAssociatedDtoFromIdUserExternal", TransactionContext, new Dictionary<string, string>
                    {
                        { "idUserExternal", idUserExternal }, { "idApp", idApp}
                
                    })
                );
        }


    }
}
