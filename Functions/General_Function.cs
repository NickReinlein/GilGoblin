using System;

namespace GilGoblin.Functions
{
    internal static class General_Function
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
                Console.WriteLine(ex.Message);
                return new DateTime();
            }
        }

        public static DateTime ConvertLongUnixMillisecondsToDateTime(long elapsed_time)
        {
            return ConvertLongUnixSecondsToDateTime(elapsed_time / 1000);
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
                Console.WriteLine(ex.Message);
                return new DateTime();
            }

        }

        
    }
}
