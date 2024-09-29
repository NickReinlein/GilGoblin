using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.PostgreSql;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataPointTests
{
    private GilGoblinDbContext? _dbContext;
    private PostgreSqlContainer? _postgresContainer;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            _postgresContainer = new ContainerBuilder<PostgreSqlContainer>()
                .ConfigureDatabaseConfiguration( "goblin", "goblin", "goblin_db")
                .Build();
            await _postgresContainer.StartAsync();

            var configKvps = new Dictionary<string, string?>
            {
                { "ConnectionStrings:GilGoblinDbContext", _postgresContainer.GetConnectionString() }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configKvps)
                .Build();
            var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                .UseNpgsql(configuration.GetConnectionString(nameof(GilGoblinDbContext)))
                .Options;

            _dbContext = new GilGoblinDbContext(options, configuration);
            await _dbContext.Database.EnsureCreatedAsync();
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

        await _dbContext!.AverageSalePrice.AddAsync(averageSalePrice);
        await _dbContext.SaveChangesAsync();

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
        _dbContext?.Database?.EnsureDeleted();
        _dbContext?.Dispose();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_postgresContainer is not null)
            await _postgresContainer.StopAsync();
    }
}