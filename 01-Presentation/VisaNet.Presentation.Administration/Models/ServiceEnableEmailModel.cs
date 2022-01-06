using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceGatewayModel
    {
        public Guid Id { get; set; }
        public Guid GatewayId { get; set; }
        public int GatewayEnum { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string GatewayName { get; set; }
        public bool Active { get; set; }
        
        public bool SendExtract { get; set; }

        [CustomDisplay("ServiceGateway_ReferenceId")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceId { get; set; }
        [CustomDisplay("ServiceGateway_ServiceType")]
        public string ServiceType { get; set; }

        public string AuxiliarData { get; set; }
        public string AuxiliarData2 { get; set; }
    }
}