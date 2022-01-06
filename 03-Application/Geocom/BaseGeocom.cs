using System;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Geocom
{
    public class BaseGeocom
    {
        private readonly IServiceFixedNotification _serviceFixedNotification;

        public BaseGeocom(IServiceFixedNotification serviceFixedNotification)
        {
            _serviceFixedNotification = serviceFixedNotification;
        }

        public void NotificationFix(int codResponse, string codDesc, int idPadron, string[] references, string preFactura, string service, string method, DepartamentDtoType dtpo, string transactionId)
        {
            var notify = true;
            if (codResponse == 0) return;

            switch (codResponse)
            {
                case 111: //NO ESTA HABILITADO VISANETPAGOS
                    var notiService = string.Format("Geocom ({0}) deshabilitado.", dtpo + " - " + service);
                    var notiDetail = string.Format("Ha ocurrido un error al consultar el servicio Geocom - {0}.<br /><br />" +
                                                   "El codigo de error {1} significa que VisaNet no esta habilitada para consultar el servicio.<br />" +
                                                   "Posiblemente sea un problema de IP entre VisaNet y Geocom. <br /><br />" +
                                                   "Los datos de la consulta son: <br/> Metodo: {2} <br/> Id Padron: {3} <br/> Referencias: {4} <br/>" +
                                                   "Pre factura: {5} <br/>", dtpo + " - " + service, codResponse, method, idPadron < 0 ? string.Empty : idPadron.ToString(),
                                                   references != null ? string.Join("; ", references) : string.Empty, preFactura);

                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.ServiceConfiguration,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Error,
                        Description = notiService,
                        Detail = notiDetail
                    });
                    notify = false;
                    break;

            }

            if (!notify) return;
            
            switch (method)
            {
                case "Ws05ConfirmacionSoapPortClient":
                    var notiService = string.Format("Geocom ({0}) Error al confirmar.", dtpo + " - " + service);
                    var notiDetail = string.Format("Ha ocurrido un error al intentar confirmar una transacción en Geocom - {0}.<br /><br />", dtpo + " - " + service);
                    notiDetail += string.Format("El codigo de error es el {0}, motivo: {1}.<br />", codResponse, codDesc);
                    notiDetail += string.Format("Los datos de la transacción son los siguientes: <br /> " +
                                            "Nro de pre factura en sucive : {0}<br />" +
                                            "Nro de Transaccion en cybersource: {1}<br />", preFactura, transactionId);

                    _serviceFixedNotification.Create(new FixedNotificationDto()
                    {
                        Category = FixedNotificationCategoryDto.GatewayError,
                        DateTime = DateTime.Now,
                        Level = FixedNotificationLevelDto.Error,
                        Description = notiService,
                        Detail = notiDetail
                    });
                    notify = false;
                    break;
            }
        }
    }
}
