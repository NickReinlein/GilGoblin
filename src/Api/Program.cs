using GilGoblin.Api.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddGoblinDatabases();
        builder.Services.AddGoblinServices();

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
    }
}
