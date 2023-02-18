using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.DI;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddGoblinServices();
        services.AddGoblinDatabases();
    }

    public static void AddGoblinServices(this IServiceCollection services)
    {
        services.AddScoped<IPriceDataFetcher, PriceFetcher>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
    }

    public static void AddGoblinDatabases(this IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<IItemRepository, ItemGateway>();
        services.AddScoped<IRecipeRepository, RecipeGateway>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
        services.AddScoped<GoblinDatabase>();
    }
}
