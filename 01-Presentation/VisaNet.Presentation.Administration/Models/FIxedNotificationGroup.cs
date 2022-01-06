using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class FixedNotificationGroup
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string QueryString { get; set; }
        public FixedNotificationLevelDto Level { get; set; }
        public int Count { get; set; }
    }
}