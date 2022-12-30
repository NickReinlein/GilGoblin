using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPriceRepository, PriceRepository>();
    builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
    builder.Services.AddScoped<IItemRepository, ItemRepository>();
    builder.Services.AddScoped<IRecipeGrocer, RecipeGrocer>();
    builder.Services.AddScoped<ICraftingCalculator, CraftingCalculator>();
    builder.Services.AddScoped<ICraftRepository<CraftSummaryPoco>, CraftRepository>();
}
