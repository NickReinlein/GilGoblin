using System;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Converters;
using GilGoblin.Fetcher;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public IWebHostEnvironment Environment { get; } = environment;
    public IConfiguration _configuration = configuration;

    public const bool isDebug = false;

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

        services
            .AddScoped<IMarketableItemIdsFetcher, MarketableItemIdsFetcher>()
            .AddScoped<IPriceFetcher, PriceFetcher>()
            .AddScoped<IBulkDataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>()
            .AddScoped<IPriceSaver, PriceSaver>()
            .AddScoped<IPriceConverter, PriceConverter>()
            .AddScoped<IPriceDataDetailConverter, PriceDataDetailConverter>()
            .AddScoped<IPriceDataPointConverter, PriceDataPointConverter>()
            .AddScoped<IQualityPriceDataConverter, QualityPriceDataConverter>()
            .AddScoped<IDailySaleVelocityConverter, DailySaleVelocityConverter>()
            .AddScoped<IWorldFetcher, WorldFetcher>()
            .AddScoped<IDataSaver<DailySaleVelocityPoco>, DailySaleVelocitySaver>()
            .AddScoped<IDataSaver<WorldPoco>, WorldSaver>();

        services
            .AddHostedService<WorldUpdater>()
            .AddHostedService<PriceUpdater>();
    }

    private static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository, CraftRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
    }

    public static void AddGoblinCaches(IServiceCollection services)
    {
        services
            .AddScoped<IItemCache, ItemCache>()
            .AddScoped<IPriceCache, PriceCache>()
            .AddScoped<IRecipeCache, RecipeCache>()
            .AddScoped<IItemRecipeCache, ItemRecipeCache>()
            .AddScoped<IWorldCache, WorldCache>()
            .AddScoped<ICraftCache, CraftCache>()
            .AddScoped<ICalculatedMetricCache<RecipeCostPoco>, RecipeCostCache>()
            .AddScoped<ICalculatedMetricCache<RecipeProfitPoco>, RecipeProfitCache>()
            .AddScoped<IRepositoryCache, ItemRepository>()
            .AddScoped<IRepositoryCache, PriceRepository>()
            .AddScoped<IRepositoryCache, RecipeRepository>();
    }

    private void AddGoblinDatabases(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("GilGoblinDbContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Failed to get connection string");

        services.AddDbContext<GilGoblinDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .EnableDetailedErrors(isDebug)
                .EnableSensitiveDataLogging(isDebug)
                .LogTo(Console.WriteLine, isDebug ? LogLevel.Information : LogLevel.Warning);
        });

        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>()
            .AddScoped<IItemRepository, ItemRepository>()
            .AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
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
        using var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }
}