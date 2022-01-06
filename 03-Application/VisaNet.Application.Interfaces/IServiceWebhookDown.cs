using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWebhookDown : IService<WebhookDown, WebhookDownDto>
    {
        ICollection<WebhookDownDto> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto);
        int GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto);
    }
}