using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationSummary")]
    public class ConciliationSummary : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string TransactionNumber { get; set; }
        public Int64 VisaTransactionId { get; set; }
        public ConciliationType Type { get; set; }
        public TransactionType TransactionType { get; set; }
        public ConciliationCybersourceSummary ConciliationCybersourceSummary { get; set; }
        public ConciliationGatewaySummary ConciliationGatewaySummary { get; set; }
        public ConciliationVisaNetSummary ConciliationVisaNetSummary { get; set; }
        public ConciliationBatchSummary ConciliationBatchSummary { get; set; }
        public ConciliationState State { get; set; }
        public string GeneralComment { get; set; }
        public ConciliationState ConciliationPortalState { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }

    [ComplexType]
    public class ConciliationCybersourceSummary
    {
        public Guid ConciliationCybersourceId { get; set; }
        public ConciliationState State { get; set; }
    }

    [ComplexType]
    public class ConciliationGatewaySummary
    {
        public string Gateway { get; set; }
        public Guid ConciliationGatewayId { get; set; }
        public ConciliationState State { get; set; }
    }

    [ComplexType]
    public class ConciliationVisaNetSummary
    {
        public Guid ConciliationVisaNetId { get; set; }
        public ConciliationState State { get; set; }
    }

    [ComplexType]
    public class ConciliationBatchSummary
    {
        public Guid ConciliationVisaNetCallbackId { get; set; }
        public ConciliationState State { get; set; }
    }
}
