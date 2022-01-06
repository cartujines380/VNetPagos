using System;

namespace VisaNet.Utilities.Helpers
{
    public static class TimeSpanExtensions
    {
        public static string ToSmartString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays > 1)
                return string.Format("{0} día/s", Math.Floor(timeSpan.TotalDays));
            if(timeSpan.TotalHours > 1)
                return string.Format("{0} hora/s", Math.Floor(timeSpan.TotalHours));
            if (timeSpan.Minutes > 1)
                return string.Format("{0} minutos/s", Math.Floor(timeSpan.TotalMinutes));

            return string.Format("{0} segundo/s", Math.Floor(timeSpan.TotalSeconds));
        }
    }
}