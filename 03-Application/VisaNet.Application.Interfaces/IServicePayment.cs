using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePayment : IService<Payment, PaymentDto>
    {
        //List<PaymentDto> GetDataForTable(PaymentFilterDto filters);
        ICollection<BillDto> MakePayment(NotifyPaymentDto notify);
        void GeneratePaymentTicket(Guid id, string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType);
        void SendPaymentTicketByEmail(Guid id, string transactionNumber, Guid userId);
        CyberSourceOperationData CancelPaymentCybersource(CancelPayment cancel);
        CyberSourceOperationData ReversePayment(RefundPayment reverse);

        IEnumerable<PaymentBillDto> ReportsTransactionsDataFromDbView(ReportsTransactionsFilterDto filters);
        IEnumerable<PaymentDto> GetPaymentBatch(DateTime date, GatewayEnum gatewayEnum, Guid serviceId, int departament);
        IEnumerable<PaymentDto> GetDashboardData(ReportsDashboardFilterDto filters);
        List<TransactionHistory> TransactionsHistoryForWebService(WsBillQueryDto dto);

        int ReportsTransactionsDataCount(ReportsTransactionsFilterDto filters);
        bool IsPaymentDoneWithServiceAssosiated(Guid serviceAssosiatedId);
        int CountPaymentsDone(Guid registredUserId, Guid anonymousUserId, Guid serviceId);
        void NotifyError(string data);

        IEnumerable<PaymentDto> GetDataForUserReports(ReportFilterDto filters);

        CyberSourceOperationData TestCyberSourcePayment(Guid serviceId);
        CyberSourceOperationData TestCyberSourceCancelPayment(Guid serviceId, CyberSourceOperationData cSourceOperationData);
        CyberSourceOperationData TestCyberSourceReversal(Guid serviceId);
        bool TestCyberSourceReports(Guid serviceId);

        CybersourceCreatePaymentDto NotifyGateways(IDictionary<string, string> csDictionaryData);
        CybersourceCreatePaymentDto NotifyGateways(PaymentDto entity);
        List<PaymentDto> GetDataForFromList(PaymentFilterDto filters);

        CyberSourceOperationData CancelPaymentDone(Guid id, string transactionNumber, bool notify);
        CyberSourceOperationData DeleteCardInCybersource(Guid userId, Guid id, string transactionNumber);
        bool BillAlreadyPaid(CheckBillInsertedDto checkBillInsertedDto);

        CybersourceTransactionsDataDto CybersourceAnalyze(IDictionary<string, string> csDictionaryData);
        void LogCybersourceData(PaymentDto payment, CybersourceTransactionsDataDto csTransactionsDataDto);

        bool NotifyExternalSourceNewPayment(PaymentDto paymentDto);
    }
}