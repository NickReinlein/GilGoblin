using GilGoblin.Pocos;
using GilGoblin.Services;
using GilGoblin.Database;
using NSubstitute;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseInitializerTests : InMemoryTestDb
{
    private ISqlLiteDatabaseConnector _dbConnector;
    private ICsvInteractor _csvInteractor;
    private ILogger<GilGoblinDatabaseInitializer> _logger;

    private GilGoblinDatabaseInitializer _databaseInitializer;

    [SetUp]
    public void SetUp()
    {
        _dbConnector = Substitute.For<ISqlLiteDatabaseConnector>();
        _csvInteractor = Substitute.For<ICsvInteractor>();
        _logger = Substitute.For<ILogger<GilGoblinDatabaseInitializer>>();

        _databaseInitializer = new GilGoblinDatabaseInitializer(
            _dbConnector,
            _csvInteractor,
            _logger
        );
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForItems_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("TestDatabase");
        _csvInteractor
            .LoadFile<ItemInfoPoco>("TestDatabase")
            .Returns(
                new List<ItemInfoPoco>()
                {
                    new ItemInfoPoco { ID = 123 },
                    new ItemInfoPoco { ID = 456 }
                }
            );
        using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.ItemInfo.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForPrices_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("TestDatabase");
        _csvInteractor
            .LoadFile<PricePoco>("TestDatabase")
            .Returns(
                new List<PricePoco>()
                {
                    new PricePoco(),
                    new PricePoco()
                    // {
                    //     ItemID = 123,
                    //     WorldID = 23,
                    //     LastUploadTime = 1677798249750,
                    //     AverageListingPrice = 256,
                    //     AverageSold = 200,

                    // }
                }
            );
        using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.Price.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenNoEntriesExistForRecipes_ThenWeFillTheTable()
    {
        _dbConnector.GetDatabasePath().Returns("TestDatabase");
        _csvInteractor
            .LoadFile<RecipePoco>("TestDatabase")
            .Returns(
                new List<RecipePoco>()
                {
                    new RecipePoco
                    {
                        ID = 123,
                        TargetItemID = 33,
                        ResultQuantity = 1
                    },
                    new RecipePoco
                    {
                        ID = 456,
                        TargetItemID = 44,
                        ResultQuantity = 1
                    }
                }
            );
        using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

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

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenAnExceptionIsThrown_ThenWeLogAnError()
    {
        _dbConnector.GetDatabasePath().Returns("TestDatabase");
        _csvInteractor.LoadFile<ItemInfoPoco>("TestDatabase").Throws<Exception>();
        using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        _logger.Received(1).LogError("Exception of type 'System.Exception' was thrown.");
    }
}
