using System;
using Serilog;

namespace GilGoblin.Services;

public static class GeneralFunctions
{
    public static DateTime ConvertLongUnixMsToDateTime(this long elapsed_time)
    {
        try
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(elapsed_time).LocalDateTime;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return new DateTime();
        }
    }
}
