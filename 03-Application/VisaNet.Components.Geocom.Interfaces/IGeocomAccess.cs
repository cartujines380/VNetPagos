using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Geocom.Interfaces
{
    public interface IGeocomAccess
    {
        List<BillGeocomDto> GetBills(string[] refs, string param, string type, int dpto);
        BillGeocomDto CheckIfBillPayable(string lines, int idPadron, int depto, string param);
        long ConfirmPayment(int numeroPreFactura, string sucursal, string transactionId, int depto, string param);
        List<BillGeocomDto> GetBillsIdPadron(int idPadron, int depto, string param);
        int CheckAccount(string[] refs, string param, string type, int dpto);
        List<ConciliationGeocomDto> GetConciliation(DateTime from, DateTime to, List<int> dptos);
    }
}
