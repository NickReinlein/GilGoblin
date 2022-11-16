using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;
using GilGoblin.DI;

namespace GilGoblin
{
    public static class Bootstrap
    {
        public static IHostBuilder CreateHostBuilder(params string[] args) => CreateHostBuilder(EnvironmentName, args);
        public static IHostBuilder CreateHostBuilder(string environment, params string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseEnvironment(environment)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(hostContext.Configuration)
                        .AddServices();
                });
        }
        private const string NETCORE_ENVIRONMENT = nameof(NETCORE_ENVIRONMENT);
        public static string EnvironmentName => (Environment.GetEnvironmentVariable(NETCORE_ENVIRONMENT) ?? "local").ToLower();
    }
}