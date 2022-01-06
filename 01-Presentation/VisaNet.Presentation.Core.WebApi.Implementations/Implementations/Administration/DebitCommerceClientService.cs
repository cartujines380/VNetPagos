using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class DebitCommerceClientService : WebApiClientService, IDebitCommerceClientService
    {
        public DebitCommerceClientService(ITransactionContext transactionContext)
            : base("DebitCommerce", transactionContext)
        {

        }
        public Task UpdateCommerceDebitCatche()
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/UpdateCommerceDebitCatche", TransactionContext ));
        }

        public Task<ICollection<DebitRequestDto>> GetDebitSuscriptionList(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<DebitRequestDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDebitSuscriptionList",
                TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetDebitSuscriptionListCount(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDebitSuscriptionListCount",
                TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<DebitRequestDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<DebitRequestDto>(new WebApiHttpRequestGet(
                    BaseUri + "/FindSuscription", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<CustomerSiteCommerceDto> FindCommerce(int debitProductId)
        {
            return
                WebApiClient.CallApiServiceAsync<CustomerSiteCommerceDto>(new WebApiHttpRequestGet(
                    BaseUri + "/FindCommerce", TransactionContext, new Dictionary<string, string> { { "debitProductId", debitProductId.ToString() } }));
        }

        public Task<ICollection<DebitRequestExcelDto>> ExcelExportManualSynchronization()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<DebitRequestExcelDto>>(new WebApiHttpRequestPost(BaseUri + "/ExcelExportManualSynchronization", TransactionContext));
        }

    }
}
