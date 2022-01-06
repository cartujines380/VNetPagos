using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    [DataContract]
    public class EnvioFacturasData
    {
        [DataMember]
        [Required(ErrorMessage = "El campo CodComercio es obligatorio.")]
        [Range(0, 9999999, ErrorMessage = "El campo CodComercio excede el largo máximo permitido (7).")]
        public int CodComercio { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo CodSucursal es obligatorio.")]
        [Range(0, 999, ErrorMessage = "El campo CodSucursal excede el largo máximo permitido (3).")]
        public int CodSucursal { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo ReemplazarFacturas es obligatorio.")]
        public bool ReemplazarFacturas { get; set; }
        [DataMember]
        public ICollection<Factura> Facturas { get; set; }
        [DataMember]
        [Required(ErrorMessage = "FirmaDigital")]
        public string FirmaDigital { get; set; }
    }
}