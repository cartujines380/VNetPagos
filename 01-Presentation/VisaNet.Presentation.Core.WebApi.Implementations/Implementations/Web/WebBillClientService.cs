using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebBillClientService : WebApiClientService, IWebBillClientService
    {
        public WebBillClientService(ITransactionContext transactionContext)
            : base("WebBill", transactionContext)
        {

        }

        public Task<ICollection<BillDto>> FindAll(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType, string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4, string referenceNumber5, string referenceNumber6, int serviceDepartament)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BillDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext,
                new Dictionary<string, string>
                {
                    { "gateway", gateway.ToString() },
                    { "serviceId", serviceId.ToString() },
                    { "gatewayReference", gatewayReference }, 
                    { "serviceType", serviceType }, 
                    { "referenceNumber", referenceNumber },
                    { "referenceNumber2", referenceNumber2 },
                    { "referenceNumber3", referenceNumber3 }, 
                    { "referenceNumber4", referenceNumber4 },
                    { "referenceNumber5", referenceNumber5 },
                    { "referenceNumber6", referenceNumber6 },
                    { "serviceDepartament", serviceDepartament.ToString() }
                }));
        }

        public Task<ICollection<BillDto>> GetBillsForDashboard(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType, string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4, string referenceNumber5, string referenceNumber6, int serviceDepartament)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BillDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBillsForDashboard", TransactionContext,
                new Dictionary<string, string>
                {
                    { "gateway", gateway.ToString() },
                    { "serviceId", serviceId.ToString() },
                    { "gatewayReference", gatewayReference }, 
                    { "serviceType", serviceType }, 
                    { "referenceNumber", referenceNumber },
                    { "referenceNumber2", referenceNumber2 },
                    { "referenceNumber3", referenceNumber3 }, 
                    { "referenceNumber4", referenceNumber4 },
                    { "referenceNumber5", referenceNumber5 },
                    { "referenceNumber6", referenceNumber6 },
                    { "serviceDepartament", serviceDepartament.ToString() }
                }));
        }

        public Task<ApplicationUserBillDto> GetBillsForRegisteredUser(IBillFilterDto billFilterDto)
        {
            return WebApiClient.CallApiServiceAsync<ApplicationUserBillDto>(new WebApiHttpRequestPost(
                BaseUri + "/GetBillsForRegisteredUser", TransactionContext, billFilterDto));
        }

        public Task<AnonymousUserBillDto> GetBillsForAnonymousUser(IBillFilterDto billFilterDto)
        {
            return WebApiClient.CallApiServiceAsync<AnonymousUserBillDto>(new WebApiHttpRequestPost(
                BaseUri + "/GetBillsForAnonymousUser", TransactionContext, billFilterDto));
        }

        public Task<ICollection<BillDto>> FindAllWithServices(GatewayEnumDto gateway, string gatewayReference, string serviceType, Guid serviceAssosiatedId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BillDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext,
                new Dictionary<string, string>
                {
                    { "gateway", gateway.ToString() },
                    { "gatewayReference", gatewayReference }, 
                    { "serviceType", serviceType }, 
                    { "serviceAssosiatedId", serviceAssosiatedId.ToString() }
                }));
        }
        
        public Task<BillDto> ChekBills(string lines, int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            return WebApiClient.CallApiServiceAsync<BillDto>(new WebApiHttpRequestGet(BaseUri + "/ChekBills", TransactionContext,
                new Dictionary<string, string>
                {
                    { "lines", lines },
                    { "idPadron", idPadron.ToString() }, 
                    { "depto", depto.ToString() },
                    { "gateway", gateway.ToString() },
                    { "param", param },
                }));
        }
        public Task<BillDto> GeneratePreBill(GeneratePreBillDto generatePreBillDto)
        {
            return
                WebApiClient.CallApiServiceAsync<BillDto>(new WebApiHttpRequestPost(BaseUri + "/GeneratePreBill", TransactionContext, generatePreBillDto));

        }

        public Task<BillDto> GetInputAmountBill(InputAmountBillFilterDto billFilterDto)
        {
            return WebApiClient.CallApiServiceAsync<BillDto>(new WebApiHttpRequestPost(BaseUri + "/GetInputAmountBill", TransactionContext, billFilterDto));
        }

        public Task<bool> IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/IsBillExlternalIdRepitedByServiceId", TransactionContext,
                new Dictionary<string, string>{{ "serviceId", serviceId.ToString() },{ "billExternalId", billExternalId }}));
        }

        public Task<bool> IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/IsBillExlternalIdRepitedByMerchantId", TransactionContext,
                new Dictionary<string, string> { { "merchantId", merchantId }, { "billExternalId", billExternalId } }));
        }

        public Task<ICollection<BillDto>> GetBillsIdPadron(int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BillDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBillsIdPadron", TransactionContext,
                new Dictionary<string, string>
                {
                    { "idPadron", idPadron.ToString() },
                    { "depto", depto.ToString() },
                    { "gateway", gateway.ToString() },
                    { "param", param },
                }));
        }

        public Task<int> CheckAccount(IBillFilterDto billFilterDto)
                {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/CheckAccount", TransactionContext, billFilterDto));
        }

        public Task<ApplicationUserBillDto> GenerateAnnualPatenteForRegisteredUser(IBillFilterDto billFilterDto)
        {
            return WebApiClient.CallApiServiceAsync<ApplicationUserBillDto>(new WebApiHttpRequestPost(
                BaseUri + "/GenerateAnnualPatenteForRegisteredUser", TransactionContext, billFilterDto));
        }
        public Task<AnonymousUserBillDto> GenerateAnnualPatenteForAnonymousUser(IBillFilterDto billFilterDto)
        {
            return WebApiClient.CallApiServiceAsync<AnonymousUserBillDto>(new WebApiHttpRequestPost(
                BaseUri + "/GenerateAnnualPatenteForAnonymousUser", TransactionContext, billFilterDto));
        }
    }
}
