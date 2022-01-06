using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebServiceAssosiateClientService
    {

        Task<ICollection<ServiceAssociatedDto>> Get(ServiceFilterAssosiateDto filterDto);
        Task<ServiceAssociatedDto> Find(Guid id);
        Task<ServiceAssociatedDto> Create(ServiceAssociatedDto service);
        Task Edit(ServiceAssociatedDto service);

        Task AddPayment(AutomaticPaymentDto automaticPayment);

        Task DeleteService(Guid serviceAssociatedId);

        Task<ICollection<ServiceAssociatedDto>> GetServicesWithAutomaticPayment(ServiceFilterAssosiateDto filterDto);

        Task<ICollection<ServiceAssociatedDto>> GetServicesActiveAutomaticPayment(ServiceFilterAssosiateDto filterDto);

        Task DeleteAutomaticPayment(Guid serviceAssosiatedId);

        Task<Guid> IsServiceAssosiatedToUser(Guid userId, Guid serviceId, string[] refNumber);

        Task<ServiceAssociatedDto> ServiceAssosiatedToUser(Guid userId, Guid serviceId, string[] refNumber);

        Task<ICollection<ServiceAssociatedDto>> GetServicesForBills(Guid userId);

        Task EditDescription(ServiceAssociatedDto service);

        Task<bool> HasAutomaticPaymentCreated(Guid userId);

        Task<bool> HasAsosiatedService(Guid userId);

        Task<ServiceAssociatedDto> CreateOrUpdateDeleted(ServiceAssociatedDto entityDto);
        Task<bool> DeleteCardFromService(CardServiceDataDto dto);
        Task<bool> AddCardToService(CardServiceDataDto dto);

        Task<CybersourceCreateServiceAssociatedDto> ProccesDataFromCybersource(IDictionary<string, string> csDictionary);
        Task<CybersourceCreateAppAssociationDto> ProccesDataFromCybersourceForApps(IDictionary<string, string> csDictionary);
        Task<ServiceAssociatedDto> AssociateServiceToUserFromCardCreated(ServiceAssociatedDto dto);

        Task<IList<ServiceAssociatedDto>> GetDataForFrontList(ServiceFilterAssosiateDto filterDto);

        Task<ServiceAssociatedDto> GetServiceAssociatedDtoFromIdUserExternal(string idUserExternal, string idApp);
    }
}
