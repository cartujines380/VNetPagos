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
    public class AuditClientService : WebApiClientService, IAuditClientService
    {
        public AuditClientService(ITransactionContext transactionContext)
            : base("Audit", transactionContext)
        {

        }

        public Task<ICollection<AuditTransactionLogDto>> FindAll(AuditFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AuditTransactionLogDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDataForTable", TransactionContext, filter));
        }

        public Task<ICollection<AuditLogDto>> GetLogs(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AuditLogDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDetails", TransactionContext, id));
        }

        public Task<ICollection<AuditExcelDto>> ExcelExport(AuditFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AuditExcelDto>>(new WebApiHttpRequestPost(BaseUri + "/ExcelExport", TransactionContext, filter));
        }

        public Task<ICollection<ChangeTrackerExcelDto>> ChangeLogExcelExport(ChangeTrackerFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ChangeTrackerExcelDto>>(new WebApiHttpRequestPost(BaseUri + "/ChangeLogExcelExport", TransactionContext, filter));
        }
    }
}
