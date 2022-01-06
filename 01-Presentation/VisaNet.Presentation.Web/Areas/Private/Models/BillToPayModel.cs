using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class BillToPayModel
    {
        public Guid ServiceAssociatedId { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceImageName { get; set; }

        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<string> Cards { get; set; }
        public string DefaultCardMask { get; set; }
        public string DefaultCardDescription { get; set; }
        public GatewayEnumDto GatewayEnumDto { get; set; }

        public string AutomaticPaymentDateString { get; set; }
        public bool AllowsAutomaticPayment { get; set; }

        public string BillExternalId { get; set; }
        public string Line { get; set; }
        public bool Payable { get; set; }
        public string DashboardDescription { get; set; }
        public bool MultipleBills { get; set; }

        public string ServiceContainerName { get; set; }
    }
}