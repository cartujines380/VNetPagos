using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Resultado de la opreación consulta de comercios
    /// </summary>
    [DataContract]
    public class VNPRespuestaBajaTarjeta
    {
        /// <summary>
        /// <para /> Valor que indica el resultado de la operación. 
        /// 0 indica que la transacción de pago de la factura se completó exitosamente, y cualquier valor distinto a 0 indica un error. 
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Valor que indica el resultado de la operación. </value>
        [DataMember]
        public int CodResultado { get; set; }

        /// <summary>
        /// <para /> Descripción indicando el resultado de la operación ejecutada
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Descripción indicando el resultado de la operación ejecutada.</value>
        [DataMember]
        public string DescResultado { get; set; }

        /// <summary>
        /// <para /> Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada.</value>
        [DataMember]
        public string IdOperacion { get; set; }
    }
}