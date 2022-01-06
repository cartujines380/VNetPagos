
namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Resultado de la opreación invocada
    /// </summary>
    public class VNPRespuesta
    {
        
        /// <summary>
        /// <para /> Valor que indica el resultado de la operación. 0 indica que la transacción de pago de la factura se completó exitosamente, y cualquier valor distinto a 0 indica un error. En caso de ocurrir un error se anulará toda la transacción. 
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Valor que indica el resultado de la operación.</value>
        public int CodResultado { get; set; }
        
        /// <summary>
        /// <para /> Descripción indicando el resultado de la operación ejecutada.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Descripción indicando el resultado de la operación ejecutada.</value>
        public string DescResultado { get; set; }
        
        /// <summary>
        /// <para /> Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada.</value>
        public string IdOperacion { get; set; }
        
        /// <summary>
        /// <para /> Número de la transacción realizada por VisaNetPagos.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Número de la transacción realizada por VisaNetPagos.</value>
        public string NroTransaccion { get; set; }
    }
}