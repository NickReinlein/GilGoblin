using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.DI;

public static class DependencyInjectionExtensions
{
    public static void AddGoblinServices(this IServiceCollection services)
    {
        services.AddScoped<IPriceRepository, PriceRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
    }

    public static void AddGoblinDatabases(this IServiceCollection services)
    {
        services.AddScoped<IItemGateway, ItemGateway>();
        services.AddScoped<IRecipeGateway, RecipeGateway>();
        services.AddScoped<IPriceGateway, PriceGateway>();
        services.AddDbContext<GilGoblinDbContext>();
    }
}
