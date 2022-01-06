using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebWebhookLogClientService
    {
        Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrations();
        Task<WebhookRegistrationDto> CreateWebhookRegistration(WebhookRegistrationDto webhook);
        Task<ICollection<WebhookDownDto>> GetWebhookDowns();
        Task CreateWebhookDown(WebhookDownDto webhook);
        Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociations();
        Task CreateWebhookNewAssociation(WebhookNewAssociationDto webhook);
        Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, string idapp);
        Task<WebhookRegistrationDto> GetwebHookRegistrationsByIdOperation(string idOperation, Guid serviceId);
        Task<ICollection<WebhookRegistrationDto>> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto);
        Task<ICollection<WebhookDownDto>> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto);
        Task<ICollection<WebhookNewAssociationDto>> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto);
        Task<int> GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto);
        Task<bool> WebhookRegistrationIsIdOperationRepited(string idOperation, string idApp);
    }
}