using GilGoblin.Api.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace GilGoblin.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = args.GetGoblinBuilder();
        var app = builder.Build();
        app.Run();
    }
}
