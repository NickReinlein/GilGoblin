using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;

namespace GilGoblin.DI;

public static class DependencyInjectionExtensions
{
    public static void AddGoblinServices(this IServiceCollection services)
    {
        services.AddGoblinDatabases();
        services.AddScoped<IPriceRepository<PriceWebPoco>, PriceFetcher>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddScoped<GoblinDatabase>();
    }

    public static void AddGoblinDatabases(this IServiceCollection services)
    {
        services.AddScoped<IItemRepository, ItemGateway>();
        services.AddScoped<IRecipeRepository, RecipeGateway>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
        services.AddDbContext<GilGoblinDbContext>();
    }
}
