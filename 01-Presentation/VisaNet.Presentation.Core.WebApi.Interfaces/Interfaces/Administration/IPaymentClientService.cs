using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IPaymentClientService
    {
        Task<PaymentDto> GetByTransactionNumber(string transactionNumber);
        Task<PaymentDto> GetFullPaymentByTransactionNumber(string transactionNumber);
        Task<ICollection<PaymentDto>> FindAll();
        Task<ICollection<PaymentBillDto>> ReportsTransactionsDataFromDbView(ReportsTransactionsFilterDto filtersDto);

        Task<int> ReportsTransactionsDataCount(ReportsTransactionsFilterDto filtersDto);

        Task<IEnumerable<PaymentDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto);

        Task<byte[]> DownloadTicket(Guid id, string transactionNumber, Guid userId);

        Task<CyberSourceOperationData> TestCyberSourcePayment(Guid serviceId);
        Task<CyberSourceOperationData> TestCyberSourceCancelPayment(Guid serviceId, CyberSourceOperationData cSOperationData);
        Task<CyberSourceOperationData> TestCyberSourceReversal(Guid serviceId);

        Task<bool> TestCyberSourceReports(Guid serviceId);

        Task ReversePayment(RefundPayment reverse);
        Task<CyberSourceOperationData> CancelPayment(CancelTrnsDto cancelPayment);        
    }
}