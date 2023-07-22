using System;
using Serilog;

namespace GilGoblin.Services;

public static class GeneralFunctions
{
    public static DateTime ConvertLongUnixSecondsToDateTime(long elapsed_time)
    {
        try
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(elapsed_time).LocalDateTime;
            return date;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return new DateTime();
        }
    }
}
