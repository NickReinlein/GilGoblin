using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class CsvInteractorTests
{
    private CsvInteractor _interactor;
    private GilGoblinDatabaseConnector _connector;
    private ILogger<CsvInteractor> _logger;

    [SetUp]
    public void SetUp()
    {
        _connector = new GilGoblinDatabaseConnector(NullLogger<GilGoblinDatabaseConnector>.Instance);
        _logger = Substitute.For<ILogger<CsvInteractor>>();
        _interactor = new CsvInteractor(_logger);
    }

    [Test]
    public void GivenAFile_WhenWeFailToLoadTheFile_ThenWeLogAnErrorAndReturnAnEmptyArray()
    {
        var result = _interactor.LoadFile<ItemInfoPoco>(ResourceFilePath("itsAFake"));

        Assert.That(result, Is.Empty);
        _logger.ReceivedWithAnyArgs(1).LogError(default);
    }

    [Test]
    public void GivenAnItemTestFile_WhenWeLoadTheFile_ThenWeDeserializeItems()
    {
        var result = _interactor.LoadFile<ItemInfoPoco>(ResourceFilePath(itemTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        var gilItemEntry = result.First(i => i.Id == 1);
        var lightningShardItemEntry = result.First(i => i.Id == 6);
        Assert.Multiple(() =>
        {
            Assert.That(gilItemEntry.Name, Is.EqualTo("Gil"));
            Assert.That(gilItemEntry.IconId, Is.EqualTo(65002));
            Assert.That(gilItemEntry.StackSize, Is.GreaterThanOrEqualTo(9999999));
            Assert.That(lightningShardItemEntry.Name, Is.EqualTo("Lightning Shard"));
            Assert.That(lightningShardItemEntry.IconId, Is.EqualTo(20005));
            Assert.That(lightningShardItemEntry.StackSize, Is.EqualTo(9999));
        });
    }

    [Test]
    public void GivenARecipesTestFile_WhenWeLoadTheFileThenWeDeserializeRecipes()
    {
        var result = _interactor.LoadFile<RecipePoco>(ResourceFilePath(recipeTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        var recipeSix = result.First(i => i.Id == 6);
        Assert.Multiple(() =>
        {
            Assert.That(recipeSix.Id, Is.EqualTo(6));
            Assert.That(recipeSix.TargetItemId, Is.EqualTo(1750));
            Assert.That(recipeSix.ResultQuantity, Is.EqualTo(1));
            Assert.That(recipeSix.ItemIngredient0TargetId, Is.EqualTo(5056));
            Assert.That(recipeSix.AmountIngredient0, Is.EqualTo(2));
            Assert.That(recipeSix.ItemIngredient1TargetId, Is.EqualTo(5361));
            Assert.That(recipeSix.AmountIngredient1, Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenAPricesTestFile_WhenWeLoadTheFile_ThenWeDeserializePrices()
    {
        var result = _interactor.LoadFile<PricePoco>(ResourceFilePath(priceTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        Assert.Multiple(() =>
        {
            var price = result.First(i => i.ItemId == 31100);
            Assert.That(price.ItemId, Is.EqualTo(31100));
            Assert.That(price.WorldId, Is.GreaterThan(0));
            Assert.That(price.AverageListingPrice, Is.GreaterThan(0));
            Assert.That(price.AverageSold, Is.GreaterThan(0));
            Assert.That(price.LastUploadTime, Is.GreaterThan(0));
        });
    }

    private string ResourceFilePath(string filename) =>
        Path.Combine(_connector.GetResourcesPath(), filename);

    private const string itemTestFileName = "ItemInfoTest.csv";
    private const string recipeTestFileName = "RecipeTest.csv";
    private const string priceTestFileName = "PriceTest.csv";
}