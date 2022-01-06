using System;

namespace VisaNet.LIF.WebApi.Models
{
    public class DiscountCalculationAppOutModel
    {
        /// <summary>
        /// Este constructor debe permanecer privado, ya que todas las instancias deberán ser creadas por el método Create
        /// </summary>
        private DiscountCalculationAppOutModel() { }

        /// <summary>
        /// Usar este método para obtener instancias de DiscountCalculationAppOutModel.
        /// De esta manera nos aseguramos que los montos están multiplicados por 100.
        /// </summary>
        /// <param name="cardType">El tipo de tarjeta</param>
        /// <param name="discountAmount">El valor original del descuento, que se convertirá al formato multiplicado por 100 sin decimales</param>
        /// <param name="amountToCybersource">El valor original del monto que se debe enviar a Cybersource, que se convertirá al formato multiplicado por 100 sin decimales</param>
        /// <param name="issuingCompany">El banco emisor de la tarjeta</param>
        /// <returns></returns>
        public static DiscountCalculationAppOutModel Create(char cardType, double discountAmount, double amountToCybersource, string issuingCompany)
        {
            var formatedDiscountAmount = (int) (Math.Round(discountAmount, 2)*100);
            var formatedAmountToCyberSource = (int)(Math.Round(amountToCybersource, 2) * 100);

            return new DiscountCalculationAppOutModel
            {
                CardType = cardType,
                DiscountAmount = formatedDiscountAmount,
                AmountToCyberSource = formatedAmountToCyberSource,
                IssuingCompany = issuingCompany
            };
        }

        /// <summary>
        /// Tipo de tarjeta
        /// </summary>
        public char CardType { get; set; }

        /// <summary>
        /// Compañía emisora del BIN
        /// </summary>
        public string IssuingCompany { get; set; }

        /// <summary>
        /// Monto del descuento en formato sin decimales multiplicado por 100
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Monto a enviar a CyberSource en formato sin decimales multiplicado por 100
        /// </summary>
        public int AmountToCyberSource { get; set; }
    }
}