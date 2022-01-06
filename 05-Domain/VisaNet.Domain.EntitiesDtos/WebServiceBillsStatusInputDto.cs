using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WebServiceBillsStatusInputDto
    {

        public int CodComercio { get; set; }
        public int CodSucursal { get; set; }
        public string NroFactura{ get; set; }
        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        
        public DateTime FechaDesde{ get; set; }
        public DateTime FechaHasta { get; set; }
        
    }
}
