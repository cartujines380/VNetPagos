using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using AuditLogDto = VisaNet.Domain.EntitiesDtos.ChangeTracker.AuditLogDto;


namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ChangeTrackerClientService : WebApiClientService, IChangeTrackerClientService
    {
        public ChangeTrackerClientService(ITransactionContext transactionContext)
            : base("ChangeTracker", transactionContext)
        {

        }

        public Task<ICollection<AuditLogDto>> FindAll(ChangeTrackerFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AuditLogDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDataForTable", TransactionContext, filter));
        }

        public Task<AuditLogDto> GetChangesDetails(int id)
        {
            return WebApiClient.CallApiServiceAsync<AuditLogDto>(new WebApiHttpRequestPost(BaseUri + "/GetChangesDetails", TransactionContext, id));
        }

        public Task<int[]> Count(ChangeTrackerFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<int[]>(new WebApiHttpRequestPost(BaseUri + "/Count", TransactionContext, filter));
        }

        public Task<List<string>> GetEntities()
        {
            return WebApiClient.CallApiServiceAsync<List<string>>(new WebApiHttpRequestGet(BaseUri + "/GetEntities", TransactionContext));
        }
    }
}
