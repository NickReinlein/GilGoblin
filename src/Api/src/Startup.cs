using System;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Middleware;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace GilGoblin.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment env)
{
    public readonly IConfiguration Configuration = configuration;

    public const bool isDebug = false;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
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
        AddAppServices(builder);
        DatabaseValidation(builder);
    }

    private void AddGoblinServices(IServiceCollection services)
    {
        services = AddGoblinDatabases(services, Configuration);
        services = AddGoblinCrafting(services);
        services = AddGoblinControllers(services);
        services = AddBasicBuilderServices(services);
        _ = AddGoblinCaches(services);
    }

    public static IServiceCollection AddGoblinCaches(IServiceCollection services)
    {
        return services.AddScoped<IItemCache, ItemCache>()
            .AddScoped<IPriceCache, PriceCache>()
            .AddScoped<IRecipeCache, RecipeCache>()
            .AddScoped<IWorldCache, WorldCache>()
            .AddScoped<IItemRecipeCache, ItemRecipeCache>()
            .AddScoped<ICalculatedMetricCache<RecipeCostPoco>, RecipeCostCache>()
            .AddScoped<ICalculatedMetricCache<RecipeProfitPoco>, RecipeProfitCache>()
            .AddScoped<ICraftCache, CraftCache>()
            .AddScoped<IRepositoryCache, ItemRepository>()
            .AddScoped<IRepositoryCache, PriceRepository>()
            .AddScoped<IRepositoryCache, RecipeRepository>();
    }

    private static IServiceCollection AddGoblinControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ItemController).Assembly)
            .AddApplicationPart(typeof(CraftController).Assembly)
            .AddApplicationPart(typeof(PriceController).Assembly)
            .AddApplicationPart(typeof(RecipeController).Assembly);
        return services;
    }

    public static IServiceCollection AddGoblinCrafting(IServiceCollection services)
    {
        return services.AddScoped<ICraftingCalculator, CraftingCalculator>()
            .AddScoped<ICraftRepository, CraftRepository>()
            .AddScoped<IRecipeGrocer, RecipeGrocer>();
    }

    public static IServiceCollection AddGoblinDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GilGoblinDbContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Failed to get connection string");

        services.AddDbContext<GilGoblinDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .EnableDetailedErrors(isDebug)
                .EnableSensitiveDataLogging(isDebug)
                // ReSharper disable once HeuristicUnreachableCode
                // Flag is changed to access "debugging mode"
                .LogTo(Console.WriteLine, isDebug ? LogLevel.Information : LogLevel.Warning);
        });

        return services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>()
            .AddScoped<IItemRepository, ItemRepository>()
            .AddScoped<IRecipeRepository, RecipeRepository>()
            .AddScoped<IRecipeCostRepository, RecipeCostRepository>()
            .AddScoped<IRecipeProfitRepository, RecipeProfitRepository>()
            .AddScoped<IWorldRepository, WorldRepository>();
    }

    private IServiceCollection AddBasicBuilderServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer()
            .AddLogging()
            .AddHealthChecks();

        if (IsTestingEnvironment() || isDebug)
            return services;

        return services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerApi", Version = "v1" });
        });
    }

    private void AddAppServices(IApplicationBuilder builder)
    {
        if (!IsTestingEnvironment() && !isDebug)
            builder.UseSwagger()
                .UseSwaggerUI();

        builder.UseRouting()
            .UseCors("GilGoblin")
            .UseAuthorization()
            .UseMiddleware<RequestInfoMiddleware>()
            .UseHttpMetrics()
            .UseMetricServer()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health");
            });
    }

    private static void DatabaseValidation(IApplicationBuilder builder)
    {
        try
        {
            var serviceScopeFactory = builder.ApplicationServices.GetService<IServiceScopeFactory>();
            if (serviceScopeFactory == null)
                throw new Exception("Failed to create service scope for database validation");

            using var serviceScope = serviceScopeFactory.CreateScope();
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
        if (dbContext.Database.CanConnect() != true)
            throw new Exception("Failed to connect to the database");
    }

    private static async Task FillGoblinCaches(IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();

            await using var dbContext = serviceProvider.GetRequiredService<GilGoblinDbContext>();
            if (await dbContext.Database.CanConnectAsync() != true)
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

    public bool IsTestingEnvironment()
    {
        return env.IsEnvironment("Testing");
    }
}