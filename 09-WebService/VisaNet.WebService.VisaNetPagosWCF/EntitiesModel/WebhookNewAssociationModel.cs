using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto con los datos necesarios evniados al comercio por medio de la url_transacción
    /// </summary>
    [DataContract]
    public class WebhookUrlTransactionModel
    {
        /// <summary>
        /// Correo electrónico asociado al usuario. 
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// Referencia del usuario a quien pertenecen los datos. Este dato es creado por VisaNetOn, y será la forma de relacionar los usuarios entre ambas partes.  Nota: En caso que se esté asociando una tarjeta a un usuario previamente creado (ya existente), solamente se enviará el IdUsuario y no será necesario enviar toda la información del mismo.
        /// </summary>
        [DataMember]
        public string IdUsuario { get; set; }
        /// <summary>
        /// Id de la Tarjeta asociada al Usuario. 
        /// </summary>
        [DataMember]
        public string IdTarjeta { get; set; }
        /// <summary>
        /// Se indicara la fecha de vencimiento de la tarjeta. Formato MM/AA. La App se ocupara de validar que no se envié un cobro con una tarjeta vencida y le podrá indicar al Usuario que debe ingresar una tarjeta nueva. 
        /// </summary>
        [DataMember]
        public string VencTarjeta { get; set; }
        /// <summary>
        /// Sufijo de la tarjeta VISA. Se enviarán únicamente los últimos 4 números de la tarjeta a modo de que el Usuario pueda identificarla, principalmente en los casos que la App permita asociar más de una Tarjeta
        /// </summary>
        [DataMember]
        public string SufijoTarjeta { get; set; }

        /// <summary>
        /// Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente1 { get; set; }
        /// <summary>
        ///Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente2 { get; set; }
        /// <summary>
        ///Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente3 { get; set; }
        /// <summary>
        ///Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente4 { get; set; }
        /// <summary>
        ///Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente5 { get; set; }
        /// <summary>
        ///Referencias del servicio que se está asociando o pagando. La utilización de estos campos será acordada con cada comercio al momento de la configuración del servicio, su objetivo es enviarle al comercio datos del pago o de la asociación del usuario.
        /// </summary>
        [DataMember]
        public string RefCliente6 { get; set; }
        /// <summary>
        /// Repite IdOperacion que fue informado por la App al invocar a la página de Asociación. En caso de originarse la asociación desde VisaNetOn se omite. 
        /// </summary>
        [DataMember]
        public string IdOperacionApp { get; set; }
        /// <summary>
        /// Indicador que identifica la operación. El IdOperacion debe ser único en cada llamada por parte de App. 
        /// </summary>
        [DataMember]
        public string IdOperacion { get; set; }
        /// <summary>
        /// True – Se envían datos para asociación de servicio/tarjeta. 
        /// False – No se enviaran datos para asociación de servicio/tarjeta. 
        /// </summary>
        [DataMember]
        public string EnvioAsociacion { get; set; }
        /// <summary>
        /// True – Se envían datos sobre pago de factura. 
        /// False – No se envían datos sobre pago de factura. 
        /// </summary>
        [DataMember]
        public string EnvioPago { get; set; }
        /// <summary>
        /// Nro de transacción de pago de factura de VisaNet. 
        /// </summary>
        [DataMember]
        public string NroTransacción { get; set; }
        /// <summary>
        /// Monto descuento de la factura incluyendo dos decimales sin separador. 
        /// </summary>
        [DataMember]
        public string FacturaImporteDescuento { get; set; }
        /// <summary>
        /// 1 – Tarjeta débito nacional 
        /// 2 – Tarjeta crédito nacional 
        /// 3 – Tarjeta débito internacional 
        /// 4 - Tarjeta crédito internacional 
        /// 5 – Tarjeta prepaga nacional 
        /// 6 – Tarjeta prepaga internacional  
        /// </summary>
        [DataMember]
        public string TipoTarjeta { get; set; }
        /// <summary>
        /// Nombre del emisor de la tarjeta. 
        /// </summary>
        [DataMember]
        public string EmisorTarjeta { get; set; }
        /// <summary>
        /// Código del emisor de la tarjeta.
        /// </summary>
        [DataMember]
        public string CodEmisorTarjeta { get; set; }
        /// <summary>
        /// Nombre del programa de la tarjeta.
        /// </summary>
        [DataMember]
        public string ProgramaTarjeta { get; set; }
        /// <summary>
        /// Código del programa de la tarjeta.
        /// </summary>
        [DataMember]
        public string CodProgramaTarjeta { get; set; }

        /// <summary>
        /// Código de respuesta devuelto por el comercio cuando se notifico por medio de la url_transacción
        /// </summary>
        [DataMember]
        public string HttpCodigoRespuesta { get; set; }
    }
}