using GilGoblin.Database;
using GilGoblin.Tests;
using Serilog;
using System;

class main
{
    static void Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose() // PROD: reduce verbosity
            .WriteTo.Console()
            .WriteTo.File("logs/test.txt")
            .CreateLogger();

        Log.Information("Application started.");
        DatabaseAccess.Startup();

        Log.Information("Application finished.");
        Console.ReadLine();

    }

}
