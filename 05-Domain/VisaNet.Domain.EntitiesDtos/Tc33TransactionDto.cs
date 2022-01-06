using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class Tc33TransactionDto
    {
        public Guid Id { get; set; }

        public string RequestId { get; set; }
        public Guid Tc33Id { get; set; }
        public Tc33Dto Tc33 { get; set; }
        public string CreationUser { get; set; }

    }
}
