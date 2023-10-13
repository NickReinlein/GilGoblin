using GilGoblin.Api;
using GilGoblin.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests;

public class TestWithDatabase
{
    protected IServiceCollection _services;
    protected HttpClient _client;
    private WebApplicationFactory<Api.Startup> _factory;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Api.Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var relativePath = "../../../../resources/GilGoblin.db";
                var fullPath = Path.Combine(baseDirectory, relativePath);

                var absolutePath = Path.GetFullPath(fullPath);
                Console.WriteLine("Absolute Path: " + absolutePath);

                var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                    .UseSqlite($"Data Source={fullPath}")
                    .Options;

                services.AddSingleton(_ => new GilGoblinDbContext(options));
                _services = services;
            });
        });
        _client = _factory.CreateClient();
    }

    [TearDown]
    public virtual void OneTimeTearDown()
    {
        _client?.CancelPendingRequests();
        _client?.Dispose();
        _factory?.Dispose();
    }
}