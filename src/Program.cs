using Serilog;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var app = WebApplication.CreateBuilder(args).Build();
            app.MapGet("/", () => "Hello World!");
            // app.UseHttpsRedirection();
            // app.UseAuthorization();
            // app.MapControllers();
            app.Run("http://localhost:3000");
            Console.WriteLine("Goodbye, cruel world!");
        }
    }
}