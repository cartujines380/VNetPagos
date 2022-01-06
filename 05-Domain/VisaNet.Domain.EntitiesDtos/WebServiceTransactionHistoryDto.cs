using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WebServiceTransactionHistoryDto
    {
        public int CodResultado { get; set; }
        public string DescResultado { get; set; }
        public ICollection<TransactionHistoryDto> EstadoFacturas { get; set; }
        public ResumenPagosDto ResumenPagos { get; set; }
    }

    public class ResumenPagosDto
    {
        public int CantFacturas { get; set; }
        public int CantPesosPagados { get; set; }
        public double SumaPesosPagados { get; set; }
        public int CantDolaresPagados { get; set; }
        public double SumaDolaresPagados { get; set; }
    }

    public class TransactionHistoryDto
    {
        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        public string NroFactura { get; set; }
        public string Moneda { get; set; }
        public double MontoTotal { get; set; }
        public DateTime FchPago { get; set; }
        public int CantCuotas { get; set; }
        public double MontoDescIVA { get; set; }
        public int Estado { get; set; }
        public int CodError { get; set; }
        public string DescEstado { get; set; }
        public ICollection<HighwayAuxiliarDataDto> AuxiliarData { get; set; }
    }
}
