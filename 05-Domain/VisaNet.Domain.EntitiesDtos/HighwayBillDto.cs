using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class HighwayBillDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El campo CodComercio es obligatorio.")]
        [Range(0, 9999999, ErrorMessage = "El campo CodComercio excede el largo máximo permitido (7).")]
        public int CodComercio { get; set; }

        [Required(ErrorMessage = "El campo CodSucursal es obligatorio.")]
        [Range(0, 999, ErrorMessage = "El campo CodSucursal excede el largo máximo permitido (3).")]
        public int CodSucursal { get; set; }

        [Required(ErrorMessage = "El campo RefCliente es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El campo RefCliente excede el largo máximo permitido (30).")]
        public string RefCliente { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }

        [Required(ErrorMessage = "El campo NroFactura es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo NroFactura excede el largo máximo permitido (50).")]
        public string NroFactura { get; set; }

        [MaxLength(200, ErrorMessage = "El campo Descripcion excede el largo máximo permitido (200).")]
        public string Descripcion { get; set; }
        
        public string FchFacturaString { get; set; }
        public string FchVencimientoString { get; set; }
        [Required(ErrorMessage = "El campo FchFactura es obligatorio.")]
        public DateTime FchFactura { get; set; }
        [Required(ErrorMessage = "El campo FchVencimiento es obligatorio.")]
        public DateTime FchVencimiento { get; set; }

        [Range(0, 999, ErrorMessage = "El campo DiasPagoVenc excede el largo máximo permitido (3).")]
        public int DiasPagoVenc { get; set; }

        [Required(ErrorMessage = "El campo Moneda es obligatorio.")]
        [MaxLength(1, ErrorMessage = "El campo Moneda excede el largo máximo permitido (1).")]
        [RegularExpression(@"^[n]|[N]|[D]|[d]$", ErrorMessage = "El campo Moneda solo acepta los valores 'N' o 'D'")]
        public string Moneda { get; set; }

        [Required(ErrorMessage = "El campo MontoTotal es obligatorio.")]
        [Range(0, 99999999999999999, ErrorMessage = "El campo MontoTotal excede el largo máximo permitido (18).")]
        [RegularExpression(@"^\d{1,18}(\.\d{1,2})?$", ErrorMessage = "El campo MontoTotal excede el largo máximo permitido de decimales (2).")]
        public double MontoTotal { get; set; }
        
        [Range(0, 99999999999999999, ErrorMessage = "El campo MontoMinimo excede el largo máximo permitido (18).")]
        [RegularExpression(@"^\d{1,18}(\.\d{1,2})?$", ErrorMessage = "El campo MontoMinimo excede el largo máximo permitido de decimales (2).")]
        public double MontoMinimo { get; set; }

        [Range(0, 99999999999999999, ErrorMessage = "El campo MontoGravado excede el largo máximo permitido (18).")]
        [RegularExpression(@"^\d{1,18}(\.\d{1,2})?$", ErrorMessage = "El campo MontoGravado excede el largo máximo permitido de decimales (2).")]
        public double MontoGravado { get; set; }

        [Required(ErrorMessage = "El campo ConsFinal es obligatorio.")]
        public bool ConsFinal { get; set; }

        [Required(ErrorMessage = "El campo PagoAuto es obligatorio.")]
        public bool PagoDebito { get; set; }

        [Range(0, 1, ErrorMessage = "El campo Cuotas excede el largo máximo permitido (3).")]
        public int Cuotas { get; set; }

        public ICollection<HighwayAuxiliarDataDto> AuxiliarData { get; set; }

        public Guid HighwayEmailId { get; set; }
        public HighwayEmailDto HighwayEmailDto { get; set; }

        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }

        public HighwayBillTypeDto Type { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDesc { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public string CreationDateString { get; set; }
        public DateTime CreationDate { get; set; }
        public string LastModificationDateString { get; set; }
        public DateTime LastModificationDate { get; set; }
    }

    public class HighwayAuxiliarDataDto
    {
        [MaxLength(20, ErrorMessage = "El campo AuxiliarData - Id_auxiliar  excede el largo máximo permitido (20).")]
        public string Id_auxiliar { get; set; }
        [MaxLength(200, ErrorMessage = "El campo AuxiliarData - Dato_auxiliar excede el largo máximo permitido (200).")]
        public string Dato_auxiliar { get; set; }
    }
}
