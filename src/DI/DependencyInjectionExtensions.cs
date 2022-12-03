using GilGoblin.Database;
using GilGoblin.Web;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.DI
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddGateways().AddHostedServices();
        }

        public static IServiceCollection AddGateways(this IServiceCollection services)
        {
            services
                .AddSingleton(_ => new MarketDataGateway())
                .AddSingleton(_ => new RecipeGateway())
                .AddSingleton(_ => new ItemGateway());
            return services;
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services.AddHostedService<GilGoblinService>();
        }
    }
}
