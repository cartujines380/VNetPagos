using System;

namespace VisaNet.Domain.Entities
{
    public class WebServiceApplicationClient
    {

        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        public string Email{ get; set; }
        public string Documento{ get; set; }
        public string Nombre{ get; set; }
        public string Apellido{ get; set; }
        public string Telefono{ get; set; }
        public DateTime FchModificacion{ get; set; }
        public int Estado { get; set; }
        public Guid UserId { get; set; }

    }
}
