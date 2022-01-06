using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWebhookRegistration : IService<WebhookRegistration, WebhookRegistrationDto>
    {
        WebhookRegistrationDto GetByIdOperation(string idOperation, string idapp);
        WebhookRegistrationDto GetByIdOperation(string idOperation, Guid serviceId);
        ICollection<WebhookRegistrationDto> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto);
        int GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto);
        bool IsOperationIdRepited(string idOperation, string idApp);
        WebhookAccessTokenDto GenerateAccessToken(WebhookRegistrationDto entity);
        void ResetAccessToken(Guid webhookRegistrationId);
        WebhookAccessTokenDto RegenerateToken(Guid webhookRegistrationId);
        WebhookRegistrationDto GetByAccessToken(WebhookAccessTokenDto dto);
        bool ValidateAccessToken(WebhookAccessTokenDto dto);
        bool IsTokenActive(AccessTokenFilterDto dto);
        void UpdatewithPaymentId(string idApp, string idOperation, Guid paymentId);
        bool CancelAccessToken(Guid accessTokenId);
        bool UpdateStatusAccessToken(Guid webhookId, WebhookAccessState status);
    }
}