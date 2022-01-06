using System;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string TransactionNumber { get; set; }
        public Int64 VisaTransactionId { get; set; }
        public ConciliationTypeDto Type { get; set; }
        public TransactionType TransactionType { get; set; }
        public ConciliationCybersourceSummaryDto ConciliationCybersourceSummary { get; set; }
        public ConciliationGatewaySummaryDto ConciliationGatewaySummary { get; set; }
        public ConciliationVisaNetSummaryDto ConciliationVisaNetSummary { get; set; }
        public ConciliationBatchSummaryDto ConciliationBatchSummary { get; set; }
        public ConciliationStateDto State { get; set; }
        public string GeneralComment { get; set; }
        public ConciliationStateDto ConciliationPortalState { get; set; }
    }

    public class ConciliationCybersourceSummaryDto
    {
        public Guid ConciliationCybersourceId { get; set; }
        public ConciliationStateDto State { get; set; }
    }

    public class ConciliationGatewaySummaryDto
    {
        public string Gateway { get; set; }
        public Guid ConciliationGatewayId { get; set; }
        public ConciliationStateDto State { get; set; }
    }

    public class ConciliationVisaNetSummaryDto
    {
        public Guid ConciliationVisaNetId { get; set; }
        public ConciliationStateDto State { get; set; }
    }

    public class ConciliationBatchSummaryDto
    {
        public Guid ConciliationVisaNetCallbackId { get; set; }
        public ConciliationStateDto State { get; set; }
    }

}
