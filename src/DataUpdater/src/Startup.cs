using System;
using GilGoblin.Database;
using GilGoblin.Fetcher;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.DataUpdater;

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
        AddGoblinUpdaterServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        DatabaseValidation(app);
    }

    private void AddGoblinUpdaterServices(IServiceCollection services)
    {
        services.AddHttpClient();

        Api.Startup.AddGoblinCrafting(services);
        Api.Startup.AddGoblinDatabases(services);
        Api.Startup.AddGoblinCaches(services);

        services.AddScoped<IMarketableItemIdsFetcher, MarketableItemIdsFetcher>();
        services.AddScoped<IItemFetcher, ItemFetcher>();
        services.AddScoped<ISingleDataFetcher<ItemWebPoco>, ItemFetcher>();
        services.AddScoped<IPriceFetcher, PriceFetcher>();
        services.AddScoped<IBulkDataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>();

        services.AddScoped<IDataSaver<ItemPoco>, DataSaver<ItemPoco>>();
        services.AddScoped<IPriceSaver, PriceSaver>();

        services.AddScoped<IDataUpdater<ItemPoco, ItemWebPoco>, ItemUpdater>();
        services.AddScoped<IDataUpdater<PricePoco, PriceWebPoco>, PriceUpdater>();

        services.AddHostedService<PriceUpdater>();
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