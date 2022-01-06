using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceBill : IService<Bill, BillDto>
    {
        ApplicationUserBillDto GetBillsForRegisteredUser(RegisteredUserBillFilterDto filterDto);
        AnonymousUserBillDto GetBillsForAnonymousUser(AnonymousUserBillFilterDto filterDto);

        ICollection<BillDto> TestGatewayGetBills(TestGatewaysFilterDto filterDto);

        int CheckAccount(IBillFilterDto filterDto);

        ICollection<BillDto> GetBillsForDashboard(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType,
            string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4,
            string referenceNumber5, string referenceNumber6, int serviceDepartament);

        ICollection<BillDto> PayBills(NotifyPaymentDto notify);

        double GetAmount(ICollection<Guid> billsId);

        BillDto ChekBills(string lines, int idPadron, int depto, GatewayEnumDto gateway, string param);
        List<BillDto> GetBillsIdPadron(int idPadron, int depto, GatewayEnumDto gateway, string param);

        IQueryable<HighwayBill> StatusBIlls(WebServiceBillsStatusInputDto dto);
        BillDto ToDto(HighwayBillDto entity, Guid gatewayId, DateTime data, bool scheduledPayment);

        BillDto GeneratePreBill(GeneratePreBillDto generatePreBillDto);
        AnonymousUserBillDto GetInputAmountBillForAnonymousUser(AnonymousUserInputAmountBillFilterDto billFilterDto);
        ApplicationUserBillDto GetInputAmountBillForRegisteredUser(RegisteredUserInputAmountBillFilterDto billFilterDto);
        BillDto GetInputAmountBill(double amount, CurrencyDto currency);

        bool IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId);
        bool IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId);

        AnonymousUserBillDto GenerateAnnualPatenteForAnonymousUser(AnonymousUserBillFilterDto filter);
        ApplicationUserBillDto GenerateAnnualPatenteForRegisteredUser(RegisteredUserBillFilterDto filter);
    }
}
