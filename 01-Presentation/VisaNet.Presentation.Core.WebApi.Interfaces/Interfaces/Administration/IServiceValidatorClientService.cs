using System;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IServiceValidatorClientService
    {
        Task<bool> ValidateLinkService(Guid serviceId);
        Task<bool> ValidateLinkService(ServiceDto service);
        Task<bool> ValidateVisaNetOnService(Guid serviceId);
        Task<bool> ValidateVisaNetOnService(ServiceDto service);
        Task<bool> ValidateDebitService(Guid serviceId);
        Task<bool> ValidateDebitService(ServiceDto service);
        Task<bool> ValidateServiceGatewayActive(Guid serviceId, GatewayEnumDto gatewayEnum);
    }
}