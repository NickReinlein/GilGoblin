using System;
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
        Api.Startup.AddGoblinCrafting(services);
        Api.Startup.AddGoblinDatabases(services, _configuration);
        Api.Startup.AddGoblinCaches(services);

        services.AddScoped<IAccountant<RecipeCostPoco>, RecipeCostAccountant>();
        services.AddScoped<IAccountant<RecipeProfitPoco>, RecipeProfitAccountant>();

        services.AddHostedService<RecipeCostAccountant>();
        services.AddHostedService<RecipeProfitAccountant>();
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
        var dbContextService = serviceScope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var canConnect = dbContextService.Database.CanConnect();
        if (canConnect != true)
            throw new Exception("Failed to connect to the database");
    }
}