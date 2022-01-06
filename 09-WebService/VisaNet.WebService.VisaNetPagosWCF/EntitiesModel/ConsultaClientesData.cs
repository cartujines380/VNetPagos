using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    [DataContract]
    public class ConsultaClientesData
    {
        [DataMember]
        [Required(ErrorMessage = "CodComercio")]
        [Range(0, 999999, ErrorMessage = "CodComercioNegativo")]
        public int CodComercio { get; set; }
        [DataMember]
        [Required(ErrorMessage = "CodSucursal")]
        [Range(0, 999999, ErrorMessage = "CardBinNumbers")]
        public int CodSucursal { get; set; }

        [DataMember]
        public DateTime FechaDesde { get; set; }
        [DataMember]
        public string RefCliente1 { get; set; }
        [DataMember]
        public string RefCliente2 { get; set; }
        [DataMember]
        public string RefCliente3 { get; set; }
        [DataMember]
        public string RefCliente4 { get; set; }
        [DataMember]
        public string RefCliente5 { get; set; }
        [DataMember]
        public string RefCliente6 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "FirmaDigital")]
        public string FirmaDigital { get; set; }
        
    }
}