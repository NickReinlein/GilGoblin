using System;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Fetcher;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.DataUpdater;

public class Startup
{
    public IConfiguration _configuration;
    public IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        AddGoblinDatabases(services);
        AddGoblinCrafting(services);
        AddGoblinCaches(services);
        AddGoblinUpdaterServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        DatabaseValidation(app);
    }

    private void AddGoblinUpdaterServices(IServiceCollection services)
    {
        services.AddHttpClient();

        var connectionString = _configuration.GetConnectionString("GilGoblinDbContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Failed to get connection string");

        services.AddScoped<IMarketableItemIdsFetcher, MarketableItemIdsFetcher>()
            .AddScoped<IPriceFetcher, PriceFetcher>()
            .AddScoped<IBulkDataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>()
            .AddScoped<IDataUpdater<PricePoco, PriceWebPoco>, PriceUpdater>()
            .AddScoped<IDataSaver<PricePoco>, PriceSaver>()
            .AddScoped<IWorldFetcher, WorldFetcher>()
            .AddScoped<IDataSaver<WorldPoco>, WorldSaver>()
            .AddScoped<IWorldUpdater, WorldUpdater>();

        services.AddHostedService<WorldUpdater>();
        services.AddHostedService<PriceUpdater>();
    }

    private static void AddGoblinCrafting(IServiceCollection services)
    {
        // services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        // services.AddScoped<ICraftRepository, CraftRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
    }

    public static void AddGoblinCaches(IServiceCollection services)
    {
        services.AddScoped<IItemCache, ItemCache>();
        services.AddScoped<IPriceCache, PriceCache>();
        services.AddScoped<IRecipeCache, RecipeCache>();
        services.AddScoped<IItemRecipeCache, ItemRecipeCache>();
        services.AddScoped<IWorldCache, WorldCache>();
        // services.AddScoped<ICraftCache, CraftCache>();
        // services.AddScoped<IRecipeCostCache, RecipeCostCache>();
        // services.AddScoped<IRecipeProfitCache, RecipeProfitCache>();

        services.AddScoped<IRepositoryCache, ItemRepository>();
        services.AddScoped<IRepositoryCache, PriceRepository>();
        services.AddScoped<IRepositoryCache, RecipeRepository>();
    }

    private void AddGoblinDatabases(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("GilGoblinDbContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Failed to get connection string");

        services.AddDbContext<GilGoblinDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        // services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        // services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
        services.AddScoped<IWorldRepository, WorldRepository>();
    }

    private void DatabaseValidation(IApplicationBuilder app)
    {
        try
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            if (serviceScope == null)
                throw new Exception("Failed to create service scope for database validation");
            ValidateCanConnectToDatabase(serviceScope);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to validate database during startup: {e.Message}");
        }
    }

    private void ValidateCanConnectToDatabase(IServiceScope serviceScope)
    {
        var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }
}