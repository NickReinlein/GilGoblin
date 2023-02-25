using GilGoblin.Api.DI;

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
