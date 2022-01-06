using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Models
{
    public class PaymentModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Guid CardId { get; set; }
        public virtual CardDto Card { get; set; }

        public Guid ServiceId { get; set; }
        public virtual ServiceDto Service { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public virtual ICollection<BillDto> Bills { get; set; }
        public virtual ICollection<BillDto> AllBills { get; set; }

        public ServiceGatewayDto BillsGatewayDto { get; set; }

        public Guid? RegisteredUserId { get; set; }
        public virtual ApplicationUserDto RegisteredUser { get; set; }

        public Guid? AnonymousUserId { get; set; }
        public virtual AnonymousUserDto AnonymousUser { get; set; }

        public PaymentTypeDto PaymentType { get; set; }
        public GatewayEnumDto GatewayEnum { get; set; }

        public string TransactionNumber { get; set; }

        public string Currency { get; set; }
        //Aplica descuento
        public bool DiscountApplyed { get; set; }
        //Total facturas
        public double TotalAmount { get; set; }
        //Total gravado
        public double TotalTaxedAmount { get; set; }
        //Descuento
        public double Discount { get; set; }

        //Monto enviado a cybersource 
        public double AmountTocybersource { get; set; }

        //Datos para area privada
        public Guid? ServicesAssosiatedId { get; set; }
        public virtual ServiceAssociatedDto ServiceAssociated { get; set; }
        public string ServiceImageName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public double AmountDolars { get; set; }
        public double AmountPesos { get; set; }

        //datos para SUCIVE
        public int IdPadron { get; set; }
        public bool EnableMultipleBills { get; set; }
        public string ServiceType { get; set; }


        //datos para pago por importe
        public bool EnableImporte { get; set; }
        public bool EnableBills { get; set; }

        public int PaymentMethod { get; set; } // 1 bill, 2 importe

        public string BillExternalId { get; set; }
        public string Line { get; set; }

        public Guid? DiscountObjId { get; set; }
        public virtual DiscountDto DiscountObj { get; set; }

        public string ServiceContainerName { get; set; }

        public bool DisableEditServicePage { get; set; }

        public int Quota { get; set; }
    }
}
