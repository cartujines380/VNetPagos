using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ReportsHighwayService : WebApiClientService, IReportsHighwayService
    {
        public ReportsHighwayService(ITransactionContext transactionContext)
            : base("ReportsHighway", transactionContext)
        {
        }

        public Task<ICollection<HighwayEmailDto>> GetHighwayEmailsReports(ReportsHighwayEmailFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HighwayEmailDto>>(new WebApiHttpRequestPost(BaseUri + "/GetHighwayEmailsReports", TransactionContext, filter));
        }

        public Task<int> GetHighwayEmailsReportsCount(ReportsHighwayEmailFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetHighwayEmailsReportsCount", TransactionContext, filter));
        }

        public Task<ICollection<HighwayBillDto>> GetHighwayBillReports(ReportsHighwayBillFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HighwayBillDto>>(new WebApiHttpRequestPost(BaseUri + "/GetHighwayBillReports", TransactionContext, filter));
        }

        public Task<int> GetHighwayBillReportsCount(ReportsHighwayBillFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetHighwayBillReportsCount", TransactionContext, filter));
        }

        public Task<ICollection<HighwayEmailErrorDto>> ProccessEmailFile(HighwayEmailDto email)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HighwayEmailErrorDto>>(new WebApiHttpRequestPut(BaseUri + "/ProccessEmailFile", TransactionContext, email));
        }

        public Task<ICollection<HighwayEmailErrorDto>> ProccessEmailFileExternalSoruce(HighwayEmailDto email)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HighwayEmailErrorDto>>(new WebApiHttpRequestPut(BaseUri + "/ProccessEmailFileExternalSoruce", TransactionContext, email));
        }

        public Task<HighwayEmailDto> GetHighwayEmail(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<HighwayEmailDto>(new WebApiHttpRequestPost(BaseUri + "/GetHighwayEmail", TransactionContext, id));
        }

    }
}