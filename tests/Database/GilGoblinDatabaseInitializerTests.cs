using GilGoblin.Pocos;
using GilGoblin.Services;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using NSubstitute;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseInitializerTests : InMemoryTestDb
{
    private ICsvInteractor _csvInteractor;
    private ISqlLiteDatabaseConnector _dbConnector;
    private ILogger<GilGoblinDatabaseInitializer> _logger;

    private GilGoblinDatabaseInitializer _databaseInitializer;

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTableItemInfoIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.ItemInfo.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTablePriceIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.Price.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTableRecipeIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
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
            Assert.That(GilGoblinDatabaseInitializer.TestWorldId, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenAnExceptionIsThrown_ThenWeLogAnError()
    {
        _csvInteractor.LoadFile<ItemInfoPoco>("TestDatabase").Throws<Exception>();
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseInitializer.FillTablesIfEmpty(context);

        _logger.Received(1).LogError("Exception of type 'System.Exception' was thrown.");
    }

    // [Test]
    // public async Task GiveWeCallSaveBatchResult_WhenPricesAreAllValid_ThenWeConvertAndSaveTheBatch()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var initialCount = context.Price.Count();
    //     var batchToSave = GenerateListOf2ValidPocos();
    //
    //     await _databaseInitializer.SaveBatchResult(context, batchToSave);
    //
    //     await using var context2 = new GilGoblinDbContext(_options, _configuration);
    //     Assert.That(context2.Price.Count, Is.EqualTo(initialCount + batchToSave.Count));
    // }
    //
    // [Test]
    // public async Task GiveWeCallSaveBatchResult_WhenSomePricesAreValid_ThenWeConvertAndSaveTheValidEntries()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var initialCount = context.Price.Count();
    //     var batchToSave = new List<PriceWebPoco?>()
    //     {
    //         new()
    //         {
    //             ItemId = 456,
    //             WorldId = 23,
    //             LastUploadTime = 1677798249999,
    //             AveragePrice = 512,
    //             CurrentAveragePrice = 400,
    //         },
    //         null
    //     };
    //
    //     await _databaseInitializer.SaveBatchResult(context, batchToSave);
    //
    //     await using var context2 = new GilGoblinDbContext(_options, _configuration);
    //     Assert.That(context2.Price.Count, Is.EqualTo(initialCount + batchToSave.Count - 1));
    // }
    //
    // [Test]
    // public async Task GiveWeCallSaveBatchResult_WhenNoPricesAreValid_ThenDoNothing()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var initialCount = context.Price.Count();
    //
    //     await _databaseInitializer.SaveBatchResult(
    //         context,
    //         new List<PriceWebPoco?>() { null, null }
    //     );
    //
    //     await using var context2 = new GilGoblinDbContext(_options, _configuration);
    //     Assert.That(context2.Price.Count, Is.EqualTo(initialCount));
    // }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _dbConnector = Substitute.For<ISqlLiteDatabaseConnector>();
        _dbConnector.GetDatabasePath().Returns("TestDatabase");
        SetupCsvInteractor();
        _logger = Substitute.For<ILogger<GilGoblinDatabaseInitializer>>();

        _databaseInitializer = new GilGoblinDatabaseInitializer(
            _dbConnector,
            _csvInteractor,
            _logger
        );
    }

    private static List<PriceWebPoco> GenerateListOf2ValidPocos()
    {
        return new List<PriceWebPoco?>()
        {
            new PriceWebPoco
            {
                ItemId = 456,
                WorldId = 23,
                LastUploadTime = 1677798249999,
                AveragePrice = 512,
                CurrentAveragePrice = 400,
            },
            new PriceWebPoco
            {
                ItemId = 789,
                WorldId = 23,
                LastUploadTime = 1677798949999,
                AveragePrice = 1024,
                CurrentAveragePrice = 800,
            },
        };
    }

    private void SetupCsvInteractor()
    {
        _csvInteractor = Substitute.For<ICsvInteractor>();
        _csvInteractor
            .LoadFile<ItemInfoPoco>("TestDatabase")
            .Returns(
                new List<ItemInfoPoco>() { new ItemInfoPoco { Id = 123 }, new ItemInfoPoco { Id = 456 } }
            );
        _csvInteractor
            .LoadFile<RecipePoco>("TestDatabase")
            .Returns(
                new List<RecipePoco>()
                {
                    new RecipePoco { Id = 123, TargetItemId = 33, ResultQuantity = 1 },
                    new RecipePoco { Id = 456, TargetItemId = 44, ResultQuantity = 1 }
                }
            );
        _csvInteractor
            .LoadFile<PricePoco>("TestDatabase")
            .Returns(
                new List<PricePoco>()
                {
                    new PricePoco
                    {
                        ItemId = 456,
                        WorldId = 23,
                        LastUploadTime = 1677798249999,
                        AverageListingPrice = 512,
                        AverageSold = 400,
                    },
                    new PricePoco
                    {
                        ItemId = 123,
                        WorldId = 23,
                        LastUploadTime = 1677798249750,
                        AverageListingPrice = 256,
                        AverageSold = 200,
                    }
                }
            );
    }
}