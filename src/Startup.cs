// using GilGoblin.Crafting;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;

// namespace GilGoblin;

// public class Startup
// {
//     private IConfiguration Configuration { get; }
//     private IWebHostEnvironment Environment { get; }

//     public Startup(IConfiguration configuration, IWebHostEnvironment environment)
//     {
//         Configuration = configuration;
//         Environment = environment;
//     }

//     public void ConfigureServices(IServiceCollection services)
//     {
//         services.AddSingleton(Configuration).AddHttpClient();
//         AddGoblinServices(services);
//     }

//     public void Configure(WebApplication app, IWebHostEnvironment env)
//     {
//         app.UseHttpsRedirection()
//             .UseRouting()
//             .UseAuthentication()
//             .UseAuthorization()
//             .UseEndpoints(endpoints => endpoints.MapControllers());
//     }

//     public void AddGoblinServices(IServiceCollection services)
//     {
//         services.AddScoped<IPriceRepository, PriceRepository>();
//         services.AddScoped<IRecipeRepository, RecipeRepository>();
//         services.AddScoped<IItemRepository, ItemRepository>();
//         services.AddScoped<IRecipeGrocer, RecipeGrocer>();
//         services.AddScoped<ICraftingCalculator, CraftingCalculator>();
//         services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
//     }
// }
