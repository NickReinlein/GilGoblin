using Serilog;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

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

            var s = new GilGoblinService();
        }

        public static IBackgroundService CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<GilGoblinService>();
            return services;
        }
    }
}