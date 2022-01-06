using System;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities.ReportsModel
{
    public class ReportConciliationDetail
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string TransactionNumber { get; set; }
        public Int64 VisaTransactionId { get; set; }
        public ConciliationType Type { get; set; }
        public TransactionType TransactionType { get; set; }
        public ConciliationState State { get; set; }
        public string GeneralComment { get; set; }
        public Guid ConciliationCybersourceSummaryConciliationCybersourceId { get; set; }
        public ConciliationState ConciliationCybersourceSummaryState { get; set; }
        public string ConciliationGatewaySummaryGateway { get; set; }
        public Guid ConciliationGatewaySummaryConciliationGatewayId { get; set; }
        public ConciliationState ConciliationGatewaySummaryState { get; set; }
        public Guid ConciliationVisaNetSummaryConciliationVisaNetId { get; set; }
        public ConciliationState ConciliationVisaNetSummaryState { get; set; }
        public Guid ConciliationBatchSummaryConciliationVisaNetCallbackId { get; set; }
        public ConciliationState ConciliationBatchSummaryState { get; set; }
        public ConciliationState ConciliationPortalState { get; set; }
        public int RowsCount { get; set; }

    }
}