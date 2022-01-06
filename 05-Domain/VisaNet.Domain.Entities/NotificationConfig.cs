using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("NotificationConfigs")]
    [TrackChanges]
    public class NotificationConfig : EntityBase
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }

        public Guid ServiceAsociatedId { get; set; }
        
        public int DaysBeforeDueDate { get; set; }
        public DaysBeforeDueDateConfig BeforeDueDateConfig { get; set; }
        public SuccessPayment SuccessPayment { get; set; }
        public FailedAutomaticPayment FailedAutomaticPayment { get; set; }
        public NewBill NewBill { get; set; }
        public ExpiredBill ExpiredBill { get; set; }
        
    }

    [ComplexType]
    public class DaysBeforeDueDateConfig
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }
    [ComplexType]
    public class SuccessPayment
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }
    [ComplexType]
    public class FailedAutomaticPayment
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }
    [ComplexType]
    public class NewBill
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }
    [ComplexType]
    public class ExpiredBill
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool Web { get; set; }
    }


}
