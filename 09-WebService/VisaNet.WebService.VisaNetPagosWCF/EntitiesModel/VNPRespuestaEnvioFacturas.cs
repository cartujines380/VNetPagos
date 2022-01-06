using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
     [DataContract]
    public class VNPRespuestaEnvioFacturas
    {
        [DataMember]
        public int CodResultado { get; set; }
        [DataMember]
        public string DescResultado { get; set; }
        [DataMember]
        public ICollection<FacturaError> FacturasError { get; set; }
    }

    [DataContract]
    public class FacturaError
    {
        [DataMember]
        public string RefCliente { get; set; }
        [DataMember]
        public string NroFactura { get; set; }
        [DataMember]
        public int CodError { get; set; }
        [DataMember]
        public string DescError { get; set; }
    }
}