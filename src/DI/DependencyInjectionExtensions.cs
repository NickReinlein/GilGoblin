using GilGoblin.Crafting;
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
}
