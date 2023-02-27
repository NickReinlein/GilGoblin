using GilGoblin.Api.DI;

namespace GilGoblin.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var app = args.GetGoblinBuilder().Build();
        app.AddAppGoblinServices();
        app.Run();
    }
}
