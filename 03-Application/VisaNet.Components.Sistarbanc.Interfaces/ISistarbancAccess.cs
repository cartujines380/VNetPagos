using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Sistarbanc.Interfaces
{
    public interface ISistarbancAccess
    {
        void AltaUsuario(string idBanco, string idClienteBanco, string clienteNombre, string clienteApellido, string transactionNumber);
        void BajaUsuario(string idBanco, string idClienteBanco, string transactionNumber);
        IEnumerable<BillSistarbancDto> GetBills(string idBancoVisa, string idBancoBrou, string idOrganismo, string tipoServicio, string[] refServicio);

        /// <summary>
        /// Notifica que el pago fue realizado para cada una de las facturas, y si es necesario marca el pago programado como hecho
        /// </summary>
        /// <param name="idBanco"></param>
        /// <param name="idOrganismo"></param>
        /// <param name="tipoServicio"></param>
        /// <param name="refServicio"></param>
        /// <param name="facturasIds"></param>
        /// <param name="idClienteBanco"></param>
        /// <param name="nroTrasnferenciaVisa"></param>
        /// <param name="automaticPaymentDto"></param>
        /// <param name="usertype"></param>
        //Collection<BillSistarbancDto> PagoRecibo(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, string[] facturasIds, string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto);

        
        IEnumerable<ConciliationSistarbancDto> GetConciliation(DateTime from, DateTime to);

        //BillDto PagoRecibo(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, BillDto bill, string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto);
        BillDto PagoReciboLif(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, BillDto bill, string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto);
        
        int CheckAccount(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio);

        IEnumerable<BillSistarbancDto> ServicioImpagoLif(string idBanco, string idOrganismo,string tipoServicio, string[] refServicio);
    }
}
