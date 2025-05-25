using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GilGoblin.Api;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Tests.Component;

[Category("Component")]
public class TestBase
{
    private const string databaseName = "gilgoblin_db";
    private const string username = "gilgoblin";
    private const string password = "gilgoblin_pw";
    private const string postgresImage = "postgres:15";

    protected HttpClient _client;
    private IWebHost _host;
    private PostgreSqlContainer _postgresContainer;
    private GilGoblinDbContext _dbContext;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage(postgresImage)
            .WithDatabase(databaseName)
            .WithUsername(username)
            .WithPassword(password)
            .WithPortBinding(5432, true)
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        var pgHost = _postgresContainer.Hostname;
        var pgPort = _postgresContainer.GetMappedPublicPort(5432);
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__GilGoblinDbContext",
            $"Host={pgHost};Port={pgPort};Database={databaseName};Username={username};Password={password}"
        );

        var webHostBuilder = WebHost.CreateDefaultBuilder()
            .UseEnvironment("Testing")
            .UseStartup<Startup>()
            .UseTestServer();

        _host = webHostBuilder.Build();
        await _host.StartAsync();
        _client = _host.GetTestClient();

        using var scope = _host.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        await _dbContext.Database.EnsureCreatedAsync();
        await AddTestDataAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_client is not null)
            _client.Dispose();

        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        if (_postgresContainer is not null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }

        await _dbContext.DisposeAsync();
    }

    private async Task AddTestDataAsync()
    {
        await _dbContext.Item.AddAsync(new ItemPoco
        {
            Id = 10348,
            Name = "Test Item",
            Description =
                "Black-and-white floorboards and carpeting of the same design as those used to furnish the Manderville Gold Saucer.",
            IconId = 51024,
            CanHq = false,
            Level = 1,
            PriceLow = 20,
            PriceMid = 35,
            StackSize = 1
        });

        await _dbContext.SaveChangesAsync();
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };
}