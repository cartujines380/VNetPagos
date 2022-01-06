using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    [DataContract]
    public class QuitarFacturasData
    {
        [DataMember]
        [Required(ErrorMessage = "El campo CodComercio es obligatorio.")]
        [Range(0, 9999999, ErrorMessage = "El campo CodComercio excede el largo máximo permitido (7).")]
        public int CodComercio { get; set; }
        
        [DataMember]
        [Required(ErrorMessage = "El campo CodSucursal es obligatorio.")]
        [Range(0, 9999999, ErrorMessage = "El campo CodSucursal excede el largo máximo permitido (3).")]
        public int CodSucursal { get; set; }
        
        [DataMember]
        [Required(ErrorMessage = "Facturas")]
        public ICollection<FacturaQuita> FacturaEliminar { get; set; }

        [DataMember]
        [Required(ErrorMessage = "FirmaDigital")]
        public string FirmaDigital { get; set; }
    }

    [DataContract]
    public class FacturaQuita
    {
        [DataMember]
        public string NroFactura { get; set; }
    }
}