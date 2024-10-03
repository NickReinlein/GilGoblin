using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace GilGoblin.Tests.Database.Integration;

public class GilGoblinDatabaseFixture
{
    protected PostgreSqlContainer _postgresContainer;
    protected IConfigurationRoot _configuration;
    protected DbContextOptions<GilGoblinDbContext> _options;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("goblin_db")
            .WithUsername("goblin")
            .WithPassword("goblin_pw")
            .WithPortBinding(0, 5432)
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        Console.WriteLine($"PostgreSQL started on: {_postgresContainer.GetConnectionString()}");

        var connectionString = GetConnectionString();
        var configKvps = new Dictionary<string, string?>
        {
            { "ConnectionStrings:GilGoblinDbContext", connectionString }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configKvps)
            .Build();

        _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseNpgsql(connectionString)
            .Options;

        await using var ctx = GetNewDbContext();
        var created = await ctx.Database.EnsureCreatedAsync();
        if (!created)
            throw new Exception("Failed to connect to database");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _postgresContainer.DisposeAsync();
    }

    protected GilGoblinDbContext GetNewDbContext() => new(_options, _configuration);

    protected string GetConnectionString() => _postgresContainer.GetConnectionString() ??
                                              throw new InvalidOperationException("Connection string not found.");
}