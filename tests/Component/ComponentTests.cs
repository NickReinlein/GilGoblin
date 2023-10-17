using System.Text.Json;
using GilGoblin.Api;
using GilGoblin.Database;
using GilGoblin.Tests.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ComponentTests
{
    protected IServiceCollection _services;
    protected HttpClient _client;
    private WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var relativePath = "../../../../resources/GilGoblin.db";
                var fullPath = Path.Combine(baseDirectory, relativePath);

                var absolutePath = Path.GetFullPath(fullPath);
                Console.WriteLine("Absolute Path: " + absolutePath);

                var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                    .UseSqlite($"Data Source={absolutePath}")
                    .Options;

                services.AddSingleton(_ => new GilGoblinDbContext(options));
                _services = services;
            });
        });
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    protected static readonly double MissingEntryPercentageThreshold = 0.85;

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
}