using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWebhookNewAssociation : IService<WebhookNewAssociation, WebhookNewAssociationDto>
    {
        ICollection<WebhookNewAssociationDto> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto);
        int GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto);
        bool AlreadyNotifiedExternalCard(string idApp, string idUserExternal, string idCardExternal);

        IList<WebhookNewAssociationDto> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto);
    }
}