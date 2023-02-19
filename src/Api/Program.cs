using GilGoblin.Api.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace GilGoblin.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = SetupBuilder(args);
        var app = builder.Build();
        app.Run();
    }

    public static WebApplicationBuilder SetupBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseDefaultServiceProvider(
            (_, options) =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            }
        );
        builder.Services.AddGoblinServices();
        return builder;
    }
}
