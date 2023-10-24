using System;
using GilGoblin.Cache;
using GilGoblin.Controllers;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Fetcher;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.DataUpdater;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GilGoblin.Api;

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
        AddGoblinServices(services);
        FillGoblinCaches(services).Wait();
    }

    public void Configure(IApplicationBuilder app)
    {
        AddAppGoblinServices(app);
        DatabaseValidation(app);
    }


    private static void AddGoblinServices(IServiceCollection services)
    {
        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddGoblinUpdaterServices(services);
        AddBasicBuilderServices(services);
        AddGoblinCaches(services);
    }

    private static void AddGoblinUpdaterServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IMarketableItemIdsFetcher, MarketableItemIdsFetcher>();

        services.AddSingleton<IItemSingleFetcher, ItemSingleFetcher>();
        services.AddSingleton<IDataUpdater<ItemWebPoco>, ItemUpdater>();
        services.AddSingleton<IDataSaver<ItemWebPoco>, DataSaver<ItemWebPoco>>();

        services.AddSingleton<IPriceBulkDataFetcher, PriceBulkFetcher>();
    }

    private static void AddGoblinCaches(IServiceCollection services)
    {
        services.AddSingleton<IItemCache, ItemCache>();
        services.AddSingleton<IPriceCache, PriceCache>();
        services.AddSingleton<IRecipeCache, RecipeCache>();
        services.AddSingleton<IItemRecipeCache, ItemRecipeCache>();
        services.AddSingleton<ICraftCache, CraftCache>();
        services.AddSingleton<IRecipeCostCache, RecipeCostCache>();

        services.AddSingleton<IRepositoryCache, ItemRepository>();
        services.AddSingleton<IRepositoryCache, PriceRepository>();
        services.AddSingleton<IRepositoryCache, RecipeRepository>();
    }

    private static void AddGoblinControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ItemController).Assembly)
            .AddApplicationPart(typeof(CraftController).Assembly)
            .AddApplicationPart(typeof(PriceController).Assembly)
            .AddApplicationPart(typeof(RecipeController).Assembly);
    }

    private static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddSingleton<ICraftingCalculator, CraftingCalculator>();
        services.AddSingleton<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
    }

    private static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>(ServiceLifetime.Singleton);
        services.AddSingleton<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IRecipeRepository, RecipeRepository>();
        services.AddSingleton<IRecipeCostRepository, RecipeCostRepository>();
    }

    private static void AddBasicBuilderServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddLogging();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerApi", Version = "v1" });
        });
    }

    private static void AddAppGoblinServices(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger();
        });
    }

    private void DatabaseValidation(IApplicationBuilder app)
    {
        try
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            ValidateCanConnectToDatabase(serviceScope);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to validate database during startup: {e.Message}");
        }
    }

    private static void ValidateCanConnectToDatabase(IServiceScope serviceScope)
    {
        var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }

    private static async Task FillGoblinCaches(IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();

            var dbContextService = serviceProvider.GetRequiredService<GilGoblinDbContext>();
            if (await dbContextService.Database.CanConnectAsync() != true)
                throw new Exception("Failed to connect to the database");

            var itemRepository = serviceProvider.GetRequiredService<IItemRepository>();
            var priceRepository = serviceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var recipeRepository = serviceProvider.GetRequiredService<IRecipeRepository>();
            var recipeCostRepository = serviceProvider.GetRequiredService<IRecipeCostRepository>();

            var itemTask = itemRepository.FillCache();
            var priceTask = priceRepository.FillCache();
            var recipeTask = recipeRepository.FillCache();
            var recipeCostTask = recipeCostRepository.FillCache();
            await Task.WhenAll(itemTask, priceTask, recipeTask, recipeCostTask);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to fill caches during startup: {e.Message}");
        }
    }
}