using System;
using System.Collections.Generic;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ServiceCardListModel
    {

        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceRefValue { get; set; }
        public string ServiceRefValue2 { get; set; }
        public string ServiceRefValue3 { get; set; }
        public string ServiceRefValue4 { get; set; }
        public string ServiceRefValue5 { get; set; }
        public string ServiceRefValue6 { get; set; }
        public string ServiceRefName { get; set; }
        public string ServiceRefName2 { get; set; }
        public string ServiceRefName3 { get; set; }
        public string ServiceRefName4 { get; set; }
        public string ServiceRefName5 { get; set; }
        public string ServiceRefName6 { get; set; }

        public ICollection<CardListModel> Cards { get; set; }

        public string ServiceNameFiltered { get; set; }
    }
}