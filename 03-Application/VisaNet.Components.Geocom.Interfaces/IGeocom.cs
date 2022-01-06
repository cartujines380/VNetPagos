using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Geocom.Interfaces
{
    public interface IGeocom
    {
        List<BillGeocomDto> GetBills(string[] refs, string param, string type);
        BillGeocomDto CheckIfBillPayable(string lines, int idPadron, string param);
        long ConfirmarPago(int numeroPreFactura, string sucursal, string transactionId, string param);
        List<BillGeocomDto> GetBillsWithCc(int idPadron, string param);
        int CheckAccount(string[] references, string param, string type);
        List<ConciliationGeocomDto> Conciliation(DateTime date);
        
    }


}
