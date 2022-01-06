using System;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Models
{

    public class LogFiltersModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string UserName { get; set; }
        public LogType? LogType { get; set; }
        public LogUserType? LogUserType { get; set; }
        public LogCommunicationType LogCommunicationType { get; set; }


    }
}