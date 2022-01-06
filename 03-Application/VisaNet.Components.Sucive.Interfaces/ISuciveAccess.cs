using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Sucive.Interfaces
{
    public interface ISuciveAccess
    {
        List<BillSuciveDto> GetBills(string[] refs, string param, string type, int dpto);
        BillSuciveDto CheckIfBillPayable(string lines, int idPadron, int depto, string param);
        long ConfirmPayment(long numeroPreFactura, string sucursal, string transactionId, int depto);
        List<BillSuciveDto> GetBillsIdPadron(int idPadron, int depto, string param);
        int CheckAccount(string[] refs, string param, string type, int dpto);
        List<ConciliationSuciveDto> GetConciliation(DateTime from, DateTime to, List<int> dptos);
        BillSuciveDto GenerateAnnualPatente(string[] refs, string param, string type, int dpto);
    }
}
