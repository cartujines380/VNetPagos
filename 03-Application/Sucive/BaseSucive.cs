using System;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Sucive
{
    public class BaseSucive
    {
        private readonly IServiceFixedNotification _serviceFixedNotification;

        public BaseSucive(IServiceFixedNotification serviceFixedNotification)
        {
            _serviceFixedNotification = serviceFixedNotification;
        }

        public void NotificationFix(string preBill, string transactionId, string codigoretorno, string mensajeretorno,DepartamentDtoType dpto, Exception exception)
        {
            if (string.IsNullOrEmpty(codigoretorno) || codigoretorno.Equals("000")) return;

            var notiService = string.Format("Sucive ({0}) Error al confirmar.", dpto);
            var notiDetail = string.Format("Ha ocurrido un error al intentar confirmar una transacciión en sucive.<br /><br />");
            notiDetail += string.Format("El codigo de error es el {0}, motivo: {1}.<br />", codigoretorno, mensajeretorno);
            notiDetail += string.Format("Los datos de la transacción son los siguientes: <br /> " +
                                            "Nro de pre factura en sucive : {0}<br />" +
                                            "Nro de Transaccion en cybersource: {1}<br />", preBill, transactionId);

            if (exception != null)
            {
                notiDetail += _serviceFixedNotification.ExceptionMsg(exception);
            }

            _serviceFixedNotification.Create(new FixedNotificationDto()
            {
                Category = FixedNotificationCategoryDto.GatewayError,
                DateTime = DateTime.Now,
                Level = FixedNotificationLevelDto.Error,
                Description = notiService,
                Detail = notiDetail
            });
        }
    }
}
