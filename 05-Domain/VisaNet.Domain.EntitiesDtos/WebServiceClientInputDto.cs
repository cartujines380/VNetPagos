using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WebServiceClientInputDto
    {
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }
        public string IdApp { get; set; }
        public DateTime FechaDesde { get; set; }

        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
    }
}
