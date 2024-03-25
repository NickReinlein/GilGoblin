using System;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Middleware;
using GilGoblin.Api.Pocos;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Prometheus;

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
        services.AddCors(options =>
        {
            options.AddPolicy(name: "GilGoblin",
                builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
        });
        AddGoblinServices(services);
        FillGoblinCaches(services).Wait();
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseCors("GilGoblin");
        AddAppGoblinServices(builder);
        DatabaseValidation(builder);
    }


    private static void AddGoblinServices(IServiceCollection services)
    {
        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddBasicBuilderServices(services);
        AddGoblinCaches(services);
    }

    public static void AddGoblinCaches(IServiceCollection services)
    {
        services.AddScoped<IItemCache, ItemCache>();
        services.AddScoped<IPriceCache, PriceCache>();
        services.AddScoped<IRecipeCache, RecipeCache>();
        services.AddScoped<IItemRecipeCache, ItemRecipeCache>();
        services.AddScoped<IWorldCache, WorldCache>();
        services.AddScoped<ICraftCache, CraftCache>();
        services.AddScoped<IRecipeCostCache, RecipeCostCache>();
        services.AddScoped<IRecipeProfitCache, RecipeProfitCache>();

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
    }

    public static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();

        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
        services.AddScoped<IWorldRepository, WorldRepository>();
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

    private static void AddAppGoblinServices(IApplicationBuilder builder)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI();
        builder.UseHttpMetrics();
        builder.UseMetricServer();
        builder.UseAuthorization();
        builder.UseMiddleware<RequestInfoMiddleware>();

        builder.UseRouting();
        builder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger();
            endpoints.MapMetrics();
        });
    }

    private void DatabaseValidation(IApplicationBuilder builder)
    {
        try
        {
            using var serviceScope = builder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            ValidateCanConnectToDatabase(dbContext);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to validate database during startup: {e.Message}");
        }
    }

    private static void ValidateCanConnectToDatabase(DbContext dbContext)
    {
        var canConnect = dbContext.Database.CanConnect();
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
            var recipeProfitRepository = serviceProvider.GetRequiredService<IRecipeProfitRepository>();
            var worldRepository = serviceProvider.GetRequiredService<IWorldRepository>();

            var itemTask = itemRepository.FillCache();
            var priceTask = priceRepository.FillCache();
            var recipeTask = recipeRepository.FillCache();
            var recipeCostTask = recipeCostRepository.FillCache();
            var recipeProfitTask = recipeProfitRepository.FillCache();
            var worldsTask = worldRepository.FillCache();
            await Task.WhenAll(itemTask, priceTask, recipeTask, recipeCostTask, recipeProfitTask, worldsTask);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to fill caches during startup: {e.Message}");
        }
    }
}