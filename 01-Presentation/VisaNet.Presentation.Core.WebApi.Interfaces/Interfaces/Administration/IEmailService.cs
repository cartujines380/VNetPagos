using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IEmailService
    {
        Task<ICollection<EmailMessageDto>> ReportsEmailsData(ReportsEmailsFilterDto filtersDto);
        Task<int> ReportsEmailsDataCount(ReportsEmailsFilterDto filtersDto);
        Task RegisterDelivery(MailgunDeliveryDto model);
        Task RegisterBounce(MailgunBounceDto model);
        Task RegisterFailure(MailgunFailureDto model);
        Task CancelEmail(Guid id);
        Task<EmailMessageDto> Find(Guid id);
        Task ResendEmail(Guid id);
        Task CheckStatus();
        Task<int> ResendAll();
        Task<FileDto> DownloadAttachment(Guid emailId);

        Task SendCustomerSiteSystemUserCreationEmail(CustomerSiteSystemUserDto user);
        Task SendNotificationManualSynchronization(DebitManualSyncNotificationDto dto);
        Task SendNotificationTc33Synchronization(Tc33SyncNotificationDto dto);

    }
}
