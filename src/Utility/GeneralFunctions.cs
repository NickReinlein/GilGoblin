using Serilog;
using System;

namespace GilGoblin
{
    internal static class GeneralFunctions
    {
        public static DateTime ConvertLongUnixSecondsToDateTime(long elapsed_time)
        {
            try
            {
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(elapsed_time).LocalDateTime;
                return date;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new DateTime();
            }
        }

        public static DateTime ConvertLongUnixMillisecondsToDateTime(long elapsed_time)
        {
            DateTime date
                = DateTimeOffset.FromUnixTimeMilliseconds(elapsed_time).LocalDateTime;
            return date;
        }

        public static DateTime ConvertLongFromBinaryToDateTime(long elapsed_time)
        {
            try
            {
                DateTime date = DateTime.FromBinary(elapsed_time).ToLocalTime();
                return date;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new DateTime();
            }

        }


    }
}
