using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public class DeleteCardDto
    {
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }
        public string RequestId { get; set; }
        public string Token { get; set; }
        public LogUserType UserType { get; set; }
        public string IdOperation { get; set; }
        public string IdTransaccion { get; set; }
    }
}
