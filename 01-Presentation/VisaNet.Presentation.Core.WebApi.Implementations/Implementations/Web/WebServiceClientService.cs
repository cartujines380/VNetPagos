using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebServiceClientService : WebApiClientService, IWebServiceClientService
    {
        public WebServiceClientService(ITransactionContext transactionContext)
            : base("WebService", transactionContext)
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
        public Task<ServiceDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ServiceDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        public Task<ServiceDto> GetServiceByUrlName(string nameUrl)
        {
            return WebApiClient.CallApiServiceAsync<ServiceDto>(new WebApiHttpRequestGet(BaseUri + "/GetServiceByUrlName", TransactionContext, new Dictionary<string, string> { { "nameUrl", nameUrl } }));
        }
        public Task<ICollection<ServiceDto>> ServicesWithImages()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesWithImage", TransactionContext));
        }

        public Task<ICollection<CardDto>> GetEnableCards(Guid userId, Guid serviceId)
        {
            return
                WebApiClient.CallApiServiceAsync<ICollection<CardDto>>(new WebApiHttpRequestGet(
                    BaseUri + "/GetEnableCards", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() }, { "serviceId", serviceId.ToString() } }));
        }

        //public Task<ICollection<ServiceDto>> ServicesForPayment(BaseFilter filtersDto)
        //{
        //    var filter = filtersDto as ServiceFilterDto;
        //    filter.OnlyToPay = true;
        //    filter.ServiceWithOutContainerAndContainer = true;
        //    return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filter.GetFilterDictionary()));
        //}

        //public Task<ICollection<ServiceDto>> ServicesForAssociation(BaseFilter filtersDto)
        //{
        //    var filter = filtersDto as ServiceFilterDto;
        //    filter.OnlyToAssociate = true;
        //    filter.ServiceWithOutContainerAndContainer = true;
        //    return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filter.GetFilterDictionary()));
        //}

        public Task<ICollection<ServiceDto>> GetServicesEnableAssociation()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesEnableAssociation", TransactionContext));
        }
        public Task<ICollection<ServiceDto>> GetServicesPaymentPrivate()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesPaymentPrivate", TransactionContext));
        }
        public Task<ICollection<ServiceDto>> GetServicesPaymentPublic()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesPaymentPublic", TransactionContext));
        }
        public Task<ICollection<ServiceDto>> GetServicesProvider(Guid? serviceId = null)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesProvider", TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }
        public Task<ICollection<ServiceDto>> GetServicesFromContainer(Guid containerId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesFromContainer",
                TransactionContext, new Dictionary<string, string> { { "containerId", containerId.ToString() } }));
        }

        public Task<ICollection<GatewayDto>> GetGateways()
        {
            return
                WebApiClient.CallApiServiceAsync<ICollection<GatewayDto>>(new WebApiHttpRequestGet(BaseUri + "/GetGateways", TransactionContext));
        }

        public Task<string> GetCertificateThumbprintIdApp(string idApp)
        {
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestGet(BaseUri + "/GetCertificateThumbprintIdApp", TransactionContext, new Dictionary<string, string> { { "friendlyname", idApp } }));
        }

        public Task<bool> IsBinAssociatedToService(int binValue, Guid serviceId)
        {
            return
               WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/IsBinAssociatedToService", TransactionContext, new Dictionary<string, string> { { "BinValue", binValue.ToString() }, { "id", serviceId.ToString() } }));
        }

        public Task<List<ServiceDto>> GetServicesFromMerchand(string idApp, string merchandId, GatewayEnumDto gateway)
        {
            return
                WebApiClient.CallApiServiceAsync<List<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetServicesFromMerchand", TransactionContext,
                    new Dictionary<string, string> { { "idApp", idApp }, { "merchandId", merchandId }, { "gateway", gateway.ToString() } }));
        }

        //public Task<Tuple<ServiceDto, string, string>> FindAndValidateServiceForVisaNetOnAssociation(string idApp)
        //{
        //    return WebApiClient.CallApiServiceAsync<Tuple<ServiceDto, string, string>>(new WebApiHttpRequestGet(BaseUri +
        //        "/FindAndValidateServiceForVisaNetOnAssociation", TransactionContext,
        //        new Dictionary<string, string> { { "idApp", idApp } }));
        //}

        //public Task<Tuple<ServiceDto, string, string>> FindAndValidateServiceForVisaNetOnPayment(string idApp, string merchantId)
        //{
        //    return WebApiClient.CallApiServiceAsync<Tuple<ServiceDto, string, string>>(new WebApiHttpRequestGet(BaseUri +
        //        "/FindAndValidateServiceForVisaNetOnPayment", TransactionContext,
        //        new Dictionary<string, string> { { "idApp", idApp }, { "merchantId", merchantId } }));
        //}

    }
}