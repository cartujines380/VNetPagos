using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Resultado de la opreación consulta de facturas
    /// </summary>
    [DataContract]
    public class VNPRespuestaConsultaFacturas
    {
        /// <summary>
        /// <para /> Valor que indica el resultado de la operación. 0 indica que la transacción de pago de la factura se completó exitosamente, y cualquier valor distinto a 0 indica un error. En caso de ocurrir un error se anulará toda la transacción. 
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Valor que indica el resultado de la operación.</value>
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

        /// <summary>
        /// <para /> Listado de facturas 
        /// </summary>
        /// <value>Listado de facturas.</value>
        [DataMember]
        public ICollection<EstadoFacturas> EstadoFacturas { get; set; }

        /// <summary>
        /// <para /> Resumen de las transacciones realizadas
        /// </summary>
        /// <value>Resumen de las transacciones realizadas.</value>
        [DataMember]
        public ResumenPagos ResumenPagos { get; set; }
    }
    
    /// <summary>
    /// Objeto con el resumen de las transacciones realizadas
    /// </summary>
    [DataContract]
    public class ResumenPagos
    {
        /// <summary>
        /// <para /> Cantidad de facturas pagadas
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Cantidad de facturas pagadas.</value>
        [DataMember]
        public int CantFacturas { get; set; }
        
        /// <summary>
        /// <para /> Cantidad de facturas en pesos pagadas
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Cantidad de facturas en pesos pagadas.</value>
        [DataMember]
        public int CantPesosPagados { get; set; }
        
        /// <summary>
        /// <para /> Suma de las facturas en pesos pagadas
        /// <para /> Tipo de dato <see cref="double"> Double</see>
        /// </summary>
        /// <value>Suma de las facturas en pesos pagadas.</value>
        [DataMember]
        public double SumaPesosPagados { get; set; }
        
        /// <summary>
        /// <para /> Cantidad de facturas en dolares pagadas
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Cantidad de facturas en dolares pagadas.</value>
        [DataMember]
        public int CantDolaresPagados { get; set; }
        
        /// <summary>
        /// <para /> Suma de las facturas en dolares pagadas
        /// <para /> Tipo de dato <see cref="double"> Double</see>
        /// </summary>
        /// <value>Suma de las facturas en dolares pagadas.</value>
        [DataMember]
        public double SumaDolaresPagados { get; set; }
    }
}