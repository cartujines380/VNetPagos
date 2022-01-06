using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos.NotificationsData
{
    public class NewBillNotificationInfoDto
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string BillNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public BillTypeDto BillType { get; set; }

        public Guid? ServiceId { get; set; }
        //public ServiceDto ServiceDto { get; set; }

        public Guid? ApplicationUserId { get; set; }
        //public ApplicationUserDto ApplicationUserDto { get; set; }

    }
}