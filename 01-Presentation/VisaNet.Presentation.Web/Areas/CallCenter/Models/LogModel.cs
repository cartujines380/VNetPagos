using System;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Models
{
    public class CallCenterLogModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("LogDto_UserName")]
        public string UserName { get; set; }

        [CustomDisplay("LogDto_DateTime")]
        public DateTime DateTime { get; set; }

        [CustomDisplay("LogDto_LogType")]
        public LogType LogType { get; set; }

        [CustomDisplay("LogDto_LogUserType")]
        public LogUserType LogUserType { get; set; }

        [CustomDisplay("LogDto_LogCommunicationType")]
        public LogCommunicationType LogCommunicationType { get; set; }

        [CustomDisplay("LogDto_Message")]
        public string Message { get; set; }
    }
}
