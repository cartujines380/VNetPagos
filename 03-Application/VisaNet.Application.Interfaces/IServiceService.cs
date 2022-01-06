using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceService : IService<Service, ServiceDto>
    {
        List<GatewayDto> GetGateways();
        IEnumerable<ServiceDto> GetDataForTable(ServiceFilterDto filters);
        IEnumerable<ServiceDto> GetServicesAutoComplete(string contains);
        ICollection<CardDto> GetEnableCards(Guid userId, Guid serviceId);
        GatewayDto GetGateway(GatewayEnumDto enumDto);
        ServiceDto GetService(GatewayDto gateway, string serviceName, string serviceType);
        IEnumerable<ServiceDto> GetServicesFromMerchand(string idApp, string merchandId, GatewayEnumDto gateway);
        IEnumerable<ServiceDto> GetServices(string idApp, int codCommerce, int codBranch, GatewayEnumDto gateway);
        void ChangeStatus(Guid serviceId);
        string GetCertificateName(string codcommerce, string codBranch);
        ServiceDto GetserviceByUrlName(string serUrlName);
        IEnumerable<ServiceDto> GetDataForList(Guid serviceId, bool container);
        string GetCertificateNameIdApp(string idApp);
        bool IsFatherOrHim(string idApp, string merchandId, string codcommerce, string codBranch);
        TransactionCommerceResult GetServicesFromFather(string idApp, bool all = true);

        /// <summary>
        /// </summary>
        /// <param name="serviceId">Cuando es un pago debe ser el Id de un servicio único o servicio hijo</param>
        /// <returns></returns>
        bool IsBinAssociatedToService(int binValue, Guid serviceId);

        ServiceGatewayDto GetBestGateway(ServiceDto service, IList<ServiceGatewayDto> gatewayDtosList, Guid? cardId);
        void NotifyServiceWithoutActiveGateway(ServiceDto service);
        IEnumerable<ServiceDto> GetServicesForApp(ServiceFilterDto filters);
        void SetImporteValues(ServiceDto serviceDto);

        IEnumerable<ServiceDto> GetServicesLigthWithoutChildens(Guid? selectedServiceId = null, GatewayEnumDto? gatewayEnumDto = null);
        IEnumerable<ServiceDto> GetServicesFromContainer(Guid containerId);

        IEnumerable<ReportsUsersVonDto> GetDataForReportsUsersVon(ReportsUserVonFilterDto filter);
        int GetDataForReportsUsersVonCount(ReportsUserVonFilterDto filter);
        IEnumerable<CardVonDto> GetVonUsersCards(Guid userId, Guid serviceId);
    }
}