using System;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Lif.Domain.EntitesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.LIF
{
    public class DiscountClientService : WebApiClientService, IDiscountClientService
    {
        public DiscountClientService(ITransactionContext transactionContext)
            : base("Discount", transactionContext)
        {
        }

        public Task<DiscountCalculationDto> CalculateDiscount(BillDto bill, string bin)
        {
            return WebApiClient.CallApiServiceAsync<DiscountCalculationDto>(new WebApiHttpRequestPost(BaseUri + "/CalculateDiscount", TransactionContext, new { bill, bin }));
        }

        public Task<DiscountCalculationDto> CalculateDiscount(BillDto bill, string bin, Guid serviceId)
        {                                                                                                          //VER SI USAR ESTE VERBO HTTP O CAMBIAR NOMBRE
            return WebApiClient.CallApiServiceAsync<DiscountCalculationDto>(new WebApiHttpRequestPost(BaseUri + "/CalculateDiscountForService", TransactionContext, new { bill, bin, serviceId }));
        }
    }
}