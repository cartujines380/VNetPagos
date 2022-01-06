using System.ServiceModel;
using System.ServiceModel.Web;

using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    /// <summary>
    /// Proporciona los métodos necesarios para la obtención, realización y consulta de pagos de servicios.
    /// </summary>
    [ServiceContract]
    public interface IVisaNetAccess
    {
        /// <summary>
        /// Devuelve un listado de los servicios habilitados para pagar.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para la obtención de servicios.</param>
        /// <returns>Resultado de la obtención de servicios.</returns>
        /// <example>
        /// <code source="Examples.cs" region="GetServices" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "GetServices")]
        ServicesResponse GetServices(ServicesData data);

        /// <summary>
        /// Devuelve un listado de las facturas pendientes de pago correspondientes a un determinado comercio/servicio y referencia de pago, para una pasarela determinada.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para la obtención de facturas a pagar.</param>
        /// <returns>Resultado de la obtención de facturas.</returns>
        /// <example>
        /// <code source="Examples.cs" region="SearchBills" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "SearchBills")]
        BillsResponse SearchBills(BillsData data);

        /// <summary>
        /// Realiza el procesamiento previo al pago de un listado de facturas. 
        /// Devuelve, en el caso de Geocom o Sucive, una única factura consolidada y, en otro caso, una lista de facturas listas para ser pagadas, con los correspondientes descuentos aplicados.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para el preprocesamiento de facturas.</param>
        /// <returns>Resultado del preprocesamiento.</returns>
        /// <example>
        /// <code source="Examples.cs" region="PreprocessPayment" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "PreprocessPayment")]
        PreprocessPaymentResponse PreprocessPayment(PreprocessPaymentData data);

        /// <summary>
        /// Realiza el pago de una determinada factura.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para el pago de una factura.</param>
        /// <returns>Resultado del proceso de pago.</returns>
        /// <example>
        /// <code source="Examples.cs" region="Payment" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "Payment")]
        PaymentResponse Payment(PaymentData data);

        /// <summary>
        /// Devuelve el pago efectuado para una determinada factura (opcional), o un listado de todos los pagos realizados dentro del rango de fechas especificado (opcional), para un determinado servicio (opcional).
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para la búsqueda de pagos realizados.</param>
        /// <returns>Resultado de la búsqueda de pagos.</returns>
        /// <example>
        /// <code source="Examples.cs" region="SearchPayments" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "SearchPayments")]
        SearchPaymentsResponse SearchPayments(SearchPaymentsData data);

        /// <summary>
        /// Permite notificar sobre el resultado de una operación.
        /// </summary>
        /// <param name="data">Contiene los parámetros necesarios para la notificación.</param>
        /// <returns>Resultado de la notificación.</returns>
        /// <example>
        /// <code source="Examples.cs" region="NotifyOperationResult" lang="cs" title="C#" />
        /// </example>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "NotifyOperationResult")]
        NotificationResponse NotifyOperationResult(NotificationData data);
    }

}
