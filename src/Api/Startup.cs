using GilGoblin.Cache;
using GilGoblin.Controllers;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Services;
using GilGoblin.Web;
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
        services.AddScoped<IRepositoryCache, ItemRepository>();
        services.AddScoped<IRepositoryCache, PriceRepository>();
        services.AddScoped<IRepositoryCache, RecipeRepository>();
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
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        services.AddSingleton<DataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>();
        services.AddSingleton<IPriceDataFetcher, PriceFetcher>();
    }

    public static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        services.AddScoped<ISqlLiteDatabaseConnector, GilGoblinDatabaseConnector>();
        services.AddScoped<ICsvInteractor, CsvInteractor>();
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

    public static void FillGoblinCaches(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var itemRepository = serviceProvider.GetRequiredService<IItemRepository>();
        var priceRepository = serviceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
        var recipeRepository = serviceProvider.GetRequiredService<IRecipeRepository>();
        var recipeCostRepository = serviceProvider.GetRequiredService<IRecipeCostRepository>();
        var dbContextService = serviceProvider.GetRequiredService<GilGoblinDbContext>();
        if (dbContextService.Database?.CanConnect() == true)
        {
            itemRepository.FillCache();
            priceRepository.FillCache();
            recipeRepository.FillCache();
            recipeCostRepository.FillCache();
        }
    }
}
