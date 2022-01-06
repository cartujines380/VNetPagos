namespace VisaNet.Utilities.Cybersource
{
    public class BillForToken
    {
        public string BillNumber { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public double TaxedAmount { get; set; }
        public int DiscountType { get; set; }
        public double DiscountAmount { get; set; }
        //MDD36 -
        public string BillExpirationDate { get; set; }
        //MDD37 -
        public string BillDescription { get; set; }
        //MDD38 -
        public string BillGatewayTransactionId { get; set; }
        //MDD39 -
        public string BillSucivePreBillNumber { get; set; }
        //MDD40 -
        public string BillFinalConsumer { get; set; }
        //MDD41 -
        public string BillDiscount { get; set; }
        //MDD42 -
        public string BillDateInitTransaccion { get; set; }
        //MDD43 -
        public string BillGatewayTransactionBrouId { get; set; }

        public string IdPadron { get; set; }

        //MDD17 - Número de factura adicional por facturación electrónica
        public string AditionalNumberElectronicBill { get; set; }

        //MDD47 -
        public int Quota { get; set; }


    }
}
