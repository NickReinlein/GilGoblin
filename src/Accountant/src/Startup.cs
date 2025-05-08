using System;
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
        services
            .AddLogging()
            .AddHealthChecks();

        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy(name: "GilGoblinAccountant",
                builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
        });

        AddGoblinAccountingServices(services);
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder
            .UseRouting()
            .UseCors("GilGoblinAccountant")
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        DatabaseValidation(builder);
    }

    private void AddGoblinAccountingServices(IServiceCollection services)
    {
        Api.Startup.AddGoblinCrafting(services);
        Api.Startup.AddGoblinDatabases(services, configuration);
        Api.Startup.AddGoblinCaches(services);

        services
            .AddSingleton<IDataSaver<RecipeCostPoco>, DataSaver<RecipeCostPoco>>()
            .AddSingleton<IDataSaver<RecipeProfitPoco>, DataSaver<RecipeProfitPoco>>()
            .AddSingleton<IPriceSaver, PriceSaver>()
            .AddSingleton<IAccountant, RecipeCostAccountant>()
            .AddSingleton<IAccountant, RecipeProfitAccountant>()
            .AddHostedService<RecipeCostAccountant>()
            .AddHostedService<RecipeProfitAccountant>();
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
        if (!dbContextService.Database.CanConnect())
            throw new Exception("Failed to connect to the database");
    }
}