using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebServiceClientService
    {
        Task<ICollection<ServiceDto>> FindAll();
        Task<ICollection<ServiceDto>> FindAll(BaseFilter filtersDto);
        Task<ServiceDto> Find(Guid id);
        Task<ICollection<ServiceDto>> ServicesWithImages();
        Task<ICollection<CardDto>> GetEnableCards(Guid userId, Guid serviceId);

        Task<ServiceDto> GetServiceByUrlName(string nameUrl);

        Task<ICollection<ServiceDto>> GetServicesEnableAssociation();
        Task<ICollection<ServiceDto>> GetServicesPaymentPrivate();
        Task<ICollection<ServiceDto>> GetServicesProvider(Guid? serviceId = null);
        Task<ICollection<ServiceDto>> GetServicesPaymentPublic();
        Task<ICollection<ServiceDto>> GetServicesFromContainer(Guid containerId);
        Task<ICollection<GatewayDto>> GetGateways();

        Task<string> GetCertificateThumbprintIdApp(string idApp);

        /// <summary>
        /// </summary>
        /// <param name="serviceId">Cuando es un pago debe ser el Id de un servicio único o servicio hijo</param>
        /// <returns></returns>
        Task<bool> IsBinAssociatedToService(int binValue, Guid serviceId);

        Task<List<ServiceDto>> GetServicesFromMerchand(string idApp, string merchandId, GatewayEnumDto gateway);

        //Task<Tuple<ServiceDto, string, string>> FindAndValidateServiceForVisaNetOnAssociation(string idApp);
        //Task<Tuple<ServiceDto, string, string>> FindAndValidateServiceForVisaNetOnPayment(string idApp, string merchantId);
    }
}