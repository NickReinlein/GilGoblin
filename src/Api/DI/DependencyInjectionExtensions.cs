using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.DI;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGoblinServices(this IServiceCollection services)
    {
        services.AddGoblinCrafting();
        services.AddGoblinDatabases();
        services.AddBasicBuilderServices();
        return services;
    }

    public static IServiceCollection AddGoblinCrafting(this IServiceCollection services)
    {
        services.AddScoped<IPriceDataFetcher, PriceFetcher>();
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
        return services;
    }

    public static IServiceCollection AddGoblinDatabases(this IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<GoblinDatabase>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
        services.AddSingleton<IItemRepository, ItemGateway>();
        services.AddSingleton<IRecipeRepository, RecipeGateway>();
        return services;
    }

    public static IServiceCollection AddBasicBuilderServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static WebApplication AddAppGoblinServices(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }

    public static WebApplicationBuilder GetGoblinBuilder(this string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseDefaultServiceProvider(
            (_, options) =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            }
        );
        builder.Services.AddGoblinServices();
        return builder;
    }
}
