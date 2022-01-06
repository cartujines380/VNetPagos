using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ServiceGatewayDto
    {

        public  Guid Id { get; set; }

        public Guid GatewayId { get; set; }
        [CustomDisplay("ServiceDto_Gateway")]
        public virtual GatewayDto Gateway { get; set; }

        [CustomDisplay("ServiceDto_Active")]
        public bool Active { get; set; }

        public bool SendExtract { get; set; }

        [CustomDisplay("ServiceDto_ReferenceId")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceId { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceType { get; set; }

        public string AuxiliarData { get; set; }
        public string AuxiliarData2 { get; set; }
    }
}
