using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.Api.DI;

public static class DependencyInjectionExtensions
{
    // public static IServiceCollection AddGoblinServices(this IServiceCollection services)
    // {
    //     services.AddGoblinCrafting();
    //     services.AddGoblinDatabases();
    //     services.AddBasicBuilderServices();
    //     return services;
    // }

    // public static IServiceCollection AddGoblinCrafting(this IServiceCollection services)
    // {
    //     services.AddScoped<ICraftingCalculator, CraftingCalculator>();
    //     services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
    //     services.AddSingleton<IPriceDataFetcher, PriceFetcher>();
    //     services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
    //     return services;
    // }

    // public static IServiceCollection AddGoblinDatabases(this IServiceCollection services)
    // {
    //     services.AddDbContext<GilGoblinDbContext>();
    //     services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
    //     services.AddSingleton<GoblinDatabase>();
    //     services.AddSingleton<IItemRepository, ItemGateway>();
    //     services.AddSingleton<IRecipeRepository, RecipeGateway>();
    //     return services;
    // }

    // public static IServiceCollection AddBasicBuilderServices(this IServiceCollection services)
    // {
    //     services.AddControllers();
    //     services.AddEndpointsApiExplorer();
    //     services.AddSwaggerGen();
    //     return services;
    // }

    // public static WebApplication AddAppGoblinServices(this WebApplication app)
    // {
    //     // if (app.Environment.IsDevelopment())
    //     // {
    //     app.UseSwagger();
    //     app.UseSwaggerUI();
    //     // }
    //     // app.UseHttpsRedirection();
    //     app.UseAuthorization();
    //     app.UseRouting();
    //     app.UseEndpoints(endpoints =>
    //     {
    //         endpoints.MapControllers();
    //     });
    //     app.MapGet("/test", () => "Hello World!");
    //     return app;
    // }
}
