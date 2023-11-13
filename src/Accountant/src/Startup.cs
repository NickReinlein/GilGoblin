using System;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Accountant;

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
        AddGoblinAccountingServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        DatabaseValidation(app);
    }

    private void AddGoblinAccountingServices(IServiceCollection services)
    {
        // services.AddDbContext<GilGoblinDbContext>();
        //
        // services.AddScoped<IPriceRepository<PricePoco>, PriceRepository>();
        // services.AddScoped<IItemRepository, ItemRepository>();
        // services.AddScoped<IRecipeRepository, RecipeRepository>();
        // services.AddScoped<IRecipeCostRepository, RecipeCostRepository>();
        // // services.AddScoped<IRecipeProfitRepository, RecipeProfitRepository>();
        //
        // services.AddScoped<IDataSaver<RecipeCostPoco>, DataSaver<RecipeCostPoco>>();
        // services.AddScoped<IDataSaver<RecipeProfitPoco>, DataSaver<RecipeProfitPoco>>();
        //
        // services.AddScoped<IAccountant<RecipeCostPoco>, RecipeCostAccountant>();
        // // services.AddScoped<IAccountant<RecipeProfitPoco>, RecipeProfitAccountant>();
        //
        // services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        // // services.AddScoped<IRecipeGrocer, RecipeGrocer>();
        Api.Startup        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddBasicBuilderServices(services);
        AddGoblinCaches(services);

        services.AddHostedService<RecipeCostAccountant>();
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

    private void ValidateCanConnectToDatabase(IServiceScope serviceScope)
    {
        var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }
}