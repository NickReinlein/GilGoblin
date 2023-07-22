using System;
using GilGoblin.Api.DI;
using GilGoblin.Controllers;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GilGoblin.Api;

public class Startup
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AddGoblinServices(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        AddAppGoblinServices(app);
    }

    public static WebApplicationBuilder GetGoblinBuilder(string[]? args)
    {
        var builder = WebApplication.CreateBuilder(args ?? Array.Empty<string>());
        builder.WebHost.UseDefaultServiceProvider(
            (_, options) =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            }
        );
        return builder;
    }

    public static void AddGoblinServices(IServiceCollection services)
    {
        AddGoblinCrafting(services);
        AddGoblinDatabases(services);
        AddGoblinControllers(services);
        AddBasicBuilderServices(services);
    }

    private static void AddGoblinControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ItemController).Assembly)
            .AddApplicationPart(typeof(CraftController).Assembly)
            .AddApplicationPart(typeof(PriceController).Assembly);
    }

    public static void AddGoblinCrafting(IServiceCollection services)
    {
        services.AddScoped<ICraftingCalculator, CraftingCalculator>();
        services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
        services.AddSingleton<DataFetcher<PriceWebPoco, PriceWebResponse>, PriceFetcher>();
        services.AddSingleton<IPriceDataFetcher, PriceFetcher>();
        services.AddSingleton<IRecipeGrocer, RecipeGrocer>();
    }

    public static void AddGoblinDatabases(IServiceCollection services)
    {
        services.AddSingleton<GilGoblinDatabase>();
        services.AddDbContext<GilGoblinDbContext>();
        services.AddScoped<IPriceRepository<PricePoco>, PriceGateway>();
        services.AddSingleton<IItemRepository, ItemGateway>();
        services.AddSingleton<IRecipeRepository, RecipeGateway>();
        services.AddSingleton<ISqlLiteDatabaseConnector, GilGoblinDatabaseConnector>();
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
        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwagger();
        app.UseSwaggerUI();
        // }

        // app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger();
        });
    }
}
