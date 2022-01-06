using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class ServiceListModel
    {
        public bool Active { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceImageName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceRefValue { get; set; }
        public string ServiceRefValue2 { get; set; }
        public string ServiceRefValue3 { get; set; }
        public string ServiceRefName { get; set; }
        public string ServiceRefName2 { get; set; }
        public string ServiceRefName3 { get; set; }
        public string ServiceRefValue4 { get; set; }
        public string ServiceRefValue5 { get; set; }
        public string ServiceRefValue6 { get; set; }
        public string ServiceRefName4 { get; set; }
        public string ServiceRefName5 { get; set; }
        public string ServiceRefName6 { get; set; }


        public List<string> CardsMask { get; set; }
        public string DefaultMask { get; set; }
        public Guid? ServiceAutomaticPaymentId { get; set; }

        public bool EnableAutomaticPayment { get; set; }
        //public bool IsApp { get; set; }
        //public bool Deleted { get; set; }
        public List<CardModel> Cards { get; set; }
        public string ServiceContainerName { get; set; }

        //public GatewayEnumDto GatewayEnumDto { get; set; }
        public bool AskUserForReferences { get; set; }

        public bool AllowGetBills { get; set; }
        public bool AllowInputAmount { get; set; }

    }
}