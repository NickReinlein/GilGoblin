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
    private ILogger<DatabaseLoader> _logger;

    private DatabaseLoader _databaseLoader;

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTableItemInfoIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseLoader.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.ItemInfo.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTablePriceIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseLoader.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.Price.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenTableRecipeIsEmpty_ThenWeFillTheTable()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseLoader.FillTablesIfEmpty(context);

        await using var context2 = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context2.Recipe.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GiveWeHaveADatabaseInitializer_WhenWeGetGlobalParameters_ThenTheyAreSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                DatabaseLoader.ApiSpamPreventionDelayInMS,
                Is.GreaterThan(10)
            );
            Assert.That(DatabaseLoader.TestWorldId, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task GiveWeCallFillTablesIfEmpty_WhenAnExceptionIsThrown_ThenWeLogAnError()
    {
        _dbConnector.GetResourcesPath().Returns("garbage");
        _csvInteractor.LoadFile<ItemInfoPoco>("TestDatabase").Throws<Exception>();
        await using var context = new GilGoblinDbContext(_options, _configuration);

        await _databaseLoader.FillTablesIfEmpty(context);

        _logger.Received().LogError("Failed to load CSV file: Value cannot be null. (Parameter 'source')");
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
        _logger = Substitute.For<ILogger<DatabaseLoader>>();

        _databaseLoader = new DatabaseLoader(
            _dbConnector,
            _csvInteractor,
            _logger
        );
    }

    private static List<PriceWebPoco> GenerateListOf2ValidPocos()
    {
        return new List<PriceWebPoco?>
        {
            new()
            {
                ItemId = 456,
                WorldId = 23,
                LastUploadTime = 1677798249999,
                AveragePrice = 512,
                CurrentAveragePrice = 400,
            },
            new()
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
                new List<ItemInfoPoco>() { new() { Id = 123 }, new() { Id = 456 } }
            );
        _csvInteractor
            .LoadFile<RecipePoco>("TestDatabase")
            .Returns(
                new List<RecipePoco>()
                {
                    new() { Id = 123, TargetItemId = 33, ResultQuantity = 1 },
                    new() { Id = 456, TargetItemId = 44, ResultQuantity = 1 }
                }
            );
        _csvInteractor
            .LoadFile<PricePoco>("TestDatabase")
            .Returns(
                new List<PricePoco>()
                {
                    new()
                    {
                        ItemId = 456,
                        WorldId = 23,
                        LastUploadTime = 1677798249999,
                        AverageListingPrice = 512,
                        AverageSold = 400,
                    },
                    new()
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