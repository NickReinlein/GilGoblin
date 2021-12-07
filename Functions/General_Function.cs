using System;

namespace GilGoblin.Functions
{
    internal static class General_Function
    {
        public static DateTime Conver_Long_To_DateTime(long elapsed_time)
        {
            try
            {
                DateTimeOffset dateTimeOffset =
                    DateTimeOffset.FromUnixTimeSeconds(elapsed_time);
                DateTime date = dateTimeOffset.LocalDateTime;
                DateTime date2 = DateTime.FromFileTime(elapsed_time);

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
