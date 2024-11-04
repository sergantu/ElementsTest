using System;

namespace CodeBase.Utils
{
    public static class TimeUtils
    {
        private const int SEC_IN_MINUTE = 60;

        public static int Now()
        {
            return ToUnixTimestamp(DateTime.Now);
        }

        public static long NowMs()
        {
            return ToMsTimestamp(DateTime.Now);
        }

        public static int ToUnixTimestamp(DateTime dateTime)
        {
            return (int) dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long ToMsTimestamp(DateTime dateTime)
        {
            return (long) dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DateTime FromUnixTimestamp(int timestamp)
        {
            DateTime dateTime = GetDefaultDateTime();
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }

        public static string ConvertToTimerFormat(int time)
        {
            return TimeSpan.FromSeconds(time).ToString("hh\\:mm\\:ss");
        }

        public static string ConvertToTimerWithoutSeconds(int time)
        {
            return TimeSpan.FromSeconds(time + SEC_IN_MINUTE).ToString("hh\\:mm");
        }

        public static string ConvertToDateTimeFormat(int time)
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(time).DateTime;
            return date.ToString("dd.MM.yyyy hh:mm");
        }

        public static int MinToSec(int minutes)
        {
            return minutes * SEC_IN_MINUTE;
        }

        public static int TimeZoneOffset()
        {
            return (int) TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
        }

        public static DateTime FromTimestamp(int timestamp)
        {
            DateTime dateTime = GetDefaultDateTime();
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }

        public static DateTime GetDefaultDateTime()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}