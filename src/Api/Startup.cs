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

namespace GilGoblin.Api;

public class Startup
{
    public IConfiguration _configuration { get; }
    public IWebHostEnvironment _environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AddGoblinServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        AddAppGoblinServices(app);
    }

    public static void AddGoblinServices(IServiceCollection services)
    {
        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddGoblinCaches(services);
        AddBasicBuilderServices(services);
        FillGoblinCaches(services);
    }

    public static void AddGoblinCaches(IServiceCollection services)
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

    public static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddSingleton<ICraftingCalculator, CraftingCalculator>();
        services.AddSingleton<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
        services.AddSingleton<DataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>();
        services.AddSingleton<IPriceDataFetcher, PriceFetcher>();
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

    public static async void FillGoblinCaches(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var dbContextService = serviceProvider.GetRequiredService<GilGoblinDbContext>();
        if (dbContextService.Database?.CanConnect() != true)
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
