using System;

namespace VisaNet.Domain.Entities
{
    public class TransactionHistory 
    {
        public TransactionHistory()
        {
            
        }

        public Guid Idservicio { get; set; }
        public string NombreServicio { get; set; }
        public Guid? IdServicioContenedor { get; set; }
        public string NombreServicioContenedor { get; set; }
        public string Urlname { get; set; }
        public string RefCliente { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        public string NroFactura { get; set; }
        public double MontoTotal { get; set; }
        public string Moneda { get; set; }
        public DateTime FchPago { get; set; }
        public int CantCuotas { get; set; }
        public double MontoDescIVA { get; set; }
        public int Estado { get; set; }
                
		
    }
}
