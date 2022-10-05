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
            return services.AddDatabase();
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {

            services
                .AddSingleton(_ => new GilGoblinDbContext())
                .AddSingleton(_ => new RecipeGateway());
            return services;
        }

        private static string GetReadConnectionString(this IServiceProvider service)
        {
            return service.GetService<IConfiguration>().GetConnectionString("Read");
        }

        private static string GetWriteConnectionString(this IServiceProvider service)
        {
            return service.GetService<IConfiguration>().GetConnectionString("Write");
        }
    }
}