namespace VisaNet.LIF.WebApi.Models
{
    public class CardDataOutModel
    {
        /// <summary>
        /// Tipo de tarjeta
        /// </summary>
        public char CardType { get; set; }

        /// <summary>
        /// Compañía emisora del BIN
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Indicador de si es local o no
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// BIN de la tarjeta
        /// </summary>
        public string Bin { get; set; }

        /// <summary>
        /// Cuotas permitidas para el BIN
        /// </summary>
        public int[] Installments { get; set; }
    }
}