using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.ComplexTypes;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Payments")]
    public class Payment : EntityBase, IAuditable
    {
        public Payment()
        {
            Bills = new Collection<Bill>();
        }
        [Key]
        public override Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid CardId { get; set; }
        public virtual Card Card { get; set; }
        public Guid GatewayId { get; set; }
        [ForeignKey("GatewayId")]
        public virtual Gateway Gateway { get; set;}

        //EN CASO DE PAGOS CON SERVICIOS PADRE, EL SERVICEID PUEDE NO CORRESPONDER AL SERVICIO DEL SERVICIO ASOCIADO
        public Guid ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        
        public Guid? ServiceAssosiatedId { get; set; }
        [ForeignKey("ServiceAssosiatedId")]
        public virtual ServiceAssociated ServiceAssosiated { get; set; }

        [MaxLength(100)]
        public string ReferenceNumber { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber2 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber3 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber4 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber5 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber6 { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public Guid? RegisteredUserId { get; set; }
        public virtual ApplicationUser RegisteredUser { get; set; }
        public Guid? AnonymousUserId { get; set; }
        public virtual AnonymousUser AnonymousUser { get; set; }
        public PaymentType PaymentType { get; set; }
        [MaxLength(100)]
        public string TransactionNumber { get; set; }
        public CyberSourceData CyberSourceData { get; set; }
        public VerifyByVisaData VerifyByVisaData { get; set; }
        public Guid PaymentIdentifierId { get; set; }
        public virtual PaymentIdentifier PaymentIdentifier { get; set; }
        
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
        
        public PaymentPlatform PaymentPlatform { get; set; }

        public Guid? DiscountObjId { get; set; }
        public virtual Discount DiscountObj { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public int Quotas { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
