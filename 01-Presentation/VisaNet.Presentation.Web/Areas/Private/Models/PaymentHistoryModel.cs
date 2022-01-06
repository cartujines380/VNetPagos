using System;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class PaymentHistoryModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Guid? ServiceAssosiatedId { get; set; }
        public string ServiceImageUrl { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceContainerName { get; set; }

        public double AmountDolars { get; set; }
        public double AmountPesos { get; set; }

        public string TransactionNumber { get; set; }

        public string CardMask { get; set; }
        public string CardDescription { get; set; }
        public int Quotas { get; set; }
    }
}
