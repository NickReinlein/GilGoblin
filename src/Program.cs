using Microsoft.EntityFrameworkCore;

namespace GilGoblin;

var webApplicationOptions = new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
};
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/", () => "Hello World!");

app.Run();