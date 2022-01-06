using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class AutomaticPaymentStatisticsDto
    {
        public int ProcessRunNumber { get; set; }
        public int ServicesCount { get; set; }
        public int BillsCount { get; set; }
        public int BillsToPayCount { get; set; }
        public int BillsNotReadyToPayCount { get; set; }
        public int BillsPayedCount { get; set; }
        public int BillsFailedAmountCount { get; set; }
        public int BillsFailedQuotasCount { get; set; }
        public int BillsFailedCardCount { get; set; }
        public int BillsFailedServiceValidationCount { get; set; }
        public int BillsFailedCybersourceCount { get; set; }
        public int BillsFailedGatewayCount { get; set; }
        public int BillsFailedDiscountCalculationCount { get; set; }
        public int BillsFailedBinNotValidForServiceCount { get; set; }
        public int BillsFailedExceptionsCount { get; set; }
        public int BillsFailedExpiredCount { get; set; }
        public int GetBillsExceptionsCount { get; set; }
        public Dictionary<Guid, PaymentResultTypeDto> ServicesResult { get; set; }
    }
}
