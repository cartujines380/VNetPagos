using System;

namespace VisaNet.Domain.EntitiesDtos
{
    
    public class NotificationConfigDto
    {
        public Guid Id { get; set; }

        public Guid ServiceAsociatedDtoId { get; set; }
        public ServiceAssociatedDto ServiceAsociatedDto { get; set; }

        public int DaysBeforeDueDate { get; set; }
        public DaysBeforeDueDateConfigDto BeforeDueDateConfigDto { get; set; }
        public SuccessPaymentDto SuccessPaymentDto { get; set; }
        public FailedAutomaticPaymentDto FailedAutomaticPaymentDto { get; set; }
        public NewBillDto NewBillDto { get; set; }
        public ExpiredBillDto ExpiredBillDto { get; set; }
        
    }

    public interface IBasicNotifConfig
    {
        string Label { get; set; }
        bool Email { get; set; }
        bool Sms { get; set; }
        bool Web { get; set; }
    }

    public class DaysBeforeDueDateConfigDto : IBasicNotifConfig
    {
        public string Label { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }

    public class SuccessPaymentDto : IBasicNotifConfig
    {
        public string Label { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }

    public class FailedAutomaticPaymentDto : IBasicNotifConfig
    {
        public string Label { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }

    public class NewBillDto : IBasicNotifConfig
    {
        public string Label { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }

    public class ExpiredBillDto : IBasicNotifConfig
    {
        public string Label { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }
}
