using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Middleware;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace GilGoblin.Api;

public class Startup(IConfiguration configuration)
{
    public readonly IConfiguration Configuration = configuration;

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
        services.AddScoped<IItemCache, ItemCache>();
        services.AddScoped<IPriceCache, PriceCache>();
        services.AddScoped<IRecipeCache, RecipeCache>();
        services.AddScoped<IWorldCache, WorldCache>();
        services.AddScoped<IItemRecipeCache, ItemRecipeCache>();
        // services.AddScoped<IRecipeCostCache, RecipeCostCache>();
        // services.AddScoped<IRecipeProfitCache, RecipeProfitCache>();
        // services.AddScoped<ICraftCache, CraftCache>();
        services.AddScoped<IRepositoryCache, ItemRepository>();
        // services.AddScoped<IRepositoryCache, PriceRepository>();
        services.AddScoped<IRepositoryCache, RecipeRepository>();
        return services;
    }

    private static IServiceCollection AddGoblinControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ItemController).Assembly)
            // .AddApplicationPart(typeof(CraftController).Assembly)
            .AddApplicationPart(typeof(PriceController).Assembly)
            .AddApplicationPart(typeof(RecipeController).Assembly);
        return services;
    }

    public static IServiceCollection AddGoblinCrafting(IServiceCollection services)
    {
        // services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        // services.AddScoped<ICraftRepository, CraftRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        return services;
    }

    public static IServiceCollection AddGoblinDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GilGoblinDbContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Failed to get connection string");
        
        connectionString += "Include Error Detail=true;";

        services.AddDbContext<GilGoblinDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        // services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        // services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
        services.AddScoped<IWorldRepository, WorldRepository>();
        return services;
    }

    private static IServiceCollection AddBasicBuilderServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddLogging();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerApi", Version = "v1" });
        });
        return services;
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
        var canConnect = dbContext.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }

    private static async Task FillGoblinCaches(IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();

            await using var dbContextService = serviceProvider.GetRequiredService<GilGoblinDbContext>();
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