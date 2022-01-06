using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IServiceAssociatedClientService
    {
        //Task<ICollection<ServiceAssociatedDto>> Get(ReportsServicesAssociatedFilterDto filterDto);
        Task<ICollection<ServiceAssociatedViewDto>> ReportsServicesAssociatedDataFromDbView(ReportsServicesAssociatedFilterDto filterDto);
        Task<int> ReportsServicesAssociatedDataCount(ReportsServicesAssociatedFilterDto filterDto);

        Task<ICollection<AutomaticPaymentsViewDto>> ReportsAutomaticPaymentsDataFromDbView(ReportsAutomaticPaymentsFilterDto filterDto);
        Task<int> ReportsAutomaticPaymentsDataCount(ReportsAutomaticPaymentsFilterDto filterDto);

        Task<ServiceAssociatedDto> Find(Guid id);

        Task<ICollection<ServiceAssociatedDto>> GetByServiceId(Guid id);
    }
}