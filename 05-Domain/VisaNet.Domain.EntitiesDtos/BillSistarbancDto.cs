using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BillSistarbancDto
    {

        public Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Amount { get; set; }
        public string Currency { get; set; }
        public string[] Description { get; set; }
        public string BillExternalId { get; set; }
        public bool FinalConsumer { get; set; }
        public double TaxedAmount { get; set; }

        public string IdTransaccionSTB { get; set; }
        public int Precedence { get; set; }
        public string DateInit { get; set; }
        public bool Payable { get; set; }

        //esto se agrega para involucrar metodo viejo de acceso a las facturas.
        public string AmountWithDiscount { get; set; }

        //esto se agrega porque como hay dos bancosId (visa y brou) necesito oteber dos transacciones de sistarbanc
        public string IdTransaccionStbBrou { get; set; }
        public string DashboardDescription { get; set; }

    }
}
