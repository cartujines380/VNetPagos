using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class EmailService : WebApiClientService, IEmailService
    {
        public EmailService(ITransactionContext transactionContext)
            : base("Email", transactionContext)
        {
        }
        
        public Task<ICollection<EmailMessageDto>> ReportsEmailsData(ReportsEmailsFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<EmailMessageDto>>(new WebApiHttpRequestGet(BaseUri + "/ReportsEmailsData", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<int> ReportsEmailsDataCount(ReportsEmailsFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/ReportsEmailsDataCount", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task CancelEmail(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri + "/CancelEmail", TransactionContext, id));
        }
        
        public Task<EmailMessageDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<EmailMessageDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task ResendEmail(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/ResendEmail", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task CheckStatus()
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/CheckStatus", TransactionContext));
        }

        public Task<int> ResendAll()
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/ResendAll", TransactionContext));
        }

        public Task RegisterDelivery(MailgunDeliveryDto model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterDelivery", TransactionContext, model));
        }

        public Task RegisterBounce(MailgunBounceDto model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterBounce", TransactionContext, model));
        }
        
        public Task RegisterFailure(MailgunFailureDto model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterFailure", TransactionContext, model));
        }

        public Task<FileDto> DownloadAttachment(Guid emailId)
        {
            return WebApiClient.CallApiServiceAsync<FileDto>(new WebApiHttpRequestGet(BaseUri + "/DownloadAttachment", TransactionContext, new Dictionary<string, string> { { "id", emailId.ToString() } }));
        }

        public Task SendCustomerSiteSystemUserCreationEmail(CustomerSiteSystemUserDto user)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/SendCustomerSiteSystemUserCreationEmail", TransactionContext, user));
        }

        public Task SendNotificationManualSynchronization(DebitManualSyncNotificationDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/SendNotificationManualSynchronization", TransactionContext, dto));
        }

        public Task SendNotificationTc33Synchronization(Tc33SyncNotificationDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/SendNotificationTc33Synchronization", TransactionContext, dto));
        }
    }
}
