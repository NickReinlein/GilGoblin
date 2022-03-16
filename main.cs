using GilGoblin.Database;
using GilGoblin.Tests;
using Serilog;
using System;

class main
{
    static void Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug() // PROD: reduce verbosity
            .WriteTo.Console()
            .WriteTo.File("logs/test.txt")
            .CreateLogger();

        Log.Information("Application started. Logging enabled.");
        DatabaseAccess.Startup();

        testCalcs.test_Top_Crafts();

        Log.Information("Application finished. Press any key to exit.");
        Console.ReadLine();

    }

}
