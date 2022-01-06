using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    [DataContract]
    public class Factura
    {
        [DataMember]
        [Required(ErrorMessage = "El campo RefCliente1 es obligatorio.")]
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
        [Required(ErrorMessage = "El campo NroFactura es obligatorio.")]
        public string NroFactura { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo FchFactura es obligatorio.")]
        public DateTime FchFactura { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo FchVencimiento es obligatorio.")]
        public DateTime FchVencimiento { get; set; }
        [DataMember]
        public int DiasPagoVenc { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo Moneda es obligatorio.")]
        public string Moneda { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo MontoTotal es obligatorio.")]
        public double MontoTotal { get; set; }
        [DataMember]
        public double MontoMinimo { get; set; }
        [DataMember]
        public double MontoGravado { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El campo ConsFinal es obligatorio.")]
        public bool ConsFinal { get; set; }
        [DataMember]
        public int Cuotas { get; set; }
        [DataMember]
        public bool PagoAuto { get; set; }

        [DataMember]
        public ICollection<AuxiliarData> AuxiliarData { get; set; }


    }
    [DataContract]
    public class AuxiliarData
    {
        /// <summary>
        /// Identificar del dato auxiliar
        /// </summary>
        [DataMember]
        public string Id_auxiliar { get; set; }

        /// <summary>
        /// Valor del dato auxiliar
        /// </summary>
        [DataMember]
        public string Dato_auxiliar { get; set; }
    }
}