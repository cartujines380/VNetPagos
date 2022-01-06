using System;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebWebhookRegistrationClientService
    {
        Task<WebhookAccessTokenDto> GenerateAccessToken(WebhookRegistrationDto entity);
        Task<WebhookRegistrationDto> GetByAccessToken(WebhookAccessTokenDto dto);
        Task<bool> ValidateAccessToken(WebhookAccessTokenDto dto);
        Task<WebhookRegistrationDto> FindById(Guid id);
        Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, string idapp);
        Task<WebhookRegistrationDto> GetByIdOperation(string idOperation, Guid serviceId);
        Task<bool> IsTokenActive(AccessTokenFilterDto dto);
        Task<bool> SetAccessTokenAsPaid(Guid webhookRegistrationId);
    }
}