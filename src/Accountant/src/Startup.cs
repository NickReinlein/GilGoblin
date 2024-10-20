using System;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Accountant;

public class Startup(IConfiguration configuration)
{
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
        Api.Startup.AddGoblinCrafting(services);
        Api.Startup.AddGoblinDatabases(services, configuration);
        Api.Startup.AddGoblinCaches(services);

        services.AddSingleton<IDataSaver<RecipeCostPoco>, DataSaver<RecipeCostPoco>>();
        services.AddSingleton<IPriceSaver, PriceSaver>();

        services.AddSingleton<IAccountant<RecipeCostPoco>, RecipeCostAccountant>();
        // services.AddScoped<IAccountant<RecipeProfitPoco>, RecipeProfitAccountant>();

        services.AddHostedService<RecipeCostAccountant>();
        // services.AddHostedService<RecipeProfitAccountant>();

    }

    private static void DatabaseValidation(IApplicationBuilder app)
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

    private static void ValidateCanConnectToDatabase(IServiceScope serviceScope)
    {
        using var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }
}