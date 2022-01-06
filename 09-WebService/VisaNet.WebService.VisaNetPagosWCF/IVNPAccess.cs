using System.ServiceModel;
using System.ServiceModel.Web;
using VisaNet.WebService.VisaNetPagosWCF.EntitiesModel;

namespace VisaNet.WebService.VisaNetPagosWCF
{
    /// <summary>
    /// Proporciona los métodos necesarios para realizar el pago de facturas, anulación de transacciónes, consulta de transacciónes, servicios asociados y baja de tarjetas.
    /// </summary>
    [ServiceContract]
    public interface IVNPAccess
    {
        /// <summary>
        /// Este método permite a la App procesar el cobro de una factura específica en línea en caso que exista un Usuario asociado al Servicio con autorización explícita para esta operativa.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para realizar el pago de una factura.</param>
        /// <returns>Resultado de el pago de la factura.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "CobroFacturaOnlineApp")]
        VNPRespuesta CobroFacturaOnlineApp(CobrarFacturaData data);

        /// <summary>
        /// Este metodo permite a la App anular un cobro previamente procesado por CobroFacturaOnlineApp.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para realizar la anulación de una transacción.</param>
        /// <returns>Resultado de la anulación de una transacción.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "AnulacionCobroApp")]
        VNPRespuesta AnulacionCobroApp(AnularFacturaData data);

        /// <summary>
        /// Este método devuelve las facturas registradas en VisaNetPagos por medio del metodo CobroFacturaOnlineApp.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para realizar la busqueda de transacciones.</param>
        /// <returns>Resultado de la obtención del listado de transacciones.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "ConsultaTransacciones")]
        VNPRespuestaConsultaFacturas ConsultaTransacciones(ConsultaFacturasData data);
        
        /// <summary>
        /// Este metodo permite a la App consultar todos los comercios disponibles para cobros mediante el método CobroFacturaOnline.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para realizar la búsqueda de comercios.</param>
        /// <returns>Resultado de la obtención del listado de comercios.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "ConsultaComercios")]
        VNPRespuestaConsultaComercios ConsultaComercios(ConsultaComerciosData data);

        /// <summary>
        /// Este metodo permite a la App realizar la baja de una tarjeta asociada por parte de un usuario o la baja de la asociación entre el usuario y el servicio.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para la baja de tarjetas. Si no se pasa una tarjeta se procedera a la baja de asociación usuario-servicio.</param>
        /// <returns>Resultado de la baja de tarjeta.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "BajaTarjeta")]
        VNPRespuestaBajaTarjeta BajaTarjeta(BajaTarjetaData data);

        /// <summary>
        /// Este metodo permite a la App consultar todas las asociaciones realizadas en VisaNetOn/VisaNetPagos y la respuesta obtenida por el comercio.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para realizar la búsqueda de asociaciones.</param>
        /// <returns>Resultado de la obtención del listado de asociaciones.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "ConsultaAsociaciones")]
        VNPRespuestaConsultaUrlTransaccion ConsultaUrlTransaccion(ConsultaUrlTransaccionData data);
    }
}
