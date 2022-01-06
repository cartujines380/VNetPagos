using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de la notificación.
    /// </summary>
    [DataContract]
    public class NotificationResponse : VisaNetAccessBaseResponse
    {
        public NotificationResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }
}