using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("HighwayBills")]
    public class HighwayBill : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public int CodComercio { get; set; }
        public int CodSucursal { get; set; }
        public string RefCliente { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        public string NroFactura { get; set; }
        public string Descripcion { get; set; }
        public DateTime FchFactura { get; set; }
        public DateTime FchVencimiento { get; set; }
        public int DiasPagoVenc { get; set; }
        public string Moneda { get; set; }
        public double MontoTotal { get; set; }
        public double MontoMinimo { get; set; }
        public double MontoGravado { get; set; }
        public bool ConsFinal { get; set; }
        public int Cuotas { get; set; }
        public bool PagoDebito { get; set; }

        public ICollection<HighwayAuxiliarData> AuxiliarData { get; set; }

        public Guid HighwayEmailId { get; set; }
        public virtual HighwayEmail HighwayEmail { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; }

        public HighwayBillType Type { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        
        [MaxLength(50)]
        [SkipTracking]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        [SkipTracking]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }
    }

    public class HighwayAuxiliarData
    {
        public Guid Id { get; set; }
        public string IdValue { get; set; }
        public string Value { get; set; }
        public Guid HighwayBillId { get; set; }
        public virtual HighwayBill HighwayBill { get; set; }
    }
}
