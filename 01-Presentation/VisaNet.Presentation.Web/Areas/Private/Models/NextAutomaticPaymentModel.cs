using System;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class NextAutomaticPaymentModel
    {
        public Guid ServiceAssociatedId { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceImageName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime Date { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }
        public string CardMask { get; set; }
        public string CardDescription { get; set; }
    }
}