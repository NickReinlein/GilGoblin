using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataPointTests
{
    private GilGoblinDbContext _dbContext;
    private PostgreSqlContainer _postgresContainer;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase("goblin_db")
                .WithUsername("goblin")
                .WithPassword("goblin_pw")
                .WithCleanUp(true)
                .WithPortBinding(5432, true)
                .Build();

            await _postgresContainer.StartAsync();

            var connectionString = _postgresContainer.GetConnectionString();

            var configKvps = new Dictionary<string, string?>
            {
                { "ConnectionStrings:GilGoblinDbContext", connectionString }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configKvps)
                .Build();

            var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(connectionString)
                .Options;

            _dbContext = new GilGoblinDbContext(options, configuration);
            await _dbContext.Database.EnsureCreatedAsync();
            var created = await _dbContext.Database.EnsureCreatedAsync();
            if (!created)
                throw new Exception("Failed to connect to database");

            Console.WriteLine("Database context created.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Test]
    public async Task GivenAverageSalePricePoco_IsValid_WhenSaving_ThenObjectIsSavedSuccessfully()
    {
        var averageSalePrice = new AverageSalePricePoco(100, true, 100, 200, 300);

        await _dbContext.AverageSalePrice.AddAsync(averageSalePrice);
        var savedCount = await _dbContext.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));

        var result = await _dbContext.AverageSalePrice.FirstOrDefaultAsync(x => x.ItemId == 22);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(averageSalePrice.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(averageSalePrice.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(averageSalePrice.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(averageSalePrice.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(averageSalePrice.RegionDataPointId));
        });
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _postgresContainer.StopAsync();
    }
}