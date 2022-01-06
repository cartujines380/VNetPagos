using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class Tc33ClientService : WebApiClientService, ITc33ClientService
    {
        public Tc33ClientService(ITransactionContext transactionContext)
            : base("Tc33", transactionContext)
        {
        }

        public Task<IEnumerable<Tc33Dto>> GetDataForTable(ReportsTc33FilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<Tc33Dto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForTable", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<int> GetDataForTableCount(ReportsTc33FilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForTableCount", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<Tc33Dto> CreateProcess(Tc33Dto dto)
        {
            return WebApiClient.CallApiServiceAsync<Tc33Dto>(new WebApiHttpRequestPut(BaseUri + "/CreateProcess", TransactionContext, dto));
        }

        public Task<IList<string>> GetTC33Transactions(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<IList<string>>(new WebApiHttpRequestPost(BaseUri + "/GetTC33Transactions", TransactionContext, id));
        }

        public Task<Tc33Dto> GetTC33(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<Tc33Dto>(new WebApiHttpRequestPost(BaseUri + "/GetTC33", TransactionContext, id));

        }

        public Task<byte[]> GenerateDetailsFile(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<byte[]>(new WebApiHttpRequestPost(BaseUri + "/DownloadDetails", TransactionContext, id));
        }

        public Task<Tc33Dto> EditProcess(Tc33Dto dto)
        {
            return WebApiClient.CallApiServiceAsync<Tc33Dto>(new WebApiHttpRequestPost(BaseUri + "/EditProcess", TransactionContext, dto));
        }

        public Task<bool> WasAlreadyProccessed(string requestId)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/WasAlreadyProccessed", TransactionContext, new Dictionary<string, string>() { { "requestId", requestId } }));
        }

        public Task<LogDto> GetLogFromDb(string requestId)
        {
            return WebApiClient.CallApiServiceAsync<LogDto>(new WebApiHttpRequestGet(BaseUri + "/GetLogFromDb", TransactionContext, new Dictionary<string, string>() { { "requestId", requestId } }));
        }

    }
}