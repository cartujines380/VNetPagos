using System;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class ConciliationSummaryModel
    {
        public Guid ConciliationSummaryId { get; set; }
        public PaymentDto Payment { get; set; }
        public LogDto Log { get; set; }
        public ConciliationTypeDto ConciliationType { get; set; }
        public TransactionType TransactionType { get; set; }
        public ConciliationCybersourceDto ConciliationCybersource { get; set; }
        public ConciliationStateDto CybersourceState { get; set; }
        public ConciliationBanredDto ConciliationBanred { get; set; }
        public ConciliationSistarbancDto ConciliationSistarbanc { get; set; }
        public ConciliationSuciveDto ConciliationSucive { get; set; }
        public ConciliationVisanetDto ConciliationVisanet { get; set; }
        public ConciliationVisanetCallbackDto ConciliationVisanetCallback { get; set; }
        public ConciliationStateDto VisanetState { get; set; }
        public ConciliationStateDto GatewayState { get; set; }
        public ConciliationStateDto BatchState { get; set; }
        public string GeneralComment { get; set; }
        public bool Checked { get; set; }

        public ConciliationStateDto PortalState { get; set; }
        public string ConciliationSummeryGatewayName { get; set; }
    }
}