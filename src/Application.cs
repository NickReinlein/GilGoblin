using Serilog;

namespace GilGoblin
{
    public class Application
    {
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/GilGoblin.txt", shared: true, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Console.WriteLine("Hello World");
            Log.CloseAndFlush();
        }
    }
}