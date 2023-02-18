using GilGoblin.Api.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.Api;

public static class Bootstrap
{
    public static IHostBuilder CreateHostBuilder(string environment, params string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseEnvironment(environment)
            .ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddSingleton(hostContext.Configuration).AddGoblinServices();
                }
            );
    }
}
