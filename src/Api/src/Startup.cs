using System;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
        AddBasicBuilderServices(services);
        AddGoblinCaches(services);
    }

    private static void AddGoblinCaches(IServiceCollection services)
    {
        services.AddScoped<IItemCache, ItemCache>();
        services.AddScoped<IPriceCache, PriceCache>();
        services.AddScoped<IRecipeCache, RecipeCache>();
        services.AddScoped<IItemRecipeCache, ItemRecipeCache>();
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

    private static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddScoped<IRecipeGrocer, RecipeGrocer>();
    }

    private static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
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