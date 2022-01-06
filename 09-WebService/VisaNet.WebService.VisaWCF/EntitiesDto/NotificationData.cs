using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Tipo de notificación.
    /// </summary>
    public enum NotificationType
    {
        Ok = 1,
        Error = 2
    }

    /// <summary>
    /// Operación.
    /// </summary>
    public enum Operation
    {
        Payment = 1,
        Void = 2,
        Refund = 3,
        Reversal = 4
    }

    /// <summary>
    /// Parámetros para el registro del resultado de una operación.
    /// </summary>
    [DataContract]
    public class NotificationData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Tipo de notificación.
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "NotificationType")]
        public NotificationType NotificationType { get; set; }
        /// <summary>
        /// Operación.
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Operation")]
        public Operation Operation { get; set; }
        /// <summary>
        /// Mensaje.
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Message")]
        public string Message { get; set; }
    }
}