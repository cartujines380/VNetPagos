using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BillBanredDto
    {

        public Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public bool Payable { get; set; }
        public bool FinalConsumer { get; set; }
        public double TaxedAmount { get; set; }
        public string BanredTransactionId { get; set; }

        public double MinAmount { get; set; }
        public int PaymentType { get; set; } //1 = Solo Total 2 = Total y Mínimo 3 =  Parcial (cualquier monto entre el total y el mínimo)
        public string PayableType { get; set; } //1 = La deuda se puede pagar, 2 = La deuda no se puede pagar por estar vencida, 3 = La deuda no se puede pagar porque existen deudas impagas anteriores ( las facturas ya vencidas deben pagarse primero) 
        public string DashboardDescription { get; set; }
    }
}
