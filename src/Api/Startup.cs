using GilGoblin.Cache;
using GilGoblin.Controllers;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Services;
using GilGoblin.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using GilGoblin.DataUpdater;

namespace GilGoblin.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var add = AddGoblinServices(services);
        add.Wait();
    }

    public void Configure(IApplicationBuilder app)
    {
        AddAppGoblinServices(app);
    }

    public static async Task AddGoblinServices(IServiceCollection services)
    {
        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddGoblinUpdaterServices(services);
        AddBasicBuilderServices(services);
        AddGoblinCaches(services);
        await FillGoblinCaches(services);
    }

    private static void AddGoblinUpdaterServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ISingleDataFetcher<ItemInfoWebPoco>, ItemInfoSingleFetcher>();
        services.AddSingleton<IItemInfoSingleFetcher, ItemInfoSingleFetcher>();

        services.AddSingleton<IBulkDataFetcher<PriceWebPoco>, PriceBulkFetcher>();
        services.AddSingleton<IPriceBulkDataFetcher, PriceBulkFetcher>();

        services.AddSingleton<IItemInfoSingleFetcher, ItemInfoSingleFetcher>();
        services.AddSingleton<IMarketableItemIdsFetcher, MarketableItemIdsFetcher>();

        // services.AddSingleton<DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>, ItemInfoUpdater>();

        services.AddSingleton<IDataSaver<ItemInfoWebPoco>, DataSaver<ItemInfoWebPoco>>();
    }

    public static void AddGoblinCaches(IServiceCollection services)
    {
        services.AddSingleton<IItemInfoCache, ItemInfoCache>();
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

    public static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddSingleton<ICraftingCalculator, CraftingCalculator>();
        services.AddSingleton<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
    }

    public static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>(ServiceLifetime.Singleton);
        services.AddSingleton<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IRecipeRepository, RecipeRepository>();
        services.AddSingleton<IRecipeCostRepository, RecipeCostRepository>();
        services.AddSingleton<ISqlLiteDatabaseConnector, GilGoblinDatabaseConnector>();
        services.AddSingleton<ICsvInteractor, CsvInteractor>();
    }

    public static void AddBasicBuilderServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddLogging();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerApi", Version = "v1" });
        });
    }

    public static void AddAppGoblinServices(IApplicationBuilder app)
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

    public static async Task FillGoblinCaches(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var dbContextService = serviceProvider.GetRequiredService<GilGoblinDbContext>();
        if (await dbContextService.Database.CanConnectAsync() != true)
            return;

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
}