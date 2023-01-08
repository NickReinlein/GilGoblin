using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

//builder.WebHost.UseStartup<Startup>();
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
