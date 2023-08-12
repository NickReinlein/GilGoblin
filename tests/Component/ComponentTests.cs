using GilGoblin.Api;
using GilGoblin.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ComponentTests
{
    protected WebApplicationFactory<Startup> _factory;
    protected HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                    .UseSqlite($"Data Source=../../../../resources/GilGoblin.db;")
                    .Options;

                services.AddSingleton(_ => new GilGoblinDbContext(options));
            });
        });

        _client = _factory.CreateClient();
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        _client.CancelPendingRequests();
        _client.Dispose();
        _factory.Dispose();
    }
}
