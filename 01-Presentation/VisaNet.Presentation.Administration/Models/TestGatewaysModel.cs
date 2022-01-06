using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Administration.Models
{
    public class TestGatewaysModel
    {
        public string GatewayName { get; set; }

        public bool SuccessfulConnection { get; set; }
        public string ErrorMessage { get; set; }

        public List<BillDto> Bills { get; set; }
    }
}