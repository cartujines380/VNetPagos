using System;

namespace VisaNet.Utilities.ExtensionMethods
{
    public static class DateTimeExtensionMethods
    {
        public static int LastDayOfMonth(this DateTime theDay)
        {
            if (theDay.Month == 12) //last month of the year
                return new DateTime(theDay.Year + 1, 1, 1).AddDays(-1).Day;

            return new DateTime(theDay.Year, theDay.Month + 1, 1).AddDays(-1).Day;
        }

        public static DateTime LastDayOfPreviousMonth(this DateTime theDay)
        {
            return new DateTime(theDay.AddMonths(-1).Year, theDay.AddMonths(-1).Month, theDay.AddMonths(-1).LastDayOfMonth());
        }

        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static int DaysDifference(this DateTime lValue, DateTime rValue)
        {
            if (lValue < rValue)
                return (rValue - lValue).Days + 1;

            return (lValue - rValue).Days + 1;
        }

        public static int YearsCountTillNow(this DateTime value)
        {
            var now = DateTime.Now;
            var years = DateTime.Now.Year - value.Year;
            if (now.Month < value.Month || (now.Month == value.Month && now.Day < value.Day)) years--;
            return years;
        }
    }
}
