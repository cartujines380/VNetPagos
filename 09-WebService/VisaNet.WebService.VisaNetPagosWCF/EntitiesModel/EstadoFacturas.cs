using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto con la información de las facturas pagadas
    /// </summary>
    [DataContract]
    public class EstadoFacturas
    {
        /// <summary>
        /// <para /> Referencia numero 1 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente1 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 2 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente2 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 3 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente3 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 4 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente4 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 5 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente5 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 6 del cliente
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 100 caracteres.</value>
        [DataMember]
        public string RefCliente6 { get; set; }

        /// <summary>
        /// <para /> Número de factura asociada.
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string NroFactura { get; set; }

        /// <summary>
        /// <para /> Moneda de la factura.
        /// <para> El campo deberá ser "N" para moneda nacional (pesos uruguayos) o "D" para dólares americanos.</para>
        /// <para /> Tipo de dato <see cref="String"> String</see>
        /// </summary>
        /// <value>El campo deberá ser "N" para moneda nacional (pesos uruguayos) o "D" para dólares americanos.</value>
        [DataMember]
        public string Moneda { get; set; }

        /// <summary>
        /// <para /> Monto total de la factura. 
        /// <para> El campo deberá ser Decimal de 18,2. </para> 
        /// <para /> Tipo de dato <see cref="System.Decimal"> Decimal </see>
        /// </summary>
        /// <value>El campo deberá ser Decimal de 18,2. </value>
        [DataMember]
        public double MontoTotal { get; set; }

        /// <summary>
        /// <para /> Fecha en que se realizó el pago de la factura.
        /// <para /> Tipo de dato <see cref="DateTime"> DateTime</see>
        /// </summary>
        /// <value> Fecha en que se realizó el pago de la factura.</value>
        [DataMember]
        public DateTime FchPago { get; set; }

        /// <summary>
        /// <para /> No aplica actualmente. 
        /// <para /> Tipo de dato <see cref="short"> Short </see>
        /// </summary>
        /// <value>No aplica actualmente. </value>
        [DataMember]
        public int CantCuotas { get; set; }

        /// <summary>
        /// <para /> Monto descontado. 
        /// <para> El campo deberá ser Decimal de 18,2. </para> 
        /// <para /> Tipo de dato <see cref="System.Decimal"> Decimal </see>
        /// </summary>
        /// <value>El campo deberá ser Decimal de 18,2. </value>
        [DataMember]
        public double MontoDescIVA { get; set; }

        /// <summary>
        /// <para /> Estado de la factura
        /// <para /> Los estados posibles son los siguientes: Realizada = 0, Cancelada = 1
        /// <para /> Tipo de dato <see cref="short"> short </see>
        /// </summary>
        /// <value>Los estados posibles son los siguientes: Realizada = 0, Cancelada = 1</value>
        [DataMember]
        public int Estado { get; set; }

        /// <summary>
        /// Valor que indica el resultado de la operación. 
        /// 0 indica que la transacción de pago de la factura se completó exitosamente, y cualquier valor distinto a 0 indica un error. 
        /// En caso de ocurrir un error se anulará toda la transacción. 
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        [DataMember]
        public int CodError{ get; set; }

        /// <summary>
        /// Descripción indicando el resultado de la operación ejecutada
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        [DataMember]
        public string DescEstado { get; set; }

        /// <summary>
        /// <para /> En coordinación con VisaNet, se podrían incluir campos auxiliares para funcionalidades específicas.
        /// </summary>
        [DataMember]
        public ICollection<AuxiliarData> AuxiliarData { get; set; }
    }
}