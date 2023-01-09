using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AddGoblinServices(builder.Services);

builder.WebHost.UseDefaultServiceProvider(
    (_, options) =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    }
);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void AddGoblinServices(IServiceCollection services)
{
    services.AddScoped<IPriceRepository, PriceRepository>();
    services.AddScoped<IRecipeRepository, RecipeRepository>();
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<IRecipeGrocer, RecipeGrocer>();
    services.AddScoped<ICraftingCalculator, CraftingCalculator>();
    services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
}
