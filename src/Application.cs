using Serilog;

namespace GilGoblin
{
    public class Application
    {
        public static void Main()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/GilGoblin.txt", shared: true, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Console.WriteLine("Hello World");

            var context = Database.DatabaseAccess.GetContext();
            var recipe = context.Recipe.First();

            Log.CloseAndFlush();
        }
    }
}