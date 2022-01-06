using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto con los datos a utilizar para realizar el pago de la factura
    /// </summary>
    [DataContract]
    public class CobrarFacturaData
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
        /// Factura a pagar en VisaNetPagos
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Objeto FacturaOnline</value>
        [DataMember]
        [Required(ErrorMessage = "El campo Factura es obligatorio.")]
        public FacturaOnline Factura { get; set; }

        /// <summary>
        /// <para />Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.
        /// <para />El orden de los parametros es el siguiente: IdApp, Factura (CodComercio, CodSucursal, IdUsuario, IdTarjeta, NroFactura, 
        /// Descripcion, FchFactura (para la firma se utiliza formato YYYYMMDD), Moneda, MontoTotal(#0,00), Indi, MontoGravado(#0,00), ConsFinal , Cuotas, IdOperacion, IdMerchant,
        /// DeviceFingerprint, IpCliente, TelefonoCliente, DireccionEnvioCliente (Calle, NumeroPuerta, Complemento, Esquina, Barrio, CodigoPostal, Latitud, Longitud, Telefono, Ciudad, Pais))
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo FirmaDigital es obligatorio.")]
        public string FirmaDigital { get; set; }
    }
}