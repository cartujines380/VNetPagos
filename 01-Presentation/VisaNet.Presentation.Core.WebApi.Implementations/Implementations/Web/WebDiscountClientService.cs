using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebDiscountClientService : WebApiClientService, IWebDiscountClientService
    {
        public WebDiscountClientService(ITransactionContext transactionContext)
            : base("WebDiscount", transactionContext)
        {

        }

        public Task<List<CyberSourceExtraDataDto>> GetDiscount(DiscountQueryDto discountQuery)
        {
            return WebApiClient.CallApiServiceAsync<List<CyberSourceExtraDataDto>>(new WebApiHttpRequestPost(BaseUri, TransactionContext, discountQuery));
        }

        public Task<bool> ValidateBin(int binNumber, Guid serviceId)
        {

            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri, TransactionContext, 
                new Dictionary<string, string> {
                    { "binNumber" ,  binNumber.ToString() },
                    { "serviceId" ,  serviceId.ToString() }                    
                } ));
        }
    }
}
