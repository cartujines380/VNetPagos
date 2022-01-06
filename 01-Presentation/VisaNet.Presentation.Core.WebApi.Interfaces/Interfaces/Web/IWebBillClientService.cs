using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebBillClientService
    {
        Task<ICollection<BillDto>> FindAll(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType,
            string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4,
            string referenceNumber5, string referenceNumber6, int serviceDepartament);

        Task<ICollection<BillDto>> GetBillsForDashboard(GatewayEnumDto gateway, Guid serviceId, string gatewayReference,
            string serviceType, string referenceNumber, string referenceNumber2, string referenceNumber3,
            string referenceNumber4, string referenceNumber5, string referenceNumber6, int serviceDepartament);

        Task<ApplicationUserBillDto> GetBillsForRegisteredUser(IBillFilterDto billFilterDto);
        Task<AnonymousUserBillDto> GetBillsForAnonymousUser(IBillFilterDto billFilterDto);

        Task<ICollection<BillDto>> FindAllWithServices(GatewayEnumDto gateway, string gatewayReference, string serviceType,
            Guid serviceAssosiatedId);

        Task<BillDto> ChekBills(string lines, int idPadron, int depto, GatewayEnumDto gateway, string param);

        Task<ICollection<BillDto>> GetBillsIdPadron(int idPadron, int depto, GatewayEnumDto gateway, string param);

        Task<int> CheckAccount(IBillFilterDto billFilterDto);
        Task<BillDto> GeneratePreBill(GeneratePreBillDto generatePreBillDto);

        Task<BillDto> GetInputAmountBill(InputAmountBillFilterDto billFilterDto);

        Task<bool> IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId);
        Task<bool> IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId);

        Task<ApplicationUserBillDto> GenerateAnnualPatenteForRegisteredUser(IBillFilterDto billFilterDto);
        Task<AnonymousUserBillDto> GenerateAnnualPatenteForAnonymousUser(IBillFilterDto billFilterDto);
    }
}
