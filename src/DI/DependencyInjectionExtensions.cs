using GilGoblin.Database;
using GilGoblin.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DI
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddGateways();
        }

        public static IServiceCollection AddGateways(this IServiceCollection services)
        {

            services
                .AddSingleton(_ => new MarketDataGateway())
                .AddSingleton(_ => new RecipeGateway());
            return services;
        }
    }
}