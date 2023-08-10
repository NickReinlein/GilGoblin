using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Services;
using GilGoblin.Web;
using GilGoblin.Extensions;
using Serilog;
using System.Data.Entity;
using GilGoblin.Database;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseInitializerTests : InMemoryTestDb
{
    private ISqlLiteDatabaseConnector _dbConnector;
    private ICsvInteractor _csvInteractor;
    private GilGoblinDatabaseInitializer _databaseInitializer;

    [SetUp]
    public void SetUp()
    {
        _dbConnector = Substitute.For<ISqlLiteDatabaseConnector>();
        _csvInteractor = Substitute.For<ICsvInteractor>();
        _databaseInitializer = new GilGoblinDatabaseInitializer(_dbConnector, _csvInteractor);
    }

    [Test]
    public void GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForItems_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("test");
        _csvInteractor
            .LoadFile<ItemInfoPoco>("test")
            .Returns(new List<ItemInfoPoco>() { new ItemInfoPoco(), new ItemInfoPoco() });
        using var context = new GilGoblinDbContext(_options, _configuration);

        var result = _databaseInitializer.FillTablesIfEmpty(context);

        using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.ItemInfo.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForPrices_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("test");
        _csvInteractor
            .LoadFile<PricePoco>("test")
            .Returns(new List<PricePoco>() { new PricePoco(), new PricePoco() });
        using var context = new GilGoblinDbContext(_options, _configuration);

        var result = _databaseInitializer.FillTablesIfEmpty(context);

        using var context2 = new GilGoblinDbContext(_options, _configuration);
        // Assert.That(context2.Price.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForRecipes_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("test");
        _csvInteractor
            .LoadFile<RecipePoco>("test")
            .Returns(new List<RecipePoco>() { new RecipePoco(), new RecipePoco() });
        using var context = new GilGoblinDbContext(_options, _configuration);

        var result = _databaseInitializer.FillTablesIfEmpty(context);

        using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.Recipe.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GiveWeHaveADatabaseInitializer_WhenWeGetGlobalParameters_ThenTheyAreSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                GilGoblinDatabaseInitializer.ApiSpamPreventionDelayInMS,
                Is.GreaterThan(10)
            );
            Assert.That(GilGoblinDatabaseInitializer.TestWorldID, Is.GreaterThan(0));
        });
    }
}
