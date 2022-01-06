using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto con los datos a utilizar para realizar la consulta de transacciones realizadas
    /// </summary>
    [DataContract]
    public class ConsultaFacturasData
    {
        /// <summary>
        /// <para /> Indicador que identifica la operación del lado de la APP.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Indicador que identifica la operación del lado de la APP.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdOperacion es obligatorio.")]
        public string IdOperacion { get; set; }

        /// <summary>
        /// <para /> Código asignado por VisaNet al momento del alta de una nueva App.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Código asignado por VisaNet al momento del alta de una nueva App.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdApp es obligatorio.")]
        public string IdApp { get; set; }

        /// <summary>
        /// <para /> Fecha en que se realizo la transacción
        /// <para /> Tipo de dato <see cref="System.DateTime"> DateTime</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Fecha en que se realizo la transacción.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo Fecha es obligatorio.")]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// <para /> Código del comercio que asigna VisaNet al momento del alta de una nueva Institución. 
        /// <para /> El campo no puede exceder el largo de 7 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 7 caracteres</value>
        [DataMember]
        [StringLength(7, ErrorMessage = "El campo CodComercio excede el largo máximo permitido (7).")]
        public string CodComercio { get; set; }

        /// <summary>
        /// <para /> Código de Sucursal que asigna VisaNet al momento del alta de una nueva Institución.
        /// <para /> El campo no puede exceder el largo de 3 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 3 caracteres</value>
        [DataMember]
        [StringLength(3, ErrorMessage = "El campo CodSucursal excede el largo máximo permitido (3).")]
        public string CodSucursal { get; set; }

        /// <summary>
        /// <para /> Es el valor asignado por VisaNet en el proceso de creación de Comercio.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        [DataMember]
        public string IdMerchant { get; set; }

        /// <summary>
        /// <para /> Número de factura asociada.
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string NroFactura { get; set; }

        /// <summary>
        /// <para /> Referencia numero 1 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente { get; set; }

        /// <summary>
        /// <para /> Referencia numero 2 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente2 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 3 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente3 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 4 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente4 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 5 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente5 { get; set; }

        /// <summary>
        /// <para /> Referencia numero 6 del cliente
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        public string RefCliente6 { get; set; }

        /// <summary>
        /// <para />Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.
        /// <para />El orden de los parametros es el siguiente: IdApp, CodComercio, CodSucursal, Fecha (para la firma se utiliza formato YYYYMMDD), NroFactura, RefCliente,
        /// RefCliente2, RefCliente3, RefCliente4, RefCliente5, RefCliente6, IdOperacion, IdMerchant
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo FirmaDigital es obligatorio.")]
        public string FirmaDigital { get; set; }
        
    }
}