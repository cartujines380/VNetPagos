using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IServiceClientService
    {
        Task<ICollection<ServiceDto>> FindAll();
        Task<ICollection<ServiceDto>> FindAll(BaseFilter filtersDto);
        Task<ServiceDto> Find(Guid id);
        Task Create(ServiceDto service);
        Task Edit(ServiceDto service);
        Task Delete(Guid id);
        Task ChangeStatus(Guid id);
        Task<ICollection<ServiceDto>> GetDataForList(Guid serviceId, bool container);
        Task<ICollection<ServiceDto>> GetServicesLigthWithoutChildens(Guid containerId, GatewayEnumDto? gatewayEnumDto = null);
        Task<ICollection<ServiceDto>> GetServicesFromContainer(Guid containerId);
        Task<IEnumerable<ReportsUsersVonDto>> GetDataForReportsUsersVon(ReportsUserVonFilterDto filter);
        Task<int> GetDataForReportsUsersVonCount(ReportsUserVonFilterDto filter);
        Task<IEnumerable<CardVonDto>> GetVonUsersCards(Guid userId, Guid serviceId);
    }
}