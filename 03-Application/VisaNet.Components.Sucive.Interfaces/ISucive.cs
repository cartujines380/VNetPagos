using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Sucive.Interfaces
{
    public interface ISucive
    {
        List<BillSuciveDto> GetBills(string[] refs, string param, string type);
        BillSuciveDto CheckIfBillPayable(string lines, int idPadron, string param);
        long ConfirmarPago(long numeroPreFactura, string sucursal, string transactionId);
        List<BillSuciveDto> GetBillsWithCc(int idPadron, string param);
        int CheckAccount(string[] references, string param, string type);
        List<ConciliationSuciveDto> Conciliation(DateTime date);
        
    }


}
