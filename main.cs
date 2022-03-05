using GilGoblin.Database;
using GilGoblin.Tests;
using Serilog;

class main
{
    static void Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Information() // PROD: reduce verbosity
            .WriteTo.Console()
            .WriteTo.File("logs/test.txt")
            .CreateLogger();

        Log.Information("Application started.");
        DatabaseAccess.Startup();
    }

}
