using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.AspNetCore.Builder;
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
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        return services;
    }

    public static IServiceCollection AddGoblinDatabases(this IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<IItemRepository, ItemGateway>();
        services.AddScoped<IRecipeRepository, RecipeGateway>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
        services.AddScoped<GoblinDatabase>();
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
}
