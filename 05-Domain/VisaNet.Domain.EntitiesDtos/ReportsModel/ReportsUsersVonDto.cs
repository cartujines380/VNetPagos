using System;

namespace VisaNet.Domain.EntitiesDtos.ReportsModel
{
    public class ReportsUsersVonDto
    {
        public Guid AnonymousUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserExternalId { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string AppId { get; set; }
        public int PaymentsCount { get; set; }
        public int CardsCount { get; set; }
    }
}