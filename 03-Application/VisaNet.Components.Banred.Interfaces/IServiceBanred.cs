using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Banred.Interfaces
{
    public interface IServiceBanred
    {
        ICollection<BillBanredDto> ConsultaFacturas(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte);
        string PagarFactura(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte, string nroFactura,
                            double montoPago, double montoDescuentoIva, string monedaPago, string fechaVencimiento, string transactionNumber);

        int CheckAccount(string idAgenteExterno, string codigoEnte, string[] codigoCuentaEnte);
    }
}
