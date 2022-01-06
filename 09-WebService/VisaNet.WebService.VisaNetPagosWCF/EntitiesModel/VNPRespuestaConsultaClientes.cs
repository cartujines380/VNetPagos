using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    [DataContract]
    public class VNPRespuestaConsultaClientes
    {
        [DataMember]
        public int CodResultado { get; set; }
        [DataMember]
        public string DescResultado { get; set; }
        [DataMember]
        public ICollection<Cliente> ListadoClientes { get; set; }

    }

    [DataContract]
    public class Cliente
    {
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
        public string Email { get; set; }
        [DataMember]
        public string Documento { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Apellido { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public DateTime FchModificacion { get; set; }
        
        [DataMember]
        public int Estado { get; set; }
        
    }
}