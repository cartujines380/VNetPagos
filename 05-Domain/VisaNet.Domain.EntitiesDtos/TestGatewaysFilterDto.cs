using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class TestGatewaysFilterDto
    {
        public Guid ServiceId { get; set; }
        public ServiceGatewayDto GatewayDto { get; set; }
        public string[] References { get; set; }
    }
}
