using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BillDto
    {
        public Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime CreationDate { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string BillExternalId { get; set; }

        //public string GatewayTransactionId { get; set; }
        public string GatewayTransactionId { get; set; }
        //Tanto para sucive como geocom
        public string SucivePreBillNumber { get; set; }

        public Guid? PaymentId { get; set; }
        public virtual PaymentDto Payment { get; set; }

        public double Amount { get; set; }
        [StringLength(5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Currency { get; set; }
        public bool Payable { get; set; }
        public bool FinalConsumer { get; set; }
        public double TaxedAmount { get; set; }
        public int Discount { get; set; }
        public double DiscountAmount { get; set; }

        public GatewayEnumDto Gateway { get; set; }
        public Guid GatewayId { get; set; }

        //dato para sucive/geocom
        public string Line { get; set; }
        public int IdPadron { get; set; }
        public bool HasAnnualPatent { get; set; }

        //dato para sistarbanc
        public string DateInitTransaccion { get; set; }
        //public double AmountStbWithDiscount { get; set; }
        public string GatewayTransactionBrouId { get; set; }

        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }

        public string DashboardDescription { get; set; }
        public bool ItauPayable { get; set; } //Banred 1 y 3 

        public List<BillDto> Bills { get; set; }

        public bool DontApplyDiscount { get; set; }

        public bool GatewayAcceptsPreBills { get; set; }

    }
}
