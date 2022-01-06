using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public interface IBillFilterDto
    {
        Guid ServiceId { get; set; }
        IDictionary<string, string> References { get; set; }
        bool ScheduledPayment { get; set; }
    }

    public class AnonymousUserBillFilterDto : IBillFilterDto
    {
        public Guid ServiceId { get; set; }
        public IDictionary<string, string> References { get; set; }
        public AnonymousUserDto AnonymousUserDto { get; set; }
        private bool _scheduledPayment;
        public bool ScheduledPayment
        {
            get { return _scheduledPayment; }
            set { _scheduledPayment = false; }
        }
    }

    public class RegisteredUserBillFilterDto : IBillFilterDto
    {
        public Guid? CardId { get; set; }
        public Guid? UserId { get; set; }
        public Guid ServiceId { get; set; }
        public IDictionary<string, string> References { get; set; }
        public bool ScheduledPayment { get; set; }
    }

    public class RegisteredUserInputAmountBillFilterDto : IBillFilterDto
    {
        public double Amount { get; set; }
        public CurrencyDto Currency { get; set; }
        public Guid? CardId { get; set; }
        public Guid? UserId { get; set; }
        public Guid ServiceId { get; set; }
        public IDictionary<string, string> References { get; set; }
        private bool _scheduledPayment;
        public bool ScheduledPayment
        {
            get { return _scheduledPayment; }
            set { _scheduledPayment = false; }
        }
    }

    public class AnonymousUserInputAmountBillFilterDto : IBillFilterDto
    {
        public double Amount { get; set; }
        public CurrencyDto Currency { get; set; }
        public Guid ServiceId { get; set; }
        public IDictionary<string, string> References { get; set; }
        public AnonymousUserDto AnonymousUserDto { get; set; }
        private bool _scheduledPayment;
        public bool ScheduledPayment
        {
            get { return _scheduledPayment; }
            set { _scheduledPayment = false; }
        }
    }

    public class InputAmountBillFilterDto
    {
        public double Amount { get; set; }
        public CurrencyDto Currency { get; set; }
    }

}