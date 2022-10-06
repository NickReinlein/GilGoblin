using Serilog;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;

namespace GilGoblin
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/GilGoblin.txt", shared: true, rollingInterval: RollingInterval.Day)
            .CreateLogger();

            var builder = Bootstrap.CreateHostBuilder(args);

            builder.Build().Run();

            Console.WriteLine("Hello world!");

        }
    }
}