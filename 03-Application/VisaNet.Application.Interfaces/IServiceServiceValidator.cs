using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceServiceValidator
    {
        bool ValidateLinkService(Guid serviceId);
        bool ValidateLinkService(ServiceDto service);
        bool ValidateVisaNetOnService(Guid serviceId);
        bool ValidateVisaNetOnService(ServiceDto service);
        bool ValidateDebitService(Guid serviceId);
        bool ValidateDebitService(ServiceDto service);
        bool ValidateServiceGatewayActive(Guid serviceId, GatewayEnumDto gatewayEnum);
        bool ValidateServiceGatewayActive(ServiceDto service, GatewayEnumDto gatewayEnum);
    }
}