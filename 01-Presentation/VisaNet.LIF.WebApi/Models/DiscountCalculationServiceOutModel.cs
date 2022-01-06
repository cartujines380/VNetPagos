namespace VisaNet.LIF.WebApi.Models
{
    public class DiscountCalculationServiceOutModel
    {
        /// <summary>
        /// Tipo de tarjeta
        /// </summary>
        public int CardType { get; set; }
        
        /// <summary>
        /// Puntos de descuento aplicados
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Compañía emisora del BIN
        /// </summary>
        public string IssuingCompany { get; set; }

        /// <summary>
        /// Monto del descuento
        /// </summary>
        public double DiscountAmount { get; set; }

        /// <summary>
        /// Monto a envia a CyberSource
        /// </summary>
        public double AmountToCyberSource { get; set; }
    }
}