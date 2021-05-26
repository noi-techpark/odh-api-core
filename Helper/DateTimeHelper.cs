using System;
using System.Collections.Generic;
using System.Text;
using TimeZoneConverter;

namespace Helper
{
    public class DateTimeHelper
    {
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var tzinfo = TZConvert.GetTimeZoneInfo("W. Europe Standard Time");

            return (TimeZoneInfo.ConvertTimeToUtc(dateTime, tzinfo) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }

        public static double DateTimeToUnixTimestampMilliseconds(DateTime dateTime)
        {
            var tzinfo = TZConvert.GetTimeZoneInfo("W. Europe Standard Time");

            return (TimeZoneInfo.ConvertTimeToUtc(dateTime, tzinfo) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
        }

        ////TEST without conversion to UTC simply convert to UTC what is passed
        //public static double DateTimeToUnixTimestampMillisecondsWithoutUTCConvert(DateTime dateTime)
        //{
        //    return (dateTime -
        //           new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;        
        //}

        //TODO UnixTimeStampToDateTime

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTimeMilliseconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }



    }
}
