using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceHighway : IService<HighwayBill, HighwayBillDto>
    {
        void ProccessEmail(HighwayEmailDto dto);
        ICollection<HighwayEmailErrorDto> ProccessEmailFile(HighwayEmailDto dto);
        ICollection<HighwayEmailErrorDto> ProccessEmailFileExternalSoruce(HighwayEmailDto dto);

        List<ServiceEnableEmail> CreateRoutes(List<ServiceEnableEmail> list);
        void DeleteRoutes(List<ServiceEnableEmail> list);
        List<HighwayBillDto> GetBills(string[] references, int codCommerce, int codBrunch);
        bool ConfirmPayment(string gatewayReference, string serviceType, string[] referenceNumbers, ICollection<BillDto> bills, string transactionNumber);
        int CheckAccount(int codCommerce, int codBrunch, string[] codigoCuentaEnte);

        IEnumerable<HighwayEmailDto> GetHighwayEmailsReports(ReportsHighwayEmailFilterDto filter);
        int GetHighwayEmailsReportsCount(ReportsHighwayEmailFilterDto filter);
        IEnumerable<HighwayBillDto> GetHighwayBillReports(ReportsHighwayBillFilterDto filter);
        int GetHighwayBillReportsCount(ReportsHighwayBillFilterDto filter);

        List<HighwayBillDto> ProccesBillsFromWcf(WebServiceBillsInputDto dto);
        HighwayEmailDto GetHighwayEmail(Guid emailId);
        int DeleteBills(WebServiceBillsDeleteDto billsNroFactura);

        IOrderedQueryable<HighwayBill> StatusBIlls(WebServiceBillsStatusInputDto dto);
        void ChangeType(Guid billId, HighwayBillType type, int errorCode = 0, string errorDesc = null);

    }
}