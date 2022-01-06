using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IWebhookLogClientService
    {
        Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WebhookDownDto>> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto);

        Task<WebhookNewAssociationDto> GetWebhookNewAssociation(Guid id);
        Task<WebhookRegistrationDto> GetWebhookRegistration(Guid id);
    }
}