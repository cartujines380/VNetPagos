using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.WebService
{
    public class WsBankClientService: WebApiClientService, IWsBankClientService
    {
        public WsBankClientService(IWebServiceTransactionContext transactionContext)
            : base("WebServiceBank", transactionContext)
        {

        }

        public Task<ICollection<ServiceDto>> AllServices()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestPut(BaseUri + "/AllServices", TransactionContext));
        }

        public Task<WsBankBillsResultDto> GetBills(WsBankBillsInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBankBillsResultDto>(new WebApiHttpRequestPut(BaseUri + "/GetBills", TransactionContext, dto));
        }

        public Task<WsBankPaymentResultDto> Payment(WsBankPaymentInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBankPaymentResultDto>(new WebApiHttpRequestPut(BaseUri + "/Payment", TransactionContext, dto));
        }

        public Task<WsBankReverseResultDto> ReversePayment(WsBankReverseInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBankReverseResultDto>(new WebApiHttpRequestPut(BaseUri + "/ReversePayment", TransactionContext, dto));
        }

        public Task<WsBankBillsResultDto> PreprocessPayment(WsBankPreprocessPaymentInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBankBillsResultDto>(new WebApiHttpRequestPut(BaseUri + "/PreprocessPayment", TransactionContext, dto));
        }

        public Task<List<CyberSourceExtraDataDto>> CalculateDiscount(WsBankPreprocessPaymentInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<List<CyberSourceExtraDataDto>>(new WebApiHttpRequestPut(BaseUri + "/CalculateDiscount", TransactionContext, dto));
        }

        public Task<ICollection<PaymentDto>> GetPayments(WsBankSearchPaymentsInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PaymentDto>>(new WebApiHttpRequestPut(BaseUri + "/GetPayments", TransactionContext, dto));
        }
    }
}
