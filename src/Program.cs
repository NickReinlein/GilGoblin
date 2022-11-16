using Serilog;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;

namespace GilGoblin
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var builder = Bootstrap.CreateHostBuilder(args).Build();

            Console.WriteLine("Hello world!");

            builder.Run();

            Console.WriteLine("Goodbye, cruel world!");
        }
    }
}