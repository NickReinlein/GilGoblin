using System;

namespace GilGoblin.Functions
{
    internal static class General_Function
    {
        public static DateTime ConvertLongToDateTime(long elapsed_time)
        {
            try
            {
                DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(elapsed_time).LocalDateTime;
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
