using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class FixedNotificationDto
    {
        public Guid Id { get; set; }
        public FixedNotificationLevelDto Level { get; set; }
        public FixedNotificationCategoryDto Category { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public bool Resolved { get; set; }
        public string Comment { get; set; }
    }
}